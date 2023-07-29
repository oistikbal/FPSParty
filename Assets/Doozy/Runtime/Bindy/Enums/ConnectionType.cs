// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Describes the connection type between a <see cref="Bindable"/> and a <see cref="Bind"/>.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Bindable only sends value updates to the <see cref="Bind"/>, when its value changes.
        /// It does not receive value updates from the <see cref="Bind"/>.
        /// </summary>
        Sender,

        /// <summary>
        /// Bindable sends value updates to the <see cref="Bind"/>, when its value changes.
        /// It also receives value updates from the <see cref="Bind"/>.
        /// </summary>
        Bidirectional,

        /// <summary>
        /// Bindable only receives value updates from the <see cref="Bind"/>.
        /// It does not send value updates to the <see cref="Bind"/>, when its value changes.
        /// </summary>
        Receiver
    }
}
