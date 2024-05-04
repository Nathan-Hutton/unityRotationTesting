using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CustomInput input = null;
    private Vector3 direction = Vector2.zero;
    private Rigidbody rb = null;
    private float moveSpeed = 10f;

    private void Awake() 
    {
        input = new CustomInput();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable() 
    {
        input.Enable();
        input.Player.Movement.performed += OnPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
    }
    private void OnDisable() 
    {
        input.Disable();
        input.Player.Movement.performed -= OnPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
    }
    private void FixedUpdate()
    {
        rb.position += direction.normalized * moveSpeed * Time.fixedDeltaTime;
    }
    private void OnPerformed(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<Vector3>();
    }
    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        direction = Vector3.zero;
    }
}
