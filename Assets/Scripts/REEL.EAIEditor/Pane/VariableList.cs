using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class VariableList : LeftMenuList
    {
        private MCVariableListPopup popup = null;

        public override void AddItem()
        {
            if (IsProjectNullOrOnSimulation)
            {
                return;
            }

            if (popup == null)
            {
                popup = MCEditorManager.Instance
                    .GetPopup(MCEditorManager.PopupType.VariableList)
                    .GetComponent<MCVariableListPopup>();
            }

            // 팝업이 닫혀있을 때만 창이 열리도록 처리.
            // 다른 변수가 편집 중일 때는 창이 열려있기 때문에 이 때는 변수 추가 안되도록.
            if (popup.IsOn == false)
            {
                popup.HideAllPopups();
                popup.SetVariableItem(null);
                popup.ShowPopup();
            }
        }

        public void AddLocalVariableItem()
        {
            if (IsProjectNullOrOnSimulation)
            {
                return;
            }

            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                return;
            }

            if (popup == null)
            {
                popup = MCEditorManager.Instance
                    .GetPopup(MCEditorManager.PopupType.VariableList)
                    .GetComponent<MCVariableListPopup>();
            }

            popup.SetVariableItem(null);
            popup.SetIsLocalVariable(true, Utils.CurrentTabName);
            popup.ShowPopup();
        }
    }
}