using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Shared;
using UnityEngine;

namespace DataHandling
{
    public class SaveDataManager : Singleton<SaveDataManager>
    {
        private const string SavePath = "/saveFile.save";

        public SaveData SaveData { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Load();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("Saved");
                Save();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                Clear();
            }
        }

        [ContextMenu("Save")]
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, SavePath), FileMode.Create,
                FileAccess.Write);
            formatter.Serialize(stream, SaveData);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            try
            {
                if (!File.Exists(string.Concat(Application.persistentDataPath, SavePath))) return;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, SavePath), FileMode.Open,
                    FileAccess.Read);
                SaveData = (SaveData)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Clear();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            SaveData = new SaveData();
        }
    }
}