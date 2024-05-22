using Gpm.Adapter.Internal;
using Gpm.Common.Util;
using System;
using System.Collections.Generic;

namespace Gpm.Adapter
{
    public sealed class GpmAdapter
    {
        public const string VERSION = "2.0.1";
        public const string SERVICE_NAME = "Adapter";

        /// <summary>
        /// AdapterError 객체를 사용하여 성공 여부를 확인합니다.
        /// @since Added 1.0.0
        /// </summary>
        /// <param name="error">AdapterError 객체입니다.</param>
        /// <returns>성공 여부가 반환됩니다.</returns>
        /// <example>
        /// Example Usage :
        /// <code>
        /// private void SampleIsSucces(AdapterError error)
        /// {
        ///     if (GpmAdapter.IsSuccess(error) == true)
        ///     {
        ///         Debug.Log("success");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log(string.Format("failure. error:{0}", error));
        ///     }
        /// }
        /// </code>
        /// </example>
        public static bool IsSuccess(AdapterError error)
        {
            return (error == null || error.code == AdapterErrorCode.SUCCESS);
        }

        /// <summary>
        /// Facebook, Google과 같은 IdP의 인터페이스를 구현합니다.
        /// @since Added 1.0.0
        /// </summary>
        public static class IdP
        {
            /// <summary>
            /// IdP 로그인을 시도합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="idPName">IdP의 이름입니다.</param>
            /// <param name="additionalInfo">IdP 로그인을 위한 추가 정보입니다.</param>
            /// <param name="callback">API 호출에 대한 오류 정보가 반환되며 callback은 null을 입력할 수 없습니다.</param>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleLogin(string idPName)
            /// {
            ///     Dictionary&lt;string, object&gt; additionalInfo;
            ///     
            ///     switch (idPName)
            ///     {
            ///         case GpmAdapterType.IdP.FACEBOOK:
            ///         {
            ///             var facebookPermissionList = new List&lt;string&gt; { "public_profile", "email" };
            ///             additionalInfo = new Dictionary&lt;string, object&gt;();
            ///             additionalInfo.Add("facebook_permissions", facebookPermissionList);
            ///             break;
            ///         }
            ///         case GpmAdapterType.IdP.GPGS:
            ///         default:
            ///         {
            ///             additionalInfo = null;
            ///             break;
            ///         }
            ///     }
            ///  
            ///     GpmAdapter.IdP.Login(GpmAdapterType.IdP.FACEBOOK, additionalInfo, (error) => 
            ///     {
            ///         if (GpmAdapter.IsSuccess(error) == true)
            ///         {
            ///             Debug.Log("success");
            ///         }
            ///         else
            ///         {
            ///             Debug.Log(string.Format("failure. error:{0}", error));
            ///         }
            ///     });
            /// }
            /// </code>
            /// </example>
            public static void Login(string idPName, Dictionary<string, object> additionalInfo, Action<AdapterError> callback)
            {
                Miscellaneous.CheckNotNull(callback, "callback");
                IdPAdapterManager.Instance.Login(idPName, additionalInfo, callback);
            }

            /// <summary>
            /// IdP 로그아웃을 시도합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="idPName">IdP의 이름입니다.</param>
            /// <param name="callback">API 호출에 대한 오류 정보가 반환되며 callback은 null을 입력할 수 없습니다.</param>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleLogout()
            /// {
            ///     GpmAdapter.IdP.Logout(GpmAdapterType.IdP.FACEBOOK, (error) => 
            ///     {
            ///         if (GpmAdapter.IsSuccess(error) == true)
            ///         {
            ///             Debug.Log("success");
            ///         }
            ///         else
            ///         {
            ///             Debug.Log(string.Format("failure. error:{0}", error));
            ///         }
            ///     });
            /// }
            /// </code>
            /// </example>
            public static void Logout(string idPName, Action<AdapterError> callback)
            {
                Miscellaneous.CheckNotNull(callback, "callback");
                IdPAdapterManager.Instance.Logout(idPName, callback);
            }

