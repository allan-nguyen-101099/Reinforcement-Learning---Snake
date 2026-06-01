using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 1f;

    void Start()
    {
        FocusOnGrid();
    }

    void FocusOnGrid()
    {
        // Calculate grid center (grid is centered at 0, 0)
        Vector3 gridCenter = Vector3.zero;

        // Position camera at grid center with Z offset for 2D viewing
        transform.position = new Vector3(gridCenter.x, gridCenter.y, -10);

        // Calculate orthographic size to fit the entire grid
        float gridHeightWorldUnits = gridHeight * cellSize;
        float cameraOrthographicSize = gridHeightWorldUnits / 2f;

        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.orthographicSize = cameraOrthographicSize;
        }
    }
}
