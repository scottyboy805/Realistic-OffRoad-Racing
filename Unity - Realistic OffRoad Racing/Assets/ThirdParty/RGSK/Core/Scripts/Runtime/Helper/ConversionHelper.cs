using UnityEngine;

namespace RGSK.Helpers
{
    public static class ConversionHelper
    {
        /// <summary>
        /// Converts a speed value (meters per second) to a given Speed Unit.
        /// </summary>
        /// <param name="metersPerSecond"></param>
        /// <param name="unit"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static float ConvertSpeed(float metersPerSecond, SpeedUnit unit, bool round = false)
        {
            var result = 0f;

            switch (unit)
            {
                case SpeedUnit.KMH:
                    {
                        result = MPStoKMH(metersPerSecond);
                        break;
                    }

                case SpeedUnit.MPH:
                    {
                        result = MPStoMPH(metersPerSecond);
                        break;
                    }
            }

            if (round)
            {
                result = Mathf.Round(result);
            }

            return result;
        }

        /// <summary>
        /// Converts a distance value (meters) to a given Distance Unit.
        /// </summary>
        /// <param name="meters"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static float ConvertDistance(float meters, DistanceUnit unit)
        {
            switch (unit)
            {
                case DistanceUnit.Meters:
                default:
                    {
                        return meters;
                    }

                case DistanceUnit.Feet:
                    {
                        return MetersToFeet(meters);
                    }

                case DistanceUnit.Yards:
                    {
                        return MetersToYards(meters);
                    }

                case DistanceUnit.Kilometers:
                    {
                        return MetersToKilometers(meters);
                    }

                case DistanceUnit.Miles:
                    {
                        return MetersToMiles(meters);
                    }
            }
        }

        public static float MPStoKMH(float value)
        {
            return value * 3.6f;
        }

        public static float MPStoMPH(float value)
        {
            return value * 2.237f;
        }

        public static float MetersToFeet(float value)
        {
            return value * 3.281f;
        }

        public static float MetersToYards(float value)
        {
            return value * 1.1f;
        }

        public static float MetersToKilometers(float value)
        {
            return value / 1000;
        }

        public static float MetersToMiles(float value)
        {
            return value / 1609;
        }

        public static float LinearToDecibel(float linear)
        {
            return linear != 0 ? 20.0f * Mathf.Log10(linear) : -144.0f;
        }

        public static float DecibelToLinear(float dB)
        {
            return Mathf.Pow(10.0f, dB / 20.0f);
        }
    }
}