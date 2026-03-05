using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    public class RaceSettingsEntryUI : GameSettingsEntryUI
    {
        [SerializeField] RaceSettingUIType type;
        [SerializeField] int maxLaps = 100;
        [SerializeField] int timeIncrements = 30;
        [SerializeField] int maxGhostOffset = 1;

        RaceSettingsScreen _screen;
        RaceSession _session;
        List<RaceType> _racetypes;
        float _timeLimit;
        float _ghostOffset;
        int _selectedRaceType;

        public void Setup(RaceSettingsScreen screen)
        {
            _screen = screen;

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(type.ToString().InsertSpacesBeforeCapitals());
            }
        }

        public void UpdateSession(RaceSession session)
        {
            _session = session;
            
            switch (type)
            {
                case RaceSettingUIType.RaceType:
                    {
                        if (RGSKCore.runtimeData.SelectedTrack != null)
                        {
                            if (RGSKCore.runtimeData.SelectedTrack.allowedRaceTypes.Count > 0)
                            {
                                _racetypes = RGSKCore.runtimeData.SelectedTrack.allowedRaceTypes;
                            }
                            else
                            {
                                _racetypes = RGSKCore.Instance.RaceSettings.raceTypes.Where(x => x.selectableFromMenu)
                                .Where(x => EnumFlags.GetSelectedIndexes(x.allowedTrackLayouts).
                                Contains((int)RGSKCore.runtimeData.SelectedTrack.layoutType)).ToList();
                            }
                        }
                        break;
                    }
            }
        }

        public override void SelectOption(int direction)
        {
            if (_session == null || RGSKCore.runtimeData.SelectedTrack == null)
                return;

            switch (type)
            {
                case RaceSettingUIType.RaceType:
                    {
                        SelectRaceType(direction);
                        break;
                    }

                case RaceSettingUIType.LapCount:
                    {
                        SelectLaps(direction);
                        break;
                    }

                case RaceSettingUIType.OpponentCount:
                    {
                        SelectOpponents(direction);
                        break;
                    }

                case RaceSettingUIType.SessionTimeLimit:
                    {
                        SelectTimeLimit(direction);
                        break;
                    }

                case RaceSettingUIType.StartingPosition:
                    {
                        SelectPlayerStartingPosition(direction);
                        break;
                    }

                case RaceSettingUIType.Collision:
                    {
                        SelectCollision(direction);
                        break;
                    }

                case RaceSettingUIType.StartMode:
                    {
                        SelectStartMode(direction);
                        break;
                    }

                case RaceSettingUIType.AIDifficulty:
                    {
                        SelectAIDifficulty(direction);
                        break;
                    }

                case RaceSettingUIType.Slipstream:
                    {
                        SelectSlipstream(direction);
                        break;
                    }

                case RaceSettingUIType.Catchup:
                    {
                        SelectCatchup(direction);
                        break;
                    }

                case RaceSettingUIType.Ghost:
                    {
                        SelectGhost(direction);
                        break;
                    }

                case RaceSettingUIType.GhostOffset:
                    {
                        SelectGhostOffset(direction);
                        break;
                    }

                case RaceSettingUIType.SpectatorMode:
                    {
                        SelectSpectatorMode(direction);
                        break;
                    }
            }

            if (direction != 0)
            {
                _screen?.RefreshEntries();
            }
        }

        public void ToggleActive()
        {
            if (_session == null || RGSKCore.runtimeData.SelectedTrack == null)
                return;

            switch (type)
            {
                case RaceSettingUIType.StartMode:
                    {
                        gameObject.SetActive(RGSKCore.runtimeData.SelectedTrack.allowRollingStarts);
                        break;
                    }

                case RaceSettingUIType.LapCount:
                    {
                        gameObject.SetActive(RGSKCore.runtimeData.SelectedTrack.layoutType == TrackLayoutType.Circuit && !_session.IsInfiniteLaps());
                        break;
                    }

                case RaceSettingUIType.SessionTimeLimit:
                    {
                        gameObject.SetActive(_session.IsTimedSession());
                        break;
                    }

                case RaceSettingUIType.OpponentCount:
                    {
                        gameObject.SetActive(_session.raceType.minMaxCompetitorCount.y > 1 && RGSKCore.runtimeData.SelectedTrack.gridSlots > 1);
                        break;
                    }

                case RaceSettingUIType.AIDifficulty:
                    {
                        gameObject.SetActive(_session.raceType.minMaxCompetitorCount.y > 1 && RGSKCore.runtimeData.SelectedTrack.gridSlots > 1 && _session.autoPopulateEntrantOptions.opponentCount > 0);
                        break;
                    }

                case RaceSettingUIType.StartingPosition:
                    {
                        gameObject.SetActive(!_session.IsSoloSession() && _session.playerGridStartMode == SelectionMode.Selected && _session.raceType.minMaxCompetitorCount.y > 1 && RGSKCore.runtimeData.SelectedTrack.gridSlots > 1 && _session.autoPopulateEntrantOptions.opponentCount > 0);
                        break;
                    }

                case RaceSettingUIType.Collision:
                    {
                        gameObject.SetActive(!_session.IsSoloSession() && _session.autoPopulateEntrantOptions.opponentCount > 0);
                        break;
                    }

                case RaceSettingUIType.Slipstream:
                    {
                        gameObject.SetActive(!_session.IsSoloSession() && !_session.disableCollision && _session.autoPopulateEntrantOptions.opponentCount > 0);
                        break;
                    }

                case RaceSettingUIType.Catchup:
                    {
                        gameObject.SetActive(!_session.IsSoloSession() && _session.autoPopulateEntrantOptions.opponentCount > 0);
                        break;
                    }

                case RaceSettingUIType.Ghost:
                    {
                        gameObject.SetActive(_session.UseGhostVehicle());
                        break;
                    }

                case RaceSettingUIType.GhostOffset:
                    {
                        gameObject.SetActive(_session.UseGhostVehicle() && _session.enableGhost);
                        break;
                    }
            }
        }

        void SelectRaceType(int direction)
        {
            _selectedRaceType = GeneralHelper.ValidateIndex(_selectedRaceType + direction, 0, _racetypes.Count, loop);
            _session.raceType = _racetypes[_selectedRaceType];
            valueText?.SetText(_session.raceType.displayName);

            if (direction != 0)
            {
                _screen?.RefreshEntries();
            }
        }

        void SelectStartMode(int direction)
        {
            var count = Enum.GetValues(typeof(RaceStartMode)).Length;
            var index = (int)_session.startMode;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);
            _session.startMode = (RaceStartMode)index;
            valueText?.SetText(_session.startMode.ToString().InsertSpacesBeforeCapitals());
        }

        void SelectLaps(int direction)
        {
            var index = _session.lapCount;
            index = GeneralHelper.ValidateIndex(index + direction, _session.raceType.minLaps, maxLaps + (loop ? 1 : 0), loop);
            _session.lapCount = index;
            valueText?.SetText(_session.lapCount.ToString());
        }

        void SelectOpponents(int direction)
        {
            var index = _session.autoPopulateEntrantOptions.opponentCount;

            index = GeneralHelper.ValidateIndex(index + direction,
                _session.raceType.minMaxCompetitorCount.x - 1,
                Mathf.Min(_session.raceType.minMaxCompetitorCount.y - (!loop ? 1 : 0), RGSKCore.runtimeData.SelectedTrack.gridSlots - (!loop ? 1 : 0)),
                loop);

            _session.autoPopulateEntrantOptions.opponentCount = index;

            valueText?.SetText(_session.autoPopulateEntrantOptions.opponentCount.ToString());
        }

        void SelectTimeLimit(int direction)
        {
            _timeLimit += direction * timeIncrements;
            _timeLimit = Mathf.Clamp(_timeLimit, timeIncrements, _timeLimit);
            _session.sessionTimeLimit = _timeLimit;
            valueText?.SetText(UIHelper.FormatTimeText(_session.sessionTimeLimit, TimeFormat.MM_SS));
        }

        void SelectPlayerStartingPosition(int direction)
        {
            var index = _session.playerStartPosition;
            index = GeneralHelper.ValidateIndex(index + direction, 1, _session.autoPopulateEntrantOptions.opponentCount + 1, false);
            _session.playerStartPosition = index;
            valueText?.SetText(UIHelper.FormatOrdinalText(index));
        }

        void SelectAIDifficulty(int direction)
        {
            var index = _session.opponentDifficulty.index;
            index = GeneralHelper.ValidateIndex(index + direction, 0, RGSKCore.Instance.AISettings.difficulties.Count, loop);
            _session.opponentDifficulty.index = index;
            valueText?.SetText(RGSKCore.Instance.AISettings.difficulties[index].displayName);
        }

        void SelectCollision(int direction)
        {
            var index = _session.disableCollision ? 1 : 0;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _session.disableCollision = index == 1;
            valueText?.SetText(toggleOptions[index]);
        }

        void SelectSlipstream(int direction)
        {
            var index = _session.enableSlipstream ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _session.enableSlipstream = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }
    
        void SelectCatchup(int direction)
        {
            var index = _session.enableCatchup ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _session.enableCatchup = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SelectGhost(int direction)
        {
            var index = _session.enableGhost ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _session.enableGhost = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SelectGhostOffset(int direction)
        {
            _ghostOffset += direction * 0.1f;
            _ghostOffset = Mathf.Clamp(_ghostOffset, 0, maxGhostOffset);
            _session.ghostOffset = _ghostOffset;
            valueText?.SetText(_ghostOffset.ToString("F1"));
        }

        void SelectSpectatorMode(int direction)
        {
            var index = _session.spectatorMode ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _session.spectatorMode = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }
    }
}