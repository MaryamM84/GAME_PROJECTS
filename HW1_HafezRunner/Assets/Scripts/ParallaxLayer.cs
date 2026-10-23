
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Layer Objects")]
    public Transform[] tiles;

    [Header("Speed")]
    public GameSpeedManager speedManager;

    [Range(0f, 1f)]
    public float speedMultiplier = 0.2f;

    [Header("Spacing")]
    public float gap = 3f;

    private float[] tileWidths;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (tiles == null || tiles.Length < 2 ||
            speedManager == null || mainCamera == null)
        {
            Debug.LogError(
                "Assign at least two tiles, a speed manager, and a Main Camera."
            );

            enabled = false;
            return;
        }

        tileWidths = new float[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                Debug.LogError(
                    $"Tile at index {i} is not assigned."
                );

                enabled = false;
                return;
            }

            SpriteRenderer sr =
                tiles[i].GetComponent<SpriteRenderer>();

            if (sr == null || sr.sprite == null)
            {
                Debug.LogError(
                    $"{tiles[i].name} needs a SpriteRenderer with a sprite."
                );

                enabled = false;
                return;
            }

            tileWidths[i] = sr.bounds.size.x;
        }
    }

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || speedManager == null)
            return;

        float currentSpeed =
            speedManager.CurrentSpeed * speedMultiplier;

        Vector3 movement =
            Vector3.left * currentSpeed * Time.deltaTime;

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].position += movement;
        }

        float cameraLeft =
            mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        for (int i = 0; i < tiles.Length; i++)
        {
            float tileRight =
                tiles[i].position.x + tileWidths[i] / 2f;

            if (tileRight < cameraLeft)
            {
                float rightmostEdge = float.NegativeInfinity;

                for (int j = 0; j < tiles.Length; j++)
                {
                    if (j == i)
                        continue;

                    float otherRight =
                        tiles[j].position.x + tileWidths[j] / 2f;

                    if (otherRight > rightmostEdge)
                        rightmostEdge = otherRight;
                }

                tiles[i].position = new Vector3(
                    rightmostEdge + gap + tileWidths[i] / 2f,
                    tiles[i].position.y,
                    tiles[i].position.z
                );
            }
        }
    }
}
