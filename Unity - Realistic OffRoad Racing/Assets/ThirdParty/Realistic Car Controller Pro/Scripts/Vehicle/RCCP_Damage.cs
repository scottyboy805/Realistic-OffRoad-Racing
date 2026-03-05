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
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Deforms the meshes, wheels, lights, and other parts of the vehicle.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Damage")]
public class RCCP_Damage : RCCP_Component {

    private bool initialized = false;

    public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// Save file name will be used to save / load damage data with json.
    /// </summary>
    public string saveName = "";

    /// <summary>
    /// Collected mesh filters.
    /// </summary>
    public MeshFilter[] meshFilters;

    /// <summary>
    /// Collected lights.
    /// </summary>
    public RCCP_Light[] lights;

    /// <summary>
    /// Collected parts.
    /// </summary>
    public RCCP_DetachablePart[] parts;

    /// <summary>
    /// Collected wheels.
    /// </summary>
    public RCCP_WheelCollider[] wheels;

    /// <summary>
    /// If set to enabled, all parts of the vehicle will be processed. If disabled, each part can be selected individually.
    /// </summary>
    public bool automaticInstallation = true;

    /// <summary>
    /// LayerMask filter. Damage will be taken from the objects with these layers.
    /// </summary>
    public LayerMask damageFilter = -1;

    /// <summary>
    /// Maximum Vert Distance For Limiting Damage. 0 Value Will Disable The Limit.
    /// </summary>
    [Min(0f)] public float maximumDamage = .75f;

    /// <summary>
    /// Process inactive gameobjects too?
    /// </summary>
    public bool processInactiveGameobjects = false;

    /// <summary>
    /// Mesh deformation
    /// </summary>
    [Space()] public bool meshDeformation = true;

    /// <summary>
    /// Verticies in this radius will be effected on collisions.
    /// </summary>
    [Min(0f)] public float deformationRadius = .75f;

    /// <summary>
    /// Damage multiplier.
    /// </summary>
    [Min(0f)] public float deformationMultiplier = 1f;

    /// <summary>
    /// Minimum collision force.
    /// </summary>
    private readonly float minimumCollisionImpulse = .5f;

    /// <summary>
    /// Comparing Original Vertex Positions Between Last Vertex Positions To Decide Mesh Is Repaired Or Not.
    /// </summary>
    private readonly float minimumVertDistanceForDamagedMesh = .002f;

    /// <summary>
    /// Struct for Original Mesh Verticies positions.
    /// </summary>
    [System.Serializable] public struct OriginalMeshVerts { public Vector3[] meshVerts; }

    /// <summary>
    /// Struct for Original Wheel positions and rotations.
    /// </summary>
    [System.Serializable] public struct OriginalWheelPos { public Vector3 wheelPosition; public Quaternion wheelRotation; }

    public OriginalMeshVerts[] originalMeshData;        // Array for struct above.
    public OriginalMeshVerts[] damagedMeshData;     // Array for struct above.
    public OriginalWheelPos[] originalWheelData;       // Array for struct above.
    public OriginalWheelPos[] damagedWheelData;        // Array for struct above.

    /// <summary>
    /// Damage data class.
    /// </summary> 
    [System.Serializable]
    public class DamageData {

        public OriginalMeshVerts[] originalMeshData;        // Array for struct above.
        public OriginalMeshVerts[] damagedMeshData;     // Array for struct above.
        public OriginalWheelPos[] originalWheelData;       // Array for struct above.
        public OriginalWheelPos[] damagedWheelData;        // Array for struct above.
        public bool[] lightData;        //  Array for struct above.

        public void Initialize(RCCP_Damage damageComponent) {

            originalMeshData = damageComponent.originalMeshData;
            damagedMeshData = damageComponent.damagedMeshData;
            originalWheelData = damageComponent.originalWheelData;
            damagedWheelData = damageComponent.damagedWheelData;
            lightData = new bool[damageComponent.lights.Length];

            for (int i = 0; i < lightData.Length; i++) {

                if (damageComponent.lights[i] != null)
                    lightData[i] = damageComponent.lights[i].broken;

            }

        }

    }

