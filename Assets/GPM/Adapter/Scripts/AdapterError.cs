using Gpm.Adapter.Internal;
using Gpm.Common.Util;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gpm.Adapter
{
    public class AdapterError
    {
        public string domain = string.Empty;
        public int code = 0;
        public string message = string.Empty;
        public AdapterError error;

        public AdapterError()
        {
        }

        public AdapterError(int code, string domain = null, string message = null, AdapterError error = null)
        {
            this.code = code;
            this.domain = domain;

            if (string.IsNullOrEmpty(message) == true)
            {
                this.message = RetrieveErrorMessage();
            }
            else
            {
                this.message = message;
            }

            this.error = error;
        }

        public override string ToString()
        {
            return Regex.Unescape(GpmJsonMapper.ToJson(this));
        }

        private string RetrieveErrorMessage()
        {
            string errorName = string.Empty;
            FieldInfo[] fields = typeof(AdapterErrorCode).GetFields();

            var fieldIndex = Array.FindIndex(fields, SearchFieldIndex);
            errorName = fields[fieldIndex].Name;

            if (string.IsNullOrEmpty(errorName) == true)
            {
                LoggerMapper.Debug(string.Format("{0} code:{1}", AdapterStrings.ERROR_MESSAGE_NOT_FOUND, code), GetType());
                return string.Empty;
            }

            FieldInfo field = typeof(AdapterStrings).GetField(errorName);
            if (field == null)
            {
                return string.Empty;
            }

            return field.GetValue(null).ToString();
        }

        private bool SearchFieldIndex(FieldInfo field)
        {
            return (int)field.GetValue(null) == code;
        }
    }
}