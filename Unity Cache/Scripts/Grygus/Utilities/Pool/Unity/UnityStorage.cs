using UnityEngine;
using System;
using System.Collections.Generic;
using Grygus.Utilities.Pool;
using Grygus.Utilities.Pool.Unity;

namespace Grygus.Utilities.Pool.Unity
{
    public class UnityStorage : MonoBehaviour
    {
        public List<UnityPool> Pools
        {
            get { return _pools; }
        }
        [SerializeField]
        private List<UnityPool> _pools;
        private Cache<GameObject>.CacheRegister _caches;
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _caches = Cache<GameObject>.Caches;
            for (int i = 0; i < _pools.Count; i++)
            {
                var pool = _pools[i];
                var parent = new GameObject(pool.Prefab.name);
                parent.transform.parent = transform;
                _caches[pool.Prefab.name].SetFactory(() =>
                    {
                        var gm = new GameObject();
                        gm.transform.parent = parent.transform;
                        return gm;
                    })
                    .SetResetAction((item) =>
                    {
                        item.SetActive(false);
                    })
                    .Generate(pool.Size);
            }
        }

        [ContextMenu("TestGenerate")]
        public void TestGenerateOne()
        {
            for (int i = 0; i < _pools.Count; i++)
            {
                var pool = _pools[i];
                _caches[pool.Prefab.name].Generate(1);

            }
        }
    }
}