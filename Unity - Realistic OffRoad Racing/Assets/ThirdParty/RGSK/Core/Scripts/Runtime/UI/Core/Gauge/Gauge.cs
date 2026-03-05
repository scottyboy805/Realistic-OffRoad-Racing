using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public abstract class Gauge : MonoBehaviour
    {
        [Tooltip("Analog - the min/max angle of the needle.\nDigital - the min/max fill of the image.")]
        [SerializeField] protected Vector2 minMaxReading = new Vector2(0, 1);

        [SerializeField] protected Vector2 minMaxValue = new Vector2(0, 1);

        public float Value => _reading;

        protected float _reading;

        public virtual void SetValue(float value)
        {
            _reading = Mathf.Lerp(minMaxReading.x, minMaxReading.y,
                Mathf.InverseLerp(minMaxValue.x, minMaxValue.y, value));
        }
    }
}