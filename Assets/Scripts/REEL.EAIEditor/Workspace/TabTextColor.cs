using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class TabTextColor : MonoBehaviour
    {
        public Color selectedColor;
        public Color unselectedColor;

        public Graphic[] graphics;

        public void OnSelected()
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = selectedColor;
            }
        }

        public void OnUnselected()
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = unselectedColor;
            }
        }
    }
}