using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class GhostfaceController : MonoBehaviour, INPC
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Vector3 target;
    private Transform player;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] clips;

    private bool _gotHim = false;
    //public static event Action OnPlayersDeath;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.None)[0].transform;
    }
    
    public async UniTask Behaviour(CancellationToken token)
    {
        try
        {
            _navMeshAgent.SetDestination(target);
            await UniTask.WaitUntil(() => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance, cancellationToken: token);

            _animator.CrossFadeInFixedTime("Base Layer.Idle", 0.1f);
            await UniTask.Delay(100, cancellationToken: token);
            
            _animator.CrossFadeInFixedTime("Base Layer.ArmUp", 0.2f);
            _audioSource.clip = clips[0];
            _audioSource.Play();
            await UniTask.Delay(2000, cancellationToken: token);

            _animator.CrossFadeInFixedTime("Base Layer.RunWithKnife", 0.3f);
            Vector3 previousPosition = player.position;
            while (!_gotHim)
            {
                var delta = player.position - previousPosition;
                _navMeshAgent.SetDestination(player.position + delta);
                previousPosition = player.position;
                await UniTask.Delay(500, cancellationToken: token);
            }

            _animator.CrossFadeInFixedTime("Idle", 0.2f);

            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion targetRotationA = Quaternion.LookRotation(direction);
        
            transform.DORotateQuaternion(targetRotationA, 0.25f).SetEase(Ease.InQuad);;

            Vector3 directionB = transform.position - player.position;
            directionB.y = 0;
            Quaternion targetRotationB = Quaternion.LookRotation(directionB);
        
            player.DORotateQuaternion(targetRotationB, 0.25f).SetEase(Ease.InQuad)
                .onComplete += () =>
            {
                var newPosition = player.position + player.forward * 0.9f;
                newPosition.y = 0;
                transform.DOMove(newPosition, 0.1f)
                    .SetEase(Ease.InQuad);
            };
            _navMeshAgent.enabled = false;
            
            await UniTask.Delay(250, cancellationToken: token);
            
            _animator.CrossFadeInFixedTime("Kill", 0.1f);
            _audioSource.clip = clips[1];
            _audioSource.volume = 0.15f;
            _audioSource.Play();

            Debug.Log("Killer is done his job");
        }
        catch (OperationCanceledException)
        {
            _navMeshAgent.enabled = false;
            _animator.CrossFadeInFixedTime("Idle", 1f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
            _gotHim = true;
    }
}