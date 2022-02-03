using System;
using System.Collections;
using UnityEngine;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class ScrollbarValue
    {
        public float hValue = 0f;
        public float vValue = 0f;

        public ScrollbarValue()
        {
            hValue = 0f;
            vValue = 0f;
        }

        public ScrollbarValue(float hValue, float vValue)
        {
            SetValues(hValue, vValue);
        }

        public void SetValues(float hValue, float vValue)
        {
            this.hValue = hValue;
            this.vValue = vValue;
        }
    }

    public class TabComponent : MonoBehaviour
    {
        [SerializeField] protected TabUI tabUI;
        //[SerializeField] private TabData tabData;

        [SerializeField] protected TabManager tabManager;
        [SerializeField] protected int tabID = 0;
        [SerializeField] protected Constants.OwnerGroup ownerGroup;

        // 탭 상태 저장을 위한 변수.
        // 탭의 H/V 스크롤바 값 / 탭의 스케일 값.
        protected ScrollbarValue scrollbarValue;
        protected float tabScaleValue = 1f;
        protected Vector3 tabPosition;
        protected Vector2 tabPivot;

        private bool hasInitialized = false;

        protected virtual void OnEnable()
        {
            if (hasInitialized == false)
            {
                Initialize();
            }
        }

        protected virtual void OnDisable()
        {

        }

        // Init components.
        protected virtual void Initialize()
        {
            if (!tabUI)
            {
                tabUI = GetComponent<TabUI>();
            }

            if (scrollbarValue == null)
            {
                scrollbarValue = new ScrollbarValue();
            }

            WorkspaceScrollRect rect = MCWorkspaceManager.Instance.workspaceScrollRect;
            tabPivot = rect.content.pivot;
            tabPosition = rect.content.transform.localPosition;

            //if (!tabData) tabData = GetComponent<TabData>();
            hasInitialized = true;
        }

        public virtual void CloseTab()
        {
            //MCWorkspaceManager.Instance.OnCloseCurrentProjectClicked();

            if (tabManager)
            {
                tabManager.RemoveTab(this);
            }
        }

        public void SetManager(TabManager tabManager)
        {
            this.tabManager = tabManager;
        }

        public virtual void OnTabClicked()
        {
            if (tabManager)
            {
                tabManager.ChangeTabState(tabID);
            }
        }

        protected virtual void OnSelected()
        {
            DelayFunctionCall(() =>
            {
                WorkspaceScrollRect rect = MCWorkspaceManager.Instance.workspaceScrollRect;
                rect.content.pivot = tabPivot;
                rect.content.transform.localPosition = tabPosition;
                rect.GetComponent<ContentScaler>().SetScaleAndPivot(tabScaleValue, tabPivot);
                SetScrollbarValues(scrollbarValue.hValue, scrollbarValue.vValue);

                //Utils.LogRed($"[OnSelected] rect.content.transform.localScale: {rect.content.transform.localScale} / scrollScaleValue: {tabScaleValue}");
            }, 0.1f);
        }

        protected virtual void OnUnSelected()
        {
            WorkspaceScrollRect rect = MCWorkspaceManager.Instance.workspaceScrollRect;
            tabPivot = rect.content.pivot;
            tabPosition = rect.content.transform.localPosition;
            scrollbarValue.SetValues(rect.horizontalScrollbar.value, rect.verticalScrollbar.value);
            tabScaleValue = rect.content.transform.localScale.x;

            //Utils.LogRed($"[OnUnSelected] scrollScaleValue : {tabScaleValue } / scrollScaleValue: {rect.content.transform.localScale}");
        }

        private IEnumerator DelayFunctionCallCoroutine(Action function, float delay)
        {
            yield return new WaitForSeconds(delay);
            function?.Invoke();
        }

        private void DelayFunctionCall(Action function, float delay)
        {
            StartCoroutine(DelayFunctionCallCoroutine(function, delay));
        }

        public void ChangeState(bool isSelected)
        {
            if (IsSelected == true && isSelected == false)
            {
                OnUnSelected();
            }
            else if (IsSelected == false && isSelected == true)
            {
                OnSelected();
            }

            tabUI.ChangeState(isSelected);
            //tabData.ChangeState(isSelected);
        }

        public void ReturnToPool(string itemName, Transform parent = null)
        {
            ChangeState(false);
            tabUI.TabName = string.Empty;
            tabID = 0;
            hasInitialized = false;
            tabScaleValue = 1f;
            tabPivot = new Vector2(0f, 1f);
            ObjectPool.Instance.PushToPool(itemName, gameObject);
        }

        public void SetScrollbarValues(float hValue, float vValue)
        {
            scrollbarValue.hValue = hValue;
            scrollbarValue.vValue = vValue;
        }

        public void SetScrollScaleValue(float scale)
        {
            tabScaleValue = scale;
        }

        // Getter Properties.
        public Vector2 TabSize {  get { return tabUI.TabSize; } }
        public string TabName
        {
            get { return tabUI.TabName; }
            set { tabUI.TabName = value; }
        }
        public bool IsSelected { get { return tabUI.IsSelected; } }

        public int TabID
        {
            get { return tabID; }
            set { if (value != -1) tabID = value; }
        }

        public Constants.OwnerGroup OwnerGroup { get { return ownerGroup; } }
    }
}