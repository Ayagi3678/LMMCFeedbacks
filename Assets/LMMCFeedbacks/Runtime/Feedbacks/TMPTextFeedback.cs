using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using TMPro;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class TMPTextFeedback : IFeedback, IFeedbackTagColor, IFeedbackSceneRepaint,IFeedbackForceMeshUpdate
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private TMP_Text target;
        [SerializeField] private ScrambleMode scrambleMode;
        [SerializeField][DisplayIf(nameof(scrambleMode),5)] private string scrambleChars;
        [SerializeField] private bool richText;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private string zero;
        [SerializeField] private string one;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Text Mesh Pro/Text (TMP_Text)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            #if UNITY_EDITOR
            target.ForceMeshUpdate(false,true);
            #endif
            
            Cancel();
            var builder = LMotion.String.Create128Bytes(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
#if UNITY_EDITOR
                .WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .WithRichText(richText);

            if (scrambleMode != ScrambleMode.Custom) builder.WithScrambleChars(scrambleMode);
            else builder.WithScrambleChars(scrambleChars);
                
            Handle = builder.BindToText(target);
            return Handle;
        }

        public Color TagColor => FeedbackStyling.UIFeedbackColor;
        public TMP_Text Target => target;
    }
}