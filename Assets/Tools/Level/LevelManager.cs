using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
using System.Linq;
using UnityEditor;


namespace Tools
{
    public class LevelManager : ILevelManager, IInitializable
    {
        [Inject] IPlayerData m_playerData;
        public List<Level> Levels {  get; private set; }   
        public event Action OnInitDone;
        public Level CurrentLevel => GetLevel(m_playerData.CurrentLevel);
        private static string LEVEL_PATH = Application.streamingAssetsPath + "/Levels/";
        private static string PERSISTENT_STORAGE = $"{Application.persistentDataPath}/Levels/";
        public async void Initialize()
        {
            Levels = new List<Level>();
            foreach (string levelBundle in Directory.GetFiles(LEVEL_PATH, "*.zip"))
            {
                string fileName = Path.GetFileName(levelBundle);
                //string directoryTarget = $"{PERSISTENT_STORAGE}{Path.GetFileNameWithoutExtension(fileName)}";
                if (!Directory.Exists(PERSISTENT_STORAGE))
                {
                    ZipFile.ExtractToDirectory(levelBundle, PERSISTENT_STORAGE);
                }

                List<Level> tempLevels = new List<Level>();
                string directoryTarget = $"{PERSISTENT_STORAGE}{Path.GetFileNameWithoutExtension(fileName)}";
                foreach (FileInfo dir in new DirectoryInfo(directoryTarget).GetFiles())
                {
                    string file = await File.ReadAllTextAsync(dir.FullName);
                    Level level = JsonConvert.DeserializeObject<Level>(file);
                    tempLevels.Add(level);
                }
                var ordered = tempLevels.OrderBy((lvl) => lvl.level).ToList();
                Levels.AddRange(ordered);
            }
            OnInitDone?.Invoke();
        }
        public Level GetLevel(int level) 
        {
            level -= 1;
            if(Levels == null || Levels.Count == 0) 
            {
                throw new Exception("[LevelManager] Missing levels in LevelManager");
            }
            if (level < 0 || level >= Levels.Count)
            {
                throw new Exception("[LevelManager] Attempt to get a Level with wrong index");
            }
            return Levels[level];
        }

        [MenuItem("Tools/Delete Levels")]
        public static void DeleteLevels() 
        {
            if (Directory.Exists(PERSISTENT_STORAGE)) 
            {
                Debug.Log("Deleting levels");
                Directory.Delete(PERSISTENT_STORAGE, true);
            }
        }
    }

    [Serializable]
    public class Level 
    {
        public int level;
        public int difficulty;
        public int[] gridSize;
        public int moves;
        public int items;
        public int Column => gridSize[0];
        public int Row => gridSize[1];
    }
    public interface ILevelManager 
    {
        List<Level> Levels { get; }
        event Action OnInitDone;
        Level CurrentLevel { get; }
        Level GetLevel(int level);
    }
}
