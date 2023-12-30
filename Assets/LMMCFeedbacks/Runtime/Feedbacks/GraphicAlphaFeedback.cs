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
    [Serializable] public class GraphicAlphaFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Graphic target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialAlpha;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Graphic/Alpha (Graphic)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            InitialSetup();
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


            Handle = builder.BindWithState(target,
                (value, state) => { state.color = new Color(state.color.r, state.color.g, state.color.b, value); });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.GraphicFeedbackColor;

        public void Initialize()
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, initialAlpha);
        }

        public void InitialSetup()
        {
            if (!isInitialized)
            {
                initialAlpha = target.color.a;
                isInitialized = true;
            }
        }
    }
}