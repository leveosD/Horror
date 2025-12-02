using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ItemModule;
using UnityEngine;

public class Game : IDisposable
{
    private readonly Vector3 _clientStartPosition = new Vector3(1.35f, 0, 23);
    private readonly Vector3 _ghosfaceStartPosition = new Vector3(-0.5f, 0, -5);

    private GameObject _clientObject;
    private GameObject _ghostfaceObject;
    
    private INPC _client;
    private INPC _ghostface;

    private bool _isPlayerSafe = false;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    
    public static event Action OnClientsDeath;

    public Game(Transform parent, GameObject client, GameObject ghostface)
    {
        _clientObject = GameObject.Instantiate(client, parent);
        _ghostfaceObject = GameObject.Instantiate(ghostface, parent);

        _clientObject.transform.position = _clientStartPosition;
        _ghostfaceObject.transform.position = _ghosfaceStartPosition;

        _clientObject.TryGetComponent(out _client);
        _ghostfaceObject.TryGetComponent(out _ghostface);
        
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        
        Car.InteractWithCar += TryToWin;
    }

    public void Dispose()
    {
        Car.InteractWithCar -= TryToWin;
    }

    public async UniTask<bool> Play()
    {
        //await _client.Behaviour(_cancellationToken);
        OnClientsDeath?.Invoke();

        UniTask killerTask = _ghostface.Behaviour(_cancellationToken);
        UniTask playerTask = PlayerTask();
        int index = await UniTask.WhenAny(killerTask, playerTask);
        
        _cancellationTokenSource.Cancel();
        return index == 1;
    }

    private async UniTask PlayerTask()
    {
        await UniTask.WaitUntil(() => _isPlayerSafe, cancellationToken: _cancellationToken);
    }

    private void TryToWin()
    {
        if (_ghostfaceObject)
        {
            _isPlayerSafe = true;
        }
    }
}