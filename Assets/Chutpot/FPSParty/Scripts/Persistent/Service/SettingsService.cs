using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

namespace Chutpot.FPSParty.Persistent
{
    public class SettingsService : AbstractWriterService, ISettingsService
    {
        [Inject]
        public SettingsSignal SettingsSignal { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }
        [Inject]
        public InputModel InputModel { get; set; }
        [Inject]
        public CameraService CameraService { get; set; }

        private const string _fileName = "settings.json";

        private readonly string _settingsFile;

        public SettingsService() : base()
        {
            _settingsFile = _fileLocation + _fileName;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!LocalizationSettings.InitializeSynchronously)
            {
                LocalizationSettings.InitializationOperation.WaitForCompletion();
            }
        }

        protected override void InitializeFile()
        {
            if (!Directory.Exists(_fileLocation)) 
            {
                Directory.CreateDirectory(_fileLocation);
            }

            using (FileStream fs = new FileStream(_settingsFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        Settings settings = JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd());
                        SettingsModel.Settings = settings;
                        ApplyVideoSettings(new Video(Screen.currentResolution, Screen.fullScreenMode, true, false));
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[settings.LanguageIndex];
                        sr.Dispose();
                    }
                }
                catch (JsonSerializationException je)
                {
                    WriteDefault();
                    Debug.Log(je);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.LogException(ex);
                }
                catch (FileNotFoundException ex)
                {
                    Debug.LogException(ex);
                }
                catch (IOException ex)
                {
                    Debug.LogException(ex);
                }
                catch (NullReferenceException ex)
                {
                    WriteDefault();
                    Debug.Log(ex);
                }
                catch (Exception ex)
                {
                    WriteDefault();
                    Debug.LogException(ex);
                }
                fs.Dispose();
            }
        }

        public override void Write()
        {
            try
            {
                using (StreamWriter sw = File.CreateText(_settingsFile))
                {
                    string str = JsonConvert.SerializeObject(SettingsModel.Settings);
                    sw.Write(str);
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        protected override void WriteDefault()
        {
            try
            {
                using (StreamWriter sw = File.CreateText(_settingsFile))
                {
                    VideoData videoData = new VideoData(true, false);
                    Audio audio = new Audio(10.0f, 10.0f, 10.0f);
                    Settings settings = new Settings(videoData, audio, 0, null);
                    ApplyVideoSettings(new Video(Screen.currentResolution, Screen.fullScreenMode, videoData.Vsync, videoData.LimitFrameRate));

                    string str = JsonConvert.SerializeObject(settings);
                    SettingsModel.Settings = settings;
                    sw.Write(str);
                    sw.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void SetVideo(Video video)
        {
            var videoData = SettingsModel.Settings.VideoData;
            videoData.LimitFrameRate = video.LimitFrameRate;
            videoData.Vsync = video.Vsync;

            SettingsModel.Settings.VideoData = videoData;
            ApplyVideoSettings(video);
            SettingsSignal.Dispatch(SettingsModel.Settings);
        }

        public void SetAudio(Audio audio)
        {
            SettingsModel.Settings.Audio = audio;
            SettingsSignal.Dispatch(SettingsModel.Settings);
        }

        public void SetLocalization(int index)
        {
            SettingsModel.Settings.LanguageIndex = index;
            SettingsSignal.Dispatch(SettingsModel.Settings);
        }

        private void ApplyVideoSettings(Video video) 
        {
            Screen.SetResolution(video.Resolution.width, video.Resolution.height, video.ScreenMode, video.Resolution.refreshRate);

            if (video.LimitFrameRate) 
            {
                QualitySettings.SetQualityLevel(0);
                Application.targetFrameRate = video.Resolution.refreshRate;
            }
            else 
            {
                if (video.Vsync)
                {
                    QualitySettings.vSyncCount = 1;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                }
            }

        }

#if UNITY_EDITOR
        public static void EditorClearSettings()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ChutPot\\Project2D\\" + _fileName))
                {
                    VideoData videoData = new VideoData(true, false);
                    Audio audio = new Audio(10.0f, 10.0f, 10.0f);
                    Settings settings = new Settings(videoData, audio, 0, null);

                    string str = JsonConvert.SerializeObject(settings);
                    sw.Write(str);
                    sw.Dispose();
                }
            }
            else
            {
                Debug.LogWarning("Can't Clear while playing");
            }
        }
#endif
    }
}
