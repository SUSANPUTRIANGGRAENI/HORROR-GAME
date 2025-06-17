using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform playerBody; // drag objek Player ke sini di Inspector
    public float mouseSensitivity = 100f;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // kunci kursor ke tengah layar
    }

    void Update()
    {
        // Ambil input mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotasi vertikal kamera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // agar tidak bisa muter 360 derajat ke atas

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // rotasi kamera

        // Rotasi horizontal player
        playerBody.Rotate(Vector3.up * mouseX); // rotasi player (horizontal)
    }
}

