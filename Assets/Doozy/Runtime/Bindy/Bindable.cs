// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Global;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Bindy
{
    [Serializable]
    public class Bindable : IBindable, ITickable
    {
        [SerializeField] private Ticker Ticker;

        /// <summary>
        /// Internal reference to the GameObject that this <see cref="Bindable"/> is attached to.
        /// This is set by a Binder when the Bindable is added to a Bind.
        /// </summary>
        internal GameObject gameObject { get; set; }

        /// <summary> Bindable value type </summary>
        public Type valueType => bindyValue?.valueType;

        /// <summary>
        /// Unique identifier for this <see cref="Bindable"/>.
        /// </summary>
        public Guid guid { get; }

        /// <summary>
        /// The <see cref="Bind"/> that this <see cref="Bindable"/> is currently part of.
        /// </summary>
        public Bind bind { get; protected internal set; }

        [SerializeField] protected ConnectionType ConnectionType = ConnectionType.Sender;
        /// <summary>
        /// Connection type of this <see cref="Bindable"/>.
        /// </summary>
        public ConnectionType connectionType => ConnectionType;

        [SerializeField] protected BindyValue BindyValue;
        /// <summary>
        /// The <see cref="BindyValue"/> of this <see cref="Bindable"/>.
        /// </summary>
        public BindyValue bindyValue => BindyValue;

        [SerializeField] protected OnBindBehavior OnBindBehavior = OnBindBehavior.DoNothing;
        /// <summary>
        /// Describes what happens when this <see cref="Bindable"/> is added to a <see cref="Bind"/>.
        /// </summary>
        public OnBindBehavior onBindBehavior => OnBindBehavior;

        [SerializeField] protected ValueTransformer Transformer;
        /// <summary>
        /// Transformer applied before the value is set (when receiving a value).
        /// </summary>
        public ValueTransformer transformer
        {
            get => Transformer;
            set => Transformer = value;
        }

        /// <summary>
        /// Check if this <see cref="Bindable"/> has a formatter.
        /// </summary>
        public bool hasTransformer => transformer != null;

        /// <summary>
        /// Invoked when the newValue of this <see cref="Bindable"/> changes.
        /// </summary>
        public UnityAction onValueChanged { get; set; }

        public Bindable()
        {
            guid = Guid.NewGuid();
            Ticker = new Ticker(this);
        }

        public void Initialize()
        {
            bindyValue.Initialize();
            bindyValue.onValueChanged += NotifyBind;
        }

        /// <summary>
        /// The newValue of this <see cref="Bindable"/>.
        /// </summary>
        public object value
        {
            get => GetValue();
            set => SetValue(value);
        }

        /// <summary>
        /// Starts ticking this <see cref="Bindable"/> via a <see cref="Runtime.Bindy.Ticker"/>.
        /// </summary>
        public void StartTicking()
        {
            Ticker.StartTicking();
        }

        /// <summary>
        /// Stops ticking this <see cref="Bindable"/> via a <see cref="Runtime.Bindy.Ticker"/>.
        /// </summary>
        public void StopTicking()
        {
            Ticker.StopTicking();
        }

        /// <summary>
        /// Ticks this <see cref="Bindable"/> via a <see cref="Runtime.Bindy.Ticker"/>.
        /// This method is called by the <see cref="Runtime.Bindy.Ticker"/> at regular intervals.
        /// It if the value has changed, it will notify the bind if it has.
        /// </summary>
        public void Tick()
        {
            bindyValue?.HasValueChanged();
        }

        /// <summary>
        /// Gets the newValue of this <see cref="Bindable"/>.
        /// </summary>
        /// <returns> The newValue of this <see cref="Bindable"/> </returns>
        public object GetValue()
        {
            object currentValue = BindyValue.GetValue();
            return currentValue;
        }

        /// <summary>
        /// Sets the newValue of this <see cref="Bindable"/> and notifies the bind that it has changed.
        /// </summary>
        /// <param name="newValue"> The newValue to set </param>
        public void SetValue(object newValue)
        {
            BindyValue.SetValue(newValue);
            NotifyBind();
        }

        /// <summary>
        /// Sets the newValue of this <see cref="Bindable"/> without notifying the bind that it has changed.
        /// </summary>
        /// <param name="newValue"> The newValue to set </param>
        public void SetValueWithoutNotify(object newValue)
        {
            BindyValue.SetValue(newValue);
        }

        /// <summary>
        /// Notifies the bind that this <see cref="Bindable"/> has changed and invokes the <see cref="onValueChanged"/> event.
        /// </summary>
        protected void NotifyBind()
        {
            if (connectionType == ConnectionType.Receiver) return; //if this bindable is a target, then it doesn't need to notify the bind that it has changed
            bind.NotifyChange(guid);
            onValueChanged?.Invoke();
        }

        /// <summary>
        /// Runs the set OnBindBehavior
        /// </summary>
        public Bindable RunOnBindBehavior()
        {
            if (!bindyValue.IsValid())
                return this; //if the bindable value is not valid, then we don't need to add it to the bind

            switch (onBindBehavior)
            {
                case OnBindBehavior.DoNothing:
                    break;
                case OnBindBehavior.SetValue:
                    //notify the bind that this bindable has changed so that it can update the other bindables and the last value
                    bind.NotifyChange(guid);
                    break;
                case OnBindBehavior.GetValue:
                    //if the last bindable is not null and it's not this bindable
                    //update the value of this bindable with the last value of the bind
                    //this is done at the end of the frame so that the bind has time to update the last value
                    //this fixes the race condition where the bindable value is set before the bind has time to update the last value
                    Coroutiner.ExecuteAtEndOfFrame(() =>
                    {
                        if (bind.lastBindable != null && bind.lastBindable != this)
                            ProcessValue(bind, bind.lastBindable, this);
                    });
                    // ProcessValue(bind, bind.lastValue, bind.lastBindable.valueType, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        /// <summary>
        /// Sets the target of this <see cref="Bindable"/> to the specified <see cref="UnityEngine.Object"/> and property.
        /// </summary>
        /// <param name="targetObject"> The target object which contains the property </param>
        /// <param name="targetPropertyName"> The name of the property </param>
        /// <returns> This <see cref="Bindable"/> </returns>
        public Bindable SetTargetProperty(UnityEngine.Object targetObject, string targetPropertyName)
        {
            bindyValue
                .SetTarget(targetObject)
                .SetProperty(targetPropertyName);
            return this;
        }

        /// <summary>
        /// Sets the target of this <see cref="Bindable"/> to the specified <see cref="UnityEngine.Object"/> and field.
        /// </summary>
        /// <param name="targetObject"> The target object which contains the field </param>
        /// <param name="targetFieldName"> The name of the field </param>
        /// <returns> This <see cref="Bindable"/> </returns>
        public Bindable SetTargetField(UnityEngine.Object targetObject, string targetFieldName)
        {
            bindyValue
                .SetTarget(targetObject)
                .SetField(targetFieldName);
            return this;
        }

        /// <summary>
        /// Sets the target of this <see cref="Bindable"/> to the specified <see cref="UnityEngine.Object"/> and method.
        /// </summary>
        /// <param name="behavior"> The behavior of the method </param>
        /// <returns> This <see cref="Bindable"/> </returns>
        public Bindable SetOnBindBehavior(OnBindBehavior behavior)
        {
            OnBindBehavior = behavior;
            return this;
        }

        /// <summary>
        /// Sets the target of this <see cref="Bindable"/> to the specified <see cref="UnityEngine.Object"/> and method.
        /// </summary>
        /// <param name="connection"> The connection type of this bindable </param>
        /// <returns> This <see cref="Bindable"/> </returns>
        public Bindable SetConnectionType(ConnectionType connection)
        {
            ConnectionType = connectionType;
            return this;
        }

        /// <summary>
        /// Binds this <see cref="Bindable"/> to the specified <see cref="Bind"/>.
        /// </summary>
        /// <param name="targetBind">The bind to bind this bindable to.</param>
        public Bindable BindTo(Bind targetBind)
        {
            targetBind?.AddBindable(this);
            return this;
        }

        /// <summary>
        /// Unbinds this <see cref="Bindable"/> from the specified <see cref="Bind"/>.
        /// </summary>
        /// <param name="targetBind">The bind to unbind this bindable from.</param>
        public Bindable UnbindFrom(Bind targetBind)
        {
            targetBind?.RemoveBindable(this);
            return this;
        }

        /// <summary>
        /// Processes the specified value and applies any transformations or conversions that are needed.
        /// </summary>
        /// <param name="bind"> The bind that this bindable belongs to </param> 
        /// <param name="sourceBindable"> The bindable that the value belongs to </param>
        /// <param name="targetBindable"> The bindable to process the value for </param>
        public static void ProcessValue(Bind bind, Bindable sourceBindable, Bindable targetBindable)
        {
            object sourceValue = sourceBindable.value;
            Type sourceValueType = sourceBindable.valueType;

            //check if the bindable is using a transformer and if it is enabled
            if (targetBindable.hasTransformer && targetBindable.transformer.enabled && targetBindable.transformer.CanFormatFrom(sourceValueType))
            {
                object newValue = targetBindable.transformer.Transform(sourceValue, targetBindable.value);
                Type newValueType = newValue.GetType();
                //check if we need a conversion for the transformed value
                if (newValueType != targetBindable.valueType)
                {
                    IValueConverter converter = ConverterRegistry.GetConverter(newValueType, targetBindable.valueType);
                    if (converter == null)
                    {
                        Debug.Log
                        (
                            $"[Bindy] Source value type: {sourceValueType} has been transformed to {newValueType} " +
                            $"but there is no converter available to convert it to {targetBindable.valueType}. " +
                            $"This happened after using the transformer: '{targetBindable.transformer.GetType().Name}'." +
                            $"You can create and register a converter that can convert between these two types or " +
                            $"you can create and use a Value Transformer to handle the conversion." +
                            $"Next two logs will try to show where the bindables are located."
                        );

                        Debug.Log
                        (
                            sourceBindable.gameObject != null
                                ? $"[Bindy] Source Bindable is on the '{sourceBindable.gameObject.name}' GameObject"
                                : $"[Bindy] Source Bindable is not on a GameObject",
                            sourceBindable.gameObject
                        );

                        Debug.Log
                        (
                            targetBindable.gameObject != null
                                ? $"[Bindy] Target Bindable is on the '{targetBindable.gameObject.name}' GameObject"
                                : $"[Bindy] Target Bindable is not on a GameObject",
                            targetBindable.gameObject
                        );

                        return; // Skip the bindable if there is no converter available
                    }

                    // Update the value of the bindable with the converted formatted value
                    targetBindable.SetValueWithoutNotify(converter.Convert(newValue, targetBindable.valueType));
                    return;
                }

                // No conversion needed for the formatted value - just update the value
                targetBindable.SetValueWithoutNotify(newValue);
                return;
            }

            // Check if the value types are compatible and convert if needed
            if (sourceValueType != targetBindable.valueType)
            {
                IValueConverter converter = ConverterRegistry.GetConverter(sourceValueType, targetBindable.valueType);
                if (converter == null)
                {
                    Debug.Log
                    (
                        $"[Bindy] Did not find a converter for {sourceValueType} to {targetBindable.valueType}. " +
                        $"You can either create and use a Value Transformer to handle this use-case OR " +
                        $"create and register a converter that can convert between them without the need for a transformer." +
                        $"Next two logs will try to show where the bindables are located."
                    );

                    Debug.Log
                    (
                        sourceBindable.gameObject != null
                            ? $"[Bindy] Source Bindable is on the '{sourceBindable.gameObject.name}' GameObject"
                            : $"[Bindy] Source Bindable is not on a GameObject",
                        sourceBindable.gameObject
                    );

                    Debug.Log
                    (
                        targetBindable.gameObject != null
                            ? $"[Bindy] Target Bindable is on the '{targetBindable.gameObject.name}' GameObject"
                            : $"[Bindy] Target Bindable is not on a GameObject",
                        targetBindable.gameObject
                    );

                    return; // Skip the bindable if there is no converter available
                }
                // Update the value of the bindable with the converted value
                targetBindable.SetValueWithoutNotify(converter.Convert(sourceValue, targetBindable.valueType));
                return;
            }

            // No conversion needed - just update the value
            targetBindable.SetValueWithoutNotify(sourceValue);
        }
    }
}