    /// <summary>
    /// Creating a new damage data.
    /// </summary>
    public DamageData damageData = new DamageData();

    /// <summary>
    /// Repairing now.
    /// </summary>
    [Space()] public bool repairNow = false;

    /// <summary>
    /// Returns true if vehicle is completely repaired.
    /// </summary>
    public bool repaired = true;

    /// <summary>
    /// Deforming the mesh now.
    /// </summary>
    public bool deformingNow = false;

    /// <summary>
    /// Returns true if vehicle is completely deformed.
    /// </summary>
    public bool deformed = false;

    /// <summary>
    /// Recalculate normals while deforming / restoring the mesh.
    /// </summary>
    [Space()] public bool recalculateNormals = false;

    /// <summary>
    /// Recalculate bounds while deforming / restoring the mesh.
    /// </summary>
    public bool recalculateBounds = false;

    /// <summary>
    /// Wheel deformation. Use wheel damage.
    /// </summary>
    [Space()] public bool wheelDamage = true;

    /// <summary>
    /// Wheel damage radius.
    /// </summary>
    [Min(0f)] public float wheelDamageRadius = 2f;

    /// <summary>
    /// Wheel damage multiplier.
    /// </summary>
    [Min(0f)] public float wheelDamageMultiplier = 1f;

    /// <summary>
    /// Use wheel detachment.
    /// </summary>
    public bool wheelDetachment = true;

    /// <summary>
    /// Light deformation. Use light damage.
    /// </summary>
    [Space()] public bool lightDamage = true;

    /// <summary>
    /// Light damage radius.
    /// </summary>
    [Min(0f)] public float lightDamageRadius = .75f;

    /// <summary>
    /// Light damage multiplier.
    /// </summary>
    [Min(0f)] public float lightDamageMultiplier = 1f;

    /// <summary>
    /// Part deformation. Use part damage.
    /// </summary>
    [Space()] public bool partDamage = true;

    /// <summary>
    /// Part damage radius.
    /// </summary>
    [Min(0f)] public float partDamageRadius = 1f;

    /// <summary>
    /// Part damage multiplier.
    /// </summary>
    [Min(0f)] public float partDamageMultiplier = 1f;

    /// <summary>
    /// Contact point.
    /// </summary>
    private ContactPoint contactPoint = new ContactPoint();

    public RCCP_Octree[] octrees;

    /// <summary>
    /// Collecting all meshes and detachable parts of the vehicle.
    /// </summary>
    public override void Start() {

        base.Start();
        Reload();
        Initialize();

    }

    private void Initialize() {

        if (initialized)
            return;

        if (automaticInstallation) {

            MeshFilter[] allMeshFilters = CarController.gameObject.GetComponentsInChildren<MeshFilter>(processInactiveGameobjects);
            List<MeshFilter> properMeshFilters = new List<MeshFilter>();

            // Model import must be readable. If it's not readable, inform the developer. We don't wanna deform wheel meshes. Exclude any meshes belongs to the wheels.
            foreach (MeshFilter mf in allMeshFilters) {

                if (mf.mesh != null) {

                    if (!mf.mesh.isReadable)
                        Debug.LogError("Not deformable mesh detected. Mesh of the " + mf.transform.name + " isReadable is false; Read/Write must be enabled in import settings for this model to use with damage component!");
                    else
                        properMeshFilters.Add(mf);

                }

            }

            for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

                if (CarController.AllWheelColliders[i] != null && CarController.AllWheelColliders[i].wheelModel != null) {

                    for (int k = 0; k < properMeshFilters.Count; k++) {

                        if (properMeshFilters[k] != null) {

                            if (properMeshFilters[k].transform.IsChildOf(CarController.AllWheelColliders[i].wheelModel))
                                properMeshFilters.RemoveAt(k);

                        }

                    }

                }

            }

            meshFilters = properMeshFilters.ToArray();
            parts = CarController.GetComponentsInChildren<RCCP_DetachablePart>(processInactiveGameobjects);
            lights = CarController.GetComponentsInChildren<RCCP_Light>(processInactiveGameobjects);
            wheels = CarController.GetComponentsInChildren<RCCP_WheelCollider>(processInactiveGameobjects);

        }

