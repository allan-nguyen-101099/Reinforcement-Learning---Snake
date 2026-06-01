using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// ML-Agents agent that learns to play Snake.
/// Attach this to the same GameObject as Snake.cs.
/// Also requires a BehaviorParameters and DecisionRequester component (add via Inspector).
/// BehaviorParameters: Behavior Name = "SnakeBehavior", Space Size = 11, Discrete Branches = [3]
/// DecisionRequester: Decision Period = 1
/// </summary>
public class SnakeAgent : Agent
{
    private Snake snake;
    private Movement movement;
    private CherryController cherryController;
    private ObstacleManager obstacleManager;

    private Transform cherryTransform;
    private float previousDistanceToCherry;
    private float boundaryX;
    private float boundaryY;

    public override void Initialize()
    {
        snake = GetComponent<Snake>();
        cherryController = FindObjectOfType<CherryController>();
        obstacleManager = FindObjectOfType<ObstacleManager>();

        boundaryY = Camera.main.orthographicSize;
        boundaryX = boundaryY * Camera.main.aspect;

        GameManager.Instance.RegisterAgent(this);
        cherryController.RegisterAgent(this);
    }

    public override void OnEpisodeBegin()
    {
        snake.ResetSnake();
        movement = snake.GetHeadMovement();

        obstacleManager.RespawnWalls();
        cherryController.SpawnCherry();

        cherryTransform = cherryController.GetCurrentCherry().transform;
        previousDistanceToCherry = Vector2.Distance(snake.HeadPosition, cherryTransform.position);
    }

    // Called every FixedUpdate by ML-Agents
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 head = snake.HeadPosition;
        Vector2 dir = movement.Direction;
        Vector2 left  = new Vector2(-dir.y,  dir.x);
        Vector2 right = new Vector2( dir.y, -dir.x);

        // Danger in 3 relative directions (3 values)
        sensor.AddObservation(IsDangerous(head + dir));
        sensor.AddObservation(IsDangerous(head + left));
        sensor.AddObservation(IsDangerous(head + right));

        // Current direction one-hot (4 values)
        sensor.AddObservation(dir == Vector2.up    ? 1f : 0f);
        sensor.AddObservation(dir == Vector2.down  ? 1f : 0f);
        sensor.AddObservation(dir == Vector2.left  ? 1f : 0f);
        sensor.AddObservation(dir == Vector2.right ? 1f : 0f);

        // Cherry relative position (4 values)
        Vector2 cherry = cherryTransform.position;
        sensor.AddObservation(cherry.x < head.x ? 1f : 0f); // cherry is left
        sensor.AddObservation(cherry.x > head.x ? 1f : 0f); // cherry is right
        sensor.AddObservation(cherry.y > head.y ? 1f : 0f); // cherry is up
        sensor.AddObservation(cherry.y < head.y ? 1f : 0f); // cherry is down

        // Total: 11 observations
    }

    // Called every FixedUpdate with the action chosen by the neural network
    public override void OnActionReceived(ActionBuffers actions)
    {
        movement.SetAction(actions.DiscreteActions[0]);

        // Small penalty each step to discourage looping
        AddReward(-0.001f);

        // Reward moving toward cherry, penalize moving away
        float dist = Vector2.Distance(snake.HeadPosition, cherryTransform.position);
        AddReward(dist < previousDistanceToCherry ? 0.01f : -0.01f);
        previousDistanceToCherry = dist;
    }

    // Lets you test with keyboard while ML-Agents is running in Heuristic mode
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discrete = actionsOut.DiscreteActions;
        discrete[0] = 0; // default: straight

        if (movement == null) return;

        Vector2 dir   = movement.Direction;
        Vector2 left  = new Vector2(-dir.y,  dir.x);
        Vector2 right = new Vector2( dir.y, -dir.x);

        Vector2 input = Vector2.zero;
        if      (Input.GetKey(KeyCode.UpArrow))    input = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow))  input = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow))  input = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow)) input = Vector2.right;

        if      (input == left)  discrete[0] = 1;
        else if (input == right) discrete[0] = 2;
    }

    // Called by CherryController when cherry is eaten
    public void NotifyCherryEaten()
    {
        AddReward(1.0f);
        cherryTransform = cherryController.GetCurrentCherry().transform;
        previousDistanceToCherry = Vector2.Distance(snake.HeadPosition, cherryTransform.position);
    }

    // Called by GameManager when snake dies
    public void OnDied()
    {
        AddReward(-1.0f);
        EndEpisode();
    }

    private bool IsDangerous(Vector2 pos)
    {
        // Obstacle wall
        Collider2D hit = Physics2D.OverlapCircle(pos, 0.3f);
        if (hit != null && hit.GetComponent<ObstacleCollision>() != null)
            return true;

        // Own body (screen boundaries are safe — snake wraps)
        return snake.IsNearSnake(pos, 0.4f);
    }
}
