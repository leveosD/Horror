using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class Cup : TakeableItem
    {
        private bool _isFilled = false;
        private bool _isClosed = false;
        public bool IsFilled => _isFilled;
        public bool IsClosed => _isClosed;

        public override bool Activate(IInteractable item)
        {
            Debug.Log("Cup activated");
            if (item is CoffeeMachine && !_isFilled)
            {
                IsHandled = false;
                IsTaking = true;
                gameObject.GetComponent<Collider>().enabled = false;
                transform.parent = item.Transform;
                transform.DOLocalMove(new Vector3(-0.0015f, 0.002f, -0.0021f), 0.7f);
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0.7f).SetEase(Ease.OutBounce);

                audioSource.clip = clips[1];
                audioSource.Play();

                WaitForBrewing();
                
                return true;
            }
            if (item is not Lid || !_isFilled)
            {
                return base.Activate(item);
            }

            Debug.Log("Lid");
            if(_isFilled) _isClosed = true;

            return false;
        }

        private async void WaitForBrewing()
        {
            audioSource.clip = clips[2];
            audioSource.Play();
            Debug.Log(audioSource.clip);
            await UniTask.Delay((int)clips[2].length * 1000);
            _isFilled = true;
            IsTaking = false;
            gameObject.GetComponent<Collider>().enabled = true;

        }
    }
}