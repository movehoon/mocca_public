using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class McShorcutListItem : MonoBehaviour
    {
        [Header("TMP_Texts")]
        public TMP_Text commandText;
        public TMP_Text shorcutText;

        public void SetShortCutInformation(ShortCutInfo shortCutInfo)
        {
            LocalizationManager.LocalText localText 
                = LocalizationManager.GetLocalText(shortCutInfo.command, "ShortCut");
            commandText.text = LocalizationManager.CurrentLanguage
                == LocalizationManager.Language.KOR ? localText.kor : localText.eng;

            localText = LocalizationManager.GetLocalText(shortCutInfo.shortcut, "ShortCut");
            if (localText == null)
            {
                shorcutText.text = shortCutInfo.shortcut;
            }
            else
            {
                shorcutText.text = LocalizationManager.CurrentLanguage
                == LocalizationManager.Language.KOR ? localText.kor : localText.eng;
            }
        }
    }
}