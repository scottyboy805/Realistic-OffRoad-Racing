using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class Odometer : MonoBehaviour
    {
        [SerializeField] int distance;

        public int Distance
        {
            get => distance;
            set => distance = value;
        }

        IVehicle _vehicle = null;
        Vector3 _lastPos;
        int _threshold = 10;

        void Start()
        {
            _lastPos = transform.position;
        }

        public void Initialize(IVehicle vehicle)
        {
            _vehicle = vehicle;
        }

        void Update()
        {
            var pos = transform.position;
            var dist = Vector3.Distance(pos, _lastPos);

            if (Mathf.Abs(dist) > _threshold)
            {
                distance += _threshold;

                if (PlayerVehicleInput.Instance != null && PlayerVehicleInput.Instance.Vehicle == _vehicle)
                {
                    SaveData.Instance.playerData.totalDistance += _threshold;
                }

                _lastPos = pos;
            }
        }
    }
}