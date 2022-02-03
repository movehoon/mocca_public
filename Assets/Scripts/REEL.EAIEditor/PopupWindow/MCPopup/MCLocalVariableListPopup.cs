using TMPro;

using UnityEngine;
using UnityEngine.UI;
using ESingleOrListType = REEL.D2EEditor.MCVariableListTypeDropdown.ESingleOrListType;

namespace REEL.D2EEditor
{
    public class MCLocalVariableListPopup : MCPopup
    {
        [SerializeField]
        private TMP_InputField nameText;

        [SerializeField]
        private Dropdown typeDropdown;

        [SerializeField]
        private Dropdown singleOrListDropdown;


        [SerializeField]
        private TMP_InputField valueText;

        [SerializeField]
        private GameObject valueInput;

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
        private GameObject faceInput;

        [SerializeField]
        private GameObject motionInput;

        [SerializeField]
        private GameObject mobilityInput;

        private float normalHeight = 190f;
        private float facialOrMotionHeight = 190f;
        private float expressionHeight = 280f;

        private float inputBaseYPos = -90f;

        private float blockHeight = 45f;

        [SerializeField]
        private LeftMenuVariableItem variableItem;

        private void OnDisable()
        {
            variableItem = null;
        }

        public override void OnOKClicked()
        {
            if (string.IsNullOrEmpty(nameText.text)
                || string.IsNullOrWhiteSpace(nameText.text))
            {
                //Utils.LogRed("Must enter a varabble name");
                MessageBox.Show("[ID_MSG_ENTER_VARIABLE_NAME]변수 이름을 입력해주세요."); // local 추가 완료
                //HidePopup();
                return;
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

            if (variableItem != null)
            {
                variableItem.SetName(nameText.text);
                variableItem.SetNodeType(finalNodeType);
                variableItem.SetDataType(dataType);
                variableItem.SetValue(GetValue(), dataType == PROJECT.DataType.EXPRESSION ? "EXPRESSION" : "");

                MCWorkspaceManager.Instance.UpdateVariable();
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
            valueText.text = string.Empty;

            expressionInput.ResetValues();
            faceInput.GetComponentInChildren<Dropdown>().value = 0;
            motionInput.GetComponentInChildren<Dropdown>().value = 0;
            mobilityInput.GetComponentInChildren<Dropdown>().value = 0;
        }

        public override void ShowPopup(MCNode node = null)
        {
            base.ShowPopup(node);

            if (variableItem == null)
                return;

            nameText.text = variableItem.VariableName;
            typeDropdown.value = Constants.ConvertDataTypeIndexToVariableTypeIndex(variableItem.dataType);

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
                if (variableItem.dataType == PROJECT.DataType.FACIAL)
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
                }
            }

            else
            {
                currentSingleOrListType = 0;            // Single Variable Type.
                singleOrListDropdown.value = currentSingleOrListType;
                if (variableItem.dataType == PROJECT.DataType.FACIAL)
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
                    valueText.text = variableItem.value;
                }
            }
        }

        public override void HidePopup()
        {
            base.HidePopup();

            nameText.text = string.Empty;
            OpenNormalParamWindow();
        }

        public PROJECT.DataType currentVariableType;
        public int currentSingleOrListType = 0;
        public void OnChangeType(int value)
        {
            currentVariableType = Constants.GetDataType(typeDropdown.options[value].text);
            if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.SINGLE)
            {
                OpenNormalParamWindow();
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
            valueInput.SetActive(false);
            valueListInput.gameObject.SetActive(false);
            faceListInput.gameObject.SetActive(false);
            motionListInput.gameObject.SetActive(false);
            mobilityListInput.gameObject.SetActive(false);

            expressionInput.ResetValues();
            expressionInput.gameObject.SetActive(false);
            faceInput.gameObject.SetActive(false);
            motionInput.gameObject.SetActive(false);
            mobilityInput.gameObject.SetActive(false);
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
            SetTargetWindow(valueInput, normalHeight);
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
                case PROJECT.DataType.NUMBER:
                case PROJECT.DataType.STRING:
                    {
                        if ((ESingleOrListType)currentSingleOrListType == ESingleOrListType.LIST)
                        {
                            return valueListInput.GetValue();
                        }
                        else
                        {
                            return valueText.text;
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

        public void SetVariableItem(LeftMenuVariableItem item)
        {
            variableItem = item;
        }
    }
}