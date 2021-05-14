/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 DiverMovement v1.1
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{
    [Header("Player")]
    public CharacterController controller;
    [Tooltip("Main FPV camera")]
    public Camera viewCamera;

    [Header("Movement Parameters")]
    public bool godMode = false;
    [Tooltip("Player mass (higher = higher inertia)")]
    public float playerMass = 7f;
    [Tooltip("Water drag multiplier (higher = more drag)")]
    public float dragMultiplier = 4f;
    [Tooltip("Movement force multiplier (higher = more movement power)")]
    public float movementMultiplier = 20f;

    [Tooltip("Vertical force multiplier (gravity + buoyancy) — requires force generation on update")]
    public float verticalForceMultiplier = -0.8f;

    [Header("Water Parameters")]
    [Tooltip("Water current direction vector (not normalized) — requires force generation on update")]
    public Vector3 waterCurrentDirection = new Vector3 (1, 0, 0);
    [Tooltip("Water current velocity — requires force generation on update")]
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

        // Generate forces
        this.RegenerateForces();
    }


    void Update ()
    {
        // Get camera direction vectors
        Vector3 cameraForward = this.viewCamera .transform.forward;  // or Camera.main
        Vector3 cameraRight = this.viewCamera .transform.right;  // or Camera.main

        // Get player input
        float inputLateral = Input.GetAxis("Horizontal");  // lateral axis (left / right)
        float inputTangential = Input.GetAxis("Vertical");  // tangential axis (forwards / backwards)

        // Compute forces
        Vector3 dragForce = -1 * this.dragMultiplier * (this.velocity - this.waterVelocity);
        Vector3 movementForce = this.movementMultiplier * (cameraForward * inputTangential + cameraRight * inputLateral);



        if (this.godMode)
        {
            movementForce = 100f * (cameraForward * inputTangential + cameraRight * inputLateral);
        }

        // Integrate motion
        this.acceleration = (dragForce + this.verticalForce + movementForce) / this.playerMass;
        this.velocity += this.acceleration * Time.deltaTime;


        if (this.godMode)
        {
            this.acceleration = 500f * (cameraForward * inputTangential + cameraRight * inputLateral);
            this.velocity = this.acceleration * Time.deltaTime;
        }

        // Move player
        controller.Move (this.velocity * Time.deltaTime);
    }


    public void RegenerateForces ()
    {
        // Compute vertical force vector
        this.verticalForce = new Vector3 (0f, this.verticalForceMultiplier, 0f);

        // Compute water velocity vector
        this.waterVelocity = this.waterCurrentVelocity * Vector3.Normalize (this.waterCurrentDirection);
    }


    public void ResetMovement ()
    {
        this.velocity = Vector3.zero;
    }
}
