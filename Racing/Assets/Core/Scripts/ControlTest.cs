using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTest : MonoBehaviour
{

    Controls _controls;

    private Vector2 _moveDirection;

    [SerializeField] AudioSource bikeNoises, bell;

    private PlayerInput _inputs;
    private Rigidbody _body;

    public float _accelerationRate = 1, _maxSpeed, _currentSpeed, _currentAcceleration, _maxRotationSpeed = 1, _wheelRotationFactor = 5;
    private Vector3 _wheelRotation;

    public CinemachineBrain _brain;
    public CinemachineVirtualCamera _camera1, _camera2;

    public Transform _wheelFront,  _wheelRear;

    private float _rotationAngle;

    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;

        InputAction cameraChange = _inputs.actions["ChangeCamera"];

        cameraChange.performed += OnChangeCamera;

        _body = GetComponentInChildren<Rigidbody>();

    }


    #region Inputs

    private void OnChangeCamera(InputAction.CallbackContext obj)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if (currentCamera == _camera1)
        {
            _camera1.Priority = 0;
            _camera2.Priority = 10;
        }
        else
        {
            _camera1.Priority = 10;
            _camera2.Priority = 0;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        _moveDirection = Vector2.zero;
        bikeNoises.Stop();
    }

    public void OnMovePerformed(InputAction.CallbackContext context)

    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
 bikeNoises.Play();
        _moveDirection = context.ReadValue<Vector2>();
    }

    #endregion

    /// <summary>
    /// Gestion de la physique
    /// </summary>
    void FixedUpdate()
    {
        _body.AddForce(transform.forward * _currentSpeed);
    }

    // Update is called once per frame
    void Update()
    {

        bellSound();
        // Récupère les données de mouvement
        float rotationAngle = _moveDirection.x;
        float acceleration = _moveDirection.y;

        // Si on accélère pas (laché)
        if (acceleration == 0)
        {
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, 0, Time.deltaTime * _accelerationRate);
          
        }
        else if (acceleration < 0)
        {
            //Si on est en train d'avancer, on freine
            if (_currentAcceleration > 0)
            {
                _currentAcceleration -= Time.deltaTime;
            }
            else // On recule
            {
                _currentAcceleration += acceleration * Time.deltaTime;
            }
        }
        else
        {
            // On accélère progressivement
            _currentAcceleration += acceleration * Time.deltaTime;
        }

        _currentAcceleration = Mathf.Clamp(_currentAcceleration, -1, 1);

        if (_currentAcceleration >= 0)
        {
            _currentSpeed = Mathf.Lerp(0, _maxSpeed, _currentAcceleration);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(0, -_maxSpeed, -_currentAcceleration);
        }


        RotateWheels(rotationAngle);

        // Influence accelerations sur la rotation
        _rotationAngle = rotationAngle * _currentAcceleration * _maxRotationSpeed;

        // Rotation du player.
        // /!\ N'utilise pas la physique pour la rotation, mais simplification ok pour nos besoins
        transform.Rotate(0, _rotationAngle * Time.deltaTime, 0);
    }

    private void RotateWheels(float rotationAngle)
    {
        _wheelRotation.x += _currentSpeed * Time.deltaTime * _wheelRotationFactor;

        if (rotationAngle != 0)
        {
            _wheelRotation.y = Mathf.Clamp(_wheelRotation.y + rotationAngle * Mathf.Sign(_currentSpeed), -30, 30);
        }
        else
        {
            _wheelRotation.y = Mathf.Lerp(_wheelRotation.y, 0, Time.deltaTime);
        }

        _wheelFront.localEulerAngles = _wheelRotation;

        Vector3 rearRotation = _wheelRotation;
        rearRotation.y = 0;

        _wheelRear.localEulerAngles = rearRotation;
    }

    private void bellSound()
    {
        if (Input.GetKey(KeyCode.E)) 
        {
           bell.Play();
        }
    }


}