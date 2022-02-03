using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class MultiLanguageData
    {
        public string nameEnglish;
        public string nameKorean;

        public MultiLanguageData(string english, string korean)
        {
            nameEnglish = english;
            nameKorean = korean;
        }
    }

    public static class Constants
    {
        public enum ConditionBlockType
        {
            STT, TTS, FACIAL, MOTION, VARIABLE, LENGTH
        }

        public enum BranchCompareBlockType
        {
            STT, VARIABLE, LENGTH
        }

        public enum APIBranchType
        {
            XML, JSON, LENGTH
        }

        public enum MobilityType
        {
            stop, forward, backward, turnLeft, turnRight, Length
        }

        public enum EmotionType
        {
            CALM, SURPRISED, HAPPY, ANGRY, DISGUSTED, CONFUSED, SAD, Length
        }

        public enum UIType { None, DropDown, InputField };

        // Tab Owner Group.
        public enum OwnerGroup { NONE, PROJECT, FUNCTION }

        public static readonly int switchBlockMin = 2;
        public static readonly int switchBlockMax = 5;

        public static readonly int apiBlockMin = 1;
        public static readonly int apiBlockMax = 5;

        public static string[] mobilityList = new string[]
        {
            "forward", "backward", "left", "right", "stop"
        };

        public static MultiLanguageData[] mobilityListData = new MultiLanguageData[]
        {
            new MultiLanguageData("forward", "전진"),
            new MultiLanguageData("backward", "후진"),
            new MultiLanguageData("left", "좌회전"),
            new MultiLanguageData("right", "우회전"),
            new MultiLanguageData("stop", "정지")
        };

        //public static string[] motionList = new string[]
        //{
        //    "hi", "hello", "angry", "sad", "ok", "clap", "no", "wrong", "happy", "nodRight", "nodLeft", "none",
        //    "head_up", "head_left_up", "head_right_up", "head_left", "head_right", "head_down", "head_left_down", "head_right_down", 
        //    //"breathing_active"
        //};

        public static MultiLanguageData[] motionListData = new MultiLanguageData[]
        {
            new MultiLanguageData("hi", "하이"),
            new MultiLanguageData("hello", "헬로"),
            new MultiLanguageData("angry", "화남"),
            new MultiLanguageData("sad", "슬픔"),
            new MultiLanguageData("ok", "오케이"),
            new MultiLanguageData("clap", "박수"),
            new MultiLanguageData("no", "아니"),
            new MultiLanguageData("wrong", "틀림"),
            new MultiLanguageData("happy", "행복"),
            new MultiLanguageData("nodRight", "끄덕이기(우)"),
            new MultiLanguageData("nodLeft", "끄덕이기(좌)"),
            new MultiLanguageData("head_up", "고개들기"),
            new MultiLanguageData("head_left_up", "고개들기(좌)"),
            new MultiLanguageData("head_right_up", "고개들기(우)"),
            new MultiLanguageData("head_left", "고개돌림(좌)"),
            new MultiLanguageData("head_right", "고개돌림(우)"),
            new MultiLanguageData("head_down", "고개숙임"),
            new MultiLanguageData("head_left_down", "고개숙임(좌)"),
            new MultiLanguageData("head_right_down", "고개숙임(우)"),
            new MultiLanguageData("none", "없음")
            //new MultiLanguageData("breathing_active", "숨쉬기"),
        };

        public static MultiLanguageData[] motionExpressionListData = new MultiLanguageData[]
        {
            new MultiLanguageData("auto motion", "자동 모션"),
            new MultiLanguageData("hi", "하이"),
            new MultiLanguageData("hello", "헬로"),
            new MultiLanguageData("angry", "화남"),
            new MultiLanguageData("sad", "슬픔"),
            new MultiLanguageData("ok", "오케이"),
            new MultiLanguageData("clap", "박수"),
            new MultiLanguageData("no", "아니"),
            new MultiLanguageData("wrong", "틀림"),
            new MultiLanguageData("happy", "행복"),
            new MultiLanguageData("nodRight", "끄덕이기(우)"),
            new MultiLanguageData("nodLeft", "끄덕이기(좌)"),
            new MultiLanguageData("head_up", "고개들기"),
            new MultiLanguageData("head_left_up", "고개들기(좌)"),
            new MultiLanguageData("head_right_up", "고개들기(우)"),
            new MultiLanguageData("head_left", "고개돌림(좌)"),
            new MultiLanguageData("head_right", "고개돌림(우)"),
            new MultiLanguageData("head_down", "고개숙임"),
            new MultiLanguageData("head_left_down", "고개숙임(좌)"),
            new MultiLanguageData("head_right_down", "고개숙임(우)"),
            
            new MultiLanguageData("none", "없음"),
            //new MultiLanguageData("breathing_active", "숨쉬기"),
        };

        //public static string[] facialList = new string[]
        //{
        //    "happy", "angry", "fear", "gazeup", "gazedown", "gazeleft", "gazeright",
        //    "normal", "sad", "smile", "speak", "surprised", "winkleft", "winkright", "none"
        //};

        public static MultiLanguageData[] facialListData = new MultiLanguageData[]
        {
            new MultiLanguageData("happy", "행복"),
            new MultiLanguageData("angry", "화남"),
            new MultiLanguageData("fear", "두려움"),
            new MultiLanguageData("gazeup", "쳐다보기(상)"),
            new MultiLanguageData("gazedown", "쳐다보기(하)"),
            new MultiLanguageData("gazeleft", "쳐다보기(좌)"),
            new MultiLanguageData("gazeright", "쳐다보기(우)"),
            new MultiLanguageData("normal", "보통"),
            new MultiLanguageData("sad", "슬픔"),
            new MultiLanguageData("smile", "미소"),
            new MultiLanguageData("speak", "말하기"),
            new MultiLanguageData("surprised", "놀람"),
            new MultiLanguageData("winkleft", "윙크(좌)"),
            new MultiLanguageData("winkright", "윙크(우)"),
            new MultiLanguageData("none", "없음"),
        };

        public static MultiLanguageData[] facialExpressionListData = new MultiLanguageData[]
        {
            new MultiLanguageData("auto facial", "자동 표정"),
            new MultiLanguageData("happy", "행복"),
            new MultiLanguageData("angry", "화남"),
            new MultiLanguageData("fear", "두려움"),
            new MultiLanguageData("gazeup", "쳐다보기(상)"),
            new MultiLanguageData("gazedown", "쳐다보기(하)"),
            new MultiLanguageData("gazeleft", "쳐다보기(좌)"),
            new MultiLanguageData("gazeright", "쳐다보기(우)"),
            new MultiLanguageData("normal", "보통"),
            new MultiLanguageData("sad", "슬픔"),
            new MultiLanguageData("smile", "미소"),
            new MultiLanguageData("speak", "말하기"),
            new MultiLanguageData("surprised", "놀람"),
            new MultiLanguageData("winkleft", "윙크(좌)"),
            new MultiLanguageData("winkright", "윙크(우)"),
            
            new MultiLanguageData("none", "없음"),
        };

        public static List<string> variableTypeList = new List<string>();

        public static List<Dropdown.OptionData> GetVariableList
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (string name in MCWorkspaceManager.Instance.VariableNameList)
                {
                    options.Add(new Dropdown.OptionData() { text = name });
                }

                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> TMPVariableList
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (string name in MCWorkspaceManager.Instance.VariableNameList)
                {
                    options.Add(new TMP_Dropdown.OptionData() { text = name });
                }

                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> TMPLocalVariableList
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (string name in MCWorkspaceManager.Instance.LocalVariableNameList)
                {
                    options.Add(new TMP_Dropdown.OptionData() { text = name });
                }

                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetExpressionVariableListTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (string name in MCWorkspaceManager.Instance.GetVariableNameListWithType(PROJECT.DataType.EXPRESSION))
                {
                    options.Add(new TMP_Dropdown.OptionData() { text = name });
                }

                string[] localVariableList = MCWorkspaceManager.Instance.GetLocalVariableNameListWithType(PROJECT.DataType.EXPRESSION);
                if (localVariableList != null && localVariableList.Length > 0)
                {
                    foreach (string name in localVariableList)
                    {
                        options.Add(new TMP_Dropdown.OptionData() { text = name });
                    }
                }

                return options;
            }
        }

        public static string ParseMobilityKoreanToEnglish(int index)
        {
            return mobilityListData[index].nameEnglish;
        }

        public static string ParseMotionKoreanToEnglish(int index)
        {
            return motionListData[index].nameEnglish;
        }

        public static string ParseMotionExpressionKoreanToEnglish(int index)
        {
            return motionExpressionListData[index].nameEnglish;
        }

        public static string ParseFacialKoreanToEnglish(int index)
        {
            return facialListData[index].nameEnglish;
        }

        public static string ParseFacialExpressionKoreanToEnglish(int index)
        {
            return facialExpressionListData[index].nameEnglish;
        }

        public static string GetFacialKoreanName(string value)
        {
            for (int i = 0; i < facialListData.Length; i++)
            {
                if (facialListData[i].nameEnglish.Equals(value))
                    return facialListData[i].nameKorean;
            }

            return string.Empty;
        }

        public static string GetMotionKoreanName(string value)
        {
            for (int i = 0; i < motionListData.Length; i++)
            {
                if (motionListData[i].nameEnglish.Equals(value))
                    return motionListData[i].nameKorean;
            }

            return string.Empty;
        }

        public static string GetMobilityKoreanName(string value)
        {
            for (int i = 0; i < mobilityListData.Length; i++)
            {
                if (mobilityListData[i].nameEnglish.Equals(value))
                    return mobilityListData[i].nameKorean;
            }

            return string.Empty;
        }

        public static string GetValueForKorean(string value, PROJECT.DataType dataType)
        {
            switch (dataType)
            {
                case PROJECT.DataType.BOOL:
                case PROJECT.DataType.NUMBER:
                case PROJECT.DataType.STRING:
                    return value;
                case PROJECT.DataType.FACIAL:
                    return GetFacialKoreanName(value);
                case PROJECT.DataType.MOTION:
                    return GetMotionKoreanName(value);
                case PROJECT.DataType.MOBILITY:
                    return GetMobilityKoreanName(value);
                default:
                    return string.Empty;
            }
        }

        public static int GetDropdownValueIndex(string value, PROJECT.DataType dataType)
        {
            switch (dataType)
            {
                case PROJECT.DataType.FACIAL:
                    return GetFacialIndexFromEnglish(value);
                case PROJECT.DataType.MOTION:
                    return GetMotionIndexFromEnglish(value);
                case PROJECT.DataType.MOBILITY:
                    return GetMobilityIndexFromEnglish(value);
                default:
                    return -1;
            }
        }

        public static int GetMobilityIndexFromEnglish(string mobilityName)
        {
            for (int i = 0; i < mobilityListData.Length; i++)
            {
                if (mobilityListData[i].nameEnglish.Equals(mobilityName))
                    return i;
            }

            return -1;
        }

        public static int GetMobilityIndexFromKorean(string mobilityName)
        {
            for (int i = 0; i < mobilityListData.Length; i++)
            {
                if (mobilityListData[i].nameKorean.Equals(mobilityName))
                    return i;
            }

            return -1;
        }

        public static int GetFacialIndexFromEnglish(string faceName)
        {
            for (int ix = 0; ix < facialListData.Length; ++ix)
            {
                if (facialListData[ix].nameEnglish.Equals(faceName))
                    return ix;
            }

            return -1;
        }

        public static int GetFacialExpressionIndexFromEnglish(string faceName)
        {
            for (int ix = 0; ix < facialExpressionListData.Length; ++ix)
            {
                if (facialExpressionListData[ix].nameEnglish.Equals(faceName))
                    return ix;
            }

            return -1;
        }

        public static int GetFacialIndexFromKorean(string faceName)
        {
            for (int ix = 0; ix < facialListData.Length; ++ix)
            {
                if (facialListData[ix].nameKorean.Equals(faceName))
                    return ix;
            }

            return -1;
        }

        public static int GetFacialExpressionFromKorean(string faceName)
        {
            for (int ix = 0; ix < facialExpressionListData.Length; ++ix)
            {
                if (facialExpressionListData[ix].nameKorean.Equals(faceName))
                    return ix;
            }

            return -1;
        }

        public static int GetMotionIndexFromEnglish(string motionName)
        {
            for (int ix = 0; ix < motionListData.Length; ++ix)
            {
                if (motionListData[ix].nameEnglish.Equals(motionName))
                    return ix;
            }

            return -1;
        }

        public static int GetMotionExpressionIndexFromEnglish(string motionName)
        {
            for (int ix = 0; ix < motionExpressionListData.Length; ++ix)
            {
                if (motionExpressionListData[ix].nameEnglish.Equals(motionName))
                    return ix;
            }

            return -1;
        }

        public static int GetMotionIndexFromKorean(string motionName)
        {
            for (int ix = 0; ix < motionListData.Length; ++ix)
            {
                if (motionListData[ix].nameKorean.Equals(motionName))
                    return ix;
            }

            return -1;
        }

        public static int GetMotionExpressionIndexFromKorean(string motionName)
        {
            for (int ix = 0; ix < motionExpressionListData.Length; ++ix)
            {
                if (motionExpressionListData[ix].nameKorean.Equals(motionName))
                    return ix;
            }

            return -1;
        }

        public static int GetProcessIndexWithProcessID(string processID)//MCPause, MCResume에서 process.id를 저장해서 만든 함수.
        {
            List<PROJECT.Process> processes = MCWorkspaceManager.Instance.Processes;
            for (int ix = 0; ix < processes.Count; ++ix)
            {
                if (processes[ix].id.ToString().Equals(processID))
                    return ix;
            }

            return -1;
        }

        public static int GetExpressionVariableIndexWithName(string variableName)
        {
            List<TMP_Dropdown.OptionData> options = GetExpressionVariableListTMPOptionData;
            for (int ix = 0; ix < options.Count; ++ix)
            {
                if (options[ix].text.Equals(variableName) == true)
                {
                    return ix;
                }
            }

            return -1;
        }

        public static List<Dropdown.OptionData> GetMobilityOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (MultiLanguageData item in mobilityListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetMobilityTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (MultiLanguageData item in mobilityListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<Dropdown.OptionData> GetMotionOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (MultiLanguageData item in motionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetMotionTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (MultiLanguageData item in motionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<Dropdown.OptionData> GetMotionExpressionOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (MultiLanguageData item in motionExpressionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetMotionExpressionTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (MultiLanguageData item in motionExpressionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<Dropdown.OptionData> GetFacialOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (MultiLanguageData item in facialListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetFacialTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (MultiLanguageData item in facialListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<Dropdown.OptionData> GetFacialExpressionOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (MultiLanguageData item in facialExpressionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetFacialExpressionTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (MultiLanguageData item in facialExpressionListData)
                {
                    string name = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
                        item.nameKorean : item.nameEnglish;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
                return options;
            }
        }

        public static List<Dropdown.OptionData> GetVariableTypeData
        {
            get
            {
                if (variableTypeList.Count == 0)
                {
                    InitVariableTypeList();
                }

                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (string type in variableTypeList)
                {
                    options.Add(new Dropdown.OptionData(type));
                }

                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetTMPVariableTypeData
        {
            get
            {
                if (variableTypeList.Count == 0)
                {
                    InitVariableTypeList();
                }

                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (string type in variableTypeList)
                {
                    options.Add(new TMP_Dropdown.OptionData(type));
                }

                return options;
            }
        }

        public static List<Dropdown.OptionData> GetProcessOptionData
        {
            get
            {
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (var process in MCWorkspaceManager.Instance.Processes)
                {
                    options.Add(new Dropdown.OptionData() { text = process.name });
                }

                return options;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetProcessTMPOptionData
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                foreach (var process in MCWorkspaceManager.Instance.Processes)
                {
                    options.Add(new TMP_Dropdown.OptionData() { text = process.name });
                }

                return options;
            }
        }
        

        private static void InitVariableTypeList()
        {
            int length = (int)(PROJECT.DataType.FUNCTION) + 1;
            for (int ix = 0; ix < length; ++ix)
            {
                PROJECT.DataType dataType = ((REEL.PROJECT.DataType)(ix));
                if (dataType == PROJECT.DataType.NONE
                    || dataType == PROJECT.DataType.VARIABLE
                    || dataType == PROJECT.DataType.LIST
                    || dataType == PROJECT.DataType.FUNCTION)
                    continue;

                variableTypeList.Add(dataType.ToString());
            }
        }

        public static PROJECT.DataType GetDataType(string typeName)
        {
            int length = (int)(PROJECT.DataType.FUNCTION) + 1;
            for (int ix = 0; ix < length; ++ix)
            {
                PROJECT.DataType dataType = ((PROJECT.DataType)(ix));
                if (dataType.ToString().Equals(typeName))
                {
                    return dataType;
                }
            }

            return PROJECT.DataType.NONE;
        }

        public static List<Dropdown.OptionData> GetOptionDataWithType(MoccaDropdownHelper.Type type)
        {
            switch (type)
            {
                case MoccaDropdownHelper.Type.Facial:
                    {
                        return GetFacialOptionData;
                    }
                case MoccaDropdownHelper.Type.Motion:
                    {
                        return GetMotionOptionData;
                    }
                case MoccaDropdownHelper.Type.Mobility:
                    {
                        return GetMobilityOptionData;
                    }
                case MoccaDropdownHelper.Type.VariableList:
                    {
                        return GetVariableList;
                    }
                case MoccaDropdownHelper.Type.Processes:
                    {
                        return GetProcessOptionData;
                    }
                default: return null;
            }
        }

        public static List<TMP_Dropdown.OptionData> GetTMPOptionDataWithType(MoccaTMPDropdownHelper.Type type)
        {
            switch (type)
            {
                case MoccaTMPDropdownHelper.Type.Facial:
                    {
                        return GetFacialTMPOptionData;
                    }
                case MoccaTMPDropdownHelper.Type.Motion:
                    {
                        return GetMotionTMPOptionData;
                    }
                case MoccaTMPDropdownHelper.Type.Mobility:
                    {
                        return GetMobilityTMPOptionData;
                    }
                case MoccaTMPDropdownHelper.Type.VariableList:
                    {
                        return TMPVariableList;
                    }
                case MoccaTMPDropdownHelper.Type.Processes:
                    {
                        return GetProcessTMPOptionData;
                    }
                default: return null;
            }
        }

        public static int ConvertVariableTypeIndexToDataType(int variableTypeIndex)
        {
            //Debug.Log("ConvertVariableTypeIndexToDataType: " + variableTypeIndex);
            int length = (int)(PROJECT.DataType.FUNCTION) + 1;
            string typeText = variableTypeList[variableTypeIndex];
            for (int ix = 0; ix < length; ++ix)
            {
                string dataTypeText = ((PROJECT.DataType)ix).ToString();
                if (typeText.Equals(dataTypeText))
                {
                    return ix;
                }
            }

            return 0;
        }

        public static int ConvertDataTypeIndexToVariableTypeIndex(PROJECT.DataType dataType)
        {
            int length = GetVariableTypeData.Count;
            for (int ix = 0; ix < length; ++ix)
            {
                if (dataType.ToString().Equals(GetVariableTypeData[ix].text))
                {
                    return ix;
                }
            }

            return 0;
        }
    }
}