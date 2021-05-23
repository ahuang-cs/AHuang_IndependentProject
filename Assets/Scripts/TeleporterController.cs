using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    public float teleportXOffset = 5f;

    private float teleportXLocation;

    // Start is called before the first frame update
    void Start()
    {
        if(name == "Teleporter Left")
        {
            teleportXLocation = GameObject.Find("Teleporter Right").transform.position.x - teleportXOffset;
        } else if (name == "Teleporter Right")
        {
            teleportXLocation = GameObject.Find("Teleporter Left").transform.position.x + teleportXOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.position = new Vector3(teleportXLocation, other.gameObject.transform.position.y, other.gameObject.transform.position.z);
    }
}
