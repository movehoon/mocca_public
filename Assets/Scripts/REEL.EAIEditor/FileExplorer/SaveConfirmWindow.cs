using UnityEngine;

namespace REEL.D2EEditor
{
    public class SaveConfirmWindow : MonoBehaviour
    {
        public SaveWindow saveWindow;

        public void OnOKClicked()
        {
            saveWindow.SaveAnyway();
            CloseWindow();
        }

        public void OnCancelClicked()
        {
            CloseWindow();
        }

        public void ShowWindow()
        {
            gameObject.SetActive(true);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
        }
    }
}