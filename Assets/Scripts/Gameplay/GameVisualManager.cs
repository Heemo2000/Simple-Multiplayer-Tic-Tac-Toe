using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Unity.Netcode;
using System;

namespace Game.Gameplay
{
    public class GameVisualManager : NetworkBehaviour
    {
        [SerializeField]private Vector2 origin = Vector2.zero;
        [SerializeField]private Vector2 gap = Vector2.zero;
        [SerializeField]private Vector2 cellSize = Vector2.zero;
        [SerializeField]private SquashStretchEffect circlePrefab;
        [SerializeField]private SquashStretchEffect crossPrefab;
        [SerializeField]private SquashStretchEffect linePrefab;

        private List<SquashStretchEffect> visualGameobjects;

        private void ResetVisuals()
        {
            foreach(var current in visualGameobjects)
            {
                Destroy(current.gameObject);
            }

            visualGameobjects.Clear();
        }
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

        private void SpawnLine(Line line, MarkType playerType)
        {
            if(!NetworkManager.IsServer)
            {
                return;
            }
            float angleZ = 0.0f;
            switch(line.Orientation)
            {
                case Orientation.Horizontal: 
                                            angleZ = 0.0f;
                                            break;
                
                case Orientation.Vertical: 
                                            angleZ = 90.0f;
                                            break;
                
                case Orientation.DiagonalA:
                                            angleZ = -45.0f;
                                            break;

                case Orientation.DiagonalB:
                                            angleZ = 45.0f;
                                            break;
            }
            SpawnLineServerRpc(line.Centre, angleZ);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void SpawnLineServerRpc(Vector2Int centre, float angleZ)
        {
            Vector2 spawnPosition = CommonUtility.GetWorldPosition2D(origin, centre.x, centre.y, 1, -1, cellSize, gap.x, gap.y);
            var lineSprite = Instantiate(linePrefab, spawnPosition, Quaternion.Euler(0.0f, 0.0f, angleZ));
            lineSprite.GetComponent<NetworkObject>().Spawn(true);
            visualGameobjects.Add(lineSprite);
            lineSprite.PlayEffect();
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)] 
        private void SpawnCircleServerRpc(int x, int y)
        {
            Debug.Log("Spawn Circle");
            Vector3 spawnPosition = CommonUtility.GetWorldPosition2D(origin, x, y, 1,-1, cellSize, gap.x, gap.y);
            var circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);
            circle.GetComponent<NetworkObject>().Spawn(true);
            visualGameobjects.Add(circle);
            circle.PlayEffect();
            InstantiateParticles(x,y);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)] 
        private void SpawnCrossServerRpc(int x, int y)
        {
            Debug.Log("Spawn Cross");
            Vector3 spawnPosition = CommonUtility.GetWorldPosition2D(origin, x, y, 1,-1, cellSize, gap.x, gap.y);
            var cross = Instantiate(crossPrefab, spawnPosition, Quaternion.identity);
            cross.GetComponent<NetworkObject>().Spawn(true);
            visualGameobjects.Add(cross);
            cross.PlayEffect();
            InstantiateParticles(x,y);
        }

        private void InstantiateParticles(int x, int y)
        {
            Vector3 spawnPosition = CommonUtility.GetWorldPosition2D(origin, x, y, 1,-1, cellSize, gap.x, gap.y);
            ParticlePoolHandler.Instance.PlayParticleEffect(spawnPosition);
        }


        private void Awake() {
            visualGameobjects = new List<SquashStretchEffect>();
        }
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnClickedGridPosition += InstantiateSomething;
            GameManager.Instance.OnGameWin += SpawnLine;
            GameManager.Instance.OnRematch += ResetVisuals;
        }

        

        public override void OnDestroy() 
        {
            GameManager.Instance.OnClickedGridPosition -= InstantiateSomething;
            GameManager.Instance.OnGameWin -= SpawnLine;
            GameManager.Instance.OnRematch -= ResetVisuals;
            base.OnDestroy();
        }
    }
}
