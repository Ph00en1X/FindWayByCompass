using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioPlayer : MonoBehaviour, IAudioPlayer
{
    private AudioSource _source;

    private void Awake() => _source = GetComponent<AudioSource>();

    public void Configure(AudioClip clip, bool loop = false, float pitch = 1f)
    {
        _source.clip = clip;
        _source.loop = loop;
        _source.pitch = pitch;
    }

    public void Play() => _source.Play();
    public void Stop() => _source.Stop();
    public void Pause() => _source.Pause();
    public bool Playing() => _source.isPlaying;
}
