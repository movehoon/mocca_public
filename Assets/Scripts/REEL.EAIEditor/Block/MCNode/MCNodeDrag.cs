using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
	public class MCNodeDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        protected Action OnChanged;
        protected bool canDrag = true;
        protected Vector3 dragOffset;
        protected RectTransform refRT;
		protected MCNodeMovement movement;

        private Action<int> OnBeginDragEvent;

        public Vector3 DragOffset
        {
            get
            {
                return dragOffset;
            }
        }

        private void Awake()
		{
            if (GetComponent<MCNodeMovement>() == null)
            {
                gameObject.AddComponent<MCNodeMovement>();
            }
			
            if (GetComponent<MCNodeMovementFollow>() == null)
            {
                gameObject.AddComponent<MCNodeMovementFollow>();
            }
		}

		private void OnEnable()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }
            
            if (movement == null)
            {
                movement = GetComponent<MCNodeMovement>();
            }
		}

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(TutorialManager.IsPlaying) return;

            if (!IsLeftButton(eventData))
            {
                return;
            }
            
            if (!canDrag)
            {
                return;
            }

            ExecuteOnBeginDrag();

            MCWorkspaceManager.Instance.SetDragOffset(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(TutorialManager.IsPlaying) return;

            if(!canDrag || !IsLeftButton(eventData))
            {
                return;
            }

            MCNodeDragManager.Instance.DragNode(eventData, GetComponent<MCNode>());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(TutorialManager.IsPlaying) return;

            SetEnableDrag(true);

            // 노드 이동 명령 이동 목적 지점 갱신.
            MCNodeDragManager.Instance.UpdateNodeMoveCommand();
        }
        
        public void SetDragOffset(Vector3 pointerPosition)
        {
            dragOffset = refRT.position - pointerPosition;
        }

        public void ChangedPosition(PointerEventData eventData)
        {
            refRT.position = (Vector3)eventData.position + dragOffset;
            ExecuteOnChanged();
        }

		public void SetPosition(Vector2 position)
		{
			Vector3 newPos = position;
			newPos.z = transform.localPosition.z;

			transform.localPosition = newPos;
			ExecuteOnChanged();
		}

        public void SetEnableDrag(bool canDrag)
        {
            this.canDrag = canDrag;
        }

        public void ExecuteOnChanged()
        {
            OnChanged?.Invoke();
        }

        public void SubscribeOnChanged(Action update)
        {
            OnChanged += update;
        }

        public void UnsubscribeOnChanged(Action update)
        {
            OnChanged -= update;
        }

        private void ExecuteOnBeginDrag()
        {
            //Utils.LogGreen($"ExecuteOnBeginDrag: {gameObject.GetInstanceID()}");
            OnBeginDragEvent?.Invoke(gameObject.GetInstanceID());
        }

        public void SubscribeOnBeginDrag(Action<int> callback)
        {
            OnBeginDragEvent += callback;
        }

        public void UnsubscribeOnBeginDrag(Action<int> callback)
        {
            OnBeginDragEvent -= callback;
        }

        private bool IsLeftButton(PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Left;
        }
    }
}