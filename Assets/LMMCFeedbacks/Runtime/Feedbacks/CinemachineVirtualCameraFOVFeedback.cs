#if USE_CINEMACHINE
#if UNITY_EDITOR
using System;
using Cinemachine;
using LitMotion.Editor;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class CinemachineVirtualCameraFOVFeedback : IFeedback, IFeedbackTagColor
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

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            InitialSetup();
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


            Handle = builder.BindWithState(target, (value, state) => { state.m_Lens.FieldOfView = value; });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;

        public void Initialize()
        {
            target.m_Lens.FieldOfView = initialFOV;
        }

        public void InitialSetup()
        {
            if (!isInitialized)
            {
                initialFOV = target.m_Lens.FieldOfView;
                isInitialized = true;
            }
        }
    }
}
#endif