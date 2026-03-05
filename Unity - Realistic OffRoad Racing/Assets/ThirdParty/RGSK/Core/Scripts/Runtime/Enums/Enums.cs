using UnityEngine;

namespace RGSK
{
    #region EDITOR
    public enum AIRouteToolMode
    {
        None,
        RacingLine,
        SpeedZones
    }
    #endregion

    #region AI
    public enum TargetApproachAction
    {
        Follow,
        SlowDown,
        SlowDownAndStop
    }
    #endregion

    #region AUDIO
    public enum AudioGroup
    {
        Master = 0,
        Music = 1,
        SFX = 2,
        Vehicle = 3,
        UI = 4,
    }
    #endregion

    #region INPUT
    public enum InputMode
    {
        Disabled,
        Gameplay,
        UI,
        Replay,
        Spectator,
    }

    public enum InputController
    {
        MouseAndKeyboard,
        Xbox,
        PS4,
        PS5,
    }

    public enum BindingGroup
    {
        Keyboard,
        Gamepad
    }
    #endregion

    #region MISC
    public enum SortOrder
    {
        Descending = 0,
        Ascending = 1
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum DirectionAxis
    {
        Vertical,
        Horizontal
    }

    public enum ItemUnlockMode
    {
        None,
        ByDefault,
        Purchase,
        XPLevel
    }

    public enum ItemOwnedMode
    {
        None,
        ByDefault,
        Purchase,
    }

    public enum VehicleSortOptions
    {
        None = 0,
        Alphabetical = 1,
        Price = 2,
        ClassPerformanceRating = 3,
        TopSpeed = 4,
    }

    public enum TrackSortOptions
    {
        None = 0,
        Alphabetical = 1,
        Price = 2,
        Length = 3,
    }
    #endregion

    #region RACE
    public enum RaceState
    {
        PreRace = 0,
        Countdown = 1,
        Racing = 2,
        PostRace = 3,
        RollingStart = 4,
    }

    public enum RaceDurationMode
    {
        LapBased = 0,
        TimeBased = 1,
        LapAndTime = 2
    }

    public enum RaceStartMode
    {
        StandingStart = 0,
        RollingStart = 1,
    }

    public enum RacePositioningMode
    {
        Distance = 0,
        DriftPoints = 1,
        TotalTime = 2,
        BestLapTime = 3,
        TotalSpeed = 4,
        AverageSpeed = 5,
        Score = 6,
    }

    public enum RaceTimerMode
    {
        Global = 0,
        PerCompetitor = 1
    }

    public enum GlobalTimerElapsedAction
    {
        Finish = 0,
        DisqualifyLastPlace = 1,
        FinalLap = 2,
    }

    public enum DnfTimerStartMode
    {
        Off = 0,
        AfterFirstFinish = 1,
        AfterHalfFinish = 2
    }

    public enum CheckpointType
    {
        Sector = 0,
        TimeExtend = 1,
        Speedtrap = 2
    }

    public enum TrackLayoutType
    {
        Circuit = 0,
        PointToPoint = 1,
    }

    public enum RaceSessionType
    {
        Race = 0,
        TargetScore = 1
    }

    public enum SelectionMode
    {
        Random,
        Selected,
    }

    public enum ColorSelectionMode
    {
        Random,
        Color,
        Livery
    }

    public enum OpponentClassOptions
    {
        All = 0,
        Selected = 1,
        SameAsPlayer = 2,
    }

    public enum ChampionshipSpawnMode
    {
        Random,
        TotalPoints,
        PreviousRoundPosition,
    }

    public enum RaceRepositionRotationOptions
    {
        NextCheckpointDirection,
        PreviousCheckpointRotation
    }
    #endregion

    #region UI
    public enum TimeFormat
    {
        MM_SS_FFF = 0,
        MM_SS = 1,
        SS = 2,
        S_FFF = 3,
        HH_MM_SS = 4
    }

    public enum NumberDisplayMode
    {
        Default = 0,
        Single = 1,
        Ordinal = 2
    }

    public enum NameDisplayMode
    {
        FullName = 0,
        FirstName = 1,
        LastName = 2,
        Initials = 3,
        ThreeLetterAbbreviation = 4,
    }

    public enum VehicleNameDisplayMode
    {
        FullName,
        ModelName
    }

