using System;
using UnityEngine;

namespace LMMCFeedbacks.Runtime
{
    [Serializable] public struct FeedbackPlayerOption
    {
        public bool loop;
        [DisplayIf(nameof(loop))] [Min(1)] public int loopCount;
        public bool initializeOnComplete;
    }
}