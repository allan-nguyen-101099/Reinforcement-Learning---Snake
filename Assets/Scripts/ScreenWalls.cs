using UnityEngine;

public class ScreenWalls : MonoBehaviour
{
    public float thickness = 0.1f; // how thick the walls are

    void Start()
    {
        float boundaryY = Camera.main.orthographicSize;
        float boundaryX = boundaryY * Camera.main.aspect;

        // Create 4 walls as simple white cubes
        CreateWall(new Vector3(0, boundaryY, 0), new Vector3(boundaryX * 2, thickness, 1));   // Top
        CreateWall(new Vector3(0, -boundaryY, 0), new Vector3(boundaryX * 2, thickness, 1));  // Bottom
        CreateWall(new Vector3(-boundaryX, 0, 0), new Vector3(thickness, boundaryY * 2, 1));  // Left
        CreateWall(new Vector3(boundaryX, 0, 0), new Vector3(thickness, boundaryY * 2, 1));   // Right
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        // Make it white
        wall.GetComponent<Renderer>().material.color = Color.white;
    }
}
