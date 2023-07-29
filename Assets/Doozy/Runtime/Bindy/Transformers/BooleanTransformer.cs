// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable UnusedMember.Global
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a boolean value by returning a specified string when true or false.
    /// Useful for displaying a boolean value as a string in a text component.
    /// <para/> For example, you can display 'Yes' when true and 'No' when false.
    /// Or you can display 'On' when true and 'Off' when false.
    /// </summary>
    [CreateAssetMenu(fileName = "Boolean", menuName = "Doozy/Bindy/Transformer/Boolean", order = -950)]
    public class BooleanTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a boolean value by returning a specified string when true or false.\n\n" +
            "Useful for displaying a boolean value as a string in a text component.\n\n" +
            "For example, you can display 'Yes' when true and 'No' when false.\n\n" +
            "Or you can display 'On' when true and 'Off' when false.";
        
        protected override Type[] fromTypes => new[] { typeof(bool) };
        protected override Type[] toTypes => new[] { typeof(string) };

        public override bool CanFormat(Type fromType, Type toType)
        {
            if (fromType == null) return false;
            if (toType == null) return false;
            if (fromType != typeof(bool)) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (toType != typeof(string)) return false;
            return false;
        }

        [SerializeField] protected string TrueString = "On";
        /// <summary>
        /// The string to return when the boolean value is true.
        /// </summary>
        public string trueString
        {
            get => TrueString;
            set => TrueString = value;
        }

        [SerializeField] protected string FalseString = "Off";
        /// <summary>
        /// The string to return when the boolean value is false.
        /// </summary>
        public string falseString
        {
            get => FalseString;
            set => FalseString = value;
        }

        /// <summary>
        /// Transforms the given value if it is a boolean and returns the transformed value as a string.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (source.GetType() != typeof(bool)) return source;
            if (!enabled) return source;
            bool boolValue = (bool)source;
            return boolValue ? TrueString : FalseString;
        }
    }
}
