using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshController : MonoBehaviour
{
    public float minDistance = 20f;

    private List<Vector3> wayPoints = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject intersection in GameObject.FindGameObjectsWithTag("Intersection"))
        {
            wayPoints.Add(intersection.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getRandomWayPoint(Vector3 currentPosition)
    {
        Vector3 randomWayPoint;
        do
        {
            randomWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];
        } while (Vector3.Distance(currentPosition, randomWayPoint) < minDistance);

        return randomWayPoint;
    }
}
