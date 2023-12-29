using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class SizeDeltaFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private RectTransform target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector2 zero;
        [SerializeField] private Vector2 one;

        public bool IsActive { get; set; } = true;

        public string Name => "Rect Transform/Size Delta";
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
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.sizeDelta = value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.RectTransformFeedbackColor;
    }
}