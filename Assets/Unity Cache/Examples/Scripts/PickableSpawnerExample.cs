using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Grygus.Utilities.Pool;
using Grygus.Utilities.Pool.Unity;

public class PickableSpawnerExample : MonoBehaviour
{
    public enum ESpawnerType
    {
        Line,
        Circle    
    }

    [SerializeField] private GameObject _prefab;
    [SerializeField]
    private float _spawnDistance;
    [SerializeField]
    private float _spawnDelay;
    [SerializeField]
    private int _maxSpawned;
    [SerializeField]
    private float _maxDistance;

    [SerializeField] private AnimationCurve _spawnRatio;
    [SerializeField] private float _ratioTimer;
    [SerializeField] private float _ratioMaxTime;

    private float _spawnTimer;
    private float _lastSpawnDistance;

    private List<GameObject> _activeGameObjects;
//    [Header("Line Settings")]
    [SerializeField]
    private ESpawnerType _eSpawner;
    [SerializeField]
    private float _circleRadius;

    [SerializeField] private bool _clampDistanceToCircle;
    public UnityPool CachePool;

    private void OnValidate()
    {
        if (_clampDistanceToCircle)
            _maxDistance = _circleRadius * Mathf.PI * 2;
    }
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
        Spawn(pickable, transform.position+GetNextPosition(), transform.rotation);
    }

    private Vector3 GetNextPosition()
    {
        var position = Vector3.zero;
        var distance = _lastSpawnDistance + _spawnDistance;
        if (distance > _maxDistance)
            distance = 0;
        _lastSpawnDistance = distance;
        switch (_eSpawner)
        {
            case ESpawnerType.Line:
                position = Vector3.forward * distance;
                break;
            case ESpawnerType.Circle:
                var ratio = distance / (_circleRadius * Mathf.PI * 2);
                var x = _circleRadius * Mathf.Cos(Mathf.PI * 2 * ratio);
                var y = _circleRadius * Mathf.Sin(Mathf.PI * 2 * ratio);
                position = new Vector3(x,0,y);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return position;
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
