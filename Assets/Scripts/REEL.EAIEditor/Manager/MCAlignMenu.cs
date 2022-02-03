using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAlignMenu : Singleton<MCAlignMenu>
    {
        private UIEffect effect = null;

        private void OnEnable()
        {
            if (effect == null)
            {
                effect = GetComponent<UIEffect>();
            }

            Utils.GetGraphPane().SubscribeOnPointDown(CloseMenu);
        }

        private void OnDisable()
        {
            Utils.GetGraphPane().UnSubscribeOnPointDown(CloseMenu);
        }

        public void ToggleMenu()
        {
            if (gameObject.activeSelf == true)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        public void OpenMenu()
        {
            gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            if (effect != null)
            {
                effect.CloseUIEffect();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}