        CheckMeshData();
        CheckWheelData();

        initialized = true;

    }

    private void Update() {

        if (RCCPSceneManager.mutlithreadingSupported) {

            if (!repaired && repairNow)
                CheckRepair();

            if (!deformed && deformingNow)
                CheckDamage();

        } else {

            if (!repaired && repairNow)
                CheckRepairRaw();

            if (!deformed && deformingNow)
                CheckDamageRaw();

        }

    }

    /// <summary>
    /// We will be using two structs for deformed sections. Original part struction, and deformed part struction. 
    /// All damaged meshes and wheel transforms will be using these structs. At this section, we're creating them with original struction.
    /// </summary>
    private void CheckMeshData() {

        originalMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {

            meshFilters[i].mesh.MarkDynamic();
            originalMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

        }

        damagedMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
            damagedMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

    }

    /// <summary>
    /// We will be using two structs for deformed sections. Original part struction, and deformed part struction. 
    /// All damaged meshes and wheel transforms will be using these structs. At this section, we're creating them with original struction.
    /// </summary>
    private void CheckWheelData() {

        originalWheelData = new OriginalWheelPos[CarController.AllWheelColliders.Length];

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            originalWheelData[i].wheelPosition = CarController.AllWheelColliders[i].transform.localPosition;
            originalWheelData[i].wheelRotation = CarController.AllWheelColliders[i].transform.localRotation;

        }

        damagedWheelData = new OriginalWheelPos[CarController.AllWheelColliders.Length];

        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            damagedWheelData[i].wheelPosition = CarController.AllWheelColliders[i].transform.localPosition;
            damagedWheelData[i].wheelRotation = CarController.AllWheelColliders[i].transform.localRotation;

        }

    }

    /// <summary>
    /// Moving deformed vertices to their original positions while repairing.
    /// </summary>
    public void CheckRepairRaw() {

        if (!CarController || !initialized)
            return;

        //  If vehicle is not repaired completely, and repairNow is enabled, restore all deformed meshes to their original structions.
        if (!repaired && repairNow) {

            repaired = true;
            repairNow = false;

            int k;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the original positions. If verticies are close enough to the original
            //  position, repaired = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    for (int i = 0; i < vertices.Length; i++) {

                        //  And then move all single verticies to the original positions
                        vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]);

                        //  If verticies are close enough to their original positions, repaired = true;
                        if ((originalMeshData[k].meshVerts[i] - vertices[i]).sqrMagnitude >= (minimumVertDistanceForDamagedMesh * minimumVertDistanceForDamagedMesh))
                            repaired = false;

                    }

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedMeshData[k].meshVerts = vertices;

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    //  You may want to disable last three lines.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    //  Get all verticies of the mesh first.
                    Vector3 wheelPos = CarController.AllWheelColliders[k].transform.localPosition;

                    //  And then move all single verticies to the original positions
                    wheelPos += (originalWheelData[k].wheelPosition - wheelPos);

                    //  If verticies are close enough to their original positions, repaired = true;
                    if ((originalWheelData[k].wheelPosition - wheelPos).sqrMagnitude >= (minimumVertDistanceForDamagedMesh * minimumVertDistanceForDamagedMesh))
                        repaired = false;

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedWheelData[k].wheelPosition = wheelPos;

                    CarController.AllWheelColliders[k].transform.localPosition = wheelPos;
                    CarController.AllWheelColliders[k].transform.localRotation = Quaternion.identity;

                    if (!CarController.AllWheelColliders[k].WheelCollider.enabled)
                        CarController.AllWheelColliders[k].WheelCollider.enabled = true;

                    CarController.AllWheelColliders[k].Inflate();

                }

            }

            //  Repairing and restoring all detachable parts of the vehicle.
            for (int i = 0; i < parts.Length; i++) {

                if (parts[i] != null)
                    parts[i].OnRepair();

            }

            //  Repairing and restoring all lights of the vehicle.
            if (CarController.Lights) {

                for (int i = 0; i < CarController.Lights.lights.Count; i++) {

                    if (CarController.Lights.lights[i] != null)
                        CarController.Lights.lights[i].OnRepair();

                }

            }

            //  If all meshes are completely restored, make sure repairing now is false.
            if (repaired)
                repairNow = false;

        }

    }

    /// <summary>
    /// Moving vertices of the collided meshes to the damaged positions while deforming.
    /// </summary>
    public void CheckDamageRaw() {

        if (!CarController || !initialized)
            return;

        //  If vehicle is not deformed completely, and deforming is enabled, deform all meshes to their damaged structions.
        if (!deformed && deformingNow) {

            deformed = true;
            deformingNow = false;

            int k;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the damaged positions. If verticies are close enough to the original
            //  position, deformed = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    Mesh mesh = meshFilters[k].mesh;

                    //  And then move all single verticies to the damaged positions.
                    for (int i = 0; i < vertices.Length; i++) {

                        if (cancellationTokenSource.IsCancellationRequested)
                            return;

                        vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]);

                    }

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        mesh.RecalculateNormals();

                    if (recalculateBounds)
                        mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    Vector3 vertices = CarController.AllWheelColliders[k].transform.localPosition;
                    vertices += (damagedWheelData[k].wheelPosition - vertices);
                    CarController.AllWheelColliders[k].transform.localPosition = vertices;

                }

            }

            //  If all meshes are completely deformed, make sure deforming is false and timer is set to 0.
            deformingNow = false;
            deformed = true;

        }

    }

    /// <summary>
    /// Deforming meshes.
    /// </summary>
    /// <param name="impulse"></param>
    private void DamageMesh(float impulse) {

        if (!CarController || !initialized)
            return;

        if (meshFilters == null || (meshFilters != null && meshFilters.Length < 1))
            return;

        if (originalMeshData == null || originalMeshData.Length < 1)
            CheckMeshData();

        Transform carTransform = CarController.transform;  // Cache the car's transform

        Vector3 localContactPointRelativeToRoot = carTransform.InverseTransformPoint(contactPoint.point);

        // Calculate the collision direction in local space and reverse it
        Vector3 collisionDirection = -(localContactPointRelativeToRoot).normalized;

        if (octrees == null || (octrees != null && octrees.Length < 1))
            octrees = new RCCP_Octree[meshFilters.Length];

        //  We will be checking all mesh filters with these contact points. If contact point is close enough to the mesh, deformation will be applied.
        for (int i = 0; i < meshFilters.Length; i++) {

            MeshFilter currentMeshFilter = meshFilters[i];

            if (currentMeshFilter == null || currentMeshFilter.mesh == null || !currentMeshFilter.gameObject.activeSelf)
                continue;

            // Create an Octree with bounds in world space
            if (octrees[i] == null) {

                octrees[i] = new RCCP_Octree(currentMeshFilter);

                foreach (var vertex in currentMeshFilter.mesh.vertices)
                    octrees[i].Insert(vertex); // Insert the local-space vertex into the Octree.

            }

            Vector3 localContactPointRelativeToMesh = currentMeshFilter.transform.InverseTransformPoint(contactPoint.point);

            //  Getting closest point to the mesh.
            Vector3 nearestVert = NearestVertexWithOctree(i, localContactPointRelativeToMesh, currentMeshFilter);

            // Distance.
            float distance = (nearestVert - localContactPointRelativeToMesh).sqrMagnitude;

            //  If distance between contact point and closest point of the mesh is in range...
            if (distance <= (deformationRadius * deformationRadius)) {

                // All vertices of the mesh
                Vector3[] vertices = damagedMeshData[i].meshVerts;

                for (int k = 0; k < vertices.Length; k++) {

                    float distanceToVertSqr = (localContactPointRelativeToMesh - vertices[k]).sqrMagnitude;

                    if (distanceToVertSqr <= (deformationRadius * deformationRadius)) {

                        //  Calculate damage based on the impulse and distance
                        float damage = impulse * (1f - Mathf.Clamp01(Mathf.Sqrt(distanceToVertSqr) / deformationRadius));

                        // Apply deformation in the local space of the mesh (in the reverse direction)
                        vertices[k] += collisionDirection * damage * (deformationMultiplier / 10f);

                        //  If deformation exceeds limits, apply limits
                        if (maximumDamage > 0f && (vertices[k] - originalMeshData[i].meshVerts[k]).sqrMagnitude > (maximumDamage * maximumDamage))
                            vertices[k] = originalMeshData[i].meshVerts[k] + (vertices[k] - originalMeshData[i].meshVerts[k]).normalized * maximumDamage;

                    }

                }

            }

        }

    }

    /// <summary>
    /// Moving deformed vertices to their original positions while repairing.
    /// </summary>
    public async void CheckRepair() {

        if (!CarController || !initialized)
            return;

        //  If vehicle is not repaired completely, and repairNow is enabled, restore all deformed meshes to their original structions.
        if (!repaired && repairNow) {

            repaired = true;
            repairNow = false;

            int k;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the original positions. If verticies are close enough to the original
            //  position, repaired = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    if (cancellationTokenSource == null)
                        cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(() => {

                        for (int i = 0; i < vertices.Length; i++) {

                            //  And then move all single verticies to the original positions
                            vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]);

                            //  If verticies are close enough to their original positions, repaired = true;
                            if ((originalMeshData[k].meshVerts[i] - vertices[i]).sqrMagnitude >= (minimumVertDistanceForDamagedMesh * minimumVertDistanceForDamagedMesh))
                                repaired = false;

                        }

                    }, cancellationTokenSource.Token);

                    if (cancellationTokenSource.IsCancellationRequested)
                        return;

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedMeshData[k].meshVerts = vertices;

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    //  You may want to disable last three lines.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    //  Get all verticies of the mesh first.
                    Vector3 wheelPos = CarController.AllWheelColliders[k].transform.localPosition;

                    //  And then move all single verticies to the original positions
                    wheelPos += (originalWheelData[k].wheelPosition - wheelPos);

                    //  If verticies are close enough to their original positions, repaired = true;
                    if ((originalWheelData[k].wheelPosition - wheelPos).sqrMagnitude >= (minimumVertDistanceForDamagedMesh * minimumVertDistanceForDamagedMesh))
                        repaired = false;

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedWheelData[k].wheelPosition = wheelPos;

                    CarController.AllWheelColliders[k].transform.localPosition = wheelPos;
                    CarController.AllWheelColliders[k].transform.localRotation = Quaternion.identity;

                    if (!CarController.AllWheelColliders[k].WheelCollider.enabled)
                        CarController.AllWheelColliders[k].WheelCollider.enabled = true;

                    CarController.AllWheelColliders[k].Inflate();

                }

            }

            //  Repairing and restoring all detachable parts of the vehicle.
            for (int i = 0; i < parts.Length; i++) {

                if (parts[i] != null)
                    parts[i].OnRepair();

            }

            //  Repairing and restoring all lights of the vehicle.
            if (CarController.Lights) {

                for (int i = 0; i < CarController.Lights.lights.Count; i++) {

                    if (CarController.Lights.lights[i] != null)
                        CarController.Lights.lights[i].OnRepair();

                }

            }

            //  If all meshes are completely restored, make sure repairing now is false.
            if (repaired)
                repairNow = false;

        }

    }

    /// <summary>
    /// Moving vertices of the collided meshes to the damaged positions while deforming.
    /// </summary>
    public async void CheckDamage() {

        if (!CarController || !initialized)
            return;

        //  If vehicle is not deformed completely, and deforming is enabled, deform all meshes to their damaged structions.
        if (!deformed && deformingNow) {

            deformed = true;
            deformingNow = false;

            int k;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the damaged positions. If verticies are close enough to the original
            //  position, deformed = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null && meshFilters[k].mesh != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    Mesh mesh = meshFilters[k].mesh;

                    if (cancellationTokenSource == null)
                        cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(() => {

                        //  And then move all single verticies to the damaged positions.
                        for (int i = 0; i < vertices.Length; i++) {

                            if (cancellationTokenSource.IsCancellationRequested)
                                return;

                            vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]);

                        }

                    }, cancellationTokenSource.Token);

                    if (cancellationTokenSource.IsCancellationRequested)
                        return;

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        mesh.RecalculateNormals();

                    if (recalculateBounds)
                        mesh.RecalculateBounds();

                }

            }

            for (k = 0; k < CarController.AllWheelColliders.Length; k++) {

                if (CarController.AllWheelColliders[k] != null) {

                    Vector3 vertices = CarController.AllWheelColliders[k].transform.localPosition;
                    vertices += (damagedWheelData[k].wheelPosition - vertices);
                    CarController.AllWheelColliders[k].transform.localPosition = vertices;

                }

            }

        }

    }

    /// <summary>
    /// Deforming wheels. Actually changing their local positions and rotations based on the impact.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageWheel(float impulse) {

        if (!CarController || !initialized)
            return;

        if (originalWheelData == null || originalWheelData.Length < 1)
            CheckWheelData();

        Transform carTransform = CarController.transform;  // Cache the car's transform

        // Pre-calculate the collision direction outside the loop
        Vector3 collisionDirection = -((contactPoint.point - CarController.transform.position).normalized);

        for (int i = 0; i < wheels.Length; i++) {

            if (wheels[i] != null && wheels[i].gameObject.activeSelf) {

                Vector3 wheelPos = damagedWheelData[i].wheelPosition;

                // Calculate the closest point on the wheel collider
                Vector3 closestPoint = wheels[i].WheelCollider.ClosestPointOnBounds(contactPoint.point);
                float distanceSqr = (closestPoint - contactPoint.point).sqrMagnitude;  // Use squared magnitude for optimization

                // If the distance between the contact point and the closest point on the wheel collider is within range
                if (distanceSqr < (wheelDamageRadius * wheelDamageRadius)) {

                    float damage = (impulse * wheelDamageMultiplier) / 30f;

                    // Decrease damage based on the distance from the contact point
                    damage -= damage * Mathf.Clamp01(distanceSqr / (wheelDamageRadius * wheelDamageRadius)) * .5f;

                    Vector3 vW = carTransform.TransformPoint(wheelPos);
                    vW += (collisionDirection * damage);
                    wheelPos = carTransform.InverseTransformPoint(vW);

                    // If damage exceeds the maximum allowed, either cap the damage or detach the wheel
                    if (maximumDamage > 0 && (wheelPos - originalWheelData[i].wheelPosition).sqrMagnitude > (maximumDamage * maximumDamage)) {

                        // Uncomment this if you want to limit the damage instead of detaching
                        // wheelPos = originalWheelData[i].wheelPosition + (wheelPos - originalWheelData[i].wheelPosition).normalized * maximumDamage;

                        if (wheelDetachment && wheels[i].gameObject.activeSelf)
                            DetachWheel(wheels[i]);  // Detach the wheel if it's active and damage is over the threshold

                    }

                    // Update the damaged wheel position
                    damagedWheelData[i].wheelPosition = wheelPos;

                }

            }

        }

    }

    /// <summary>
    /// Deforming the detachable parts.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamagePart(float impulse) {

        if (!CarController)
            return;

        if (!initialized)
            return;

        if (parts != null && parts.Length >= 1) {

            for (int i = 0; i < parts.Length; i++) {

                if (parts[i] != null && parts[i].gameObject.activeSelf) {

                    if (parts[i].partColliders != null) {

                        Vector3 closestPoint = parts[i].partColliders[0].ClosestPointOnBounds(contactPoint.point);

                        float distance = (closestPoint - contactPoint.point).sqrMagnitude;
                        float damage = impulse * partDamageMultiplier;

                        // The damage should decrease with distance from the contact point.
                        damage -= damage * Mathf.Clamp01(distance / (partDamageRadius * partDamageRadius)) * .5f;

                        if (distance <= (partDamageRadius * partDamageRadius))
                            parts[i].OnCollision(damage);

                    } else {

                        if ((contactPoint.point - parts[i].transform.position).sqrMagnitude < (partDamageRadius * partDamageRadius))
                            parts[i].OnCollision(impulse);

                    }

                }

            }

        }

    }

    /// <summary>
    /// Deforming the lights.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageLight(float impulse) {

        if (!CarController)
            return;

        if (!initialized)
            return;

        if (lights != null && lights.Length >= 1) {

            for (int i = 0; i < lights.Length; i++) {

                if (lights[i] != null && lights[i].gameObject.activeSelf) {

                    if ((contactPoint.point - lights[i].transform.position).sqrMagnitude < (lightDamageRadius * lightDamageRadius)) {

                        float distance = (lights[i].transform.position - contactPoint.point).sqrMagnitude;
                        float damage = impulse * lightDamageMultiplier;

                        // The damage should decrease with distance from the contact point.
                        damage -= damage * Mathf.Clamp01(distance / (lightDamageRadius * lightDamageRadius)) * .5f;

                        if (distance <= (lightDamageRadius * lightDamageRadius))
                            lights[i].OnCollision(damage);

                    }

                }

            }

        }

    }

    /// <summary>
    /// Detach wheel.
    /// </summary>
    /// <param name="wheelCollider"></param>
    public void DetachWheel(RCCP_WheelCollider wheelCollider) {

        if (!CarController || !initialized)
            return;

        if (!wheelCollider)
            return;

        if (!wheelCollider.enabled)
            return;

        wheelCollider.WheelCollider.enabled = false;
        Transform wheelModel = wheelCollider.wheelModel;

        GameObject clonedWheel = Instantiate(wheelModel.gameObject, wheelModel.transform.position, wheelModel.transform.rotation, null);
        clonedWheel.SetActive(true);
        clonedWheel.AddComponent<Rigidbody>();

        GameObject clonedMeshCollider = new GameObject("Mesh Collider");
        clonedMeshCollider.transform.SetParent(clonedWheel.transform, false);
        clonedMeshCollider.transform.position = RCCP_GetBounds.GetBoundsCenter(clonedWheel.transform);
        MeshCollider mc = clonedMeshCollider.AddComponent<MeshCollider>();
        MeshFilter biggestMesh = RCCP_GetBounds.GetBiggestMesh(clonedWheel.transform);
        mc.sharedMesh = biggestMesh.mesh;
        mc.convex = true;

        //carController.ESPBroken = true;

    }

    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">Collision.</param>
    public void OnCollision(Collision collision) {

        if (!enabled)
            return;

        if (((1 << collision.gameObject.layer) & damageFilter) != 0) {

            float impulse = collision.impulse.magnitude / 7500f;

            if (impulse < minimumCollisionImpulse)
                impulse = 0f;

            if (impulse > 10f)
                impulse = 10f;

            if (impulse > 0f) {

                deformingNow = true;
                deformed = false;

                repairNow = false;
                repaired = false;

                //  First, we are getting first contact point.
                contactPoint = collision.GetContact(0);

                if (meshDeformation && meshFilters != null && meshFilters.Length >= 1)
                    DamageMesh(impulse);

                if (wheelDamage && CarController.AllWheelColliders != null && CarController.AllWheelColliders.Length >= 1)
                    DamageWheel(impulse);

                if (partDamage && parts != null && parts.Length >= 1)
                    DamagePart(impulse);

                if (lightDamage && CarController.Lights && CarController.Lights.lights.Count >= 1)
                    DamageLight(impulse);

            }

        }

    }

    /// <summary>
    /// Finds closest vertex to the target point.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="mf"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 NearestVertexRaw(Transform trans, MeshFilter mf, Vector3 point) {

        // Convert point to local space.
        point = trans.InverseTransformPoint(point);

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        Vector3[] vertex = mf.mesh.vertices;

        for (int i = 0; i < vertex.Length; i++) {

            Vector3 diff = point - vertex[i];
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr) {

                minDistanceSqr = distSqr;
                nearestVertex = vertex[i];

            }

        }

        // Convert nearest vertex back to the world space.
        return nearestVertex;

    }

    /// <summary>
    /// Finds closest vertex to the target point.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="mf"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public async Task<Vector3> NearestVertex(Transform trans, MeshFilter mf, Vector3 point) {

        // Convert point to local space.
        point = trans.InverseTransformPoint(point);

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        Vector3[] vertex = mf.mesh.vertices;

        if (cancellationTokenSource == null)
            cancellationTokenSource = new CancellationTokenSource();

        await Task.Run(() => {

            for (int i = 0; i < vertex.Length; i++) {

                Vector3 diff = point - vertex[i];
                float distSqr = diff.sqrMagnitude;

                if (distSqr < minDistanceSqr) {

                    minDistanceSqr = distSqr;
                    nearestVertex = vertex[i];

                }

            }

            return nearestVertex;

        }, cancellationTokenSource.Token);

        if (cancellationTokenSource.IsCancellationRequested)
            return nearestVertex;

        // Convert nearest vertex back to the world space.
        return nearestVertex;

    }

    /// <summary>
    /// Finds closest vertex to the target point using the Octree.
    /// </summary>
    /// <param name="meshIndex"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 NearestVertexWithOctree(int meshIndex, Vector3 contactPoint, MeshFilter meshFilter) {

        if (meshIndex < 0 || meshIndex >= octrees.Length || octrees[meshIndex] == null) {

            Debug.LogWarning("Invalid Octree or mesh index.");
            return Vector3.zero;

        }

        return octrees[meshIndex].FindNearestVertex(contactPoint, meshFilter);

    }

    public void Reload() {

        repairNow = false;
        repaired = true;
        deformingNow = false;
        deformed = false;

    }

    public void Save() {

        if (!CarController || !initialized)
            return;

        if (RCCPSceneManager.mutlithreadingSupported)
            RCCP_DamageData.SaveDamage(CarController, saveName);
        else
            RCCP_DamageData.SaveDamageRaw(CarController, saveName);

    }

    public void Load() {

        if (!CarController || !initialized)
            return;

        if (RCCPSceneManager.mutlithreadingSupported)
            RCCP_DamageData.LoadDamage(CarController, saveName);
        else
            RCCP_DamageData.LoadDamageRaw(CarController, saveName);

    }

    public void Delete() {

        PlayerPrefs.DeleteKey(saveName + "_DamageData");

    }

    public void OnDestroy() {

        if (cancellationTokenSource == null)
            cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

    }

    private void OnValidate() {

        if (saveName == "")
            saveName = GetComponentInParent<RCCP_CarController>(true).transform.name;

    }

}
