// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable UnusedMemberInSuper.Global
namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Represents an object that can be ticked by a <see cref="Ticker"/> object.
    /// </summary>
    public interface ITickable
    {
        /// <summary>
        /// Works like the Update method, but it is called by a <see cref="Ticker"/> object.
        /// </summary>
        void Tick();
        
        /// <summary>
        /// Starts and stops the ticking of this object via a <see cref="Ticker"/> object.
        /// </summary>
        void StartTicking();
        
        /// <summary>
        /// Starts and stops the ticking of this object via a <see cref="Ticker"/> object.
        /// </summary>
        void StopTicking();
    }
}
