using System;
using UnityEngine;
using RGSK.Extensions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK.Helpers
{
    public static class GeneralHelper
    {
        static Dictionary<float, WaitForSeconds> _cachedWFS = new Dictionary<float, WaitForSeconds>();
        static Dictionary<float, WaitForSecondsRealtime> _cachedWFSRT = new Dictionary<float, WaitForSecondsRealtime>();
        static Transform _dynamicParent;

        #region Entity
        public static RGSKEntity GetFocusedEntity()
        {
            return CameraManager.Instance?.FocusedEntity ?? null;
        }

        public static RGSKEntity GetEntity(int index)
        {
            return RGSKCore.Instance.GeneralSettings.entitySet.GetItem(index) ?? null;
        }

        public static int GetEntityIndex(GameObject go)
        {
            return RGSKCore.Instance.GeneralSettings.entitySet.GetIndexOf(go);
        }
        #endregion

        #region Race
        public static int GetLapsAhead(Competitor a, Competitor b)
        {
            var lapDist = a.TotalRaceDistance / a.TotalLaps;
            var gap = GetDistanceGapBetween(a, b);

            return (int)Mathf.Abs(gap / lapDist);
        }

        public static float GetDistanceGapBetween(Competitor a, Competitor b)
        {
            if (a == null || b == null)
                return 0;

            return a.DistanceTravelled - b.DistanceTravelled;
        }

        public static float GetTimeGapBetween(Competitor a, Competitor b)
        {
            if (a == null || b == null)
                return 0;

            if (b.Entity.CurrentSpeed < 1)
            {
                return 0;
            }

            var dist = GetDistanceGapBetween(a, b);
            return dist / Math.Abs(b.Entity.CurrentSpeed);
        }

        public static void PopulatePlayerEntrant(ref List<RaceEntrant> entrants)
        {
            if (RGSKCore.runtimeData.SelectedVehicle == null)
            {
                Logger.LogWarning("You are trying to auto populate the player entrant but no vehicle has been selected! Please ensure that a vehicle has been selected.");
                return;
            }

            var player = entrants.FirstOrDefault(x => x.isPlayer);
            if (player != null)
            {
                Logger.LogWarning("A player entrant has been assigned and 'Auto Populate Player' is enabled! The player entrant will be replaced.");
                entrants.Remove(player);
            }

            entrants.Add(new RaceEntrant
            {
                prefab = RGSKCore.runtimeData.SelectedVehicle.prefab,
                colorSelectMode = RGSKCore.runtimeData.SelectedVehicleLivery == null ? ColorSelectionMode.Color : ColorSelectionMode.Livery,
                color = RGSKCore.runtimeData.SelectedVehicleColor,
                livery = RGSKCore.runtimeData.SelectedVehicleLivery,
                isPlayer = true
            });
        }

        public static List<RaceEntrant> PopulateOpponentEntrants(AutoPopulateEntrantOptions options, VehicleClass playerVehicleClass)
        {
            var list = new List<RaceEntrant>();

            if (!options.autoPopulateOpponents)
            {
                return list;
            }

            var vehicles = RGSKCore.Instance.ContentSettings.vehicles.ToList();
            var index = 0;

            if (vehicles.Count == 0)
            {
                Logger.LogWarning("Cannot auto populate opponents because no vehicles were found!");
                return list;
            }

            switch (options.opponentClassOptions)
            {
                case OpponentClassOptions.SameAsPlayer:
                    {
                        if (playerVehicleClass != null)
                        {
                            vehicles = RGSKCore.Instance.ContentSettings.vehicles.Where(x => x.vehicleClass == playerVehicleClass).ToList();
                        }
                        break;
                    }

                case OpponentClassOptions.Selected:
                    {
                        if (options.opponentVehicleClass != null)
                        {
                            vehicles = RGSKCore.Instance.ContentSettings.vehicles.Where(x => x.vehicleClass == options.opponentVehicleClass).ToList();
                        }
                        break;
                    }
            }

            if (vehicles.Count == 0)
            {
                Logger.LogWarning("No vehicles were found in the specified class! Oponents will be auto populated from all vehicle classes.");
                vehicles = RGSKCore.Instance.ContentSettings.vehicles.ToList();
            }

            vehicles.Shuffle();

            for (int i = 0; i < options.opponentCount; i++)
            {
                if (vehicles[index].prefab != null)
                {
                    list.Add(new RaceEntrant
                    {
                        prefab = vehicles[index].prefab,
                        colorSelectMode = ColorSelectionMode.Random,
                        isPlayer = false,
                    });
                }

                index = (index + 1) % vehicles.Count;
            }

            return list;
        }

        public static VehicleClass GetPlayerEntrantVehicleClass(List<RaceEntrant> entrants)
        {
            var player = entrants.FirstOrDefault(x => x.isPlayer);

            if (player != null && player.prefab != null)
            {
                if (player.prefab.TryGetComponent<VehicleDefiner>(out var v))
                {
                    return v?.definition?.vehicleClass;
                }
            }

            return null;
        }

        public static ProfileDefinition GetEntrantProfile(RaceEntrant e, ref List<ProfileDefinition> profiles, ref int counter)
        {
            return e.profile == null ?
                   e.isPlayer ? RGSKCore.Instance.GeneralSettings.playerProfile :
                   GetRandomOpponentProfile(ref profiles, ref counter) :
                   e.profile;

            ProfileDefinition GetRandomOpponentProfile(ref List<ProfileDefinition> profiles, ref int counter)
            {
                if (profiles == null || profiles.Count == 0)
                {
                    profiles = RGSKCore.Instance.AISettings.opponentProfiles.ToList();
                }

                if (profiles.Count > 0)
                {
                    var p = profiles.GetRandom();
                    profiles.Remove(p);
                    return p;
                }
                else
                {
                    var p = GeneralHelper.CreateProfileDefinition($"AI {counter}");

                    if (RGSKCore.Instance.GeneralSettings.countrySettings != null)
                    {
                        p.nationality = RGSKCore.Instance.GeneralSettings.countrySettings.countries.GetRandom();
                    }

                    counter++;
                    return p;
                }
            }
        }
        #endregion

        #region Misc
        public static void SetTransmission(GameObject go, TransmissionType type)
        {
            if (go.TryGetComponent<IVehicle>(out var v))
            {
                v.TransmissionType = type;
            }
        }

        public static void SetHandlingMode(GameObject go, VehicleHandlingMode mode)
        {
            if (go.TryGetComponent<IVehicle>(out var v))
            {
                v.HandlingMode = mode;
            }
        }

        public static void ToggleGhostedMesh(GameObject target, bool on)
        {
            var materialController = target.GetOrAddComponent<GhostMaterialController>();
            materialController.ToggleGhostMaterials(on);
        }

        public static void SetColor(GameObject target, Color col, int colIndex = 0)
        {
            var colors = target.GetComponentsInChildren<MeshColorizer>();
            if (colors.Length > 0)
            {
                foreach (var c in colors)
                {
                    c.SetColor(col, colIndex);
                }
            }
        }

        public static void SetColor(GameObject target, int col, int colIndex = 0)
        {
            var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;
            
            if (colors != null && colors.colors.Count > 0)
            {
                col = Mathf.Clamp(col, 0, colors.colors.Count - 1);
                SetColor(target, colors.colors[col]);
            }
        }

        public static Color GetRandomVehicleColor() => RGSKCore.Instance.VehicleSettings.vehicleColorList?.GetRandom() ?? Color.white;

        public static Color GetColor(GameObject target, int colIndex = 0)
        {
            var color = target.GetComponentsInChildren<MeshColorizer>().FirstOrDefault(x => x.colorIndex == colIndex);

            if (color != null)
            {
                return color.GetColor();
            }

            return Color.black;
        }

        public static int GetVehicleColorIndex(GameObject target, int colIndex = 0)
        {
            if(CanApplyColor(target))
            {
                return GetVehicleColorIndex(GetColor(target, colIndex));
            }
            else if(CanApplyLivery(target))
            {
                return GetLiveryIndex(target);
            }

            return 0;
        }

        public static int GetVehicleColorIndex(Color col)
        {
            if (RGSKCore.Instance.VehicleSettings.vehicleColorList != null)
            {
                for (int i = 0; i < RGSKCore.Instance.VehicleSettings.vehicleColorList.colors.Count; i++)
                {
                    var color = RGSKCore.Instance.VehicleSettings.vehicleColorList.colors[i];

                    if (ColorUtility.ToHtmlStringRGBA(col) == ColorUtility.ToHtmlStringRGBA(color))
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        public static void SetLivery(GameObject target, Texture2D tex)
        {
            var liveries = target.GetComponentsInChildren<MeshLivery>();

            if (liveries.Length > 0)
            {
                foreach (var l in liveries)
                {
                    l.SetLivery(tex);
                }
            }
        }

        public static void SetLivery(GameObject target, int texIndex)
        {
            var liveries = GetVehicleLiveries(target);

            if (liveries != null && liveries.liveries.Count > 0)
            {
                texIndex = Mathf.Clamp(texIndex, 0, liveries.liveries.Count - 1);
                SetLivery(target, liveries.liveries[texIndex].texture);
            }
        }

        public static void SetRandomLivery(GameObject target)
        {
            var liveries = GetVehicleLiveries(target);

            if (liveries != null)
            {
                SetLivery(target, liveries.GetRandom().texture);
            }
        }

        public static Texture2D GetRandomLivery(GameObject target)
        {
            var liveries = GetVehicleLiveries(target);

            if (liveries != null)
            {
                return liveries.GetRandom().texture;
            }

            return null;
        }

        public static int GetLiveryIndex(GameObject target)
        {
            var livery = target.GetComponentInChildren<MeshLivery>();

            if (livery != null)
            {
                return livery.CurrentLiveryIndex;
            }

            return 0;
        }

        public static LiveryList GetVehicleLiveries(GameObject target)
        {
            var livery = target.GetComponentInChildren<MeshLivery>();

            if (livery != null)
            {
                return livery.Liveries;
            }

            return null;
        }

        public static bool CanApplyColor(GameObject target)
        {
            if (target == null)
                return false;

            return target.GetComponentInChildren<MeshColorizer>() != null;
        }

        public static bool CanApplyLivery(GameObject target)
        {
            if (target == null)
                return false;

            return target.GetComponentInChildren<MeshLivery>() != null;
        }

        public static int GetLevelFromXP(int xp)
        {
            var level = 1;

            for (int i = 1; i < RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.GetMaxTime() + 1; i++)
            {
                if (xp >= (int)RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.Evaluate(i))
                {
                    level = i;
                }
            }

            return level;
        }

        public static void PurchaseItem(ItemDefinition item, Action OnSuccess = null, Action OnFail = null)
        {
            switch (item.unlockMode)
            {
                case ItemUnlockMode.None:
                    {
                        if (!string.IsNullOrWhiteSpace(item.unlockCondition))
                        {
                            ModalWindowManager.Instance.Show(new ModalWindowProperties
                            {
                                header = RGSKCore.Instance.UISettings.lockedItemConditionalModal.header,
                                message = string.Format(RGSKCore.Instance.UISettings.lockedItemConditionalModal.message, item.unlockCondition),
                                confirmButtonText = RGSKCore.Instance.UISettings.lockedItemConditionalModal.confirmButtonText,
                                confirmAction = () => { },
                                declineAction = () => { },
                                startSelection = RGSKCore.Instance.UISettings.lockedItemConditionalModal.startSelection,
                                prefabIndex = RGSKCore.Instance.UISettings.lockedItemConditionalModal.prefabIndex
                            });
                        }
                        break;
                    }

                case ItemUnlockMode.Purchase:
                    {
                        var currency = SaveData.Instance.playerData.currency;

                        if (currency >= item.unlockPrice)
                        {
                            ModalWindowManager.Instance.Show(new ModalWindowProperties
                            {
                                header = RGSKCore.Instance.UISettings.purchasePromptModal.header,
                                message = string.Format(RGSKCore.Instance.UISettings.purchasePromptModal.message, item.objectName, UIHelper.FormatCurrencyText(item.unlockPrice)),
                                confirmButtonText = RGSKCore.Instance.UISettings.purchasePromptModal.confirmButtonText,
                                declineButtonText = RGSKCore.Instance.UISettings.purchasePromptModal.declineButtonText,
                                confirmAction = () =>
                                {
                                    currency -= item.unlockPrice;
                                    SaveData.Instance.playerData.currency = currency;
                                    item.Unlock();
                                    OnSuccess?.Invoke();
                                    SaveManager.Instance?.Save();
                                },
                                declineAction = () => { },
                                startSelection = RGSKCore.Instance.UISettings.purchasePromptModal.startSelection,
                                prefabIndex = RGSKCore.Instance.UISettings.purchasePromptModal.prefabIndex
                            });
                        }
                        else
                        {
                            ModalWindowManager.Instance.Show(new ModalWindowProperties
                            {
                                header = RGSKCore.Instance.UISettings.purchaseFailModal.header,
                                message = RGSKCore.Instance.UISettings.purchaseFailModal.message,
                                confirmButtonText = RGSKCore.Instance.UISettings.purchaseFailModal.confirmButtonText,
                                confirmAction = () => { OnFail?.Invoke(); },
                                declineAction = () => { },
                                startSelection = RGSKCore.Instance.UISettings.purchaseFailModal.startSelection,
                                prefabIndex = RGSKCore.Instance.UISettings.purchaseFailModal.prefabIndex
                            });
                        }

                        break;
                    }

                case ItemUnlockMode.XPLevel:
                    {
                        ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = RGSKCore.Instance.UISettings.lockedItemModal.header,
                            message = string.Format(RGSKCore.Instance.UISettings.lockedItemModal.message, item.unlockXPLevel),
                            confirmButtonText = RGSKCore.Instance.UISettings.lockedItemModal.confirmButtonText,
                            confirmAction = () => { },
                            declineAction = () => { },
                            startSelection = RGSKCore.Instance.UISettings.lockedItemModal.startSelection,
                            prefabIndex = RGSKCore.Instance.UISettings.lockedItemModal.prefabIndex
                        });

                        break;
                    }
            }
        }

        public static IEnumerator OpenVehicleSelectScreenWithCallback(UnityEngine.Events.UnityAction onSelectedCallback)
        {
            var screen = RGSKCore.Instance.UISettings?.screens?.VehicleSelectScreen;

            if (screen == null)
            {
                Logger.LogWarning("Unable to open the vehicle selection screen! Please assign it to the RGSK Menu under the 'UI' tab.");
                yield break;
            }

            screen.Open();

            yield return null;

            var activeScreen = UIManager.Instance.ActiveScreen;

            if (activeScreen != null && activeScreen.TryGetComponent<VehicleSelectScreen>(out var s))
            {
                s.OnSelected.RemoveAllListeners();
                s.OnSelected.AddListener(onSelectedCallback);
            }
        }

        public static IEnumerator OpenTrackSelectScreenWithCallback(UnityEngine.Events.UnityAction onSelectedCallback)
        {
            var screen = RGSKCore.Instance.UISettings?.screens?.TrackSelectScreen;

            if (screen == null)
            {
                Logger.LogWarning("Unable to open the track selection screen! Please assign it to the RGSK Menu under the 'UI' tab.");
                yield break;
            }

            screen.Open();

            yield return null;

            var activeScreen = UIManager.Instance.ActiveScreen;

            if (activeScreen != null && activeScreen.TryGetComponent<TrackSelectScreen>(out var s))
            {
                s.OnSelected.RemoveAllListeners();
                s.OnSelected.AddListener(onSelectedCallback);
            }
        }
        #endregion

        #region Input
        public static void TogglePlayerInput(GameObject go, bool enable)
        {
            if (enable)
            {
                PlayerVehicleInput.Instance?.Bind(go);
            }
            else
            {
                PlayerVehicleInput.Instance?.Unbind(go);
            }
        }

        public static void ToggleAIInput(GameObject go, bool enable)
        {
            var ai = go.GetOrAddComponent<AIController>();
            ai.ToggleActive(enable);

            if (enable)
            {
                SetTransmission(go, TransmissionType.Automatic);
            }
        }

        public static void ToggleInputControl(GameObject go, bool enable)
        {
            var input = go.GetComponent<IInput>();

            if (enable)
            {
                input?.EnableControl();
            }
            else
            {
                input?.DisableControl();
            }
        }
        #endregion

        #region Physics
        public static void ToggleVehicleCollision(GameObject go, bool on)
        {
            go.SetColliderLayer(on ?
                            RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index :
                            RGSKCore.Instance.GeneralSettings.ghostLayerIndex.Index);

#if UNITY_6000_0_OR_NEWER
            var rbs = go.GetComponentsInChildren<Rigidbody>(true).ToList();
            rbs.ForEach(x => x.excludeLayers |= 1 << LayerMask.NameToLayer(LayerMask.LayerToName(RGSKCore.Instance.GeneralSettings.ghostLayerIndex.Index)));
#endif
        }

        public static void TogglePhysics(GameObject go, bool enable)
        {
            if (go.TryGetComponent<Rigidbody>(out var rigid))
            {
                rigid.isKinematic = !enable;
            }
        }

        public static void SetRigidbodySpeed(GameObject go, float speed)
        {
            if (go.TryGetComponent<Rigidbody>(out var rigid))
            {
                rigid.SetSpeed(speed, SpeedUnit.KMH);
            }
        }
        #endregion

        #region AI
        public static void SetAIBehaviour(GameObject go, AIBehaviourSettings behaviour)
        {
            if (behaviour == null)
                return;

            var ai = go.GetComponent<AIController>();

            if (ai != null)
            {
                ai.SetBehaviour(behaviour);
            }
        }
        #endregion

        #region Util
        public static Timer CreateTimer(float startValue, bool countdown, bool autoReset,
                        System.Action onElapsed = null,
                        System.Action onStart = null,
                        System.Action onStop = null,
                        System.Action onRestart = null,
                        string name = "timer",
                        bool fixedUpdate = false)
        {
            var timer = new GameObject(name).AddComponent<Timer>();

            timer.Initialize(startValue, countdown, autoReset, fixedUpdate);
            timer.OnTimerElapsed += onElapsed;
            timer.OnTimeStart += onStart;
            timer.OnTimerStop += onStop;
            timer.OnTimerRestart += onRestart;

            timer.transform.SetParent(GetDynamicParent());
            return timer;
        }

        public static Transform GetDynamicParent()
        {
            if (_dynamicParent == null)
            {
                _dynamicParent = new GameObject("[RGSK] Runtime Objects").transform;
                _dynamicParent.transform.SetAsLastSibling();
            }

            return _dynamicParent;
        }

        public static WaitForSeconds GetCachedWaitForSeconds(float value)
        {
            if (_cachedWFS.TryGetValue(value, out var wait))
            {
                return wait;
            }
            else
            {
                _cachedWFS[value] = new WaitForSeconds(value);
                return _cachedWFS[value];
            }
        }

        public static WaitForSecondsRealtime GetCachedWaitForSecondsRealtime(float value)
        {
            if (_cachedWFSRT.TryGetValue(value, out var wait))
            {
                return wait;
            }
            else
            {
                _cachedWFSRT[value] = new WaitForSecondsRealtime(value);
                return _cachedWFSRT[value];
            }
        }

        public static List<Resolution> GetResolutionsList(int minHeight = 720)
        {
            var resolutions = Screen.resolutions.Select(resolution => new Resolution
            { width = resolution.width, height = resolution.height }).Distinct();

            return resolutions.Where(x => x.height >= minHeight).ToList();
        }

        public static int GetCurrentResolutionIndex()
        {
            var resolutions = GetResolutionsList();
            var currentResolution = Screen.currentResolution;
            return resolutions.FindIndex(x => x.width == currentResolution.width && x.height == currentResolution.height);
        }

        public static int ValidateIndex(int index, int minLength, int maxLength, bool loop)
        {
            if (loop)
            {
                return (int)CustomRepeat(index, minLength, maxLength);
            }

            float CustomRepeat(float value, float minValue, float maxValue)
            {
                var range = maxValue - minValue;
                return minValue + ((value - minValue) % range + range) % range;
            }

            return Mathf.Clamp(index, minLength, maxLength);
        }

        public static ProfileDefinition CreateProfileDefinition(string name)
        {
            var names = name.Split(' ');
            var profile = ScriptableObject.CreateInstance<ProfileDefinition>();

            if (names.Length > 0)
            {
                profile.firstName = names[0];
                profile.lastName = names.Length > 1 ? names[1] : string.Empty;
            }

            return profile;
        }

        public static bool IsMobilePlatform()
        {
            if (Application.isEditor)
            {
                return false;
            }

            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
        }
        #endregion

        #region IO
        public static string GetSaveDataDirectory()
        {
            var path = Path.Combine(Application.persistentDataPath, "SaveData");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetProfilesDirectory()
        {
            var path = Path.Combine(GetSaveDataDirectory(), "Profiles");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetReplayDirectory()
        {
            var path = Path.Combine(GetSaveDataDirectory(), "Replays");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
        #endregion
    }
}