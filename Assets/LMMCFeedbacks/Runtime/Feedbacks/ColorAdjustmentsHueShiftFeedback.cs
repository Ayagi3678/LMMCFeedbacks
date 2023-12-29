using System;
using LitMotion;

using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

namespace LMMCFeedbacks
{
    [Serializable] public class ColorAdjustmentsHueShiftFeedback : IFeedback, IFeedbackTagColor , IFeedbackSceneRepaint
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Volume target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField][Range(-180,180)] private float zero;
        [SerializeField][Range(-180,180)] private float one;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Color Adjustments/Hue Shift (Color Adjustments)";
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
                    var colorAdjustments = target.TryGetVolumeComponent<UnityEngine.Rendering.Universal.ColorAdjustments>();
                    colorAdjustments.EnableVolumeparameterAll();
                    colorAdjustments.hueShift.Override(value);
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}