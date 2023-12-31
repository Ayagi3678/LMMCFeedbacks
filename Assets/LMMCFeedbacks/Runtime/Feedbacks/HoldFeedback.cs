using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class HoldFeedback : IFeedback, IFeedbackTagColor, IFeedbackHold, IFeedbackNoPlayButton
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private float holdTime = 1f;
        public bool IsActive { get; set; } = true;

        public string Name => "etc.../Hold";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }


        public MotionHandle Create()
        {
            if (Handle.IsActive()) Handle.Complete();
            Handle = LMotion.Create(0f, 0f, holdTime)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .RunWithoutBinding();
            return Handle;
        }

        public float HoldTime => holdTime;

        public Color TagColor => FeedbackStyling.EtcFeedbackColor;
    }
}