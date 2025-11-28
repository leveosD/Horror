using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class Door : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;
        protected bool isOpened = false;
        
        protected AudioSource audioSource;
        [SerializeField] protected AudioClip[] clips;

        protected float angle = 85f;

        [SerializeField] private int doorDirection;
        public int DoorDirection => doorDirection;
        public int k;
        
        protected void Start()
        {
            audioSource = GetComponent<AudioSource>();
            k = doorDirection;
        }
        
        public virtual IInteractable Interact(Transform initiator)
        {
            if (!isOpened)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 
                   k * angle), 0.3f).SetEase(Ease.InQuad).onComplete += Close;
                isOpened = true;
            }

            return this;
        }
        
        public IInteractable Activate(IInteractable item)
        {
            return item;
        }

        protected virtual void Close()
        {
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag($"NPC"))
            {
                Interact(null);
            }
        }
    }
}