using UnityEngine;

namespace RGSK
{
    public class RespawnTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            var e = col.GetComponentInParent<RGSKEntity>();
            e?.Repositioner?.Reposition();
        }
    }
}