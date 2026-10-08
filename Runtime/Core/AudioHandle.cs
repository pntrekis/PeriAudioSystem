
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PeriAudio
{
    public class AudioHandle
    {
        private EventInstance instance;
        private GameObject attachedObject;

        internal AudioHandle(EventReference eventReference)
        {
            instance = RuntimeManager.CreateInstance(eventReference);
        }

        internal AudioHandle(EventReference eventReference, GameObject go)
        {
            instance = RuntimeManager.CreateInstance(eventReference);
            attachedObject = go;

            if (go != null) RuntimeManager.AttachInstanceToGameObject(instance, go);
        }

        public bool IsValid => instance.isValid();

        public void Play()
        {
            if (!IsValid) return;

            if (attachedObject != null) instance.set3DAttributes(RuntimeUtils.To3DAttributes(attachedObject));

            instance.start();
        }

        public void Stop(bool allowFadeOut = true)
        {
            if (!IsValid) return;

            instance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        public void Pause(bool paused)
        {
            if (!IsValid) return;
            instance.setPaused(paused);
        }

        public void SetParameter(string parameter, float value)
        {
            if (!IsValid) return;
            instance.setParameterByName(parameter, value);
        }

        public float GetParameter(string parameter)
        {
            if (!IsValid) return 0f;

            instance.getParameterByName(parameter, out float value);
            return value;
        }

        public PLAYBACK_STATE GetPlaybackState()
        {
            if (!IsValid) return PLAYBACK_STATE.STOPPED;

            instance.getPlaybackState(out PLAYBACK_STATE state);
            return state;
        }

        public void Release()
        {
            if (!IsValid) return;

            instance.release();
            instance.clearHandle();
        }

        public void StopAndRelease(bool allowFadeOut = true)
        {
            Stop(allowFadeOut);
            Release();
        }
    }
}
