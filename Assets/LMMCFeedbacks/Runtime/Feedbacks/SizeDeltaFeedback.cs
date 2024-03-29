﻿using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class SizeDeltaFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
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

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Complete();
            if (!isInitialized) InitialSetup();
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            if (options.ignoreTimeScale) builder.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
            Handle = builder.BindToSizeDelta(target);
            return Handle;
        }

        public void Initialize()
        {
            target.sizeDelta = initialSizeDelta;
        }

        public void InitialSetup()
        {
            initialSizeDelta = target.sizeDelta;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;
    }
}