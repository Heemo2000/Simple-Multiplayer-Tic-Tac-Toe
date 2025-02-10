using System;
using UnityEngine;
using Unity.Netcode;
using Game.Core;
using Unity.Collections;
using System.Collections.Generic;

namespace Game.Gameplay
{
    public class GameManager : NetworkSingelton<GameManager>
    {
        private MarkType localPlayerType = MarkType.None;
        private NetworkVariable<MarkType> currentPlayablePlayerType = new NetworkVariable<MarkType>(MarkType.None);
        private MarkType[,] playerTypeArray = null;
        private List<Line> linesList;
        public Action<int,int, MarkType> OnClickedGridPosition;
        public Action OnGameStarted;
        public Action<Line> OnGameWin;
        public Action OnCurrentPlayablePlayerChanged;
        public MarkType LocalPlayerType { get => localPlayerType; }
        public MarkType CurrentPlayablePlayerType { get => currentPlayablePlayerType.Value; }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        public void ClickedOnGridPositionServerRpc(int x, int y, MarkType playerType)
        {
            Debug.Log("Clicked on grid position: " + x + ", " + y); 
            if(playerType != currentPlayablePlayerType.Value)
            {
                return;
            }

            if(playerTypeArray[x,y] != MarkType.None)
            {
                return;
            }

            playerTypeArray[x,y] = playerType;

            OnClickedGridPosition?.Invoke(x, y, playerType);

            switch(currentPlayablePlayerType.Value)
            {
                default: 
                         currentPlayablePlayerType.Value = MarkType.Cross;
                         break;

                case MarkType.Cross:
                                    currentPlayablePlayerType.Value = MarkType.Circle;
                                    break;

                case MarkType.Circle:
                                    currentPlayablePlayerType.Value = MarkType.Cross;
                                    break;
            }

            CheckWinner();
        }

        public override void OnNetworkSpawn()
        {
            if(NetworkManager.IsServer)
            {
                localPlayerType = MarkType.Cross;
            }
            else
            {
                localPlayerType = MarkType.Circle;
            }

            Debug.Log("Local Player Type: " + localPlayerType.ToString());

            if(NetworkManager.IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnected;
            }

            currentPlayablePlayerType.OnValueChanged += (MarkType oldPlayerType, MarkType newPlayerType) => {
                OnCurrentPlayablePlayerChanged?.Invoke();
            };
        }

        public override void OnNetworkDespawn()
        {
            if(NetworkManager.IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        private void OnClientConnected(ulong value)
        {
            if(NetworkManager.ConnectedClientsList.Count == 2)
            {
                TriggerOnGameStartedServerRpc();
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void TriggerOnGameStartedServerRpc()
        {
            currentPlayablePlayerType.Value = MarkType.Cross;
            TriggerOnGameStartedClientRpc();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void TriggerOnGameStartedClientRpc()
        {
            OnGameStarted?.Invoke();
        }

        private bool CheckLine(Line line)
        {
            MarkType firstPoint = playerTypeArray[line.Points[0].x, line.Points[0].y];
            MarkType secondPoint = playerTypeArray[line.Points[1].x, line.Points[1].y];
            MarkType thirdPoint = playerTypeArray[line.Points[2].x, line.Points[2].y];
            return CheckWinnerLine(firstPoint, secondPoint, thirdPoint);
        }

        private bool CheckWinnerLine(MarkType playerTypeA, MarkType playerTypeB, MarkType playerTypeC)
        {
            return playerTypeA != MarkType.None && playerTypeA == playerTypeB && playerTypeB == playerTypeC;
        }
        private void CheckWinner()
        {
            //Top
            //Middle
            //Bottom
            //Diagonal from top left to bottom right
            //Diagonal from top right to bottom left

            foreach(Line line in linesList)
            {
                if(CheckLine(line))
                {
                    OnGameWin?.Invoke(line);
                    currentPlayablePlayerType.Value = MarkType.None;
                    break;    
                }
            }
        }

        protected override void InternalInit()
        {
            playerTypeArray = new MarkType[3,3];
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    playerTypeArray[x,y] = MarkType.None; 
                }
            }

            linesList = new List<Line>
            {
                //Horizontal

                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                          new Vector2Int(0, 1)),
                new Line(new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                          new Vector2Int(1, 1)),
                new Line(new List<Vector2Int>() { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                          new Vector2Int(2, 1)),

                //Vertical
                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                          new Vector2Int(1, 0)),
                new Line(new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                          new Vector2Int(1, 1)),
                new Line(new List<Vector2Int>() { new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                          new Vector2Int(1, 2)),

                //Diagonal
                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                          new Vector2Int(1, 1)),
                new Line(new List<Vector2Int>() { new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                          new Vector2Int(1, 1))
            };
        }

        protected override void InternalOnStart()
        {
            
        }

        protected override void InternalOnDestroy()
        {
            
        }
    }
}
