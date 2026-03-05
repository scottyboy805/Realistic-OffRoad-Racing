using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RGSK
{
    public class FPSText : MonoBehaviour
    {
        [SerializeField] TMP_Text fpsText;
        [SerializeField][Range(0.5f, 2)] float updateRate = 1;
        [SerializeField][TextArea] string format = "{0} fps\n{1} ms";

        float _deltaTime;
        float _nextUpdate;

        void Update()
        {
            if (fpsText == null)
                return;

            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            var msec = _deltaTime * 1000.0f;
            var fps = 1.0f / _deltaTime;

            if (Time.time > _nextUpdate)
            {
                fpsText.text = string.Format(format, fps.ToString("F0"), msec.ToString("F1"));
                _nextUpdate = Time.time + updateRate;
            }
        }
    }
}