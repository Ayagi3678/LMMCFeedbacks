﻿using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class DebugLogFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private string playMessage = "";
        [SerializeField] private string stopMessage = "";

        public bool IsActive { get; set; } = true;

        public string Name => "etc.../Debug Log";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            Debug.Log("<color=red>" + stopMessage + "</color>");
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
                    Debug.Log("<color=green>" + playMessage + "</color>");
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