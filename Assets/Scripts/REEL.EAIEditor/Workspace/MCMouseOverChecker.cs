using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCMouseOverChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float fireStartEventTime = 1f;

        public bool IsHover { get; private set; }
        private bool hasFireStartEvent = false;
        private float elapsedTime = 0f;

        private Vector2 prevMousePosition = Vector2.zero;

        public UnityEvent OnMouseOverStartEvent;
        public UnityEvent OnMouseOverEndEvent;

        private void OnEnable()
        {
            if (OnMouseOverStartEvent == null)
            {
                OnMouseOverStartEvent = new UnityEvent();
            }
            if (OnMouseOverEndEvent == null)
            {
                OnMouseOverEndEvent = new UnityEvent();
            }
        }

        private void Update()
        {
            if (IsHover && !hasFireStartEvent)
            {
                if (elapsedTime < fireStartEventTime)
                {
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    hasFireStartEvent = true;
                    OnMouseOverStartEvent?.Invoke();
                }

                if (prevMousePosition.Equals(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) == false)
                {
                    elapsedTime = 0f;
                }

                prevMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseOverEndEvent?.Invoke();
            ResetState();
        }

        public void ResetState()
        {
            IsHover = false;
            hasFireStartEvent = false;
            elapsedTime = 0f;
            prevMousePosition = Vector2.zero;
        }

        public void ResetStateButIsHover()
        {
            IsHover = true;
            hasFireStartEvent = false;
            elapsedTime = 0f;
            prevMousePosition = Vector2.zero;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            prevMousePosition = eventData.position;
            IsHover = true;
        }

        public void SetOnMouseOverStartEvent(UnityAction action)
        {
            OnMouseOverStartEvent.AddListener(action);
        }

        public void SetOnMouseOverEndEvent(UnityAction action)
        {
            OnMouseOverEndEvent.AddListener(action);
        }
    }
}