using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCTooltipManager : Singleton<MCTooltipManager>
    {
		static public bool Enabled = true;

        public MCTooltipWindow tooltipWindow;
        public MCVariableDebuggerWindow variableDebuggerWindow;
        public RectTransform menuPane;
        public List<MCNodeTooltipData> tooltips = new List<MCNodeTooltipData>();

        private Dictionary<PROJECT.NodeType, MCNodeTooltipData> tooltipData = new Dictionary<PROJECT.NodeType, MCNodeTooltipData>();

        private void OnEnable()
        {
            InitDictionary();
        }

        private void InitDictionary()
        {
            foreach (var tooltip in tooltips)
            {
                tooltipData.Add(tooltip.nodeType, tooltip);
            }
        }


		bool canShowPopup = true;

		public void CanShowPopup(bool flgShow)
		{
			canShowPopup = flgShow;

			if(canShowPopup == false )
			{
				CloseTooltip();
			}
		}

        public void ShowTooltip(MCNode node)
        {
			if (canShowPopup == false) return;
			if (Enabled == false) return;

            if (tooltipData.TryGetValue(node.nodeData.type, out MCNodeTooltipData data))
            {
                // 툴팁 데이터 설정.
                tooltipWindow.SetIcon(data.tooltipImageSprite);
                tooltipWindow.SetNodeNameText(data.nodeName);
                tooltipWindow.SetInputText(data.inputTexts[0]);
                tooltipWindow.SetOutputText(data.outputTexts[0]);
                tooltipWindow.SetDescText(data.descTexts);

                // 툴팁 창 켜기.
                //tooltipWindow.ShowWindow(node.transform.position);

                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, node.transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    menuPane, screenPosition, null, out Vector2 localPoint);

                tooltipWindow.ShowWindow(screenPosition);
            }
        }

        public void ShowTooltip(NodeListItemComponent nodeListItem)
        {
            if (canShowPopup == false) return;
            if (Enabled == false) return;

            if (tooltipData.TryGetValue(nodeListItem.nodeType, out MCNodeTooltipData data))
            {
                // 툴팁 데이터 설정.
                tooltipWindow.SetIcon(data.tooltipImageSprite);
                tooltipWindow.SetNodeNameText(data.nodeName);
                tooltipWindow.SetInputText(data.inputTexts[0]);
                tooltipWindow.SetOutputText(data.outputTexts[0]);
                tooltipWindow.SetDescText(data.descTexts);

                // 툴팁 창 켜기.
                tooltipWindow.ShowWindow(nodeListItem.transform.position);
            }
        }

        public void ShowTooltip(LeftMenuVariableItem variableItem)
        {
            if (canShowPopup == false) return;
            //if (Enabled == false) return;

            //if (MCWorkspaceManager.Instance.IsSimulation == true)
            if (MCPlayStateManager.Instance.IsSimulation == true)
            {
                //Utils.LogRed("Variable ShowTooltip");
                variableDebuggerWindow.SetVariable(variableItem);
                string variableName = variableItem.VariableName;
                if (variableItem.nodeType == PROJECT.NodeType.VARIABLE)
                {   
                    if (variableItem.dataType == PROJECT.DataType.BOOL)
                    {
                        if (Player.Instance.Variables_bool.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_bool, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_bool[variableName]);
                        }
                    }

                    else if (variableItem.dataType == PROJECT.DataType.NUMBER)
                    {
                        if (Player.Instance.Variables_number.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_number, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_number[variableName]);
                        }
                    }

                    else if (variableItem.dataType == PROJECT.DataType.STRING)
                    {
                        if (Player.Instance.Variables_string.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_string, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_string[variableName]);
                        }
                    }

                    else if (variableItem.dataType == PROJECT.DataType.FACIAL)
                    {
                        if (Player.Instance.Variables_facial.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_facial, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_facial[variableName]);
                        }
                    }

                    else if (variableItem.dataType == PROJECT.DataType.MOTION)
                    {
                        if (Player.Instance.Variables_motion.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_motion, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_motion[variableName]);
                        }
                    }

                    else if (variableItem.dataType == PROJECT.DataType.MOBILITY)
                    {
                        if (Player.Instance.Variables_mobility.ContainsKey(variableName) == true)
                        {
                            variableDebuggerWindow.SetData(variableName, Player.Instance.Variables_mobility, variableItem.dataType);
                            //Utils.LogBlue(Player.Instance.Variables_mobility[variableName]);
                        }
                    }
                }

                else if (variableItem.nodeType == PROJECT.NodeType.LIST)
                {
                    if (variableItem.dataType == PROJECT.DataType.BOOL)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_bool, variableItem.dataType);
                    }

                    else if (variableItem.dataType == PROJECT.DataType.NUMBER)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_number, variableItem.dataType);
                    }

                    else if (variableItem.dataType == PROJECT.DataType.STRING)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_string, variableItem.dataType);
                    }

                    else if (variableItem.dataType == PROJECT.DataType.FACIAL)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_facial, variableItem.dataType);
                    }

                    else if (variableItem.dataType == PROJECT.DataType.MOTION)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_motion, variableItem.dataType);
                    }

                    else if (variableItem.dataType == PROJECT.DataType.MOBILITY)
                    {
                        variableDebuggerWindow.SetData(variableName, Player.Instance.Lists_mobility, variableItem.dataType);
                    }
                }

                else if (variableItem.nodeType == PROJECT.NodeType.EXPRESSION)
                {
                    variableDebuggerWindow.SetData(variableName, Player.Instance.Expressions);
                }

                // 디버거 창 열기.
                //variableDebuggerWindow.ShowWindow(variableItem.transform.position);
                variableDebuggerWindow.ShowWindow(Input.mousePosition);
            }
        }

        public void CloseTooltip()
        {
            tooltipWindow.CloseWindow();
            variableDebuggerWindow.CloseWindow();
        }
    }
}