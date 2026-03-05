using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Misc/CountrySettings")]
    public class CountrySettings : ScriptableObject
    {
        public CountryDefinition defaultCountry;
        public List<CountryDefinition> countries = new List<CountryDefinition>();

        public CountryDefinition GetCountryIndex(int index)
        {
            if (index < 0 || index > countries.Count)
                return defaultCountry;

            return countries[index];
        }

        public int GetCountryIndex(CountryDefinition c)
        {
            if (!countries.Contains(c))
                return -1;

            return countries.IndexOf(c);
        }
    }
}