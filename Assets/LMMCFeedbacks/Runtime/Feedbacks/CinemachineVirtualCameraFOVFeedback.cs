using System;
using Cinemachine;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class CinemachineVirtualCameraFOVFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private CinemachineVirtualCamera target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField][Range(1e-05f,179)] private float zero =60f;
        [SerializeField][Range(1e-05f,179)] private float one = 60f;

        public bool IsActive { get; set; } = true;

        public string Name => "Camera/FOV (Virtual Camera)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            Handle = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.m_Lens.FieldOfView = value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.CameraFeedbackColor;
    }
}