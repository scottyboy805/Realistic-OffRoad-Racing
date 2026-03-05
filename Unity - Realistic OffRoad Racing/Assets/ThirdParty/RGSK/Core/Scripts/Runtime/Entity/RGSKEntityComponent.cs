using UnityEngine;

namespace RGSK
{
    public abstract class RGSKEntityComponent : MonoBehaviour
    {
        public RGSKEntity Entity { get; private set; }
        public bool Initialized { get; private set; }

        public virtual void Initialize(RGSKEntity e)
        {
            Entity = e;
            Initialized = true;
        }

        public virtual void ResetValues()
        {

        }
    }
}