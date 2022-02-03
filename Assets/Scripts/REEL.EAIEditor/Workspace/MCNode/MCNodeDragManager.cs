using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCNodeDragManager : Singleton<MCNodeDragManager>
    {
        [SerializeField]
        private MCTables tables;

        [SerializeField]
        private MCWorkspaceManager manager;

        private void OnEnable()
        {
            if (tables == null)
            {
                tables = GetComponent<MCTables>();
            }

            if (manager == null)
            {
                manager = MCWorkspaceManager.Instance;
            }
        }

        public void DragNode(PointerEventData eventData, MCNode node = null)
        {
            List<MCNode> selectedNodes = new List<MCNode>();
            Constants.OwnerGroup ownerGroup = manager.CurrentTabOwnerGroup;
            if (ownerGroup == Constants.OwnerGroup.NONE)
            {
                return;
            }

            if (ownerGroup == Constants.OwnerGroup.PROJECT)
            {
                selectedNodes = tables.SelectedNodesInProject;
            }
            else if (ownerGroup == Constants.OwnerGroup.FUNCTION)
            {
                selectedNodes = MCFunctionTable.Instance
                    .GetSelectedNodesInFunction(Utils.CurrentTabName);
            }

            DragNode(selectedNodes, eventData, node);
        }

        private void DragNode(List<MCNode> selectedNodes, PointerEventData eventData, MCNode node = null)
        {
            if (node != null && selectedNodes.Count == 1)
            {
                //Debug.Log(selectedNodes[0].gameObject.GetInstanceID() + " : " + node.gameObject.GetInstanceID());

                if (!selectedNodes[0].gameObject.GetInstanceID().Equals(node.gameObject.GetInstanceID()))
                {
                    selectedNodes[0].IsSelected = false;

                    node.GetComponent<MCNodeDrag>().SetDragOffset(eventData.position);
                    node.IsSelected = true;

                    selectedNodes[0] = node;
                }
            }

            
            foreach (MCNode selected in selectedNodes)
            {
                MCNodeDrag nodeDrag = selected.GetComponent<MCNodeDrag>();
                nodeDrag.ChangedPosition(eventData);
            }

            //Utils.Log($"[MCNodeDragManager] SelectedNode.Count: {selectedNodes.Count}");

            //// 노드 이동 명령의 목적지(Destination) 설정.
            //if (MCCommandHelper.nodeMoveCommand != null)
            //{
            //    MCCommandHelper.nodeMoveCommand.SetDestinations(nodeDrags);

            //    // Undo/Redo 매니저에 추가.
            //    MCUndoRedoManager.Instance.AddCommandDontExecute(MCCommandHelper.nodeMoveCommand);

            //    // null 설정.
            //    MCCommandHelper.nodeMoveCommand = null;
            //}
        }

        public void UpdateNodeMoveCommand()
        {
            List<MCNode> selectedNodes = MCWorkspaceManager.Instance.SelectedNodes;
            List < MCNodeDrag > nodeDrags = new List<MCNodeDrag>();
            foreach (var node in selectedNodes)
            {
                if (node.IsSelected == true)
                {
                    nodeDrags.Add(node.GetComponent<MCNodeDrag>());
                }
            }

            // 노드 이동 명령의 목적지(Destination) 설정.
            if (MCCommandHelper.nodeMoveCommand != null)
            {
                MCCommandHelper.nodeMoveCommand.SetDestinations(nodeDrags);

                // Undo/Redo 매니저에 추가.
                MCUndoRedoManager.Instance.AddCommandDontExecute(MCCommandHelper.nodeMoveCommand);

                // null 설정.
                MCCommandHelper.nodeMoveCommand = null;
            }
        }

        // 드래그로 블록 선택하기.
        public void SetBlockSelectionWithDragArea(DragInfo dragInfo)
        {
            List<MCNode> targetNodes = new List<MCNode>();
            if (manager.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                targetNodes = tables.locatedNodes;
            }
            else if (manager.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                targetNodes = MCFunctionTable.Instance.LogicNodesOfCurrentFunction;
            }

            if (targetNodes.Count == 0) return;

			bool shouldMultiSelect = KeyInputManager.Instance.shouldMultiSelect;

			for (int ix = 0; ix < targetNodes.Count; ++ix)
            {
                RectTransform itemRect = targetNodes[ix].GetComponent<RectTransform>();

                if (IsOutsideOfDragArea(dragInfo, itemRect))
                {
                    // Shift 키가 눌린 경우에는 선택 해제하지 않고 진행.
                    if (shouldMultiSelect)	continue;

                    SetBlockUnSelected(targetNodes[ix]);
                    continue;
                }

                SetBlockSelectedByDrag(targetNodes[ix], shouldMultiSelect);
            }
        }

        public void SetCommentSelectionWithDragArea(DragInfo dragInfo)
        {
            List<MCComment> targetComments = MCCommentManager.Instance.locatedComments;

            bool shouldMultiSelect = KeyInputManager.Instance.shouldMultiSelect;

            for (int ix = 0; ix < targetComments.Count; ++ix)
            {
                RectTransform itemRect = targetComments[ix].GetComponent<RectTransform>();

                if (IsOutsideOfDragArea(dragInfo, itemRect))
                {
                    if (shouldMultiSelect)
                    {
                        continue;
                    }

                    MCCommentManager.Instance.SetCommentUnSelected(targetComments[ix]);
                    continue;
                }

                MCCommentManager.Instance.SetCommentSelected(targetComments[ix]);
            }
        }

        public void SetBlockUnSelected(MCNode node)
        {
            Constants.OwnerGroup ownerGroup = manager.CurrentTabOwnerGroup;
            List<MCNode> selected = new List<MCNode>();
            if (ownerGroup == Constants.OwnerGroup.PROJECT)
            {
                selected = tables.SelectedNodesInProject;

            }
            else if (ownerGroup == Constants.OwnerGroup.FUNCTION)
            {
                selected = MCFunctionTable.Instance
                    .GetSelectedNodesInFunction(Utils.CurrentTabName);
            }

            for (int ix = 0; ix < selected.Count; ++ix)
            {
                if (selected[ix].OwnedProcess.id.Equals(node.OwnedProcess.id) 
                    && selected[ix].NodeID.Equals(node.NodeID))
                {
                    selected[ix].IsSelected = false;
                    break;
                }
            }
        }

		public void SetBlockSelectedByDrag(MCNode node , bool xorSelect = false)
        {
            if (!tables.CheckIfBlockSelected(node))
            {
                node.IsSelected = true;
            }
        }

        bool IsOutsideOfDragArea(DragInfo dragInfo, RectTransform itemRect)
        {
            RectTransform pane = MCEditorManager.Instance.LineParentTransform;
            Vector2 localPoint = itemRect.anchoredPosition;
            float halfX = itemRect.sizeDelta.x * 0.5f;
            float halfY = itemRect.sizeDelta.y * 0.5f;

            Vector2 dragLocalPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(pane, dragInfo.topLeft, Camera.main, out dragLocalPoint);

            //
            Vector2 topLeft = WorldToLocalPointInRect(pane, dragInfo.topLeftMarker.position);
            Vector2 bottomRight = WorldToLocalPointInRect(pane, dragInfo.bottomRightMarker.position);

            float width = Mathf.Abs(topLeft.x - bottomRight.x);
            float height = Mathf.Abs(topLeft.y - bottomRight.y);

            //Debug.Log("width: " + width + " , height: " + height + " , halfX: " + halfX + " , halfY: " + halfY);

            return topLeft.x > localPoint.x + halfX ||
                    topLeft.x + width < localPoint.x - halfX ||
                    topLeft.y < localPoint.y - halfY ||
                    topLeft.y - height > localPoint.y + halfY;
        }

        private Vector2 WorldToLocalPointInRect(RectTransform rect, Vector3 worldPos)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

            Vector2 localPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, Camera.main, out localPoint);

            return localPoint;
        }
    }
}