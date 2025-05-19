using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class ButtonWithAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _clickClip;
    private IAudioPlayer _audio;

    private void Awake()
    {
        _audio = gameObject.AddComponent<AudioPlayer>();
        _audio.Configure(_clickClip);
        GetComponent<Button>().onClick.AddListener(_audio.Play);
    }
}
