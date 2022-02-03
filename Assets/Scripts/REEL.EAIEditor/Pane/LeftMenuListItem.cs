using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class LeftMenuListItem : MonoBehaviour, IPointerDownHandler
    {
        public TMP_Text nameText;
        //public Text nameText;

        public virtual void SetName(string name)
        {
            nameText.text = name;
            this.name = name;
        }

        public string VariableName { get { return nameText.text; } }

        protected virtual void OnEnable()
        {
            if (nameText == null)
            {
                //nameText = GetComponentInChildren<Text>();
                nameText = GetComponentInChildren<TMP_Text>();
            }
        }

        protected virtual void OnDisable()
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            
        }

        protected MCPopup GetPopup(MCEditorManager.PopupType popuptype)
        {
            return MCEditorManager.Instance.GetPopup(popuptype);
        }
    }
}