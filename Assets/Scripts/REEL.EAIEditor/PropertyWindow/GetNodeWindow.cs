#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class GetNodeWindow : GetSetNodeWindow
    {
#if USINGTMPPRO
        [SerializeField] private TMP_Dropdown getVarDropdown;
        [SerializeField] private TMP_Text variableType;
        [SerializeField] private TMP_Text variableName;
#else
        [SerializeField] private Dropdown getVarDropdown;
        [SerializeField] private Text variableType;
        [SerializeField] private Text variableName;
#endif

        private void SetDropdownReference()
        {
#if USINGTMPPRO
            getVarDropdown = refNode.GetComponentInChildren<TMP_Dropdown>();
#else
            getVarDropdown = refNode.GetComponentInChildren<Dropdown>();
#endif
        }

        protected override void UpdateProperty()
        {
            base.UpdateProperty();

            SetDropdownReference();

            MCGetNode getNode = refNode as MCGetNode;
            if (getNode == null)
            {
                return;
            }

            LeftMenuVariableItem variable;
            if (getNode.IsLocalVariable == true)
            {
                variable = MCWorkspaceManager.Instance.GetLocalVariable(getVarDropdown.value);
            }
            else
            {
                variable = MCWorkspaceManager.Instance.GetVariable(getVarDropdown.value);
            }

            variableType.text = variable.dataType.ToString();
            variableName.text = getVarDropdown.options[getVarDropdown.value].text;

            if (variable.nodeType == PROJECT.NodeType.VARIABLE)
            {
                currentVarCount = 1;
                SetBlocks(variable.nodeType, currentVarCount);
                valueBlocks[0].value.text = Constants.GetValueForKorean(variable.value, variable.dataType);
            }
            else if (variable.nodeType == PROJECT.NodeType.LIST)
            {
                PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(variable.value);

                currentVarCount = listValue.listValue.Length;
                SetBlocks(variable.nodeType, currentVarCount);

                for (int i = 0; i < currentVarCount; i++)
                    valueBlocks[i].value.text = Constants.GetValueForKorean(listValue.listValue[i], variable.dataType);
            }
            else if (variable.nodeType == PROJECT.NodeType.EXPRESSION)
            {
                // JSON 파싱 ( EXPRESSION 배열 )
                PROJECT.Expression[] expArray = JsonConvert.DeserializeObject<PROJECT.Expression[]>(variable.value);
                currentVarCount = expArray.Length;
                SetBlocks(variable.nodeType, currentVarCount);

                for (int i = 0; i < expArray.Length; i++)
                {
                    expressionBlocks[i].tts.text = expArray[i].tts;
                    expressionBlocks[i].facial.text = Constants.GetValueForKorean(expArray[i].facial, PROJECT.DataType.FACIAL);
                    expressionBlocks[i].motion.text = Constants.GetValueForKorean(expArray[i].motion, PROJECT.DataType.MOTION);
                }
            }

            if (variable.nodeType == PROJECT.NodeType.EXPRESSION)
            {
                scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + currentVarCount * itemHeight * 4 + itemHeight);
            }
            else
            {
                scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + (currentVarCount + 1) * itemHeight);
            }
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            UpdateProperty();
        }
    }
}