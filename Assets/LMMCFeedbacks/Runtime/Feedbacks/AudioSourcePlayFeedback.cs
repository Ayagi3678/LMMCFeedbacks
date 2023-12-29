using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class AudioSourcePlayFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private AudioSource target;
        [SerializeField] private ulong delay;
        
        public bool IsActive { get; set; } = true;

        public string Name => "Sound/Audio Source/Play (Audio Source)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if(Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            if(Handle.IsActive()) Handle.Cancel();
            Handle = LMotion.Create(0f, 0f, 0f)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithOnComplete(() =>
                {
                    target.Play(delay);
                })
#if UNITY_EDITOR
                .WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .RunWithoutBinding();
            return Handle;
        }
        public Color TagColor => FeedbackStyling.AudioFeedbackColor;
    }
}