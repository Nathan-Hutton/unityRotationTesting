using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private CustomInput input = null;
    private Vector3 direction = Vector2.zero;
    private Vector2 lookDirection = Vector2.zero;
    private Rigidbody rb = null;
    private Transform cameraTransform = null;
    private float currentCameraPitch;
    private float currentCameraYaw;
    private float cameraDistance;
    private Vector3[] rayCasters = new Vector3[5];
    public float moveSpeed = 10f;
    public float lookSpeed = 20f;
    public float shockwaveRadius = 10f;
    public float shockwaveForce = 10f;

    private void Awake() 
    {
        input = new CustomInput();
        rb = GetComponent<Rigidbody>();
        cameraTransform =   GetComponentInChildren<Camera>().GetComponent<Transform>();
        currentCameraPitch = cameraTransform.rotation.x;
        currentCameraYaw = cameraTransform.rotation.y;
        cameraDistance = (transform.position - cameraTransform.position).magnitude;

        rayCasters[0] = Vector3.down * 0.5f;
        rayCasters[1] = new Vector3(0, -0.5f, 0.5f);
        rayCasters[2] = new Vector3(0, -0.5f, -0.5f);
        rayCasters[3] = new Vector3(0, 0.5f, 0.5f);
        rayCasters[4] = new Vector3(0, 0.5f, -0.5f);
    }
    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
        input.Player.ShockWave.performed += OnShockwavePerformed;
        input.Player.Look.performed += OnLookPerformed;
        input.Player.Look.canceled += OnLookCancelled;
        input.Player.Jump.performed += OnJumpPerformed;
    }
    private void OnDisable() 
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
        input.Player.ShockWave.performed -= OnShockwavePerformed;
        input.Player.Look.performed -= OnLookPerformed;
        input.Player.Look.canceled -= OnLookCancelled;
    }
    private void Update()
    {
        // ***********
        // Camera
        // ***********
        float yaw = lookDirection.x * lookSpeed * Time.deltaTime;
        currentCameraYaw += yaw;

        float pitch = lookDirection.y * lookSpeed * Time.deltaTime;
        currentCameraPitch -= pitch;
        currentCameraPitch = Mathf.Clamp(currentCameraPitch, 0f, 80f);

        // Calculate camera's new position relative to player using sphereical coordinates
        Vector3 offset = new(0, 0, -cameraDistance);
        offset = Quaternion.Euler(currentCameraPitch, currentCameraYaw, 0) * offset;

        cameraTransform.position = transform.position + offset;
        cameraTransform.LookAt(transform);

        // ****************
        // Player RigidBody
        // ****************
        transform.Rotate(0, yaw, 0);
    }
    private void FixedUpdate()
    {
        rb.position += rb.transform.TransformDirection(direction.normalized) * moveSpeed * Time.fixedDeltaTime;
    }
    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<Vector3>();
    }
    private void OnLookPerformed(InputAction.CallbackContext value)
    {
        lookDirection = value.ReadValue<Vector2>();
    }
    private void OnLookCancelled(InputAction.CallbackContext value)
    {
        lookDirection = Vector2.zero;
    }
    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        direction = Vector3.zero;
    }
    private void OnShockwavePerformed(InputAction.CallbackContext value)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, shockwaveRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb == null)
                continue;

            Vector3 direction = rb.transform.position - transform.position;
            rb.AddForce(direction.normalized * shockwaveForce, ForceMode.Impulse);
        }
    }
    private void OnJumpPerformed(InputAction.CallbackContext value)
    {
        if (rayCasters.AsQueryable().Any(ray => Physics.Raycast(transform.position, ray, 1f, -1)))
            rb.AddForce(new Vector3(0,1,0) * 10, ForceMode.Impulse);
    }
}
