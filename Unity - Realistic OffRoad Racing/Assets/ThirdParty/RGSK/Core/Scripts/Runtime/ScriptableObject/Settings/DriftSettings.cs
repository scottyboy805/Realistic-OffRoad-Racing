using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class DriftMessage
    {
        public string message;
        public float points;
    }

    [CreateAssetMenu(menuName = "RGSK/Settings/Drift Settings")]
    public class DriftSettings : ScriptableObject
    {
        [Header("General")]
        [Tooltip("The minimum speed (km/h) required to start a drift.")]
        public float minSpeed = 50;

        [Tooltip("The minimum angle required to start a drift.")]
        public float minAngle = 20;

        [Tooltip("How many points are added per second while drifting.")]
        public float pointsPerSecond = 500;

        [Tooltip("How long it will take (seconds) to complete a drift.")]
        public float completeWaitTime = 2f;

        [Tooltip("The collision force required to fail a drift.")]
        public float failCollisionForce = 5f;

        [Tooltip("Dropping below this speed (km/h) will fail the drift.")]
        public float failSpeed = 10;

        [Tooltip("This will prevent the player from going back and \"re-drifting\" sections of the track.")]
        public bool checkForValidDistance = true;

        [Header("Multiplier")]
        [Tooltip("How long it will take (seconds) while drifting to increment the multiplier.")]
        public float multiplierIncreaseRate = 3;

        [Tooltip("How fast the multiplier timer will decrease when not drifting")]
        public float multiplierDecreaseRate = 0.5f;

        [Tooltip("The amount to add on to the multiplier every time it increases.")]
        public float multiplierIncrementValue = 0.1f;

        [Tooltip("The maximum point multiplier value.")]
        public float maxMultiplierValue = 10;

        [Header("Messages")]
        [Tooltip("The message shown when the amount of points is reached.")]
        public DriftMessage[] messages;
    }
}