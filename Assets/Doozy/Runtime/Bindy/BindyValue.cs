// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;
using Component = UnityEngine.Component;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy
{
    [Serializable]
    public class BindyValue
    {
        /// <summary>
        /// Invoked when the value changes
        /// </summary>
        public UnityAction onValueChanged { get; set; } = () => {};

        [SerializeField] protected internal UnityEngine.Object Target;
        /// <summary> Value target </summary>
        public UnityEngine.Object target => Target;

        [SerializeField] protected internal string VariableName;
        /// <summary> Value variable name </summary>
        public string variableName => VariableName;

        [SerializeField] protected internal VariableType VariableType;
        /// <summary> Value variable type </summary>
        public VariableType variableType => VariableType;

        /// <summary>
        /// Get the type of the value if it's valid
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Type valueType
        {
            get
            {
                if (!initialized) return null;
                if (!IsValid()) return null;

                switch (variableType)
                {
                    case VariableType.None: return null;
                    case VariableType.Field: return targetField != null ? targetField.FieldType : null;
                    case VariableType.Property: return targetProperty != null ? targetProperty.PropertyType : null;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected FieldInfo targetField { get; set; }
        protected PropertyInfo targetProperty { get; set; }

        private Func<object> getter { get; set; }
        private Action<object> setter { get; set; }

        protected bool initialized { get; set; }

        private int lastFrameValueWasChanged { get; set; } = -1;
        private object previousValue { get; set; }

        /// <summary> Reflected value </summary>
        public object value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public BindyValue()
        {
            getter = () => null;
            setter = _ => {};
        }

        public object GetValue()
        {
            if (!initialized) Initialize();
            return !initialized ? default : getter?.Invoke();
        }

        public void SetValue(object newValue)
        {
            if (!initialized) Initialize();
            if (!initialized) return;
            if (setter == null) return;

            // Check if the new value is equal to the current value
            if (Equals(newValue, value)) return;

            // fail safe to avoid infinite loops and stack overflows    
            if (lastFrameValueWasChanged == Time.frameCount) return;
            lastFrameValueWasChanged = Time.frameCount;
            setter.Invoke(newValue);
            onValueChanged?.Invoke();
            // HasValueChanged();                                       
        }

        /// <summary>
        /// Check if it's valid and get either the target FieldInfo or the target PropertyInfo
        /// </summary>
        public bool Initialize()
        {
            initialized = false;
            targetField = null;
            targetProperty = null;
            if (!IsValid()) return false;

            switch (VariableType)
            {
                case VariableType.Property:
                {
                    targetProperty = Target.GetType().GetProperty(VariableName);
                    if (targetProperty != null && targetProperty.CanRead)
                    {
                        getter = () => targetProperty.GetValue(Target, null);
                        if (targetProperty.CanWrite)
                        {
                            setter = val => targetProperty.SetValue(Target, val, null);
                        }
                    }
                    break;
                }
                case VariableType.Field:
                {
                    targetField = Target.GetType().GetField(VariableName);
                    if (targetField != null)
                    {
                        getter = () => targetField.GetValue(Target);
                        setter = val => targetField.SetValue(Target, val);
                    }
                    break;
                }
                case VariableType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            initialized = getter != null;

            // initialized = true;
            previousValue = GetValue();
            return true;
        }

        /// <summary>
        /// Checks if the value has changed since the last time this method was called.
        /// If the value has changed, the onValueChanged event is invoked and the previousValue is updated.
        /// </summary>
        /// <returns> True if the value has changed, false otherwise </returns>
        public bool HasValueChanged()
        {
            if (!initialized) return false;
            if (variableType == VariableType.None) return false;
            object testValue = null;
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (VariableType == VariableType.Property)
                testValue = targetProperty.GetValue(Target, null);
            else if (VariableType == VariableType.Field)
                testValue = targetField.GetValue(Target);
            if (testValue == null) return false;
            bool valueChanged = !Equals(testValue, previousValue);
            if (!valueChanged) return false;
            previousValue = testValue;
            onValueChanged?.Invoke();
            //fail safe to avoid infinite loops and stack overflows
            if (lastFrameValueWasChanged != Time.frameCount)
                lastFrameValueWasChanged = Time.frameCount;
            return true;
        }
        
        /// <summary>
        /// Check if this BindyValue is valid by if it has access to either a Field or a Property (depending on the VariableType)
        /// </summary>
        /// <returns> True if it's valid, false otherwise </returns>
        public bool IsValid()
        {
            if (Target == null)
                return false;

            switch (VariableType)
            {
                case VariableType.None:
                    return false;
                case VariableType.Property:
                    if (VariableName.IsNullOrEmpty()) return false;
                    if (targetProperty != null && targetProperty.Name.Equals(VariableName)) return true;
                    targetProperty = GetPropertyInfos(Target).FirstOrDefault(p => p.Name.Equals(VariableName));
                    return targetProperty != null;
                case VariableType.Field:
                    if (VariableName.IsNullOrEmpty()) return false;
                    if (targetField != null && targetField.Name.Equals(VariableName)) return true;
                    targetField = GetFieldInfos(Target).FirstOrDefault(f => f.Name.Equals(VariableName));
                    return targetField != null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public BindyValue SetTarget(UnityEngine.Object targetObject)
        {
            ClearValueDetails();
            Target = targetObject;
            return this;
        }

        public BindyValue SetProperty(string nameOfProperty)
        {
            VariableName = nameOfProperty;
            VariableType = nameOfProperty.IsNullOrEmpty() ? VariableType.None : VariableType.Property;
            return this;
        }

        public BindyValue SetField(string nameOfField)
        {
            VariableName = nameOfField;
            VariableType = nameOfField.IsNullOrEmpty() ? VariableType.None : VariableType.Field;
            return this;
        }

        public BindyValue ClearValueDetails()
        {
            VariableName = "";
            VariableType = VariableType.None;
            targetField = null;
            targetProperty = null;
            return this;
        }

        public GameObject GetGameObject() =>
            Target switch
            {
                GameObject go => go,
                Component co  => co.gameObject,
                _             => null
            };

        public static FieldInfo[] CachedFieldInfos(IReflect targetType, Type ofType = null) =>
            BindyValueReflectionCache.GetFieldInfos(targetType, ofType);

        public static PropertyInfo[] CachedPropertyInfos(IReflect targetType, Type ofType = null) =>
            BindyValueReflectionCache.GetPropertyInfos(targetType, ofType);

        public static IEnumerable<FieldInfo> GetCachedFieldInfos(IReflect targetType, Type fieldType = null)
        {
            FieldInfo[] fieldInfos = BindyValueReflectionCache.GetFieldInfos(targetType, fieldType);
            return fieldInfos ?? Enumerable.Empty<FieldInfo>();
        }

        public static IEnumerable<PropertyInfo> GetCachedPropertyInfos(IReflect targetType, Type propertyType = null)
        {
            PropertyInfo[] propertyInfos = BindyValueReflectionCache.GetPropertyInfos(targetType, propertyType);
            return propertyInfos ?? Enumerable.Empty<PropertyInfo>();
        }

        // public static IEnumerable<FieldInfo> GetFieldInfos(IReflect targetType, Type fieldType = null) =>
        //     FieldInfos(targetType, fieldType);
        //
        // public static IEnumerable<PropertyInfo> GetPropertyInfos(IReflect targetType, Type propertyType = null) =>
        //     PropertyInfos(targetType, propertyType);

        public static IEnumerable<FieldInfo> GetFieldInfos(object targetObject, Type fieldType = null) =>
            GetCachedFieldInfos(targetObject.GetType(), fieldType);

        public static IEnumerable<PropertyInfo> GetPropertyInfos(object targetObject, Type propertyType = null) =>
            GetCachedPropertyInfos(targetObject.GetType(), propertyType);

        public static IEnumerable<FieldInfo> FieldInfos(IReflect targetType, Type ofType = null) =>
            targetType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(f => ofType == null || f.FieldType == ofType);

        public static IEnumerable<PropertyInfo> PropertyInfos(IReflect targetType, Type ofType = null) =>
            targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(p => (ofType == null || p.PropertyType == ofType) && p.CanRead && p.CanWrite);
    }

    public static class BindyValueReflectionCache
    {
        [ExecuteOnReload]
        // ReSharper disable once UnusedMember.Local
        private static void ClearCache()
        {
            CachedFieldInfos.Clear();
            CachedPropertyInfos.Clear();
        }

        private static readonly Dictionary<(IReflect, Type), FieldInfo[]> CachedFieldInfos = new();
        private static readonly Dictionary<(IReflect, Type), PropertyInfo[]> CachedPropertyInfos = new();

        public static FieldInfo[] GetFieldInfos(IReflect targetType, Type ofType = null)
        {
            (IReflect targetType, Type ofType) key = (targetType, ofType);
            if (CachedFieldInfos.TryGetValue(key, out FieldInfo[] cachedFieldInfos))
                return cachedFieldInfos;

            FieldInfo[] fieldInfos = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(f => ofType == null || f.FieldType == ofType)
                .ToArray();

            CachedFieldInfos.Add(key, fieldInfos);
            return fieldInfos;
        }

        public static PropertyInfo[] GetPropertyInfos(IReflect targetType, Type ofType = null)
        {
            (IReflect targetType, Type ofType) key = (targetType, ofType);
            if (CachedPropertyInfos.TryGetValue(key, out PropertyInfo[] cachedPropertyInfos))
                return cachedPropertyInfos;

            PropertyInfo[] propertyInfos = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(p => (ofType == null || p.PropertyType == ofType) && p.CanRead && p.CanWrite)
                .ToArray();

            CachedPropertyInfos.Add(key, propertyInfos);
            return propertyInfos;
        }
    }
}
