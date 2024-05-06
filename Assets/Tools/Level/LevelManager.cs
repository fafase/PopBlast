using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;


namespace Tools
{
    public class LevelManager : ILevelManager, IInitializable
    {
        public List<Level> Levels {  get; private set; }   
        public event Action OnInitDone;

        public async void Initialize()
        {
            string path = Application.streamingAssetsPath + "/Levels/bundle_0.zip";
            string target = $"{Application.persistentDataPath}/Levels/";
            if (!Directory.Exists(target)) 
            {
                ZipFile.ExtractToDirectory(path, target);               
            }
            Levels = new List<Level>();
            DirectoryInfo di = new DirectoryInfo(target);
            foreach (FileInfo dir in new DirectoryInfo(target).GetFiles())
            {
                string file =  await File.ReadAllTextAsync(dir.FullName);
                Level level = JsonConvert.DeserializeObject<Level>(file);
                Levels.Add(level);
            }
            OnInitDone?.Invoke();
        }
    }

    [Serializable]
    public class Level 
    {
        public int level;
        public int difficulty;
    }
    public interface ILevelManager 
    {
        List<Level> Levels { get; }
        event Action OnInitDone;
    }
}
