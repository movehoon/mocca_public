using TMPro;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class SwitchNodeWindow : MCNodeWindow
    {
        private const int minCount = 2;
        private const int maxCount = 5;
        private const float itemHeight = 50f;
        private const float defaultHeight = 100f;
        private bool isInputConnected = false;

        private TMP_InputField[] inputFields;
        private MCSwitchNode switchNode;

        private List<SwitchElemBlock> switchElems = new List<SwitchElemBlock>();

        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject valuePrefab;
        [SerializeField] private TMP_InputField inputValue;

        protected override void OnEnable()
        {
            foreach (var item in switchElems)
            {
                item.SetManager(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveAllListener();
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);
            switchNode = node as MCSwitchNode;

            int connectedNodeID = switchNode.GetNodeInputWithIndex(0).input.source;
            MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);

            inputFields = node.GetComponentsInChildren<TMP_InputField>();

            if (connectedNode == null) //input연결 x
            {
                isInputConnected = false;
                inputValue.interactable = true;
                InitValueItems(inputFields.Length - 1);
            }
            else if (connectedNode is MCGetNode)
            {
                isInputConnected = true;
                inputValue.interactable = false;
                InitValueItems(inputFields.Length);

                MCGetNode getNode = connectedNode as MCGetNode;
                inputValue.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
            }
            else
            {
                isInputConnected = true;
                InitValueItems(inputFields.Length);
                onOffBlocks[0].SetActive(false);
            }
        }

        private void RemoveAllListener()
        {
            for (int i = 0; i < switchElems.Count; i++)
            {
                switchElems[i].inputText.onValueChanged.RemoveAllListeners();
            }

            inputValue.onValueChanged.RemoveAllListeners();
        }

        private void SyncInputFields()
        {
            RemoveAllListener();
            inputFields = refNode.GetComponentsInChildren<TMP_InputField>();

            for (int i = 0; i < switchElems.Count; i++)
            {
                switchElems[i].inputText.text = inputFields[i].text;
                switchElems[i].inputText.onValueChanged.AddListener(SyncValuePropertyToNode);
            }

            if (!isInputConnected)
            {
                inputValue.text = inputFields[inputFields.Length - 1].text;
                inputValue.onValueChanged.AddListener(SyncInputPropertyToNode);
            }
        }

        private void SyncInputPropertyToNode(string input)
        {
            input = inputValue.text;
            inputFields[inputFields.Length - 1].text = input;
        }

        private void SyncValuePropertyToNode(string input)
        {
            for (int i = 0; i < switchElems.Count; i++)
            {
                input = switchElems[i].inputText.text;
                inputFields[i].text = input;
            }
        }

        private void ClearValueItems()
        {
            for (int i = 0; i < switchElems.Count; i++)
            {
                Destroy(switchElems[i].gameObject);
            }

            switchElems.Clear();
            content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight);
        }

        private void InitValueItems(int count)
        {
            ClearValueItems();
            for (int i = 0; i < count; i++)
            {
                AddValueItem();
            }

            SyncInputFields();
        }

        public void AddbuttonClicked()
        {
            if (switchElems.Count == maxCount) return;

            AddValueItem();
            switchNode.AddSwitchElem();

            SyncInputFields();

            refNode.GetComponent<MCNodeDrag>().ExecuteOnChanged();
        }

        private void AddValueItem()
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + itemHeight);

            GameObject valueItem = Instantiate(valuePrefab, content);
            SwitchElemBlock newBlock = valueItem.GetComponent<SwitchElemBlock>();
            newBlock.SetManager(this);
            switchElems.Add(newBlock);

            ChangeInputValueOrder();
        }

        public void RemoveButtonClicked(SwitchElemBlock item)
        {
            if (switchElems.Count == minCount) return;

            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y - itemHeight);

            switchNode.RemoveSwitchElem(switchElems.IndexOf(item));

            switchElems.Remove(item);
            Destroy(item.gameObject);

            SyncInputFields();

            ChangeInputValueOrder();

            refNode.GetComponent<MCNodeDrag>().ExecuteOnChanged();
        }

        private void ChangeInputValueOrder()
        {
            int currentChildCount = content.transform.childCount;
            GameObject inputValueObject = inputValue.transform.parent.gameObject;
            inputValueObject.transform.SetSiblingIndex(currentChildCount - 1);
        }
    }
}