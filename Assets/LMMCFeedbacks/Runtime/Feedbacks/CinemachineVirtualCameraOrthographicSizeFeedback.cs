# if USE_CINEMACHINE
#if UNITY_EDITOR
using System;
using LitMotion.Editor;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#endif

namespace LMMCFeedbacks
{
    [Serializable]
    public class CinemachineVirtualCameraOrthographicSizeFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Camera target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialFOV;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Camera/Orthographic Size (Virtual Camera)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            if (!isInitialized) InitialSetup();
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


            Handle = builder.BindWithState(target, (value, state) => { state.orthographicSize = value; });
            return Handle;
        }

        public void Initialize()
        {
            target.orthographicSize = initialFOV;
        }

        public void InitialSetup()
        {
            initialFOV = target.orthographicSize;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;
    }
}
#endif