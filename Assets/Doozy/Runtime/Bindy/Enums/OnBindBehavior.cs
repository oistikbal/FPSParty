// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Describes what happens when a <see cref="Bindable"/> is added to a <see cref="Bind"/>.
    /// </summary>
    public enum OnBindBehavior
    {
        /// <summary>
        /// Do nothing. The Bindable does not send or receive any value.
        /// </summary>
        DoNothing,
            
        /// <summary>
        /// The Bindable sends its value to the Bind.
        /// </summary>
        SetValue,
            
        /// <summary>
        /// The Bindable receives the value from the Bind, by checking the last value sent by the Bind.
        /// </summary>
        GetValue
    }
}
