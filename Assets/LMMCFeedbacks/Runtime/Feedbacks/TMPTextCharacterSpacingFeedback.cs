using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class TMPTextCharacterSpacingFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,
        IFeedbackForceMeshUpdate
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private TMP_Text target;

        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private float initialCharacterSpacing;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Text Mesh Pro/Character Spacing (TMP_Text)";
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


            Handle = builder.BindWithState(target, (value, state) => state.characterSpacing = value);
            return Handle;
        }

        public TMP_Text Target => target;

        public Color TagColor => FeedbackStyling.UIFeedbackColor;

        public void Initialize()
        {
            target.characterSpacing = initialCharacterSpacing;
        }

        public void InitialSetup()
        {
            if (isInitialized) return;
            initialCharacterSpacing = target.characterSpacing;
            isInitialized = true;
        }
    }
}