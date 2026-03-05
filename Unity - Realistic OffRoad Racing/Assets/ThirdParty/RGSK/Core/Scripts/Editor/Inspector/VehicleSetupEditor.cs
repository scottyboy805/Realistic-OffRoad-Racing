using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Extensions;
using System.Linq;

namespace RGSK.Editor
{
    [CustomEditor(typeof(VehicleSetup))]
    public class VehicleSetupEditor : UnityEditor.Editor
    {
        VehicleSetup _target;
        SerializedProperty colliderBody;
        SerializedProperty frontAxleWheels;
        SerializedProperty rearAxleWheels;
        SerializedProperty cameraPerspectives;

        static bool addCameraTargets = true;
        static bool addDriverSeat = false;
        static bool addCollider = false;
        static bool addLights = false;
        static bool addDashboard = false;
        static bool addMeshDeformer = false;

        GUIContent[] tabs = new GUIContent[]
        {
            new GUIContent("Default", "Built-in Vehicle Physics"),
            new GUIContent("Custom", "Custom Vehicle Physics"),
            new GUIContent("EVP", "Edy's Vehicle Physics"),
            new GUIContent("RCC", "Realistic Car Controller"),
            new GUIContent("RCCPro", "Realistic Car Controller Pro"),
            new GUIContent("VPP", "Vehicle Physics Pro - Community Edition"),
            new GUIContent("NWH2", "NWH Vehicle Physics 2"),
            new GUIContent("HSC", "Highroad Solid Controller"),
            new GUIContent("ASHVP", "Ash Vehicle Physics"),
            new GUIContent("UVC", "Universal Vehicle Controller"),
            new GUIContent("SCVP", "Sim-Cade Vehicle Physics"),
        };

        static int tabIndex;

        bool hasIntegrationSupport;
        string intergrationMissingMessage = $"Please add support for this integration in the RGSK Menu!";

        void OnEnable()
        {
            _target = (VehicleSetup)target;
            colliderBody = serializedObject.FindProperty(nameof(colliderBody));
            frontAxleWheels = serializedObject.FindProperty(nameof(frontAxleWheels));
            rearAxleWheels = serializedObject.FindProperty(nameof(rearAxleWheels));
            cameraPerspectives = serializedObject.FindProperty(nameof(cameraPerspectives));

            RGSKEditorSettings.Instance.defaultCameraPerspectives.ForEach(x =>
            {
                if (!_target.cameraPerspectives.Contains(x))
                {
                    _target.cameraPerspectives.Add(x);
                }
            });
        }

