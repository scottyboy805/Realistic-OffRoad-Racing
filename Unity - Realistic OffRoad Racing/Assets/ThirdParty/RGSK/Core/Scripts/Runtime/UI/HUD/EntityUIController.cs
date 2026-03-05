using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class EntityUIController : MonoBehaviour
    {
        public RGSKEntity entity;
        public bool autoBindToFocusedEntity;

        List<EntityUIComponent> _components = new List<EntityUIComponent>();

        protected virtual void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        protected virtual void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        IEnumerator Start()
        {
            //wait for entities to initialize
            yield return null;

            if (entity != null)
            {
                BindElements(entity);
            }
            else
            {
                AutoBind();
            }
        }

        protected virtual void Update()
        {
            foreach (var c in _components)
            {
                c.Update();
            }
        }

        void OnDestroy()
        {
            foreach (var c in _components)
            {
                c.Destroy();
            }
        }

        void OnCameraTargetChanged(Transform target)
        {
            AutoBind();
        }

        public void AddUIComponent(EntityUIComponent e)
        {
            if (_components.Contains(e))
                return;

            _components.Add(e);
        }

        void AutoBind()
        {
            if (!autoBindToFocusedEntity)
                return;
                
            BindElements(GeneralHelper.GetFocusedEntity());
        }

        public void RefreshUI()
        {
            foreach (var c in _components)
            {
                c.Refresh();
            }
        }

        public virtual void BindElements(RGSKEntity e)
        {
            entity = e;
            foreach (var c in _components)
            {
                c.Bind(entity);
            }
        }
    }
}