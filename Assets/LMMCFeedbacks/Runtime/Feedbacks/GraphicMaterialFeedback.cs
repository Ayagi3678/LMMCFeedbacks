using System;
using LitMotion;

using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace LMMCFeedbacks
{
    [Serializable] public class GraphicMaterialFeedback : IFeedback, IFeedbackTagColor
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Graphic target;
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

        public string Name => "UI/Graphic/Material (Graphic)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }
        public void Cancel()
        {
            if (Handle.IsActive()) Handle.Cancel();
        }

        public MotionHandle Create()
        {
            Cancel();
            if(IsMaterialCacheNull) _materialCache = target.material;
            Handle = propertyType switch
            {
                TweenMaterialPropertyType.Float => LMotion.Create(floatZero, floatOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                    .Bind(value => { _materialCache.SetFloat(propertyName, value); }),
                TweenMaterialPropertyType.Int => LMotion.Create(intZero, intOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                    .Bind(value => { _materialCache.SetInt(propertyName, value); }),
                TweenMaterialPropertyType.Color => LMotion.Create(colorZero, colorOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                    .Bind(value => { _materialCache.SetColor(propertyName, value); }),
                TweenMaterialPropertyType.Vector2 => LMotion.Create(vector2Zero, vector2One, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                    .Bind(value => { _materialCache.SetVector(propertyName, value); }),
                TweenMaterialPropertyType.Vector3 => LMotion.Create(vector3Zero, vector3One, durationTime)
                    .WithDelay(options.delayTime)
                    .WithIgnoreTimeScale(options.ignoreTimeScale)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    #if UNITY_EDITOR
.WithScheduler(LitMotion.Editor.EditorMotionScheduler.Update)
#endif
                    .Bind(value => { _materialCache.SetVector(propertyName, value); }),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Handle;
        }
        public Color TagColor => FeedbackStyling.GraphicFeedbackColor;
    }
}