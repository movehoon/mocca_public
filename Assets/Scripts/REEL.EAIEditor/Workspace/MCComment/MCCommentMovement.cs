using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCCommentMovement : MonoBehaviour
    {
        protected RectTransform refRT;
        private MCComment refComment;
        private MCCommentHeader refCommentHeader;

        protected bool isTargetMoving = false;

        protected Vector2 targetPosition = Vector2.zero;
        protected Vector2 velocity = Vector2.zero;
        protected float smoothTime = 0.125f;

        private void OnEnable()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }

            if (refComment == null)
            {
                refComment = GetComponent<MCComment>();
            }

            if (refCommentHeader == null)
            {
                refCommentHeader = GetComponentInChildren<MCCommentHeader>();
            }

            refCommentHeader.SubscribeOnHeaderBeginDrag(OnBeginDrag);
            refCommentHeader.SubscribeOnHeaderPointerUp(OnPointerUp);

            // 시작할 때 화면 벗어난 경우에 위치를 바로 잡기 위해 호출.
            DelayAlignPosition();
        }

        public void DelayAlignPosition()
        {
            Invoke("AlignPosition", 0.1f);
        }

        private void AlignPosition()
        {
            if (IsOutsideArea)
            {
                SetMoveTarget(GetPosition());
            }
        }

        private void OnDisable()
        {
            if (refCommentHeader != null)
            {
                refCommentHeader.UnSubscribeOnHeaderBeginDrag(OnBeginDrag);
                refCommentHeader.UnSubscribeOnHeaderPointerUp(OnPointerUp);
            }
        }

        Vector2 GetPosition()
        {
            return transform.localPosition;
        }

        Vector2 GetSize()
        {
            return refRT.sizeDelta;
        }

        Vector2 GetSizeHalf()
        {
            return GetSize() * 0.5f;
        }

        void SetPosition(Vector2 pos)
        {
            refComment.SetPosition(pos);
        }

        void Start()
        {
            InitCreateEffect();
        }

        private void InitCreateEffect()
        {
            var effect = gameObject.AddComponent<UIEffect>();
            effect.PlayScale(0.0f, 1.0f, 1.0f);
        }

        void LateUpdate()
        {
            if (isTargetMoving)
            {
                UpdateMove();
            }
        }

        private void UpdateMove()
        {
            var pos = Vector2.SmoothDamp(GetPosition(), targetPosition, ref velocity, smoothTime);

            SetPosition(pos);

            //이동 종료 체크
            if (Vector2.Distance(pos, targetPosition) < 1.5f)
            {
                SetPosition(pos);

                isTargetMoving = false;
                velocity = Vector2.zero;
            }
        }

        public void SetMoveTarget(Vector2 targetPos)
        {
            isTargetMoving = true;

            var pos = targetPos;
            var size = GetSizeHalf() * 1.05f;    //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

            // left right 비교
            pos.x = Mathf.Clamp(pos.x, Utils.EDITOR_AREA_LEFT + size.x, Utils.EDITOR_AREA_RIGHT - size.x);

            // top 비교
            if (pos.y > Utils.EDITOR_AREA_TOP - size.y) pos.y = Utils.EDITOR_AREA_TOP - size.y;

            // bottom 비교
            if (pos.y < Utils.EDITOR_AREA_BOTTOM + size.y) pos.y = Utils.EDITOR_AREA_BOTTOM + size.y;

            targetPosition = pos;
        }

        public void StopMove()
        {
            isTargetMoving = false;
        }

        public void OnBeginDrag()
        {
            StopMove();
        }

        public void OnPointerUp()
        {
            var movements = FindObjectsOfType<MCCommentMovement>();

            Array.ForEach(movements, x => x.CheckOutEditorArea());
        }

        public void CheckOutEditorArea()
        {
            if (enabled == false) return;

            var pos = GetPosition();
            var size = GetSizeHalf() * 1.2f; //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

            //left, top 비교
            if (pos.x < Utils.EDITOR_AREA_LEFT + size.x ||
                 pos.x > Utils.EDITOR_AREA_RIGHT - size.x ||
                 pos.y > Utils.EDITOR_AREA_TOP - size.y ||
                 pos.y < Utils.EDITOR_AREA_BOTTOM + size.y)
            {
                SetMoveInside();
            }
        }

        private bool IsOutsideArea
        {
            get
            {
                if (enabled == false)
                {
                    return false;
                }

                Vector2 pos = GetPosition();
                Vector2 size = GetSizeHalf() * 1.2f; //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

                //left, top 비교
                if (pos.x < Utils.EDITOR_AREA_LEFT + size.x ||
                     pos.x > Utils.EDITOR_AREA_RIGHT - size.x ||
                     pos.y > Utils.EDITOR_AREA_TOP - size.y ||
                     pos.y < Utils.EDITOR_AREA_BOTTOM + size.y)
                {
                    return true;
                }

                return false;
            }
        }

        private void SetMoveInside()
        {
            var pos = GetPosition();

            SetMoveTarget(pos);
        }
    }
}