using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class Door : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;
        protected bool _isOpened = false;
        
        protected AudioSource audioSource;
        [SerializeField] protected AudioClip[] clips;

        protected float angle = 85f;
        
        protected void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        public IInteractable Interact(IInteractable item)
        {
            if (!_isOpened)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 
                    angle), 0.3f).SetEase(Ease.InQuad).onComplete += Close;
                _isOpened = true;
            }

            return item;
        }
        
        public bool Activate(IInteractable item)
        {
            return true;
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