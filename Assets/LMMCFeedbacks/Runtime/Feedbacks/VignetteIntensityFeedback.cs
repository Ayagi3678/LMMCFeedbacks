using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LMMCFeedbacks
{
    [Serializable]
    public class VignetteIntensityFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialIntensity;

        [HideInInspector] public bool isInitialized;

        private Vignette _vignetteCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Vignette/Intensity (Vignette)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Complete();
            if (!isInitialized) InitialSetup();
            if (_vignetteCache == null)
                _vignetteCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Vignette>();
            _vignetteCache.active = true;
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            Handle = builder.BindWithState(_vignetteCache, (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_vignetteCache == null)
                _vignetteCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Vignette>();
            _vignetteCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (_vignetteCache == null)
                _vignetteCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Vignette>();
            initialIntensity = _vignetteCache.intensity.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}