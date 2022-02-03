using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace REEL.D2EEditor
{
    public class WorkSpaceBlocker : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI text;

        public Scrollbar verticalScrollbar;
        public Scrollbar horizontalScrollbar;

        public WorkspaceScrollRect workspaceScrollRect;

        public ContentScaler contentScaler;
        public GameObject loginButtonGO = null;
        public GameObject newProjectButtonGO = null;
        public GameObject loadProjectButtonGO = null;

        private readonly string loginRequiredString = "로그인을 하면\n모카 서비스를 이용할 수 있어요!";

        private readonly string noProjectString = "프로젝트가 없음";
        private readonly string onPlayingString = "프로젝트 실행중";

        private bool isLoginRequired = false;

        private void OnEnable()
        {
            isLoginRequired = true;

            if (text == null)
            {
                text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }

            //if (MCWorkspaceManager.IsProjectNull)
            //{
            //    text.text = noProjectString;
            //    SetScrollbarActive(false);
            //}
            //else
            //{
            //    if (MCWorkspaceManager.Instance.IsSimulation)
            //    {
            //        text.text = onPlayingString;
            //    }
            //}

            if (workspaceScrollRect == null)
            {
                workspaceScrollRect = FindObjectOfType<WorkspaceScrollRect>();
            }

            if (contentScaler == null)
            {
                contentScaler = FindObjectOfType<ContentScaler>();
            }

            FirebaseManager.SubscribeOnLoginStatusChanged(OnLoginStatusChanged);
            OnLoginStatusChanged(FirebaseManager.CheckLogin());
        }

        private void OnDisable()
        {
            text.text = string.Empty;

            //SetScrollbarActive(true);

            //if (MCWorkspaceManager.IsProjectNull)
            //{
            //    SetScrollbarActive(false);
            //}
            //else
            //{
            //    SetScrollbarActive(true);
            //}

            FirebaseManager.UnsubscribeOnLoginStatusChanged(OnLoginStatusChanged);
        }

        private void OnLoginStatusChanged(bool isLoggedin)
        {
            if (isLoggedin)
            {
                isLoginRequired = false;
                loginButtonGO.SetActive(false);
                

                if (MCWorkspaceManager.IsProjectNull)
                {
                    text.text = LocalizationManager.ConvertText(noProjectString);
                    newProjectButtonGO.SetActive(true);
                    loadProjectButtonGO.SetActive(true);
                    //SetScrollbarActive(false);
                }
                else
                {
                    //if (MCWorkspaceManager.Instance.IsSimulation)
                    if (MCPlayStateManager.Instance.IsSimulation)
                    {
                        text.text = LocalizationManager.ConvertText(onPlayingString);
                        newProjectButtonGO.SetActive(false);
                        loadProjectButtonGO.SetActive(false);
                    }
                }
            }
            else
            {
                text.text = LocalizationManager.ConvertText(loginRequiredString);
                loginButtonGO.SetActive(true);
                newProjectButtonGO.SetActive(false);
                loadProjectButtonGO.SetActive(false);
            }
        }

        private void SetScrollbarActive(bool isActive)
        {
            MCWorkspaceManager.Instance.GetWorkspaceScrollRect.horizontal = isActive;
            MCWorkspaceManager.Instance.GetWorkspaceScrollRect.vertical = isActive;
        }

        public void OnDrag(PointerEventData eventData)
        {
            workspaceScrollRect.OnDrag(eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            contentScaler.OnScroll(eventData);
        }
    }
}