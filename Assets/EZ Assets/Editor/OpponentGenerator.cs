#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Boxing.Editor
{
    public class OpponentGenerator
    {
        [MenuItem("Tools/Generate Opponents JSON")]
        public static void GenerateOpponentsJson()
        {
            var opponents = new List<CharacterProfile>();

            for (int i = 1; i <= 10; i++)
            {
                opponents.Add(new CharacterProfile
                {
                    OpponentName = $"Fighter{i:00}",
                    MaxHP = 70 + i * 10,
                    MaxMana = 10 + i * 5
                });
            }

            var wrapper = new CharacterProfiles { opponents = opponents };
            var json = JsonUtility.ToJson(wrapper, true);

            var path = Path.Combine(Application.dataPath, "EZ Assets/Resources/opponents.json");
            File.WriteAllText(path, json);

            Debug.Log("Generated opponents.json at: " + path);
            AssetDatabase.Refresh();
        }
    }
}
#endif