#define USINGTMPPRO

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if USINGTMPPRO
using TMPro;
#endif

namespace REEL.D2EEditor
{
	public class MCVariableDebuggerWindow : MonoBehaviour
	{
#if USINGTMPPRO
        public TMP_Text variableNameText;
#else
        public Text variableNameText;
#endif
        public Transform listValueParent;

        private string variableValuePrefabName = "VariableValue";

        private List<GameObject> valueOrListValuesCreated = new List<GameObject>();

        private RectTransform refRT;
        private Vector2 windowSize;

        private float itemHeight = 30f;
        private float itemLineSpace = 2f;

        private Text ValueOrListValueText
        {
            get
            {
                GameObject valueGO = ObjectPool.Instance.PopFromPool(variableValuePrefabName, listValueParent);
                valueGO.transform.localScale = Vector3.one;
                valueGO.SetActive(true);
                valueOrListValuesCreated.Add(valueGO);
                currentValueText = valueGO.GetComponent<Text>();
                return currentValueText;
            }
        }

        private LeftMenuVariableItem currentVariableItem = null;
        private float updateInterval = 0.2f;
        private float elapsedTime = 0f;

        private void OnEnable()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
                windowSize = refRT.sizeDelta;
            }

            elapsedTime = 0f;
        }

        private void OnDisable()
        {
            elapsedTime = 0f;
            currentVariableItem = null;
            currentValueText = null;
        }

        private void Update()
        {
            //if (MCWorkspaceManager.Instance.IsSimulation == false)
            if (MCPlayStateManager.Instance.IsSimulation == false)
            {
                CloseWindow();
                ResetInfomation();
                return;
            }

            if (elapsedTime > updateInterval)
            {
                elapsedTime = 0f;
                UpdateValue();
                return;
            }

            elapsedTime += Time.deltaTime;
        }

        private void SetVariableName(string variableName, string dataType)
        {
            variableNameText.text = string.Format("{0} | {1}", variableName, dataType);
        }

        private void UpdateValue()
        {
            if (currentVariableItem == null)
            {
                return;
            }

            string variableName = currentVariableItem.VariableName;
            if (currentVariableItem.nodeType == PROJECT.NodeType.VARIABLE)
            {
                if (currentVariableItem.dataType == PROJECT.DataType.BOOL)
                {
                    if (Player.Instance.Variables_bool.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_bool, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_bool[variableName]);
                    }
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.NUMBER)
                {
                    if (Player.Instance.Variables_number.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_number, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_number[variableName]);
                    }
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.STRING)
                {
                    if (Player.Instance.Variables_string.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_string, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_string[variableName]);
                    }
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.FACIAL)
                {
                    if (Player.Instance.Variables_facial.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_facial, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_facial[variableName]);
                    }
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.MOTION)
                {
                    if (Player.Instance.Variables_motion.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_motion, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_motion[variableName]);
                    }
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.MOBILITY)
                {
                    if (Player.Instance.Variables_mobility.ContainsKey(variableName) == true)
                    {
                        SetData(variableName, Player.Instance.Variables_mobility, currentVariableItem.dataType);
                        //Utils.LogBlue(Player.Instance.Variables_mobility[variableName]);
                    }
                }
            }

            else if (currentVariableItem.nodeType == PROJECT.NodeType.LIST)
            {
                if (currentVariableItem.dataType == PROJECT.DataType.BOOL)
                {
                    SetData(variableName, Player.Instance.Lists_bool, currentVariableItem.dataType);
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.NUMBER)
                {
                    SetData(variableName, Player.Instance.Lists_number, currentVariableItem.dataType);
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.STRING)
                {
                    SetData(variableName, Player.Instance.Lists_string, currentVariableItem.dataType);
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.FACIAL)
                {
                    SetData(variableName, Player.Instance.Lists_facial, currentVariableItem.dataType);
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.MOTION)
                {
                    SetData(variableName, Player.Instance.Lists_motion, currentVariableItem.dataType);
                }

                else if (currentVariableItem.dataType == PROJECT.DataType.MOBILITY)
                {
                    SetData(variableName, Player.Instance.Lists_mobility, currentVariableItem.dataType);
                }
            }

            else if (currentVariableItem.nodeType == PROJECT.NodeType.EXPRESSION)
            {
                SetData(variableName, Player.Instance.Expressions);
            }
        }

        public void SetVariable(LeftMenuVariableItem variableItem)
        {
            currentVariableItem = variableItem;
        }

        private Text currentValueText = null;
        public void SetData(string variableName, Dictionary<string, string> values, PROJECT.DataType dataType)
        {
            SetVariableName(variableName, dataType.ToString());
            float textWidth = variableNameText.preferredWidth;
            if (values.ContainsKey(variableName))
            {
                Text valueText = currentValueText == null ? ValueOrListValueText : currentValueText;
                valueText.text = $"Value | {values[variableName]}";
                valueText.transform.SetAsLastSibling();

                textWidth = valueText.preferredWidth > textWidth ? valueText.preferredWidth : textWidth;
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            refRT.sizeDelta = new Vector2(textWidth + 10f, itemHeight * 2 + itemLineSpace);
        }

        public void SetData(string variableName, Dictionary<string, List<string>> listValues, PROJECT.DataType dataType)
        {
            SetVariableName(variableName, string.Format("{0}[{1}]", dataType.ToString(), listValues[variableName].Count));
            float maxWidth = variableNameText.preferredWidth;
            if (listValues.ContainsKey(variableName))
            {
                for (int ix = 0; ix < listValues[variableName].Count; ++ix)
                {
                    Text listValueText = currentValueText == null ? ValueOrListValueText : currentValueText;
                    listValueText.text = $"Value[{ix}] | {listValues[variableName][ix]}";
                    listValueText.transform.SetAsLastSibling();

                    maxWidth = listValueText.preferredWidth > maxWidth ? listValueText.preferredWidth : maxWidth;
                }
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            refRT.sizeDelta = new Vector2(maxWidth + 10f, itemHeight * (listValues[variableName].Count + 1)
                + itemLineSpace * (listValues[variableName].Count));
        }

        public void SetData(string variableName, Dictionary<string, List<PROJECT.Expression>> expressionValues)
        {
            SetVariableName(variableName, PROJECT.DataType.EXPRESSION.ToString());
            float maxWidth = variableNameText.preferredWidth;
            for (int ix = 0; ix < expressionValues[variableName].Count; ++ ix)
            {
                PROJECT.Expression expression = expressionValues[variableName][ix];

                Text listValueText = currentValueText == null ? ValueOrListValueText : currentValueText;
                listValueText.text = $"Value[{ix}] | tts: {expression.tts}, facial: {expression.facial}, motion: {expression.motion}";
                listValueText.transform.SetAsLastSibling();

                maxWidth = listValueText.preferredWidth > maxWidth ? listValueText.preferredWidth : maxWidth;
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            refRT.sizeDelta = new Vector2(maxWidth + 10f, 
                itemHeight * (expressionValues[variableName].Count + 1) 
                + itemLineSpace * (expressionValues[variableName].Count));
        }

        public void ShowWindow(Vector3 position)
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            float offset = 15f;
            refRT.position = new Vector3(position.x + offset, position.y, 0f);
            gameObject.SetActive(true);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);

            ResetInfomation();
        }

        private void ResetInfomation()
        {
            variableNameText.text = string.Empty;
            foreach (GameObject go in valueOrListValuesCreated)
            {
                ObjectPool.Instance.PushToPool(variableValuePrefabName, go);
            }

            valueOrListValuesCreated.Clear();
        }
    }
}