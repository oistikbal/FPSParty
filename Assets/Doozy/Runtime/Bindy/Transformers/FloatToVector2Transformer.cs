// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UseNegatedPatternInIsExpression

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a float value to a Vector2 value by setting it as any or all of the Vector2 components values.
    /// The x and y components can be optionally set with a boolean flag.
    /// </summary>
    [CreateAssetMenu(fileName = "Float to Vector2", menuName = "Doozy/Bindy/Transformer/Float to Vector2", order = -950)]
    public class FloatToVector2Transformer : ValueTransformer
    {
        public override string description => 
            "Transforms a float value to a Vector2 value either by setting it as any or all of the Vector2 components values.";
        
        protected override Type[] fromTypes => new[] {typeof(float)};
        protected override Type[] toTypes => new[] {typeof(Vector2), typeof(Vector3)};
        
        [SerializeField] private bool SetX = true;
        /// <summary>
        /// Set the x component of the Vector2 value to the float value.
        /// </summary>
        public bool setX
        {
            get => SetX;
            set => SetX = value;
        }
        
        [SerializeField] private bool SetY = true;
        /// <summary>
        /// Set the y component of the Vector2 value to the float value.
        /// </summary>
        public bool setY
        {
            get => SetY;
            set => SetY = value;
        }
       
        /// <summary>
        /// Transforms a float value to a Vector2 value either by setting it as any or all of the Vector2 components values.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is float sourceValue)) return source;
            if (!enabled) return source;

            if (target is Vector2 v2TargetValue)
            {
                Vector2 outputValue = v2TargetValue;
                outputValue.x = setX ? sourceValue : outputValue.x;
                outputValue.y = setY ? sourceValue : outputValue.y;

                return outputValue;
            }
            
            if (target is Vector3 v3TargetValue)
            {
                
                Vector3 outputValue = v3TargetValue;
                outputValue.x = setX ? sourceValue : outputValue.x;
                outputValue.y = setY ? sourceValue : outputValue.y;

                return outputValue;
            }

            return source;
        }
    }
}