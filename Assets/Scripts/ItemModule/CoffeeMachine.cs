using System;
using UnityEngine;

namespace ItemModule
{
    public class CoffeeMachine : MonoBehaviour, IInteractable
    {
        public Transform Transform => transform;

        public IInteractable Interact(IInteractable item)
        {
            return null;
        }

        public bool Activate(IInteractable item)
        {
            return true;
        }
    }
}