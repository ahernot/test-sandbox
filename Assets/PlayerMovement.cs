using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class PlayerMovement : MonoBehaviour
{

    [Header("Player object")]
    public CharacterController controller;

    [Header("Simulation parameters")]
    [Range(0,10)]
    public float speed = 4.5f;
    public float gravity = -15f; //-9.81f;
    public float jumpHeight = 3f;

    [Header("Ground check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask; // control which objects are ground

    Vector3 velocity;

    [SerializeField]
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Debug.Log(groundCheck.position);
        // bool isGrounded = IsGrounded();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f; //-3f; // force player down on the ground, since sphere radius
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        if (!isGrounded) {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

}
