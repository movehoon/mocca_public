using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class NodeDragInfo : MonoBehaviour
    {
        public Image image;
        public Text text;
        public TMPro.TextMeshProUGUI TMP_Text;

        public string Text 
        { 
            get 
            {
                if (TMP_Text != null) return TMP_Text.text;
                if (text != null) return text.text;
                return "";
            }
            set
            {
                if (TMP_Text != null) TMP_Text.text = value;
                if (text != null) text.text = value;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetText(string text)
        {
            //this.text.text = text;
            Text = text;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public bool IsActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }
    }
}