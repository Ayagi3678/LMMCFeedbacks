using System;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class ShakeScaleFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 startValue;
        [SerializeField] private Vector3 strength;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Shake Scale";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            var createScale = target.localScale;
            Handle = LMotion.Shake.Create(startValue, strength, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.localScale = isRelative ? value + createScale : value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}