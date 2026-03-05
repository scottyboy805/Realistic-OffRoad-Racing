using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageScroller : MonoBehaviour
    {
        [SerializeField] float _xSpeed = -0.05f;
        [SerializeField] float _ySpeed = 0.05f;

        RawImage _image;
        float _xVal;
        float _yVal;

        void Awake()
        {
            _image = GetComponent<RawImage>();
        }

        void Update()
        {
            _xVal += Time.deltaTime * _xSpeed;
            _yVal += Time.deltaTime * _ySpeed;
            _image.uvRect = new Rect(_xVal, _yVal, _image.uvRect.width, _image.uvRect.height);
        }
    }
}