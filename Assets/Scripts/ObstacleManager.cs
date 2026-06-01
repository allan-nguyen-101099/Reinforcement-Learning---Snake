using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public int wallCount = 10;
    public float minLength = 2f;
    public float maxLength = 5f;
    public float thickness = 0.3f;
    public float safeRadius = 3f;

    private float boundaryX;
    private float boundaryY;
    private List<GameObject> spawnedWalls = new List<GameObject>();

    void Start()
    {
        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;
        SpawnWalls();
    }

    public void RespawnWalls()
    {
        foreach (var wall in spawnedWalls)
            if (wall != null) Destroy(wall);
        spawnedWalls.Clear();
        SpawnWalls();
    }

    void SpawnWalls()
    {
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 pos = RandomPosition();
            float length = Random.Range(minLength, maxLength);
            float angle = Random.Range(0f, 180f);
            CreateWall(pos, length, angle);
        }
    }

    Vector3 RandomPosition()
    {
        Vector3 pos;
        int attempts = 0;
        do
        {
            float x = Random.Range(-boundaryX + 2f, boundaryX - 2f);
            float y = Random.Range(-boundaryY + 2f, boundaryY - 2f);
            pos = new Vector3(x, y, 0f);
            attempts++;
        }
        while (pos.magnitude < safeRadius && attempts < 20);
        return pos;
    }

    void CreateWall(Vector3 position, float length, float angle)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Obstacle_" + position.x.ToString("F1");
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(thickness, length, 1f);
        wall.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        wall.GetComponent<Renderer>().material.color = new Color(0.8f, 0.4f, 0.1f);

        Destroy(wall.GetComponent<BoxCollider>());
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        wall.AddComponent<ObstacleCollision>();
        spawnedWalls.Add(wall);
    }
}
