using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
	public class MouseOverChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
        public float showTooltipTime = 1f;

        public bool IsHover { get; private set; }
        private bool hasShownTooltip = false;
        private float elapsedTime = 0f;

        private Vector2 prevMousePosition = Vector2.zero;

        private Action showTooltipFunction;
        private Action closeTooltipFunction;

        private void Update()
        {
            if (IsHover && !hasShownTooltip)
            {
                if (elapsedTime < showTooltipTime)
                {
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    hasShownTooltip = true;
                    showTooltipFunction?.Invoke();
                }

                if (prevMousePosition.Equals(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) == false)
                {
                    elapsedTime = 0f;
                }

                prevMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        public void SetShowTooltipFunction(Action function)
        {
            showTooltipFunction = function;
        }

        public void SetCloseTooltipFunction(Action function)
        {
            closeTooltipFunction = function;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            prevMousePosition = eventData.position;
            IsHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            closeTooltipFunction?.Invoke();
            IsHover = false;
            hasShownTooltip = false;
            elapsedTime = 0f;
            prevMousePosition = Vector2.zero;
        }
    }
}