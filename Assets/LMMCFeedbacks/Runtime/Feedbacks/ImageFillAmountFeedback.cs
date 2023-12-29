using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace LMMCFeedbacks
{
    [Serializable] public class ImageFillAmountFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Image target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Image/Fill Amount (Image)";
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
                    target.fillAmount = value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.UIFeedbackColor;
    }
}