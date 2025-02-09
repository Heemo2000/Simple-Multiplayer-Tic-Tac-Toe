using System;
using UnityEngine;
using Unity.Netcode;
using Game.Core;

namespace Game.Gameplay
{
    public class GameManager : NetworkSingelton<GameManager>
    {
        private MarkType localPlayerType = MarkType.None;
        private MarkType currentPlayablePlayerType = MarkType.None;
        public Action<int,int, MarkType> OnClickedGridPosition;

        public MarkType LocalPlayerType { get => localPlayerType; }
        

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        public void ClickedOnGridPositionServerRpc(int x, int y, MarkType playerType)
        {
            Debug.Log("Clicked on grid position: " + x + ", " + y); 
            if(playerType != currentPlayablePlayerType)
            {
                return;
            }
            OnClickedGridPosition?.Invoke(x, y, playerType);

            switch(currentPlayablePlayerType)
            {
                case MarkType.Cross:
                                    currentPlayablePlayerType = MarkType.Circle;
                                    break;

                case MarkType.Circle:
                                    currentPlayablePlayerType = MarkType.Cross;
                                    break;
            }
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

            Debug.Log("Local Mark Type: " + localPlayerType.ToString());

            if(NetworkManager.IsServer)
            {
                currentPlayablePlayerType = MarkType.Cross;
            }
        }

        protected override void InternalInit()
        {
            
        }

        protected override void InternalOnStart()
        {
            
        }

        protected override void InternalOnDestroy()
        {
            
        }
    }
}
