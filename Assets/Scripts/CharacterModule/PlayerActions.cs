using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ItemModule;
    public class PlayerActions
    {
        //private CharacterController _characterController;
        private readonly Rigidbody _rigidbody;
        private readonly CapsuleCollider _collider;
        private readonly float _speed;
        private float _sensitivity;
        private float _smoothing;
        private readonly Camera _camera;
        private float _cameraAngle;
        private IInteractable _previousItem;
        private readonly IInteractable _zeroItem;
        
        private Vector2 _mouseLook; // Угол поворота камеры
        private Vector2 _smoothV;   // Сглаженный вектор
        
        public PlayerActions(Rigidbody rigidbody, CapsuleCollider collider, float speed, float sensitivity, float smoothing, IInteractable zeroItem)
        {
//            _characterController = characterController;
            _rigidbody = rigidbody;
            _collider = collider;
            _speed = speed;
            _sensitivity = sensitivity;
            _smoothing = smoothing;
            _camera = Camera.main;
            _cameraAngle = 0;
            _zeroItem = zeroItem;
            _previousItem = zeroItem;
        }

        public void Move(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }
            
            Vector3 direction = new Vector3(input.x, 0, input.y);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = _rigidbody.transform.TransformDirection(direction);
            
            /*List<RaycastHit> hits = _rigidbody.SweepTestAll(direction, 0.05f).ToList();
            if (hits.Count != 0)
            {
                int i = 1;
                foreach (var hit in hits)
                {
                    Debug.Log($"{i++} {hit.transform.gameObject.name}");
                    if (hit.transform.gameObject.layer != 7)// && hit.distance > 0.015f)
                    {
                        return;
                    }
                }
                Debug.Log("--------------------------");
            }*/
            _rigidbody.MovePosition(_rigidbody.transform.position + direction * (_speed * Time.fixedDeltaTime));
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
            _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        public void TryInteract()
        {
            IInteractable item = null;
            if (Physics.Raycast(_camera.transform.position, 
                    _camera.transform.forward, out var hit, 2.2f))
            {
                if (hit.transform.CompareTag("Interactable"))
                {
                    hit.transform.gameObject.TryGetComponent(out item);
                }
            }
            
            /*if(item != null)
                Debug.Log(item);
            else
                Debug.Log("null");
            if(_previousItem != null)
                Debug.Log(_previousItem);
            else
                Debug.Log("null");*/
            
            var temp = item?.Interact(_previousItem ?? _zeroItem) ?? _zeroItem;
            bool activated = _previousItem?.Activate(item) ?? false;
            if (activated)
                _previousItem = temp;

        }
    }