
using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ground Obstacles")]
    public GameObject[] groundObstaclePrefabs;

    [Header("Air Obstacle")]
    public GameObject airObstaclePrefab;

    [Header("References")]
    public GameSpeedManager speedManager;
    public Collider2D groundCollider;

    [Header("Spawn Settings")]
    public float minSpawnTime = 2.5f;
    public float maxSpawnTime = 4f;
    public float spawnOffset = 2f;
    public float airSpawnY = 0.5f;

    [Range(0f, 1f)]
    public float groundObstacleChance = 0.7f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "ObstacleSpawner: Main Camera not found."
            );
            enabled = false;
            return;
        }

        if (speedManager == null ||
            groundCollider == null ||
            groundObstaclePrefabs == null ||
            groundObstaclePrefabs.Length == 0 ||
            airObstaclePrefab == null)
        {
            Debug.LogError(
                "ObstacleSpawner: Assign all required references."
            );
            enabled = false;
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            float waitTime = Random.Range(
                minSpawnTime,
                maxSpawnTime
            );

            yield return new WaitForSeconds(waitTime);

            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        bool spawnGround =
            Random.value < groundObstacleChance;

        GameObject prefab;
        float spawnY;

        if (spawnGround)
        {
            int index = Random.Range(
                0,
                groundObstaclePrefabs.Length
            );

            prefab = groundObstaclePrefabs[index];

            SpriteRenderer spriteRenderer =
                prefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null ||
                spriteRenderer.sprite == null)
            {
                Debug.LogWarning(
                    "Ground obstacle needs a Sprite Renderer."
                );
                return;
            }

            float objectHeight =
                spriteRenderer.sprite.bounds.size.y
                * prefab.transform.localScale.y;

            spawnY =
                groundCollider.bounds.max.y
                + objectHeight / 2f;
        }
        else
        {
            prefab = airObstaclePrefab;
            spawnY = airSpawnY;
        }

        float spawnX =
            mainCamera.transform.position.x
            + mainCamera.orthographicSize
            * mainCamera.aspect
            + spawnOffset;

        GameObject spawnedObject = Instantiate(
            prefab,
            new Vector3(spawnX, spawnY, 0f),
            Quaternion.identity
        );

        ObstacleMover mover =
            spawnedObject.GetComponent<ObstacleMover>();

        if (mover != null)
        {
            mover.speedManager = speedManager;
        }
        else
        {
            Debug.LogWarning(
                "Spawned obstacle has no ObstacleMover."
            );
        }
    }
}
