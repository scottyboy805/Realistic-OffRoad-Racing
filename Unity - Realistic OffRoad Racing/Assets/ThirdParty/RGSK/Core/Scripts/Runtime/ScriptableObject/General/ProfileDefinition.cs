using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Profile")]
    public class ProfileDefinition : ScriptableObject
    {
        public string firstName;
        public string lastName;
        public CountryDefinition nationality;
        public Sprite avatar;

        [TextArea(5, 5)]
        public string description;
    }
}