using UnityEngine;

namespace ItemModule
{
    public interface IInteractable
    {
        Transform Transform
        {
            get;
        }
        IInteractable Interact(IInteractable itemTransform);
        bool Activate(IInteractable itemTransform);
    }
}