using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ItemModule;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector3 clientStartPosition;
    [SerializeField] private Vector3 ghostfaceStartPosition;
    
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private GameObject ghostfacePrefab;

    private GameObject _clientObject;
    private GameObject _ghostfaceObject;
    
    private INPC _client;
    private INPC _ghostface;

    private bool _isPlayerSafe = false;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    
    public static event Action OnClientsDeath;
    public static event Action<bool> GameOver;

    private void OnEnable()
    {
        Car.InteractWithCar += TryToWin;
    }
    
    private void OnDisable()
    {
        Car.InteractWithCar -= TryToWin;
    }

    private void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        Play();
    }

    private async void Play()
    {
        _clientObject = Instantiate(clientPrefab, transform);
        _clientObject.transform.position = clientStartPosition;
        _clientObject.TryGetComponent(out _client);
        
        await _client.Behaviour(_cancellationToken);
        OnClientsDeath?.Invoke();
        
        _ghostfaceObject = Instantiate(ghostfacePrefab, transform);
        _ghostfaceObject.transform.position = ghostfaceStartPosition;
        _ghostfaceObject.TryGetComponent(out _ghostface);

        UniTask killerTask = _ghostface.Behaviour(_cancellationToken);
        UniTask playerTask = PlayerTask();
        int index = await UniTask.WhenAny(killerTask, playerTask);
        _cancellationTokenSource.Cancel();
        GameOver?.Invoke(index == 1);
    }

    private async UniTask PlayerTask()
    {
        await UniTask.WaitUntil(() => _isPlayerSafe, cancellationToken: _cancellationToken);
        Debug.Log(_isPlayerSafe);
    }

    private void TryToWin()
    {
        if (_ghostfaceObject)
        {
            _isPlayerSafe = true;
        }
    }
}