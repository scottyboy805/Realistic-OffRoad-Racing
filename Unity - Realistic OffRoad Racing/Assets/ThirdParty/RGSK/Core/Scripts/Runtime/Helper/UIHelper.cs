using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;

namespace RGSK.Helpers
{
    public static class UIHelper
    {
        #region Formatting
        public static string FormatTimeText(float time, TimeFormat format = TimeFormat.MM_SS_FFF, bool addSymbol = false)
        {
            if (time == 0 || float.IsNaN(time) || float.IsPositiveInfinity(time))
                return NullTimeString(format);

            var symbol = addSymbol ? time >= 0 ? "+" : "-" : "";
            var timespan = TimeSpan.FromSeconds(Mathf.Abs(time));

            if (timespan.Hours > 0 || timespan.Days > 0)
            {
                return $"{symbol}{FormatTimespan(timespan, TimeFormat.HH_MM_SS)}";
            }

            string NullTimeString(TimeFormat format)
            {
                switch (format)
                {
                    case TimeFormat.MM_SS_FFF:
                    default:
                        {
                            return "--:--.---";
                        }

                    case TimeFormat.MM_SS:
                        {
                            return "--:--";
                        }

                    case TimeFormat.S_FFF:
                        {
                            return "-.---";
                        }
                }
            }

            return $"{symbol}{FormatTimespan(timespan, format)}";
        }

        static string FormatTimespan(TimeSpan span, TimeFormat format)
        {
            switch (format)
            {
                case TimeFormat.MM_SS_FFF:
                default:
                    {
                        return string.Format("{0:00}:{1:00}.{2:000}", span.Minutes, span.Seconds, span.Milliseconds);
                    }

                case TimeFormat.MM_SS:
                    {
                        return string.Format("{0:00}:{1:00}", span.Minutes, span.Seconds);
                    }

                case TimeFormat.SS:
                    {
                        return string.Format("{0:00}", span.TotalSeconds);
                    }

                case TimeFormat.S_FFF:
                    {
                        return string.Format("{0:0}.{1:000}", span.TotalSeconds, span.Milliseconds);
                    }

                case TimeFormat.HH_MM_SS:
                    {
                        return string.Format("{0:00}:{1:00}:{2:00}", span.TotalHours, span.Minutes, span.Seconds);
                    }
            }
        }

        public static string FormatPositionText(int position, NumberDisplayMode mode = NumberDisplayMode.Default, float ordinalSize = -1)
        {
            string result = "";

            switch (mode)
            {
                case NumberDisplayMode.Default:
                    {
                        var total = RGSKCore.Instance.GeneralSettings.entitySet.Items.Count;
                        result = $"{position}/{(total == 0 ? "-" : total.ToString())}";
                        break;
                    }

                case NumberDisplayMode.Single:
                    {
                        result = position.ToString();
                        break;

                    }

                case NumberDisplayMode.Ordinal:
                    {
                        result = FormatOrdinalText(position, ordinalSize);
                        break;
                    }
            }

            return result;
        }

        public static string FormatLapText(int current, NumberDisplayMode mode = NumberDisplayMode.Default, float ordinalSize = -1)
        {
            var result = "";
            var infiniteLaps = false;
            var total = 0;

            if (RaceManager.Instance?.Session != null)
            {
                total = RaceManager.Instance.Session.lapCount;
                infiniteLaps = RaceManager.Instance.InfiniteLaps;
            }

            switch (mode)
            {
                case NumberDisplayMode.Default:
                    {
                        result = $"{current}/{(infiniteLaps ? "-" : total.ToString())}";
                        break;
                    }

                case NumberDisplayMode.Single:
                    {
                        result = current.ToString();
                        break;
                    }

                case NumberDisplayMode.Ordinal:
                    {
                        result = FormatOrdinalText(current, ordinalSize);
                        break;
                    }
            }

            return result;
        }

        public static string FormatCheckpointText(int current)
        {
            var result = "";
            var total = 0;
            var infiniteLaps = false;

            if (RaceManager.Instance?.Track != null)
            {
                total = RaceManager.Instance.Track.CheckpointRoute.TotalCheckpointCount * RaceManager.Instance.Session.lapCount;
                infiniteLaps = RaceManager.Instance.InfiniteLaps;
            }

            result = $"{current}/{(infiniteLaps ? "-" : total.ToString())}";
            return result;
        }

        public static string FormatNameText(int index, NameDisplayMode mode = NameDisplayMode.FullName)
        {
            var profile = GeneralHelper.GetEntity(index);

            if (profile != null)
            {
                return FormatNameText(profile?.ProfileDefiner?.definition, mode);
            }

            return "";
        }

