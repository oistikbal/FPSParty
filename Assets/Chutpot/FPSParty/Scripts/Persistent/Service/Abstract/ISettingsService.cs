using System;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public interface ISettingsService
    {
        public void Write();
        public void SetVideo(Video video);
        public void SetAudio(Audio audio);
        public void SetLocalization(int index);
    }

    [Serializable]
    public struct Video : ICloneable
    {
        public Resolution Resolution;
        public FullScreenMode ScreenMode;
        public bool Vsync;
        public bool LimitFrameRate;

        public Video(Resolution resolution, FullScreenMode screenMode, bool vsync, bool limitFrameRate)
        {
            Resolution = resolution;
            ScreenMode = screenMode;
            LimitFrameRate = limitFrameRate;
            Vsync = vsync;
        }

        public object Clone()
        {
            Video video = new Video(Resolution, ScreenMode, Vsync, LimitFrameRate);
            return video;
        }
    }

    [Serializable]
    public struct VideoData : ICloneable
    {
        public bool Vsync;
        public bool LimitFrameRate;

        public VideoData(bool vsync, bool limitFrameRate)
        {
            LimitFrameRate = limitFrameRate;
            Vsync = vsync;
        }

        public object Clone()
        {
            VideoData video = new VideoData(Vsync, LimitFrameRate);
            return video;
        }
    }

    [Serializable]
    public struct Audio : ICloneable
    {
        public float MasterAudio;
        public float GameAudio;
        public float MusicAudio;

        public Audio(float masterAudio, float gameAudio, float musicAudio)
        {
            MasterAudio = masterAudio;
            GameAudio = gameAudio;
            MusicAudio = musicAudio;
        }

        public object Clone()
        {
            Audio audio = new Audio(MasterAudio, GameAudio, MusicAudio);
            return audio;
        }
    }


    [Serializable]
    public struct Settings : ICloneable
    {
        public VideoData VideoData;
        public Audio Audio;
        public int LanguageIndex;
        public string Input;

        public Settings(VideoData videoData, Audio audio, int languageIndex, string input)
        {
            VideoData = videoData;
            Audio = audio;
            LanguageIndex = languageIndex;
            Input = input;
        }

        public object Clone()
        {
            Settings settings = new Settings(VideoData, Audio,LanguageIndex ,Input);
            return settings;
        }
    }
}
