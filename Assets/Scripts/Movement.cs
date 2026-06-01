using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float segmentSpacing = 1f;
    public float collisionThreshold = 0.2f;
    public List<GameObject> bodySegments = new List<GameObject>();

    public Vector2 Direction => direction;

    private Vector2 direction = Vector2.right;
    private float boundaryX;
    private float boundaryY;

    private List<Vector3> positionHistory = new List<Vector3>();
    private int segmentGap;
    private const int maxHistory = 1000;

    void Start()
    {
        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;
        segmentGap = Mathf.Max(1, Mathf.RoundToInt(segmentSpacing / (moveSpeed * Time.fixedDeltaTime)));
    }

    // Called by SnakeAgent. 0 = straight, 1 = turn left, 2 = turn right
    public void SetAction(int action)
    {
        direction = action switch
        {
            1 => new Vector2(-direction.y,  direction.x), // 90° CCW
            2 => new Vector2( direction.y, -direction.x), // 90° CW
            _ => direction
        };
    }

    public void ResetMovement()
    {
        direction = Vector2.right;
        positionHistory.Clear();
    }

    void FixedUpdate()
    {
        transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);

        Vector3 pos = transform.position;

        if (pos.x > boundaryX) pos.x = -boundaryX;
        else if (pos.x < -boundaryX) pos.x = boundaryX;

        if (pos.y > boundaryY) pos.y = -boundaryY;
        else if (pos.y < -boundaryY) pos.y = boundaryY;

        transform.position = pos;

        positionHistory.Insert(0, pos);
        if (positionHistory.Count > maxHistory)
            positionHistory.RemoveAt(positionHistory.Count - 1);

        for (int i = 0; i < bodySegments.Count; i++)
        {
            int idx = (i + 1) * segmentGap;
            if (idx < positionHistory.Count)
            {
                bodySegments[i].transform.position = positionHistory[idx];

                if (Vector2.Distance(transform.position, bodySegments[i].transform.position) < collisionThreshold)
                {
                    GameManager.Instance.GameOver();
                    return;
                }
            }
        }
    }
}
