using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    //[CreateAssetMenu(menuName = "RGSK/RGSK Editor Settings", order = 100)]
    public class RGSKEditorSettings : ScriptableObjectSingleton<RGSKEditorSettings>
    {
        [Header("Scene Setup")]
        public GameObject raceInitTemplate;
        public GameObject cameraManagerTemplate;
        public GameObject trackTemplate;
        public GameObject musicTemplate;

        [Header("Track Setup")]
        public GameObject checkpointRoute;
        public GameObject aiRoute;
        public GameObject standingStartGridTemplate;
        public GameObject rollingStartGridTemplate;
        public GameObject cinematicCameraTemplate;

        [Header("Vehicle Setup")]
        public GameObject vehicleParentTemplate;
        public GameObject camerasTargetTemplate;
        public GameObject vehicleSeatTemplate;
        public GameObject vehicleDriverRigTemplate;
        public GameObject vehicleLightsTemplate;
        public GameObject vehicleDashboardTemplate;
        public Wheel vehicleWheelTemplate;
        public WheelVisual vehicleWheelVisualTemplate;
        public GameObject boundingBoxTemplate;
        public List<CameraPerspective> defaultCameraPerspectives = new List<CameraPerspective>();

        [Header("Cinematic Camera Setup")]
        public GameObject lookAtCameraTemplate;
        public GameObject fixedCameraTemplate;
        public GameObject dollyCameraTemplate;

        [Header("Textures")]
        public Texture2D welcomeHeader;
        public Texture2D aiRouteSpeedzoneToolIcon;
        public Texture2D aiRouteRacinglineToolIcon;
        public Texture2D greenIcon;
        public Texture2D redIcon;
        public Texture2D yellowIcon;
        public Texture2D nullIcon;
        public Texture2D menuIcon;

        [Header("Gizmos")]
        public Color racingLineColor = Color.blue;
        public Color aiSpeedzoneColor = Color.cyan;
        public Color cornerColor = Color.yellow;
        public Color standingStartGridColor = Color.red;
        public Color rollingStartGridColor = Color.blue;

        [Header("Demo Scenes")]
        public List<SceneReference> demoScenes = new List<SceneReference>();
    }
}