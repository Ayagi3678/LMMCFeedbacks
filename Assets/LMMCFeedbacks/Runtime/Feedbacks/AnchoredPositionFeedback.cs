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
    [Serializable] public class AnchoredPositionFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private RectTransform target;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector2 zero;
        [SerializeField] private Vector2 one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector2 initialPosition;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Rect Transform/Anchored Position";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            if (!isInitialized) InitialSetup();
            var builder = LMotion
                .Create(isRelative ? zero + target.anchoredPosition : zero,
                    isRelative ? one + target.anchoredPosition : one, durationTime).WithDelay(options.delayTime)
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
            Handle = builder.BindToAnchoredPosition(target);
            return Handle;
        }


        public void Initialize()
        {
            target.anchoredPosition = initialPosition;
        }

        public void InitialSetup()
        {
            initialPosition = target.anchoredPosition;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;
    }
}