        public static string FormatNameText(ProfileDefinition profile, NameDisplayMode mode = NameDisplayMode.FullName)
        {
            if (profile == null ||
                string.IsNullOrWhiteSpace(profile.firstName) &&
                string.IsNullOrWhiteSpace(profile.lastName))
            {
                return "NO NAME";
            }

            var result = "";

            var firstName = profile.firstName.Trim();
            var lastName = profile.lastName.Trim();

            switch (mode)
            {
                case NameDisplayMode.FullName:
                    {
                        result = $"{profile.firstName} {profile.lastName}";
                        break;
                    }

                case NameDisplayMode.FirstName:
                    {
                        result = firstName;
                        break;
                    }

                case NameDisplayMode.LastName:
                    {
                        result = lastName;
                        break;
                    }

                case NameDisplayMode.Initials:
                    {
                        if (firstName.Length > 0)
                        {
                            result = $"{firstName[0]}. {lastName}";
                        }
                        break;
                    }

                case NameDisplayMode.ThreeLetterAbbreviation:
                    {
                        if (lastName.Length >= 3)
                        {
                            result = $"{lastName[0]}{lastName[1]}{lastName[2]}";
                            result = result.ToUpper();
                        }
                        else
                        {
                            result = lastName;
                        }
                        break;
                    }
            }

            return result;
        }

        public static string FormatVehicleNameText(int index, VehicleNameDisplayMode mode = VehicleNameDisplayMode.FullName)
        {
            var entity = GeneralHelper.GetEntity(index);

            if (entity != null && entity.Vehicle != null)
            {
                return FormatVehicleNameText(entity.Vehicle.VehicleDefinition, mode);
            }

            return "";
        }

        public static string FormatVehicleNameText(VehicleDefinition vehicle, VehicleNameDisplayMode mode = VehicleNameDisplayMode.FullName)
        {
            if (vehicle == null)
                return "NO NAME";

            switch (mode)
            {
                case VehicleNameDisplayMode.FullName:
                    {
                        if (vehicle.manufacturer == null)
                        {
                            return vehicle.objectName;
                        }

                        return $"{vehicle.manufacturer.displayName} {vehicle.objectName} {FormatYearText(vehicle.year)}";
                    }

                case VehicleNameDisplayMode.ModelName:
                    {
                        return vehicle.objectName;
                    }
            }

            return "";
        }

        public static string FormatTargetScoreText(int index)
        {
            if (RaceManager.Instance == null)
            {
                return "";
            }

            var scores = RaceManager.Instance.Session.targetScores;

            if (index < 0 || index >= scores.Count)
            {
                return "N/A";
            }

            return Format(scores[index]);

            string Format(float value)
            {
                switch (RaceManager.Instance.Session.GetPositioningMode())
                {
                    case RacePositioningMode.Distance:
                        {
                            return UIHelper.FormatOrdinalText((int)value);
                        }

                    case RacePositioningMode.DriftPoints:
                    case RacePositioningMode.Score:
                        {
                            return UIHelper.FormatPointsText(value);
                        }

                    case RacePositioningMode.TotalSpeed:
                    case RacePositioningMode.AverageSpeed:
                        {
                            return UIHelper.FormatSpeedText(value, true);
                        }

                    case RacePositioningMode.TotalTime:
                    case RacePositioningMode.BestLapTime:
                        {
                            return UIHelper.FormatTimeText(value);
                        }

                    default:
                        return value.ToString();
                }
            }
        }

        public static string FormatDistanceText(float distance, bool addSymbol = false, bool shorten = true)
        {
            var unit = RGSKCore.Instance.UISettings.distanceUnit;

            if (shorten && distance >= 999)
            {
                unit = RGSKCore.Instance.UISettings.distanceUnit == DistanceUnit.Meters ||
                       RGSKCore.Instance.UISettings.distanceUnit == DistanceUnit.Kilometers ?
                       DistanceUnit.Kilometers : DistanceUnit.Miles;
            }

            var newDist = ConversionHelper.ConvertDistance(distance, unit);
            var symbol = addSymbol ? distance >= 0 ? "+" : "-" : "";
            return $"{symbol}{Mathf.Abs(newDist).ToString(GetDistanceFormatSpecifier(unit))} {GetDistanceUnitSymbol(unit)}";
        }

