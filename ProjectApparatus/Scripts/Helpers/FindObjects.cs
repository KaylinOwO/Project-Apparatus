using UnityEngine;

namespace Hax
{
    public static partial class Helper
    {
        public static T FindObject<T>() where T : Component
        {
            return Object.FindObjectOfType<T>();
        }

        public static T[] FindObjects<T>() where T : Component
        {
            return Object.FindObjectsOfType<T>();
        }
    }
}
