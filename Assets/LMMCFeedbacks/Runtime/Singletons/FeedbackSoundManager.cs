using UnityEngine;

namespace LMMCFeedbacks.Runtime.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class FeedbackSoundManager : MonoBehaviour
    {
        private static FeedbackSoundManager _instance;

        public AudioSource AudioSource { get; private set; }

        public static FeedbackSoundManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    _instance.Setup();
                    return _instance;
                }

                var type = typeof(FeedbackSoundManager);

                _instance = (FeedbackSoundManager)FindObjectOfType(type);
                if (_instance != null)
                {
                    _instance.Setup();
                    return _instance;
                }

                var gameObject = new GameObject("FeedbackSoundManager", type);
                _instance = gameObject.GetComponent<FeedbackSoundManager>();

                if (_instance == null)
                    Debug.LogError("Problem during the creation of FeedbackSoundManager", gameObject);

                _instance.Setup();
                return _instance;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        public void PlaySound(AudioClip clip, float volumeScale = 1f)
        {
            AudioSource.PlayOneShot(clip, volumeScale);
        }

        public void StopSound()
        {
            AudioSource.Stop();
        }

        public void Setup()
        {
            if (AudioSource == null) AudioSource = GetComponent<AudioSource>();
        }

        public bool Initialize()
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