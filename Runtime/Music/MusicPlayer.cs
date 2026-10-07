using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PeriAudio
{
    public class MusicPlayer
    {
        private readonly ParameterTweener parameterTweener;

        private EventInstance currentInstance;
        private EventReference currentEvent;

        internal MusicPlayer(MonoBehaviour coroutineRunner)
        {
            parameterTweener = new ParameterTweener(coroutineRunner);
        }

        public bool IsPlaying
        {
            get
            {
                if (!currentInstance.isValid()) return false;

                currentInstance.getPlaybackState(out PLAYBACK_STATE state);
                return state != PLAYBACK_STATE.STOPPED;
            }
        }

        public void Play(EventReference musicEvent)
        {
            if (musicEvent.IsNull) return;
            if (currentEvent.Equals(musicEvent) && IsPlaying) return;

            Stop();

            currentEvent = musicEvent;
            currentInstance = RuntimeManager.CreateInstance(musicEvent);
            currentInstance.start();
        }

        public void Stop(bool allowFadeOut = true)
        {
            if (!currentInstance.isValid()) return;

            parameterTweener.StopAll();

            currentInstance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentInstance.release();
            currentInstance.clearHandle();

            currentEvent = default;
        }

        public void Pause(bool paused)
        {
            if (!currentInstance.isValid()) return;
            currentInstance.setPaused(paused);
        }

        public void SetParameter(string parameter, float value)
        {
            if (!currentInstance.isValid()) return;

            parameterTweener.Stop(parameter);
            currentInstance.setParameterByName(parameter, value);
        }

        public void SetParameter(string parameter, float value, float duration)
        {
            if (!currentInstance.isValid()) return;

            parameterTweener.Tween(parameter, value, duration, () =>
                {
                    currentInstance.getParameterByName(parameter, out float currentValue);
                    return currentValue;
                },
                newValue =>
                {
                    if (currentInstance.isValid()) currentInstance.setParameterByName(parameter, newValue);
                }
            );
        }
    }
}