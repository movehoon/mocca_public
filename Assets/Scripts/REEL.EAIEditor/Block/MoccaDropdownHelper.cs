using UnityEngine;
using UnityEngine.UI;
using REEL;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class MoccaDropdownHelper : MonoBehaviour
    {
        [SerializeField] private bool isExpressionDropdown = false;
        Dropdown dropdown;

        public enum Type
        {
            Facial, Motion, Mobility, VariableTypes, VariableList, Processes
        }

        public Type type = Type.Facial;

        private void OnEnable()
        {
            List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
            dropdown = GetComponent<Dropdown>();

            switch (type)
            {
                case Type.Facial:
                    {
                        data = isExpressionDropdown ? 
                            Constants.GetFacialExpressionOptionData : Constants.GetFacialOptionData;
                    }
                    break;
                case Type.Motion:
                    {
                        data = isExpressionDropdown ? 
                            Constants.GetMotionExpressionOptionData : Constants.GetMotionOptionData;
                    }
                    break;
                case Type.Mobility:
                    {
                        data = Constants.GetMobilityOptionData;
                    }
                    break;
                case Type.VariableTypes:
                    {
                        data = Constants.GetVariableTypeData;
                    }
                    break;
                case Type.VariableList:
                    {
                        MCWorkspaceManager.Instance.SubscribeVariableUpdate(ChangeVariableState);
                        data = Constants.GetVariableList;
                    }
                    break;
                case Type.Processes:
                    {
                        MCWorkspaceManager.Instance.SubscribeProcessUpdate(ChangeProcessState);
                        data = Constants.GetProcessOptionData;
                    }
                    break;
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(data);
        }

        public void SetType(PROJECT.DataType dataType)
        {
            tempDataType = dataType;
            Invoke("SetTypeWithDelay", 0.1f);
        }

        private PROJECT.DataType tempDataType;
        private void SetTypeWithDelay()
        {
            if (tempDataType == PROJECT.DataType.NONE)
            {
                return;
            }

            List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
            dropdown = GetComponent<Dropdown>();
            switch (tempDataType)
            {
                case PROJECT.DataType.FACIAL:
                    {
                        data = isExpressionDropdown ? 
                            Constants.GetFacialExpressionOptionData : Constants.GetFacialOptionData;

                        type = Type.Facial;
                    }
                    break;
                case PROJECT.DataType.MOTION:
                    {
                        data = isExpressionDropdown ? 
                            Constants.GetMotionExpressionOptionData : Constants.GetMotionOptionData;

                        type = Type.Motion;
                    }
                    break;
                case PROJECT.DataType.MOBILITY:
                    {
                        data = Constants.GetMobilityOptionData;

                        type = Type.Mobility;
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
            else if(type == Type.Processes)
            {
                MCWorkspaceManager.Instance.UnsubscribeProcessUpdate(ChangeProcessState);
            }

            dropdown.value = 0;
        }

        private void ChangeVariableState()
        {
            List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
            Dropdown dropdown = GetComponent<Dropdown>();
            data = Constants.GetVariableList;
            dropdown.ClearOptions();
            dropdown.AddOptions(data);
        }

        private void ChangeProcessState()
        {
            List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
            Dropdown dropdown = GetComponent<Dropdown>();
            data = Constants.GetProcessOptionData;
            dropdown.ClearOptions();
            dropdown.AddOptions(data);
        }
    }
}
