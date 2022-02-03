using System;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCUserMenu : Singleton<MCUserMenu>
    {
        public GameObject window = null;
        public TMPro.TextMeshProUGUI TMP_LogInOutText = null;

        private UIEffect effect = null;

        private readonly string loginString = "로그인";
        private readonly string logoutString = "로그아웃";


        private void Awake()
        {
            //MCWorkspaceManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
        }

		private void OnEnable()
        {
            if (effect == null)
            {
                effect = GetComponentInChildren<UIEffect>();
            }

            FirebaseManager.SubscribeOnLoginStatusChanged(OnLoginStatusChanged);
            OnLoginStatusChanged(FirebaseManager.CheckLogin());

            Utils.GetGraphPane().SubscribeOnPointDown(CloseUserMenu);
        }

        private void OnDisable()
        {
            FirebaseManager.UnsubscribeOnLoginStatusChanged(OnLoginStatusChanged);

            Utils.GetGraphPane().UnSubscribeOnPointDown(CloseUserMenu);
        }

        public void ToggleMenu()
        {
            if (window.activeSelf)
            {
                CloseUserMenu();
            }
            else
            {
                OpenUserMenu();
            }
        }

        public void OpenUserMenu()
        {
            //if( MCWorkspaceManager.Instance.IsSimulation == false )
            if (MCPlayStateManager.Instance.IsSimulation == false)
            {
                window.SetActive(true);
            }
        }

        public void CloseUserMenu()
        {
            if (effect != null)
            {
                effect.CloseUIEffect();
                return;
            }
            else
            {
                effect = GetComponentInChildren<UIEffect>();
                if (effect != null)
                {
                    effect.CloseUIEffect();
                    return;
                }

                window.SetActive(false);
            }
        }

        private void OnSimulationStateChanged(bool isSimulation)
        {
            CloseUserMenu();
        }

        private void OnLoginStatusChanged(bool isLoggedin)
        {
            TMP_LogInOutText.text = isLoggedin == true ? logoutString : loginString;
        }
    }
}