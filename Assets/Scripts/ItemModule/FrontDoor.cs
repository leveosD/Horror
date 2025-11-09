using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class FrontDoor : Door
    {
        public Transform Transform => transform;

        [SerializeField] private int doorDirection = 1;

        private int _k = 1;

        /*public IInteractable Interact(IInteractable item)
        {
            if (!_isOpened)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
                transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 
                   doorDirection * _k * 85), 0.3f).SetEase(Ease.InQuad).onComplete += Close;
                _isOpened = true;
            }

            return item;
        }*/

        protected override async void Close()
        {
            await UniTask.Delay(500);
            transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
              doorDirection * (-_k) * 85), 0.3f).SetEase(Ease.InQuad).onComplete += () => _isOpened = false;
            audioSource.clip = clips[1];
            audioSource.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag($"NPC"))
            {
                var vect = transform.position - other.transform.position;
                _k = vect.x > 0 ? 1 : -1;
                angle *= _k * doorDirection;
                Interact(null);
            }
        }
    }
}