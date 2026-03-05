using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Content")]
    public class RGSKContentSettings : ScriptableObject
    {
        public List<TrackDefinition> tracks = new List<TrackDefinition>();
        public List<VehicleDefinition> vehicles = new List<VehicleDefinition>();

        [HideInInspector] public VehicleSortOptions vehicleSortOptions = VehicleSortOptions.None;
        [HideInInspector] public SortOrder vehicleSortOrder = SortOrder.Ascending;
        [HideInInspector] public TrackSortOptions trackSortOptions = TrackSortOptions.None;
        [HideInInspector] public SortOrder trackSortOrder = SortOrder.Ascending;

        void OnEnable()
        {
            tracks.RemoveAll(x => x == null);
            vehicles.RemoveAll(x => x == null);
        }
    }
}