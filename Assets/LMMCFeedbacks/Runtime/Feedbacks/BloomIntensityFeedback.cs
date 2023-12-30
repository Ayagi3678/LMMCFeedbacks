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
    [Serializable] public class BloomIntensityFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint
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
        private Bloom _bloomCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Bloom/Intensity (Bloom)";
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
            if (_bloomCache == null) _bloomCache = target.TryGetVolumeComponent<Bloom>();
            _bloomCache.active = true;
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


            builder.BindWithState(_bloomCache, (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;

        public void Initialize()
        {
            if (_bloomCache != null) _bloomCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            if (_bloomCache == null) _bloomCache = target.TryGetVolumeComponent<Bloom>();
            initialIntensity = _bloomCache.intensity.value;
            isInitialized = true;
        }
    }
}