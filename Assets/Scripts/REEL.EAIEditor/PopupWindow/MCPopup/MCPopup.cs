using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCPopup : MonoBehaviour, IMCPopup
    {
        protected MCNode targetNode;
        protected RectTransform refRT;

        protected virtual void OnEnable()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }
        }

        protected virtual void Update()
        {
            if (MessageBox.IsActive == false)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HidePopup();
                }
            }
        }

        public virtual void OnOKClicked()
        {
            HidePopup();
        }

        public virtual void OnCancelClicked()
        {
            HidePopup();
        }

        public virtual void ShowPopup(MCNode node = null)
        {
            // 오른쪽 상단 사용자 메뉴 닫기.
            var userMenu = Utils.GetUserMenu();
            if (userMenu != null)
            {
                userMenu.CloseUserMenu();
            }

            gameObject.SetActive(true);
            if (node != null)
            {
                SetTargetNode(node);
            }
        }

        public virtual void HidePopup()
        {
            SetTargetNode(null);
            gameObject.SetActive(false);
        }

        public virtual void HideAllPopups()
        {
            int count = (int)MCEditorManager.PopupType.Length;
            for (int ix = 0; ix < count; ++ix)
            {
                MCPopup popup = MCEditorManager.Instance.GetPopup((MCEditorManager.PopupType)(ix));
                if (popup != null && popup.gameObject.activeSelf)
                {
                    popup.gameObject.SetActive(false);
                    popup.SetTargetNode(null);
                }
            }
        }

        public virtual void HideAllPopupWithout(MCEditorManager.PopupType popupType)
        {
            int count = (int)MCEditorManager.PopupType.Length;
            for (int ix = 0; ix < count; ++ix)
            {
                if (popupType == (MCEditorManager.PopupType)(ix))
                {
                    continue;
                }

                MCPopup popup = MCEditorManager.Instance.GetPopup((MCEditorManager.PopupType)(ix));
                if (popup != null && popup.gameObject.activeSelf)
                {
                    popup.gameObject.SetActive(false);
                    popup.SetTargetNode(null);
                }
            }
        }

        public virtual void SetTargetNode(MCNode node)
        {
            targetNode = node;
        }

        // 팝업이 열렸는지 확인하는 프로퍼티.
        public bool IsOn
        {
            get { return gameObject.activeSelf; }
        }
    }
}