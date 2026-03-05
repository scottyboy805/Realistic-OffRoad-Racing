using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class VehicleLiveryRandomizer : MonoBehaviour
    {
        [ContextMenu("Randomize")]
        void Start() => GeneralHelper.SetRandomLivery(gameObject);
    }
}