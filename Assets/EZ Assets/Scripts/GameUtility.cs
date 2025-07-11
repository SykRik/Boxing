using UnityEngine;

namespace Boxing
{
    public static class GameUtility
    {
        public static Vector3 ToV3XZ(this Vector3 position)
        {
            return new Vector3(position.x, 0f, position.z);
        }

        public static Vector3 ToV3XZ(this Vector2 position)
        {
            return new Vector3(position.x, 0f, position.y);
        }
    }

}
