using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PeriAudio
{
    internal class ParameterTweener
    {
        private readonly MonoBehaviour coroutineRunner;
        private readonly Dictionary<string, Coroutine> tweens = new();

        internal ParameterTweener(MonoBehaviour coroutineRunner)
        {
            this.coroutineRunner = coroutineRunner;
        }

        public void Tween(string parameter, float targetValue, float duration, Func<float> getter, Action<float> setter)
        {
            Stop(parameter);

            if (duration <= 0f)
            {
                setter(targetValue);
                return;
            }

            tweens[parameter] = coroutineRunner.StartCoroutine(TweenRoutine(parameter, targetValue, duration, getter, setter));
        }

        public void Stop(string parameter)
        {
            if (!tweens.TryGetValue(parameter, out Coroutine tween)) return;

            coroutineRunner.StopCoroutine(tween);
            tweens.Remove(parameter);
        }

        public void StopAll()
        {
            foreach (Coroutine tween in tweens.Values)
                if (tween != null) coroutineRunner.StopCoroutine(tween);

            tweens.Clear();
        }

        private IEnumerator TweenRoutine(string parameter, float targetValue, float duration, Func<float> getter, Action<float> setter)
        {
            float startValue = getter();
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                setter(Mathf.Lerp(startValue, targetValue, t));

                yield return null;
            }

            setter(targetValue);
            tweens.Remove(parameter);
        }
    }
}