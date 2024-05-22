namespace Gpm.Adapter.Internal
{
    public static class AdapterStrings
    {
        #region error
        public const string SUCCESS = "성공하였습니다.";
        public const string ADAPTER_NOT_FOUND = "어댑터를 찾을 수 없습니다. 어댑터를 설정하십시오.";
        public const string NOT_LOGGED_IN = "로그인이 되어 있지 않습니다. 로그인 후에 해당 API를 호출하십시오.";
        public const string USER_CANCELED = "유저가 취소하였습니다.";
        public const string EXTERNAL_LIBRARY_ERROR = "외부 라이브러리 오류입니다.";
        #endregion

        #region message
        public const string ALREADY_LOGGED_IN = "이미 로그인이 되어 있습니다.";
        public const string ERROR_MESSAGE_NOT_FOUND = "오류 코드와 일치하는 메시지를 찾을 수 없습니다.";

        public const string GOOGLE_SERVER_AUTH_CODE_EMPTY = "Google ServerAuthCode가 비어있습니다.";

        public const string FACEBOOK_ACCESS_TOKEN_IS_EMPTY = "Facebook AccessToken이 비어있습니다.";
        public const string FACEBOOK_PERMISSIONS_IS_EMPTY = "Facebook 권한이 비어있습니다.";
        public const string FACEBOOK_PERMISSIONS_NOT_FOUND = "Facebook 권한을 찾을 수 없습니다.";
        public const string FACEBOOK_PERMISSIONS_INVALID_TYPE = "잘못된 유형입니다. Facebook 권한의 데이터 유형은 List<string>입니다.";
        public const string SET_DEFAULT_FACEBOOK_PERMISSIONS = "Facebook 권한이 비어있거나 잘못 설정되어 [public_profile, email]로 설정됩니다.";
        public const string FAILED_TO_LOAD_FACEBOOK_PROFILE = "Facebook 프로필 로드에 실패하였습니다.";
        #endregion
    }
}