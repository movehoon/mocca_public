using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using REEL.D2EEditor;

namespace REEL.Test
{
    public class SplitController : MonoBehaviour
    {
        public UISplitBar verticalSplitBar;
        public UISplitBar horizontalSplitBar;
        public EventSystem eventSystem;
        public GraphicRaycaster raycaster;
        public bool isSelected = false;

        public Texture2D resizeHIcon;
        public Texture2D resizeVIcon;
        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;

        RectTransform rectTransform;
        private float offset = 0f;
        private float rectX;
        private float rectY;
        private float halfWidth = 0f;
        private float halfHeight = 0f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            rectX = rectTransform.position.x;
            rectY = rectTransform.position.y;
            halfWidth = rectTransform.rect.width * 0.5f;
            halfHeight = rectTransform.rect.height * 0.5f;
        }

        private void Update()
        {
            ChangeMouseCursor();
            ProcessMouseInput();
        }

        void ChangeMouseCursor()
        {
            if (isSelected) return;

            RaycastResult result;
            if (Util.GetRaycastResult(raycaster, eventSystem, Input.mousePosition, out result))
            {
                if (result.gameObject.CompareTag("Player"))
                {
                    Cursor.SetCursor(resizeHIcon, new Vector2(23f, 10f), cursorMode);
                }
                else if (result.gameObject.CompareTag("Finish"))
                {
                    Cursor.SetCursor(resizeVIcon, new Vector2(10f, 23f), cursorMode);
                }
                else
                {
                    Cursor.SetCursor(null, Vector2.zero, cursorMode);
                }
            }
        }

        void ProcessMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastResult result;
                if (Util.GetRaycastResult(raycaster, eventSystem, Input.mousePosition, out result))
                {
                    if (result.gameObject.CompareTag("Player"))
                    {
                        verticalSplitBar = result.gameObject.GetComponent<UISplitBar>();
                        isSelected = true;
                        offset = verticalSplitBar.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
                    }
                    else if (result.gameObject.CompareTag("Finish"))
                    {
                        horizontalSplitBar = result.gameObject.GetComponent<UISplitBar>();
                        isSelected = true;
                        offset = horizontalSplitBar.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
                    }
                }
            }

            if (Input.GetMouseButton(0) && isSelected)
            {
                if (verticalSplitBar)
                {
                    float xPos = Input.mousePosition.x;
                    xPos = Mathf.Clamp(xPos, rectX - halfWidth + offset, rectX + halfWidth - offset);
                    verticalSplitBar.PositionUpdate(new Vector2(xPos, verticalSplitBar.GetComponent<RectTransform>().position.y));
                }
                else if (horizontalSplitBar)
                {
                    float yPos = Input.mousePosition.y;
                    yPos = Mathf.Clamp(yPos, rectY - halfHeight + offset, rectY + halfHeight - offset);
                    horizontalSplitBar.PositionUpdate(new Vector2(horizontalSplitBar.GetComponent<RectTransform>().position.x, yPos));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isSelected = false;
                verticalSplitBar = horizontalSplitBar = null;
            }
        }
    }
}