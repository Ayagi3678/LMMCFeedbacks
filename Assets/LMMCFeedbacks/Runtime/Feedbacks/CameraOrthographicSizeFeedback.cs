using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class CameraOrthographicSizeFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
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

        public string Name => "Camera/Orthographic Size (Camera)";
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