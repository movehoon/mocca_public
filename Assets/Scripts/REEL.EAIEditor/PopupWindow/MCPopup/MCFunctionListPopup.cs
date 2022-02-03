using TMPro;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;

namespace REEL.D2EEditor
{
    public class MCFunctionListPopup : MCPopup
    {
        [Header("함수 팝업창 UI 참조 객체")]
        public TMP_InputField nameText;
        public TMP_InputField descText;

        public MCFunctionInputList functionInput;
        public MCFunctionOutputList functionOutput;

        [Header("함수 팝업 창 위치 조절 값")]
        public float popupOriginYsize = 235f;
        public float blockHeight = 45f;
        public float outputListOriginYPos = -135f;

        [SerializeField]
        private LeftMenuFunctionItem functionItem;

        public string FunctionName { get { return nameText.text; } }
        public string Description { get { return descText.text; } }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void OnDisable()
        {
            functionItem = null;
        }

        public override void OnOKClicked()
        {
            if (functionItem is null)
            {
                // 함수 이름을 설정 안한 경우 함수 이름 설정.
                if (IsStringNullOrEmptyOrWhiteSpace(FunctionName))
                {
                    //ResetAllColumns();
                    //base.OnOKClicked();

                    //LogWindow.Instance.PrintWarning("MOCCA", "함수 이름을 입력해주세요.");
                    MessageBox.Show("[ID_MSG_ENTER_FUNCTION_NAME]함수 이름을 입력해주세요."); // local 추가 완료
                    return;
                }

                if (MCFunctionTable.Instance.CanAddFunction(FunctionName) == false)
                {
                    MessageBox.Show("[ID_SAME_FUNCTION]같은(동일한) 이름을 가진 함수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료
                    return;
                }

                #region 주석처리
                //if (Utils.IsNullOrEmptyOrWhiteSpace(Description))
                //{
                //    //base.OnOKClicked();

                //    MessageBox.Show("[ID_MSG_ENTER_FUNCTION_DESCRIPTION]함수 설명을 입력해주세요."); // local 추가 완료
                //    return;
                //}
                #endregion

                MCWorkspaceManager.Instance.AddFunction(FunctionName, functionInput.Inputs, functionOutput.Outputs, Description);
            }
            else
            {
                // 삭제/추가/변경된 입력/출력 파라미터 내용 Queue에 추가.
                //PushFunctionDataChangedToQueue();

                //Utils.LogRed("here");
                if (MCFunctionTable.Instance.CanAddFunction(FunctionName, functionItem) == false)
                {
                    MessageBox.Show("[ID_SAME_FUNCTION]같은(동일한) 이름을 가진 함수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료
                    return;
                }

                functionItem.SetName(FunctionName);
                functionItem.FunctionData.description = Description;
                functionItem.SetInputOuput(functionInput.Inputs, functionOutput.Outputs, false);

                // 함수 업데이트 호출.
                MCWorkspaceManager.Instance.UpdateFunction(functionItem.FunctionData);

                //MCWorkspaceManager.Instance.DelayValidateProject();
            }

            ResetAllColumns();
            base.OnOKClicked();
        }

        private bool IsStringNullOrEmptyOrWhiteSpace(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }
        public override void OnCancelClicked()
        {
            ResetAllColumns();
            base.OnCancelClicked();
        }

        public override void ShowPopup(MCNode node = null)
        {
            ResetAllColumns();

            base.HidePopup();
            base.ShowPopup(node);

            if (functionItem == null)
            {
                return;
            }

            nameText.text = functionItem.FunctionData.name;
            descText.text = functionItem.FunctionData.description;

            functionInput.SetInputs(functionItem.Inputs);

            SetOutputRootYPosition();

            functionOutput.SetOutputs(functionItem.Outputs);
        }

        public void UpdatePopupSize()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            int inputOutputCount = functionInput.Inputs.Length + functionOutput.Outputs.Length;
            float height = blockHeight * inputOutputCount + popupOriginYsize;
            refRT.sizeDelta = new Vector2(refRT.sizeDelta.x, height);
        }

        public override void HidePopup()
        {
            base.HidePopup();
            ResetAllColumns();
        }

        public override void SetTargetNode(MCNode node)
        {
            base.SetTargetNode(node);
        }

        public void OnInputAddClicked()
        {
            functionInput.OnAddClicked();
            SetOutputRootYPosition();
        }

        private void SetOutputRootYPosition()
        {
            float outputYPos = functionInput.ActiveItems.Count * blockHeight;
            functionOutput.GetComponent<RectTransform>().anchoredPosition
                = new Vector2(0f, outputListOriginYPos - outputYPos);
        }

        public void OnInputDeleteAllClicked()
        {
            ResetAllInputColumns();
        }

        private void ResetAllInputColumns()
        {
            List<MCFunctionInputListItem> activeItems = functionInput.ActiveItems;
            for (int ix = activeItems.Count - 1; ix >= 0; --ix)
            {
                var item = activeItems[ix];
                functionInput.OnMinusClicked(item);
            }

            functionOutput.GetComponent<RectTransform>().anchoredPosition
                = new Vector2(0f, outputListOriginYPos);
        }

        public void OnInputMinusClicked()
        {
            float outputYPos = functionInput.ActiveItems.Count * blockHeight;
            functionOutput.GetComponent<RectTransform>().anchoredPosition
                = new Vector2(0f, outputListOriginYPos - outputYPos);
        }

        public void OnOutputAddClicked()
        {
            functionOutput.OnAddClicked();
        }

        public void OnOutputDeleteAllClicked()
        {
            ResetAllOutputColumns();
        }

        private void ResetAllOutputColumns()
        {
            List<MCFunctionOutputListItem> activeItems = functionOutput.ActiveItems;
            for (int ix = activeItems.Count - 1; ix >= 0; --ix)
            {
                var item = activeItems[ix];
                functionOutput.OnMinusClicked(item);
            }
        }

        private void ResetAllColumns()
        {
            ResetAllInputColumns();
            ResetAllOutputColumns();
            nameText.text = string.Empty;
            descText.text = string.Empty;

            UpdatePopupSize();
        }

        public void SetFunctionItem(LeftMenuFunctionItem item)
        {
            functionItem = item;
        }
    }
}