using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ItemModule
{
    public class Lid : TakeableItem
    {
        public async void CloseUp(Transform parent)
        {
            await UniTask.WaitUntil(() => itemRigidbody.linearVelocity.magnitude <= 0.01f);
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            Destroy(itemRigidbody);
            Destroy(this);
        }
    }
}