using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private float sensitivity = 400f;

    private float xRotation;
    private float yRotation;
    private Transform cameraHolder;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraHolder = GameObject.FindGameObjectWithTag("CameraHolder").transform;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (cameraHolder)
        {
            cameraHolder.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
            Debug.Log("cameraHolder is not found");
    }
}
