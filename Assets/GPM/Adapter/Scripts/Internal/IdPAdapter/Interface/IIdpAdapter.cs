using System;
using System.Collections.Generic;

namespace Gpm.Adapter.Internal
{
    public interface IIdPAdapter
    {
        void Login(Dictionary<string, object> additionalInfo, Action<AdapterError> callback);
        void Logout(Action<AdapterError> callback);
        void GetAuthInfo(Action<string> callback);
        void GetProfile(Action<Dictionary<string, object>> callback);
        bool IsLoggedIn();
        string GetUserId();
        string GetIdPSdkVersion();
    }
}