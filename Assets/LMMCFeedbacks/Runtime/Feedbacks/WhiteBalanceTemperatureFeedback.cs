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
    [Serializable] public class WhiteBalanceTemperatureFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,
        IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;


        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(-100, 100)] private float zero;
        [SerializeField] [Range(-100, 100)] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialTemperature;

        [HideInInspector] public bool isInitialized;

        private WhiteBalance _whiteBalanceCache;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/White Balance/Temperature (White Balance)";
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
            if (_whiteBalanceCache == null)
                _whiteBalanceCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<WhiteBalance>();
            _whiteBalanceCache.active = true;
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


            Handle = builder.BindWithState(_whiteBalanceCache,
                (value, state) => { state.temperature.Override(value); });
            return Handle;
        }

        public void Initialize()
        {
            if (_whiteBalanceCache == null)
                _whiteBalanceCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<WhiteBalance>();
            _whiteBalanceCache.temperature.Override(initialTemperature);
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            if (_whiteBalanceCache == null)
                _whiteBalanceCache = FeedbackVolumeManager.Instance.volume.TryGetVolumeComponent<WhiteBalance>();
            initialTemperature = _whiteBalanceCache.temperature.value;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}