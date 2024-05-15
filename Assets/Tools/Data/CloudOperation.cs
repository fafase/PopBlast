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
    public class CloudOperation : ICloudOperation, IInitializable, IDisposable
    {
        [Inject] IUserPrefs m_userPrefs;

        private List<Operation> m_operations;
        private const string PLAYER_DATA = "playerData";
        private const string SERIALIZE_OPERATION = "serializeOperation";

        private bool m_isDisposed = false;
        private int m_processedIndex = 0;
        public List<Operation> Operations => m_operations;
#if UNITY_INCLUDE_TESTS
        public CloudOperation(IUserPrefs userPrefs) => m_userPrefs = userPrefs;
#endif
        public void Initialize()
        {
            string serializedOperation = PlayerPrefs.GetString(SERIALIZE_OPERATION, "");
            m_operations = string.IsNullOrEmpty(serializedOperation) ? new List<Operation>() : JsonConvert.DeserializeObject<List<Operation>>(serializedOperation);
            m_userPrefs.OnUpdate += () => AddOperation(OperationType.UserPrefsUpdate);

            //Signal.Connect<LoginSignalData>((data) => 
            //{
            //    if (data.State == ServicesInitializationState.Initialized)
            //    {
            //        //FlushOperations(null).Forget();
            //    }
            //});
        }

        public void AddOperation(OperationType type) => AddOperation(new Operation(type));
 
        public void AddOperation(Operation newOperation) 
        {
            switch (newOperation.type)
            {
                case OperationType.UserPrefsUpdate:
                    // No need to keep many calls
                    m_operations.RemoveAll(op => op.type.Equals(newOperation.type));
                    m_operations.Add(newOperation);
                    break;
            }
        }
        public async UniTask FlushOperations(Action<Dictionary<string, string>> cloudSaveCallback) 
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
                        if(Application.internetReachability == NetworkReachability.NotReachable) 
                        {
                            return;
                        }
                        try
                        {
                            if(UnityServices.State == ServicesInitializationState.Uninitialized) 
                            {
                                await UnityServices.InitializeAsync();
                            }
                            Dictionary<string, string> data = await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                                {
                                    { PLAYER_DATA, m_userPrefs.Json }
                                });
                            m_userPrefs.IsDirty = false;
                            Signal.Send(new FlushOperation(data));
                            cloudSaveCallback?.Invoke(data);
                        }
                        catch (Exception e) 
                        {
                            Debug.LogError(e.Message);
                            // Something went wrong so we return and will try later. Operation is still in the list.
                            return;
                        }
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
            m_operations = m_operations.GetRange(m_processedIndex, m_operations.Count);
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
        void AddOperation(Operation newOperation);
        UniTask FlushOperations(Action<Dictionary<string, string>> cloudSaveCallback);
        List<Operation> Operations { get; }
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