        public override void OnInspectorGUI()
        {
            tabIndex = GUILayout.SelectionGrid(tabIndex, tabs, 4, CustomEditorStyles.HorizontalToolbarButton);

            if (!hasIntegrationSupport)
            {
                EditorGUILayout.HelpBox(intergrationMissingMessage, MessageType.Error);
            }

            if (tabIndex == 1) //Custom
            {
                EditorGUILayout.HelpBox("Custom vehicle physics selected!", MessageType.Warning);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                addCameraTargets = EditorGUILayout.Toggle("Add Camera Targets", addCameraTargets);
                if (addCameraTargets)
                {
                    EditorGUILayout.HelpBox("'Chase' and 'Chase Far' targets are added by default. Assign other perspectives below to add them too.", MessageType.Info);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cameraPerspectives, true);
                    EditorGUI.indentLevel--;
                }

                EditorHelper.DrawLine();
                addCollider = EditorGUILayout.Toggle("Add Collider", addCollider);
                if (addCollider)
                {
                    EditorGUILayout.PropertyField(colliderBody);
                }

                EditorHelper.DrawLine();
                addDriverSeat = EditorGUILayout.Toggle("Add Driver Seat", addDriverSeat);
                addDashboard = EditorGUILayout.Toggle("Add Dashboard", addDashboard);

                switch (tabs[tabIndex].text.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                {
                    case "default":
                        {
                            hasIntegrationSupport = true;
                            addLights = EditorGUILayout.Toggle("Add Lights", addLights);
                            addMeshDeformer = EditorGUILayout.Toggle("Add Mesh Deformer", addMeshDeformer);
                            EditorGUI.indentLevel = 0;

                            EditorHelper.DrawLine();
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(frontAxleWheels);
                            EditorGUILayout.PropertyField(rearAxleWheels);
                            EditorGUI.indentLevel--;
                            break;
                        }

                    case "custom":
                        {
                            hasIntegrationSupport = true;
                            break;
                        }

                    case "evp":
                        {
                            hasIntegrationSupport = EditorHelper.EVPSupport();
                            break;
                        }

                    case "rcc":
                        {
                            hasIntegrationSupport = EditorHelper.RCCSupport();
                            break;
                        }

                    case "rccpro":
                        {
                            hasIntegrationSupport = EditorHelper.RCCProSupport();
                            break;
                        }

                    case "vpp":
                        {
                            hasIntegrationSupport = EditorHelper.VPPSupport();
                            break;
                        }

                    case "nwh2":
                        {
                            hasIntegrationSupport = EditorHelper.NWH2Support();
                            break;
                        }

                    case "hsc":
                        {
                            hasIntegrationSupport = EditorHelper.HSCSupport();
                            break;
                        }

                    case "ashvp":
                        {
                            hasIntegrationSupport = EditorHelper.ASHVPSupport();
                            break;
                        }

                    case "uvc":
                        {
                            hasIntegrationSupport = EditorHelper.UVCSupport();
                            break;
                        }

                    case "scvp":
                        {
                            hasIntegrationSupport = EditorHelper.SCVPSupport();
                            break;
                        }
                }
            }

            serializedObject.ApplyModifiedProperties();

            EditorHelper.DrawLine();
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }
        }

        void Setup()
        {
            switch (tabs[tabIndex].text.ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "default":
                    {
                        SetupDefault();
                        break;
                    }

                case "custom":
                    {
                        AddCommonComponents(_target.gameObject);
                        break;
                    }

                case "evp":
                    {
                        if (!EditorHelper.EVPSupport())
                            return;

                        SetupEVP();
                        break;
                    }

                case "rcc":
                    {
                        if (!EditorHelper.RCCSupport())
                            return;

                        SetupRCC();
                        break;
                    }

                case "rccpro":
                    {
                        if (!EditorHelper.RCCProSupport())
                            return;

                        SetupRCCPro();
                        break;
                    }

                case "vpp":
                    {
                        if (!EditorHelper.VPPSupport())
                            return;

                        SetupVPP();
                        break;
                    }

                case "nwh2":
                    {
                        if (!EditorHelper.NWH2Support())
                            return;

                        SetupNWH2();
                        break;
                    }

                case "hsc":
                    {
                        if (!EditorHelper.HSCSupport())
                            return;

                        SetupHSC();
                        break;
                    }

                case "ashvp":
                    {
                        if (!EditorHelper.ASHVPSupport())
                            return;

                        SetupASHVP();
                        break;
                    }

                case "uvc":
                    {
                        if (!EditorHelper.UVCSupport())
                            return;

                        SetupUVC();
                        break;
                    }

                case "scvp":
                    {
                        if (!EditorHelper.SCVPSupport())
                            return;

                        SetupSCVP();
                        break;
                    }
            }

            DestroyImmediate(_target.GetComponent<VehicleSetup>());
        }

        void SetupDefault()
        {
            var parent = Instantiate(RGSKEditorSettings.Instance.vehicleParentTemplate);
            parent.name = $"{_target.name}";
            parent.transform.SetPositionAndRotation(_target.transform.position, _target.transform.rotation);

            _target.transform.SetParent(parent.transform);
            _target.transform.SetAsFirstSibling();

            SetupCollider(parent.transform);

            foreach (var w in _target.frontAxleWheels)
            {
                SetupWheel(w, WheelAxle.Front, WParent(parent.transform), WCParent(parent.transform));
            }

            foreach (var w in _target.rearAxleWheels)
            {
                SetupWheel(w, WheelAxle.Rear, WParent(parent.transform), WCParent(parent.transform));
            }

            AddCommonComponents(parent);
            AddCameraTargets(parent);
            AddLights(parent);
        }

