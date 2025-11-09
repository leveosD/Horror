using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ItemModule;
using UnityEngine;
using UnityEngine.AI;

public class ClientController : MonoBehaviour, INPC
{
    private bool _gotCoffee = false;

    private AudioSource _audioSource;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] protected Vector3[] targets;
    [SerializeField] private AudioClip[] clips;

    private Animator _animator;

    private Rigidbody _rigidbody;
    private BoxCollider _collider;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }

    public async UniTask Behaviour(CancellationToken token)
    {
        try
        {
            _navMeshAgent.SetDestination(targets[0]);
            await UniTask.WaitUntil(() =>
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance || _gotCoffee);

            var direction = (targets[0] - transform.position).normalized;
            transform.DORotate(new Vector3(0, Vector3.Angle(direction, transform.forward), 0), 0.55f);
            _animator.CrossFadeInFixedTime("Base Layer.Idle", 0.15f);
            await UniTask.WaitUntil(() => _gotCoffee);
            _audioSource.clip = clips[0];
            _audioSource.Play();
            _navMeshAgent.SetDestination(targets[1]);
            _animator.CrossFadeInFixedTime("Base Layer.Walk", 0.15f);
            await UniTask.WaitUntil(() => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance);

            _audioSource.clip = clips[1];
            _audioSource.Play();
            _animator.Play("Dead");
            _navMeshAgent.enabled = false;
            _collider.size = new Vector3(1, 0.1f, 1);
            _rigidbody.useGravity = true;
            await UniTask.Delay(((int)_audioSource.clip.length + 1) * 1000);
        }
        catch (OperationCanceledException)
        {
            _animator.CrossFadeInFixedTime("Idle", 1f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Interactable"))
        {
            other.gameObject.TryGetComponent<TakeableItem>(out var item);
            if (item is Cup cup)
            {
                if (cup.IsFilled && cup.IsClosed)
                {
                    Destroy(other.gameObject);
                    _gotCoffee = true;
                }
            }
        }
    }
}