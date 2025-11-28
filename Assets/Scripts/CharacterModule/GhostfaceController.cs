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
            
            player.transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0, 180, 0), 0.25f)
                .SetEase(Ease.OutQuad);
            Camera.main.transform.DOLocalRotate(new Vector3(10, 0, 0), 0.25f)
                .SetEase(Ease.OutQuad).onComplete += () =>
            {
                _navMeshAgent.enabled = false;
                transform.GetComponent<Collider>().isTrigger = true;
                var offset = new Vector3(player.position.x + player.forward.x, 0, player.position.z + player.forward.z);
                transform.DOMove(offset, 0.01f).SetEase(Ease.InOutSine);
            };
            
            await UniTask.Delay(250, cancellationToken: token);
            
            _animator.CrossFadeInFixedTime("Kill", 0.1f);
            _audioSource.clip = clips[1];
            _audioSource.volume = 0.15f;
            _audioSource.Play();
            Debug.Log("Killer is done his job");
        }
        catch (OperationCanceledException)
        {
            _animator.CrossFadeInFixedTime("Idle", 1f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
            _gotHim = true;
    }
}