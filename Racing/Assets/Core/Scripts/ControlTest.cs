using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTest : MonoBehaviour
{

    Controls _controls;

    private Vector2 _moveDirection;
  
    private PlayerInput _inputs;

    public float maxspeed;
    public float currentspeed;
    public float currentAcceleration;
    public float maxRotationSpeed = 1f;
    public CinemachineBrain brain;
    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;

    void Start()
    {

        _inputs = GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];

      

        move.started += OnMoveStarted;
        move.performed += OnMoveStarted;
        move.canceled+= OnMoveCanceled;

        InputAction CameraChange = _inputs.actions["ChangeCam"];

        CameraChange.performed += OnChangeCamera;
    }

    private void OnChangeCamera (InputAction.CallbackContext obj)
    {
        var currentCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
       
        if (currentCamera == camera1)
        {
            camera1.Priority= 0;
            camera2.Priority= 10;
        }
        else
        {
            camera1.Priority= 10;
            camera2.Priority= 0;
        }
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"Move started : {context.ReadValue<Vector2>()}");
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"Move performed : {context.ReadValue<Vector2>()}");
        _moveDirection = context.ReadValue<Vector2>();
     
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDirection = Vector2.zero;
    }
        

    // Update is called once per frame
    void Update()
    {
        //recupere the donnees of the movements
        float rotationAngle = _moveDirection.x;
        float acceleration = _moveDirection.y;

        // if we accelerate
        if (acceleration == 0)
        {
            //if were movin and decide to brake
            if (currentAcceleration>0)
            {
                currentAcceleration -= Time.deltaTime;
            }
            else //we backup
            {
                currentAcceleration += Time.deltaTime;
            }
          
        }
        else
        {
            //we progressively accelerate
            currentAcceleration+= acceleration*Time.deltaTime;
        }

        currentAcceleration = Mathf.Clamp(currentAcceleration, -1, 1);

        if (currentAcceleration >= 0)
        {
            currentspeed = Mathf.Lerp(0, maxspeed, currentAcceleration);
        }
        else
        {
            currentspeed = Mathf.Lerp(0, -maxspeed, -currentAcceleration);
        }

        //influence acceleration on rotation
        rotationAngle = rotationAngle* currentAcceleration* maxRotationSpeed* Time.deltaTime;

        transform.Rotate(0, rotationAngle, 0);
        transform.position = transform.position + transform.forward* (currentspeed *Time.deltaTime);
    }
}
