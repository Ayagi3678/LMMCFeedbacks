using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Managers;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class SoundFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private AudioClip clip;
        [SerializeField] private float volumeScale = 1f;
        public bool IsActive { get; set; } = true;

        public string Name => "Audio/Sound";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }


        public MotionHandle Create()
        {
            if (Handle.IsActive()) Handle.Complete();
            var builder = LMotion.Create(0f, 0f, 0f)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithOnComplete(() => { FeedbackSoundManager.Instance.PlaySound(clip, volumeScale); });

            Handle = builder.RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.AudioFeedbackColor;
    }
}