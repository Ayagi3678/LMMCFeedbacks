using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class ImageFillAmountFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Image target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialFillAmount;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Image/Fill Amount (Image)";
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


            Handle = builder.BindWithState(target, (value, state) => { state.fillAmount = value; });
            return Handle;
        }

        public void Initialize()
        {
            target.fillAmount = initialFillAmount;
        }


        public void InitialSetup()
        {
            initialFillAmount = target.fillAmount;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.UIFeedbackColor;
    }
}