﻿#if USE_CINEMACHINE
using System;
using Cinemachine;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable]
    public class CinemachineVirtualCameraFOVFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private CinemachineVirtualCamera target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(1e-05f, 179)] private float zero = 60f;
        [SerializeField] [Range(1e-05f, 179)] private float one = 60f;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialFOV;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Camera/FOV (Virtual Camera)";
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
            Handle = builder.BindWithState(target, (value, state) => { state.m_Lens.FieldOfView = value; });
            return Handle;
        }

        public void Initialize()
        {
            target.m_Lens.FieldOfView = initialFOV;
        }

        public void InitialSetup()
        {
            initialFOV = target.m_Lens.FieldOfView;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;
    }
}
#endif