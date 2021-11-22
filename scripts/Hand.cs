using System.Collections;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public CreateSendMessage message;
    public Volleyball ball;
    public Vector3 force = new Vector3();
    public Vector3 impulse = new Vector3();
    private float power; //impulse force
    public bool isLeft;
    public float pulseDuration;

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "Volleyball")
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), ball.GetComponent<Collider>(), true);
            force = collision.impulse / Time.fixedDeltaTime;
            Debug.Log("force: " + force);

            GetReflected(); //hit ball
            Debug.Log("ball reflected");

            //send haptic feedback
            StartCoroutine(sendPulse());
            Debug.Log("collison event exited");
        }
    }

    IEnumerator sendPulse()
    {
        //for testing, use one of the following message creator functions:

        /*
        // maps the force with callibration
        message.CreateMessage(force, isLeft); //create message
        yield return new WaitForSeconds(pulseDuration);
        message.CreateMessage(Vector3.zero, isLeft); //create message
        */

        // uses maximum callibration value, force not used and not mapped
        message.CreateMaxMessage(isLeft); //create message
        yield return new WaitForSeconds(pulseDuration);
        message.CreateMessage(Vector3.zero, isLeft); //create message
    }

    private void GetReflected() //to send the abll in right direction
    {
        power = GetPower();
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero; //ball stays above head
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Vector3 reflected = isLeft ? force : -force;
        rb.velocity = reflected.normalized * power;
    }

    private float GetPower()
    {
        float normal = Mathf.InverseLerp(20f, 82.9f, force.magnitude);
        float remap = Mathf.Lerp(8, 11, normal); //8 is set as minimum power for the ball the reach over the net, 11 is the maximum 
        Debug.Log("power: " + remap);
        return remap;
    }

}
