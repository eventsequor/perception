#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Gpm.Adapter.Tool
{
    public class AdapterSettingTool : EditorWindow
    {
        private const int GUI_COPYRIGHT_HEIGHT = 20;
        private const int GUI_MAIN_AREA_MARGIN_LEFT_RIGHT = 5;
        private const int GUI_MAIN_AREA_MARGIN_TOP = 8;

        private static readonly Rect WindowSize = new Rect(0, 0, 450, 400); 

        private const string GUI_LABEL_BUTTON = "Set";
        private const string GUI_TEXT_MESSAGE = "Message";
        private const string GUI_TEXT_COPYRIGHT = "ⓒ NHN Corp. All rights reserved.";
        private const string GUI_LABEL_ENABLE_FACEBOOK = "Enable FacebookAdapter";
        private const string GUI_LABEL_ENABLE_GPGS = "Enable GpgsAdapter";

        private const string MESSAGE_ADAPTER_NOT_FOUND_EXCEPTION = "<color=#ff6f02>Do not move or delete the IdP Adapter files.</color>";
        private const string MESSAGE_ACTIVATED_IDP = "<b><color=#4b96e6>Activated</color></b> {0}Adapter";
        private const string MESSAGE_DEACTIVATED_IDP = "<b><color=#43a047>Deactivated</color></b> {0}Adapter";
        private const string MESSAGE_SETTINGS_COMPLETED = "Settings completed.";

        private const string FILE_FACEBOOK_ADAPTER = "GPM/Adapter/IdP/Facebook/Scripts/FacebookAdapter.cs";
        private const string FILE_GPGS_ADAPTER = "GPM/Adapter/IdP/Gpgs/Scripts/GpgsAdapter.cs";

        private const string TEXT_DEFINE_FACEBOOK = "#define GPM_USE_FACEBOOK";
        private const string TEXT_DEFINE_GPGS = "#define GPM_USE_GPGS";

        private const string FORMAT_NOW_TIME = "yyyy/MM/dd HH:mm:ss";

        private Dictionary<string, IdPInfo> idPInfoDictionary;
        private StringBuilder message = new StringBuilder();

        [MenuItem("Tools/GPM/Adapter/Settings")]
        public static void SetDefineInIdPAdapter()
        {
            GetWindowWithRect<AdapterSettingTool>(WindowSize, true, "Adapter");
        }

        private void OnEnable()
        {
            InitIdPInfo();
        }
        
#region GUI

        private Vector2 scrollPos = new Vector2();

        private void OnGUI()
        {
            Rect mainAreaRect = new Rect(
                GUI_MAIN_AREA_MARGIN_LEFT_RIGHT,
                GUI_MAIN_AREA_MARGIN_TOP,
                position.width - (GUI_MAIN_AREA_MARGIN_LEFT_RIGHT * 2),
                position.height - GUI_COPYRIGHT_HEIGHT - (GUI_MAIN_AREA_MARGIN_TOP * 2));

            GUILayout.BeginArea(mainAreaRect); 
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        DrawAdapterSettingUI();

                        if (string.IsNullOrEmpty(message.ToString()) == false)
                        {
                            DrawErrorUI();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            EditorGUILayout.BeginHorizontal();
            {
                DrawCopyrightUI();
            }
            EditorGUILayout.EndHorizontal();            
        }

        private void DrawAdapterSettingUI()
        {
            foreach (string key in idPInfoDictionary.Keys)
            {
                if (idPInfoDictionary[key].isError == true)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle(idPInfoDictionary[key].enableLabelText, false, ToolStyles.DefaultToggle);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    idPInfoDictionary[key].useIdP = EditorGUILayout.Toggle(idPInfoDictionary[key].enableLabelText, idPInfoDictionary[key].useIdP, ToolStyles.DefaultToggle);
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(GUI_LABEL_BUTTON, ToolStyles.DefaultButton) == true)
                {
                    AddNowTimeInMessage();

                    foreach (string key in idPInfoDictionary.Keys)
                    {
                        SetDefine(key, idPInfoDictionary[key].useIdP);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawErrorUI()
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            GUILayout.Label(GUI_TEXT_MESSAGE, ToolStyles.ErrorLabel);

            EditorGUILayout.BeginHorizontal(ToolStyles.CopyrightBox);
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                GUILayout.Label(message.ToString(), ToolStyles.ErrorLabel);
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawCopyrightUI()
        {
            GUILayout.BeginArea(new Rect(0, position.height - GUI_COPYRIGHT_HEIGHT, position.width, GUI_COPYRIGHT_HEIGHT));
            EditorGUILayout.BeginVertical(ToolStyles.CopyrightBox);
            {
                GUILayout.Label(GUI_TEXT_COPYRIGHT, ToolStyles.CopyrightLabel);
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

#endregion

        private void InitIdPInfo()
        {
            if (idPInfoDictionary == null)
            {
                idPInfoDictionary = new Dictionary<string, IdPInfo>
                {
                    {
                        GpmAdapterType.IdP.FACEBOOK,
                        new IdPInfo
                        {
                            fileFullPath = string.Format("{0}/{1}", Application.dataPath, FILE_FACEBOOK_ADAPTER),
                            defineText = TEXT_DEFINE_FACEBOOK,
                            enableLabelText = GUI_LABEL_ENABLE_FACEBOOK
                        }
                    },

                    {
                        GpmAdapterType.IdP.GPGS,
                        new IdPInfo
                        {
                            fileFullPath = string.Format("{0}/{1}", Application.dataPath, FILE_GPGS_ADAPTER),
                            defineText = TEXT_DEFINE_GPGS,
                            enableLabelText = GUI_LABEL_ENABLE_GPGS
                        }
                    }
                };
            }

            foreach (string key in idPInfoDictionary.Keys)
            {
                InitializeDefines(key);
            }
        }

        private void InitializeDefines(string idP)
        {
            string sourceCodeText = string.Empty;

            try
            {
                sourceCodeText = GetSourceCodeText(idP);
                idPInfoDictionary[idP].isError = false;
            }
            catch (Exception e)
            {
                AddExceptionMessage(idP, e);
                return;
            }

            string define = idPInfoDictionary[idP].defineText;
            idPInfoDictionary[idP].useIdP = HasDefine(define, sourceCodeText);
        }

        private void SetDefine(string idP, bool isActivate)
        {
            string sourceCodeText = string.Empty;

            try
            {
                sourceCodeText = GetSourceCodeText(idP);
                idPInfoDictionary[idP].isError = false;
            }
            catch (Exception e)
            {
                AddExceptionMessage(idP, e);
                return;
            }

            string define = idPInfoDictionary[idP].defineText;

            if (isActivate == true)
            {
                if (HasDefine(define, sourceCodeText) == true)
                {
                    AddMessage(string.Format(MESSAGE_ACTIVATED_IDP, idP));
                    return;
                }

                var builder = new StringBuilder();
                builder.AppendLine(define);
                builder.Append(sourceCodeText);
                sourceCodeText = builder.ToString();
            }
            else
            {
                if (HasDefine(define, sourceCodeText) == false)
                {
                    AddMessage(string.Format(MESSAGE_DEACTIVATED_IDP, idP));
                    return;
                }

                sourceCodeText = sourceCodeText.Substring(define.Length + 1);
            }

            using (StreamWriter writer = new StreamWriter(idPInfoDictionary[idP].fileFullPath))
            {
                writer.WriteLine(sourceCodeText);
                if (isActivate == true)
                {
                    AddMessage(string.Format(MESSAGE_ACTIVATED_IDP, idP));
                }
                else
                {
                    AddMessage(string.Format(MESSAGE_DEACTIVATED_IDP, idP));
                }
            }

            AssetDatabase.Refresh();
        }

        private void AddExceptionMessage(string idP, Exception exception)
        {
            var errorMessage = new StringBuilder();

            if (exception is FileNotFoundException)
            {
                errorMessage.AppendLine(MESSAGE_ADAPTER_NOT_FOUND_EXCEPTION);
                errorMessage.AppendLine();
            }
            errorMessage.AppendLine(exception.Message);

            AddMessage(errorMessage.ToString());
            idPInfoDictionary[idP].isError = true;
        }

        private void AddNowTimeInMessage()
        {
            if (message.Length > 0)
            {
                message.AppendLine("------------------------------------------------------------");
            }

            message.AppendLine(DateTime.Now.ToString(FORMAT_NOW_TIME));
        }

        private void AddMessage(string txt)
        {
            message.AppendLine(txt);
        }

        private string GetSourceCodeText(string idP)
        {
            try
            {
                return File.ReadAllText(idPInfoDictionary[idP].fileFullPath, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool HasDefine(string define, string sourceCodeText)
        {
            if (sourceCodeText.IndexOf(define) == -1)
            {
                return false;
            }

            return true;
        }
    }

    internal class IdPInfo
    {
        public string fileFullPath;
        public string defineText;
        public string enableLabelText;
        public bool useIdP;
        public bool isError;
    }
}

#endif