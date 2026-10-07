using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PeriAudio.Extensions
{
    [CreateAssetMenu(fileName = "SceneMusicSettings", menuName = "Peri Audio/Scene Music Settings")]
    public class SceneMusicSettings : ScriptableObject
    {
        [SerializeField] private List<SceneMusicEntry> entries = new();

        public bool TryGetMusic(string sceneName, out EventReference musicEvent)
        {
            foreach (SceneMusicEntry entry in entries)
            {
                if (entry.SceneName != sceneName) continue;

                musicEvent = entry.MusicEvent;
                return !musicEvent.IsNull;
            }

            musicEvent = default;
            return false;
        }
    }

    [Serializable]
    public class SceneMusicEntry
    {
        [SerializeField] private string sceneName;
        [SerializeField] private EventReference musicEvent;

        public string SceneName => sceneName;
        public EventReference MusicEvent => musicEvent;
    }
}