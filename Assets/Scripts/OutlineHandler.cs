using ItemModule;
using UnityEngine;

namespace DefaultNamespace
{
    public class OutlineHandler : MonoBehaviour
    {
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private int outlineLayer;
        [SerializeField] private GameObject noteBackground;

        private Transform _cameraTransform;

        private GameObject _currentHit;
        private MeshRenderer _currentMeshRenderer;
        private Material[] _originalMaterials;
        private int _originalLayer = -1;

        private const float RayDistance = 1.75f;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out var hit, RayDistance, raycastLayerMask))
            {
                var go = hit.collider.gameObject;
                
                ResetHighlight();

                if (go.CompareTag("TakeableObject"))
                    TryHighlightItem(go);

                else if (go.CompareTag("Note"))
                    TryHighlightNote(go);

                _currentHit = go;
            }
            else
            {
                ResetHighlight();
            }
        }

        private void TryHighlightItem(GameObject go)
        {
            if (go == _currentHit) return;
            
            if (!go.TryGetComponent<TakeableItem>(out var item) || item.IsHandled)
                return;

            if (go.TryGetComponent<MeshRenderer>(out var mesh))
            {
                _currentMeshRenderer = mesh;
                // сохраняем оригинальные материалы, добавляем outline
                _originalMaterials = mesh.materials;
                var newMats = new Material[_originalMaterials.Length + 1];
                _originalMaterials.CopyTo(newMats, 0);
                newMats[newMats.Length - 1] = outlineMaterial;
                mesh.materials = newMats;
            }
        }

        private void TryHighlightNote(GameObject go)
        {
            if (!noteBackground.activeSelf)
            {
                if(_currentHit) ResetHighlight();
                _originalLayer = go.layer;
                SetLayerRecursively(go, outlineLayer);
            }
            else
            {
                ResetHighlight();
            }
        }

        private void ResetHighlight()
        {
            if (_currentMeshRenderer != null)
            {
                _currentMeshRenderer.materials = _originalMaterials;
                _currentMeshRenderer = null;
                _originalMaterials = null;
            }

            if (_originalLayer >= 0/*_currentNote != null && *//*_currentHit != null*/)
            {
                SetLayerRecursively(_currentHit, _originalLayer);
                _originalLayer = -1;
            }

            _currentHit = null;
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            int childLayer = 0;
            if (layer == outlineLayer)
                childLayer = outlineLayer;
            //Debug.Log($"{obj.layer} {layer}; {obj.transform.GetChild(0).gameObject.layer} {childLayer}");
            obj.layer = layer;
            
            /*foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, childLayer);
            }*/

            obj.transform.GetChild(0).gameObject.layer = childLayer;
        }
    }
}