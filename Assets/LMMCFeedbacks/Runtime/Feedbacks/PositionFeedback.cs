using System;
using LitMotion;

using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class PositionFeedback : IFeedback, IFeedbackTagColor
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

        public string Name => "Transform/Position";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            var createPosition = space==TransformSpace.World?target.position:target.localPosition;
            Handle = LMotion.Create(zero, one, durationTime)
                .WithDelay(options.delayTime)
                .WithIgnoreTimeScale(options.ignoreTimeScale)
                .WithLoops(options.loop?options.loopCount:1, options.loopType)
                .WithEase(ease)
                #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                .BindWithState( target,(value, state) =>
                {
                    switch (space)
                    {
                        case TransformSpace.World:
                            state.position = isRelative?value+createPosition:value;
                            break;
                        case TransformSpace.Local:
                            state.localPosition = isRelative?value+createPosition:value;
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