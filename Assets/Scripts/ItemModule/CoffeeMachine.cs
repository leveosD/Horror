using UnityEngine;

namespace ItemModule
{
    public class CoffeeMachine : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;

        public IInteractable Interact(Transform initiator)
        {
            return null;
        }

        public IInteractable Activate(IInteractable item)
        {
            return item;
        }
    }
}