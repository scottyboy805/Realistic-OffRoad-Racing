using System;
using UnityEngine;

namespace RGSK
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RequireInterfaceAttribute : PropertyAttribute
    {
        public readonly Type type;

        public RequireInterfaceAttribute(Type value)
        {
            if (!value.IsInterface)
            {
                throw new Exception("Type must be an interface!");
            }
            type = value;
        }
    }
}