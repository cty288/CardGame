using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Utilities;
using UnityEngine;

namespace MainGame
{
    /// <summary>
    /// For Non-reference type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ES3Serializable]
    public class SavedProperty<T> {
        [ES3Serializable]
        public T value;

        public SavedProperty() {
            value = default(T);
        }

        public SavedProperty(T value) {
            this.value = value;
        }
    }


    public interface ICanRegisterAndLoadSavedData : ICanGetSystem, IBelongToArchitecture {
        
    }

    public interface ICanSaveGame : ICanGetSystem, ICanSendEvent, IBelongToArchitecture {

    }
    public static class ICanSaveExtension {
        public static T RegisterAndLoadFromSavedData<T>(this ICanRegisterAndLoadSavedData self, string savedKey, T defaultValue,
            SaveType saveType = SaveType.ES3) where T : class
        {
            return self.GetSystem<ISaveSystem>().RegisterAndLoad(savedKey, defaultValue,saveType);
        }

        public static void SaveGame(this ICanSaveGame self) {
            self.SendEvent<ISaveEvent>(new SaveGame());
        }
    }

    public enum SaveType {
        ES3,
        Binary
    }

    public interface ISaveSystem : ISystem {
        void Save();
        T RegisterAndLoad<T>(string savedKey, T defaultValue, SaveType saveType) where T:class;
    }


    public struct SaveData {
        public object SavedObject;
        public SaveType SaveType;
    }

    public class SaveSystem : AbstractSystem, ISaveSystem {
        private Dictionary<string, SaveData> savedKeyValues = new Dictionary<string, SaveData>();

        //register events for save the game
        protected override void OnInit() {
            this.RegisterEvent<ISaveEvent>(OnGameSave);
        }

        private void OnGameSave(ISaveEvent e) {
            Save();
        }

        //save game; also triggered by events
        public void Save() {
            foreach (KeyValuePair<string, SaveData> savedKeyValue in savedKeyValues) {
                switch (savedKeyValue.Value.SaveType) {
                    case SaveType.ES3:
                        ES3.Save(savedKeyValue.Key, savedKeyValue.Value.SavedObject);
                        
                        break;
                    case SaveType.Binary:
                        BinaryFormatter bf = new BinaryFormatter();
                        FileStream saveFile = new FileStream(Application.persistentDataPath+ $"/{savedKeyValue.Key}.data",
                            FileMode.Create, FileAccess.Write);
                        bf.Serialize(saveFile, savedKeyValue.Value.SavedObject);
                        saveFile.Close();
                        break;
                }
                
            }
        }

        
        /// <summary>
        /// Only available for REFERENCE types. For Non-reference type, wrap it using SavedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="savedKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T RegisterAndLoad<T>(string savedKey, T defaultValue, SaveType saveType = SaveType.ES3) where T : class
        {
            T loadedObj = default(T);

            switch (saveType) {
                case SaveType.ES3:
                    loadedObj = ES3.Load<T>(savedKey, defaultValue);
                    break;
                case SaveType.Binary:
                   
                    if (File.Exists(Application.persistentDataPath + $"/{savedKey}.data)")) {
                        BinaryFormatter bf = new BinaryFormatter();
                        FileStream loadFile = new FileStream(Application.persistentDataPath + $"/{savedKey}.data", FileMode.Open, FileAccess.Read);

                        try
                        {
                            loadedObj = bf.Deserialize(loadFile) as T;
                        }
                        catch (Exception e)
                        {
                            loadedObj = defaultValue;
                        }
                    }
                    else {
                        loadedObj = defaultValue;
                    }
                    break;
            }
            

            if (savedKeyValues.ContainsKey(savedKey)) {
                savedKeyValues[savedKey] = new SaveData() {SaveType = saveType, SavedObject = loadedObj};
            }
            else {
                savedKeyValues.Add(savedKey, new SaveData() { SaveType = saveType, SavedObject = loadedObj });
            }
           
            return loadedObj;
        }
        
    }
}
