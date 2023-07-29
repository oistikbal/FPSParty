// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable UnusedMemberInSuper.Global

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Represents an object that can be bound to a <see cref="Bind"/> object to synchronize its newValue with other <see cref="IBindable"/> objects.
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// Gets the current value of this bindable object.
        /// </summary>
        /// <returns> The current value of this bindable object. </returns>
        object GetValue();
        
        /// <summary>
        /// Sets the a new value to this bindable object.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        void SetValue(object newValue);
        
        /// <summary>
        /// Sets a new value to this bindable object without triggering the <see cref="Bind"/> object that this bindable is part of.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        void SetValueWithoutNotify(object newValue);
        
        /// <summary>
        /// The bind that this bindable is currently part of.
        /// </summary>
        Bind bind { get; }

        /// <summary>
        /// The unique identifier for this bindable, which is used to identify it within a <see cref="Bind"/> object.
        /// </summary>
        Guid guid { get; }
    }
}
