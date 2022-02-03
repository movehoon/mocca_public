using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class McTutorialListItem : MonoBehaviour
    {
        [Header("TMP_Texts")]
        public UnityEngine.UI.Button    button;
        public TMP_Text titleText;
        public TMP_Text contentText;

        public void SetShortCutInformation( TutorialInfo tutorialInfo)
        {
            titleText.text = tutorialInfo.title;
            contentText.text = tutorialInfo.content;
        }
    }
}