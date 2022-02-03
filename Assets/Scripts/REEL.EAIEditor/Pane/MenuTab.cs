using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TabType = REEL.D2EEditor.MenuTabManager.TabType;

namespace REEL.D2EEditor
{
    public class MenuTab : MonoBehaviour
    {
        public TabType tabType;
        private MenuTabManager manager;

        public void OnTabClicked()
        {
            if (manager == null) return;
            manager.OnTabChanged(tabType);
        }

        public void SetManager(MenuTabManager manager)
        {
            this.manager = manager;
        }
    }
}