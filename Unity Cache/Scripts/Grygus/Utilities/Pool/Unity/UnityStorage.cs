using UnityEngine;
using System;
using Grygus.Utilities.Pool;

public class UnityStorage : MonoBehaviour
{
    [SerializeField]
    private UnityPool[] _poolsInfo;
    private Cache<GameObject>.CacheRegister _caches;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _caches = Cache<GameObject>.Caches;
        for (int i = 0; i < _poolsInfo.Length; i++)
        {
            var pool = _poolsInfo[i];
            var parent = new GameObject(pool.Name);
            parent.transform.parent = transform;
            _caches[pool.Name].SetFactory(() =>
                {
                    var gm = new GameObject();
                    gm.transform.parent = parent.transform;
                    return gm;
                })
                .SetResetAction((item) =>
                {
                    item.SetActive(false);
                })
                .Generate(pool.Count);
        }
    }

    [ContextMenu("TestGenerate")]
    public void TestGenerateOne()
    {
        for (int i = 0; i < _poolsInfo.Length; i++)
        {
            var pool = _poolsInfo[i];
            _caches[pool.Name].Generate(1);

        }
    }
}

[Serializable]
public class UnityPool
{
    public string Name;
    public int Count;

}