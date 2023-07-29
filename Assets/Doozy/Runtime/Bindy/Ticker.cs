// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Global;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy
{
    /// <summary> Ticker is a class that can be used to tick ITickable objects. </summary>
    [Serializable]
    public class Ticker
    {
        /// <summary> Describes the mode in which the Ticker will tick. </summary>
        public enum Mode
        {
            /// <summary> Ticks every frame, regardless of the frame rate. </summary>
            RealTime = 0,

            /// <summary> Ticks based on the FrameInterval (every X frames). </summary>
            FrameBased = 1,

            /// <summary> Ticks based on the TimeInterval (every X seconds). </summary>
            TimeBased = 2
        }

        /// <summary> Frame intervals used when the TickerMode is set to FrameBased. </summary>
        public enum FrameIntervals
        {
            /// <summary> Tick every frame </summary>
            EveryFrame = 0,

            /// <summary> Tick every two (2) frames </summary>
            EveryTwoFrames = 1,

            /// <summary> Tick every three (3) frames </summary>
            EveryThreeFrames = 2,

            /// <summary> Tick every four (4) frames </summary>
            EveryFourFrames = 3,

            /// <summary> Tick every five (5) frames </summary>
            EveryFiveFrames = 4,

            /// <summary> Tick every CustomFrameInterval number of frames </summary>
            Custom = 5
        }

        /// <summary> Time intervals used when the TickerMode is set to TimeBased. </summary>
        public enum TimeIntervals
        {
            /// <summary> Tick once per second (1 second) </summary>
            OncePerSecond = 0,

            /// <summary> Tick twice per second (0.5 seconds) </summary>
            TwicePerSecond = 1,

            /// <summary> Tick five times per second (0.2 seconds) </summary>
            FiveTimesPerSecond = 2,

            /// <summary> Tick ten times per second (0.1 seconds) </summary>
            TenTimesPerSecond = 3,

            /// <summary> Tick twenty times per second (0.05 seconds) </summary>
            TwentyTimesPerSecond = 4,

            /// <summary> Tick thirty times per second (0.033 seconds) </summary>
            ThirtyTimesPerSecond = 5,

            /// <summary> Tick sixty times per second (0.016 seconds) </summary>
            SixtyTimesPerSecond = 6,

            /// <summary> Tick based on the CustomTimeInterval value </summary>
            Custom = 7
        }

        [SerializeField] private Mode TickerMode = Mode.RealTime;
        /// <summary>
        /// Ticker mode that determines how the Ticker will tick.
        /// <para/> RealTime - tick every frame.
        /// <para/> FrameBased - tick based on the FrameInterval.
        /// <para/> TimeBased - tick based on the TimeInterval.
        /// </summary>
        public Mode tickerMode
        {
            get => TickerMode;
            set
            {

                if (TickerMode == value) return; //same value, no need to do anything
                TickerMode = value;              //set the new value
                if (!isRunning) return;          //if the ticker is not running, no need to do anything
                StartTicking();                  //restart the ticker since the mode has changed and the ticker is running
            }
        }

        [SerializeField] private FrameIntervals FrameInterval = FrameIntervals.EveryFrame;
        /// <summary>
        /// Frame interval used when the TickerMode is set to FrameBased.
        /// </summary>
        public FrameIntervals frameInterval
        {
            get => FrameInterval;
            set
            {
                if (FrameInterval == value) return;               //same value, no need to do anything
                FrameInterval = value;                            //set the new value
                Frames = GetFrameInterval(frameInterval, Frames); //set the Frames value based on the FrameInterval value
                if (TickerMode != Mode.FrameBased) return;        //if the ticker mode is not FrameBased, no need to do anything
                if (!isRunning) return;                           //if the ticker is not running, no need to do anything
                StartTicking();                                   //restart the ticker since the mode has changed and the ticker is running
            }
        }

        [SerializeField] private TimeIntervals TimeInterval = TimeIntervals.OncePerSecond;
        /// <summary>
        /// Time interval used when the TickerMode is set to TimeBased.
        /// </summary>
        public TimeIntervals timeInterval
        {
            get => TimeInterval;
            set
            {
                if (TimeInterval == value) return;                //same value, no need to do anything
                TimeInterval = value;                             //set the new value
                Seconds = GetTimeInterval(timeInterval, Seconds); //set the Seconds value based on the TimeInterval value
                if (TickerMode != Mode.TimeBased) return;         //if the ticker mode is not TimeBased, no need to do anything
                if (!isRunning) return;                           //if the ticker is not running, no need to do anything
                StartTicking();                                   //restart the ticker since the mode has changed and the ticker is running
            }
        }

        [SerializeField] private int MaxTicksPerSecond = 60;
        /// <summary>
        /// Maximum number of ticks per second that the Ticker can do.
        /// <para/> -1 means no limit.
        /// <para/> 0 will tick once per second.
        /// </summary>
        public int maxTicksPerSecond => MaxTicksPerSecond;

        [SerializeField] private int Frames = 1;
        /// <summary>
        /// Frame interval in frames used when the TickerMode is set to FrameBased.
        /// How many frames will pass before the Ticker ticks.
        /// </summary>
        public int frames => Frames;

        [SerializeField] private float Seconds = 0.1f;
        /// <summary>
        /// Time interval in seconds used when the TickerMode is set to TimeBased.
        /// How many seconds will pass before the Ticker ticks.
        /// </summary>
        public float seconds => Seconds;

        /// <summary>
        /// Target ITickable interface that will have its Tick method called every time the Ticker ticks
        /// </summary>
        public ITickable tickable { get; }

        /// <summary>
        /// Check if the Ticker is currently running
        /// </summary>
        public bool isRunning { get; private set; }

        private Coroutine tickerCoroutine { get; set; }

        public Ticker(ITickable tickable)
        {
            this.tickable = tickable;
        }

        /// <summary>
        /// Perform a single tick of the Ticker, executing the Tick method of the ITickable interface that was provided during initialization.
        /// </summary>
        public void Tick()
        {
            tickable.Tick();
        }

        /// <summary>
        /// Start the ticker based on the TickerMode
        /// </summary>
        public void StartTicking()
        {
            if (isRunning) StopTicking();
            isRunning = true;
            tickerCoroutine = Coroutiner.Start(GetIEnumerator());
        }

        /// <summary>
        /// Stop the ticker and reset the tickerCoroutine
        /// </summary>
        public void StopTicking()
        {
            isRunning = false;
            if (tickerCoroutine == null) return;
            Coroutiner.Stop(tickerCoroutine);
            tickerCoroutine = null;
        }

        /// <summary>
        /// Ticker that ticks a set number of times per second or continuously if the max value is -1.
        /// </summary>
        /// <returns>IEnumerator used to start the coroutine</returns>
        private IEnumerator RealTimeTicker()
        {
            while (isRunning)
            {
                // If maxTicksPerSecond is less than or equal to zero, set to 1 to ensure the ticker always ticks at least once per second.
                int ticksPerSecond = maxTicksPerSecond <= 0 ? 1 : maxTicksPerSecond;
                // Calculate the minimum wait time between each tick based on the desired ticks per second.
                float minWaitTime = 1f / ticksPerSecond;

                float startTime = Time.realtimeSinceStartup;
                Tick();
                float elapsedTime = Time.realtimeSinceStartup - startTime;
                float waitTime = Mathf.Max(minWaitTime - elapsedTime, 0f);
                yield return new WaitForSecondsRealtime(waitTime);
            }

            if (tickerCoroutine != null)
                Coroutiner.Stop(tickerCoroutine);

            tickerCoroutine = null;
        }


        /// <summary>
        /// Ticker that ticks every X frames
        /// </summary>
        /// <returns> IEnumerator used to start the coroutine </returns>
        private IEnumerator FrameBasedTicker()
        {
            int frameCount = 0;
            while (isRunning)
            {
                yield return null;
                frameCount++;
                if (frameCount < frames) continue;
                frameCount = 0;
                Tick();
            }
            
            if (tickerCoroutine != null)
                Coroutiner.Stop(tickerCoroutine);

            tickerCoroutine = null;
        }

        /// <summary>
        /// Ticker that ticks every X seconds
        /// </summary>
        /// <returns> IEnumerator used to start the coroutine </returns>
        private IEnumerator TimeBasedTicker()
        {
            float time = 0;
            while (isRunning)
            {
                yield return null;
                time += Time.unscaledDeltaTime;
                if (time < seconds) continue;
                time = 0;
                Tick();
            }
            
            if (tickerCoroutine != null)
                Coroutiner.Stop(tickerCoroutine);

            tickerCoroutine = null;
        }

        /// <summary>
        /// Get number of frames to wait before ticking for FrameBased mode
        /// </summary>
        /// <returns> Number of frames to wait before ticking </returns>
        /// <param name="value"> Frame interval </param>
        /// <param name="currentValue"> Current value of the Frames variable </param>
        public static int GetFrameInterval(FrameIntervals value, int currentValue)
        {
            switch (value)
            {
                case FrameIntervals.EveryFrame:
                    return 1;
                case FrameIntervals.EveryTwoFrames:
                    return 2;
                case FrameIntervals.EveryThreeFrames:
                    return 3;
                case FrameIntervals.EveryFourFrames:
                    return 4;
                case FrameIntervals.EveryFiveFrames:
                    return 5;
                case FrameIntervals.Custom:
                    return currentValue;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Get time interval to wait before ticking for TimeBased mode
        /// </summary>
        /// <returns> Time interval to wait before ticking </returns>
        /// <param name="value"> Time interval </param>
        /// <param name="currentValue"> Current value of the Seconds variable </param>
        public static float GetTimeInterval(TimeIntervals value, float currentValue)
        {
            switch (value)
            {
                case TimeIntervals.OncePerSecond:
                    return 1;
                case TimeIntervals.TwicePerSecond:
                    return 0.5f;
                case TimeIntervals.FiveTimesPerSecond:
                    return 0.2f;
                case TimeIntervals.TenTimesPerSecond:
                    return 0.1f;
                case TimeIntervals.TwentyTimesPerSecond:
                    return 0.05f;
                case TimeIntervals.ThirtyTimesPerSecond:
                    return 0.033f;
                case TimeIntervals.SixtyTimesPerSecond:
                    return 0.016f;
                case TimeIntervals.Custom:
                    return currentValue;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Get the IEnumerator that will be used to tick based on the TickerMode
        /// </summary>
        /// <returns> IEnumerator that will be used to tick </returns>
        private IEnumerator GetIEnumerator()
        {
            switch (tickerMode)
            {
                case Mode.RealTime:
                    return RealTimeTicker();
                case Mode.FrameBased:
                    return FrameBasedTicker();
                case Mode.TimeBased:
                    return TimeBasedTicker();
                default:
                    return RealTimeTicker();
            }
        }
    }
}
