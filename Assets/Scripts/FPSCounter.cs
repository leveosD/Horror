using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    float _deltaTime = 0.0f;
    int _fps = 0;

    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        int fps = (int)(1.0f / _deltaTime);
        if (fps != _fps)
        {
            _text.text = $"{fps}";
            _fps = fps;
        }
    }
}