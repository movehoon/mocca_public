using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCNodeSelect : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private MCNode refNode;
        private Vector3 originPosition;
        private RectTransform refRT;
        private MCWorkspaceManager manager;
        private KeyInputManager inputManager;
        private MCCommentManager commentManager;

        private bool multiUnSelect = false;

        private void OnEnable()
        {
            if (refNode == null)
            {
                refNode = GetComponent<MCNode>();
            }

            if (manager == null)
            {
                manager = MCWorkspaceManager.Instance;
            }

            if (inputManager == null)
            {
                inputManager = KeyInputManager.Instance;
            }

            if (commentManager == null)
            {
                commentManager = MCCommentManager.Instance;
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 마우스 눌렀을 때 툴팁 나타나지 않도록 함
            MCTooltipManager.Instance.CanShowPopup(false);

            // 마우스 눌렀을 때 코멘트 선택 해제.
            commentManager.OnNodePointDown();

            ReleaseFollow();

            if (IsSimulationOrNotLeftButton(eventData))
            {
                return;
            }

            originPosition = refRT.position;


            if (IsMultiSelectMode)
            {
                if (refNode.IsSelected == false)
                {
                    refNode.IsSelected = true;
                    multiUnSelect = false;
                }
                else
                {
                    multiUnSelect = true;
                }
            }
            else
            {
                if (refNode.IsSelected == false)
                {
                    manager.SetOneSelected(refNode);
                }

            }

            PropertyWindowManager.Instance.ShowProperty(refNode);
            BringToFront();

            MCEditorManager.Instance.GetPopup(MCEditorManager.PopupType.VariableContext).HidePopup();

            if (inputManager.isShiftPressed)
            {
                BuildMovementFollow();
            }
        }

        private void BringToFront()
        {
            refRT.SetAsLastSibling();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //마우스 뗐을때 툴팁다시 가능하도록 함
            MCTooltipManager.Instance.CanShowPopup(true);

            ReleaseFollow();

            if (IsSimulationOrNotLeftButton(eventData) || HasMoved)
            {
                return;
            }

            if (HasMoved == false && multiUnSelect == true && refNode.IsSelected == true)
            {
                refNode.IsSelected = false;
            }
            else if (HasMoved == false)
            {
                if (!inputManager.shouldMultiSelect && manager.CurrentSelectedBlockCount > 0)
                {
                    manager.SetOneSelected(refNode);
                }
            }
        }

        private void ReleaseFollow()
        {
            var movementList = FindObjectsOfType<MCNodeMovementFollow>();

            foreach (var move in movementList)
            {
                move.SetReleased();
            }
        }

        private void BuildMovementFollow()
        {
            manager.BuildMovementFollow();
        }

        private bool IsSimulationOrNotLeftButton(PointerEventData eventData)
        {
            //return MCWorkspaceManager.Instance.IsSimulation || eventData.button != PointerEventData.InputButton.Left;
            return MCPlayStateManager.Instance.IsSimulation || eventData.button != PointerEventData.InputButton.Left;
        }

        private bool CanSelect
        {
            get
            {
                return manager.CurrentSelectedBlockCount == 0
                    || inputManager.shouldMultiSelect;
            }
        }

        private bool IsMultiSelectMode
        {
            get
            {
                return inputManager.shouldMultiSelect;
            }
        }


        private bool HasMoved { get { return refRT.position != originPosition; } }
    }
}