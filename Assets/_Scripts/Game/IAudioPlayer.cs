using UnityEngine;

public interface IAudioPlayer
{
    void Play();
    void Stop();
    void Configure(AudioClip clip, bool loop = false, float pitch = 1f);
}
