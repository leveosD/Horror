using UnityEngine;

namespace ItemModule
{
    public class Note : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject label;
        public Transform Transform => transform;

        private bool _isOpened = false;

        public IInteractable Interact(Transform initiator)
        {
            background.SetActive(true);
            label.SetActive(true);
            return this;
        }

        public IInteractable Activate(IInteractable item)
        {
            label.SetActive(false);
            background.SetActive(false);
            return item;
        }
    }
}