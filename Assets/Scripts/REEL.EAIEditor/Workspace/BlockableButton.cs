using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class BlockableButton : MonoBehaviour
    {
        private Button button;

        private bool originInteractable;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponentInChildren<Button>();
            }

            MCPlayStateManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
            originInteractable = button.interactable;
        }

        private void OnSimulationStateChanged(bool isSimulation)
        {
            if (!originInteractable)
            {
                return;
            }

            button.interactable = !isSimulation;
            button.GetComponent<Image>().color = isSimulation ? button.colors.disabledColor : button.colors.normalColor;
        }
    }
}
