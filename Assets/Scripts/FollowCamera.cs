using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.PackageManager;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class FollowCamera : MonoBehaviour
{
    public Camera mainCamera;
    public float moveSpeed = 5.0f;
    
    private Vector3 _smoothV;   // Сглаженный вектор 
    [SerializeField] private float _smoothing;
    
    private Rigidbody _rigidbody;
    private Collider _collider;

    private Vector3 lastTarget;

    [SerializeField] private Transform player;

    private Vector3 collisionVector = Vector3.zero;
    
    Vector3 norm;

    private bool isInCollision = false;
    private List<Collider> connectedColliders = new List<Collider>();

    private Vector3 constraintVector = Vector3.one;

    private bool freeze = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        lastTarget = mainCamera.transform.position + mainCamera.transform.forward;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    /*void FixedUpdate()
    {
        var pos = transform.position;
        var camPos = mainCamera.transform.position;
        var forward = mainCamera.transform.forward; //Vector3.Cross(mainCamera.transform.forward, Vector3.down);
        //Debug.Log($"Distance from forward: {Vector3.Distance(pos, camPos + forward)}");
        //Vector3 norm;

        /*if (collisionVector != Vector3.zero)
        {
            _rigidbody.MovePosition(transform.position - collisionVector);
            return;
        }#1#
        
        if (camPos + forward != lastTarget)
        {
            //var norm = (pos - (camPos + forward)).normalized;
            var norm = (pos - (camPos + forward/2 + Vector3.down/3 )).normalized;
            
            /*Debug.Log("Position: " + pos);
            Debug.Log("Camera pos: " + camPos + " Forward: "+ forward);
            Debug.Log("Direction: "+ norm);#1#
        
            float mouseX = -norm.x * moveSpeed;
            float mouseY = -norm.y * moveSpeed;
            float mouseZ = -norm.z * moveSpeed;

            mouseX = Mathf.Lerp(_smoothV.x, mouseX, 1f / _smoothing);
            mouseY = Mathf.Lerp(_smoothV.y, mouseY, 1f / _smoothing);
            mouseZ = Mathf.Lerp(_smoothV.z, mouseZ, 1f / _smoothing);

            _smoothV = new Vector3(mouseX, mouseY, mouseZ);
            /*_rigidbody.MovePosition(mainCamera.transform.position + mainCamera.transform.forward/2 + Vector3.down/3 
                                    + _smoothV * Time.fixedDeltaTime);#1#
            _rigidbody.MovePosition(pos + _smoothV * Time.fixedDeltaTime);

        }
        lastTarget = camPos + forward;
        
    }*/

    void FixedUpdate()
    {
        var pos = transform.position;
        var camPos = mainCamera.transform.position;
        var forward = mainCamera.transform.forward;
        var targetPoint = mainCamera.transform.position + mainCamera.transform.forward / 2 + Vector3.down / 4;

        if (targetPoint == lastTarget)
            return;
        //Debug.Log(targetPoint + " " + lastTarget);
        
        /*if (connectedColliders.Count != 0)
        {
            Vector3 compositeVector = Vector3.zero;
            /*float compositeDistance = 0f;
            int index = 1;
            foreach (var collider in connectedColliders)
            {
                Physics.ComputePenetration(
                    GetComponent<Collider>(), targetPoint, transform.rotation,
                    collider, collider.transform.position, collider.transform.rotation,
                    out Vector3 direction, out float distance);
                var vect = direction * distance;
                float oldy = compositeVector.y;
                /*if (collisionVector.y > 0 && vect.y < 0 || collisionVector.y < 0 && vect.y > 0)
                {
                    vect.y = 0;
                    Bounds thisBounds = _collider.bounds;
                    Bounds colliderBounds = collider.bounds;
                    float overlapX = Mathf.Min(thisBounds.max.x, colliderBounds.max.x) -
                                     Mathf.Max(thisBounds.min.x, colliderBounds.min.x);
                    targetPoint -= mainCamera.transform
                }#2#
                compositeVector += vect;
                compositeVector.y = vect.y > oldy ? vect.y : oldy;
                Debug.Log($"{index++} {collider.gameObject.name} {vect}");
            }
            Debug.DrawLine(targetPoint, targetPoint + compositeVector, Color.red);
            Debug.Log($"Overwise. Vector: {compositeVector}");#1#
            int index = 1;
            //var directions = new Vector3(forward.x > 0 ? -1 : 1, forward.y > 0 ? -1 : 1, forward.z > 0 ? -1 : 1);
            foreach (var collider in connectedColliders)
            {
                var vect = FindOverlaps(_collider, collider);
                var roundedVector = vect;/*new Vector3((float)Math.Round(vect.x, 2), (float)Math.Round(vect.y, 2),
                    (float)Math.Round(vect.z, 2));#1#
                Debug.Log(index++ + " " + collider.gameObject.name + " " + vect.ToString("F10"));
                if (roundedVector.y <= roundedVector.x && roundedVector.y <= roundedVector.z 
                                                       && roundedVector.y != 0)
                {
                    Debug.Log($"Y: {compositeVector.y} {roundedVector.y}");
                    compositeVector.y = Mathf.Max(compositeVector.y, roundedVector.y);
                }
                else if (roundedVector.x <= roundedVector.y && roundedVector.x <= roundedVector.z 
                                                            && roundedVector.x != 0)
                {
                    Debug.Log($"X: {compositeVector.x} {roundedVector.x}");
                    compositeVector.x = Mathf.Max(compositeVector.x, vect.x);
                }
                else if (roundedVector.z <= roundedVector.x && roundedVector.z <= roundedVector.y
                                                            && roundedVector.z != 0)
                {
                    Debug.Log($"Z: {compositeVector.z} {roundedVector.z}");
                    compositeVector.z = Mathf.Max(compositeVector.z, vect.z);
                }
                /*compositeVector = new Vector3(directions.x * Mathf.Max(compositeVector.x, vect.x),
                    directions.y * Mathf.Max(compositeVector.y, vect.y), directions.z * Mathf.Max(compositeVector.z, vect.z));#1#
            }
            Debug.Log("Composite: " + compositeVector);
            //_rigidbody.MovePosition(targetPoint + compositeVector);
            _rigidbody.MovePosition(targetPoint + compositeVector);
            freeze = true;
        }
        else*/
        
        {
            if (Vector3.Distance(pos, camPos + forward) > 0.1f)
            {
                norm = (pos - (camPos)).normalized;
            }
            else if (camPos + forward == lastTarget)
            {
                norm = Vector3.zero;
            }

            float mouseX = -norm.x * moveSpeed;
            float mouseY = -norm.y * moveSpeed;
            float mouseZ = -norm.z * moveSpeed;

            mouseX = Mathf.Lerp(_smoothV.x, mouseX, 1f / _smoothing);
            mouseY = Mathf.Lerp(_smoothV.y, mouseY, 1f / _smoothing);
            mouseZ = Mathf.Lerp(_smoothV.z, mouseZ, 1f / _smoothing);

            _smoothV = new Vector3(mouseX * constraintVector.x, mouseY * constraintVector.y,
                mouseZ * constraintVector.z);

            //Debug.Log($"Target: {targetPoint} Smooth: {_smoothV}");

            /*var offset = Vector3.zero;
            var direction = (pos - camPos).normalized;
            if (_rigidbody.SweepTest(direction, out var hit, 0.1f))
            {
                Debug.Log($"It's time to stop. Hit object: {hit.collider.gameObject.name} Distance: {hit.distance}");
                offset = direction * hit.distance;
            }*/
            
            Vector3 half = new Vector3()
            {
                x = _collider.bounds.max.x - _collider.bounds.center.x,
                y = _collider.bounds.max.y - _collider.bounds.center.y,
                z = _collider.bounds.max.z - _collider.bounds.center.z
            };
            Collider[] colliders = new Collider[5];
            if (Physics.OverlapBoxNonAlloc(targetPoint + _smoothV * Time.fixedDeltaTime, 
                    half, colliders, Quaternion.identity, ~_collider.excludeLayers) != 0)
            {
                Debug.Log($"{colliders[0].transform.gameObject.name}");
                return;
            }
            
            _rigidbody.MovePosition(targetPoint + _smoothV * Time.fixedDeltaTime);
            //_rigidbody.Move(targetPoint + _smoothV * Time.fixedDeltaTime, transform.rotation);
        }

        lastTarget = targetPoint;
    }

    private void OnCollisionEnter(Collision other)
    {
        isInCollision = true;
        connectedColliders.Add(other.collider);
        //Debug.Log($"Enter: {other.collider.gameObject.name}");
        /*Debug.Log(other.collider.gameObject.name);
        int index = 1;
        foreach (var contact in other.contacts)
        {
            Debug.Log(index++ + " " + contact.point);
        }*/
    }

    private void OnCollisionExit(Collision other)
    {
        connectedColliders.Remove(other.collider);
        Debug.Log($"Exit: {other.collider.gameObject.name}");
    }

    /*private List<float> FindOverlaps(Collider colliderA, Collider colliderB)
    {
        Bounds boundsA = colliderA.bounds;
        Bounds boundsB = colliderB.bounds;

        //Vector3 overlap = new Vector3
        List<float> list = new List<float>()
        {
            /*x =#1# Mathf.Max(0,
                boundsB.max.x - boundsA.min.x, boundsA.max.x - boundsB.min.x),
            /*y =#1# Mathf.Max(0,
                boundsB.max.y - boundsA.min.y, boundsA.max.y - boundsB.min.y),
            /*z =#1# Mathf.Max(0,
                boundsB.max.z - boundsA.min.z, boundsA.max.z - boundsB.min.z)
        };

        return list;
    }*/
    
    private Vector3 FindOverlaps(Collider colliderA, Collider colliderB)
    {
        Bounds boundsA = colliderA.bounds;
        Bounds boundsB = colliderB.bounds;

        Vector3 overlap = new Vector3()
        {
            x = Mathf.Max(0, Mathf.Min(boundsA.max.x, boundsB.max.x) - Mathf.Max(boundsA.min.x, boundsB.min.x)),
            y = Mathf.Max(0, Mathf.Min(boundsA.max.y, boundsB.max.y) - Mathf.Max(boundsA.min.y, boundsB.min.y)),
            z = Mathf.Max(0, Mathf.Min(boundsA.max.z, boundsB.max.z) - Mathf.Max(boundsA.min.z, boundsB.min.z))
        };

        return overlap;
    }
}