            /// <summary>
            /// 모든 IdP 로그아웃을 시도합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="callback">API 호출에 대한 오류 정보가 반환되며 callback은 null을 입력할 수 없습니다.</param>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleLogoutAll()
            /// {
            ///     GpmAdapter.IdP.LogoutAll((error) =>
            ///     {
            ///         if (GpmAdapter.IsSuccess(error) == true)
            ///         {
            ///             Debug.Log("success");
            ///         }
            ///         else
            ///         {
            ///             Debug.Log(string.Format("failure. error:{0}", error));
            ///         }
            ///     });
            /// }
            /// </code>
            /// </example>
            public static void LogoutAll(Action<AdapterError> callback)
            {
                Miscellaneous.CheckNotNull(callback, "callback");
                IdPAdapterManager.Instance.LogoutAll(callback);
            }

            /// <summary>
            /// IdP의 인증 정보를 조회합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="idPName">IdP의 이름입니다.</param>
            /// <param name="callback">IdP의 인증 정보가 반환되며 callback은 null을 입력할 수 없습니다.</param>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleGetAuthInfo()
            /// {
            ///     GpmAdapter.IdP.GetAuthInfo(GpmAdapterType.IdP.FACEBOOK, (facebookAuthInfo) => 
            ///     {
            ///         Debug.Log(string.Format("authInfo:{0}", facebookAuthInfo));
            ///     });
            /// }
            /// </code>
            /// </example>
            public static void GetAuthInfo(string idPName, Action<string> callback)
            {
                Miscellaneous.CheckNotNull(callback, "callback");
                IdPAdapterManager.Instance.GetAuthInfo(idPName, callback);
            }

            /// <summary>
            /// IdP의 Profile 정보를 조회합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="idPName">IdP의 이름입니다.</param>
            /// <param name="callback">IdP의 프로필 정보가 반환되며 callback은 null을 입력할 수 없습니다.</param>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleGetProfile()
            /// {
            ///     GpmAdapter.IdP.GetProfile(GpmAdapterType.IdP.FACEBOOK, (facebookProfile) =>
            ///     {
            ///         if (facebookProfile == null)
            ///         {
            ///             Debug.Log("Facebook profile is null.");
            ///         }
            ///         else
            ///         {
            ///             foreach (KeyValuePair&lt;string, object&gt;kvp in facebookProfile)
            ///             {
            ///                 Debug.Log(string.Format("{0}:{1}\n", kvp.Key, kvp.Value));
            ///             }
            ///         }
            ///     });
            /// }
            /// </code>
            /// </example>
            public static void GetProfile(string idPName, Action<Dictionary<string, object>> callback)
            {
                Miscellaneous.CheckNotNull(callback, "callback");
                IdPAdapterManager.Instance.GetProfile(idPName, callback);
            }

            /// <summary>
            /// 로그인된 모든 IdP의 이름을 조회합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <returns>로그인된 모든 IdP의 이름이 반환됩니다.</returns>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleGetLoggedInIdPList()
            /// {
            ///     var loggedInIdPList = GpmAdapter.IdP.GetLoggedInIdPList();
            ///     foreach (var loggedInIdP in loggedInIdPList)
            ///     {
            ///         Debug.Log(string.Format("loggedInIdP:{0}", loggedInIdP));
            ///     }
            /// }
            /// </code>
            /// </example>
            public static List<string> GetLoggedInIdPList()
            {
                return IdPAdapterManager.Instance.GetLoggedInIdPList();
            }

            /// <summary>
            /// IdP의 UserId 정보를 조회합니다.
            /// @since Added 1.0.0
            /// </summary>
            /// <param name="idPName">IdP의 이름입니다.</param>
            /// <returns>IdP의 UserId 정보가 반환됩니다.</returns>
            /// <example>
            /// Example Usage :
            /// <code>
            /// private void SampleGetUserId()
            /// {
            ///     var facebookUserId = GpmAdapter.IdP.GetUserId(GpmAdapterType.IdP.FACEBOOK);
            ///     Debug.Log(string.Format("facebookUserId:{0}", facebookUserId));
            /// }
            /// </code>
            /// </example>
            public static string GetUserId(string idPName)
            {
                return IdPAdapterManager.Instance.GetUserId(idPName);
            }
        }
    }
}