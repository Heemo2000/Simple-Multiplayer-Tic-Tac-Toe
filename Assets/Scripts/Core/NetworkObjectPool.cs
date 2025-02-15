using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Game.Core
{
    public class NetworkObjectPool : NetworkSingelton<NetworkObjectPool>
    {
        [SerializeField]private List<PoolConfigObject> pooledPrefabList;

        private Dictionary<GameObject, ObjectPool<NetworkObject>> pooledObjects;
        private HashSet<GameObject> prefabs;

        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var networkObject = pooledObjects[prefab].Get();

            var noTransform = networkObject.transform;
            noTransform.position = position;
            noTransform.rotation = rotation;

            return networkObject;
        }

        public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
        {
            pooledObjects[prefab].Release(networkObject);
        }

        void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
        {
            NetworkObject CreateFunc()
            {
                return Instantiate(prefab).GetComponent<NetworkObject>();
            }

            void ActionOnGet(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(true);
            }

            void ActionOnRelease(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(false);
            }

            void ActionOnDestroy(NetworkObject networkObject)
            {
                Destroy(networkObject.gameObject);
            }

            prefabs.Add(prefab);

            // Create the pool
            pooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

            // Populate the pool
            var prewarmNetworkObjects = new List<NetworkObject>();
            for (var i = 0; i < prewarmCount; i++)
            {
                prewarmNetworkObjects.Add(pooledObjects[prefab].Get());
            }
            foreach (var networkObject in prewarmNetworkObjects)
            {
                pooledObjects[prefab].Release(networkObject);
            }

            // Register Netcode Spawn handlers
            NetworkManager.PrefabHandler.AddHandler(prefab, new NetworkPooledPrefabInstanceHandler(prefab, this));
        }
    
        public override void OnNetworkSpawn()
        {
             foreach (var configObject in pooledPrefabList)
            {
                RegisterPrefabInternal(configObject.prefab, configObject.preWarmCount);
            }
        }

        public override void OnNetworkDespawn()
        {
            foreach(var prefab in prefabs)
            {
                NetworkManager.PrefabHandler.RemoveHandler(prefab);
                pooledObjects[prefab].Clear();
            }

            pooledObjects.Clear();
            prefabs.Clear();
        }

        protected override void InternalInit()
        {
            pooledObjects = new Dictionary<GameObject, ObjectPool<NetworkObject>>();
            prefabs = new HashSet<GameObject>();
        }

        protected override void InternalOnStart()
        {
            
        }

        protected override void InternalOnDestroy()
        {
            
        }

        private void OnValidate() 
        {
            for(int i = 0; i < pooledPrefabList.Count; i++)
            {
                var prefab = pooledPrefabList[i].prefab;
                if(prefab != null)
                {
                    Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"Pooled prefab at index {i.ToString()} must have NetworkObject component");
                }
            }    
        }
    }
}
