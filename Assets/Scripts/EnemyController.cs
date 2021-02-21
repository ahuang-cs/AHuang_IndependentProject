using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 10f;

    private List<Vector3> randomDirections = new List<Vector3>();
    private Vector3 currentDirection;
    private Vector3 lastDirection;
    private bool bounceOffWall = false;

    // Start is called before the first frame update
    void Start()
    {
        initializeRandomDirections();
        changeDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if(bounceOffWall)
        {
            transform.Translate(-lastDirection * speed * Time.deltaTime);
        } else
        {
            transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                bounceOffWall = true;
                changeDirection();
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                bounceOffWall = false;
                break;
        }
    }

    private void changeDirection()
    {
        lastDirection = currentDirection;
        Vector3 randomDirection;
        do {
            randomDirection = randomDirections[Random.Range(0, randomDirections.Count - 1)];
        } while(randomDirection == lastDirection); 

        currentDirection = randomDirection;
    }
    private void initializeRandomDirections()
    {
        randomDirections.Add(Vector3.forward);
        randomDirections.Add(Vector3.back);
        randomDirections.Add(Vector3.right);
        randomDirections.Add(Vector3.left);
    }
}
