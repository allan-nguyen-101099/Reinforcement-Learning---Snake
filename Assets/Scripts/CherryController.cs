using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;   // assign Cherry prefab in Inspector
    private GameObject currentCherry;

    private float boundaryX;
    private float boundaryY;

    void Start()
    {
        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;

        SpawnCherry();
    }

    public void SpawnCherry()
    {
        float x = Random.Range(-boundaryX + 1, boundaryX - 1);
        float y = Random.Range(-boundaryY + 1, boundaryY - 1);
        Vector3 spawnPos = new Vector3(x, y, 0);

        if (currentCherry != null)
        {
            Destroy(currentCherry);
        }

        currentCherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
        currentCherry.name = "Cherry";

        // Ensure collider is 2D
        BoxCollider2D collider = currentCherry.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = currentCherry.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;

        // Attach collision script
        CherryCollision cherryCollision = currentCherry.GetComponent<CherryCollision>();
        if (cherryCollision == null)
        {
            cherryCollision = currentCherry.AddComponent<CherryCollision>();
        }
        cherryCollision.controller = this;
    }

    public void OnCherryEaten()
    {
        SpawnCherry();
    }
}
