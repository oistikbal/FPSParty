using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using System.Text;

namespace Chutpot.Project2D.Persistent
{
    [Serializable]
    public struct Game : ICloneable
    {
        public int ChapterIndex;
        public int ChapterCheckpoint;

        public Game(int chapterIndex, int chapterCheckpoint)
        {
            ChapterIndex = chapterIndex;
            ChapterCheckpoint = chapterCheckpoint;
        }

        public object Clone()
        {
            Game game = new Game(ChapterIndex, ChapterCheckpoint);
            return game;
        }
    }

    public enum SelectGame 
    {
        First = 0,
        Second = 1,
        Third = 2
    }

    public class GameSettingsService : AbstractWriterService
    {
        [Inject]
        public GameSettingsModel GameSettingsModel { get; set; }

        private SelectGame _selectGame;

        public Game[] Games { get; protected set; }

        private readonly string _gameFile;

        private const string _fileName = "game.dat";

        public GameSettingsService() : base() 
        {
            _gameFile = _fileLocation + _fileName;
            PopulateGames();
        }

        public override void Initialize() 
        {
            base.Initialize();
        }

        protected override void InitializeFile() 
        {
            if (!Directory.Exists(_fileLocation))
            {
                Directory.CreateDirectory(_fileLocation);
            }

            using (FileStream fs = new FileStream(_gameFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        var decrypt = DecryptStringFromBytes_Aes(sr.ReadToEnd());
                        sr.Dispose();

                        Games = JsonConvert.DeserializeObject<Game[]>(decrypt);
                        if (Games == null) 
                        {
                            WriteDefault();
                        }
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
                using (StreamWriter sw = File.CreateText(_gameFile))
                {
                    string str = JsonConvert.SerializeObject(Games);
                    var bytes = EncryptStringToBytes_Aes(str);
                    sw.Write(Convert.ToBase64String(bytes));
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
                PopulateGames();
                using (StreamWriter sw = File.CreateText(_gameFile))
                {
                    string str = JsonConvert.SerializeObject(Games);
                    var bytes = EncryptStringToBytes_Aes(str);
                    sw.Write(Convert.ToBase64String(bytes));
                    sw.Dispose();
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public Game SelectGame(SelectGame selectGame)
        {
            _selectGame = selectGame;
            GameSettingsModel.Game = Games[(int)selectGame];
            return GameSettingsModel.Game;
        }

        public void SetGame(Game game)
        {
            Games[(int)_selectGame] = game;
        }

        private void PopulateGames() 
        {
            //if Games read null from json, it'll be null, need to be initialized for each populating
            Games = new Game[3];
            for (int i = 0; i < Games.Length; i++)
            {
                Games[i] = new Game(0, 0);
            }
        }

#if UNITY_EDITOR
        public static void EditorSaveGames(Game[] games)
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ChutPot\\Project2D\\" + _fileName))
                    {

                        string str = JsonConvert.SerializeObject(games);
                        var bytes = EncryptStringToBytes_Aes(str);
                        sw.Write(Convert.ToBase64String(bytes));
                        sw.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            else
            {
                Debug.LogWarning("Can't Clear while playing");
            }
        }

        public static Game[] EditorLoadGames()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ChutPot\\Project2D\\" + _fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            var decrypt = DecryptStringFromBytes_Aes(sr.ReadToEnd());
                            sr.Dispose();

                            var games = JsonConvert.DeserializeObject<Game[]>(decrypt);
                            if (games == null)
                            {
                                EditorSaveGames(new Game[3]);
                            }
                            return games;
                        }
                    }
                    catch (JsonSerializationException je)
                    {
                        Debug.Log(je);
                        return null;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Can't Load while playing");
                return null;
            }
        }
#endif
    }
}
