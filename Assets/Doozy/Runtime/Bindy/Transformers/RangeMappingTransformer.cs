// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MergeIntoLogicalPattern
// ReSharper disable UnusedMember.Global
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a value by mapping it from one range to another.
    /// <para/> For example from 0-1 to 0-100 or from 0-100 to 0-1.
    /// The transformer can be configured with any input and output range.
    /// </summary>
    [CreateAssetMenu(fileName = "Range Mapping", menuName = "Doozy/Bindy/Transformer/Range Mapping", order = -950)]
    public class RangeMappingTransformer : ValueTransformer
    {
        public override string description => 
            "Transforms a value by mapping it from one range to another. For example from 0-1 to 0-100 or from 0-100 to 0-1. " +
            "The transformer can be configured with any input and output range.";
        
        protected override Type[] fromTypes => new[] { typeof(float), typeof(double), typeof(int) };
        protected override Type[] toTypes => new[] { typeof(float) };

        [SerializeField] private float InputMin;
        /// <summary>
        /// The minimum value of the input range.
        /// </summary>
        public float inputMin
        {
            get => InputMin;
            set => InputMin = value;
        }

        [SerializeField] private float InputMax = 1f;
        /// <summary>
        /// The maximum value of the input range.
        /// </summary>
        public float inputMax
        {
            get => InputMax;
            set => InputMax = value;
        }

        [SerializeField] private float OutputMin;
        /// <summary>
        /// The minimum value of the output range.
        /// </summary>
        public float outputMin
        {
            get => OutputMin;
            set => OutputMin = value;
        }

        [SerializeField] private float OutputMax = 100f;
        /// <summary>
        /// The maximum value of the output range.
        /// </summary>
        public float outputMax
        {
            get => OutputMax;
            set => OutputMax = value;
        }

        /// <summary>
        /// Transforms a value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            float floatValue;

            if (source is float || source is double || source is int)
            {
                floatValue = Convert.ToSingle(source);
            }
            else
            {
                return source;
            }

            float outputValue = OutputMin + (floatValue - InputMin) / (InputMax - InputMin) * (OutputMax - OutputMin);
            return outputValue;
        }
    }
}