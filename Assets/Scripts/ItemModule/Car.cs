using System;
using UnityEngine;

namespace ItemModule
{
    public class Car : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;
        public static Action InteractWithCar;

        public IInteractable Interact(Transform initiator)
        {
            InteractWithCar?.Invoke();
            return this;
        }

        public IInteractable Activate(IInteractable item)
        {
            return this;
        }
    }
}