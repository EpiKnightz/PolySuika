using System;
using UnityEngine;

public class ToppingSpawner : MonoBehaviour
{
    float CurrentCountdown = 0;

    public float Interval = 0.5f;

    public GameObject[] ToppingPrefabs;
    public Vector3 SpawnAreaMin;
    public Vector3 SpawnAreaMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCountdown += Time.deltaTime;
        if (CurrentCountdown > Interval)
        {
            CurrentCountdown = 0;
            SpawnTopping();
        }
    }

    void SpawnTopping()
    {
        int idx = UnityEngine.Random.Range(0, ToppingPrefabs.Length);
        Vector3 pos = new Vector3(
            UnityEngine.Random.Range(SpawnAreaMin.x, SpawnAreaMax.x),
            UnityEngine.Random.Range(SpawnAreaMin.y, SpawnAreaMax.y),
            UnityEngine.Random.Range(SpawnAreaMin.z, SpawnAreaMax.z)
        );
        Instantiate(ToppingPrefabs[idx], pos, Quaternion.identity);
    }
}
