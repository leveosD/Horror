using UnityEngine;

namespace ItemModule
{
    public class Note : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject label;
        public Transform Transform => transform;

        private bool _isOpened = false;

        public IInteractable Interact(IInteractable item)
        {
            label.SetActive(true);
            if (item == this)
                return null;
            return this;
        }

        public bool Activate(IInteractable item)
        {
            label.SetActive(false);
            return true;
        }
    }
}