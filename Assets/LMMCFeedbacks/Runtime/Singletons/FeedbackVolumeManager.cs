using UnityEngine;
using UnityEngine.Rendering;

namespace LMMCFeedbacks.Runtime.Managers
{
    [RequireComponent(typeof(Volume))] public class FeedbackVolumeManager : MonoBehaviour
    {
        private static FeedbackVolumeManager _instance;

        public Volume volume;

        public static FeedbackVolumeManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var type = typeof(FeedbackVolumeManager);

                _instance = (FeedbackVolumeManager)FindObjectOfType(type);
                if (_instance != null) return _instance;
                var volume = (Volume)FindObjectOfType(typeof(Volume));
                if (volume != null) return volume.gameObject.AddComponent<FeedbackVolumeManager>();
                var typeName = type.ToString();

                var gameObject = new GameObject(typeName, type);
                _instance = gameObject.GetComponent<FeedbackVolumeManager>();

                if (_instance == null) Debug.LogError("Problem during the creation of " + typeName, gameObject);

                return _instance;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Reset()
        {
            TryGetComponent(out volume);
        }

        private bool Initialize()
        {
            if (_instance == null)
            {
                _instance = this;
                return true;
            }

            if (Instance == this) return true;

            Destroy(this);
            return false;
        }
    }
}