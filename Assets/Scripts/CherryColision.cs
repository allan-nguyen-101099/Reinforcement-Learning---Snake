using UnityEngine;

public class CherryCollision : MonoBehaviour
{
    public CherryController controller;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Cherry eaten!");
            controller.OnCherryEaten();
        }
    }
}
