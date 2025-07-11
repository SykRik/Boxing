#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Boxing.Editor
{
    public class PlayerPrefabGenerator
    {
        #region ===== Fields =====

        private const string saveFolder = "Assets/EZ Assets/Prefabs";
        private const string assetName = "PlayerPrefab.prefab";
        private static string assetPath = $"{saveFolder}/{assetName}";

        #endregion

        #region ===== Methods =====

        [MenuItem("Tools/Generate Player Prefab")]
        public static void GenerateFighterPrefab()
        {

            var playerGO = new GameObject("PlayerPrefab");
            var playerCO = playerGO.AddComponent<CapsuleCollider>();
            var playerRB = playerGO.AddComponent<Rigidbody>();
            var playerMH = new GameObject("ModelHolder");

            playerRB.useGravity = true;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            playerMH.transform.SetParent(playerGO.transform);
            playerMH.transform.localPosition = Vector3.zero;

            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
                AssetDatabase.Refresh();
            }

            PrefabUtility.SaveAsPrefabAsset(playerGO, assetPath);
            Debug.Log("PlayerPrefab saved to: " + assetPath);
            Object.DestroyImmediate(playerGO);
        }

        #endregion
    }
}
#endif