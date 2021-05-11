/*
 Copyright Anatole Hernot, 2021
 All rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{

    public CharacterController controller;

    [Header("Movement parameters")]
    [Range(0,10)]
    public float speed = 4.5f;

    Vector3 cameraForward;
    Vector3 cameraRight;

    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    Vector3 position;


    void Update()
    {

        // TODO: child camera instead
        cameraForward = Camera.main.transform.forward;
        cameraRight = Camera.main.transform.right;

        float inputLateral = Input.GetAxis("Horizontal");  // lateral axis (left / right)
        float inputTangential = Input.GetAxis("Vertical");  // tangential axis (forwards / backwards)

        //
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
}
