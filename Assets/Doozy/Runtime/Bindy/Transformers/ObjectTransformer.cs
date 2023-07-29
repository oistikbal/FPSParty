// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms an object by returning its string representation.
    /// </summary>
    [CreateAssetMenu(fileName = "Object", menuName = "Doozy/Bindy/Transformer/Object", order = -950)]
    public class ObjectTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms an object by returning its string representation.";
        
        protected override Type[] fromTypes => new[] { typeof(object) };
        protected override Type[] toTypes => new[] { typeof(string) };
        
        /// <summary>
        /// Transforms an object value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            return 
                enabled 
                    ? source.ToString() 
                    : source;

        }
    }
}