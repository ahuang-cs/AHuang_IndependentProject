using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float levelUpSeconds = 30f;

    private NavMeshController navMeshController;
    private NavMeshAgent agent;
    private Vector3 lastPosition;

    private int currentLevel = 0;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //initializeRandomDirections();
        navMeshController = GameObject.Find("NavMeshController").GetComponent<NavMeshController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(levelUp());
    }

    IEnumerator levelUp()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(levelUpSeconds);
            currentLevel++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((!agent.pathPending && agent.remainingDistance < 0.5f) || Vector3.Distance(lastPosition,transform.position) < 0.01f )
        {
            if (currentLevel > 0)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                agent.SetDestination(navMeshController.getRandomWayPoint(transform.position));
            }
        }
        lastPosition = transform.position;
    }
}
