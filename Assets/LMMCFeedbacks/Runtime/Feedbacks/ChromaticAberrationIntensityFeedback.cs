﻿using System;
using LitMotion;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LMMCFeedbacks
{
    [Serializable] public class ChromaticAberrationIntensityFeedback : IFeedback, IFeedbackTagColor,
        IFeedbackSceneRepaint,
        IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialIntensity;

        [HideInInspector] public bool isInitialized;

        private ChromaticAberration _chromaticAberrationCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Chromatic Aberration/Intensity (Chromatic Aberration)";
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
            if (_chromaticAberrationCache == null)
                _chromaticAberrationCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ChromaticAberration>();
            _chromaticAberrationCache.active = true;
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            if (options.ignoreTimeScale) builder.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
            Handle = builder.BindWithState(_chromaticAberrationCache,
                (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_chromaticAberrationCache == null)
                _chromaticAberrationCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ChromaticAberration>();
            _chromaticAberrationCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (_chromaticAberrationCache == null)
                _chromaticAberrationCache =
                    FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<ChromaticAberration>();
            initialIntensity = _chromaticAberrationCache.intensity.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}