using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public Snake snake;

    private GameObject currentCherry;
    private SnakeAgent agent;
    private float boundaryX;
    private float boundaryY;

    void Start()
    {
        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;
        SpawnCherry();
    }

    public void RegisterAgent(SnakeAgent a) => agent = a;

    public GameObject GetCurrentCherry() => currentCherry;

    public void SpawnCherry()
    {
        Vector3 spawnPos;
        int attempts = 0;
        do
        {
            float x = Random.Range(-boundaryX + 1, boundaryX - 1);
            float y = Random.Range(-boundaryY + 1, boundaryY - 1);
            spawnPos = new Vector3(x, y, 0);
            attempts++;
        }
        while (IsInsideObstacle(spawnPos) && attempts < 100);

        if (currentCherry != null)
            Destroy(currentCherry);

        currentCherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
        currentCherry.name = "Cherry";

        BoxCollider2D col = currentCherry.GetComponent<BoxCollider2D>();
        if (col == null)
            col = currentCherry.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        CherryCollision cc = currentCherry.GetComponent<CherryCollision>();
        if (cc == null)
            cc = currentCherry.AddComponent<CherryCollision>();
        cc.controller = this;
    }

    public void OnCherryEaten()
    {
        snake.Grow();
        SpawnCherry();
        agent?.NotifyCherryEaten();
    }

    private bool IsInsideObstacle(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, 0.5f);
        if (hit != null && hit.GetComponent<ObstacleCollision>() != null)
            return true;
        return snake.IsNearSnake(pos, 0.5f);
    }
}
