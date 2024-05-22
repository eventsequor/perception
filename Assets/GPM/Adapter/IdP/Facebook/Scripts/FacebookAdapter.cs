#if GPM_USE_FACEBOOK

using Facebook.Unity;
using System;
using System.Collections.Generic;
using Gpm.Adapter.Internal;
using Gpm.Common.Util;

namespace Gpm.Adapter.IdP
{
    public sealed class FacebookAdapter : AdapterBase, IIdPAdapter
    {
        public const string VERSION = "1.0.0";
        public const string KEY_FACEBOOK_PERMISSIONS = "facebook_permissions";

        protected override string Domain
        {
            get { return typeof(FacebookAdapter).Name; }
        }

        protected override string Version
        {
            get { return VERSION; }
        }

        public string GetIdPSdkVersion()
        {
            return FacebookSdkVersion.Build;
        }

        public void Login(Dictionary<string, object> additionalInfo, Action<AdapterError> callback)
        {
            if (IsLoggedIn() == true)
            {
                LoggerMapper.Debug(AdapterStrings.ALREADY_LOGGED_IN, GetType());
                callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
                return;
            }

            InitializeFacebook(() =>
            {
                FB.LogInWithReadPermissions(GetFBPermissions(additionalInfo), (fbResult) =>
                {
                    if (fbResult.Cancelled == true)
                    {
                        callback(new AdapterError(AdapterErrorCode.USER_CANCELED, Domain, fbResult.Error));
                        return;
                    }

                    if (string.IsNullOrEmpty(fbResult.Error) == true)
                    {
                        callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
                    }
                    else
                    {
                        callback(new AdapterError(AdapterErrorCode.EXTERNAL_LIBRARY_ERROR, Domain, fbResult.Error));
                    }
                });
            });
        }

        public void Logout(Action<AdapterError> callback)
        {
            if (IsLoggedIn() == true)
            {
                FB.LogOut();
                callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
            }
            else
            {
                callback(new AdapterError(AdapterErrorCode.NOT_LOGGED_IN, Domain));
            }
        }

        public void GetAuthInfo(Action<string> callback)
        {
            string accessToken = AccessToken.CurrentAccessToken.TokenString;

            if (string.IsNullOrEmpty(accessToken) == true)
            {
                LoggerMapper.Warn(AdapterStrings.FACEBOOK_ACCESS_TOKEN_IS_EMPTY, GetType());
            }

            callback(accessToken);
        }

        public void GetProfile(Action<Dictionary<string, object>> callback)
        {
            FB.API("me?fields=id,name,email", HttpMethod.GET, (graphResult) =>
            {
                Dictionary<string, object> profile = null;

                if (string.IsNullOrEmpty(graphResult.Error) == true)
                {
                    profile = GpmJsonMapper.ToObject<Dictionary<string, object>>(graphResult.RawResult);
                }
                else
                {
                    LoggerMapper.Warn(string.Format("{0} error:{1}", AdapterStrings.FAILED_TO_LOAD_FACEBOOK_PROFILE, graphResult.Error), GetType());
                }

                callback(profile);
            });
        }

        public bool IsLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public string GetUserId()
        {
            return AccessToken.CurrentAccessToken.UserId;
        }

        private void InitializeFacebook(Action onInitCompleted)
        {
            if (FB.IsInitialized)
            {
                onInitCompleted();
                return;
            }

            FB.Init(() =>
            {
                onInitCompleted();
            });
        }

        private List<string> GetFBPermissions(Dictionary<string, object> additionalInfo)
        {
            var permissions = new List<string> { "public_profile", "email" };

            if (ValidatePermissions(additionalInfo) == true)
            {
                permissions = (List<string>)additionalInfo[KEY_FACEBOOK_PERMISSIONS];
            }
            else
            {
                LoggerMapper.Debug(AdapterStrings.SET_DEFAULT_FACEBOOK_PERMISSIONS, GetType());
                return permissions;
            }

            return permissions;
        }

        private bool ValidatePermissions(Dictionary<string, object> additionalInfo)
        {
            if (additionalInfo == null || additionalInfo.ContainsKey(KEY_FACEBOOK_PERMISSIONS) == false)
            {
                LoggerMapper.Debug(AdapterStrings.FACEBOOK_PERMISSIONS_NOT_FOUND, GetType());
                return false;
            }

            if ((additionalInfo[KEY_FACEBOOK_PERMISSIONS] is List<string>) == false)
            {
                LoggerMapper.Debug(AdapterStrings.FACEBOOK_PERMISSIONS_INVALID_TYPE, GetType());
                return false;
            }

            var permissions = (List<string>)additionalInfo[KEY_FACEBOOK_PERMISSIONS];

            if (permissions.Count == 0)
            {
                LoggerMapper.Debug(AdapterStrings.FACEBOOK_PERMISSIONS_IS_EMPTY, GetType());
                return false;
            }

            return true;
        }
    }
}

#endif





