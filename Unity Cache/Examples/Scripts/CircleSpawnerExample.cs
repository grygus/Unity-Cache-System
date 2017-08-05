using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Grygus.Utilities.Pool;
using Grygus.Utilities.Pool.Unity;
using UnityEngine.SocialPlatforms;

public class CircleSpawnerExample : MonoBehaviour
{
    [SerializeField]
    private GameObject Prefab;

    [SerializeField]
    private float _spawnDelay;
    private float _spawnTimer;
    [SerializeField]
    private float _circleRadius;
    private float _lastRatio;
    [Range(0,1)]
    [SerializeField]
    private float _spawnOffset;

    [SerializeField]
    private int MaxSpawns;

    private List<GameObject> _spawns;

    [SerializeField] private AnimationCurve _spawnRatio;
    [SerializeField] private float _ratioTimer;
    [SerializeField] private float _ratioMaxTime;

	void Start ()
	{
        _spawns = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update ()
	{

	    if (_spawnTimer <= 0)
	    {
	        _spawnTimer = _spawnDelay;
	        var x = _circleRadius * Mathf.Cos(Mathf.PI * 2 * _lastRatio);
	        var y = _circleRadius * Mathf.Sin(Mathf.PI * 2 * _lastRatio);
	        _lastRatio += _spawnOffset;
	        if (_spawnOffset >= 1)
	            _spawnOffset = _spawnOffset - 1;

            var obj = Cache<GameObject>.Caches[Prefab.name].Pop();
            obj.transform.position = new Vector3(x,0,y);
            obj.SetActive(true);
            _spawns.Add(obj);
	        if (_spawns.Count == MaxSpawns)
	        {
	            var despawnObj = _spawns[0];
                _spawns.RemoveAt(0);
                Cache<GameObject>.Caches[Prefab.name].Push(despawnObj);

            }
	    }
	    else
	    {
	        _spawnTimer -= Time.deltaTime;
	    }
	}
}
