using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Zenject;
using Newtonsoft.Json;
using UnityEngine;

namespace Tools
{
    public class CloudOperation :ICloudOperation, IInitializable, IDisposable
    {
        [Inject] IUserPrefs m_userPrefs;

        private List<Operation> m_operations;
        private const string PLAYER_DATA = "playerData";
        private const string SERIALIZE_OPERATION = "serializeOperation";

        private bool m_isDisposed = false;
        private int m_processedIndex = 0;
        public void Initialize()
        {
            string serializedOperation = PlayerPrefs.GetString(SERIALIZE_OPERATION, "");
            m_operations = string.IsNullOrEmpty(serializedOperation) ? new List<Operation>() : JsonConvert.DeserializeObject<List<Operation>>(serializedOperation);
            m_userPrefs.OnUpdate += () => AddOperation(OperationType.UserPrefsUpdate);
        }

        public void AddOperation(OperationType type) 
        {
            m_operations.Add(new Operation(type));
        }

        public async UniTask SendOperations() 
        {
            m_processedIndex = 0;
            foreach(Operation operation in m_operations) 
            {
                switch (operation.type) 
                {
                    case OperationType.UserPrefsUpdate:
                        if (!m_userPrefs.IsDirty)
                        {
                            return;
                        }
                        if (UnityServices.State != ServicesInitializationState.Initialized)
                        {
                            return;
                        }
                        await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { PLAYER_DATA, m_userPrefs.Json }
            });
                        m_userPrefs.IsDirty = false;
                        break;
                }
                ++m_processedIndex;
            }
            m_processedIndex = 0;
            m_operations.Clear();
        }

        public void Dispose()
        {
            if (m_isDisposed) 
            {
                return;
            }
            m_isDisposed = true;
            if (m_operations == null || m_operations.Count == 0) 
            {
                return;
            }
            m_operations = m_operations.GetRange(m_processedIndex, m_operations.Count - 1);
            string json = JsonConvert.SerializeObject(m_operations);
            PlayerPrefs.SetString(SERIALIZE_OPERATION, json);
            PlayerPrefs.Save();
            m_operations.Clear();
            m_operations = null;
        }
    }

    public interface ICloudOperation 
    {
        void AddOperation(OperationType newOperation);
        UniTask SendOperations();
    }
    public enum OperationType
    {
        UserPrefsUpdate
    }

    [Serializable]
    public class Operation 
    {
        public DateTime date;
        public OperationType type;
        public Operation(OperationType type)
        {
            this.type = type;
            date = DateTime.Now;
        }
    }
}   
