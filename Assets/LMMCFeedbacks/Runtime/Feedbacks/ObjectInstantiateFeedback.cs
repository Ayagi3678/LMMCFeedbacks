using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LMMCFeedbacks
{
    [Serializable] public class ObjectInstantiateFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool setParent;

        [SerializeField] [DisplayIf(nameof(setParent))]
        private Transform parent;

        [Space(5)] [SerializeField] private TransformSpace positionSpace;

        [SerializeField] private Vector3 initialPosition;
        [SerializeField] private TransformSpace rotationSpace;
        [SerializeField] private Vector3 initialEulerAngles;
        public bool IsActive { get; set; } = true;

        public string Name => "Object/Instantiate";
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
                .WithOnComplete(() =>
                {
                    var instance = Object.Instantiate(prefab);
                    switch (positionSpace)
                    {
                        case TransformSpace.World:
                            instance.transform.position = initialPosition;
                            break;
                        case TransformSpace.Local:
                            instance.transform.localPosition = initialPosition;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (rotationSpace)
                    {
                        case TransformSpace.World:
                            instance.transform.eulerAngles = initialEulerAngles;
                            break;
                        case TransformSpace.Local:
                            instance.transform.localEulerAngles = initialEulerAngles;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (setParent) instance.transform.SetParent(parent);
                });

            if (options.ignoreTimeScale) builder.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
            Handle = builder.RunWithoutBinding();
            return Handle;
        }

        public Color TagColor => FeedbackStyling.ObjectFeedbackColor;
    }
}