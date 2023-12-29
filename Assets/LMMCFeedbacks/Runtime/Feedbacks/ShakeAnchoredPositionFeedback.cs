using System;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class ShakeAnchoredPositionFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private RectTransform target;
        
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector2 startValue;
        [SerializeField] private Vector2 strength;

        public bool IsActive { get; set; } = true;

        public string Name => "Rect Transform/Shake Anchored Position";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            var createPosition = target.anchoredPosition;
            Handle = LMotion.Shake.Create(startValue, strength, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.anchoredPosition = isRelative ? value + createPosition : value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;
    }
}