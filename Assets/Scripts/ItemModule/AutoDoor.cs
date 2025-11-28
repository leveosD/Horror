using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class AutoDoor : Door
    {
        protected override async void Close()
        {
            await UniTask.Delay(700);
            audioSource.clip = clips[1];
            audioSource.Play();
            transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 
                -k * angle), 0.3f).SetEase(Ease.InQuad).onComplete += () => isOpened = false;
        }
    }
}