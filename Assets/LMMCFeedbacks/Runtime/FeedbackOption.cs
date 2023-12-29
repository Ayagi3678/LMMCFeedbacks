using System;
using LitMotion;

namespace LMMCFeedbacks.Runtime
{
    [Serializable]
    public struct FeedbackOption
    {
        public float delayTime;
        public bool loop;
        [DisplayIf(nameof(loop))] public int loopCount;
        [DisplayIf(nameof(loop))] public LoopType loopType;
        public bool ignoreTimeScale;
    }
}