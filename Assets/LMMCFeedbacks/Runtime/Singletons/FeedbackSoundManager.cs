using UnityEngine;

namespace LMMCFeedbacks.Runtime.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class FeedbackSoundManager : MonoBehaviour
    {
        private static FeedbackSoundManager _instance;
        private AudioSource _audioSource;

        public AudioSource AudioSource
        {
            get
            {
                if (_audioSource != null) return _audioSource;
                _audioSource = GetComponent<AudioSource>();
                return _audioSource;
            }
            private set => _audioSource = value;
        }

        public static FeedbackSoundManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var type = typeof(FeedbackSoundManager);

                _instance = (FeedbackSoundManager)FindObjectOfType(type);
                if (_instance != null) return _instance;

                var gameObject = new GameObject("FeedbackSoundManager", type);
                _instance = gameObject.GetComponent<FeedbackSoundManager>();

                if (_instance == null)
                    Debug.LogError("Problem during the creation of FeedbackSoundManager", gameObject);

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