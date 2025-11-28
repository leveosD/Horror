using UnityEngine;
using ItemModule;

public class PlayerActions
{
    private readonly Rigidbody _rigidbody;
    private readonly float _speed;
    private float _sensitivity;
    private float _smoothing;
    private readonly Transform _camera;
    private float _cameraAngle;
    private IInteractable _currentItem;

    private Vector2 _mouseLook; // Угол поворота камеры
    private Vector2 _smoothV; // Сглаженный вектор

    private GameObject _anchor;
    
    public PlayerActions(Rigidbody rigidbody, float speed, float sensitivity, float smoothing)
    {
        _rigidbody = rigidbody;
        _speed = speed;
        _sensitivity = sensitivity;
        _smoothing = smoothing;
        _camera = Camera.main.transform;
        _cameraAngle = 0;
        
        _anchor = new GameObject
        {
            name = "Anchor"
        };
        _anchor.transform.parent = _camera;
        _anchor.transform.localPosition = new Vector3(0, -0.1f, 0.5f);
    }

    public void Move(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0, input.y);
        direction = _rigidbody.transform.TransformDirection(direction);
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 desiredVelocity = direction * _speed;
        desiredVelocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = desiredVelocity;
    }

    public void Rotate(Vector2 input)
    {
        float mouseX = input.x * _sensitivity;
        float mouseY = input.y * _sensitivity;

        mouseX = Mathf.Lerp(_smoothV.x, mouseX, 1f / _smoothing);
        mouseY = Mathf.Lerp(_smoothV.y, mouseY, 1f / _smoothing);

        _smoothV = new Vector2(mouseX, mouseY);
        _mouseLook += _smoothV;

        Vector3 rotation = new Vector3(0f, _smoothV.x, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(rotation));

        float xRotation = -_mouseLook.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        _camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void TryInteract()
    {
        IInteractable item = null;
        if (Physics.Raycast(_camera.transform.position,
                _camera.forward, out var hit, 2.2f))
        {
            if (hit.transform.CompareTag("Interactable"))
            {
                hit.transform.gameObject.TryGetComponent(out item);
            }
        }

        item?.Interact(_anchor.transform);
        _currentItem = _currentItem is not null ? _currentItem.Activate(item) : item;
    }
}