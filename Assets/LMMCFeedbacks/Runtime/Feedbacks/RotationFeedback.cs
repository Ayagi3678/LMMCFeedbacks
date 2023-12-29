using System;
using LitMotion;

using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class RotationFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private TransformSpace space;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime=1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 zero;
        [SerializeField] private Vector3 one;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Rotation";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            var initialRotation = space==TransformSpace.World?target.rotation:target.localRotation;
            Cancel();
            Handle = LMotion.Create(zero, one, durationTime).WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop?options.loopCount:1, options.loopType)
                .WithEase(ease)
                #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .Bind(value =>
                {
                    switch (space)
                    {
                        case TransformSpace.World:
                            target.rotation = isRelative?Quaternion.Euler(value)*target.rotation:Quaternion.Euler(value);
                            break;
                        case TransformSpace.Local:
                            target.localRotation = isRelative?Quaternion.Euler(value)*target.localRotation:Quaternion.Euler(value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                });
            return Handle;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}