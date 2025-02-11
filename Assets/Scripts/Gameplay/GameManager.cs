using System;
using UnityEngine;
using Unity.Netcode;
using Game.Core;
using System.Collections.Generic;

namespace Game.Gameplay
{
    public class GameManager : NetworkSingelton<GameManager>
    {
        private MarkType localPlayerType = MarkType.None;
        private NetworkVariable<MarkType> currentPlayablePlayerType = new NetworkVariable<MarkType>(MarkType.None);
        private NetworkVariable<int> crossWinsCount = new NetworkVariable<int>(0);
        private NetworkVariable<int> circleWinsCount = new NetworkVariable<int>(0);
        private MarkType[,] playerTypeArray = null;
        private List<Line> linesList;
        public Action<int,int, MarkType> OnClickedGridPosition;
        public Action OnPlacedObject;
        public Action OnGameStarted;
        public Action<Line, MarkType> OnGameWin;
        public Action OnCurrentPlayablePlayerChanged;
        public Action OnRematch;
        public Action OnGameTied;
        public Action OnScoreChanged;
        public MarkType LocalPlayerType { get => localPlayerType; }
        public MarkType CurrentPlayablePlayerType { get => currentPlayablePlayerType.Value; }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        public void RematchServerRpc()
        {
            for(int x = 0; x < playerTypeArray.GetLength(0); x++)
            {
                for(int y = 0; y < playerTypeArray.GetLength(1); y++)
                {
                    playerTypeArray[x,y] = MarkType.None; 
                }
            }

            currentPlayablePlayerType.Value = MarkType.Cross;
            TriggerOnRematchClientRpc();
        }

        

        public void GetScores(out int crossWinsCount, out int circleWinsCount)
        {
            crossWinsCount = this.crossWinsCount.Value;
            circleWinsCount = this.circleWinsCount.Value;
        }

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
            OnPlacedObject?.Invoke();

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

            crossWinsCount.OnValueChanged += (int oldScore, int newScore) => {
                OnScoreChanged?.Invoke();
            };

            circleWinsCount.OnValueChanged += (int oldScore, int newScore) => {
                OnScoreChanged?.Invoke();
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
            crossWinsCount.Value = 0;
            circleWinsCount.Value = 0;
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

            for(int i = 0 ; i < linesList.Count; i++)
            {
                Line line = linesList[i];
                if(CheckLine(line))
                {
                    MarkType winPlayerType = playerTypeArray[line.Centre.x, line.Centre.y];
                    
                    switch(winPlayerType)
                    {
                        case MarkType.Cross:
                                             crossWinsCount.Value++;
                                             break;
                        
                        case MarkType.Circle:
                                             circleWinsCount.Value++;
                                             break;
                    }

                    TriggerOnGameWinServerRpc(i, winPlayerType);
                    currentPlayablePlayerType.Value = MarkType.None;
                    return;    
                }
            }

            bool hasTie = true;
            for(int x = 0; x < playerTypeArray.GetLength(0); x++)
            {
                for(int y = 0; y < playerTypeArray.GetLength(1); y++)
                {
                    if(playerTypeArray[x,y] == MarkType.None)
                    {
                        hasTie = false;
                        break;
                    }
                }
            }

            if(hasTie)
            {
                TriggerOnGameTiedServerRpc();
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void TriggerOnRematchClientRpc()
        {
            OnRematch?.Invoke();
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void TriggerOnGameWinServerRpc(int lineIndex, MarkType playerType)
        {
            TriggerOnGameWinClientRpc(lineIndex, playerType);
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void TriggerOnGameWinClientRpc(int lineIndex, MarkType playerType)
        {
            Line line = linesList[lineIndex];
            OnGameWin?.Invoke(line, playerType);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void TriggerOnGameTiedServerRpc()
        {
            TriggerOnGameTiedClientRpc();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void TriggerOnGameTiedClientRpc()
        {
            OnGameTied?.Invoke();
        }

        protected override void InternalInit()
        {
            playerTypeArray = new MarkType[3,3];
            for(int x = 0; x < playerTypeArray.GetLength(0); x++)
            {
                for(int y = 0; y < playerTypeArray.GetLength(1); y++)
                {
                    playerTypeArray[x,y] = MarkType.None; 
                }
            }

            linesList = new List<Line>
            {
                //Horizontal

                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                          new Vector2Int(0, 1), Orientation.Horizontal),

                new Line(new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                          new Vector2Int(1, 1), Orientation.Horizontal),

                new Line(new List<Vector2Int>() { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                          new Vector2Int(2, 1), Orientation.Horizontal),

                //Vertical
                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                          new Vector2Int(1, 0), Orientation.Vertical),

                new Line(new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                          new Vector2Int(1, 1), Orientation.Vertical),

                new Line(new List<Vector2Int>() { new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                          new Vector2Int(1, 2), Orientation.Vertical),

                //Diagonal
                new Line(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                          new Vector2Int(1, 1), Orientation.DiagonalA),

                new Line(new List<Vector2Int>() { new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                          new Vector2Int(1, 1), Orientation.DiagonalB)
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
