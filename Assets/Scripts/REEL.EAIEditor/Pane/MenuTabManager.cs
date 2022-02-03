using UnityEngine;

namespace REEL.D2EEditor
{
    public class MenuTabManager : MonoBehaviour
    {
        public enum TabType
        {
            Block, VarFunc, Robot, Cam, Robot2, Property, None
        }

        public MenuTab[] tabs;
        public GameObject[] tabMenu;

        //private int currentTabIndex = 0;
        private TabType currentTabType = TabType.None;

        //private void Awake()
        //{
            
        //}

        private void OnEnable()
        {
            foreach (MenuTab tab in tabs)
            {
                tab.SetManager(this);
            }
        }

        public void OnTabChanged(TabType tabType)
        {
            if (tabType == TabType.None || tabType.Equals(currentTabType))
            {
                return;
            }

            for (int ix = 0; ix < tabs.Length; ++ix)
            {
                if (tabs[ix].tabType.Equals(tabType))
                {
                    tabMenu[ix].SetActive(true);
                }
                else
                {
                    tabMenu[ix].SetActive(false);
                }
            }

            currentTabType = tabType;

            //tabMenu[currentTabIndex].SetActive(false);
            //tabMenu[(int)tabType].SetActive(true);

            //currentTabIndex = (int)tabType;
        }
    }
}