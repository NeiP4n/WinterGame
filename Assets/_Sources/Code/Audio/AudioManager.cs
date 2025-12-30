using UnityEngine;
using Sources.Code.Config.Audio;

namespace Sources.Code.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioCategory audioCategory;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void Play(SoundData sound)
        {
            if (Instance == null || sound == null || sound.clips.Length == 0)
                return;

            var clip = sound.clips[Random.Range(0, sound.clips.Length)];

            Instance.sfxSource.pitch = sound.randomPitch
                ? Random.Range(sound.pitch - 0.1f, sound.pitch + 0.1f)
                : sound.pitch;

            Instance.sfxSource.PlayOneShot(clip, sound.volume);
        }

        public static void PlayMusic(SoundData music)
        {
            if (Instance == null || music == null || music.clips.Length == 0)
                return;

            if (Instance.musicSource.clip == music.clips[0])
                return;

            Instance.musicSource.clip = music.clips[0];
            Instance.musicSource.volume = music.volume;
            Instance.musicSource.loop = true;
            Instance.musicSource.Play();
        }

        public static AudioCategory Cat => Instance.audioCategory;
    }
}
