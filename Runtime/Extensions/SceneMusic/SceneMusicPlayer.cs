using UnityEngine;
using UnityEngine.SceneManagement;

namespace PeriAudio.Extensions
{
    public class SceneMusicPlayer : MonoBehaviour
    {
        [SerializeField] private SceneMusicSettings settings;
        [SerializeField] private bool playCurrentSceneOnStart = true;
        [SerializeField] private bool stopMusicIfUnmapped = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (playCurrentSceneOnStart) PlaySceneMusic(SceneManager.GetActiveScene());
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlaySceneMusic(scene);
        }

        private void PlaySceneMusic(Scene scene)
        {
            if (settings == null)
            {
                Debug.LogWarning("[PeriAudio] No SceneMusicSettings assigned.");
                return;
            }

            if (settings.TryGetMusic(scene.name, out var musicEvent))
            {
                AudioManager.Music?.Play(musicEvent);
                return;
            }

            if (stopMusicIfUnmapped) AudioManager.Music?.Stop();
        }
    }
}