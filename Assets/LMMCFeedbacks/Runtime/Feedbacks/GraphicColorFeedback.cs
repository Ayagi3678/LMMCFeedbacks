﻿using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace LMMCFeedbacks
{
    [Serializable] public class GraphicColorFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Graphic target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Color zero;
        [SerializeField] private Color one;

        [SerializeField] [Space(10)] private Color initialColor;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Graphic/Color (Graphic)";
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
            Handle = builder.BindWithState(target, (value, state) => { state.color = value; });
            return Handle;
        }

        public void Initialize()
        {
            target.color = initialColor;
        }

        public void InitialSetup()
        {
            initialColor = target.color;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.GraphicFeedbackColor;
    }
}