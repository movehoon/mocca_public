using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCCommentBody : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Serializable]
        public class NodeInComment
        {
            public MCNode node;
            public Vector3 offset;
        }

        //private List<NodeInComment> nodes = new List<NodeInComment>();
        private Dictionary<int, NodeInComment> nodes = new Dictionary<int, NodeInComment>();
        private MCComment refComment;
        private RectTransform refRect;

        [Header("Event")]
        public UnityEvent OnCommentBodyDown;

        private void OnEnable()
        {
            if (refComment == null)
            {
                refComment = GetComponentInParent<MCComment>();
            }

            if (refRect == null)
            {
                refRect = GetComponent<RectTransform>();
            }

            //refComment.SubscribeOnCommentMoveStart(OnMoveStart);
            //refComment.SubscribeOnCommentMove(OnMove);
            //refComment.SubscribeOnCommentMoveEnd(OnMoveEnd);

            TestEmptyFunc();
        }

        private void OnDisable()
        {
            if (refComment != null)
            {
                refComment.UnSubscribeOnCommentMoveStart(OnMoveStart);
                refComment.UnSubscribeOnCommentMove(OnMove);
                refComment.UnSubscribeOnCommentMoveEnd(OnMoveEnd);
            }
        }

        float elapsedTime = 0f;
        float interval = 0.1f;
        public bool isMoving = false;

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= interval)
            {
                elapsedTime = 0f;

                if (isMoving == false)
                {
                    GetNodesInCommentBox();
                }
            }
        }

        public void OnMoveStart()
        {
            TestEmptyFunc();
        }

        public void OnMove()
        {
            //foreach (NodeInComment node in nodes)
            //{
            //    node.node.transform.position = refRect.position + node.offset;
            //    node.node.GetComponent<MCNodeDrag>().ExecuteOnChanged();
            //}

            foreach (var data in nodes)
            {
                NodeInComment node = data.Value;

                if (node.node == null)
                {
                    //Utils.Log("[MCCommentBody.OnMove] Node is null");
                    nodes.Clear();
                    GetNodesInCommentBox();
                    OnMove();
                    break;
                }

                node.node.transform.position = refRect.position + node.offset;
                node.node.GetComponent<MCNodeDrag>().ExecuteOnChanged();
            }

            // 주석 Depth 갱신.
            MCCommentManager.Instance.UpdateCommentsDepth();
        }

        public void SetPosition(Vector3 position)
        {
            refComment.transform.position = position;
            OnMove();
        }

        public void OnMoveEnd()
        {
            //Utils.LogBlue("[MCCommentBody.OnMoveEnd]");
        }

        public void TestEmptyFunc()
        {
            // 기존에 GetNodesInCommentBox() 메소드를 참조해 호출하던 지점을 기억하기 위해 임시로 추가한 빈 메소드.
        }

        public void GetNodesInCommentBox()
        {
            //nodes.Clear();
            List<MCNode> allNodes = MCWorkspaceManager.Instance.LocatedNodes;
            foreach (MCNode node in allNodes)
            {
                // 코멘트가 노드를 포함하는지 확인하는 로직 업데이트.
                // 월드 좌표 기준 TopLeft와 BottomRight를 구해서 좌표를 각각 비교하는 방식으로 직접 구현.
                if (ContainsRect(node.RefRect) == true)
                {
                    // 이미 코멘트에 추가돼 있지 않은 노드면 정보 초기화해서 추가.
                    if (nodes.ContainsKey(node.gameObject.GetInstanceID()) == false)
                    {
                        NodeInComment info = new NodeInComment();
                        info.node = node;
                        info.offset = node.transform.position - transform.position;
                        nodes.Add(node.gameObject.GetInstanceID(), info);
                        //nodes.Add(info);

                        // 코멘트 박스에 추가된 상태에서, 노드만 선택해 움직일 때 오프셋 업데이트를 위해 이벤트 등록.
                        node.GetComponent<MCNodeDrag>().SubscribeOnBeginDrag(OnNodeBeginDrag);

                        //Utils.LogRed($"Node Added: {node.gameObject.name}");
                    }

                    // 이미 코멘트에 추가돼 있는 노드인 경우에는 위치 오프셋만 업데이트.
                    else
                    {
                        if (nodes.TryGetValue(node.gameObject.GetInstanceID(), out NodeInComment info) == true)
                        {
                            info.offset = node.transform.position - transform.position;
                            //node.transform.position = refRect.position + info.offset;
                            //Utils.LogRed($"Change node offset: {node.gameObject.name}");
                        }
                    }
                }
                
                // 코멘트에 포함돼 있던 노드가 코멘트 박스를 벗어나면,
                // 코멘트 박스가 가지고 있는 노드 목록에서 제거.
                // 노드 위치 오프셋 업데이트를 위해 등록했던 이벤트도 등록 해제.
                if (ContainsRect(node.RefRect) == false && nodes.ContainsKey(node.gameObject.GetInstanceID()) == true)
                {
                    nodes.Remove(node.gameObject.GetInstanceID());
                    node.GetComponent<MCNodeDrag>().UnsubscribeOnBeginDrag(OnNodeBeginDrag);
                    //Utils.LogRed($"Node Removed: {node.gameObject.name}");
                }

                #region 기존 코드 백업
                //if (RectTransformUtility.RectangleContainsScreenPoint(refRect, node.transform.position))
                //{
                //    NodeInComment info = new NodeInComment();
                //    info.node = node;
                //    info.offset = node.transform.position - refRect.position;
                //    nodes.Add(info);
                //}
                #endregion
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            //if (refComment.IsSelected)
            //{
            //    return;
            //}

            TestEmptyFunc();
            refComment.OnPointerDown(eventData);

            //if (KeyInputManager.Instance.shouldMultiSelect && refComment.IsSelected)
            //{
            //    refComment.OnSelect(false);
            //}
            //else
            //{
            //    MCCommentManager.Instance.AddToSelection(refComment);

            //    // 주석(Comment) 눌림 이벤트 알림 (노드 선택 해제를 위해).
            //    MCWorkspaceManager.Instance.OnCommentPointDown();
            //}

            OnCommentBodyDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼이 눌렸는지 확인.
            if (Utils.IsLeftButtonClicked(eventData) == false)
            {
                return;
            }

            TestEmptyFunc();
        }

        public void SubscribeOnCommentDown(UnityAction func)
        {
            OnCommentBodyDown.AddListener(func);
        }

        public void UnSubscribeOnCommentDown(UnityAction func)
        {
            OnCommentBodyDown.RemoveListener(func);
        }

        private void OnNodeBeginDrag(int nodeInstanceID)
        {
            try
            {
                if (nodes.TryGetValue(nodeInstanceID, out NodeInComment info) == true)
                {
                    info.offset = info.node.transform.position - transform.position;

                    //Utils.LogGreen($"Change node's offset by node's movement, node name: {info.node.gameObject}");
                }
            }
            catch (Exception ex)
            {
                Utils.LogRed($"[MCCommentBody] OnNodeBeginDrag exception: {ex.ToString()}");
            }
        }

        private void GetTopLeftAndBottomRightFromRect(RectTransform rect, out Vector3[] corners)
        {
            corners = new Vector3[2];

            Vector3[] worldCorners = new Vector3[4];
            rect.GetWorldCorners(worldCorners);

            // TopLeft와 BottomRight를 구하기 전에 일단 처음 값 설정.
            corners[0] = worldCorners[0];
            corners[1] = worldCorners[0];

            foreach (var corner in worldCorners)
            {
                // TopLeft 구하기.
                corners[0].x = Mathf.Min(corners[0].x, corner.x);
                corners[0].y = Mathf.Max(corners[0].y, corner.y);

                // BottomRight 구하기.
                corners[1].x = Mathf.Max(corners[1].x, corner.x);
                corners[1].y = Mathf.Min(corners[1].y, corner.y);
            }
        }

        private bool ContainsRect(RectTransform rect)
        {
            Vector3[] commentArea;
            GetTopLeftAndBottomRightFromRect(refRect, out commentArea);

            Vector3[] rectArea;
            GetTopLeftAndBottomRightFromRect(rect, out rectArea);

            //for (int ix = 0; ix < commentArea.Length; ++ix)
            //{
            //    Utils.LogRed($"commentArea[{ix}]: {commentArea[ix]}");
            //}

            //for (int ix = 0; ix < rectArea.Length; ++ix)
            //{
            //    Utils.LogGreen($"rectArea[{ix}]: {rectArea[ix]}");
            //}

            if ((commentArea[0].x <= rectArea[0].x) && (commentArea[1].x >= rectArea[1].x) &&
                (commentArea[0].y >= rectArea[0].y) && (commentArea[1].y <= rectArea[1].y))
            {
                //Debug.Log("Contains");
                return true;
            }

            return false;
        }

        private void OnDestroy()
        {
            if (MCWorkspaceManager.Instance.LocatedNodes != null)
            {
                List<MCNode> allNodes = MCWorkspaceManager.Instance.LocatedNodes;
                foreach (MCNode node in allNodes)
                {
                    node.GetComponent<MCNodeDrag>().UnsubscribeOnBeginDrag(OnNodeBeginDrag);
                }
            }

            nodes.Clear();
            nodes = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            refComment.OnPointerClick(eventData);
        }
    }
}