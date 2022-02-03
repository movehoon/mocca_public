using UnityEngine;
using UnityEngine.UI;
using REEL;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MoccaTMPDropdownHelper : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private bool isExpressionDropdown = false;

        private TMP_Dropdown dropdown;

        public enum Type
        {
            Facial, Motion, Mobility, VariableTypes, VariableList, Processes, LocalVariableTypes, LocalVariableList, ExpressionVariableList
        }

        public Type type = Type.Facial;

        private void OnEnable()
        {
            InitializeDropdownOptionData();
        }

        public void InitializeDropdownOptionData()
        {
            List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
            dropdown = GetComponent<TMP_Dropdown>();

            //string defaultValue = string.Empty;
            //int dropdownValue = -1;
            //var input = transform.parent.GetComponentInChildren<MCNodeInput>();
            //if (input != null && Utils.IsNullOrEmptyOrWhiteSpace(input.input.default_value) == false)
            //{
            //    defaultValue = input.input.default_value;
            //}

            switch (type)
            {
                case Type.Facial:
                    {
                        data = isExpressionDropdown ?
                            Constants.GetFacialExpressionTMPOptionData : Constants.GetFacialTMPOptionData;

                        //if (Utils.IsNullOrEmptyOrWhiteSpace(defaultValue) == false)
                        //{
                        //    dropdownValue = Constants.GetDropdownValueIndex(defaultValue, PROJECT.DataType.FACIAL);
                        //}
                    }
                    break;
                case Type.Motion:
                    {
                        data = isExpressionDropdown ?
                            Constants.GetMotionExpressionTMPOptionData : Constants.GetMotionTMPOptionData;

                        //if (Utils.IsNullOrEmptyOrWhiteSpace(defaultValue) == false)
                        //{
                        //    dropdownValue = Constants.GetDropdownValueIndex(defaultValue, PROJECT.DataType.MOTION);
                        //}
                    }
                    break;
                case Type.Mobility:
                    {
                        data = Constants.GetMobilityTMPOptionData;
                        
                        //if (Utils.IsNullOrEmptyOrWhiteSpace(defaultValue) == false)
                        //{
                        //    dropdownValue = Constants.GetDropdownValueIndex(defaultValue, PROJECT.DataType.MOBILITY);
                        //}
                    }
                    break;
                case Type.VariableTypes:
                case Type.LocalVariableTypes:
                    {
                        data = Constants.GetTMPVariableTypeData;
                    }
                    break;
                case Type.VariableList:
                    {
                        //MCWorkspaceManager.Instance.SubscribeVariableUpdate(ChangeVariableState);
                        data = Constants.TMPVariableList;
                    }
                    break;
                case Type.LocalVariableList:
                    {
                        data = Constants.TMPLocalVariableList;
                        //Utils.LogRed("[MoccaTMPDropdownHelper] LocalVariableList");
                    }
                    break;
                case Type.Processes:
                    {
                        MCWorkspaceManager.Instance.SubscribeProcessUpdate(ChangeProcessState);
                        data = Constants.GetProcessTMPOptionData;
                    }
                    break;

                case Type.ExpressionVariableList:
                    {
                        dropdown.gameObject.SetActive(true);
                        data = Constants.GetExpressionVariableListTMPOptionData;
                    }
                    break;
            }

            if (type == Type.ExpressionVariableList && data.Count == 0)
            {
                dropdown.gameObject.SetActive(false);
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(data);

            //// 기존 input.defaultvalue가 있는 경우에 이 값으로 설정하도록 처리.
            //dropdown.value = dropdownValue != -1 ? dropdown.value : dropdownValue;
            //Utils.LogBlue($"dropdown.value: {dropdown.value} / dropdownValue: {dropdownValue}");
        }

        private bool isDirty = false;
        private Action function = null;
        private void LateUpdate()
        {
            if (isDirty == true)
            {
                SetTypeWithDelay();
                function?.Invoke();

                isDirty = false;
                function = null;
            }
        }

        public void SetType(PROJECT.DataType dataType, Action function = null)
        {
            tempDataType = dataType;
            isDirty = true;
            this.function = function;
        }

        //public void SetType(PROJECT.DataType dataType)
        //{
        //    tempDataType = dataType;
        //    Invoke("SetTypeWithDelay", 0.1f);
        //}

        private PROJECT.DataType tempDataType;
        private void SetTypeWithDelay()
        {
            if (tempDataType == PROJECT.DataType.NONE)
            {
                return;
            }

            List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
            dropdown = GetComponent<TMP_Dropdown>();
            switch (tempDataType)
            {
                case PROJECT.DataType.FACIAL:
                    {
                        data = isExpressionDropdown ?
                            Constants.GetFacialExpressionTMPOptionData : Constants.GetFacialTMPOptionData;

                        type = Type.Facial;
                    }
                    break;
                case PROJECT.DataType.MOTION:
                    {
                        data = isExpressionDropdown ?
                            Constants.GetMotionExpressionTMPOptionData : Constants.GetMotionTMPOptionData;

                        type = Type.Motion;
                    }
                    break;
                case PROJECT.DataType.MOBILITY:
                    {
                        data = Constants.GetMobilityTMPOptionData;

                        type = Type.Mobility;
                    }
                    break;
                case PROJECT.DataType.VARIABLE:
                    {
                        data = type == Type.LocalVariableList ? Constants.TMPLocalVariableList : Constants.TMPVariableList;

                        type = Type.VariableList;
                    }
                    break;
                case PROJECT.DataType.EXPRESSION:
                    {
                        data = Constants.GetExpressionVariableListTMPOptionData;
                        type = Type.ExpressionVariableList;
                        if (data.Count > 0)
                        {
                            dropdown.gameObject.SetActive(true);
                        }
                        else
                        {
                            dropdown.gameObject.SetActive(false);
                        }
                    }
                    break;
                default: break;
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(data);
            tempDataType = PROJECT.DataType.NONE;
        }

        private void OnDisable()
        {
            if (type == Type.VariableList)
            {
                MCWorkspaceManager.Instance.UnsubscribeVariableUpdate(ChangeVariableState);
            }
            else if (type == Type.LocalVariableList)
            {
                MCWorkspaceManager.Instance.UnsubscribeLocalVariableUpdate(ChangeVariableState);
            }
            else if (type == Type.Processes)
            {
                MCWorkspaceManager.Instance.UnsubscribeProcessUpdate(ChangeProcessState);
            }

            // Todo: 비활성화하면서 값을 초기화하는데,
            // 이것 때문에 프로젝트 탭 -> 함수 탭 이동할 때 입력 파라미터가 모두 초기화 됨.
            // 0으로 초기화를 꼭 해야하는 지 확인 필요함.
            //dropdown.value = 0;
        }

        private void ChangeVariableState()
        {
            List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            data = type == Type.LocalVariableList ? Constants.TMPLocalVariableList : Constants.TMPVariableList;
            dropdown.ClearOptions();
            dropdown.AddOptions(data);
        }

        private void ChangeProcessState()
        {
            List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            data = Constants.GetProcessTMPOptionData;
            dropdown.ClearOptions();
            dropdown.AddOptions(data);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (type == Type.ExpressionVariableList)
            {
                string currentValue = dropdown.options[dropdown.value].text;
                var expresionVariableList = Constants.GetExpressionVariableListTMPOptionData;
                if (dropdown.options.Count != expresionVariableList.Count)
                {
                    dropdown.ClearOptions();
                    dropdown.AddOptions(expresionVariableList);
                    var variableNameList = MCWorkspaceManager.Instance.GetVariableNameListWithType(PROJECT.DataType.EXPRESSION);
                    for (int ix = 0; ix < variableNameList.Length; ++ix)
                    {
                        if (variableNameList[ix].Equals(currentValue) == true)
                        {
                            dropdown.value = ix;
                            break;
                        }
                    }
                }
            }
        }
    }
}
