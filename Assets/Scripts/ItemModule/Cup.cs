using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ItemModule
{
    public class Cup : TakeableItem
    {
        private bool _isFilled = false;
        private bool _isClosed = false;
        public bool IsFilled => _isFilled;
        public bool IsClosed => _isClosed;

        private Transform _anchorBuffer;

        public override IInteractable Activate(IInteractable item)
        {
            if (item is CoffeeMachine && !_isFilled)
            {
                speed = 700;
                _anchorBuffer = anchor;
                anchor = item.Transform;

                audioSource.clip = clips[1];
                audioSource.Play();

                WaitForBrewing();
                
                return null;
            }
            if (item is Lid lid && _isFilled && !_isClosed)
            {
                Debug.Log($"{_isFilled} {_isClosed}");
                _isClosed = true;
                WaitForClosing(lid);
                return this;
            }

            return base.Activate(item);
        }

        private async void WaitForBrewing()
        {
            await UniTask.Delay(500);
            audioSource.clip = clips[2];
            audioSource.Play();
            await UniTask.Delay((int)clips[2].length * 1000);
            _isFilled = true;
            IsHandled = false;
            anchor = _anchorBuffer;
        }

        private async void WaitForClosing(Lid lid)
        {
            lid.CloseUp(transform);
        }
    }
}