using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RGSK
{
    public static class RGSKEvents
    {
        //Entity
        public static readonly GameEvent<RGSKEntity> OnEntityAdded = new GameEvent<RGSKEntity>();
        public static readonly GameEvent<RGSKEntity> OnEntityRemoved = new GameEvent<RGSKEntity>();

        //Race
        public static readonly GameEvent OnRaceInitialized = new GameEvent();
        public static readonly GameEvent OnRaceDeInitialized = new GameEvent();
        public static readonly GameEvent<RaceState> OnRaceStateChanged = new GameEvent<RaceState>();
        public static readonly GameEvent OnRaceRestart = new GameEvent();
        public static readonly GameEvent<Competitor> OnLapCompleted = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor> OnCompetitorStarted = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor> OnCompetitorFinished = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor, RaceState> OnCompetitorStateChanged = new GameEvent<Competitor, RaceState>();
        public static readonly GameEvent OnRacePositionsChanged = new GameEvent();
        public static readonly GameEvent<Competitor> OnPositionGained = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor> OnPositionLost = new GameEvent<Competitor>();
        public static readonly GameEvent<CheckpointNode, Competitor> OnHitCheckpoint = new GameEvent<CheckpointNode, Competitor>();
        public static readonly GameEvent<Competitor, float> OnHitSector = new GameEvent<Competitor, float>();
        public static readonly GameEvent<Competitor, float> OnHitSpeedtrap = new GameEvent<Competitor, float>();
        public static readonly GameEvent<Competitor, float> OnTimerExtended = new GameEvent<Competitor, float>();
        public static readonly GameEvent<Competitor, float> OnScoreAdded = new GameEvent<Competitor, float>();
        public static readonly GameEvent<Competitor> OnSetNewBestLapTime = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor> OnWrongwayStart = new GameEvent<Competitor>();
        public static readonly GameEvent<Competitor> OnWrongwayStop = new GameEvent<Competitor>();
        public static readonly GameEvent OnDnfTimerStart = new GameEvent();
        public static readonly GameEvent OnDnfTimerStop = new GameEvent();

        //Championship
        public static readonly GameEvent OnChampionshipInitialized = new GameEvent();
        public static readonly GameEvent OnChampionshipDeInitialized = new GameEvent();
        public static readonly GameEvent OnChampionshipPositionsChanged = new GameEvent();
        public static readonly GameEvent OnChampionshipFinished = new GameEvent();

        //Drift
        public static readonly GameEvent<DriftController> OnDriftStart = new GameEvent<DriftController>();
        public static readonly GameEvent<DriftController> OnDriftPointMultiplierReached = new GameEvent<DriftController>();
        public static readonly GameEvent<DriftController, int> OnDriftComplete = new GameEvent<DriftController, int>();
        public static readonly GameEvent<DriftController, int> OnDriftFail = new GameEvent<DriftController, int>();
        public static readonly GameEvent<DriftController> OnDriftEnd = new GameEvent<DriftController>();
        public static readonly GameEvent<DriftController, string> OnDriftMessage = new GameEvent<DriftController, string>();

        //Camera
        public static readonly GameEvent<CameraPerspective> OnCameraPerspectiveChanged = new GameEvent<CameraPerspective>();
        public static readonly GameEvent<Transform> OnCameraTargetChanged = new GameEvent<Transform>();

        //Pause
        public static readonly GameEvent OnGamePaused = new GameEvent();
        public static readonly GameEvent OnGameUnpaused = new GameEvent();

        //Scene
        public static readonly GameEvent OnBeforeSceneLoad = new GameEvent();
        public static readonly GameEvent OnAfterSceneLoad = new GameEvent();

        //Audio
        public static readonly GameEvent<SoundData> OnMusicPlay = new GameEvent<SoundData>();

        //Input
        public static readonly GameEvent<InputDevice> OnInputDeviceChanged = new GameEvent<InputDevice>();
        public static readonly GameEvent OnInputBindingsChanged = new GameEvent();

        //Replay
        public static readonly GameEvent OnReplayStart = new GameEvent();
        public static readonly GameEvent OnReplayStop = new GameEvent();
    }
}