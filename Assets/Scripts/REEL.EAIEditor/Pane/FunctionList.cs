using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class FunctionList : LeftMenuList
    {
        public override void AddItem()
        {
            //base.AddItem();
            if (IsProjectNullOrOnSimulation)
            {
                return;
            }

            MCFunctionListPopup popup = MCEditorManager.Instance
                .GetPopup(MCEditorManager.PopupType.FunctionList)
                .GetComponent<MCFunctionListPopup>();

            // 함수 팝업 창이 닫혔을 때만 창 열도록 처리.
            // 다른 함수를 편집 중일 때는 창이 열려있음.
            if (popup.IsOn == false)
            {
                popup.HideAllPopups();
                popup.SetFunctionItem(null);
                popup.ShowPopup();
            }
        }

        public void CollapseAsFunction()
        {
            if (IsProjectNullOrOnSimulation || MCWorkspaceManager.Instance.CurrentSelectedBlockCount == 0)
            {
                return;
            }

            Utils.LogRed("[CollapseAsFunction]");
        }
    }
}