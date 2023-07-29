// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Base class for all value transformers.
    /// </summary>
    public abstract class ValueTransformer : ScriptableObject
    {
        /// <summary>
        /// Transformer description (used in the inspector).
        /// </summary>
        public abstract string description { get; }
        
        [SerializeField] private bool Enabled = true;
        /// <summary> Specifies if the transformer is enabled or not. </summary>
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }
        
        /// <summary>
        /// Types that the transformer can format from.
        /// </summary>
        protected abstract Type[] fromTypes { get; }

        /// <summary>
        /// Types that the transformer can format to.
        /// </summary>
        protected abstract Type[] toTypes { get; }

        /// <summary>
        /// Formats the value from the specified type to the specified type.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public abstract object Transform(object source, object target);

        /// <summary>
        /// Checks if the transformer can format the specified types.
        /// </summary>
        /// <param name="fromType"> The type to format from </param>
        /// <param name="toType"> The type to format to </param>
        /// <returns> True if the transformer can format the specified types, false otherwise </returns>
        public virtual bool CanFormat(Type fromType, Type toType)
        {
            if (fromType == null) return false;
            if (toType == null) return false;
            if (!Array.Exists(fromTypes, type => type == fromType)) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!Array.Exists(toTypes, type => type == toType)) return false;
            return true;
        }

        /// <summary>
        /// Checks if the transformer can transform from the specified type.
        /// </summary>
        /// <param name="fromType"> The type to transform from </param>
        /// <returns> True if the transformer can transform from the specified type, false otherwise </returns>
        public virtual bool CanFormatFrom(Type fromType) =>
            fromType != null && Array.Exists(fromTypes, type => type == fromType);

        /// <summary>
        /// Checks if the transformer can transform to the specified type.
        /// </summary>
        /// <param name="toType"> The type to transform to </param>
        /// <returns> True if the transformer can transform to the specified type, false otherwise </returns>
        public virtual bool CanFormatTo(Type toType) =>
            toType != null && Array.Exists(toTypes, type => type == toType);

        /// <summary>
        /// Checks if the transformer can transform the specified types.
        /// </summary>
        /// <typeparam name="TFrom"> The type to transform from </typeparam>
        /// <typeparam name="TTo"> The type to transform to </typeparam>
        /// <returns> True if the transformer can transform the specified types, false otherwise </returns>
        public bool CanFormat<TFrom, TTo>() =>
            CanFormat(typeof(TFrom), typeof(TTo));

        /// <summary>
        /// Checks if the transformer can transform from the specified type.
        /// </summary>
        /// <typeparam name="TFrom"> The type to transform from </typeparam>
        /// <returns> True if the transformer can transform from the specified type, false otherwise </returns>
        public bool CanFormatFrom<TFrom>() =>
            CanFormatFrom(typeof(TFrom));

        /// <summary>
        /// Checks if the transformer can transform to the specified type.
        /// </summary>
        /// <typeparam name="TTo"> The type to transform to </typeparam>
        /// <returns> True if the transformer can transform to the specified type, false otherwise </returns>
        public bool CanFormatTo<TTo>() =>
            CanFormatTo(typeof(TTo));
    }
}
