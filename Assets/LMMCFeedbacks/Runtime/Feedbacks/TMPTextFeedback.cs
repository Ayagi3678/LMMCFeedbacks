using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using LitMotion.Editor;
#endif

namespace LMMCFeedbacks
{
    [Serializable] public class TMPTextFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,
        IFeedbackForceMeshUpdate, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private TMP_Text target;
        [SerializeField] private ScrambleMode scrambleMode;

        [SerializeField] [DisplayIf(nameof(scrambleMode), 5)]
        private string scrambleChars;

        [SerializeField] private bool richText;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private string zero;
        [SerializeField] private string one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private string initialText;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Text Mesh Pro/Text (TMP_Text)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Cancel();
            if (!isInitialized) InitialSetup();
            var builder = LMotion.String.Create128Bytes(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                })
                .WithRichText(richText)

#if UNITY_EDITOR
                .WithScheduler(EditorMotionScheduler.Update);
#endif


            if (scrambleMode != ScrambleMode.Custom) builder.WithScrambleChars(scrambleMode);
            else builder.WithScrambleChars(scrambleChars);

            Handle = builder.BindToText(target);
            return Handle;
        }

        public TMP_Text Target => target;

        public void Initialize()
        {
            target.text = initialText;
        }

        public void InitialSetup()
        {
            initialText = target.text;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.UIFeedbackColor;
    }
}