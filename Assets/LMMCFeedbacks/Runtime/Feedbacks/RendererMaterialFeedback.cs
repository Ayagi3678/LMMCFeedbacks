using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class RendererMaterialFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Renderer target;
        [SerializeField] private string propertyName;
        [SerializeField] private TweenMaterialPropertyType propertyType;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] [DisplayIf(nameof(propertyType),0)]private float floatZero;
        [SerializeField] [DisplayIf(nameof(propertyType),0)] private float floatOne;
        [SerializeField] [DisplayIf(nameof(propertyType),1)] private int intZero;
        [SerializeField][DisplayIf(nameof(propertyType),1)] private int intOne;
        [SerializeField] [DisplayIf(nameof(propertyType),2)]private Color colorZero;
        [SerializeField] [DisplayIf(nameof(propertyType),2)]private Color colorOne;
        [SerializeField] [DisplayIf(nameof(propertyType),3)]private Vector2 vector2Zero;
        [SerializeField] [DisplayIf(nameof(propertyType),3)]private Vector2 vector2One;
        [SerializeField] [DisplayIf(nameof(propertyType),4)]private Vector3 vector3Zero;
        [SerializeField] [DisplayIf(nameof(propertyType),4)]private Vector3 vector3One;

        private Material _materialCache;
        private bool IsMaterialCacheNull=>_materialCache==null;
        public bool IsActive { get; set; } = true;

        public string Name => "Renderer/Material (Renderer)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            switch (propertyType)
            {
                case TweenMaterialPropertyType.Float:
                    Handle = LMotion.Create(floatZero, floatOne, durationTime).WithDelay(options.delayTime)
                        .WithIgnoreTimeScale(options.ignoreTimeScale)
                        .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                        .WithEase(ease)
                        #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                        .Bind(value => { _materialCache.SetFloat(propertyName, value); });
                    break;
                case TweenMaterialPropertyType.Int:
                    Handle = LMotion.Create(intZero, intOne, durationTime).WithDelay(options.delayTime)
                        .WithIgnoreTimeScale(options.ignoreTimeScale)
                        .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                        .WithEase(ease)
                        #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                        .Bind(value => { _materialCache.SetInt(propertyName, value); });
                    break;
                case TweenMaterialPropertyType.Color:
                    Handle = LMotion.Create(colorZero, colorOne, durationTime).WithDelay(options.delayTime)
                        .WithIgnoreTimeScale(options.ignoreTimeScale)
                        .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                        .WithEase(ease)
                        #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                        .Bind(value => { _materialCache.SetColor(propertyName, value); });
                    break;
                case TweenMaterialPropertyType.Vector2:
                    Handle = LMotion.Create(vector2Zero, vector2One, durationTime).WithDelay(options.delayTime)
                        .WithIgnoreTimeScale(options.ignoreTimeScale)
                        .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                        .WithEase(ease)
                        #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                        .Bind(value => { _materialCache.SetVector(propertyName, value); });
                    break;
                case TweenMaterialPropertyType.Vector3:
                    Handle = LMotion.Create(vector3Zero, vector3One, durationTime).WithDelay(options.delayTime)
                        .WithIgnoreTimeScale(options.ignoreTimeScale)
                        .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                        .WithEase(ease)
                        #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                        .Bind(value => { _materialCache.SetVector(propertyName, value); });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Handle;
        }

        public Color TagColor => FeedbackStyling.RendererFeedbackColor;
    }
}