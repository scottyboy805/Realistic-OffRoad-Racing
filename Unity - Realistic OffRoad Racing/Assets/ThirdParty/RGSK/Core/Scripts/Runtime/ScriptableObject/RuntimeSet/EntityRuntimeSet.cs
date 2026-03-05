using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Entity Runtime Set")]
    public class EntityRuntimeSet : RuntimeSet<RGSKEntity>
    {
        public int GetIndexOf(GameObject go)
        {
            var item = Items.FirstOrDefault(x => x.gameObject == go);

            if (item != null)
            {
                return Items.IndexOf(item);
            }

            return -1;
        }
    }
}