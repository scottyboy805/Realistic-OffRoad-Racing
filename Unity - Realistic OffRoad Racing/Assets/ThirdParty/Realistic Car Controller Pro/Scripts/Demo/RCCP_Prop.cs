//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Prop")]
public class RCCP_Prop : RCCP_GenericComponent {

    //public Vector3 dupVector;
    //public int dupAmount;
    //public float distance;

    public float destroyAfterCollision = 3f;

    private void Awake() {

#if UNITY_2022_2_OR_NEWER
        IgnoreLayers();
#endif

    }

    private void OnEnable() {

        if (RCCPSettings.setLayers && RCCPSettings.RCCPPropLayer != "")
            gameObject.layer = LayerMask.NameToLayer(RCCPSettings.RCCPPropLayer);

        Rigidbody rigid = GetComponent<Rigidbody>();

        if (rigid)
            rigid.Sleep();

    }

    private void Reset() {

        if (RCCP_Settings.Instance.setLayers && RCCP_Settings.Instance.RCCPPropLayer != "")
            gameObject.layer = LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPPropLayer);

#if UNITY_2022_2_OR_NEWER
        IgnoreLayers();
#endif

    }

#if UNITY_2022_2_OR_NEWER
    private void IgnoreLayers() {

        //  Getting collider.
        Collider[] partColliders = GetComponentsInChildren<Collider>(true);

        LayerMask curLayerMask = -1;

        foreach (Collider collider in partColliders) {

            curLayerMask = collider.excludeLayers;
            curLayerMask |= (1 << LayerMask.NameToLayer(RCCP_Settings.Instance.RCCPWheelColliderLayer));
            collider.excludeLayers = curLayerMask;

        }

    }
#endif

    //#if UNITY_EDITOR
    //    [ContextMenu("Duplicate")]
    //    private void Duplicate() {

    //        // Get the prefab asset that this scene GameObject is connected to
    //        GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);

    //        for (int i = 1; i < dupAmount; i++) {

    //            Vector3 newPosition = transform.position + Quaternion.LookRotation(dupVector, Vector3.up) * Vector3.forward * i * distance;
    //            GameObject instantiated = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset, null);
    //            instantiated.transform.SetPositionAndRotation(newPosition, transform.rotation);

    //        }

    //    }
    //#endif

    private void OnCollisionEnter(Collision collision) {

        if (destroyAfterCollision <= 0 || collision.impulse.magnitude < 100)
            return;

        Destroy(gameObject, destroyAfterCollision);

    }

}
