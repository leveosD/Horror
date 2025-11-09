using System;
using UnityEngine;

namespace ItemModule
{
    public class Car : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;
        public static Action InteractWithCar;

        public IInteractable Interact(IInteractable item)
        {
            InteractWithCar?.Invoke();
            return item;
        }

        public bool Activate(IInteractable item)
        {
            return true;
        }
    }
}