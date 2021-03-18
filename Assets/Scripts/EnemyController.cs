using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public GameObject followTarget;

    private NavMeshController navMeshController;
    private NavMeshAgent agent;
    private Vector3 lastPosition;


    //public float speed = 10f;
    //public float directionChangeThreshold = 3f;

    // private List<Vector3> randomDirections = new List<Vector3>();
    // private Vector3 currentDirection;
    // private Vector3 lastDirection;
    // private bool bounceOffWall = false;

    // private Collider[] colliders;
    // private Vector3 searchExtents = new Vector3(2f,0.5f,2f);

    // Start is called before the first frame update
    void Start()
    {
        //initializeRandomDirections();
        navMeshController = GameObject.Find("NavMeshController").GetComponent<NavMeshController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((!agent.pathPending && agent.remainingDistance < 0.5f) || Vector3.Distance(lastPosition,transform.position) < 0.01f )
        {
            agent.SetDestination(navMeshController.getRandomWayPoint(transform.position));
        }
        lastPosition = transform.position;
        // setRandomDirection();
        // transform.Translate(currentDirection * speed * Time.deltaTime);
        /*
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
        */
    }
    /*
    private void setRandomDirection()
    {
        List<Vector3> availableDirections = new List<Vector3>();
        foreach(Vector3 direction in randomDirections)
        {
            // don't go backwards
            if (direction == -currentDirection) continue;

            RaycastHit hit;
            if ((!Physics.Raycast(transform.position, direction, out hit) || hit.transform.tag != "Wall") && direction != -currentDirection)
            {
                availableDirections.Add(direction);
            }
        }
        if(availableDirections.Count > 0)
        {
            lastDirection = currentDirection;
            currentDirection = availableDirections[Random.Range(0, availableDirections.Count)];
        }
    }
    */

    /*
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
    */
}
