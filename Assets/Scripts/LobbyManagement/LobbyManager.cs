using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Game.LobbyManagement
{
    public class LobbyManager : MonoSingelton<LobbyManager>
    {
        [Min(10.0f)]
        [SerializeField]private float heartbeatInterval = 15.0f;
        [Min(0.5f)]
        [SerializeField]private float lobbyPollInterval = 1.1f;
        [Min(1.0f)]
        [SerializeField]private float refreshLobbyListInterval = 5.0f;
        [Min(2)]
        [SerializeField]private int maxPlayers = 2;

        public Action OnSignInSuccess;
        public Action OnSignInFailed;
        public Action OnLeftLobby;
        public Action<Lobby> OnJoinedLobby;
        public Action OnJoinLobbyFailed;
        public Action<Lobby> OnJoinedLobbyUpdate;
        public Action<Lobby> OnKickedFromLobby;
        public Action<List<Lobby>> OnLobbyListChanged;
        public Action<Lobby> OnLobbyStartGame;
        private Lobby joinedLobby = null;
        private string relayJoinCode = "";
        private string playerName = "";
        private string lobbyCode = "";
        private Coroutine lobbyHeartbeatCoroutine = null;
        private Coroutine lobbyPoolingCoroutine = null;
        private Coroutine refreshLobbyListCoroutine = null;

        public Lobby JoinedLobby { get => joinedLobby; }
        public string RelayJoinCode { get => relayJoinCode; }
        public string LobbyCode { get => lobbyCode; }

        public async void Authenticate(string playerName)
        {
            this.playerName = playerName;
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += ()=>
            {
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
                OnSignInSuccess?.Invoke();
                RefreshLobbyList();
            };

            AuthenticationService.Instance.SignInFailed += (e) =>
            {
                OnSignInFailed?.Invoke();
                Debug.LogError(e);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateLobby(string lobbyName, bool isPrivate) 
        {
            try
            {
                Allocation allocation = await AllocateRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                Player player = GetPlayer();

                CreateLobbyOptions options = new CreateLobbyOptions 
                {
                    Player = player,
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject> {
                        {
                            Constants.KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                        }
                    }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);

                joinedLobby = lobby;
                this.relayJoinCode = relayJoinCode;
                this.lobbyCode = lobby.LobbyCode;
                OnJoinedLobby?.Invoke(lobby);

                Debug.Log("Created Lobby: " + lobby.Name);

                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                unityTransport.SetRelayServerData(new RelayServerData(allocation, Constants.RELAY_PROTOCOL));

                StartHost();
            }
            catch(LobbyServiceException e)
            {
                OnJoinLobbyFailed?.Invoke();
                Debug.LogError(e);
            }
        }

        

        public async void RefreshLobbyList()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;

                options.Filters = new List<QueryFilter>{
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                    )
                };

                options.Order = new List<QueryOrder>{
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created
                    )
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

                OnLobbyListChanged?.Invoke(lobbyListQueryResponse.Results);
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async void JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                Player player = GetPlayer();

                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions{
                    Player = player
                });

                joinedLobby = lobby;

                OnJoinedLobby?.Invoke(lobby);

                string relayJoinCode = joinedLobby.Data[Constants.KEY_RELAY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                unityTransport.SetClientRelayData(
                                joinAllocation.RelayServer.IpV4,
                                (ushort)joinAllocation.RelayServer.Port,
                                joinAllocation.AllocationIdBytes,
                                joinAllocation.Key,
                                joinAllocation.ConnectionData,
                                joinAllocation.HostConnectionData
                                );

                StartClient();
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke();
            }
        }

        public async void JoinLobby(Lobby lobby)
        {
            try
            {
                Player player = GetPlayer();

                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
                                    Player = player
                                    });

                OnJoinedLobby?.Invoke(lobby);

                string relayJoinCode = joinedLobby.Data[Constants.KEY_RELAY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                unityTransport.SetClientRelayData(
                                joinAllocation.RelayServer.IpV4,
                                (ushort)joinAllocation.RelayServer.Port,
                                joinAllocation.AllocationIdBytes,
                                joinAllocation.Key,
                                joinAllocation.ConnectionData,
                                joinAllocation.HostConnectionData
                                );

                StartClient();
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke();
            }
        }

        public async void UpdatePlayerName(string playerName)
        {
            this.playerName = playerName;

            if(joinedLobby != null)
            {
                try
                {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();

                    options.Data = new Dictionary<string, PlayerDataObject>{
                        {
                            Constants.KEY_PLAYER_NAME,
                            new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: playerName)
                        }
                    };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;

                    OnJoinedLobbyUpdate?.Invoke(joinedLobby);

                }
                catch(LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public async void QuickJoinLobby()
        {
            try
            {
                //QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
                
                // options.Filter = new List<QueryFilter>()
                // {
                //     new QueryFilter(QueryFilter.FieldOptions.MaxPlayers, maxPlayers.ToString(), QueryFilter.OpOptions.EQ)
                // };

                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                joinedLobby = lobby;

                OnJoinedLobby?.Invoke(lobby);

                string relayJoinCode = lobby.Data[Constants.KEY_RELAY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                unityTransport.SetClientRelayData(
                                joinAllocation.RelayServer.IpV4,
                                (ushort)joinAllocation.RelayServer.Port,
                                joinAllocation.AllocationIdBytes,
                                joinAllocation.Key,
                                joinAllocation.ConnectionData,
                                joinAllocation.HostConnectionData
                                );

                StartClient();
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke();
            }
        }

        public async void LeaveLobby()
        {
            if(joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    joinedLobby = null;

                    OnLeftLobby?.Invoke();
                }
                catch(LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public bool IsLobbyLost()
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        public async void KickPlayer(string playerId) 
        {
            if (IsLobbyHost()) {
                try 
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                } 
                catch (LobbyServiceException e) 
                {
                    Debug.LogError(e);
                }
            }
        }

        public bool IsLobbyHost() 
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        protected override void InternalInit()
        {
            
        }

        protected override void InternalOnStart()
        {
            if(lobbyHeartbeatCoroutine == null)
            {
                lobbyHeartbeatCoroutine = StartCoroutine(HandleLobbyHeartbeat());
            }

            if(lobbyPoolingCoroutine == null)
            {
                lobbyPoolingCoroutine = StartCoroutine(HandleLobbyPooling());
            }
            if(refreshLobbyListCoroutine == null)
            {
                refreshLobbyListCoroutine = StartCoroutine(HandleRefreshLobbyList());
            }
        }

        protected override void InternalOnDestroy()
        {
            
        }

        private Player GetPlayer()
        {
            var data = new Dictionary<string, PlayerDataObject>{
                {Constants.KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName)}
            };
            return new Player(AuthenticationService.Instance.PlayerId, null, data);
        }

        private IEnumerator HandleLobbyHeartbeat()
        {
            while(Application.isPlaying)
            {
                if(IsLobbyLost())
                {
                    yield return new WaitForSeconds(heartbeatInterval);
                    var task = LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                    yield return new WaitUntil(()=> task.IsCompleted);
                    if(NetworkManager.Singleton.ConnectedClientsList.Count == maxPlayers)
                    {
                        OnLobbyStartGame?.Invoke(joinedLobby);
                    }
                }
                yield return null;
            }
        }

        private IEnumerator HandleLobbyPooling()
        {
            while(Application.isPlaying)
            {
                if(joinedLobby != null)
                {
                    yield return new WaitForSeconds(lobbyPollInterval);
                    
                    var task = LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    yield return new WaitUntil(()=> task.IsCompleted);
                    joinedLobby = task.Result;

                    OnJoinedLobbyUpdate?.Invoke(joinedLobby);

                    if(!IsPlayerInLobby())
                    {
                        Debug.Log("Kicked from Lobby");

                        OnKickedFromLobby?.Invoke(joinedLobby);
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        private IEnumerator HandleRefreshLobbyList()
        {
            while(Application.isPlaying)
            {
                if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) 
                {
                    yield return new WaitForSeconds(refreshLobbyListInterval);
                    RefreshLobbyList();
                }
                else
                {
                    yield return null;
                }
            }
        }
        

        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
                return allocation;
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e);
                return default;
            }
        }

        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return joinCode;
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e);
                return "";
            }
        }

        private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                return joinAllocation;
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e);
                return default;
            }
        }

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        private bool IsPlayerInLobby() 
        {
            if (joinedLobby != null && joinedLobby.Players != null) 
            {
                foreach (Player player in joinedLobby.Players) 
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId) 
                    {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnDisable() 
        {
            if(lobbyHeartbeatCoroutine != null)
            {
                StopCoroutine(lobbyHeartbeatCoroutine);
                lobbyHeartbeatCoroutine = null;
            }

            if(lobbyPoolingCoroutine != null)
            {
                StopCoroutine(lobbyPoolingCoroutine);
                lobbyPoolingCoroutine = null;
            }

            if(refreshLobbyListCoroutine != null)
            {
                StopCoroutine(refreshLobbyListCoroutine);
                refreshLobbyListCoroutine = null;
            }    
        }
    }
}