        public static string FormatSpeedText(float speedInKMH, bool includeUnit = false)
        {
            var newSpeed = ConversionHelper.ConvertSpeed(speedInKMH / 3.6f, RGSKCore.Instance.UISettings.speedUnit, true);
            var result = Mathf.Abs(newSpeed).ToString();

            if (includeUnit)
            {
                result += $" {GetSpeedUnitSymbol(RGSKCore.Instance.UISettings.speedUnit)}";
            }

            return result;
        }

        public static string FormatGearText(int gear)
        {
            return gear == 0 ? "R" :
                   gear == 1 ? "N" :
                   (gear - 1).ToString();
        }

        public static string FormatCurrencyText(float value, bool addFormat = false)
        {
            return addFormat ? string.Format(RGSKCore.Instance.UISettings.currencyFormat, value.ToString("N0")) : value.ToString("N0");
        }

        public static string FormatPointsText(float value, bool suffix = false)
        {
            var pts = suffix ? " pts" : "";
            return $"{value.ToString("N0")}{pts}";
        }

        public static string FormatPercentageText(float value)
        {
            var val = Mathf.Clamp(value, 0, value);
            return $"{Mathf.Round(val)}%";
        }

        public static string FormatAngleText(float value)
        {
            return $"{value}°";
        }

        public static string FormatYearText(string text)
        {
            if (text.Length < 2)
                return text;

            return $"'{text.Substring(text.Length - 2)}";
        }

        public static string FormatOrdinalText(int number, float ordinalSize = -1)
        {
            return $"{number}{Ordinal(number).AddSizeTags(ordinalSize)}";
        }

        public static string Ordinal(int number)
        {
            if (number <= 0)
            {
                return "";
            }

            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    {
                        return "th";
                    }
            }

            switch (number % 10)
            {
                case 1:
                    {
                        return "st";
                    }

                case 2:
                    {
                        return "nd";
                    }

                case 3:
                    {
                        return "rd";
                    }

                default:
                    {
                        return "th";
                    }
            }
        }

        public static string FormatFinishText(RGSKEntity entity)
        {
            var session = RaceManager.Instance.Session;

            switch (session.GetPositioningMode())
            {
                default:
                    {
                        if (session.UseGlobalTimer() && session.raceType.globalTimerElapsedAction != GlobalTimerElapsedAction.FinalLap)
                        {
                            return UIHelper.FormatDistanceText(entity.Competitor.DistanceTravelled);
                        }

                        return UIHelper.FormatTimeText(entity.Competitor.TotalRaceTime);
                    }

                case RacePositioningMode.DriftPoints:
                    {
                        if (entity.DriftController != null)
                        {
                            return UIHelper.FormatPointsText(entity.DriftController.TotalPoints, true);
                        }

                        return "0";
                    }

                case RacePositioningMode.TotalSpeed:
                    {
                        return UIHelper.FormatSpeedText(entity.Competitor.TotalSpeedtrapSpeed, true);
                    }

                case RacePositioningMode.AverageSpeed:
                    {
                        return UIHelper.FormatSpeedText(entity.Competitor.AverageSpeed, true);
                    }

                case RacePositioningMode.Score:
                    {
                        return UIHelper.FormatPointsText(entity.Competitor.Score);
                    }

                case RacePositioningMode.BestLapTime:
                    {
                        return UIHelper.FormatTimeText(entity.Competitor.GetBestLapTime());
                    }
            }
        }

        public static string FormatItemPriceText(ItemDefinition item)
        {
            if (item.IsUnlocked())
            {
                return "";
            }

            switch (item.unlockMode)
            {
                case ItemUnlockMode.Purchase:
                    {
                        return UIHelper.FormatCurrencyText(item.unlockPrice, true);
                    }

                case ItemUnlockMode.XPLevel:
                    {
                        return string.Format(RGSKCore.Instance.UISettings.lockedItemFormat, item.unlockXPLevel);
                    }

                default:
                    {
                        return "";
                    }
            }
        }

        #endregion

        #region Units

        public static string GetSpeedUnitSymbol(SpeedUnit unit)
        {
            switch (unit)
            {
                case SpeedUnit.KMH:
                default:
                    {
                        return "KM/h";
                    }

                case SpeedUnit.MPH:
                    {
                        return "mph";
                    }
            }
        }

        public static string GetDistanceUnitSymbol(DistanceUnit unit)
        {
            switch (unit)
            {
                case DistanceUnit.Meters:
                    {
                        return "m";
                    }

                case DistanceUnit.Feet:
                    {
                        return "ft";
                    }

                case DistanceUnit.Yards:
                    {
                        return "yds";
                    }

                case DistanceUnit.Kilometers:
                    {
                        return "km";
                    }

                case DistanceUnit.Miles:
                    {
                        return "mi";
                    }

                default:
                    {
                        return "m";
                    }
            }
        }

