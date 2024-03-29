﻿using System;
using LitMotion;
using LitMotion.Extensions;
using LMMCFeedbacks.Runtime;
using LMMCFeedbacks.Runtime.Enums;
using UnityEngine;

namespace LMMCFeedbacks
{
    [Serializable] public class PositionFeedback : IFeedback, IFeedbackTagColor, IFeedbackInitializable
    {
        [SerializeField] private FeedbackOption options;
        [SerializeField] private Transform target;
        [SerializeField] private TransformSpace space;
        [SerializeField] private bool isRelative;
        [SerializeField] private float durationTime = 1f;
        [SerializeField] private Ease ease;
        [SerializeField] private Vector3 zero;
        [SerializeField] private Vector3 one;

        [Space(10)] [SerializeField] [DisableIf(nameof(isInitialized))]
        private Vector3 initialPosition;

        [HideInInspector] public bool isInitialized;

        public bool IsActive { get; set; } = true;

        public string Name => "Transform/Position";
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
            var currentPosition = space == TransformSpace.World ? target.position : target.localPosition;
            var builder = LMotion.Create(isRelative ? currentPosition + zero : zero,
                    isRelative ? currentPosition + one : one
                    , durationTime)
                .WithDelay(options.delayTime)
                .WithLoops(options.loop ? options.loopCount : 1, options.loopType)
                .WithEase(ease)
                .WithOnComplete(() =>
                {
                    if (options.initializeOnComplete) Initialize();
                });


            Handle = space switch
            {
                TransformSpace.World => builder.BindToPosition(target),
                TransformSpace.Local => builder.BindToLocalPosition(target),
                _ => throw new ArgumentOutOfRangeException()
            };
            return Handle;
        }

        public void Initialize()
        {
            switch (space)
            {
                case TransformSpace.World:
                    target.position = initialPosition;
                    break;
                case TransformSpace.Local:
                    target.localPosition = initialPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitialSetup()
        {
            initialPosition = space == TransformSpace.World ? target.position : target.localPosition;
            isInitialized = true;
        }

        public Color TagColor => FeedbackStyling.TransformFeedbackColor;
    }
}