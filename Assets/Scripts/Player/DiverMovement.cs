/*
 Copyright Anatole Hernot, 2021
 All rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverMovementNew : MonoBehaviour
{
    // Character controller
    public CharacterController controller;

    [Header("Movement Parameters")]
    public float playerMass = 7f;
    public float dragMultiplier = 4f;
    public float movementMultiplier = 20f;

    [Tooltip("Vertical force multiplier (gravity + buoyancy)")]
    public float verticalForceMultiplier = -3f;

    [Header("Water Parameters")]
    public Vector3 waterCurrentDirection = new Vector3 (1, 0, 0);
    public float waterCurrentVelocity = 0.15f;

    [Header("Instantaneous Movement")]
    [SerializeField]
    Vector3 acceleration;
    [SerializeField]
    Vector3 velocity;

    Vector3 verticalForce;
    Vector3 waterVelocity;


    void Start ()
    {
        // Initialise player velocity
        this.velocity = new Vector3 ();

        // Compute vertical force vector
        this.verticalForce = new Vector3 (0f, this.verticalForceMultiplier, 0f);

        // Compute water velocity vector
        this.waterVelocity = this.waterCurrentVelocity * Vector3.Normalize (this.waterCurrentDirection);
    }


    void Update()
    {

        // Get camera direction
        // TODO: use child camera instead
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Get player input
        float inputLateral = Input.GetAxis("Horizontal");  // lateral axis (left / right)
        float inputTangential = Input.GetAxis("Vertical");  // tangential axis (forwards / backwards)

        // Compute forces
        Vector3 dragForce = -1 * this.dragMultiplier * (this.velocity - this.waterVelocity);
        Vector3 movementForce = this.movementMultiplier * (cameraForward * inputTangential + cameraRight * inputLateral);

        // Integrate motion
        this.acceleration = (dragForce + this.verticalForce + movementForce) / this.playerMass;
        this.velocity += this.acceleration * Time.deltaTime;

        // Move player
        controller.Move (this.velocity * Time.deltaTime);
    }
}
