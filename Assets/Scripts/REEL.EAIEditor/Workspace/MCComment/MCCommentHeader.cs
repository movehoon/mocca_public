using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCCommentHeader : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
        IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        public MCCommentBody body;
        public RectTransform commentRect;
        public TMP_Text titleText;
        public TMP_InputField titleInput;

        private MCComment refComment;
        private Vector2 offset;

        [Header("Event")]
        public UnityEvent OnHeaderBeginDrag;
        public UnityEvent OnHeaderPointerUp;
        public UnityEvent OnHeaderPointDown;
        public UnityEvent OnTitleInputSelectEnd;

        private readonly string defaultCommentText = "코멘트";

        private string titleBackup = string.Empty;

        private void OnEnable()
        {
            if (refComment == null)
            {
                refComment = commentRect.GetComponent<MCComment>();
            }

            if (titleInput != null)
            {
                titleInput.onSubmit.AddListener(SetTitle);
            }

            //if (body != null)
            //{
            //    body.SubscribeOnCommentDown(OnSelectEnd);
            //}

            Utils.GetGraphPane().SubscribeOnPointDown(OnSelectEnd);
        }

        private void OnDisable()
        {
            //if (body != null)
            //{
            //    body.UnSubscribeOnCommentDown(OnSelectEnd);
            //}

            Utils.GetGraphPane().UnSubscribeOnPointDown(OnSelectEnd);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                body.isMoving = true;
            }

            if (Input.GetMouseButtonUp(0) == true)
            {
                body.isMoving = false;
            }
        }

        //private void BackupTitleText()
        //{
        //    titleBackup = titleText.text;
        //}

        //private void RestoreTitleFromBackupText()
        //{
        //    titleText.text = titleBackup;
        //}

        public void OnSelectEnd()
        {
            //titleText.text = Utils.IsNullOrEmptyOrWhiteSpace(titleInput.text) ? titleText.text : titleInput.text;
            //if (Utils.IsNullOrEmptyOrWhiteSpace(titleText.text) == true
            //    && Utils.IsNullOrEmptyOrWhiteSpace(titleInput.text) == true)
            //{
            //    RestoreTitleFromBackupText();
            //}

            SetTitleInputActive(false);
        }
        
        public void OnHeaderDoubleClicked()
        {
            titleInput.text = titleText.text;
            SetTitleInputActive(true);

            //titleText.text = string.IsNullOrEmpty(titleText.text) ? defaultCommentText : titleText.text;
            //titleInput.text = string.IsNullOrEmpty(titleText.text) ? defaultCommentText : titleText.text;
        }

        public void SetTitle(string title)
        {
            SetTitleInputActive(false);
            titleText.text = title;
        }

        private void SetTitleInputActive(bool isOn)
        {
            titleText.gameObject.SetActive(!isOn);
            titleInput.gameObject.SetActive(isOn);

            if (isOn == true)
            {
                titleInput.Select();
            }
            else
            {
                titleInput.text = string.Empty;
            }
        }

        //public void SetDragOffset(Vector2 pointerPosition)
        //{
        //    offset = new Vector2(commentRect.position.x, commentRect.position.y) - pointerPosition;
        //}

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            //offset = new Vector2(commentRect.position.x, commentRect.position.y) - eventData.position;
            //if (refComment != null)
            //{
            //    refComment.MoveCommentStart();
            //}

            MCCommentManager.Instance.SetDragOffset(eventData.position);
            OnHeaderBeginDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            //commentRect.position = eventData.position + offset;
            //if (refComment != null)
            //{
            //    refComment.MoveComment();
            //}

            MCCommentManager.Instance.DragComments(eventData, refComment);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            if (refComment != null)
            {
                refComment.MoveCommentEnd();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            OnHeaderPointerUp?.Invoke();
        }

        public void SubscribeOnHeaderBeginDrag(UnityAction func)
        {
            OnHeaderBeginDrag.AddListener(func);
        }

        public void UnSubscribeOnHeaderBeginDrag(UnityAction func)
        {
            OnHeaderBeginDrag.RemoveListener(func);
        }

        public void SubscribeOnHeaderPointerUp(UnityAction func)
        {
            OnHeaderPointerUp.AddListener(func);
        }

        public void UnSubscribeOnHeaderPointerUp(UnityAction func)
        {
            OnHeaderPointerUp.RemoveListener(func);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            //refComment.OnSelect(true);

            // 헤더가 선택되면 주석 영역 안에 있는 노드 목록(리스트) 확인.
            //body.GetNodesInCommentBox();

            body.OnPointerDown(eventData);

            //OnHeaderPointDown?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            refComment.OnPointerClick(eventData);
        }
    }
}