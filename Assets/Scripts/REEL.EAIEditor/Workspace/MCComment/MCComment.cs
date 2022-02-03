using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCComment : MonoBehaviour, 
        IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, 
        IPointerClickHandler,
        IComparable<MCComment>
    {
        public enum Direction
        {
            None = -1,
            Left, Right, Top, Bottom,
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        [Header("Basic")]
        public RectTransform header;
        public RectTransform body;
        public MCCommentBody commentBody;
        public TMP_Text commentText;
        public GameObject selection;
        public RectTransform topLeft;
        public RectTransform bottomRight;

        [Header("Resize Drag Area")]
        public RectTransform left;
        public RectTransform right;
        public RectTransform top;
        public RectTransform bottom;

        [Header("Raycast")]
        public EventSystem eventSystem;

        private RectTransform refRect;

        private bool shouldCheckPointer = false;

        [Header("Events")]
        public UnityEvent OnCommentMoveStart;
        public UnityEvent OnCommentMove;
        public UnityEvent OnCommentMoveEnd;
        public UnityEvent OnCommentSelect;

        private Direction direction = Direction.None;

        private Vector2 minSize = new Vector2(200f, 200f);
        private Vector3 prevPosition = Vector2.zero;
        private Vector2 prevDragPosition = Vector2.zero;
        private bool isResizing = false;
        public bool IsResizing { get { return isResizing; } }
        private DragPane pane;
        private MCCommentMovement movement;

        public int CommentID = 0;

        public bool IsSelected { get; set; }

        public Vector2 GetTopPosition {  get { return top.position; } }

        private Vector2 offset;

        private Vector3 originPosition;

        private bool multiUnSelect = false;

        private void OnEnable()
        {
            if (eventSystem == null)
            {
                eventSystem = FindObjectOfType<EventSystem>();
            }

            if (refRect == null)
            {
                refRect = GetComponent<RectTransform>();
            }

            if (pane == null)
            {
                pane = FindObjectOfType<DragPane>();
            }

            if (movement == null)
            {
                movement = GetComponent<MCCommentMovement>();
            }

            Utils.GetGraphPane().SubscribeOnPointDown(ReleaseSelect);
        }

        private void OnDisable()
        {
            Utils.GetGraphPane().UnSubscribeOnPointDown(ReleaseSelect);
        }

        private void Update()
        {
            if (!isResizing && shouldCheckPointer)
            {
                if (prevPosition != Input.mousePosition)
                {
                    UpdateCursor();
                }

                prevPosition = Input.mousePosition;
            }
        }

        private void LateUpdate()
        {
            if (isResizing == true)
            {
                PointerEventData eventData = new PointerEventData(eventSystem);
                eventData.position = Input.mousePosition;
                OnResize(eventData.position);

                if (direction == Direction.None)
                {
                    // 주석 박스 안에서 드래그 처리하도록 호출.
                    eventData.position = Input.mousePosition;
                    pane.OnDrag(eventData);
                }
            }
        }

        private void SetDefaultCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void UpdateCursor()
        {
            //PointerEventData data = new PointerEventData(eventSystem);
            //data.position = Input.mousePosition;

            Texture2D cursorTex = MCCommentManager.Instance.horizontal;

            //if (RectTransformUtility.RectangleContainsScreenPoint(left, Input.mousePosition))
            //{
            //    if (RectTransformUtility.RectangleContainsScreenPoint(top, Input.mousePosition))
            //    {
            //        cursorTex = MCCommentManager.Instance.topLeft;
            //        direction = Direction.TopLeft;
            //        //Utils.LogRed("top left");
            //    }
            //    else if (RectTransformUtility.RectangleContainsScreenPoint(bottom, Input.mousePosition))
            //    {
            //        cursorTex = MCCommentManager.Instance.topRight;
            //        direction = Direction.BottomLeft;
            //        //Utils.LogRed("bottom left");
            //    }
            //    else
            //    {
            //        cursorTex = MCCommentManager.Instance.horizontal;
            //        direction = Direction.Left;
            //        //Utils.LogRed("left");
            //    }
            //}
            //else if (RectTransformUtility.RectangleContainsScreenPoint(right, Input.mousePosition))
            if (RectTransformUtility.RectangleContainsScreenPoint(right, Input.mousePosition))
            {
                //if (RectTransformUtility.RectangleContainsScreenPoint(top, Input.mousePosition))
                //{
                //    cursorTex = MCCommentManager.Instance.topRight;
                //    direction = Direction.TopRight;
                //    //Utils.LogBlue("top right");
                //}
                //else if (RectTransformUtility.RectangleContainsScreenPoint(bottom, Input.mousePosition))
                if (RectTransformUtility.RectangleContainsScreenPoint(bottom, Input.mousePosition))
                {
                    cursorTex = MCCommentManager.Instance.topLeft;
                    direction = Direction.BottomRight;
                    //Utils.LogBlue("bottom right");
                }
                else
                {
                    cursorTex = MCCommentManager.Instance.horizontal;
                    direction = Direction.Right;
                    //Utils.LogBlue("right");
                }
            }
            //else if (RectTransformUtility.RectangleContainsScreenPoint(top, Input.mousePosition))
            //{
            //    cursorTex = MCCommentManager.Instance.vertical;
            //    direction = Direction.Top;
            //    //Utils.LogRed("top");
            //}
            else if (RectTransformUtility.RectangleContainsScreenPoint(bottom, Input.mousePosition))
            {
                cursorTex = MCCommentManager.Instance.vertical;
                direction = Direction.Bottom;
                //Utils.LogGreen("bottom");
            }
            else
            {
                SetDefaultCursor();
                direction = Direction.None;
                return;
            }

            Cursor.SetCursor(cursorTex, new Vector2(cursorTex.width * 0.5f, cursorTex.height * 0.5f), CursorMode.Auto);
        }

        private void ReleaseSelect()
        {
            OnSelect(false);
        }

        public void OnSelect(bool isSelect)
        {
            //if (IsSelected == isSelect)
            //{
            //    return;
            //}

            IsSelected = isSelect;
            selection.SetActive(isSelect);

            //if (isSelect == false)
            //{
            //    header.GetComponent<MCCommentHeader>().OnSelectEnd();
            //}
        }

        static Vector3[] corners = new Vector3[4];
        public void OnResize(Vector2 position)
        {
            Vector2 currentPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(refRect, position, null, out currentPosition);

            refRect.GetLocalCorners(corners);
            Vector2 bottomRightPosition = new Vector2(corners[3].x, corners[3].y);

            Vector2 sizeDelta = refRect.sizeDelta;
            Vector2 resizeValue = currentPosition - bottomRightPosition;
            Vector2 anchoredPosition = refRect.anchoredPosition;

            // for test.
            //bottomRight.anchoredPosition = corners[3];
            //topLeft.anchoredPosition = currentPosition;

            switch (direction)
            {
                case Direction.Top:
                    {
                        ResizeTop(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                    }
                    break;
                case Direction.Left:
                    {
                        ResizeLeft(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
                case Direction.Right:
                    {
                        ResizeRight(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
                case Direction.Bottom:
                    {
                        ResizeBottom(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                    }
                    break;
                case Direction.TopLeft:
                    {
                        ResizeTop(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                        ResizeLeft(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
                case Direction.TopRight:
                    {
                        ResizeTop(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                        ResizeRight(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
                case Direction.BottomLeft:
                    {
                        ResizeBottom(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                        ResizeLeft(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
                case Direction.BottomRight:
                    {
                        ResizeBottom(ref resizeValue, ref sizeDelta.y, ref anchoredPosition.y);
                        ResizeRight(ref resizeValue, ref sizeDelta.x, ref anchoredPosition.x);
                    }
                    break;
            }

            refRect.sizeDelta = sizeDelta;
            refRect.anchoredPosition = anchoredPosition;

            //prevDragPosition = currentPosition;
        }

        private void ResizeLeft(ref Vector2 resizeValue, ref float sizeDeltaX, ref float anchoredPositionX)
        {
            if (sizeDeltaX - resizeValue.x < minSize.x)
            {
                float offset = sizeDeltaX - minSize.x;
                sizeDeltaX = minSize.x;
                anchoredPositionX += offset * 0.5f;
                return;
            }

            sizeDeltaX -= resizeValue.x;
            anchoredPositionX += resizeValue.x * 0.5f;
        }

        private void ResizeRight(ref Vector2 resizeValue, ref float sizeDeltaX, ref float anchoredPositionX)
        {
            if (sizeDeltaX + resizeValue.x < minSize.x)
            {
                float offset = sizeDeltaX - minSize.x;
                sizeDeltaX = minSize.x;
                anchoredPositionX -= offset * 0.5f;
                return;
            }

            sizeDeltaX += resizeValue.x;
            anchoredPositionX += resizeValue.x * 0.5f;
        }

        private void ResizeTop(ref Vector2 resizeValue, ref float sizeDeltaY, ref float anchoredPositionY)
        {
            if (sizeDeltaY + resizeValue.y < minSize.y)
            {
                float offset = sizeDeltaY - minSize.y;
                sizeDeltaY = minSize.y;
                anchoredPositionY -= offset * 0.5f;
                return;
            }

            sizeDeltaY += resizeValue.y;
            anchoredPositionY += resizeValue.y * 0.5f;
        }

        private void ResizeBottom(ref Vector2 resizeValue, ref float sizeDeltaY, ref float anchoredPositionY)
        {
            if (sizeDeltaY - resizeValue.y < minSize.y)
            {
                float offset = sizeDeltaY - minSize.y;
                sizeDeltaY = minSize.y;
                anchoredPositionY += offset * 0.5f;
                return;
            }

            sizeDeltaY -= resizeValue.y;
            anchoredPositionY += resizeValue.y * 0.5f;
        }

        public void DeleteComment()
        {
            Destroy(gameObject);
            SetDefaultCursor();
        }

        public void SetChangedPosition(PointerEventData eventData)
        {
            refRect.position = eventData.position + offset;
            MoveComment();
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
            body.GetComponent<MCCommentBody>()?.OnMove();
            MoveComment();
        }

        public void ExecuteOnCommentSelect()
        {
            OnCommentSelect?.Invoke();
            //MCWorkspaceManager.Instance.SetAllUnSelected();
        }

        
        public void OnPointerDown(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            originPosition = refRect.position;

            if (KeyInputManager.Instance.shouldMultiSelect == true)
            {
                if (IsSelected == false)
                {
                    OnSelect(true);
                    multiUnSelect = false;
                }
                else
                {
                    multiUnSelect = true;
                }
            }
            else
            {
                multiUnSelect = false;
                if (IsSelected == false)
                {
                    MCCommentManager.Instance.SetOneSelected(this);
                }
            }

            //if (HasMoved == false && KeyInputManager.Instance.shouldMultiSelect && IsSelected)
            //{
            //    OnSelect(false);
            //}
            //else if (HasMoved == false)
            //{
            //    MCCommentManager.Instance.AddToSelection(this);

            //    // 주석(Comment) 눌림 이벤트 알림 (노드 선택 해제를 위해).
            //    MCWorkspaceManager.Instance.OnCommentPointDown();
            //}

            ExecuteOnCommentSelect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            shouldCheckPointer = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            shouldCheckPointer = false;
            SetDefaultCursor();
        }

        public void SetCommentText(string text)
        {
            commentText.text = text;
        }

        public void SetDragOffset(Vector3 pointerPosition)
        {
            offset = refRect.position - pointerPosition;
        }

        public void MoveCommentStart()
        {
            OnCommentMoveStart?.Invoke();
        }

        public void MoveComment()
        {
            OnCommentMove?.Invoke();
        }

        public void MoveCommentEnd()
        {
            OnCommentMoveEnd?.Invoke();
        }

        public void SubscribeOnCommentMoveStart(UnityAction func)
        {
            OnCommentMoveStart.AddListener(func);
        }

        public void UnSubscribeOnCommentMoveStart(UnityAction func)
        {
            OnCommentMoveStart.RemoveListener(func);
        }

        public void SubscribeOnCommentMove(UnityAction func)
        {
            OnCommentMove.AddListener(func);
        }

        public void UnSubscribeOnCommentMove(UnityAction func)
        {
            OnCommentMove.RemoveListener(func);
        }

        public void SubscribeOnCommentMoveEnd(UnityAction func)
        {
            OnCommentMoveEnd.AddListener(func);
        }

        public void UnSubscribeOnCommentMoveEnd(UnityAction func)
        {
            OnCommentMoveEnd.RemoveListener(func);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            isResizing = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(refRect, eventData.position, null, out prevDragPosition);

            // 주석 박스 안에서 드래그 처리하도록 호출.
            eventData.position = Input.mousePosition;
            pane.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            //OnResize(eventData.position);

            //if (direction == Direction.None)
            //{
            //    // 주석 박스 안에서 드래그 처리하도록 호출.
            //    eventData.position = Input.mousePosition;
            //    pane.OnDrag(eventData);
            //}
        }

        
        public void OnEndDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            isResizing = false;

            // 주석 박스 안에서 드래그 처리하도록 호출.
            eventData.position = Input.mousePosition;
            pane.OnEndDrag(eventData);

            // 크기 조절 후 주석 영역 안에 있는 노드 목록(리스트) 확인.
            commentBody.TestEmptyFunc();

            // 크기 조절 후 에디터 영역 벗어났는지 확인 및 위치 조정.
            movement.DelayAlignPosition();
        }

        public int CompareTo(MCComment other)
        {
            if (GetTopPosition.y > other.GetTopPosition.y)
            {
                return -1;
            }
            if (GetTopPosition.y < other.GetTopPosition.y)
            {
                return 1;
            }

            return 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Utils.IsLeftButtonClicked(eventData) == false || HasMoved == true)
            {
                return;
            }

            if (HasMoved == false && multiUnSelect == true && IsSelected == true)
            {
                OnSelect(false);
            }
            else if (HasMoved == false)
            {
                if (KeyInputManager.Instance.shouldMultiSelect == false 
                    && MCCommentManager.Instance.CurrentSelectedCommentCount > 0)
                {
                    MCCommentManager.Instance.SetOneSelected(this);
                }
            }
        }

        private bool HasMoved { get { return refRect.position != originPosition; } }
    }
}