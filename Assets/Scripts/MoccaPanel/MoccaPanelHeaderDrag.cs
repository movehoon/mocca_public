using DynamicPanels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mocca
{
    public class MoccaPanelHeaderDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private PanelHeader panelHeader;

        private void Awake()
        {
            panelHeader = GetComponentInParent<Panel>().GetComponentInChildren<PanelHeader>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            panelHeader.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            panelHeader.OnDrag(eventData);
            Debug.Log("OnDrag: " + gameObject);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            panelHeader.OnEndDrag(eventData);
        }
    }
}