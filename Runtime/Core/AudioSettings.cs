using UnityEngine;

namespace PeriAudio
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "Peri Audio/Audio Settings")]
    public class AudioSettings : ScriptableObject
    {
        [Header("Bus Paths")]
        [SerializeField] private string masterBus = "bus:/";
        [SerializeField] private string musicBus = "bus:/Music";
        [SerializeField] private string sfxBus = "bus:/SFX";
        [SerializeField] private string ambienceBus = "bus:/Ambience";

        public string MasterBus => masterBus;
        public string MusicBus => musicBus;
        public string SFXBus => sfxBus;
        public string AmbienceBus => ambienceBus;
    }
}