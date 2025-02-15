using UnityEngine;
using Unity.Netcode;
namespace Game.Core
{
    public class NetworkPooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private GameObject prefab;
        private NetworkObjectPool pool;

        public NetworkPooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            this.prefab = prefab;
            this.pool = pool;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return pool.GetNetworkObject(prefab, position, rotation);
        }

        public void Destroy(NetworkObject networkObject)
        {
            pool.ReturnNetworkObject(networkObject, prefab);
        }
    }
}
