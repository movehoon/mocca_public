using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using REEL.PROJECT;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
	public class MCEditorManager : Singleton<MCEditorManager>
	{
		public enum PaneType
        {
            Graph_Pane, Variable_Pane, Function_Pane, Length
        }

        public enum PopupType
        {
            VariableList, VariableContext, FunctionList, WebcamPopup, WebcamPopupFullScreen, Length
        }

        [SerializeField]
        private GameObject[] panes;

        [SerializeField]
        private MCPopup[] popups;

        [SerializeField]
        private MCNode[] nodePrefabs;

        [SerializeField]
        private GraphicRaycaster raycaster;

        public GameObject linePrefab;

        public RectTransform LineParentTransform;

        [SerializeField]
        private NodeDragInfo dragInfo;

        public bool isLocalVariable = false;

        private Dictionary<NodeType, MCNode> prefabDictionary = new Dictionary<NodeType, MCNode>();
        private Dictionary<string, GameObject> paneDictionary = new Dictionary<string, GameObject>();

        public GraphicRaycaster UIRaycaster { get { return raycaster; } }

        private void OnEnable()
        {
            InitPrefabDictionary();
            InitPaneDictionary();
        }

        private void InitPrefabDictionary()
        {
            prefabDictionary = new Dictionary<NodeType, MCNode>();
            foreach (MCNode node in nodePrefabs)
            {
                prefabDictionary.Add(node.nodeData.type, node);
            }
        }

        private void InitPaneDictionary()
        {
            paneDictionary = new Dictionary<string, GameObject>();
            foreach (GameObject pane in panes)
            {
                paneDictionary.Add(pane.tag, pane);
            }
        }

        public MCNode GetNodePrefab(NodeType nodeType)
        {
            if (prefabDictionary.TryGetValue(nodeType, out MCNode retItem))
            {
                return retItem;
            }

            return null;
        }

        public GameObject CreateNewLineGO()
        {
            GameObject newLine = Instantiate(linePrefab);
            newLine.transform.SetParent(LineParentTransform, false);
            newLine.transform.localPosition = Vector3.zero;

            return newLine;
        }

        public GameObject GetPane(PaneType paneType)
        {
            int length = (int)PaneType.Length;
            for (int ix = 0; ix < length; ++ix)
            {
                GameObject pane;
                string typeName = ((PaneType)(ix)).ToString();
                if (paneDictionary.TryGetValue(typeName, out pane))
                    return pane;
            }

            return null;
        }

        public MCPopup GetPopup(PopupType popupType)
        {
            if (popupType == PopupType.Length)
            {
                return null;
            }

            return popups[(int)popupType];
        }

        public void CloseAllPopups()
        {
            foreach (MCPopup popup in popups)
            {
                if (popup != null)
                {
                    popup.HidePopup();
                }
            }
        }

        public bool IsAnyPopupActive
        {
            get
            {
                foreach (var popup in popups)
                {
                    if (popup.gameObject.activeSelf == true)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void DragInfoSetActive(bool isActive)
        {
            dragInfo.SetActive(isActive);
        }

        public void DragInfoSetPosition(Vector3 position)
        {
            dragInfo.SetPosition(position);
        }

        public void DragInfoSetSprite(Sprite sprite)
        {
            dragInfo.SetSprite(sprite);
        }

        public void DragInfoSetText(string text)
        {
            dragInfo.SetText(text);
        }

        public void SetIsLocalVariable(bool isLocalVariable)
        {
            this.isLocalVariable = isLocalVariable;
        }

        //public string DragInfoText {  get { return dragInfo.text.text; } }
        public string DragInfoText { get { return dragInfo.Text; } }
        public bool DragInfoIsActive { get { return dragInfo.IsActive; } }

        public bool IsOnGraphPane(List<RaycastResult> results)
        {
            if (!MCWorkspaceManager.Instance.isActiveAndEnabled)
            {
                return false;
            }

            foreach (RaycastResult result in results)
            {
                if (Util.CompareTwoStrings(result.gameObject.tag, PaneType.Graph_Pane.ToString()))
                    return true;
            }

            return false;
        }
    }
}