using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameObject snakeHeadPrefab;
    public GameObject bodySegmentPrefab;

    private GameObject snakeHead;
    private Movement headMovement;
    private List<GameObject> bodySegments = new List<GameObject>();

    public Vector2 HeadPosition => snakeHead.transform.position;

    void Start()
    {
        snakeHead = Instantiate(snakeHeadPrefab, Vector3.zero, Quaternion.identity);
        snakeHead.name = "SnakeHead";

        headMovement = snakeHead.GetComponent<Movement>();
        headMovement.bodySegments = bodySegments;

        GameManager.Instance.RegisterSnake(this);
    }

    public void ResetSnake()
    {
        foreach (var seg in bodySegments)
            Destroy(seg);
        bodySegments.Clear();

        snakeHead.transform.position = Vector3.zero;
        headMovement.ResetMovement();
    }

    public Movement GetHeadMovement() => headMovement;

    public void Grow()
    {
        Vector3 spawnPos = bodySegments.Count > 0
            ? bodySegments[bodySegments.Count - 1].transform.position
            : snakeHead.transform.position;

        GameObject segment = Instantiate(bodySegmentPrefab, spawnPos, Quaternion.identity);
        segment.name = "SnakeBody_" + bodySegments.Count;
        bodySegments.Add(segment);
    }

    public int GetLength() => bodySegments.Count + 1;

    public bool IsNearSnake(Vector3 pos, float radius)
    {
        if (Vector2.Distance(pos, snakeHead.transform.position) < radius)
            return true;
        foreach (var seg in bodySegments)
            if (Vector2.Distance(pos, seg.transform.position) < radius)
                return true;
        return false;
    }
}
