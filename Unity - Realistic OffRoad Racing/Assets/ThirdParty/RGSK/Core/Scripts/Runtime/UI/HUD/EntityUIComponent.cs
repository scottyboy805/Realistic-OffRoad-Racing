using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public abstract class EntityUIComponent : MonoBehaviour
    {
        public RGSKEntity Entity { get; private set; }

        protected virtual void Awake()
        {
            var controller = GetComponentInParent<EntityUIController>();

            if (controller != null)
            {
                if (Entity == null)
                {
                    Bind(controller.entity);
                }

                controller?.AddUIComponent(this);
            }
        }

        public virtual void Bind(RGSKEntity e)
        {
            if (e == null)
                return;

            Entity = e;
            Refresh();
        }

        public abstract void Update();
        public abstract void Refresh();
        public abstract void Destroy();
    }
}