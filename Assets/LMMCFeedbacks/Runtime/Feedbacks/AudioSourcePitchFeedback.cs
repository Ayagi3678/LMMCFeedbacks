using System;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class AudioSourcePitchFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private AudioSource target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        public bool IsActive { get; set; } = true;

        public string Name => "Sound/Audio Source/Pitch (Audio Source)";
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
                .WithScheduler(EditorMotionScheduler.Update)
#endif
                .Bind(value => { target.pitch = value; });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.AudioFeedbackColor;
    }
}