        void SetupEVP()
        {
#if EVP_SUPPORT
            DestroyImmediate(_target.gameObject.GetComponent<EVP.VehicleStandardInput>());
            DestroyImmediate(_target.gameObject.GetComponent<EVP.VehicleRandomInput>());
            DestroyImmediate(_target.gameObject.GetComponent<EVP.RigidbodyPause>());
            DestroyImmediate(_target.gameObject.GetComponent<EVP.VehicleViewConfig>());
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<EVPSupport>();
#endif
        }

        void SetupRCC()
        {
#if RCC_SUPPORT
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<RCCSupport>();
            _target.gameObject.SetChildLayers(LayerMask.NameToLayer("RCC_Vehicle"));
#endif
        }

        void SetupRCCPro()
        {
#if RCC_PRO_SUPPORT
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<RCCProSupport>();
            _target.gameObject.SetChildLayers(LayerMask.NameToLayer("RCCP_Vehicle"));
#endif
        }

        void SetupVPP()
        {
#if VPP_SUPPORT
            DestroyImmediate(_target.gameObject.GetComponent<VehiclePhysics.VPResetVehicle>());
            DestroyImmediate(_target.gameObject.GetComponent<VehiclePhysics.VPTelemetry>());
            DestroyImmediate(_target.gameObject.GetComponent<VehiclePhysics.VPCameraTarget>());
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<VPPSupport>();
            _target.gameObject.SetChildLayers(0, false);
#endif
        }

        void SetupNWH2()
        {
#if NWH2_SUPPORT
            var vehicleController = _target.gameObject.GetComponent<NWH.VehiclePhysics2.VehicleController>();
            var cameraChanger =_target.gameObject.GetComponentInChildren<NWH.Common.Cameras.CameraChanger>();
            
            if (vehicleController != null)
            {
                vehicleController.input.autoSetInput = false;
            }

            if (cameraChanger != null)
            {
                cameraChanger.gameObject.SetActive(false);
            }

            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<NWH2Support>();
#endif
        }

        void SetupHSC()
        {
#if HSC_SUPPORT
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<HSCSupport>();
#endif
        }

        void SetupASHVP()
        {
#if ASHVP_SUPPORT
            var cam = _target.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }

            DestroyImmediate(_target.gameObject.GetComponent<AshVP.InputManager_AshVP>());
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<ASHVPSupport>();
            _target.gameObject.SetChildLayers(0, false);
#endif
        }

        void SetupUVC()
        {
#if UVC_SUPPORT
            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<UVCSupport>();
#endif
        }

        void SetupSCVP()
        {
#if SCVP_SUPPORT
            var cam = _target.gameObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }

            DestroyImmediate(_target.gameObject.GetComponent<Ashsvp.InputManager_SVP>());
            DestroyImmediate(_target.gameObject.GetComponent<Ashsvp.ResetVehicle>());

            AddCommonComponents(_target.gameObject);
            _target.gameObject.GetOrAddComponent<SCVPSupport>();
