using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceEventEntry : SelectionItemEntry
    {
        [Header("UI")]
        [SerializeField] TMP_Text raceTypeText;
        [SerializeField] TMP_Text roundsText;
        [SerializeField] TMP_Text bestPositionText;
        [SerializeField] Image trophy;
        [SerializeField] RaceRewardsDisplay rewardsDisplayer;

        [Header("Modal")]
        [SerializeField] bool showModalWindow;
        [SerializeField]
        ModalWindowProperties modalProperties = new ModalWindowProperties
        {
            header = "Start",
            message = "Are you sure you want to start?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        RaceSession _session;
        Championship _championship;

        public void Setup(RaceSession session)
        {
            _session = session;

            base.Setup(
            item: _session,
            col: Color.white,
            onSelect: null,
            onClick: () =>
            {
                if (!_session.IsUnlocked())
                {
                    GeneralHelper.PurchaseItem(
                        item: _session,
                        OnSuccess: () =>
                        {
                            Refresh();
                        },
                        OnFail: () => { }
                        );

                    return;
                }

                if (!_session.IsUnlocked())
                    return;

                if (_session.autoPopulateEntrantOptions.autoPopulatePlayer)
                {
                    StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() => StartEvent(_session)));
                }
                else
                {
                    StartEvent(_session);
                }
            });
        }

        public void Setup(Championship championship)
        {
            _championship = championship;

            base.Setup(
            item: championship,
            col: Color.white,
            onSelect: null,
            onClick: () =>
            {
                if (!_championship.IsUnlocked())
                {
                    GeneralHelper.PurchaseItem(
                        item: _championship,
                        OnSuccess: () =>
                        {
                            Refresh();
                        },
                        OnFail: () => { }
                        );

                    return;
                }

                if (!_championship.IsUnlocked())
                    return;

                if (_championship.autoPopulateEntrantOptions.autoPopulatePlayer)
                {
                    StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() => StartEvent(_championship)));
                }
                else
                {
                    StartEvent(_championship);
                }
            });
        }

        public override void Refresh()
        {
            base.Refresh();

            if (_session == null && _championship == null)
                return;

            var raceType = _session != null ? _session.raceType : _championship.raceType;
            var rounds = _championship != null ? _championship.TotalRounds : 0;
            var record = _session != null ? _session.LoadBestPosition() : _championship.LoadBestPosition();

            raceTypeText?.SetText(raceType?.displayName ?? "");
            roundsText?.SetText(rounds.ToString());
            rewardsDisplayer?.Populate(_session != null ? _session.raceRewards : _championship.rewards);
            bestPositionText?.SetText(record > 0 ? UIHelper.FormatOrdinalText(record) : "");

            if (trophy != null)
            {
                var icon = TargetScoreIcon.GetIcon(record - 1);
                trophy.enabled = icon != null;

                if (icon != null)
                {
                    trophy.sprite = icon.icon;
                    trophy.color = icon.color;
                }
            }
        }

        void StartEvent(RaceSession session)
        {
            if (showModalWindow)
            {
                ModalWindowManager.Instance.Show(new ModalWindowProperties
                {
                    header = modalProperties.header,
                    message = modalProperties.message,
                    confirmButtonText = modalProperties.confirmButtonText,
                    declineButtonText = modalProperties.declineButtonText,
                    confirmAction = () => Load(),
                    declineAction = () => { },
                    startSelection = modalProperties.startSelection,
                    prefabIndex = modalProperties.prefabIndex
                });
            }
            else
            {
                Load();
            }

            void Load()
            {
                RGSKCore.runtimeData.SelectedSession = session;
                RGSKCore.runtimeData.SelectedTrack = session.track;

                if (session?.track?.scene != null)
                {
                    SceneLoadManager.LoadScene(session.track.scene);
                }
            }
        }

        void StartEvent(Championship c)
        {
            if (showModalWindow)
            {
                ModalWindowManager.Instance.Show(new ModalWindowProperties
                {
                    header = modalProperties.header,
                    message = modalProperties.message,
                    confirmButtonText = modalProperties.confirmButtonText,
                    declineButtonText = modalProperties.declineButtonText,
                    confirmAction = () => ChampionshipManager.Instance.StartChampionship(c),
                    declineAction = () => { },
                    startSelection = modalProperties.startSelection,
                    prefabIndex = modalProperties.prefabIndex
                });
            }
            else
            {
                ChampionshipManager.Instance.StartChampionship(c);
            }
        }
    }
}