using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] [Range(0f, 1f)] private float _volumeModifier = 0.5f;
    public static float volumeModifier = 0.5f;

    void Update() {
        volumeModifier = _volumeModifier;
    }

    public static void Play(AudioClip audio, Vector2 position, float volume) {
        AudioSource.PlayClipAtPoint(audio, position, volume * volumeModifier);
    }

    public static void Play(AudioSource audio, Vector2 position, float volume) {
        Play(audio.clip, position, volume);
    }

    public static void Play(AudioClip audio, Vector2 position) {
        Play(audio, position, 1f);
    }

    public static void Play(AudioSource audio, Vector2 position) {
        Play(audio, position, 1f);
    }

    public static void Play(AudioClip audio, float volume) {
        Play(audio, Camera.main.transform.position, volume);
    }

    public static void Play(AudioSource audio, float volume) {
        audio.volume = volume * volumeModifier;
        audio.PlayOneShot(audio.clip);
    }

    public static void Play(AudioClip audio) {
        Play(audio, 1f);
    }

    public static void Play(AudioSource audio) {
        Play(audio, 1f);
    }
}