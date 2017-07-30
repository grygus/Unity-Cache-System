using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Grygus.Utilities.Pool;

public class PickableSpawnerExample : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private float _spawnDistance;
    [SerializeField]
    private float _spawnDelay;
    [SerializeField]
    private int _maxSpawned;
    [SerializeField]
    private int _maxDistance;

    [SerializeField] private AnimationCurve _spawnRatio;
    [SerializeField] private float _ratioTimer;
    [SerializeField] private float _ratioMaxTime;

    private float _spawnTimer;
    private float _lastSpawnDistance;

    private List<GameObject> _activeGameObjects;
    // Use this for initialization
    void Start()
    {
        _activeGameObjects = new List<GameObject>();
        Cache<GameObject>.Caches[_prefab.name]
            .SetFactory(() =>
            {
                var gm = Instantiate(_prefab);
                gm.transform.parent = transform;
                return gm;
            })
            .SetResetAction(o => o.SetActive(false))
            .Generate(_maxSpawned);
    }

    // Update is called once per frame
    void Update()
    {
        if (_ratioTimer < _ratioMaxTime)
            _ratioTimer += Time.deltaTime;
        else
            _ratioTimer = 0;           

        var spawnCount = (int)(_spawnRatio.Evaluate(_ratioTimer / _ratioMaxTime)*_maxSpawned);
        if (spawnCount >= 0)
        {
            while (spawnCount > _activeGameObjects.Count)
            {
                Spawn();        
            }
            while(spawnCount < _activeGameObjects.Count)
            {
                Cache<GameObject>.Caches[_prefab.name].Push(_activeGameObjects[0]);
                _activeGameObjects.RemoveAt(0);
            }
        }
    }



    private void Spawn()
    {
        var distance = _lastSpawnDistance + _spawnDistance;
        if (distance > _maxDistance)
            distance = 0;
        _lastSpawnDistance = distance;
        var pickable = GetPickable();
        _activeGameObjects.Add(pickable);
        Spawn(pickable, transform.position + Vector3.forward * distance, transform.rotation);
    }

    private GameObject GetPickable()
    {
        return Cache<GameObject>.Caches[_prefab.name].Pop();
    }

    private void Spawn(GameObject pickable,Vector3 position,Quaternion rotation )
    {
        pickable.transform.position = position;
        pickable.transform.rotation = rotation;
        pickable.SetActive(true);
    }

    private void Despawn(GameObject pickable)
    {
        
    }

}
