using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class ShakePositionFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
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
        [SerializeField] private bool randomSeed = true;

        [SerializeField] [DisplayIf(nameof(randomSeed), false)] [Min(1)]
        private uint seed = 1;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector3 initialPosition;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Shake/Position (Shake)";
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
            var builder = LMotion.Shake
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
                });


            if (!randomSeed) builder.WithRandomSeed(seed);
            Handle = space switch
            {
                TransformSpace.World => builder.BindToPosition(target),
                TransformSpace.Local => builder.BindToLocalPosition(target),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Handle;
        }

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
            initialPosition = space == TransformSpace.World ? target.position : target.localPosition;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}