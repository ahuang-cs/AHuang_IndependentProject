using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 10f;
    public float directionChangeThreshold = 1f;

    private List<Vector3> randomDirections = new List<Vector3>();
    private Vector3 currentDirection;
    private Vector3 lastDirection;
    private bool bounceOffWall = false;

    // Start is called before the first frame update
    void Start()
    {
        initializeRandomDirections();
        changeToRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // change directions if there is a wall within the threshold
        if (Physics.Raycast(transform.position, currentDirection, out hit, directionChangeThreshold))
        {
            if(hit.transform.tag == "Wall")
            {
                changeToRandomDirection();
            }
        }

        if (bounceOffWall)
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
                changeToRandomDirection();
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

    private Vector3 getRandomDirection()
    {
        Vector3 randomDirection;
        do
        {
            randomDirection = randomDirections[Random.Range(0, randomDirections.Count)];
        } while (randomDirection == lastDirection);
        return randomDirection;
    }

    private void changeToRandomDirection()
    {
        lastDirection = currentDirection;
        currentDirection = getRandomDirection();
    }
    private void initializeRandomDirections()
    {
        randomDirections.Add(Vector3.forward);
        randomDirections.Add(Vector3.back);
        randomDirections.Add(Vector3.right);
        randomDirections.Add(Vector3.left);
    }
}
