using System.Collections;
using UnityEngine;

namespace InGame.Effects
{
    public class CameraShake : IEffect
    {
        public Camera Cam { get; set; }
        public float Duration { get; set; } = 0.2f;
        public float Magnitude { get; set; } = 0.1f;
        
        
        public IEnumerator DoEffect()
        {
            if (!Cam) yield break;
            var originalPos = Vector3.zero;
            originalPos.z = -10;
            var elapsed = 0f;

            while (elapsed < Duration)
            {
                float offsetX = Random.Range(-1f, 1f) * Magnitude;
                float offsetY = Random.Range(-1f, 1f) * Magnitude;

                Cam.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Cam.transform.localPosition = originalPos;
        }

        public void CloneStats(IEffect target)
        {
            var casted = (CameraShake)target;
            Cam = casted.Cam;
            Duration = casted.Duration;
            Magnitude = casted.Magnitude;
        }
    }
}