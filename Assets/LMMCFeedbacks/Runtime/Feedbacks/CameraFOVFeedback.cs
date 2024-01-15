using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class CameraFOVFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Camera target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [Range(1e-05f, 179)] private float zero = 60;
        [SerializeField] [Range(1e-05f, 179)] private float one = 60f;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialFOV;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Camera/FOV (Camera)";
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
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            Handle = builder.BindWithState(target, (value, state) => { state.fieldOfView = value; });
            return Handle;
        }

        public void Initialize()
        {
            target.fieldOfView = initialFOV;
        }

        public void InitialSetup()
        {
            initialFOV = target.fieldOfView;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;
    }
}