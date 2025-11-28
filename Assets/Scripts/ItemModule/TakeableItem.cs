using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class TakeableItem : MonoBehaviour, IInteractable
    {
        protected Rigidbody itemRigidbody;
        
        protected bool IsTaking = false;
        public bool IsHandled = false;
        
        public Transform Transform => transform;

        protected AudioSource audioSource;
        [SerializeField] protected AudioClip[] clips;

        [SerializeField] protected Transform anchor;
        public Transform Anchor => anchor;

        [SerializeField] protected int speed;

        private void Start()
        {
            itemRigidbody = GetComponent<Rigidbody>();

            TryGetComponent(out audioSource);
        }

        public virtual IInteractable Interact(Transform parent)
        {
            if (!IsHandled)
            {
                itemRigidbody.useGravity = false;
                IsHandled = true;              
                itemRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                transform.DORotate(new Vector3(-90, 0, 0), 0.5f)
                    .SetEase(Ease.InOutSine);
                audioSource.clip = clips[0];
                audioSource.Play();

                anchor = parent;
            }

            return this;
        }

        public virtual IInteractable Activate(IInteractable item)
        {
            if (item != null)
            {
                if (item.Transform.gameObject == this.gameObject)
                    return this;
            }

            if (item is TakeableItem or null)
            {
                DOTween.Kill(gameObject);
                
                itemRigidbody.constraints = RigidbodyConstraints.None;
                IsHandled = false;
                itemRigidbody.useGravity = true;
                anchor = null;

                if (item == null)
                {
                    itemRigidbody.AddForce(Camera.main.transform.forward * 12f, ForceMode.Impulse);
                    return null;
                }

                return item;
            }
            
            return this;
        }

        private void FixedUpdate()
        {
            if (IsHandled)
            {
                Vector3 direction = anchor.position - transform.position;
                itemRigidbody.linearVelocity = direction * speed * Time.fixedDeltaTime;
            }
        }
    }
}