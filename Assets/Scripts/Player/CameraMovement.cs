/*
 Copyright Anatole Hernot, 2021
 All rights reserved

 CameraMovement v1.1
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [Header("Mouse Settings")]
    public float mouseXSensitivity = 150f;
    public float mouseYSensitivity = 100f;
    [Header("Player object")]
    public Transform playerBody;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
