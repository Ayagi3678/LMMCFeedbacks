using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LMMCFeedbacks
{
    [Serializable] public class ObjectDestroyFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private GameObject target;

        public bool IsActive { get; set; } = true;

        public string Name => "Object/Destroy";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }


        public MotionHandle Create()
        {
            if (Handle.IsActive()) Handle.Complete();
            var builder = LMotion.Create(0f, 0f, 0f)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithOnComplete(() => { Object.Destroy(target); });

            Handle = builder.RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.ObjectFeedbackColor;
    }
}