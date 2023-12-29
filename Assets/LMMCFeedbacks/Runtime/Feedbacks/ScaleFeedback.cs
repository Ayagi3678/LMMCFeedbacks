using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class ScaleFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime=1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 zero;
        [SerializeField] private Vector3 one;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Scale";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            var initialScale = target.localScale;
            Cancel();
            Handle = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop?options.loopCount:1, options.loopType)
                .WithEase(ease)
                #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.localScale = isRelative?value+initialScale:value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}