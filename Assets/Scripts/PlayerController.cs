using Cysharp.Threading.Tasks;
using ItemModule;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //private CharacterController _characterController;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private MyInputSystem _myInputSystem;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    private PlayerActions _playerActions;
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothing;

    [SerializeField] private GameObject winWindow;
    [SerializeField] private GameObject loseWindow;

    private const float SprintK = 1.1f;
    private bool _isSprint = false;
    
    void Awake()
    {
        //_characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _myInputSystem = new MyInputSystem();
        _myInputSystem.Player.Enable();
        _playerActions = new PlayerActions(_rigidbody, _collider, speed, sensitivity, smoothing, GetComponentInChildren<ZeroItem>());
    }

    private void OnEnable()
    {
        _myInputSystem.Player.Interact.performed += Interact;
        //GhostfaceController.OnPlayersDeath += PlayersDeath;
        Game.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        _myInputSystem.Player.Interact.performed -= Interact;
        _myInputSystem.Player.Disable();
        //GhostfaceController.OnPlayersDeath -= PlayersDeath;
        Game.GameOver -= OnGameOver;
    }

    void ReadMove()
    {
        _moveInput = _myInputSystem.Player.Move.ReadValue<Vector2>();
    }
    
    void ReadLook()
    {
        _lookInput = _myInputSystem.Player.Look.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!_myInputSystem.Player.enabled)
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

    private async void OnGameOver(bool win)
    {
        _myInputSystem.Player.Disable();
        await UniTask.Delay(2500);
        if(win)
            winWindow.SetActive(true);
        else
            loseWindow.SetActive(true);
    }
}
