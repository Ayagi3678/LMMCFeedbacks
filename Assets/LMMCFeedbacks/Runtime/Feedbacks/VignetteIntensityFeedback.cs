using System;
using LitMotion;

using LitMotion.Extensions;
using LMMCFeedbacks.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LMMCFeedbacks
{
    [Serializable] public class VignetteIntensityFeedback : IFeedback, IFeedbackTagColor , IFeedbackSceneRepaint
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Volume target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        public bool IsActive { get; set; } = true;

        public string Name => "Volume/Vignette/Intensity (Vignette)";
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
                    var vignette = target.TryGetVolumeComponent<Vignette>();
                    vignette.EnableVolumeparameterAll();
                    vignette.intensity.Override(value);
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.VolumeFeedbackColor;
    }
}