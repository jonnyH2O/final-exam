using System.Collections.Generic;
using UnityEngine;

// Weaves between the level's lanes (read from WaveManager's spawn points) while
// advancing. Only steps to an ADJACENT lane each time, so it can't jump across
// the whole field at once. Works on any level without per-prefab setup.
public class ZoomEnemy : Enemy
{
    [Header("Lane Weaving")]
    [Tooltip("Seconds between steps to an adjacent lane.")]
    [SerializeField] private float laneChangeInterval = 1.2f;
    [Tooltip("How fast it slides sideways toward the next lane.")]
    [SerializeField] private float lateralSpeed = 5f;
    [Tooltip("Forward speed multiplier while mid-dodge between lanes. 1.6 = 60% faster.")]
    [SerializeField] private float dodgeSpeedMultiplier = 1.6f;
    [Tooltip("Fallback weave range (Z) around spawn if no lanes are available.")]
    [SerializeField] private float weaveAmplitude = 2f;

    private float[] _laneZ; // sorted ascending so index +/-1 is a spatial neighbour
    private int _currentLane;
    private float _targetZ;
    private float _nextChangeTime;
    private float _spawnZ;

    protected override void OnInitialized()
    {
        _spawnZ = transform.position.z;
        _laneZ = GetSortedLaneZPositions();
        _currentLane = _laneZ != null ? NearestLaneIndex(_spawnZ) : 0;
        _targetZ = _spawnZ;
        _nextChangeTime = Time.time + laneChangeInterval;
    }

    protected override void Move()
    {
        if (Time.time >= _nextChangeTime)
            StepToAdjacentLane();

        Vector3 p = transform.position;

        // Dodge burst: advance faster while still sliding toward the target lane,
        // back to normal speed once settled.
        bool changingLanes = Mathf.Abs(_targetZ - p.z) > 0.001f;
        float forwardSpeed = speed * (changingLanes ? dodgeSpeedMultiplier : 1f);
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);

        // Slide sideways (Z) toward the chosen lane.
        float z = Mathf.MoveTowards(p.z, _targetZ, lateralSpeed * Time.deltaTime);
        Vector3 np = transform.position;
        transform.position = new Vector3(np.x, np.y, z);
    }

    private void StepToAdjacentLane()
    {
        _nextChangeTime = Time.time + laneChangeInterval;

        // No lanes available: fall back to a small random weave around spawn.
        if (_laneZ == null || _laneZ.Length == 0)
        {
            _targetZ = _spawnZ + Random.Range(-weaveAmplitude, weaveAmplitude);
            return;
        }

        if (_laneZ.Length == 1)
            return;

        // Step one lane up or down, biased inward at the edges.
        int dir;
        if (_currentLane <= 0) dir = 1;
        else if (_currentLane >= _laneZ.Length - 1) dir = -1;
        else dir = Random.value < 0.5f ? -1 : 1;

        _currentLane = Mathf.Clamp(_currentLane + dir, 0, _laneZ.Length - 1);
        _targetZ = _laneZ[_currentLane];
    }

    // Lane lateral positions from this level's spawn points, sorted by Z.
    private float[] GetSortedLaneZPositions()
    {
        Transform[] points = _waveManager != null ? _waveManager.SpawnPoints : null;
        if (points == null || points.Length == 0)
            return null;

        List<float> z = new List<float>(points.Length);
        foreach (Transform t in points)
            if (t != null) z.Add(t.position.z);

        if (z.Count == 0)
            return null;

        z.Sort();
        return z.ToArray();
    }

    private int NearestLaneIndex(float z)
    {
        int best = 0;
        float bestDist = Mathf.Abs(_laneZ[0] - z);
        for (int i = 1; i < _laneZ.Length; i++)
        {
            float d = Mathf.Abs(_laneZ[i] - z);
            if (d < bestDist) { bestDist = d; best = i; }
        }
        return best;
    }
}
