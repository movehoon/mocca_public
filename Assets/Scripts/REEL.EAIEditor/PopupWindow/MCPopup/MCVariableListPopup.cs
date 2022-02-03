#define USINGTMPPRO

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ESingleOrListType = REEL.D2EEditor.MCVariableListTypeDropdown.ESingleOrListType;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
	public class MCVariableListPopup : MCPopup
	{
        [SerializeField]
        private TMP_InputField nameText;

        [SerializeField]
        private Dropdown typeDropdown;

        [SerializeField]
        private Dropdown singleOrListDropdown;


        [SerializeField]
        private MCValueInput valueInput;

        [SerializeField]
        private MCBooleanListPopup booleanListInput;

        [SerializeField]
        private MCValueListInput valueListInput;

        [SerializeField]
        private MCFaceListInput faceListInput;

        [SerializeField]
        private MCMotionListInput motionListInput;

        [SerializeField]
        private MCMobilityListInput mobilityListInput;

        [SerializeField]
        private MCExpressionInput expressionInput;

        [SerializeField]
        private GameObject booleanInput;

        [SerializeField]
        private GameObject faceInput;

        [SerializeField]
        private GameObject motionInput;

        [SerializeField]
        private GameObject mobilityInput;

        [SerializeField]
        private float normalHeight = 190f;
        [SerializeField]
        private float facialOrMotionHeight = 190f;
        [SerializeField]
        private float expressionHeight = 280f;

        [SerializeField]
        private float inputBaseYPos = -90f;

        [SerializeField]
        private float blockHeight = 45f;

        [SerializeField]
        private LeftMenuVariableItem variableItem;

        private bool isLocalVariable = false;
        private string functionName = "";

        private MCVariableFunctionManager manager = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (manager == null)
            {
                manager = FindObjectOfType<MCVariableFunctionManager>();
            }
        }

        private void OnDisable()
        {
            variableItem = null;
            isLocalVariable = false;
            functionName = "";
        }

        public override void OnOKClicked()
        {
            if (Utils.IsNullOrEmptyOrWhiteSpace(nameText.text) == true)
            {
                //Utils.LogRed("Must enter a varabble name");
                MessageBox.Show("[ID_MSG_ENTER_VARIABLE_NAME]변수 이름을 입력해주세요."); // local 추가 완료
                //HidePopup();
                return;
            }

            if (manager == null)
            {
                manager = FindObjectOfType<MCVariableFunctionManager>();
            }

            // 동일한 이름을 가진 변수가 있는지 확인.
            if (isLocalVariable == true)
            {
                //if (variableItem == null && manager.CanAddLocalVariable(nameText.text) == false)
                if (manager.CanAddLocalVariable(nameText.text, variableItem) == false)
                {
                    MessageBox.Show("[ID_SAME_LOCAL_VARIABLE]같은(동일한) 이름을 가진 지역 변수가 이미 존재합니다.\n다른 이름을 입력해주세요.");
                    return;
                }
            }
            else
            {
                //if (variableItem == null && manager.CanAddVariable(nameText.text) == false)
                if (manager.CanAddVariable(nameText.text, variableItem) == false)
                {
                    MessageBox.Show("[ID_SAME_VARIABLE]같은(동일한) 이름을 가진 변수가 이미 존재합니다.\n다른 이름을 입력해주세요.");
                    return;
                }
            }

            PROJECT.DataType dataType = Constants.GetDataType(typeDropdown.options[typeDropdown.value].text);
            PROJECT.NodeType finalNodeType;
            //Debug.Log(dataType);
            if (dataType == PROJECT.DataType.EXPRESSION)
            {
                finalNodeType = PROJECT.NodeType.EXPRESSION;
            }
            else
            {
                finalNodeType = currentSingleOrListType == 0 ? PROJECT.NodeType.VARIABLE : PROJECT.NodeType.LIST;
            }

            // 2020.10.07 로컬 변수 관련 처리 로직 추가.
            if (variableItem != null)
            {
                variableItem.SetName(nameText.text);
                variableItem.SetNodeType(finalNodeType);
                variableItem.SetDataType(dataType);
                variableItem.SetValue(GetValue(), dataType == PROJECT.DataType.EXPRESSION ? "EXPRESSION" : "");

                if (variableItem.isLocalVariable == true)
                {
                    MCWorkspaceManager.Instance.UpdateLocalVariable();
                }
                else
                {
                    MCWorkspaceManager.Instance.UpdateVariable();
                }
            }
            else
            {
                if (isLocalVariable == true)
                {
                    MCWorkspaceManager.Instance.AddLocalVariable(
                        nameText.text,
                        Constants.GetDataType(typeDropdown.options[typeDropdown.value].text),
                        finalNodeType,
                        GetValue(),
                        functionName);
                }
                else
                {
                    MCWorkspaceManager.Instance.AddVariable(
                        nameText.text,
                        Constants.GetDataType(typeDropdown.options[typeDropdown.value].text),
                        finalNodeType,
                        GetValue()
                    );
                }
            }

            #region 2020.10.07 백업.
            //if (variableItem != null)
            //{
            //    variableItem.SetName(nameText.text);
            //    variableItem.SetNodeType(finalNodeType);
            //    variableItem.SetDataType(dataType);
            //    variableItem.SetValue(GetValue(), dataType == PROJECT.DataType.EXPRESSION ? "EXPRESSION" : "");

            //    MCWorkspaceManager.Instance.UpdateVariable();
            //}
            //else
            //{
            //    MCWorkspaceManager.Instance.AddVariable(
            //        nameText.text,
            //        Constants.GetDataType(typeDropdown.options[typeDropdown.value].text),
            //        finalNodeType,
            //        GetValue()
            //    );
            //}
            #endregion

            ResetAllInputValues();
            base.OnOKClicked();
        }

        public override void OnCancelClicked()
        {
            ResetAllInputValues();
            base.OnCancelClicked();
        }

        private void ResetAllInputValues()
        {
            nameText.text = string.Empty;
            singleOrListDropdown.value = 0;
            typeDropdown.value = 0;
            valueInput.Input = string.Empty;

            booleanListInput.ResetValues();
            expressionInput.ResetValues();
            booleanInput.GetComponentInChildren<Toggle>().isOn = false;
            faceInput.GetComponentInChildren<Dropdown>().value = 0;
            motionInput.GetComponentInChildren<Dropdown>().value = 0;
            mobilityInput.GetComponentInChildren<Dropdown>().value = 0;
        }

        public override void ShowPopup(MCNode node = null)
        {
            base.ShowPopup(node);

            if (variableItem == null)
            {
                return;
            }

            nameText.text = variableItem.VariableName;
            isLocalVariable = variableItem.isLocalVariable;
            typeDropdown.value = Constants.ConvertDataTypeIndexToVariableTypeIndex(variableItem.dataType);

            // 데이터 타입이 expression이면 싱글/리스트 드롭다운 끄고, 
            // 그 외의 데이터는 켜기.
            bool show = variableItem.dataType != PROJECT.DataType.EXPRESSION;
            singleOrListDropdown.gameObject.SetActive(show);

            if (variableItem.dataType == PROJECT.DataType.EXPRESSION)
            {
                OpenExpressionParamWindow();
                expressionInput.SetValue(variableItem.value);
                return;
            }

            if (variableItem.nodeType == PROJECT.NodeType.LIST)
            {
                currentSingleOrListType = 1;            // List Type.
                singleOrListDropdown.value = currentSingleOrListType;
                if (variableItem.dataType == PROJECT.DataType.BOOL)
                {
                    OpenBooleanListParamWindow();
                    booleanListInput.SetValue(variableItem.value);
                }

                else if (variableItem.dataType == PROJECT.DataType.FACIAL)
                {
                    OpenFaceListParamWindow();
                    faceListInput.SetValue(variableItem.value);
                }

                else if (variableItem.dataType == PROJECT.DataType.MOTION)
                {
                    OpenMotionListParamWindow();
                    motionListInput.SetValue(variableItem.value);
                }

                else if (variableItem.dataType == PROJECT.DataType.MOBILITY)
                {
                    OpenMobilityListParamWindow();
                    mobilityListInput.SetValue(variableItem.value);
                }

                else
                {
                    OpenListParamWindow();
                    valueListInput.SetValue(variableItem.value);
                    valueListInput.SetDataType(variableItem.dataType);
                }
            }

            else
            {
                currentSingleOrListType = 0;            // Single Variable Type.
                singleOrListDropdown.value = currentSingleOrListType;
                if (variableItem.dataType == PROJECT.DataType.BOOL)
                {
                    OpenBooleanParamWindow();
                    var toggle = booleanInput.GetComponentInChildren<Toggle>();
                    if (variableItem.value == "0" || variableItem.value == "false")
                    {
                        toggle.isOn = false;
                    }
                    else
                    {
                        toggle.isOn = true;
                    }
                    //booleanInput.GetComponentInChildren<Toggle>().isOn = variableItem.value == "0" ? false : true;
                }

                else if (variableItem.dataType == PROJECT.DataType.FACIAL)
                {
                    OpenFacialParamWindow();
                    faceInput.GetComponentInChildren<Dropdown>().value = Constants.GetFacialIndexFromEnglish(variableItem.value);
                }

                else if (variableItem.dataType == PROJECT.DataType.MOTION)
                {
                    OpenMotionParamWindow();
                    motionInput.GetComponentInChildren<Dropdown>().value = Constants.GetMotionIndexFromEnglish(variableItem.value);
                }

                else if (variableItem.dataType == PROJECT.DataType.MOBILITY)
                {
                    OpenMobilityParamWindow();
                    mobilityInput.GetComponentInChildren<Dropdown>().value = Constants.GetMobilityIndexFromEnglish(variableItem.value);
                }

                //else if (variableItem.type == PROJECT.DataType.EXPRESSION)
                //{
                //    OpenExpressionParamWindow();
                //    expressionInput.SetValue(variableItem.value);
                //}

                else
                {
                    OpenNormalParamWindow();
                    valueInput.SetDataType(variableItem.dataType);
                    valueInput.Input = variableItem.value;
                }
            }
        }

        public override void HidePopup()
        {
            base.HidePopup();

            nameText.text = string.Empty;
            OpenBooleanParamWindow();
            //OpenNormalParamWindow();
        }

        public PROJECT.DataType currentVariableType;
        public int currentSingleOrListType = 0;
        public void OnChangeType(int value)
        {
            currentVariableType = Constants.GetDataType(typeDropdown.options[value].text);

            bool show = currentVariableType != PROJECT.DataType.EXPRESSION;
            singleOrListDropdown.gameObject.SetActive(show);

            if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.SINGLE)
            {
                //OpenNormalParamWindow();
                OpenBooleanParamWindow();
                SetTargetSingleParamWindow();
                return;
            }

            else if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
            {
                //OpenListParamWindow();
                SetTargetListParamWindow();
                return;
            }
        }

        private void SetTargetListParamWindow()
        {
            if ((ESingleOrListType)currentSingleOrListType != ESingleOrListType.LIST)
                return;

            switch (currentVariableType)
            {
                case PROJECT.DataType.BOOL:
                    {
                        OpenBooleanListParamWindow();
                        break;
                    }
                case PROJECT.DataType.EXPRESSION:
                    {
                        OpenExpressionParamWindow();
                        break;
                    }
                case PROJECT.DataType.FACIAL:
                    {
                        OpenFaceListParamWindow();
                        break;
                    }
                case PROJECT.DataType.MOTION:
                    {
                        OpenMotionListParamWindow();
                        break;
                    }
                case PROJECT.DataType.MOBILITY:
                    {
                        OpenMobilityListParamWindow();
                        break;
                    }
                default:
                    {
                        OpenListParamWindow();
                        valueListInput.SetDataType(currentVariableType);
                        break;
                    }
            }
        }

        private void SetTargetSingleParamWindow()
        {
            if ((ESingleOrListType)currentSingleOrListType != ESingleOrListType.SINGLE)
                return;

            switch (currentVariableType)
            {
                case PROJECT.DataType.BOOL:
                    {
                        OpenBooleanParamWindow();
                        break;
                    }
                case PROJECT.DataType.EXPRESSION:
                    {
                        OpenExpressionParamWindow();
                        break;
                    }
                case PROJECT.DataType.FACIAL:
                    {
                        OpenFacialParamWindow();
                        break;
                    }
                case PROJECT.DataType.MOTION:
                    {
                        OpenMotionParamWindow();
                        break;
                    }
                case PROJECT.DataType.MOBILITY:
                    {
                        OpenMobilityParamWindow();
                        break;
                    }
                default:
                    {
                        OpenNormalParamWindow();
                        valueInput.SetDataType(currentVariableType);
                        break;
                    }
            }
        }

        public void OnSingleOrListChanged(ESingleOrListType type)
        {
            currentSingleOrListType = (int)type;
            switch (type)
            {
                case ESingleOrListType.SINGLE:
                    {
                        OnChangeType(typeDropdown.value);
                    }
                    break;

                case ESingleOrListType.LIST:
                    {
                        if (currentVariableType != PROJECT.DataType.EXPRESSION)
                        {
                            SetTargetListParamWindow();
                            //OpenListParamWindow();
                        }
                    }
                    break;
                default: break;
            }
        }

        private void SetDisableAllInputs()
        {
            valueInput.gameObject.SetActive(false);
            valueListInput.gameObject.SetActive(false);
            faceListInput.gameObject.SetActive(false);
            motionListInput.gameObject.SetActive(false);
            mobilityListInput.gameObject.SetActive(false);

            expressionInput.ResetValues();
            expressionInput.gameObject.SetActive(false);
            faceInput.gameObject.SetActive(false);
            motionInput.gameObject.SetActive(false);
            mobilityInput.gameObject.SetActive(false);

            booleanInput.SetActive(false);
            booleanListInput.ResetValues();
            booleanListInput.gameObject.SetActive(false);
        }

        private void OpenBooleanParamWindow()
        {
            SetTargetWindow(booleanInput.gameObject, normalHeight, inputBaseYPos);
        }

        private void OpenFacialParamWindow()
        {
            SetTargetWindow(faceInput.gameObject, facialOrMotionHeight, inputBaseYPos);
        }

        private void OpenMotionParamWindow()
        {
            SetTargetWindow(motionInput.gameObject, facialOrMotionHeight, inputBaseYPos);
        }

        private void OpenMobilityParamWindow()
        {
            SetTargetWindow(mobilityInput.gameObject, facialOrMotionHeight, inputBaseYPos);
        }

        private void OpenListParamWindow()
        {
            SetTargetWindow(valueListInput.gameObject, normalHeight);
        }

        private void OpenBooleanListParamWindow()
        {
            SetTargetWindow(booleanListInput.gameObject, normalHeight);
        }

        private void OpenFaceListParamWindow()
        {
            SetTargetWindow(faceListInput.gameObject, normalHeight);
        }

        private void OpenMotionListParamWindow()
        {
            SetTargetWindow(motionListInput.gameObject, normalHeight);
        }

        private void OpenMobilityListParamWindow()
        {
            SetTargetWindow(mobilityListInput.gameObject, normalHeight);
        }

        private void OpenExpressionParamWindow()
        {
            SetTargetWindow(expressionInput.gameObject, normalHeight);
        }

        private void OpenNormalParamWindow()
        {
            SetTargetWindow(valueInput.gameObject, normalHeight);
        }

        private void SetTargetWindow(GameObject target, float height, float yPos = 0f)
        {
            if (refRT == null)
            {
                return;
            }

            SetDisableAllInputs();
            target.SetActive(true);

            if (yPos != 0f)
            {
                Vector2 pos = target.transform.localPosition;
                pos.y = yPos;
                target.transform.localPosition = pos;
            }

            Vector2 size = refRT.sizeDelta;
            size.y = height;
            refRT.sizeDelta = size;
        }

        private string GetValue()
        {
            PROJECT.DataType type = Constants.GetDataType(typeDropdown.options[typeDropdown.value].text);
            switch (type)
            {
                case PROJECT.DataType.BOOL:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return booleanListInput.GetValue();
                        }
                        else
                        {
                            //return booleanInput.GetComponentInChildren<Toggle>().isOn == false ? "0" : "1";
                            return booleanInput.GetComponentInChildren<Toggle>().isOn == false ? "false" : "true";
                        }
                    }
                case PROJECT.DataType.NUMBER:
                case PROJECT.DataType.STRING:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return valueListInput.GetValue();
                        }
                        else
                        {
                            return valueInput.Input;
                        }
                    }

                // TODO : Should be changed to list.
                case PROJECT.DataType.EXPRESSION:
                    {
                        return expressionInput.GetValue();
                    }

                case PROJECT.DataType.FACIAL:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return faceListInput.GetValue();
                        }
                        else
                        {
                            return Constants.ParseFacialKoreanToEnglish(faceInput.GetComponentInChildren<Dropdown>().value);
                        }
                    }

                case PROJECT.DataType.MOTION:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return motionListInput.GetValue();
                        }
                        else
                        {
                            return Constants.ParseMotionKoreanToEnglish(motionInput.GetComponentInChildren<Dropdown>().value);
                        }
                    }

                case PROJECT.DataType.MOBILITY:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return mobilityListInput.GetValue();
                        }
                        else
                        {
                            return Constants.ParseMobilityKoreanToEnglish(mobilityInput.GetComponentInChildren<Dropdown>().value);
                        }
                    }
                default: return string.Empty;
            }
        }

        public void OnBooleanToggleChanged(bool isON)
        {
            TextMeshProUGUI text = booleanInput.GetComponentInChildren<TextMeshProUGUI>();
            text.text = GetBooleanStringValue(isON);
        }

        private string GetBooleanStringValue(bool isON)
        {
            if (isON == true)
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                {
                    return "True";
                }
                else
                {
                    return "참";
                }
            }
            else
            {
                if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.ENG)
                {
                    return "False";
                }
                else
                {
                    return "거짓";
                }
            }
        }

        public void SetVariableItem(LeftMenuVariableItem item)
        {
            variableItem = item;
        }

        public void SetIsLocalVariable(bool isLocalVariable, string functionName)
        {
            this.isLocalVariable = isLocalVariable;
            this.functionName = functionName;
        }
    }
}