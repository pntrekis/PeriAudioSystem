using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PeriAudio
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        private static AudioManager instance;
        public static AudioManager Instance => instance;
        #endregion

        [Header("Settings")]
        [SerializeField] private AudioSettings settings;

        public Bus MasterBus { get; private set; }
        public Bus MusicBus { get; private set; }
        public Bus SFXBus { get; private set; }
        public Bus AmbienceBus { get; private set; }

        public static MusicPlayer Music { get; private set; }

        private ParameterTweener globalParameterTweener;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            if (settings == null)
            {
                Debug.LogError("[PeriAudio] No AudioSettings assigned.");
                return;
            }

            MasterBus = RuntimeManager.GetBus(settings.MasterBus);
            MusicBus = RuntimeManager.GetBus(settings.MusicBus);
            SFXBus = RuntimeManager.GetBus(settings.SFXBus);
            AmbienceBus = RuntimeManager.GetBus(settings.AmbienceBus);

            globalParameterTweener = new ParameterTweener(this);
            Music = new MusicPlayer(this);

            Debug.Log("[PeriAudio] Initialized.");
        }

        #region One Shots
        public static void PlayOneShot(EventReference eventReference)
        {
            if (!CanPlay(eventReference)) return;
            RuntimeManager.PlayOneShot(eventReference);
        }

        public static void PlayOneShot(EventReference eventReference, Vector3 position)
        {
            if (!CanPlay(eventReference)) return;
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        private static bool CanPlay(EventReference eventReference)
        {
            if (instance == null)
            {
                Debug.LogWarning("[PeriAudio] AudioManager has not been initialized.");
                return false;
            }

            return !eventReference.IsNull;
        }
        #endregion

        #region Instances
        public static AudioHandle Create(EventReference eventReference)
        {
            if (!CanPlay(eventReference)) return null;
            return new AudioHandle(eventReference);
        }

        public static AudioHandle Create(EventReference eventReference, GameObject go)
        {
            if (!CanPlay(eventReference)) return null;
            return new AudioHandle(eventReference, go);
        }
        #endregion

        #region Volume
        public static void SetMasterVolume(float volume) => SetBusVolume(instance?.MasterBus ?? default, volume);
        public static void SetMusicVolume(float volume) => SetBusVolume(instance?.MusicBus ?? default, volume);
        public static void SetSFXVolume(float volume) => SetBusVolume(instance?.SFXBus ?? default, volume);
        public static void SetAmbienceVolume(float volume) => SetBusVolume(instance?.AmbienceBus ?? default, volume);

        public static float GetMasterVolume() => GetBusVolume(instance?.MasterBus ?? default);
        public static float GetMusicVolume() => GetBusVolume(instance?.MusicBus ?? default);
        public static float GetSFXVolume() => GetBusVolume(instance?.SFXBus ?? default);
        public static float GetAmbienceVolume() => GetBusVolume(instance?.AmbienceBus ?? default);

        private static void SetBusVolume(Bus bus, float volume)
        {
            if (!bus.isValid()) return;
            bus.setVolume(Mathf.Clamp01(volume));
        }

        private static float GetBusVolume(Bus bus)
        {
            if (!bus.isValid()) return 0f;

            bus.getVolume(out float volume);
            return volume;
        }
        #endregion

        #region Global Parameters
        public static void SetGlobalParameter(string parameter, float value)
        {
            if (instance == null) return;

            instance.globalParameterTweener.Stop(parameter);
            RuntimeManager.StudioSystem.setParameterByName(parameter, value);
        }

        public static void SetGlobalParameter(string parameter, float value, float duration)
        {
            if (instance == null) return;

            instance.globalParameterTweener.Tween(parameter, value, duration, () =>
                {
                    RuntimeManager.StudioSystem.getParameterByName(parameter, out float currentValue);
                    return currentValue;
                },
                newValue => RuntimeManager.StudioSystem.setParameterByName(parameter, newValue)
            );
        }
        #endregion
    }
}