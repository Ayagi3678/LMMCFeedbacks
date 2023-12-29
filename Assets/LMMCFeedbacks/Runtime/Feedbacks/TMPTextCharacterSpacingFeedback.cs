﻿using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using TMPro;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class TMPTextCharacterSpacingFeedback : IFeedback, IFeedbackTagColor , IFeedbackSceneRepaint , IFeedbackForceMeshUpdate
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private TMP_Text target;
        
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private float zero;
        [SerializeField] private float one;

        public bool IsActive { get; set; } = true;

        public string Name => "UI/Text Mesh Pro/Character Spacing (TMP_Text)";
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
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    target.characterSpacing = value;
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.UIFeedbackColor;
        public TMP_Text Target => target;
    }
}