using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

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

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialPitch;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Sound/Audio Source/Pitch (Audio Source)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Complete();
            if (!isInitialized) InitialSetup();
            var builder = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                })

#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update);
#endif


            Handle = builder.BindToPitch(target);
            return Handle;
        }

        public Color TagColor => FeedbackStyling.AudioFeedbackColor;

        public void Initialize()
        {
            target.pitch = initialPitch;
        }

        public void InitialSetup()
        {
            initialPitch = target.pitch;
            isInitialized = true;
        }
    }
}