        public static string GetDistanceFormatSpecifier(DistanceUnit unit)
        {
            switch (unit)
            {
                case DistanceUnit.Kilometers:
                case DistanceUnit.Miles:
                    return "F2";

                default:
                    return "F1";
            }
        }
        #endregion

        public static void Assign(this CompetitorUI ui, RaceBoardCellValue value, TMP_Text text)
        {
            switch (value)
            {
                case RaceBoardCellValue.Position:
                    {
                        ui.positionText = text;
                        break;
                    }

                case RaceBoardCellValue.BestLapTime:
                    {
                        ui.bestLapTimeText = text;
                        break;
                    }

                case RaceBoardCellValue.FinishTime:
                    {
                        ui.finishTimeText = text;
                        break;
                    }

                case RaceBoardCellValue.FinishGap:
                    {
                        ui.finishTimeGapText = text;
                        break;
                    }

                case RaceBoardCellValue.Distance:
                    {
                        ui.distanceTravelledText = text;
                        break;
                    }

                case RaceBoardCellValue.RacePercentage:
                    {
                        ui.racePercentageText = text;
                        break;
                    }

                case RaceBoardCellValue.TotalSpeed:
                    {
                        ui.totalSpeedText = text;
                        break;
                    }

                case RaceBoardCellValue.AverageSpeed:
                    {
                        ui.averageSpeedText = text;
                        break;
                    }

                case RaceBoardCellValue.DistanceGap:
                    {
                        ui.leaderDistanceGapText = text;
                        break;
                    }

                case RaceBoardCellValue.TimeGap:
                    {
                        ui.leaderTimeGapText = text;
                        break;
                    }

                case RaceBoardCellValue.TotalLaps:
                    {
                        ui.lapsCompletedText = text;
                        break;
                    }

                case RaceBoardCellValue.Overtakes:
                    {
                        ui.overtakesText = text;
                        break;
                    }

                case RaceBoardCellValue.Score:
                    {
                        ui.scoreText = text;
                        break;
                    }

                case RaceBoardCellValue.TopSpeed:
                    {
                        ui.topSpeedText = text;
                        break;
                    }

                case RaceBoardCellValue.BestLapGap:
                    {
                        ui.bestTimeGapText = text;
                        break;
                    }

                case RaceBoardCellValue.ChampionshipPosition:
                    {
                        ui.championshipPositionText = text;
                        break;
                    }

                case RaceBoardCellValue.ChampionshipRoundPoints:
                    {
                        ui.championshipRoundPointsText = text;
                        break;
                    }

                case RaceBoardCellValue.ChampionshipTotalPoints:
                    {
                        ui.championshipTotalPointsText = text;
                        break;
                    }
            }
        }

        public static void Assign(this DriftUI ui, RaceBoardCellValue value, TMP_Text text)
        {
            switch (value)
            {
                case RaceBoardCellValue.TotalDriftPoints:
                    {
                        ui.totalPointsText = text;
                        break;
                    }
            }
        }

        public static void Assign(this VehicleUI ui, RaceBoardCellValue value, TMP_Text text)
        {
            switch (value)
            {
                case RaceBoardCellValue.VehicleName:
                    {
                        ui.vehicleDefinitionUI.nameText = text;
                        break;
                    }
            }
        }

        public static void Assign(this VehicleUI ui, RaceBoardCellValue value, Image image)
        {
            switch (value)
            {
                case RaceBoardCellValue.VehicleManufacturerLogo:
                    {
                        ui.vehicleDefinitionUI.manufacturerIcon = image;
                        break;
                    }
            }
        }

        public static void Assign(this ProfileUI ui, RaceBoardCellValue value, TMP_Text text)
        {
            switch (value)
            {
                case RaceBoardCellValue.ProfileName:
                    {
                        ui.profileDefinitionUI.nameText = text;
                        break;
                    }
            }
        }

        public static void Assign(this ProfileUI ui, RaceBoardCellValue value, Image image)
        {
            switch (value)
            {
                case RaceBoardCellValue.ProfileCountry:
                    {
                        ui.profileDefinitionUI.country = image;
                        break;
                    }

                case RaceBoardCellValue.ProfileAvatar:
                    {
                        ui.profileDefinitionUI.avatar = image;
                        break;
                    }
            }
        }
    }
}