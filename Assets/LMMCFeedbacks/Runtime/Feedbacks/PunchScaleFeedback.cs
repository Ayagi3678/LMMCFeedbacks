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
    [Serializable] public class PunchScaleFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 startValue;
        [SerializeField] private Vector3 strength;
        [SerializeField] private int frequency = 10;
        [SerializeField] [Range(0, 1)] private float dampingRatio = 1f;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector3 initialScale;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Punch/Scale (Punch)";
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
            var builder = LMotion.Punch
                    .Create(isRelative ? target.localScale + startValue : startValue, strength, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithFrequency(frequency)
                    .WithDampingRatio(dampingRatio)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                ;
#if UNITY_EDITOR
            builder.WithScheduler(EditorMotionScheduler.Update);
#endif
            Handle = builder.BindToLocalScale(target);
            return Handle;
        }

        public void Initialize()
        {
            target.localScale = initialScale;
        }

        public void InitialSetup()
        {
            initialScale = target.localScale;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}