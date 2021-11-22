using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    public float thresholdUp;
    public float thresholdDown;

    public GameObject ballOrigin;
    public GameObject XRRig;
    public GameObject player;

    public bool pointToScore = true;

    public void FixedUpdate()
    {
        //if out of court
        if (transform.position.y < thresholdDown||
            transform.position.y > thresholdUp)
            respawn();
    }

    public void respawn()
    {
        pointToScore = true;
        player.transform.position = ballOrigin.transform.position;
        transform.position = ballOrigin.transform.position;
        Vector3 volleyballVector = XRRig.transform.position - transform.position;
        volleyballVector.y = volleyballVector.y + 9.81f;
        GetComponent<Rigidbody>().velocity = 0.6f * volleyballVector;
    }

}