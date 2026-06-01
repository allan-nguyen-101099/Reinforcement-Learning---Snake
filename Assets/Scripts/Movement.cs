using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 direction = Vector2.right;

    private float boundaryX;
    private float boundaryY;

    void Start()
    {
        // Calculate boundaries based on camera size
        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            direction = Vector2.right;
    }
git 
    void FixedUpdate()
    {
        transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);

        Vector3 pos = transform.position;

        // Wrap around based on camera bounds
        if (pos.x > boundaryX)
            pos.x = -boundaryX;
        else if (pos.x < -boundaryX)
            pos.x = boundaryX;

        if (pos.y > boundaryY)
            pos.y = -boundaryY;
        else if (pos.y < -boundaryY)
            pos.y = boundaryY;

        transform.position = pos;
    }
}
