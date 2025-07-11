using UnityEngine;

namespace Boxing
{

    public class FighterController : MonoBehaviour
    {
        [Header("References")]
        public Transform modelHolder;                  // Assigned by Generator
        public GameObject[] modelPrefabs;              // Assign in Inspector or at runtime

        private GameObject currentModelInstance;
        private Animator sharedAnimator;

        private void Awake()
        {
            if (modelHolder == null)
            {
                Debug.LogError("⚠ modelHolder is not assigned.");
            }
        }

        /// <summary>
        /// Load one of the available models into the model holder
        /// </summary>
        /// <param name="index">Index in the modelPrefabs array</param>
        public void LoadCharacter(int index)
        {
            if (modelPrefabs == null || modelPrefabs.Length == 0)
            {
                Debug.LogError("⚠ modelPrefabs is empty.");
                return;
            }

            if (index < 0 || index >= modelPrefabs.Length)
            {
                Debug.LogError($"⚠ Invalid model index: {index}");
                return;
            }

            // Xóa model cũ nếu có
            if (currentModelInstance != null)
            {
                Destroy(currentModelInstance);
            }

            // Instantiate model mới
            currentModelInstance = Instantiate(modelPrefabs[index], modelHolder);
            currentModelInstance.transform.localPosition = Vector3.zero;
            currentModelInstance.transform.localRotation = Quaternion.identity;

            // Lấy Animator từ model mới
            sharedAnimator = GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// Call this to trigger an animation
        /// </summary>
        public void TriggerAnimation(string triggerName)
        {
            if (sharedAnimator != null)
            {
                sharedAnimator.SetTrigger(triggerName);
            }
        }
    }
}