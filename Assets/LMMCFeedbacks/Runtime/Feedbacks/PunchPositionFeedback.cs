using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class PunchPositionFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private TransformSpace space;
        [SerializeField] private bool isRelative = true;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 startValue;
        [SerializeField] private Vector3 strength;
        [SerializeField] private int frequency = 10;
        [SerializeField] [Range(0, 1)] private float dampingRatio = 1f;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector3 initialPosition;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Punch/Position (Punch)";
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
            var builder = LMotion.Punch
                    .Create(isRelative ? target.position + startValue : startValue, strength, durationTime)
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
            Handle = space switch
            {
                TransformSpace.World => builder.BindToPosition(target),
                TransformSpace.Local => builder.BindToLocalPosition(target),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Handle;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;

        public void Initialize()
        {
            switch (space)
            {
                case TransformSpace.World:
                    target.position = initialPosition;
                    break;
                case TransformSpace.Local:
                    target.localPosition = initialPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            initialPosition = space == TransformSpace.World ? target.position : target.localPosition;
            isInitialized = true;
        }
    }
}