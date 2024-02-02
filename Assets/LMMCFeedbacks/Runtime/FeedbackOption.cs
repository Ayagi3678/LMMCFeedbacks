using System;
using LitMotion;
using UnityEngine;

namespace LMMCFeedbacks.Runtime
{
    [Serializable] public class FeedbackOption
    {
        public float delayTime;
        public bool loop;
        [DisplayIf(nameof(loop))] [Min(1)] public int loopCount = 1;
        [DisplayIf(nameof(loop))] public LoopType loopType;
        public bool ignoreTimeScale;
        public bool initializeOnComplete;
    }
}