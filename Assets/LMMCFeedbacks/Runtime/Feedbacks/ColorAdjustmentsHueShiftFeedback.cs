﻿using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class ColorAdjustmentsHueShiftFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,
        IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(-180, 180)] private float zero;
        [SerializeField] [Range(-180, 180)] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialHueShift;

        [HideInInspector] public bool isInitialized;

        private ColorAdjustments _colorAdjustmentsCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Color Adjustments/Hue Shift (Color Adjustments)";
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
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            _colorAdjustmentsCache.active = true;
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

            Handle = builder.BindWithState(_colorAdjustmentsCache,
                (value, state) => { state.hueShift.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            _colorAdjustmentsCache.hueShift.Override(initialHueShift);
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            if (_colorAdjustmentsCache == null)
                _colorAdjustmentsCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ColorAdjustments>();
            initialHueShift = _colorAdjustmentsCache.hueShift.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}