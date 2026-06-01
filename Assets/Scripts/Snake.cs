using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameObject snakeHeadPrefab; // assign a circle sprite prefab in Inspector

    private GameObject snakeHead;

    void Start()
    {
        // Spawn the snake head at origin
        Vector3 startPosition = Vector3.zero;
        snakeHead = Instantiate(snakeHeadPrefab, startPosition, Quaternion.identity);

        // Optional: name it for clarity
        snakeHead.name = "SnakeHead";
    }
}
