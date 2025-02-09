using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Unity.Netcode;

namespace Game.Gameplay
{
    public class GameVisualManager : NetworkBehaviour
    {
        [SerializeField]private Vector2 origin = Vector2.zero;
        [SerializeField]private Vector2 gap = Vector2.zero;
        [SerializeField]private Vector2 cellSize = Vector2.zero;
        [SerializeField]private SquashStretchEffect circlePrefab;
        [SerializeField]private SquashStretchEffect crossPrefab;

        private void InstantiateSomething(int x, int y, MarkType playerType)
        {
            Debug.Log("Instantiate something");
            if(playerType == MarkType.Cross)
            {
                SpawnCrossServerRpc(x,y);
            }
            else
            {
                SpawnCircleServerRpc(x, y);
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)] 
        private void SpawnCircleServerRpc(int x, int y)
        {
            Debug.Log("Spawn Circle");
            Vector3 spawnPosition = CommonUtility.GetWorldPosition2D(origin, x, y, 1,-1, cellSize, gap.x, gap.y);
            var circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);
            circle.GetComponent<NetworkObject>().Spawn(true);
            circle.PlayEffect();
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)] 
        private void SpawnCrossServerRpc(int x, int y)
        {
            Debug.Log("Spawn Cross");
            Vector3 spawnPosition = CommonUtility.GetWorldPosition2D(origin, x, y, 1,-1, cellSize, gap.x, gap.y);
            var cross = Instantiate(crossPrefab, spawnPosition, Quaternion.identity);
            cross.GetComponent<NetworkObject>().Spawn(true);
            cross.PlayEffect();
        }


        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnClickedGridPosition += InstantiateSomething;
        }

        
        public override void OnDestroy() 
        {
            GameManager.Instance.OnClickedGridPosition -= InstantiateSomething;
            base.OnDestroy();
        }
    }
}
