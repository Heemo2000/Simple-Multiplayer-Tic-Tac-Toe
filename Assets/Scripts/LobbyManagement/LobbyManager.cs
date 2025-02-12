using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.LobbyManagement
{
    public class LobbyManager : MonoSingelton<LobbyManager>
    {
        [Min(10.0f)]
        [SerializeField]private float heartbeatInterval = 15.0f;
        [Min(0.5f)]
        [SerializeField]private float lobbyPollInterval = 1.1f;

        public Action OnSignInSuccess;
        public Action OnSignInFailed;
        public Action OnLeftLobby;
        public Action<Lobby> OnJoinedLobby;
        public Action<Lobby> OnJoinedLobbyUpdate;
        public Action<Lobby> OnKickedFromLobby;
        public Action<List<Lobby>> OnLobbyListChanged;
        private Lobby joinedLobby = null;
        private string playerName = "";
        private Coroutine lobbyHeartbeatCoroutine = null;
        private Coroutine lobbyPoolingCoroutine = null;


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
                Player player = GetPlayer();

                CreateLobbyOptions options = new CreateLobbyOptions {
                    Player = player,
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject> {}
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);

                joinedLobby = lobby;

                OnJoinedLobby?.Invoke(lobby);

                Debug.Log("Created Lobby " + lobby.Name);
            }
            catch(LobbyServiceException e)
            {
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
            Player player = GetPlayer();

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions{
                Player = player
            });

            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(lobby);
        }

        public async void JoinLobby(Lobby lobby)
        {
            Player player = GetPlayer();

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
                                Player = player
                                });

            OnJoinedLobby?.Invoke(lobby);
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
                QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
                
                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

                joinedLobby = lobby;

                OnJoinedLobby?.Invoke(lobby);
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
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
        }
    }
}
