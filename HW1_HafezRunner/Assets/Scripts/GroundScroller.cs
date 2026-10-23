
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [Header("Ground Tiles")]
    public Transform tileA;
    public Transform tileB;

    [Header("Movement Settings")]
    public float speed = 5f;
    public GameSpeedManager speedManager;
    public float targetTileWidth = 18f;

    private float tileWidth;

    void Start()
    {
        if (tileA == null || tileB == null)
        {
            Debug.LogError(
                "Please assign both ground tiles."
            );
            enabled = false;
            return;
        }

        SpriteRenderer spriteA =
            tileA.GetComponent<SpriteRenderer>();

        SpriteRenderer spriteB =
            tileB.GetComponent<SpriteRenderer>();

        if (spriteA == null || spriteB == null ||
            spriteA.sprite == null)
        {
            Debug.LogError(
                "Ground tiles need Sprite Renderers and sprites."
            );
            enabled = false;
            return;
        }

        float originalWidth =
            spriteA.sprite.bounds.size.x;

        if (originalWidth <= 0f)
        {
            enabled = false;
            return;
        }

        float scaleX =
            targetTileWidth / originalWidth;

        tileA.localScale = new Vector3(
            scaleX,
            tileA.localScale.y,
            tileA.localScale.z
        );

        tileB.localScale = new Vector3(
            scaleX,
            tileB.localScale.y,
            tileB.localScale.z
        );

        tileWidth = targetTileWidth;

        tileB.position = new Vector3(
            tileA.position.x + tileWidth,
            tileA.position.y,
            tileA.position.z
        );
    }

    void Update()
    {
        if (tileA == null || tileB == null)
            return;

        float currentSpeed = speedManager != null
            ? speedManager.CurrentSpeed
            : speed;

        float movement = currentSpeed * Time.deltaTime;

        tileA.position +=
            Vector3.left * movement;

        tileB.position +=
            Vector3.left * movement;

        if (Camera.main == null)
            return;

        float cameraLeft =
            Camera.main.transform.position.x
            - Camera.main.orthographicSize
            * Camera.main.aspect;

        if (tileA.position.x + tileWidth / 2f
            < cameraLeft)
        {
            tileA.position = new Vector3(
                tileB.position.x + tileWidth,
                tileA.position.y,
                tileA.position.z
            );
        }

        if (tileB.position.x + tileWidth / 2f
            < cameraLeft)
        {
            tileB.position = new Vector3(
                tileA.position.x + tileWidth,
                tileB.position.y,
                tileB.position.z
            );
        }
    }
}
