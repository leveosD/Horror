using UnityEngine;

namespace ItemModule
{
    public class DoubleDoor : MonoBehaviour
    {
        private Door[] _doors;

        protected void Start()
        {
            _doors = GetComponentsInChildren<Door>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag($"NPC"))
            {
                var direction = transform.position.x - other.transform.position.x > 0 ? 1 : -1;
                _doors[0].k = direction * _doors[0].DoorDirection;
                _doors[1].k = direction * _doors[1].DoorDirection;
                _doors[0].Interact(null);
                _doors[1].Interact(null);
            }
        }
    }
}