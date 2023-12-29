using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class HoldFeedback : IFeedback, IFeedbackTagColor , IFeedbackHold
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private float holdTime = 1f;
        public bool IsActive { get; set; } = true;

        public string Name => "etc.../Hold";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if(Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            if(Handle.IsActive()) Handle.Cancel();
            Handle = LMotion.Create(0f, 0f, holdTime)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
#if UNITY_EDITOR
                .WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.EtcFeedbackColor;
        public float HoldTime => holdTime;
    }
}