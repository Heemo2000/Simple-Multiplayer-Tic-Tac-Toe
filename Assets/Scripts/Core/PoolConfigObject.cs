using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    [System.Serializable]
    public struct PoolConfigObject
    {
        public GameObject prefab;
        public int preWarmCount;
    }
}