#endif
        }

        void SetupCollider(Transform parent)
        {
            if (!addCollider || _target.colliderBody == null)
                return;

            var col = new GameObject("Collider").AddComponent<MeshCollider>();
            col.sharedMesh = _target.colliderBody.sharedMesh;
            col.convex = true;

            col.transform.SetParent(parent, false);
            col.transform.position = _target.colliderBody.transform.position;
            col.transform.localScale = _target.colliderBody.transform.localScale;
        }

        void SetupWheel(Transform wheel, WheelAxle axle, Transform visualParent, Transform colliderParent)
        {
            if (wheel == null)
                return;

            var newWheel = Instantiate(wheel);
            wheel.gameObject.SetActive(false);

            var visual = Instantiate(RGSKEditorSettings.Instance.vehicleWheelVisualTemplate, visualParent);
            visual.transform.localPosition = newWheel.localPosition;
            visual.name = $"WheelVisual_{wheel.name}";
            newWheel.SetParent(visual.wheelChild, false);
            newWheel.localPosition = Vector3.zero;

            var wc = Instantiate(RGSKEditorSettings.Instance.vehicleWheelTemplate, colliderParent);
            wc.name = $"WheelCollider_{wheel.name}";
            wc.transform.position = newWheel.position + newWheel.up * wc.WheelCollider.suspensionDistance * 0.5f;
            wc.WheelCollider.radius = GetRadius();
            wc.axle = axle;
            wc.steer = wc.axle == WheelAxle.Front;
            wc.drive = wc.axle == WheelAxle.Rear;
            wc.visual = visual;

            wc.transform.SetAsLastSibling();

            float GetRadius()
            {
                var bounds = new Bounds();
                var renderers = newWheel.GetComponentsInChildren<Renderer>();

                foreach (var r in renderers)
                {
                    bounds = r.bounds;
                    bounds.Encapsulate(r.bounds);
                }

                return bounds.extents.y;
            }
        }

        Transform WParent(Transform root)
        {
            var pname = "WheelVisuals";
            var t = root.Find(pname);

            if (t == null)
            {
                t = new GameObject(pname).transform;
                t.SetParent(root, true);
            }

            return t;
        }

        Transform WCParent(Transform root)
        {
            var pname = "WheelColliders";
            var t = root.Find(pname);

            if (t == null)
            {
                t = new GameObject(pname).transform;
                t.SetParent(root, true);
            }

            return t;
        }

        void AddCameraTargets(GameObject go)
        {
            if (!addCameraTargets)
                return;

            _target.cameraPerspectives.RemoveNullElements();

            if (!go.GetComponentInChildren<CameraPerspectiveTarget>())
            {
                var root = EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.camerasTargetTemplate, go.transform, null, null, false, false);

                foreach (var p in _target.cameraPerspectives)
                {
                    var target = new GameObject(p.perspectiveName).AddComponent<CameraPerspectiveTarget>();
                    target.transform.SetParent(root.transform.GetChild(0), false);
                    target.perspectives.Add(p);
                }
            }
        }

        void AddDriverSeat(GameObject go)
        {
            if (!addDriverSeat)
                return;

            if (!go.GetComponentInChildren<VehicleSeat>())
            {
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.vehicleSeatTemplate, go.transform, null, null, false, false);
                go.GetOrAddComponent<VehicleDriverPlacer>();
            }
        }

        void AddLights(GameObject go)
        {
            if (!addLights)
                return;

            EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.vehicleLightsTemplate, go.transform, null, null, false, false);
        }

        void AddDashboard(GameObject go)
        {
            if (!addDashboard)
                return;

            var dash = EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.vehicleDashboardTemplate, go.transform, null, null, false, false);
            if (dash.TryGetComponent<EntityUIController>(out var ui))
            {
                ui.entity = go.GetComponent<RGSKEntity>();
            }
        }

        void AddMeshDeformer(GameObject go)
        {
            if (!addMeshDeformer)
                return;

            go.GetOrAddComponent<MeshDeformation>();
        }

        void AddCommonComponents(GameObject go)
        {
            go.GetOrAddComponent<RGSKEntity>();
            go.GetOrAddComponent<VehicleDefiner>();
            go.GetOrAddComponent<AIController>();
            go.GetOrAddComponent<Repositioner>();

            AddCameraTargets(go);
            AddDriverSeat(go);
            AddDashboard(go);
            AddMeshDeformer(go);

            var boundingBox = go.GetComponentInChildren<BoundingBox>();

            if (boundingBox == null)
            {
                boundingBox = EditorHelper.CreatePrefab(
                RGSKEditorSettings.Instance.boundingBoxTemplate,
                go.transform,
                null,
                null,
                false,
                false).GetComponent<BoundingBox>();
            }

            boundingBox.AutoCalculateBounds();
        }
    }
}