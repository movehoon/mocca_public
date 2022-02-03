using REEL.D2E;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    using OwnerGroup = Constants.OwnerGroup;

    public class TabManager : MonoBehaviour
    {
        public enum TabAddType { New, Load }
        //public enum TabType { PROJECT, FUNCTION, None }

        public RectTransform tabContainerRect;
        public float tabWidth = 200f;

        [SerializeField] public List<TabComponent> currentTabs = new List<TabComponent>();
        [SerializeField] private float tabOffset = 15f;
        [SerializeField] private RectTransform newTabComponent = null;

        [SerializeField] private int selectedTabIndex = 0;
        [SerializeField] private int prevSelectedTabIndex = 0;
        [SerializeField] private int maxTabCount = 8;
        [SerializeField] private string projectTabItemName = "PROJECTtab";
        [SerializeField] private string functionTabItemName = "FUNCTIONtab";

        private Action OnTabChanged;

        private void Start()
        {
            SubscribeOnTabChanged(UpdateTabcontainerWidth);
            OnTabChanged?.Invoke();
        }

        public void CreateTab(string tabName, OwnerGroup OwnerGroup = OwnerGroup.PROJECT)
        {
            switch (OwnerGroup)
            {
                case OwnerGroup.PROJECT:
                    {
                        AddProjectTab(tabName, TabAddType.New);
                    }
                    break;
                case OwnerGroup.FUNCTION:
                    {
                        // 함수 탭이 열리면 Undo 기록 모두 삭제.
                        MCUndoRedoManager.Instance.ResetAllRecord();

                        // 현재 열려있는 함수 탭이 없거나,
                        // 같은 이름의 함수 탭이 없는 경우에는 함수 탭 추가.
                        if (FunctionTabs.Count == 0 || IsFunctionTabOpened(tabName) == false)
                        {
                            AddFunctionTab(tabName, TabAddType.New);
                            MCWorkspaceManager.Instance.AddFunctionEntryNode(CurrentTab.GetComponent<MCFunctionTab>());
                            MCWorkspaceManager.Instance.ResetWorkspaceState();
                        }
                        else
                        {
                            // 위의 경우가 아니라면 -> 함수 탭이 열려있는 상태라는 의미.
                            // 전달 받은 함수 탭 이름으로 탭을 검색한 후 상태 변경.
                            MCFunctionTab tab = GetFunctionTabWithName(tabName);
                            if (tab != null)
                            {
                                ChangeTabState(tab.TabID);
                            }
                        }

                        #region 기존 코드 백업
                        //foreach (TabComponent tab in FunctionTabs)
                        //{
                        //    Debug.LogWarning($"{tab.TabName} : {tabName} : {tab.TabName.Equals(tabName)}");

                        //    if (tab.TabName.Equals(tabName))
                        //    {
                        //        ChangeTabState(tab.TabID);
                        //    }
                        //    else
                        //    {
                        //        AddFunctionTab(tabName, TabAddType.New);
                        //    }
                        //}
                        #endregion
                    }
                    break;
            }
        }

        public MCFunctionTab GetFunctionTabWithName(string tabName)
        {
            foreach (TabComponent tab in FunctionTabs)
            {
                if (tab.TabName.Equals(tabName))
                {
                    return tab as MCFunctionTab;
                }
            }

            return null;
        }

        bool IsFunctionTabOpened(string tabName)
        {
            foreach (TabComponent tab in FunctionTabs)
            {
                if (tab.TabName.Equals(tabName))
                {
                    return true;
                }
            }

            return false;
        }

        List<TabComponent> FunctionTabs
        {
            get
            {
                List<TabComponent> retList = new List<TabComponent>();
                foreach (TabComponent tab in currentTabs)
                {
                    if (tab is MCFunctionTab)
                    {
                        retList.Add(tab);
                    }
                }

                return retList;
            }
        }

        public void AddFunctionTab(string tabName, TabAddType addType)
        {
            if (!CanAddTab)
            {
                return;
            }

            switch (addType)
            {
                case TabAddType.New:
                    {
                        currentTabs.Add(CreateNewFunctionTab(tabName));
                        ChangeTabState(currentTabs.Count - 1);
                        RearrangeTabsPosition();

                        return;
                    }
                case TabAddType.Load:
                    {
                        currentTabs.Add(LoadFunctionTab(tabName));
                        MCWorkspaceManager.Instance.ResetWorkspaceState();

                        ChangeTabState(GetTabIndex(tabName));
                        RearrangeTabsPosition();

                        return;
                    }
                default: break;
            }

            ChangeTabState(currentTabs.Count - 1);
            RearrangeTabsPosition();
        }

        public void AddProjectTab(string tabName, TabAddType addType)
        {
            if (!CanAddTab)
            {
                return;
            }

            switch (addType)
            {
                case TabAddType.New:
                    {
                        currentTabs.Add(CreateNewProjectTab(tabName));

                        ChangeTabState(currentTabs.Count - 1);

                        // Test. Add Entry Block.
                        MCWorkspaceManager.Instance.AddEntryNode();
                        //MCWorkspaceManager.Instance.ResetWorkspaceState();
                        MCWorkspaceManager.Instance.ResetWorkspace();
                    }
                    break;
                case TabAddType.Load:
                    {
                        currentTabs.Add(LoadProjectTab(tabName));
                        ChangeTabState(currentTabs.Count - 1);
                        //MCWorkspaceManager.Instance.ResetWorkspaceState();
                        MCWorkspaceManager.Instance.ResetWorkspace();
                    }
                    break;
                default: break;
            }

            ChangeTabState(currentTabs.Count - 1);
            RearrangeTabsPosition();

            MCWorkspaceManager.Instance.SetWorkspaceActive(true);
        }

        // Test.. Using not PROJECT tab such as if condition tab.
        public void SaveCurrentTabsState(Action onFinished = null)
        {
            foreach (TabComponent tab in currentTabs)
            {
                ChangeTabState(tab.TabID, false);
            }

            onFinished?.Invoke();
        }

        public void RemoveAllTabs()
        {
            for (int ix = currentTabs.Count - 1; ix >= 0; --ix)
            {
                TabComponent tab = currentTabs[ix];
                RemoveTab(tab);
            }

            OnTabChanged?.Invoke();
        }

        public void RemoveTab(TabComponent tab)
        {
            if (tab == null)
            {
                return;
            }

            TabComponent foundTab = GetTab(tab.TabID);
            if (foundTab != null)
            {
                currentTabs.RemoveAt(foundTab.TabID);

                //foundTab.ChangeState(false);

                // Retun to object pool.
                foundTab.ReturnToPool(projectTabItemName, transform);
            }

            RearrangeTabsPosition(true);

            // 현재 선택된 탭이 아닌 다른 탭이 삭제된 경우, 이전에 선택된 탭 인덱스 값을 갱신해줘야함.
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                if (currentTabs[ix].TabName.Equals(CurrentTab.TabName))
                {
                    prevSelectedTabIndex = ix;
                    break;
                }
            }

            if (currentTabs.Count == 0)
            {
                //MCWorkspaceManager.Instance.IsSimulation = false;
                MCPlayStateManager.Instance.IsSimulation = false;
            }

            OnTabChanged?.Invoke();
        }

        public void ChangeTabName(string newName)
        {
            CurrentTab.TabName = newName;
        }

        private void SetAllTabUnselected()
        {
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                currentTabs[ix].ChangeState(false);
            }

            MCWorkspaceManager.Instance.SetAllUnSelected();
        }

        // 탭 전환 시 이전 탭의 상태를 저장(보존) 처리하는 메소드.
        // 프로젝트 탭 -> 함수 탭 or 함수 탭 -> 프로젝트 탭 전환 시 필요한 부분 처리.
        private void PreservePreviousTabState()
        {
            if (prevSelectedTabIndex != -1 && prevSelectedTabIndex < currentTabs.Count)
            {
                TabComponent prevTab = currentTabs[prevSelectedTabIndex];
                if (prevTab is MCProjectTab)
                {
                    if (MCProjectManager.ProjectDescription != null)
                    {
                        // 1. 프로젝트 저장
                        MCWorkspaceManager.Instance.CompileProject();

                        // 2. 프로젝트 게임 오브젝트 삭제.
                        MCWorkspaceManager.Instance.ReleaseOnlyNodeLogic();
                    }
                }
                else if (prevTab is MCFunctionTab)
                {
                    // 1. 함수 컴파일.
                    string tabName = currentTabs[prevSelectedTabIndex].TabName;
                    //Utils.LogRed($"Current Tab Name: {tabName}");
                    int index = MCWorkspaceManager.Instance.GetFunctionIndexWithName(tabName);
                    LeftMenuFunctionItem functionItem = MCWorkspaceManager.Instance.GetFunction(index);
                    if (functionItem != null)
                    {
                        functionItem.CompileFunctionData();

                        //Utils.LogRed("Function Compile And Delete Start");

                        // 2. 함수 노드 GO 삭제.
                        //MCFunctionTable.Instance.DeleteAllFunctionNodes();
                        MCFunctionTable.Instance.DeleteAllFunctionNodes(functionItem);

                        // 3. 라인 GO 삭제.
                        MCWorkspaceManager.Instance.DeleteAllLines();
                    }
                }

                currentTabs[prevSelectedTabIndex].ChangeState(false);
            }
        }

        public void ChangeTabState(int tabID, bool hasRemoved = false)
        {
            // 같은 탭을 또 눌렀을 때 처리하지 않도록.
            if (!hasRemoved && prevSelectedTabIndex.Equals(tabID))
            {
                return;
            }

            if (tabID < 0)
            {
                return;
            }

            if (!hasRemoved)
            {
                //Utils.LogRed($"PreservePreviousTabState Call");
                PreservePreviousTabState();
            }

            selectedTabIndex = tabID;
            currentTabs[selectedTabIndex].ChangeState(true);

            prevSelectedTabIndex = selectedTabIndex;

            // Test.
            OnTabChanged?.Invoke();
        }

        private TabComponent GetTab(int tabID)
        {
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                if (currentTabs[ix].TabID.Equals(tabID)) return currentTabs[ix];
            }

            return null;
        }

        private TabComponent LoadProjectTab(string tabName)
        {
            TabComponent newTab = GetTabComponentFromOjbectPool(projectTabItemName);

            newTab.TabName = tabName;
            newTab.TabID = currentTabs.Count;
            newTab.SetManager(this);

            return newTab;
        }

        private TabComponent LoadFunctionTab(string tabName)
        {
            TabComponent newTab = GetTabComponentFromOjbectPool(functionTabItemName);

            newTab.TabName = tabName;
            newTab.TabID = currentTabs.Count;
            newTab.SetManager(this);

            return newTab;
        }

        private TabComponent CreateNewFunctionTab(string tabName)
        {
            //Debug.Log("CreateNewFUNCTIONTab");

            TabComponent newTab = GetTabComponentFromOjbectPool(functionTabItemName);

            bool isTabNameNull = string.IsNullOrEmpty(tabName);
            string functioinName = isTabNameNull ? "TestFunc" + (currentTabs.Count + 1).ToString() : tabName;

            // Set TabUI.
            newTab.TabName = functioinName;
            newTab.SetManager(this);

            return newTab;
        }

        private TabComponent CreateNewProjectTab(string tabName)
        {
            TabComponent newTab = GetTabComponentFromOjbectPool(projectTabItemName);

            bool isTabNameNull = string.IsNullOrEmpty(tabName);
            string projectName = isTabNameNull ? "Test" + (currentTabs.Count + 1).ToString() : tabName;

            // Set TabUI.
            newTab.TabName = projectName;
            newTab.SetManager(this);

            return newTab;
        }

        private TabComponent GetTabComponentFromOjbectPool(string tabItemName)
        {
            // Get Object from object pool.
            GameObject newTabObj = ObjectPool.Instance.PopFromPool(tabItemName, transform);
            newTabObj.transform.position = Vector3.zero;
            newTabObj.transform.localScale = Vector3.one;
            newTabObj.SetActive(true);

            return newTabObj.GetComponent<TabComponent>();
        }

        private void RearrangeTabsPosition(bool hasRemoved = false)
        {
            if (currentTabs.Count == 0)
            {
                if (newTabComponent != null)
                {
                    newTabComponent.anchoredPosition = Vector2.zero;
                }

                selectedTabIndex = prevSelectedTabIndex = -1;
                return;
            }

            bool anyTabSelected = false;
            Vector2 newPos = Vector2.zero;
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                newPos.x = ix * currentTabs[ix].TabSize.x + (ix == 0 ? 0 : ix) * tabOffset;
                newPos.y = 0f;
                currentTabs[ix].GetComponent<RectTransform>().anchoredPosition = newPos;
                currentTabs[ix].TabID = ix;

                if (currentTabs[ix].IsSelected)
                {
                    anyTabSelected = true;
                    selectedTabIndex = ix;
                }
            }

            if (newTabComponent != null)
            {
                newTabComponent.anchoredPosition = new Vector2(newPos.x + currentTabs[0].TabSize.x + tabOffset, 0f);
            }

            //if (currentTabs.Count == 1 || !anyTabSelected)
            if (!anyTabSelected)
            {
                ChangeTabState(0, hasRemoved);
            }
        }

        public int GetTabIndex(string tabName)
        {
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                TabComponent tab = currentTabs[ix];
                if (tab.TabName.Equals(tabName))
                {
                    return ix;
                }
            }

            return -1;
        }

        public TabComponent FindTab(string tabName, OwnerGroup ownerGroup = OwnerGroup.PROJECT)
        {
            for (int ix = 0; ix < currentTabs.Count; ++ix)
            {
                if (currentTabs[ix].TabName.Equals(tabName) && currentTabs[ix].OwnerGroup == ownerGroup)
                {
                    return currentTabs[ix];
                }
            }

            return null;
        }

        // Properties.
        public TabComponent CurrentTab
        {
            get
            {
                //Debug.LogWarning($"CurrentTab Property check: <{currentTabs.Count}> : <{selectedTabIndex}>");

                if (currentTabs.Count == 0 || selectedTabIndex == -1) return null;
                return currentTabs[selectedTabIndex];
            }
        }

        public bool CanAddTab { get { return currentTabs.Count < maxTabCount; } }
        public int CurrentTabCount { get { return currentTabs.Count; } }
        public OwnerGroup CurrentOwnerGroup
        {
            get
            {
                if (CurrentTab == null)
                {
                    //Debug.LogWarning($"CurrentTab is null");
                    return OwnerGroup.NONE;
                }

                return CurrentTab.OwnerGroup;
            }
        }

        private void UpdateTabcontainerWidth()
        {
            Vector2 size = tabContainerRect.sizeDelta;
            size.x = currentTabs.Count * tabWidth;
            tabContainerRect.sizeDelta = size;
        }

        public void SubscribeOnTabChanged(Action listener)
        {
            OnTabChanged += listener;
        }

        public void UnSubscribeOnTabChanged(Action listener)
        {
            OnTabChanged -= listener;
        }
    }
}