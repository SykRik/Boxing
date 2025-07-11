#if UNITY_EDITOR
using UnityEditor.Animations;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Boxing.Editor
{
    public static class AnimatorGenerator
    {
        #region ===== Constants =====

        private const string assetFolder = "Assets/EZ Assets/Animator";
        private const string assetName = "Boxing.controller";
        private static string assetPath = $"{assetFolder}/{assetName}";
        private const string animFolder = "Assets/EZ Assets/Animations";

        #endregion

        #region ===== Menu Method =====

        [MenuItem("Tools/Generate Animator")]
        public static void GenerateAnimator()
        {
            EnsureFolderExists(assetFolder);

            var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
            var stateMachine = controller.layers[0].stateMachine;

            // === Default Idle state ===
            var idleState = AddState(controller, "Idle", "Idle", setAsDefault: true);

            // === Combat Actions ===
            AddStateWithAnyTransition(controller, "PunchHead", "Head Punch");
            AddStateWithAnyTransition(controller, "PunchKidneyLeft", "Kidney Punch Left");
            AddStateWithAnyTransition(controller, "PunchKidneyRight", "Kidney Punch Right");
            AddStateWithAnyTransition(controller, "PunchStomach", "Stomach Punch");

            // === Hit Reactions ===
            AddStateWithAnyTransition(controller, "HitHead", "Head Hit");
            AddStateWithAnyTransition(controller, "HitKidney", "Kidney Hit");
            AddStateWithAnyTransition(controller, "HitStomach", "Stomach Hit");

            // === Others ===
            AddStateWithAnyTransition(controller, "Block", "Block");
            AddStateWithAnyTransition(controller, "Knockout", "Knocked Out");
            AddStateWithAnyTransition(controller, "Victory", "Victory");

            Debug.Log($"Animator generated at: {assetPath}");
        }

        #endregion

        #region ===== Helpers =====

        private static void EnsureFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var segments = path.Split('/');
                var current = segments[0];
                for (int i = 1; i < segments.Length; i++)
                {
                    var next = $"{current}/{segments[i]}";
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, segments[i]);
                    current = next;
                }
                AssetDatabase.Refresh();
            }
        }

        private static AnimatorState AddState(AnimatorController controller, string stateName, string animClipName, bool setAsDefault = false)
        {
            var clipPath = $"{animFolder}/{animClipName}.anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null)
            {
                Debug.LogWarning($"⚠️ Missing animation clip: {clipPath}");
                return null;
            }

            var state = controller.layers[0].stateMachine.AddState(stateName);
            state.motion = clip;

            if (setAsDefault)
                controller.layers[0].stateMachine.defaultState = state;

            return state;
        }

        private static void AddStateWithAnyTransition(AnimatorController controller, string triggerName, string animClipName)
        {
            // Add parameter if missing
            if (controller.parameters.All(p => p.name != triggerName))
                controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);

            var state = AddState(controller, animClipName, animClipName);
            if (state == null) return;

            // AnyState → State
            var anyTrans = controller.layers[0].stateMachine.AddAnyStateTransition(state);
            anyTrans.AddCondition(AnimatorConditionMode.If, 0, triggerName);
            anyTrans.hasExitTime = false;
            anyTrans.duration = 0.1f;

            // State → Idle
            var idleState = controller.layers[0].stateMachine.states
                .FirstOrDefault(s => s.state.name == "Idle").state;

            if (idleState != null)
            {
                var backTrans = state.AddTransition(idleState);
                backTrans.hasExitTime = true;
                backTrans.exitTime = 1f;
                backTrans.duration = 0.15f;
            }
        }

        #endregion
    }
}
#endif
