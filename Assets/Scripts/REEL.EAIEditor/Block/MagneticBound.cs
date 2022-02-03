using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class MagneticBound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler 
        //IPointerEnterHandler, IPointerExitHandler
    {
        public MCNodeSocket nodeSocket = null;
        public MCNodeInputOutputBase inputoutputBase = null;
        private Image image;
        //private RawImage image;
        private RectTransform refRT;

        //private Color enableColor = Color.yellow;
        //private Color disableColor = new Color(1f, 1f, 1f, 0f);

        //private EventSystem eventSystem = null;
        //private PointerEventData eventData = null;
        //private List<RaycastResult> results;

        protected virtual void OnEnable()
        {
            //Initialize();
        }

        //protected virtual void Update()
        //{
        //    if (image != null)
        //    {
        //        if (nodeSocket == null)
        //        {
        //            nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
        //        }

        //        if (image.color != disableColor)
        //        {
        //            if (nodeSocket.HasLine == true)
        //            {
        //                ResetColor();
        //            }
        //        }

        //        if (image.color == disableColor)
        //        {
        //            if ((nodeSocket is MCNodeOutput) == false)
        //            {
        //                if (nodeSocket.HasLine == false && nodeSocket.line == null)
        //                {
        //                    RaycastToThisMagneticBound();
        //                }
        //            }

        //            else
        //            {
        //                MCNodeOutput output = nodeSocket as MCNodeOutput;
        //                if (output.HasLine == false && output.currentLine == null)
        //                {
        //                    RaycastToThisMagneticBound();
        //                }
        //            }
        //        }
        //    }
        //}

        //private void RaycastToThisMagneticBound()
        //{
        //    results = new List<RaycastResult>();
        //    eventData.position = Input.mousePosition;
        //    MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
        //    foreach (RaycastResult result in results)
        //    {
        //        if (result.gameObject.GetInstanceID().Equals(gameObject.GetInstanceID()) == true)
        //        {
        //            image.color = enableColor;
        //            break;
        //        }
        //    }

        //    results.Clear();
        //    results = null;
        //}

        public void SetSize(Vector2 size)
        {
            refRT.sizeDelta = size;
        }

        public void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }

        public void Initialize()
        {
            //if (eventSystem == null)
            //{
            //    eventSystem = FindObjectOfType<EventSystem>();
            //}

            //if (eventData == null)
            //{
            //    eventData = new PointerEventData(eventSystem);
            //}

            if (nodeSocket == null)
            {
                nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
            }

            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0f);
                //image.color = disableColor;
                //image = gameObject.AddComponent<RawImage>();
                //Texture texture = Resources.Load("EditorImages/Blocks/magneticboundarea") as Texture;
                //image.texture = texture;

                //ResetColor();
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
                //refRT.sizeDelta = new Vector2(35f, 25f);
                refRT.sizeDelta = new Vector2(60f, 22f);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(enabled == false) return;

            if (nodeSocket == null)
            {
                nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
            }

            nodeSocket.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(enabled == false) return;

            if(nodeSocket == null)
            {
                nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
            }

            nodeSocket.OnPointerUp(eventData);
            //ResetColor();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(enabled == false) return;

            if(nodeSocket == null)
            {
                nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
            }

            nodeSocket.OnDrag(eventData);
        }

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    if (nodeSocket == null)
        //    {
        //        nodeSocket = transform.parent.GetComponentInChildren<MCNodeSocket>();
        //    }

        //    if (nodeSocket.line == null)
        //    {
        //        if (nodeSocket is MCNodeOutput)
        //        {
        //            MCNodeOutput output = nodeSocket as MCNodeOutput;
        //            if (output.HasLine == false && output.currentLine == null)
        //            {
        //                image.color = enableColor;
        //            }
        //        }
        //        else
        //        {
        //            image.color = enableColor;
        //        }
        //    }
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    ResetColor();
        //}

        //private void ResetColor()
        //{
        //    image.color = disableColor;
        //}
    }
}