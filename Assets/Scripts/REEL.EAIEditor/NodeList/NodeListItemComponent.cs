using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class NodeListItemComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public Image iconImage;
        public TMPro.TextMeshProUGUI nameText;
        //public Text nameText;
        public PROJECT.NodeType nodeType;

        public Image image;

        public Color normalColor = Color.white;
        public Color selectedColor = Color.yellow;

        private string engLowerItemName = string.Empty;
        public string EngLowerItemName
        {
            get
            {
                if (Utils.IsNullOrEmptyOrWhiteSpace(engLowerItemName) == true)
                {
                    if (localText != null)
                    {
                        engLowerItemName = LocalText.eng.ToLower();
                    }
                }

                return engLowerItemName;
            }
        }
        private string korLowerItemName = string.Empty;
        public string KorLowerItemName
        {
            get
            {
                if (Utils.IsNullOrEmptyOrWhiteSpace(korLowerItemName) == true)
                {
                    if (localText != null)
                    {
                        korLowerItemName = LocalText.kor.ToLower();
                    }
                }

                return korLowerItemName;
            }
        }

        private LocalizationManager.LocalText localText = null;
        public LocalizationManager.LocalText LocalText
        {
            get
            {
                if (localText == null)
                {
                    localText = LocalizationManager.GetLocalText(nameText.text);
                }

                return localText;
            }
        }

        private RectTransform refRect;
        public RectTransform RefRect
        {
            get
            {
                if (refRect == null)
                {
                    refRect = GetComponent<RectTransform>();
                }

                return refRect;
            }
        }

        private BlockCategoryManager categoryManager;

        private float startYPosition = 0f;

        private void Awake()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            if (categoryManager == null)
            {
                categoryManager = GetComponentInParent<BlockCategoryManager>();
            }

            if (GetComponent<MCNodeTooltip>() == null)
            {
                gameObject.AddComponent<MCNodeTooltip>();
            }

            startYPosition = RefRect.anchoredPosition.y;

            //노드 이름 변경 - kjh
            name = string.Format($"Node List Item({nodeType.ToString()})");
        }

        private void OnEnable()
        {
            if (localText == null)
            {
                localText = LocalizationManager.GetLocalText(nameText.text);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MCEditorManager.Instance.DragInfoSetActive(true);
            MCEditorManager.Instance.DragInfoSetSprite(iconImage.sprite);
            MCEditorManager.Instance.DragInfoSetText(nodeType.ToString());
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (MCEditorManager.Instance.DragInfoIsActive == false)
            {
                MCEditorManager.Instance.DragInfoSetActive(true);
            }

            MCEditorManager.Instance.DragInfoSetPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log(string.Format("OnEndDrag {0}", MCWorkspaceManager.Instance.IsSimulation));

            // 프로젝트가 열려있지 않은 상태일 때는 배치 안하도록 처리.
            // 현재 열려있는 탭이 함수 탭인 경우, 프로세스 관련 노드는 생성 안되도록 처리.
            if (Utils.IsProjectNullOrOnSimulation || CanSpawn == false)
            {
                MCEditorManager.Instance.DragInfoSetActive(false);
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            if (MCEditorManager.Instance.IsOnGraphPane(results))
            {
                //if (nodeType == PROJECT.NodeType.START)
                //{
                //    MCWorkspaceManager.Instance.AddEntryNode(eventData.position, 0);
                //}
                //else
                //{
                //    //MCWorkspaceManager.Instance.PaneObject.AddNode(eventData.position, nodeType, Utils.NewGUID, false);
                //    MCUndoRedoManager.Instance.AddCommand(new MCAddNodeCommand(eventData.position, nodeType, Utils.NewGUID, false));
                //}

                MCUndoRedoManager.Instance.AddCommand(new MCAddNodeCommand(eventData.position, nodeType, Utils.NewGUID, false));

                //-----------------------------------------------------
                // TutorialManager.SendEvent
                // 특정 노드블록을 작업영역으로 드래그함 -kjh
                //-----------------------------------------------------
                TutorialManager.SendEvent(Tutorial.CustomEvent.NodeCreated, nodeType.ToString());
            }

            MCEditorManager.Instance.DragInfoSetActive(false);

            // Test.
            categoryManager.SetAllItemUnselected();
        }

        private bool CanSpawn
        {
            get
            {
                if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return true;
                }

                else if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                {
                    if (nodeType == PROJECT.NodeType.START ||
                        nodeType == PROJECT.NodeType.STOP || 
                        nodeType == PROJECT.NodeType.PAUSE ||
                        nodeType == PROJECT.NodeType.RESUME)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MCEditorManager.Instance.GetPopup(MCEditorManager.PopupType.VariableContext).HidePopup();

            // Test.
            categoryManager.OnItemSelected(GetInstanceID());
        }

        public void SetImage(Texture2D texture)
        {
            if (texture == null) return;

            Rect rect = new Rect(0f, 0f, texture.width, texture.height);
            iconImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }

        public void SetText(string text)
        {
            nameText.text = text;
        }

        public void SetItemSelected(bool isSelected)
        {
            image.color = isSelected ? selectedColor : normalColor;
        }

        public void ResetPosition()
        {
            SetPosition(startYPosition);
        }

        public void SetPosition(float yPosition)
        {
            Vector2 position = RefRect.anchoredPosition;
            position.y = yPosition;
            RefRect.anchoredPosition = position;
        }
    }
}