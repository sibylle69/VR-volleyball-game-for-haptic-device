using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]

public class NewPlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private InputActionReference randomThrowActionReference;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private float jumpForce = 500.0f;

    public GameObject volleyball;
    public GameObject ballOrigin;
    public GameObject XRRig;
    public GameObject player;
    public Respawn resp;
    public float distanceToBall;

    private CapsuleCollider _collider;
    private XRRig _xrRig;
    private Rigidbody _body;

    private bool IsGrounded => Physics.Raycast(
        new Vector2(transform.position.x, transform.position.y + 2.0f),
        Vector3.down, 2.0f);


    void Start()
    {
        _xrRig = GetComponent<XRRig>();
        _body = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        jumpActionReference.action.performed += OnJump;
        randomThrowActionReference.action.performed += OnRandomThrow;
        moveActionReference.action.performed += OnMove;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
      //if (!IsGrounded) return;
        _body.AddForce(Vector3.up * jumpForce);
    }

    private void OnRandomThrow(InputAction.CallbackContext obj)
    {

        //set volleyball position with player
        Vector3 pos = new Vector3(player.transform.position.x, 2.3f, player.transform.position.z);
        volleyball.transform.position = pos;

        RandomThrow();
    }

    private void RandomThrow()
    {
        //new point to score
        resp.pointToScore = true;
        
        //throw a random ball around player
        float xPos = Camera.main.transform.position.x + Random.Range(-distanceToBall, distanceToBall);
        float zPos = Camera.main.transform.position.z + Random.Range(-distanceToBall, distanceToBall);

        Debug.Log(xPos);
        Debug.Log(zPos);


        Vector3 targetPosition = new Vector3(xPos, 9.81f, zPos);
        Vector3 volleyballVector = targetPosition - volleyball.transform.position;

        volleyball.GetComponent<Rigidbody>().velocity = 0.8f * volleyballVector;
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 direction = moveActionReference.action?.ReadValue<Vector2>() ?? Vector2.zero; //thumbstick vector
        Vector3 target = new Vector3(direction.x, direction.y, 0);
        //Debug.Log("Thumbstick direction: " + target);
        target = new Vector3(target.x, 0, target.y);
        target = transform.TransformDirection(target);
        XRRig.transform.position +=  1.2f *target.normalized;
    }

    void Update()
    {
        var center = _xrRig.cameraInRigSpacePos; //current position of the headmounted camera display
        _collider.center = new Vector3(center.x, _collider.center.y, center.z);
        _collider.height = _xrRig.cameraInRigSpaceHeight;

        //prevent XR rig to fall
        if (transform.position.y < 0)
        {
            Vector3 newpos = transform.position;
            newpos.y = 0f;
            transform.position = newpos;
        }

        //keep opponent grounded at all times
        Vector3 PlayerNewpos = player.transform.position;
        PlayerNewpos.y = 0f;
        player.transform.position = PlayerNewpos;

        //keep player from getting in the net
        if (player.transform.position.z < 1f)
        {
            Vector3 newpos = player.transform.position;
            newpos.z = 1f;
            player.transform.position = newpos;
        }
    }



}