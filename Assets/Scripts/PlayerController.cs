using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    private PlayerActions _playerActions;
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothing;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerActions = new PlayerActions(_rigidbody, speed, sensitivity, smoothing);
    }

    private void OnEnable()
    {
        EntryPoint.myInputSystem.Player.Interact.performed += StartReading;
        EntryPoint.myInputSystem.Player.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        EntryPoint.myInputSystem.Player.Interact.performed -= Interact;
    }

    void ReadMove()
    {
        _moveInput = EntryPoint.myInputSystem.Player.Move.ReadValue<Vector2>();
    }
    
    void ReadLook()
    {
        _lookInput = EntryPoint.myInputSystem.Player.Look.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!EntryPoint.IsInputEnabled)
            return;
        _playerActions.Move(_moveInput);
        _playerActions.Rotate(_lookInput);
    }

    private void Update()
    {
        ReadMove();
        ReadLook();   
    }
    private void Interact(InputAction.CallbackContext context)
    {
        _playerActions.TryInteract();
    }

    private void StartReading(InputAction.CallbackContext context)
    {
        EntryPoint.myInputSystem.Player.Interact.performed -= StartReading;
    }
}
