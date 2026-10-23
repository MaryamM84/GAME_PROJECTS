
using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject coinPrefab;
    public GameSpeedManager speedManager;

    [Header("Spawn Settings")]
    public float minSpawnTime = 3f;
    public float maxSpawnTime = 6f;
    public float spawnOffset = 2f;
    public float coinSpawnY = 0.5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null ||
            coinPrefab == null ||
            speedManager == null)
        {
            Debug.LogError(
                "CoinSpawner: Assign all required references."
            );
            enabled = false;
            return;
        }

        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            float waitTime = Random.Range(
                minSpawnTime,
                maxSpawnTime
            );

            yield return new WaitForSeconds(waitTime);

            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        float spawnX =
            mainCamera.transform.position.x
            + mainCamera.orthographicSize
            * mainCamera.aspect
            + spawnOffset;

        GameObject spawnedCoin = Instantiate(
            coinPrefab,
            new Vector3(spawnX, coinSpawnY, 0f),
            Quaternion.identity
        );

        ObstacleMover mover =
            spawnedCoin.GetComponent<ObstacleMover>();

        if (mover != null)
        {
            mover.speedManager = speedManager;
        }
        else
        {
            Debug.LogWarning(
                "Coin prefab has no ObstacleMover."
            );
        }
    }
}
