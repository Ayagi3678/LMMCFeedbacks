using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

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
            var builder = LMotion.Create(0f, 0f, holdTime)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType);

            Handle = builder.RunWithoutBinding();
            return Handle;
        }

        public float HoldTime => holdTime;

        public Color TagColor => FeedbackStyling.EtcFeedbackColor;
    }
}