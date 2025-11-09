using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class TakeableItem : MonoBehaviour, IInteractable
    {
        protected Rigidbody itemRigidbody;
        //protected Collider itemCollider;
        protected bool IsTaking = false;
        public bool IsHandled = false;
        
        public Transform Transform => transform;

        protected AudioSource audioSource;
        [SerializeField] protected AudioClip[] clips;
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float moveSpeed = 5.0f;
    
        private Vector3 _smoothV;   // Сглаженный вектор 
        [SerializeField] private float smoothing;
    
        private Collider _collider;

        private Vector3 _lastTarget;
        private Vector3 _target;

        [SerializeField] private Transform player;

        private Vector3 offset = new Vector3(0, -0.2f, 0.5f);
        private Vector3 _norm;

        private void Start()
        {
            itemRigidbody = GetComponent<Rigidbody>();
            TryGetComponent(out audioSource);
            _collider = GetComponent<Collider>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        public virtual IInteractable Interact(IInteractable item)
        {
            if (!IsTaking)
            {
                //IsTaking = true;
                itemRigidbody.useGravity = false;
                //itemCollider.isTrigger = true;

                transform.parent = mainCamera.transform;
                transform.DOLocalMove(offset, 0.5f)
                    .SetEase(Ease.InOutSine).onComplete += () =>
                {
                    IsTaking = false;
                    IsHandled = true;
                    transform.parent = null;
                };
                transform.DORotate(new Vector3(-90, 0, 0), 0.5f)
                    .SetEase(Ease.InOutSine);
                
                audioSource.clip = clips[0];
                audioSource.Play();
                
                //TransformWithParent();
            }

            return this;
        }

        public virtual bool Activate(IInteractable item)
        {
            if(/*!transform.parent || */IsTaking)
                return false;
            DOTween.Kill(gameObject);
            
            var direction = mainCamera.transform.forward;
            
            transform.parent = null;
            //itemRigidbody.isKinematic = false;
            IsHandled = false;
            itemRigidbody.useGravity = true;
            
            //_audioSource.Play();

            if (item == null)
                itemRigidbody.AddForce(direction * 12f, ForceMode.Impulse);
            
            return true;
        }

        private /*async*/ void FixedUpdate()
        {
            /*Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2f, Color.red);
            Debug.DrawLine(player.transform.position, transform.TransformPoint(-player.transform.forward) * 2f, Color.blue);*/
            if (IsHandled)
            {
                /*var direction = (Camera.main.transform.position + Camera.main.transform.forward - transform.position)
                    .normalized;
                itemRigidbody.MovePosition((Camera.main.transform.position  + direction) *
                                           Time.fixedDeltaTime);
                Debug.Log("Nre pos: " + transform.position + " New local pos: " + transform.localPosition);*/
                //await UniTask.Yield();
                
                var pos = transform.position;
                var camPos = mainCamera.transform.position;
                var forward = mainCamera.transform.forward;
                _target = mainCamera.transform.TransformPoint(offset);
                Debug.Log($"Position: {pos} Camera: {camPos} Target: {_target}");
                //camPos + forward / 1.8f + Vector3.down / 4;

                if (_target == _lastTarget)
                    return;
                
                if (Vector3.Distance(pos, camPos + forward) > 0.1f)
                {
                    _norm = (pos - (camPos)).normalized;
                }
                else if (camPos + forward == _lastTarget)
                {
                    _norm = Vector3.zero;
                }

                float mouseX = -_norm.x * moveSpeed;
                float mouseY = -_norm.y * moveSpeed;
                float mouseZ = -_norm.z * moveSpeed;

                mouseX = Mathf.Lerp(_smoothV.x, mouseX, 1f / smoothing);
                mouseY = Mathf.Lerp(_smoothV.y, mouseY, 1f / smoothing);
                mouseZ = Mathf.Lerp(_smoothV.z, mouseZ, 1f / smoothing);

                _smoothV = new Vector3(mouseX, mouseY, mouseZ);
                
                Vector3 half = new Vector3()
                {
                    x = _collider.bounds.max.x - _collider.bounds.center.x,
                    y = _collider.bounds.max.y - _collider.bounds.center.y,
                    z = _collider.bounds.max.z - _collider.bounds.center.z
                };
                Collider[] colliders = new Collider[5];
                if (Physics.OverlapBoxNonAlloc(_target + _smoothV * Time.fixedDeltaTime, 
                        half, colliders, Quaternion.identity, ~_collider.excludeLayers) != 0)
                {
                    //Debug.Log($"{colliders[0].transform.gameObject.name}");
                    return;
                }
                
                itemRigidbody.MovePosition(_target + _smoothV * Time.fixedDeltaTime);
                
                _lastTarget = _target;
            }
        }
    }
}