﻿using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class ShakeAnchoredPositionFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private RectTransform target;

        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector2 startValue;
        [SerializeField] private Vector2 strength;
        [SerializeField] private int frequency = 10;
        [SerializeField] [Range(0, 1)] private float dampingRatio = 1f;
        [SerializeField] private bool randomSeed = true;

        [SerializeField] [DisplayIf(nameof(randomSeed), false)] [Min(1)]
        private uint seed = 1;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector3 initialPosition;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Rect Transform/Shake/Anchored Position (Shake)";
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
            var builder = LMotion.Shake.Create(isRelative ? target.anchoredPosition + startValue : startValue,
                        strength, durationTime)
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
            if (!randomSeed) builder.WithRandomSeed(seed);
            Handle = builder.BindToAnchoredPosition(target);
            return Handle;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;

        public void Initialize()
        {
            target.anchoredPosition = initialPosition;
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            initialPosition = target.anchoredPosition;
            isInitialized = true;
        }
    }
}