using System;
using UnityEngine;

namespace LMMCFeedbacks
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    public class NoCopyAttribute : PropertyAttribute
    {
        
    }
}