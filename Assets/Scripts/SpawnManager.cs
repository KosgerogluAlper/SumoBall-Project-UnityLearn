using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private GameObject[] powerupPrefabs;
    readonly private float spawnRange = 9.0f;
    private int waveNumber = 1;
    private int enemyCount;

    void Start()
    {
        SpawnPowerup();
        SpawnEnemyWave(waveNumber);
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0)
        {
            waveNumber++;
            SpawnPowerup();
            SpawnEnemyWave(waveNumber);
        }
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 randomPos = GenerateSpawnPos();
            int randomEnemyIndex = UnityEngine.Random.Range(0, 2);
            Instantiate(enemyPrefab[randomEnemyIndex], randomPos, enemyPrefab[randomEnemyIndex].transform.rotation);

            if ((waveNumber == 3 || waveNumber == 6) && i == 0)
            {
                Instantiate(enemyPrefab[2], randomPos, enemyPrefab[2].transform.rotation);
            }
        }
    }

    private void SpawnPowerup()
    {
        int randomPowerup = UnityEngine.Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPos(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    private Vector3 GenerateSpawnPos()
    {
        float spawnPosX = UnityEngine.Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}