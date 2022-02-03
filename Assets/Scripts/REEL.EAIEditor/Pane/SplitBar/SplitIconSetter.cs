using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    using Type = SplitBar.Type;

    public class SplitIconSetter : MonoBehaviour
    {
        public GraphicRaycaster raycaster;
        public EventSystem eventSystem;

        public Texture2D resizeHIcon;
        public Texture2D resizeVIcon;
        public CursorMode cursorMode = CursorMode.Auto;
        public CreateOrLoadWindow createOrLoadWindow;

        private SplitBar bar = null;

        private bool isFileExplorerOpened = false;

        private void Start()
        {
            createOrLoadWindow.SubscribeFileExplorerOpenEvent(OnFileExplorerOpened);
            createOrLoadWindow.SubscribeFileExplorerCloseEvent(OnFileExplorerClosed);
        }

        private void Update()
        {
            CheckMouseHoverOnSplitBar();
        }

        private void LateUpdate()
        {
            ChangeMouseCursor();
        }

        private void CheckMouseHoverOnSplitBar()
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData data = new PointerEventData(eventSystem);
            data.position = Input.mousePosition;
            raycaster.Raycast(data, results);

            if (results.Count > 0)
            {
                RaycastResult result = results[0];
                bar = result.gameObject.GetComponent<SplitBar>();
            }
        }

        private void ChangeMouseCursor()
        {
            if (isFileExplorerOpened)
                return;

            if (bar != null)
            {
                if (bar.type == Type.Horizontal)
                {
                    Cursor.SetCursor(resizeHIcon, new Vector2(23f, 10f), cursorMode);
                }
                else if (bar.type == Type.Vertical)
                {
                    Cursor.SetCursor(resizeVIcon, new Vector2(10f, 23f), cursorMode);
                }

                bar = null;
            }

            else
            {
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
            }
        }

        private void OnFileExplorerOpened()
        {
            isFileExplorerOpened = true;
        }

        private void OnFileExplorerClosed()
        {
            isFileExplorerOpened = false;
        }
    }
}