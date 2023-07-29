// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Defines what type of variable a <see cref="BindyValue"/> object holds.
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// The variable is not a field or a property, it is unset.
        /// </summary>
        None,
        
        /// <summary>
        /// The variable is a field.
        /// </summary>
        Field,
        
        /// <summary>
        /// The variable is a property.
        /// </summary>
        Property
    }
}
