using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class DiverMovement : MonoBehaviour
{

    [Header("Player object")]
    public CharacterController controller;

    [Header("Simulation parameters")]
    [Range(0,10)]
    public float speed = 4.5f;

    Vector3 cameraForward;
    Vector3 cameraRight;

    Vector3 velocity;
    Vector3 position;

    // Update is called once per frame
    void Update()
    {

        // TODO: child camera instead
        cameraForward = Camera.main.transform.forward;
        cameraRight = Camera.main.transform.right;

        float inputLateral = Input.GetAxis("Horizontal");  // lateral axis (left / right)
        float inputTangential = Input.GetAxis("Vertical");  // tangential axis (forwards / backwards)

        Vector3 positionDelta = cameraForward * inputTangential + cameraRight * inputLateral;


        // Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(positionDelta * speed * Time.deltaTime);
        // controller.Move(velocity * Time.deltaTime);


        // Vec = transform.localPosition;  
        // Vec.y += Input.GetAxis("Jump") * Time.deltaTime * 20;  
        // Vec.x += Input.GetAxis("Horizontal") * Time.deltaTime * 20;  
        // Vec.z += Input.GetAxis("Vertical") * Time.deltaTime * 20;  
        // transform.localPosition = Vec;  
    }


    // Vector3 pos;
    // public int speed = 1;

    // // Use this for initialization  
    // void Start () {
    //     pos = Input.mousePosition;
    //     // pos.z = -0.1f;
    //     pos = Camera.main.ScreenToWorldPoint(pos);
    //     transform.LookAt(pos);
    // }

    // // Update is called once per frame
    // void Update () {
    //     pos = Input.mousePosition;
    //     // pos.z = -0.1f;
    //     pos = Camera.main.ScreenToWorldPoint(pos);
    //     transform.LookAt(pos);
    // }

    // public void FixedUpdate ()
    // {
    //     transform.Translate(transform.forward * speed * Time.deltaTime);
    // }
}