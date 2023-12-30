using System;
using UnityEngine;

namespace LMMCFeedbacks
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute
    {
        public readonly float ComparedFloat;
        public readonly int ComparedInt;
        public readonly string ComparedStr;
        public readonly bool TrueThenShow;
        public readonly string VariableName;
        public readonly Type VariableType;

        public DisableIfAttribute(string variableName, Type variableType, bool trueThenShow = true)
        {
            VariableName = variableName;
            VariableType = variableType;
            TrueThenShow = trueThenShow;
        }

        public DisableIfAttribute(string boolVariableName, bool trueThenShow = true)
            : this(boolVariableName, typeof(bool), trueThenShow)
        {
        }

        public DisableIfAttribute(string strVariableName, string comparedStr, bool trueThenShow = true)
            : this(strVariableName, comparedStr.GetType(), trueThenShow)
        {
            ComparedStr = comparedStr;
        }

        public DisableIfAttribute(string intVariableName, int comparedInt, bool trueThenShow = true)
            : this(intVariableName, comparedInt.GetType(), trueThenShow)
        {
            ComparedInt = comparedInt;
        }

        public DisableIfAttribute(string floatVariableName, float comparedFloat, bool trueThenShow = true)
            : this(floatVariableName, comparedFloat.GetType(), trueThenShow)
        {
            ComparedFloat = comparedFloat;
        }
    }
}