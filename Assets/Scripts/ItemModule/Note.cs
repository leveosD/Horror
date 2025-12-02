using UnityEngine;

namespace ItemModule
{
    public class Note : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject label;
        public Transform Transform => transform;

        private bool _isOpened = false;
        public bool IsOpened => _isOpened;

        public IInteractable Interact(Transform initiator)
        {
            /*_isOpened = true;
            Debug.Log($"Game object: {gameObject.name} Interact");
            background.SetActive(true);
            label.SetActive(true);
            return null;*/
            Debug.Log($"Game object: {gameObject.name} Interact");
            if (!_isOpened)
            {
                background.SetActive(true);
                label.SetActive(true);
            }
            else
            {
                label.SetActive(false);
                background.SetActive(false);
            }

            _isOpened = !_isOpened;
            return null;
        }

        public IInteractable Activate(IInteractable item)
        {
            /*_isOpened = false;
            Debug.Log($"Game object: {gameObject.name} Active");
            label.SetActive(false);
            if(item == null || (Note)item == this) background.SetActive(false);
            return this;*/
            /*Debug.Log($"Game object: {gameObject.name} Active");
            if (!_isOpened)
            {
                background.SetActive(true);
                label.SetActive(true);
            }
            else
            {
                label.SetActive(false);
                background.SetActive(false);
            }

            _isOpened = !_isOpened;*/
            if (item == null)
            {
                label.SetActive(false);
                _isOpened = false;
                background.SetActive(false);
            }
            if (item is Note note)
            {
                if (note != this)
                {
                    label.SetActive(false);
                    _isOpened = false;
                }
                //background.SetActive(false);
            }
            return item;
        }
    }
}