using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private ImputManager input = null;
    private Camera mainCamera;
    private Rigidbody rb;

    private float speed = 3f;
    private float runningSpeed = 5f;
    private bool running = false;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 mouseLook = Vector2.zero;
    private Vector3 rotationTarget;
    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        input = new ImputManager();
        mainCamera = Camera.main;
        rb = this.GetComponent<Rigidbody>();

        input.Player.Run.performed += x => OnRun();
        input.Player.Run.canceled += x => OnRunCancel();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCancel;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCancel;
    }

    private void OnMove(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMoveCancel(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;

    }

    private void OnMouseLook(InputAction.CallbackContext value)
    {
        mouseLook = value.ReadValue<Vector2>();
    }

    private void OnRun()
    {
        running = true;
    }

    private void OnRunCancel()
    {
        running = false;
    }

    private void FixedUpdate()
    {
        movePlayer();
        Aim();
    }

    public void movePlayer()
    {
        Vector3 movement = new Vector3(moveVector.x, 0, moveVector.y);

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedMovement = matrix.MultiplyPoint3x4(movement);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(skewedMovement), 0.09f);
        }

        if (running)
        {
            transform.Translate(skewedMovement * runningSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(skewedMovement * speed * Time.deltaTime, Space.World);
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hit.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success && moveVector == Vector2.zero)
        {
            var direction = position - transform.position;

            direction.y = 0;

            var rotation = Quaternion.LookRotation(direction);

            Vector3 aimDirection = new Vector3(rotationTarget.x, 0f, rotationTarget.z);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.07f);
        }
    }
}
