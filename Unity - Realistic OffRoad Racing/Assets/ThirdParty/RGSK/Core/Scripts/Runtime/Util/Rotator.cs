using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] Axis axis = Axis.Y;
        [SerializeField] float speed = 30;

        void Update()
        {
            switch (axis)
            {
                case Axis.X:
                {
                    transform.Rotate(speed * Time.deltaTime, 0, 0);
                    break;
                }

                case Axis.Y:
                {
                    transform.Rotate(0, speed * Time.deltaTime, 0);
                    break;
                }

                case Axis.Z:
                {
                    transform.Rotate(0, 0, speed * Time.deltaTime);
                    break;
                }
            }
        }
    }
}
