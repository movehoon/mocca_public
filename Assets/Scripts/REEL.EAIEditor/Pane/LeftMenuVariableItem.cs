using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class LeftMenuVariableItem : LeftMenuListItem, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public TMP_Text typeText;
        public TMP_Text valueText;

        public Button button;

        public Button removeButton;

        public PROJECT.NodeType nodeType;       // Variable 타입이거나 LIST 타입 설정.
        public PROJECT.DataType dataType;
        public string value;

        // Local 변수일 때 사용하는 프로퍼티.
        public bool isLocalVariable = false;
        public string functionName = "";

        private Image image;

        private MCPopup popup = null;

        public int VariableID { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (image == null)
            {
                image = GetComponent<Image>();
            }

            button.onClick.AddListener(OnVariableSelected);
            removeButton.onClick.AddListener(OnVariableRemoveClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.RemoveListener(OnVariableSelected);
            removeButton.onClick.RemoveListener(OnVariableRemoveClicked);
        }

        public void OnVariableSelected()
        {
            if (Utils.IsProjectNullOrOnSimulation)
            {
                return;
            }

            // 다른 팝업이 열려있으면 진행 안함.
            if (MCEditorManager.Instance.IsAnyPopupActive == true)
            {
                return;
            }

            MCVariableListPopup popup = GetPopup(MCEditorManager.PopupType.VariableList)
                .GetComponent<MCVariableListPopup>();

            //Utils.LogRed($"popup.IsOn: {popup.IsOn}");

            popup.SetVariableItem(this);
            popup.ShowPopup();
        }

        public void OnVariableRemoveClicked()
        {
            if (Utils.IsProjectNullOrOnSimulation)
            {
                return;
            }

            //Debug.Log("OnVariableRemoveClicked Clicked");
            MessageBox.ShowYesNo("[ID_MSG_WANT_DELETE_VARIABLE]변수를 삭제하시겠습니까?", (bool isYes) =>
            {
                if (isYes)
                {
                    //MCWorkspaceManager.Instance.RequestVariableDelete(this);

                    if (isLocalVariable == true)
                    {
                        MCDeleteLocalVariableCommand command = new MCDeleteLocalVariableCommand(this);
                        MCUndoRedoManager.Instance.AddCommand(command);
                    }
                    else
                    {
                        MCDeleteVariableCommand command = new MCDeleteVariableCommand(this);
                        MCUndoRedoManager.Instance.AddCommand(command);
                    }
                }
                else
                {
                    MessageBox.Close();
                }
            }); //local 추가완료
        }

        public void SetNodeType(PROJECT.NodeType type)
        {
            this.nodeType = type;
        }

        public void SetDataType(REEL.PROJECT.DataType type)
        {
            this.dataType = type;
            //typeText.text = type.ToString() + (this.nodeType == PROJECT.NodeType.LIST ? "LIST" : "");
            typeText.text = type.ToString().Substring(0, 3) + (this.nodeType == PROJECT.NodeType.LIST ? "LIST" : "");

            if (image == null)
            {
                image = GetComponent<Image>();
            }

            //Debug.Log($"DataType Check: {type}");
            image.color = Utils.GetParameterColor(type);
            removeButton.image.color = image.color;
        }

        public void SetValue(string value, string valueText = "")
        {
            this.value = value;
            //this.valueText.text = valueText == "" ? value : valueText;
            string parsedData = value;
            if (nodeType == PROJECT.NodeType.LIST && dataType != PROJECT.DataType.EXPRESSION)
            {
                PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(value);
                parsedData = listValue.listValue[0].Length > 3 ? listValue.listValue[0].Substring(0, 3) : listValue.listValue[0];
            }
            else
            {
                parsedData = parsedData.Length > 3 ? parsedData.Substring(0, 3) : parsedData;
            }

            this.valueText.text = valueText == "" ? parsedData : valueText.Substring(0, 3);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MCEditorManager.Instance.DragInfoSetActive(true);
            MCEditorManager.Instance.DragInfoSetSprite(null);
            MCEditorManager.Instance.DragInfoSetText(nameText.text);
            MCEditorManager.Instance.SetIsLocalVariable(isLocalVariable);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (MCEditorManager.Instance.DragInfoIsActive == false)
            {
                MCEditorManager.Instance.DragInfoSetActive(true);
            }

            MCEditorManager.Instance.DragInfoSetPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            if (MCEditorManager.Instance.IsOnGraphPane(results))
            {
                MCPopup popup = GetPopup(MCEditorManager.PopupType.VariableContext);
                popup.GetComponent<RectTransform>().position = eventData.position;
                popup.ShowPopup();
            }

            MCEditorManager.Instance.DragInfoSetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (popup == null)
            {
                popup = GetPopup(MCEditorManager.PopupType.VariableList);
            }

            // 다른 팝업이 열려있으면 진행 안함.
            if (MCEditorManager.Instance.IsAnyPopupActive == true)
            {
                return;
            }

            //Utils.LogRed($"[OnPointerDown]");
            //GetPopup(MCEditorManager.PopupType.VariableContext)?.HidePopup();
            //GetPopup(MCEditorManager.PopupType.FunctionList)?.HidePopup();
            popup.HideAllPopupWithout(MCEditorManager.PopupType.VariableList);
        }
    }
}