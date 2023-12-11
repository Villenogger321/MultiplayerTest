using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerCameraMovement : NetworkBehaviour
{
    public float Sensitivity;

    Vector2 mouseMovement = new Vector2();
    float xRotation;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
        {
            this.GetComponent<Camera>().enabled = false;
            this.GetComponent<AudioListener>().enabled = false;
            this.GetComponent<PlayerInput>().enabled = false;
            this.enabled = false;
        }
            
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        CameraMovement();
    }

    void CameraMovement()
    {
        transform.parent.Rotate(Vector3.up, mouseMovement.x * Time.deltaTime * Sensitivity);

        xRotation -= mouseMovement.y * Time.deltaTime * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -85, 85);
        Vector3 targetRotation = transform.parent.eulerAngles;
        targetRotation.x = xRotation;
        transform.eulerAngles = targetRotation;
    }
    void OnLook(InputValue _value)
    {
        mouseMovement = _value.Get<Vector2>();
    }
}
