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
    [Serializable] public class WhiteBalanceTintFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Volume target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(-100, 100)] private float zero;
        [SerializeField] [Range(-100, 100)] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialTint;

        [HideInInspector] public bool isInitialized;

        private WhiteBalance _whiteBalanceCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/White Balance/Tint (White Balance)";
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
            if (_whiteBalanceCache == null) _whiteBalanceCache = target.TryGetVolumeComponent<WhiteBalance>();
            _whiteBalanceCache.active = true;
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


            Handle = builder.BindWithState(_whiteBalanceCache, (value, state) => { state.tint.Override(value); });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;

        public void Initialize()
        {
            if (_whiteBalanceCache != null) _whiteBalanceCache.tint.value = initialTint;
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            if (_whiteBalanceCache == null) _whiteBalanceCache = target.TryGetVolumeComponent<WhiteBalance>();
            initialTint = _whiteBalanceCache.tint.value;
            isInitialized = true;
        }
    }
}