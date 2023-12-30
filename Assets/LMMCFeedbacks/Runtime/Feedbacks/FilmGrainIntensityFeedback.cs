using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class FilmGrainIntensityFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,
        IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialIntensity;

        [HideInInspector] public bool isInitialized;

        private FilmGrain _filmGrainCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Film Grain/Intensity (Film Grain)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            if (isInitialized) InitialSetup();
            if (_filmGrainCache == null)
                _filmGrainCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<FilmGrain>();
            _filmGrainCache.active = true;
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


            Handle = builder.BindWithState(_filmGrainCache, (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_filmGrainCache == null)
                _filmGrainCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<FilmGrain>();
            _filmGrainCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (_filmGrainCache == null)
                _filmGrainCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<FilmGrain>();
            initialIntensity = _filmGrainCache.intensity.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}