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
        [SerializeField]private SpriteRenderer linePrefab;

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

        private void SpawnLine(Line line)
        {
            SpawnLineServerRpc(line.Centre, line.Points[0], line.Points[2]);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void SpawnLineServerRpc(Vector2Int centre, Vector2Int firstPoint, Vector2Int thirdPoint)
        {
            Vector2 spawnPosition = CommonUtility.GetWorldPosition2D(origin, centre.x, centre.y, 1, -1, cellSize, gap.x, gap.y);
            Vector2 direction = ((Vector2)thirdPoint - (Vector2)firstPoint).normalized;
            
            float temp = direction.x;
            direction.x = direction.y;
            direction.y = temp;
            
            Debug.Log("Direction: " + direction);
            float angle = Vector2.Angle(Vector2.right, direction);

            var lineSprite = Instantiate(linePrefab, spawnPosition, Quaternion.Euler(0.0f, 0.0f, angle));
            lineSprite.GetComponent<NetworkObject>().Spawn(true);
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
            GameManager.Instance.OnGameWin += SpawnLine;
        }

        
        public override void OnDestroy() 
        {
            GameManager.Instance.OnClickedGridPosition -= InstantiateSomething;
            GameManager.Instance.OnGameWin -= SpawnLine;
            base.OnDestroy();
        }
    }
}
