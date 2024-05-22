#if GPM_USE_GPGS
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections.Generic;
using Gpm.Adapter.Internal;
using Gpm.Common;
using UnityEngine;

namespace Gpm.Adapter.IdP
{
    public sealed class GpgsAdapter : AdapterBase, IIdPAdapter
    {
        public const string VERSION = "1.0.0";

        private const string KEY_PROFILE_ID = "id";
        private const string KEY_PROFILE_NAME = "name";
        private const string KEY_PROFILE_EMAIL = "email";

        protected override string Domain
        {
            get { return typeof(GpgsAdapter).Name; }
        }

        protected override string Version
        {
            get { return VERSION; }
        }

        public GpgsAdapter()
        {
            InitializeGpgs();
        }

        public string GetIdPSdkVersion()
        {
            return PluginVersion.VersionString;
        }

        public void Login(Dictionary<string, object> additionalInfo, Action<AdapterError> callback)
        {
            if (IsLoggedIn() == true)
            {
                LoggerMapper.Debug(AdapterStrings.ALREADY_LOGGED_IN, GetType());
                callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
                return;
            }

            Social.localUser.Authenticate((bool success, string error) =>
            {
                if (success == true)
                {
                    callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
                }
                else
                {
                    callback(new AdapterError(AdapterErrorCode.EXTERNAL_LIBRARY_ERROR, Domain, error));
                }
            });
        }

        public void Logout(Action<AdapterError> callback)
        {
            if (IsLoggedIn() == true)
            {
                PlayGamesPlatform.Instance.SignOut();
                callback(new AdapterError(AdapterErrorCode.SUCCESS, Domain));
            }
            else
            {
                callback(new AdapterError(AdapterErrorCode.NOT_LOGGED_IN, Domain));
            }
        }

        public void GetAuthInfo(Action<string> callback)
        {
            if (IsLoggedIn() == true)
            {
                GetAnotherServerAuthCode(callback);
            }
            else
            {
                GetServerAuthCode(callback);
            }
        }

        public void GetProfile(Action<Dictionary<string, object>> callback)
        {
            var profile = new Dictionary<string, object>();
            profile.Add(KEY_PROFILE_ID, ((PlayGamesLocalUser)Social.localUser).id);
            profile.Add(KEY_PROFILE_NAME, ((PlayGamesLocalUser)Social.localUser).userName);
            profile.Add(KEY_PROFILE_EMAIL, ((PlayGamesLocalUser)Social.localUser).Email);

            callback(profile);
        }

        public bool IsLoggedIn()
        {
            return PlayGamesPlatform.Instance.IsAuthenticated();
        }

        public string GetUserId()
        {
            return ((PlayGamesLocalUser)Social.localUser).id;
        }

        /// <summary>
        /// For setting of PlayGamesClientConfiguration, refer to the guide as below.
        /// <see href="https://github.com/playgameservices/play-games-plugin-for-unity#configuration--initialization-play-game-services">Configuration & Initialization Play Game Services</see>
        /// </summary>
        private void InitializeGpgs()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .WithInvitationDelegate(InvitationReceivedDelegate)
                .WithMatchDelegate(MatchDelegate)
                .RequestEmail()
                .RequestServerAuthCode(false)
                .RequestIdToken()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = GpmCommon.DebugLogEnabled;
            PlayGamesPlatform.Activate();
        }

        /// <summary>
        /// This is called when an invitation is received.
        /// </summary>
        private void InvitationReceivedDelegate(Invitation invitation, bool shouldAutoAccept)
        {
            LoggerMapper.Debug(string.Format("invitation info:{0}", invitation), GetType());
        }

        /// <summary>
        /// This is called when a match notification
        /// </summary>
        private void MatchDelegate(TurnBasedMatch match, bool shouldAutoLaunch)
        {
            LoggerMapper.Debug(string.Format("match info:{0}", match), GetType());
        }

        private void GetServerAuthCode(Action<string> callback)
        {
            string authCode = PlayGamesPlatform.Instance.GetServerAuthCode();

            if (string.IsNullOrEmpty(authCode) == true)
            {
                LoggerMapper.Warn(AdapterStrings.GOOGLE_SERVER_AUTH_CODE_EMPTY, GetType());
            }

            callback(authCode);
        }

        private void GetAnotherServerAuthCode(Action<string> callback)
        {
            PlayGamesPlatform.Instance.GetAnotherServerAuthCode(false, (authCode =>
            {
                if (string.IsNullOrEmpty(authCode) == true)
                {
                    LoggerMapper.Warn(AdapterStrings.GOOGLE_SERVER_AUTH_CODE_EMPTY, GetType());
                }

                callback(authCode);
            }));
        }
    }
}

#endif
#endif

