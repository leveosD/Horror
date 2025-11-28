using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EntryPoint : MonoBehaviour
{
    private Game _game;
    public static MyInputSystem myInputSystem;
    
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private GameObject ghostfacePrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject winWindow;
    [SerializeField] private GameObject loseWindow;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image fade;
    [SerializeField] private Text text;

    private Transform _camera;
    private GameObject _player;

    private const float Duration = 2.5f;
    
    void Awake()
    {
        myInputSystem = new MyInputSystem();
        myInputSystem.Player.Interact.performed += StartGame;
        myInputSystem.Player.Interact.Enable();
        
        _player = Instantiate(playerPrefab);
        _player.transform.position = new Vector3(11, 1.5f, 1);
        _player.transform.localEulerAngles = new Vector3(0, 90, 0);
        
        _camera = Camera.main.transform;
        _camera.parent = _player.transform;
        _camera.transform.localPosition = Vector3.zero;
        _camera.localEulerAngles = new Vector3(45, 0, 0);
    }

    private void OnDestroy()
    {
        _game.Dispose();
    }

    private async void StartGame(InputAction.CallbackContext context)
    {
        myInputSystem.Player.Interact.performed -= StartGame;
        Destroy(text);

        float time = 0f;
        while (time < Duration)
        {
            float t = time / Duration;
            float alpha = Mathf.Lerp(1, 0, t);
            fade.color = new Color(0, 0, 0, alpha);
            //text.color = new Color(1, 1, 1, alpha / 2);
            _camera.localEulerAngles = new Vector3(Mathf.Lerp(45, 0, t), 0, 0);
            time += Time.deltaTime;
            await UniTask.Yield();
        }
        
        fade.gameObject.SetActive(false);
        canvas.SetActive(true);
        myInputSystem.Player.Enable();
        
        _game = new Game(transform, clientPrefab, ghostfacePrefab);
        GameOver();
    }
    
    private async void GameOver()
    {
        bool result = await _game.Play();
        myInputSystem.Player.Disable();
        _camera.parent = null;
        Destroy(_player);
        await UniTask.Delay(2500);

        fade.gameObject.SetActive(true);
        fade.color = Color.black;
        if(result)
            winWindow.SetActive(true);
        else
            loseWindow.SetActive(true);
        canvas.SetActive(false);
    }
}
