using UnityEngine;

namespace ItemModule
{
    public interface IInteractable
    {
        Transform Transform
        {
            get;
        }
        IInteractable Interact(Transform initiator);
        IInteractable Activate(IInteractable itemTransform);
    }
}