using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class SizeDeltaFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private RectTransform target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector2 zero;
        [SerializeField] private Vector2 one;

        [HideInInspector] public bool isInitialized;

        private Vector2 initialSizeDelta;

        public bool IsActive { get; set; } = true;

        public string Name => "Rect Transform/Size Delta";
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


            Handle = builder.BindToSizeDelta(target);
            return Handle;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;

        public void Initialize()
        {
            target.sizeDelta = initialSizeDelta;
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            initialSizeDelta = target.sizeDelta;
            isInitialized = true;
        }
    }
}