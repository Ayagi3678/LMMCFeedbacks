﻿using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class AudioSourcePitchFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private AudioSource target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialPitch;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Audio/Audio Source/Pitch (Audio Source)";
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
            Handle = builder.BindToPitch(target);
            return Handle;
        }

        public Color TagColor => FeedbackStyling.AudioFeedbackColor;

        public void Initialize()
        {
            target.pitch = initialPitch;
        }

        public void InitialSetup()
        {
            initialPitch = target.pitch;
            isInitialized = true;
        }
    }
}