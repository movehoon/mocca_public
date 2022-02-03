using UnityEngine;
using UnityEngine.EventSystems;

using REEL.PROJECT;
using System;

namespace REEL.D2EEditor
{
    public class GraphPane : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ContentScaler contentScaler = null;
        [SerializeField] private Transform blockPane = null;
        [SerializeField] private RectTransform linePane = null;

        public RectTransform BlockPane { get { return blockPane.GetComponent<RectTransform>(); } }
        public RectTransform LinePane { get { return linePane; } }

        private Action OnPointerDownEvent;

        private Action<PointerEventData> OnPointerDownWithDataEvent;

        private Action<PointerEventData> OnPointerMoveEvent;

        private bool shouldCheckMousePosition = false;

        private void Update()
        {
            // 마우스 포인터가 작업창 안에 있을 떄는 마우스 포지션 위치 갱신해서 이벤트 호출.
            if (shouldCheckMousePosition == true)
            {
                // 마우스 위치 저장.
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = UnityEngine.Input.mousePosition;

                // 이벤트 호출.
                OnPointerMoveEvent?.Invoke(eventData);
            }
        }

        public void ResetContentScale()
        {
            contentScaler.ResetScale();
        }

        public void ResetAllContentState()
        {
            contentScaler.ResetAll();
        }

        public MCNode AddNode(Vector3 itemPosition, GameObject prefab, int nodeID = 0)
        {
            // Test.
            // 노드 추가하기 전에 현 상태 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            MCNode newNode = Instantiate(prefab, blockPane).GetComponent<MCNode>();

            // 노드 이름 변경하기.
            newNode.gameObject.name = newNode.nodeData.type.ToString() + "_" + (Utils.NewNodeNumber).ToString();

            newNode.GetComponent<RectTransform>().anchoredPosition = itemPosition;
            MCWorkspaceManager.Instance.AddNode(newNode, nodeID);

            //TutorialManager.SendEvent(newNode);

            return newNode;
        }

        public MCNode AddNode(Vector3 itemPosition, NodeType nodeType, int nodeID = 0, bool isLoaded = false , bool isLocalPos = false, bool isDuplicate = false)
        {
            MCNode nodePrefab = MCEditorManager.Instance.GetNodePrefab(nodeType);
            if (nodePrefab == null)
            {
                //Debug.Log("nodePrefab null, type: " + nodeType);
                Utils.LogRed("nodePrefab null, type: " + nodeType);
                return null;
            }

            //Debug.Log(string.Format("{0} / {1}", MCProjectManager.ProjectDescription == null, MCWorkspaceManager.Instance.LocatedNodes.Count));

            // Test.
            // 노드 추가하기 전에 현 상태 저장.
            //if (MCProjectManager.ProjectDescription != null
            //    && MCWorkspaceManager.Instance.LocatedNodes.Count > 0
            //    && isLoaded == false
            //    && isDuplicate == false)
            //{
            //    MCUndoRedoManager.Instance.RecordProject();
            //}

            // 노드 추가.
            MCNode newNode = Instantiate(nodePrefab.gameObject, blockPane).GetComponent<MCNode>();

            // 노드 이름 변경하기.
            newNode.gameObject.name = newNode.nodeData.type.ToString() + "_" + (Utils.NewNodeNumber).ToString();

            RectTransform rt = newNode.RefRect;
            if (nodeType == NodeType.START && MCWorkspaceManager.Instance.LocatedNodes.Count == 0)
            {
                rt.localPosition = itemPosition;
                newNode.DontDestroy = true;
            }
            else
            {
                //rt.position = itemPosition;
                rt.position = itemPosition + new Vector3(rt.sizeDelta.x * 0.5f, -rt.sizeDelta.y * 0.5f);
            }

			//프로젝트 로딩시에는 로컬 포지션으로 저장하고 로컬 포지션으로 생성함 kjh
			if (isLocalPos == true) 
			{
				newNode.transform.localPosition = itemPosition;
			}

            rt.localScale = Vector3.one;
            MCWorkspaceManager.Instance.AddNode(newNode, nodeID);

            TutorialManager.SendEvent(newNode);

            return newNode;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //BlockDiagramManager.Instance.SetAllUnselected();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (KeyInputManager.Instance.shouldMultiSelect) return;

            MCWorkspaceManager.Instance.SetAllUnSelected();
            PropertyWindowManager.Instance.TurnOffAll();

            OnPointerDownEvent?.Invoke();
            OnPointerDownWithDataEvent?.Invoke(eventData);
        }

        public void SubscribeOnPointDown(Action function)
        {
            OnPointerDownEvent += function;
        }

        public void SubscribeOnPointerDownWithData(Action<PointerEventData> function)
        {
            OnPointerDownWithDataEvent += function;
        }

        public void SubscribeOnPointerMoveEvent(Action<PointerEventData> function)
        {
            OnPointerMoveEvent += function;
        }

        public void UnSubscribeOnPointDown(Action function)
        {
            OnPointerDownEvent -= function;
        }

        public void UnSubscribeOnPointerDownWithData(Action<PointerEventData> function)
        {
            OnPointerDownWithDataEvent -= function;
        }

        public void UnSubscribeOnPointerMoveEvent(Action<PointerEventData> function)
        {
            OnPointerMoveEvent -= function;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            shouldCheckMousePosition = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            shouldCheckMousePosition = false;
        }
    }
}