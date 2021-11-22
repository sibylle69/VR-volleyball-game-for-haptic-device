using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Volleyball : MonoBehaviour
{
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    public GameObject XRRig;
    public GameObject ballOrigin;
    public GameObject myScoreObj;
    public GameObject HisScoreObj;
    public GameObject player;
    public GameObject RHand;
    public GameObject LHand;


    public Respawn resp;

    private float MyScore;
    private float HisScore;
    private bool Myside;
    private bool inCourt;
    private bool ignoreCollision = false;
    private bool netHit = false;
    private Vector3 lastBallPosition;

    void Start()
    {
        MyScore = 0;
        HisScore = 0;

        //ball set up
        resp.pointToScore = true;
        transform.position = ballOrigin.transform.position;
        Vector3 volleyballVector = XRRig.transform.position - transform.position;
        volleyballVector.y = volleyballVector.y + 9.81f;
        GetComponent<Rigidbody>().velocity = 0.6f *volleyballVector;

        //player set-up
        Vector3 initialpos = new Vector3(ballOrigin.transform.position.x, 0, ballOrigin.transform.position.z);
        player.transform.position = initialpos;

        //ignore some collisions
        Physics.IgnoreCollision(RHand.GetComponent<Collider>(), LHand.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(RHand.GetComponent<Collider>(), XRRig.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(LHand.GetComponent<Collider>(), XRRig.GetComponent<Collider>(), true);
    }

    //the court is a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "court" && resp.pointToScore)
        {
            //Debug.Log("ball in");
            if (Myside)
            {
                HisScore += 1;
                resp.pointToScore = false;
                AudioSource.PlayClipAtPoint(loseSound, Camera.main.transform.position);
            }

            else
            {
                //move the player to the ball
                //player.transform.position = Vector3.Lerp(player.transform.position, transform.position, 1f);
                Vector3 newpos = new Vector3(transform.position.x, 0, transform.position.z);
                player.transform.position = newpos;

                Vector3 ballToPlayer = player.transform.position - transform.position;
                if (ballToPlayer.magnitude < 4 && !netHit)
                {

                    //send the ball back or score
                    Vector3 volleyballVector = XRRig.transform.position - transform.position;
                    volleyballVector.y = volleyballVector.y + 10f;
                    GetComponent<Rigidbody>().velocity = 0.8f * volleyballVector;
                }
                else
                {
                    MyScore += 1;
                    resp.pointToScore = false;
                    AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
                    netHit = false;
                }
            }

            ignoreCollision = true; //needed to ignore the cube collision underneath the court
 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
        }

        //ball out
        if (collision.gameObject.name == "Cube" && resp.pointToScore)
        {
            if (!ignoreCollision)
            {
                //Debug.Log("ball out");
                if (Myside)
                {
                    //Debug.Log("in my side");
                    MyScore += 1;
                    AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
                }
                else
                {
                    //Debug.Log("in his side");
                    HisScore += 1;
                    AudioSource.PlayClipAtPoint(loseSound, Camera.main.transform.position);
                }

                //point already scored
                resp.pointToScore = false;
            }

            ignoreCollision = false;
            netHit = false;
        }

        if (collision.gameObject.name == "net")
        {
            netHit = true;
        }

    }
    void Update()
    {

        if (myScoreObj.GetComponentInChildren<TextMeshProUGUI>() != null &&
            HisScoreObj.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            myScoreObj.GetComponentInChildren<TextMeshProUGUI>().text = MyScore.ToString();
            HisScoreObj.GetComponentInChildren<TextMeshProUGUI>().text = HisScore.ToString();
        }

        //if ball on my side,
        if(transform.position.z < 0)
        {
            Myside = true;
        }

        else
        {
            Myside = false;
        }
        Physics.IgnoreCollision(GetComponent<Collider>(), RHand.GetComponent<Collider>(), false);
        Physics.IgnoreCollision(GetComponent<Collider>(), LHand.GetComponent<Collider>(), false);

    }
}
