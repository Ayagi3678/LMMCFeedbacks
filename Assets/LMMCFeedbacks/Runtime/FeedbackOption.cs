using System;
using LitMotion;
using UnityEngine;

namespace LMMCFeedbacks.Runtime
{
    [Serializable] public struct FeedbackOption
    {
        public float delayTime;
        public bool loop;
        [DisplayIf(nameof(loop))] [Min(1)] public int loopCount;
        [DisplayIf(nameof(loop))] public LoopType loopType;
        public bool ignoreTimeScale;
        public bool initializeOnComplete;
    }
}