    public enum ButtonType
    {
        Restart,
        BackToMenu,
        QuitApplication,
        WatchReplay,
        DeleteSaveData,
        EndRaceSession,
    }

    public enum MobileControlType
    {
        Touch = 0,
        Wheel = 1,
        Tilt = 2,
    }

    public enum BoardSize
    {
        Full,
        Mini
    }

    public enum BoardCellType
    {
        Text,
        Image
    }

    public enum BoardSortOrder
    {
        RaceStandings,
        ChampionshipStandings
    }

    public enum RaceBoardCellValue
    {
        [InspectorName("Profile/Name")] ProfileName,
        [InspectorName("Profile/Country")] ProfileCountry,

        [InspectorName("Vehicle/Name")] VehicleName,
        [InspectorName("Vehicle/Manufacturer Logo")] VehicleManufacturerLogo,

        [InspectorName("Competitor/Position")] Position,
        [InspectorName("Competitor/Lap")] CurrentLap,
        [InspectorName("Competitor/Best Lap")] BestLapTime,
        [InspectorName("Competitor/Race Percentage")] RacePercentage,
        [InspectorName("Competitor/Distance")] Distance,
        [InspectorName("Competitor/Checkpoints")] Checkpoints,
        [InspectorName("Competitor/Total Speed")] TotalSpeed,
        [InspectorName("Competitor/Total Laps")] TotalLaps,
        [InspectorName("Competitor/Finish Time")] FinishTime,
        [InspectorName("Competitor/Finish Gap")] FinishGap,
        [InspectorName("Competitor/Overtakes")] Overtakes,
        [InspectorName("Competitor/Score")] Score,
        [InspectorName("Competitor/AverageSpeed")] AverageSpeed,
        [InspectorName("Competitor/Leader Distance Gap")] DistanceGap,
        [InspectorName("Competitor/Leader Time Gap")] TimeGap,

        [InspectorName("Drift/Total Points")] TotalDriftPoints,

        [InspectorName("Profile/Avatar")] ProfileAvatar,

        [InspectorName("Competitor/Top Speed")] TopSpeed,
        [InspectorName("Competitor/Best Lap Gap")] BestLapGap,

        [InspectorName("Championship/Position")] ChampionshipPosition,
        [InspectorName("Championship/Round Points")] ChampionshipRoundPoints,
        [InspectorName("Championship/Total Points")] ChampionshipTotalPoints,
    }

    public enum RaceSettingUIType
    {
        RaceType = 0,
        StartMode = 1,
        LapCount = 2,
        OpponentCount = 3,
        StartingPosition = 4,
        SessionTimeLimit = 5,
        AIDifficulty = 6,
        Collision = 7,
        Slipstream = 9,
        Ghost = 10,
        GhostOffset = 11,
        Catchup = 12,
        SpectatorMode = 13,
    }

    public enum GameplaySettingType
    {
        SpeedUnit, //Use MeasureUnit instead
        Transmission,
        Nameplate,
        MobileInput,
        Vibration,
        DistanceUnit, //Use MeasureUnit instead
        FPS,
        ProximityArrows,
        AccelerometerSensitivity,
        AutoThrottle,
        AutoBrake,
        MeasureUnit,
    }
    #endregion

    #region UNITS
    public enum MeasureUnit
    {
        Metric,
        Imperial
    }

    public enum SpeedUnit
    {
        KMH,
        MPH
    }

    public enum DistanceUnit
    {
        Meters,
        Feet,
        Yards,
        Kilometers,
        Miles
    }
    #endregion

    #region VEHICLE
    public enum WheelAxle
    {
        Front,
        Rear
    }

    public enum Drivetrain
    {
        RWD,
        FWD,
        AWD
    }

    public enum VehicleHandlingMode
    {
        Grip,
        Drift
    }

    public enum TransmissionType
    {
        Automatic = 0,
        Manual = 1
    }

    public enum SurfaceEmissionType
    {
        Slip,
        Velocity
    }

    public enum VehicleLightType
    {
        HeadLight = 0,
        TailLight = 1,
        ReverseLight = 2
    }

    public enum ExhaustEffectType
    {
        Smoke = 0,
        Backfire = 1,
        Nitrous = 2
    }

    public enum IKTarget
    {
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
        HeadLook
    }
    #endregion
}