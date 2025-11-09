using DG.Tweening;
using UnityEngine;

namespace ItemModule
{
    public class Lid : TakeableItem
    {
        public override IInteractable Interact(IInteractable item)
        {
            if (item is Cup cup)
            {
                if (cup.IsFilled)
                {
                    transform.parent = item.Transform;
                    itemRigidbody.isKinematic = true;
                    gameObject.GetComponent<Collider>().enabled = false;
                    transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutSine);
                    transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutSine);

                    audioSource.clip = clips[0];
                    audioSource.Play();

                    return item;
                }

            }
            
            base.Interact(item);
            return this;
        }
    }
}