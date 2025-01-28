using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotation : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Adjusts the speed of mouse movement
    private float xRotation = 0f;        // Tracks vertical rotation
    private float yRotation = 0f;        // Tracks Horizontal rotation

    public float topclamp = -90;
    public float bottomclamp = 90;

    void Start()
    {
        // Locks the cursor in the center and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust vertical rotation (inverted Y-axis for natural FPS feel)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topclamp, bottomclamp); // Limits camera up/down angle

       // rotation around the Y axis (Look Left & Right)
         yRotation += mouseX;

        // Apply rotation to camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

    }
}