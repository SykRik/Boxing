#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

namespace Boxing.Editor
{

    public class InputActionGenerator
    {
        private const string assetPath = "Assets/EZ Assets/Input/PlayerInputActions.inputactions";

        [MenuItem("Tools/Generate Input Actions")]
        public static void GenerateInputActions()
        {
            // Tạo InputActionAsset mới
            var inputAsset = ScriptableObject.CreateInstance<InputActionAsset>();

            // Tạo Gameplay map
            var map = new InputActionMap("Gameplay");

            // Add actions + bindings
            AddActionWithKey(map, "PunchHead", Key.Q);
            AddActionWithKey(map, "PunchKidney", Key.W);
            AddActionWithKey(map, "PunchStomach", Key.E);
            AddActionWithKey(map, "Block", Key.A);

            inputAsset.AddActionMap(map);

            // Save ra file JSON
            var json = inputAsset.ToJson();
            File.WriteAllText(assetPath, json);
            AssetDatabase.Refresh();

            Debug.Log("PlayerInputActions.inputactions generated at: " + assetPath);
        }

        private static void AddActionWithKey(InputActionMap map, string actionName, Key key)
        {
            var action = map.AddAction(actionName, InputActionType.Button);
            action.AddBinding($"<Keyboard>/{key.ToString().ToLower()}");
        }
    }
}
#endif