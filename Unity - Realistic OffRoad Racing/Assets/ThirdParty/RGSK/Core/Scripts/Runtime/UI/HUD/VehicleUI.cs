using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    public class VehicleUI : EntityUIComponent
    {
        public TMP_Text speedText;
        public TMP_Text gearText;
        public TMP_Text rpmText;
        public TMP_Text speedUnitText;
        public TMP_Text mileageText;

        [Header("Gauges")]
        public Gauge tachometer;
        public Gauge speedometer;
        public bool normalizeRPM = true;

        [Header("Nitrous")]
        public GameObject nitrousHUD;
        public Gauge nitroMeter;

        [Header("Info")]
        public VehicleDefinitionUI vehicleDefinitionUI;
        public Image vehicleColorImage;

        [Header("Input")] 
        public Gauge throttleBar;
        public Gauge brakeBar;
        public RectTransform steeringRect;

        [Header("Extra")]
        public Graphic redlineGraphic;
        public Color redlineColor = Color.red;

        Color _defaultRedlineColor;

        protected override void Awake()
        {
            base.Awake();

            if (redlineGraphic != null)
            {
                _defaultRedlineColor = redlineGraphic.color;
            }
        }

        public override void Bind(RGSKEntity e)
        {
            base.Bind(e);
            vehicleDefinitionUI?.UpdateUI(Entity?.Vehicle?.VehicleDefinition);
        }

        public override void Update()
        {
            if (Entity == null || Entity.Vehicle == null)
                return;

            speedText?.SetText(UIHelper.FormatSpeedText(Entity.Vehicle.CurrentSpeed));
            gearText?.SetText(UIHelper.FormatGearText(Entity.Vehicle.CurrentGear));
            rpmText?.SetText(Entity.Vehicle.EngineRPM.ToString("F0"));
            speedUnitText?.SetText(UIHelper.GetSpeedUnitSymbol(RGSKCore.Instance.UISettings.speedUnit));
            tachometer?.SetValue(normalizeRPM ? Entity.Vehicle.EngineRPM / Entity.Vehicle.MaxEngineRPM : Entity.Vehicle.EngineRPM);
            speedometer?.SetValue(Entity.Vehicle.CurrentSpeed);
            nitroMeter?.SetValue(Entity.Vehicle.NitrousCapacity);
            throttleBar?.SetValue(Entity.Vehicle.ThrottleInput);
            brakeBar?.SetValue(Entity.Vehicle.BrakeInput);
            mileageText?.SetText(((int)ConversionHelper.ConvertDistance(Entity.Vehicle.OdometerReading, RGSKCore.Instance.UISettings.distanceUnit)).ToString("D6"));

            if(steeringRect != null)
            {
                var rot = Quaternion.Euler(0, 0, Entity.Vehicle.SteerInput * -90);
                steeringRect.rotation = Quaternion.Lerp(steeringRect.rotation, rot, Time.deltaTime * 3);
            }

            if (redlineGraphic != null)
            {
                redlineGraphic.color = Entity.Vehicle.EngineRPM > Entity.Vehicle.MaxEngineRPM ?
                                    redlineColor :
                                    _defaultRedlineColor;
            }
        }

        public override void Refresh()
        {
            if (Entity == null || Entity.Vehicle == null)
                return;

            if (nitrousHUD != null)
            {
                nitrousHUD.SetActiveSafe(Entity.Vehicle.HasNitrous);
            }

            if (vehicleColorImage != null)
            {
                vehicleColorImage.color = GeneralHelper.GetColor(Entity.gameObject, 0);
            }
        }

        public override void Destroy() { }
    }
}