// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable UseNegatedPatternInIsExpression

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a float value to a Vector3 value by setting it as any or all of the Vector3 components values.
    /// The x, y and z components can be optionally set with a boolean flag.
    /// </summary>
    [CreateAssetMenu(fileName = "Float to Vector3", menuName = "Doozy/Bindy/Transformer/Float to Vector3", order = -950)]
    public class FloatToVector3Transformer : ValueTransformer
    {
        public override string description =>
            "Transforms a float value to a Vector3 value either by setting it as any or all of the Vector3 components values.";

        protected override Type[] fromTypes => new[] { typeof(float) };
        protected override Type[] toTypes => new[] { typeof(Vector3), typeof(Vector2) };

        [SerializeField] private bool SetX = true;
        /// <summary>
        /// Set the x component of the Vector3 value to the float value.
        /// </summary>
        public bool setX
        {
            get => SetX;
            set => SetX = value;
        }

        [SerializeField] private bool SetY = true;
        /// <summary>
        /// Set the y component of the Vector3 value to the float value.
        /// </summary>
        public bool setY
        {
            get => SetY;
            set => SetY = value;
        }

        [SerializeField] private bool SetZ = true;
        /// <summary>
        /// Set the z component of the Vector3 value to the float value.
        /// </summary>
        public bool setZ
        {
            get => SetZ;
            set => SetZ = value;
        }

        /// <summary>
        /// Transforms a float value to a Vector3 value either by setting it as any or all of the Vector3 components values.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is float sourceValue)) return source;
            if (!enabled) return source;

            if (target is Vector3 v3TargetValue)
            {
                Vector3 outputValue = v3TargetValue;
                outputValue.x = SetX ? sourceValue : outputValue.x;
                outputValue.y = SetY ? sourceValue : outputValue.y;
                outputValue.z = SetZ ? sourceValue : outputValue.z;

                return outputValue;
            }

            if (target is Vector2 v2TargetValue)
            {
                Vector2 outputValue = v2TargetValue;
                outputValue.x = SetX ? sourceValue : outputValue.x;
                outputValue.y = SetY ? sourceValue : outputValue.y;

                if (SetZ)
                {
                    Debug.LogWarning
                    (
                        "[FloatToVector3Transformer] The target value is a Vector2, but the z component is set to be set. The z component will be ignored. " +
                        "Use FloatToVector3Transformer if you want to set the z component."
                    );
                }

                return outputValue;
            }

            return source;
        }
    }
}
