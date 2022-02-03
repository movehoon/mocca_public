using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class LocalVariableList : LeftMenuList
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

            popup.SetVariableItem(null);
            popup.ShowPopup();
        }
    }
}