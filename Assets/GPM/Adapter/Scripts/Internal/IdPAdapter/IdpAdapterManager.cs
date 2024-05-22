using Gpm.Common.Indicator;
using System;
using System.Collections.Generic;

namespace Gpm.Adapter.Internal
{
    public class IdPAdapterManager
    {
        private static readonly IdPAdapterManager instance = new IdPAdapterManager();

        public static IdPAdapterManager Instance
        {
            get { return instance; }
        }

        private string Domain
        {
            get { return typeof(IdPAdapterManager).Name; }
        }

        public IIdPAdapter adapter;
        private Dictionary<string, IIdPAdapter> adapterDict = new Dictionary<string, IIdPAdapter>();
        private Dictionary<string, bool> indicatorDictionary;

        private bool CreateIdPAdapter(string idPName)
        {
            if (HasAdapter(idPName) == true)
            {
                return true;
            }

            string adapterName = string.Format("{0}adapter", idPName);
            adapter = AdapterFactory.CreateAdapter<IIdPAdapter>(adapterName);

            if (adapter == null)
            {
                return false;
            }

            AddAdapter(idPName, adapter);
            return true;
        }
        
        public void Login(string idPName, Dictionary<string, object> additionalInfo, Action<AdapterError> callback)
        {
            if (HasAdapter(idPName) == false)
            {
                if (CreateIdPAdapter(idPName) == false)
                {
                    callback(new AdapterError(AdapterErrorCode.ADAPTER_NOT_FOUND, Domain));
                    return;
                }
            }

            var adapter = GetAdapter(idPName);
            SendIndicator("Login", idPName);

            adapter.Login(additionalInfo, (error) => 
            {
                if (GpmAdapter.IsSuccess(error) == true)
                {
                    callback(error);
                }
                else
                {
                    var adapterError = new AdapterError(AdapterErrorCode.EXTERNAL_LIBRARY_ERROR, Domain, error: error);
                    callback(adapterError);
                }
            });
        }

        public void Logout(string idPName, Action<AdapterError> callback)
        {
            if (HasAdapter(idPName) == false)
            {
                callback(new AdapterError(AdapterErrorCode.NOT_LOGGED_IN, Domain));
                return;
            }

            GetAdapter(idPName).Logout((error)=> 
            {
                if (GpmAdapter.IsSuccess(error) == true)
                {
                    RemoveAdapter(idPName);
                    callback(error);
                }
                else
                {
                    var adapterError = new AdapterError(AdapterErrorCode.EXTERNAL_LIBRARY_ERROR, Domain, error: error);
                    callback(adapterError);
                }
            });
        }

        public void LogoutAll(Action<AdapterError> callback)
        {
            if (adapterDict.Count == 0)
            {
                callback(new AdapterError(AdapterErrorCode.NOT_LOGGED_IN, Domain));
                return;
            }

            bool isSuccess = true;

            foreach (KeyValuePair<string, IIdPAdapter> kvp in adapterDict)
            {
                kvp.Value.Logout((error) => 
                {
                    if (GpmAdapter.IsSuccess(error) == false)
                    {
                        isSuccess = false;
                        callback(error);
                        return;
                    }
                });
            }

            if (isSuccess == true)
            {
                callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
            }
        }

        public void GetAuthInfo(string idPName, Action<string> callback)
        {
            if (HasAdapter(idPName) == false)
            {
                LoggerMapper.Warn(AdapterStrings.NOT_LOGGED_IN, GetType());
                callback(string.Empty);
                return;
            }

            GetAdapter(idPName).GetAuthInfo(callback);
        }

        public void GetProfile(string idPName, Action<Dictionary<string, object>> callback)
        {
            if (HasAdapter(idPName) == false)
            {
                LoggerMapper.Warn(AdapterStrings.NOT_LOGGED_IN, GetType());
                callback(null);
                return;
            }

            SendIndicator("GetProfile", idPName);

            GetAdapter(idPName).GetProfile(callback);
        }

        public List<string> GetLoggedInIdPList()
        {
            var loggedInList = new List<string>();

            foreach (KeyValuePair<string, IIdPAdapter> kvp in adapterDict)
            {
                if (kvp.Value.IsLoggedIn() == true)
                {
                    loggedInList.Add(kvp.Key);
                }
            }

            return loggedInList;
        }

        public string GetUserId(string idPName)
        {
            return GetIdPData<string>(idPName, SyncMethodName.GET_USER_ID);
        }

        private void AddAdapter(string idPName, IIdPAdapter adapter)
        {
            adapterDict.Add(idPName, adapter);
        }

        private void RemoveAdapter(string idPName)
        {
            if (adapterDict.ContainsKey(idPName) == true)
            {
                adapterDict.Remove(idPName);
            }
        }

        private IIdPAdapter GetAdapter(string idPName)
        {
            return adapterDict[idPName];
        }

        private bool HasAdapter(string idPName)
        {
            return adapterDict.ContainsKey(idPName);
        }

        private enum SyncMethodName
        {
            GET_USER_ID
        }

        private T GetIdPData<T>(string idPName, SyncMethodName methodName)
        {
            if (HasAdapter(idPName) == false)
            {
                LoggerMapper.Warn(AdapterStrings.NOT_LOGGED_IN, GetType(), string.Format("GetIdPData({0})", methodName));
                return default(T);
            }

            switch (methodName)
            {
                case SyncMethodName.GET_USER_ID:
                    {
                        SendIndicator("GetUserId", idPName);

                        return (T)Convert.ChangeType(GetAdapter(idPName).GetUserId(), typeof(T));
                    }
                default:
                    {
                        return default(T);
                    }
            }
        }

        private void SendIndicator(string action, string idPName)
        {
            if (indicatorDictionary == null)
            {
                indicatorDictionary = new Dictionary<string, bool>();
            }

            bool isSent = false;
            if (indicatorDictionary.TryGetValue(string.Format("{0}{1}", action, idPName), out isSent) == true)
            {
                if (isSent == true)
                {
                    return;
                }
            }

            var indicatorData = new GpmIndicatorData(action);
            indicatorData.AddActionDetail(idPName);
            indicatorData.AddActionDetail(adapter.GetIdPSdkVersion());
            GpmIndicator.Send(GpmAdapter.SERVICE_NAME, GpmAdapter.VERSION, indicatorData.ToDictionary());

            indicatorDictionary.Add(string.Format("{0}{1}", action, idPName), true);
        }
    }
}