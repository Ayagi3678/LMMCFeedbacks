using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class VignetteIntensityFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Volume target;

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

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            InitialSetup();
            if (_vignetteCache == null) _vignetteCache = target.TryGetVolumeComponent<Vignette>();
            _vignetteCache.active = true;
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                })

#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update);
#endif


            Handle = builder.BindWithState(_vignetteCache, (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;

        public void Initialize()
        {
            if (_vignetteCache != null) _vignetteCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            if (_vignetteCache == null) _vignetteCache = target.TryGetVolumeComponent<Vignette>();
            initialIntensity = _vignetteCache.intensity.value;
            isInitialized = true;
        }
    }
}