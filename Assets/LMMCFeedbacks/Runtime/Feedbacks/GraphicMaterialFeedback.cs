using System;
using LitMotion;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace LMMCFeedbacks
{
    [Serializable] public class GraphicMaterialFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Graphic target;
        [SerializeField] private string propertyName;
        [SerializeField] private TweenMaterialPropertyType propertyType;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;

        [SerializeField] [DisplayIf(nameof(propertyType), 0)]
        private float floatZero;

        [SerializeField] [DisplayIf(nameof(propertyType), 0)]
        private float floatOne;

        [SerializeField] [DisplayIf(nameof(propertyType), 1)]
        private int intZero;

        [SerializeField] [DisplayIf(nameof(propertyType), 1)]
        private int intOne;

        [SerializeField] [DisplayIf(nameof(propertyType), 2)]
        private Color colorZero;

        [SerializeField] [DisplayIf(nameof(propertyType), 2)]
        private Color colorOne;

        [SerializeField] [DisplayIf(nameof(propertyType), 3)]
        private Vector2 vector2Zero;

        [SerializeField] [DisplayIf(nameof(propertyType), 3)]
        private Vector2 vector2One;

        [SerializeField] [DisplayIf(nameof(propertyType), 4)]
        private Vector3 vector3Zero;

        [SerializeField] [DisplayIf(nameof(propertyType), 4)]
        private Vector3 vector3One;

        [Space(10)] [SerializeField] [DisplayIf(nameof(propertyType), 0)] [DisableIf(nameof(isInitialized))]
        private float initialFloat;

        [Space(10)] [SerializeField] [DisplayIf(nameof(propertyType), 1)] [DisableIf(nameof(isInitialized))]
        private int initialInt;

        [SerializeField] [Space(10)] [DisplayIf(nameof(propertyType), 2)] [DisableIf(nameof(isInitialized))]
        private Color initialColor;

        [Space(10)] [SerializeField] [DisplayIf(nameof(propertyType), 3)] [DisableIf(nameof(isInitialized))]
        private Vector2 initialVector2;

        [Space(10)] [SerializeField] [DisplayIf(nameof(propertyType), 4)] [DisableIf(nameof(isInitialized))]
        private Vector3 initialVector3;

        [HideInInspector] public bool isInitialized;


        private Material _materialCache;
        private bool IsMaterialCacheNull => _materialCache == null;
        public bool IsActive { get; set; } = true;

        public string Name => "UI/Graphic/Material (Graphic)";
        public FeedbackOption Options => options;
        public MotionHandle Handle { get; private set; }

        public void Complete()
        {
            if (Handle.IsActive()) Handle.Complete();
        }

        public MotionHandle Create()
        {
            Complete();
            if (!isInitialized) InitialSetup();
            if (IsMaterialCacheNull) _materialCache = target.material;
            Handle = propertyType switch
            {
                TweenMaterialPropertyType.Float => LMotion.Create(floatZero, floatOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                    .Bind(value => { _materialCache.SetFloat(propertyName, value); }),
                TweenMaterialPropertyType.Int => LMotion.Create(intZero, intOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                    .Bind(value => { _materialCache.SetInt(propertyName, value); }),
                TweenMaterialPropertyType.Color => LMotion.Create(colorZero, colorOne, durationTime)
                    .WithDelay(options.delayTime)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                    .Bind(value => { _materialCache.SetColor(propertyName, value); }),
                TweenMaterialPropertyType.Vector2 => LMotion.Create(vector2Zero, vector2One, durationTime)
                    .WithDelay(options.delayTime)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                    .Bind(value => { _materialCache.SetVector(propertyName, value); }),
                TweenMaterialPropertyType.Vector3 => LMotion.Create(vector3Zero, vector3One, durationTime)
                    .WithDelay(options.delayTime)
                    .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                    .WithEase(ease)
                    .WithOnComplete(() =>
                    {
                        if (options.initializeOnComplete) Initialize();
                    })
                    .Bind(value => { _materialCache.SetVector(propertyName, value); }),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Handle;
        }

        public void Initialize()
        {
            switch (propertyType)
            {
                case TweenMaterialPropertyType.Float:
                    _materialCache.SetFloat(propertyName, initialFloat);
                    break;
                case TweenMaterialPropertyType.Int:
                    _materialCache.SetInt(propertyName, initialInt);
                    break;
                case TweenMaterialPropertyType.Color:
                    _materialCache.SetColor(propertyName, initialColor);
                    break;
                case TweenMaterialPropertyType.Vector2:
                    _materialCache.SetVector(propertyName, initialVector2);
                    break;
                case TweenMaterialPropertyType.Vector3:
                    _materialCache.SetVector(propertyName, initialVector3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitialSetup()
        {
            if (IsMaterialCacheNull) _materialCache = target.material;

            switch (propertyType)
            {
                case TweenMaterialPropertyType.Float:
                    initialFloat = _materialCache.GetFloat(propertyName);
                    break;
                case TweenMaterialPropertyType.Int:
                    initialInt = _materialCache.GetInt(propertyName);
                    break;
                case TweenMaterialPropertyType.Color:
                    initialColor = _materialCache.GetColor(propertyName);
                    break;
                case TweenMaterialPropertyType.Vector2:
                    initialVector2 = _materialCache.GetVector(propertyName);
                    break;
                case TweenMaterialPropertyType.Vector3:
                    initialVector3 = _materialCache.GetVector(propertyName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.GraphicFeedbackColor;
    }
}