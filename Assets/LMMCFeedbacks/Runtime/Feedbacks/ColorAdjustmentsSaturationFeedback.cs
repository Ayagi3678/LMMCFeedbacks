﻿using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LMMCFeedbacks
{
    [Serializable] public class ColorAdjustmentsSaturationFeedback : IFeedback, IFeedbackTagColor,
        IFeedbackSceneRepaint,
        IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(-100, 100)] private float zero;
        [SerializeField] [Range(-100, 100)] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialSaturation;

        [HideInInspector] public bool isInitialized;

        private ColorAdjustments _colorAdjustmentsCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Color Adjustments/Saturation (Color Adjustments)";
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
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            _colorAdjustmentsCache.active = true;
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            if (options.ignoreTimeScale) builder.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
            Handle = builder.BindWithState(_colorAdjustmentsCache,
                (value, state) => { state.saturation.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            _colorAdjustmentsCache.saturation.Override(initialSaturation);
        }

        public void InitialSetup()
        {
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            initialSaturation = _colorAdjustmentsCache.saturation.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}