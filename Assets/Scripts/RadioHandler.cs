using Cysharp.Threading.Tasks;
using UnityEngine;

public class RadioHandler : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip[] clips;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Game.OnClientsDeath += MakeNoise;
    }
    
    private void OnDisable()
    {
        Game.OnClientsDeath -= MakeNoise;
    }

    private async void MakeNoise()
    {
        _audioSource.clip = clips[0];
        _audioSource.volume = 1f;
        _audioSource.Play();

        await UniTask.Delay(5000);
        _audioSource.volume = 0.5f;
        _audioSource.clip = clips[1];
        _audioSource.Play();
    }

    private void TurnOff(bool win)
    {
        _audioSource.Stop();
    }
}
