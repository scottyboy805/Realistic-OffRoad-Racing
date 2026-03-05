using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceBoard : MonoBehaviour
    {
        [SerializeField] RaceBoardLayout layout;
        [SerializeField] BoardSize items = BoardSize.Full;
        [SerializeField] BoardSize size = BoardSize.Full;
        [SerializeField] BoardSortOrder sortOrder = BoardSortOrder.RaceStandings;
        [SerializeField] bool clickToFocus;

        [Header("Header/Title")]
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text headerTextPrefab;
        [SerializeField] LayoutGroup headerLayoutGroup;

        [Header("Entry")]
        [SerializeField] RaceBoardEntry entryPrefab;
        [SerializeField] TextMeshProUGUI entryTextPrefab;
        [SerializeField] Image entryImagePrefab;
        [SerializeField] ScrollRect entryScrollView;

        public bool IsVisible => _canvasGroup.alpha > 0;

        Dictionary<RGSKEntity, RaceBoardEntry> _entries = new Dictionary<RGSKEntity, RaceBoardEntry>();
        UIScreen _parentScreen;
        CanvasGroup _canvasGroup;

        void OnEnable()
        {
            RGSKEvents.OnEntityAdded.AddListener(OnEntityAdded);
            RGSKEvents.OnEntityRemoved.AddListener(OnEntityRemoved);
            RGSKEvents.OnRacePositionsChanged.AddListener(OnRacePositionChanged);
            RGSKEvents.OnChampionshipPositionsChanged.AddListener(OnRacePositionChanged);
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnEntityAdded.RemoveListener(OnEntityAdded);
            RGSKEvents.OnEntityRemoved.RemoveListener(OnEntityRemoved);
            RGSKEvents.OnRacePositionsChanged.RemoveListener(OnRacePositionChanged);
            RGSKEvents.OnChampionshipPositionsChanged.RemoveListener(OnRacePositionChanged);
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void Awake()
        {
            Initialize(layout);
        }

        void Start()
        {
            if (RaceManager.Instance != null)
            {
                foreach (var e in RaceManager.Instance.Entities.Items)
                {
                    if (!_entries.ContainsKey(e))
                    {
                        OnEntityAdded(e);
                    }
                }
            }
        }

        public void Initialize(RaceBoardLayout layout, BoardSortOrder sortOrder = BoardSortOrder.RaceStandings)
        {
            if (layout == null)
                return;

            this.layout = layout;
            this.sortOrder = sortOrder;
            CreateHeaders();
            titleText?.SetText(layout.title);
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _parentScreen = GetComponentInParent<UIScreen>();
        }

        void CreateHeaders()
        {
            if (headerLayoutGroup == null || headerTextPrefab == null)
                return;

            foreach (var item in items == BoardSize.Full ? layout.items : layout.miniItems)
            {
                if (item.cellType == BoardCellType.Text)
                {
                    var text = CreateTextCell(item, headerLayoutGroup.transform, true);
                    text.text = item.header;
                }
                else
                {
                    CreateImageCell(item, headerLayoutGroup.transform);
                }
            }
        }

        void OnEntityAdded(RGSKEntity e)
        {
            if (e.IsVirtual || layout == null)
                return;

            if (entryPrefab == null || entryScrollView == null)
                return;

            var entry = Instantiate(entryPrefab, entryScrollView.content);
            entry.autoBindToFocusedEntity = false;

            var competitorUI = entry.gameObject.GetOrAddComponent<CompetitorUI>();
            var driftUI = entry.gameObject.GetOrAddComponent<DriftUI>();
            var vehicleUI = entry.gameObject.GetOrAddComponent<VehicleUI>();
            var profileUI = entry.gameObject.GetOrAddComponent<ProfileUI>();
            vehicleUI.vehicleDefinitionUI = entry.gameObject.GetOrAddComponent<VehicleDefinitionUI>();
            profileUI.profileDefinitionUI = entry.gameObject.GetOrAddComponent<ProfileDefinitionUI>();

            competitorUI.positionDisplayMode = RGSKCore.Instance.UISettings.raceBoardPositionFormat;
            competitorUI.lapDisplayMode = RGSKCore.Instance.UISettings.raceBoardLapFormat;
            vehicleUI.vehicleDefinitionUI.nameDisplayMode = RGSKCore.Instance.UISettings.raceBoardVehicleNameFormat;
            profileUI.profileDefinitionUI.nameDisplayMode = RGSKCore.Instance.UISettings.raceBoardNameFormat;

            foreach (var item in items == BoardSize.Full ? layout.items : layout.miniItems)
            {
                if (item.cellType == BoardCellType.Text)
                {
                    var text = CreateTextCell(item, entry.transform, false);

                    competitorUI.Assign(item.cellValue, text);
                    driftUI.Assign(item.cellValue, text);
                    vehicleUI.Assign(item.cellValue, text);
                    profileUI.Assign(item.cellValue, text);
                }
                else
                {
                    var img = CreateImageCell(item, entry.transform);

                    vehicleUI.Assign(item.cellValue, img);
                    profileUI.Assign(item.cellValue, img);
                }
            }

            if (clickToFocus)
            {
                var btn = entry.gameObject.GetOrAddComponent<Button>();
                var img = entry.gameObject.GetComponent<Image>();

                if (img == null)
                {
                    img = entry.gameObject.AddComponent<Image>();
                    img.color = new Color(0, 0, 0, 0);
                }

                var nav = btn.navigation;
                nav.mode = Navigation.Mode.None;

                btn.navigation = nav;
                btn.transition = Selectable.Transition.None;
                btn.onClick.AddListener(() => CameraManager.Instance?.SetTarget(RaceManager.Instance.GetCompetitorInPosition(entry.transform.GetSiblingIndex() + 1).transform));
            }

            entry.BindElements(e);
            _entries.Add(e, entry);
            Refresh();
        }

        void OnEntityRemoved(RGSKEntity e)
        {
            if (_entries.TryGetValue(e, out var entry))
            {
                Destroy(entry.gameObject);
                _entries.Remove(e);
            }
        }

        void OnRacePositionChanged() => Refresh();
        void OnCompetitorFinished(Competitor c) => Refresh();
        void OnCameraTargetChanged(Transform target) => Refresh();

        [ContextMenu("Refresh Board")]
        public void Refresh()
        {
            if (_parentScreen != null && !_parentScreen.IsOpen())
                return;

            foreach (var e in _entries.Values)
            {
                if (e.entity.Competitor != null)
                {
                    var index = e.entity.Competitor.Position - 1;

                    if (sortOrder == BoardSortOrder.ChampionshipStandings && ChampionshipManager.Instance.Initialized)
                    {
                        index = ChampionshipManager.Instance.GetEntrantPosition(e.entity) - 1;
                    }

                    e.transform.SetSiblingIndex(index);
                }

                e.UpdateEntry();
            }

            Resize();
        }

        void Resize()
        {
            if (size == BoardSize.Full)
            {
                foreach (var entry in _entries.Values)
                {
                    entry.ToggleVisible(true);
                }

                return;
            }

            var entity = GeneralHelper.GetFocusedEntity();
            if (entity != null && entity.Competitor != null)
            {
                var pos = entity.Competitor.Position - 1;

                foreach (var entry in _entries.Values)
                {
                    var index = entry.transform.GetSiblingIndex();

                    if (index == 0)
                    {
                        entry.ToggleVisible(true);
                        continue;
                    }

                    entry.ToggleVisible(false);

                    if (pos == 0 || pos == 1)
                    {
                        entry.ToggleVisible(index < 4);
                    }
                    else if (pos == _entries.Count - 1)
                    {
                        entry.ToggleVisible(index > _entries.Count - 4);
                    }
                    else
                    {
                        entry.ToggleVisible(index == pos ||
                                            index == pos - 1 ||
                                            index == pos + 1);
                    }
                }
            }
        }

        public void ToggleVisible(bool visible) => _canvasGroup?.SetAlpha(visible ? 1 : 0);

        void UpdateLayoutElement(LayoutElement element, BoardItem item)
        {
            if (element == null)
                return;

            element.preferredWidth = item.preferredWidth;
            element.preferredHeight = item.preferredHeight;
            element.flexibleWidth = item.flexibleWidth;
            element.flexibleHeight = item.flexibleHeight;
        }

        TMP_Text CreateTextCell(BoardItem item, Transform parent, bool isHeader)
        {
            var text = Instantiate(isHeader ? headerTextPrefab : entryTextPrefab, parent);
            text.alignment = item.alignment;
            text.font = item.font != null ? item.font : text.font;
            text.fontSize = item.fontSize > 0 ? item.fontSize : text.fontSize;

            UpdateLayoutElement(text.GetComponent<LayoutElement>(), item);

            return text;
        }

        Image CreateImageCell(BoardItem item, Transform parent)
        {
            var img = Instantiate(entryImagePrefab, parent);
            img.enabled = false;
            img.preserveAspect = true;

            UpdateLayoutElement(img.GetComponent<LayoutElement>(), item);

            return img;
        }
    }
}