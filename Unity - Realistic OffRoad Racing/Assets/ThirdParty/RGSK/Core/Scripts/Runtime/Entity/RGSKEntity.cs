using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class RGSKEntity : MonoBehaviour
    {
        public IVehicle Vehicle { get; private set; }
        public ProfileDefiner ProfileDefiner { get; private set; }
        public Competitor Competitor { get; private set; }
        public DriftController DriftController { get; private set; }
        public Rigidbody Rigid { get; private set; }
        public Repositioner Repositioner { get; private set; }
        public bool IsVirtual { get; private set; }
        public bool IsPlayer { get; set; }
        public int ID => gameObject.GetInstanceID();
        public float CurrentSpeed => Rigid?.SpeedInKPH() ?? 0;

        bool _initialized;

        void Start()
        {
            if (!_initialized)
            {
                Initialize(false, false);
            }
        }

        public void Initialize(bool isPlayer, bool isVirtual)
        {
            IsPlayer = isPlayer;
            IsVirtual = isVirtual;

            ProfileDefiner = GetComponent<ProfileDefiner>();
            Competitor = GetComponent<Competitor>();
            DriftController = GetComponent<DriftController>();
            Vehicle = GetComponent<IVehicle>();
            Rigid = GetComponent<Rigidbody>();
            Repositioner = GetComponent<Repositioner>();

            foreach (var c in GetComponentsInChildren<RGSKEntityComponent>())
            {
                c.Initialize(this);
            }

            foreach (var r in GetComponentsInChildren<Rigidbody>())
            {
                //From Unity 2022.3, there is an issue with interpolated rigidbodies, so force interpolation to "None"
                r.interpolation = RigidbodyInterpolation.None;

#if UNITY_2022_3_OR_NEWER
                r.automaticCenterOfMass = false;
#endif
            }

            RGSKCore.Instance.GeneralSettings.entitySet?.AddItem(this);
            RGSKEvents.OnEntityAdded.Invoke(this);
            _initialized = true;
        }

        void OnDestroy()
        {
            if (!_initialized)
                return;

            RGSKCore.Instance.GeneralSettings.entitySet?.RemoveItem(this);
            RGSKEvents.OnEntityRemoved.Invoke(this);
        }
    }
}