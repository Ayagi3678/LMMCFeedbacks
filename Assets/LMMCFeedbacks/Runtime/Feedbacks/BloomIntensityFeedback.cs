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
    [Serializable]
    public class BloomIntensityFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialIntensity;

        [HideInInspector] public bool isInitialized;
        private Bloom _bloomCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Bloom/Intensity (Bloom)";
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
            if (_bloomCache == null) _bloomCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Bloom>();
            _bloomCache.active = true;
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


            builder.BindWithState(_bloomCache, (value, state) => { state.intensity.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_bloomCache == null) _bloomCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Bloom>();
            _bloomCache.intensity.Override(initialIntensity);
        }

        public void InitialSetup()
        {
            if (_bloomCache == null) _bloomCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<Bloom>();
            initialIntensity = _bloomCache.intensity.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}