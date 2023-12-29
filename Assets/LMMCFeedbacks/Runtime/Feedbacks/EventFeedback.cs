using System;
using LitMotion;
using LitMotion.Editor;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace LMMCFeedbacks
{
    [Serializable] public class EventFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private UnityEvent onPlay;
        [SerializeField] private UnityEvent onStop;
        public bool IsActive { get; set; } = true;

        public string Name => "etc.../Event";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            onStop?.Invoke();
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
                    onPlay?.Invoke();
                })
#if UNITY_EDITOR
                .WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.EtcFeedbackColor;
    }
}