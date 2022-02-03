using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace REEL.D2EEditor
{
    public class NodeListGenerator : Editor
    {
        private static readonly string path = "Assets/Scripts/REEL.EAIEditor/Editor/NodeList.json";

        private static readonly string categoryPrefabPath = "NodeListPrefabs/Category";
        private static readonly string nodeListItemPrefabPath = "NodeListPrefabs/Node List Item";
        private static readonly string categoryDividerPrefabPath = "NodeListPrefabs/Category Divider";

        private static float itemHeight = 36f;
        private static float dividerHeight = 25f;
        private static float heightOffset = 0f;

        private static float offset = 1f;

        [MenuItem("Window/MOCCA Tooltip/Set Tooltip Node Name")]
        static void SetToopTipNodeName()
        {
            string jsonString = File.ReadAllText("Assets/Scripts/REEL.EAIEditor/Editor/NodeList.json");
            NodeList list = JsonUtility.FromJson<NodeList>(jsonString);

            string path = "Assets/ScriptableObjectData/TooptipData/";

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    if (file.Contains("meta") == true)
                    {
                        continue;
                    }

                    var tooltipData = AssetDatabase.LoadAssetAtPath(file, typeof(MCNodeTooltipData)) as MCNodeTooltipData;
                    foreach (var category in list.categories)
                    {
                        foreach (var item in category.items)
                        {
                            //Debug.Log($"ToolTip.NodeType: {tooltipData.nodeType} / item.NodeType: {item.nodeType}");
                            if (tooltipData.nodeType.ToString().Equals(item.nodeType) == true)
                            {
                                //Debug.Log("Here");
                                tooltipData.nodeName = item.listName;
                                EditorUtility.SetDirty(tooltipData);
                                break;
                            }
                        }
                    }

                    //Debug.Log($"File: {file}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //[MenuItem("Window/MOCCA Config/Generate Node List File")]
        static void CreateNodeListFile()
        {
            NodeList list = GenerateNodeList();
            string jsonString = JsonUtility.ToJson(list, true);
            File.WriteAllText(path, jsonString);
        }

        [MenuItem("Window/MOCCA Config/Generate Node List")]
        static void MakeListObjects()
        {
            heightOffset = 0f;
            float totalHeight = 0f;
            Transform leftMenuRoot = Selection.activeTransform;

            int count = leftMenuRoot.childCount;
            if (count > 0)
            {
                for (int ix = count - 1; ix >= 0; ix--)
                {
                    DestroyImmediate(leftMenuRoot.GetChild(ix).gameObject);
                }
            }

            string jsonString = File.ReadAllText(path);
            NodeList list = JsonUtility.FromJson<NodeList>(jsonString);

            heightOffset = 0f;

            BlockCategoryManager categoryManager = leftMenuRoot.GetComponentInParent<BlockCategoryManager>();
            categoryManager.categories = new BlockCategory[list.categories.Count];

            // Get prefab reference.
            GameObject categoryDividerPrefab = Resources.Load(categoryDividerPrefabPath) as GameObject;
            GameObject categoryPrefab = Resources.Load(categoryPrefabPath) as GameObject;
            GameObject listItemPrefab = Resources.Load(nodeListItemPrefabPath) as GameObject;
            for (int ix = 0; ix < list.categories.Count; ++ix)
            {
                NodeCategory currentCategory = list.categories[ix];

                Vector3 pos;
                GameObject categoryDivider = null;
                bool hasDivider = false;
                if (ix > 0 && ix < list.categories.Count)
                {
                    categoryDivider = Instantiate(categoryDividerPrefab, leftMenuRoot) as GameObject;
                    pos = categoryDivider.transform.localPosition;
                    pos.y = heightOffset;
                    categoryDivider.transform.localPosition = pos;
                    heightOffset -= dividerHeight;
                    totalHeight += dividerHeight;
                    hasDivider = true;
                }
                
                GameObject category = Instantiate(categoryPrefab, leftMenuRoot) as GameObject;
                category.name = $"Category({currentCategory.categoryName})";
                //category.transform.SetParent(leftMenuRoot);
                //category.transform.localPosition = Vector3.zero;

                pos = category.transform.localPosition;
                pos.y = heightOffset;
                category.transform.localPosition = pos;

                //category.GetComponentInChildren<Text>().text = currentCategory.categoryName;
                category.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = currentCategory.categoryName;

                heightOffset -= itemHeight;
                totalHeight += itemHeight;

                BlockCategory blockCategory = category.GetComponent<BlockCategory>();
                blockCategory.lists = new NodeListItemComponent[currentCategory.items.Count];
                if (hasDivider == true)
                {
                    blockCategory.categoryDivider = categoryDivider.transform;
                }

                for (int jx = 0; jx < currentCategory.items.Count; ++jx)
                {
                    NodeCategory.ListItem currentItem = currentCategory.items[jx];
                    
                    GameObject listItem = Instantiate(listItemPrefab, category.transform) as GameObject;
                    listItem.name = $"Node List Item({currentItem.nodeType.ToUpper()})";
                    //listItem.transform.SetParent(category.transform);
                    //listItem.transform.localPosition = Vector3.zero;

                    pos = listItem.transform.localPosition;
                    pos.y = -jx * itemHeight - itemHeight;
                    listItem.transform.localPosition = pos;

                    //Vector2 size = listItem.GetComponent<RectTransform>().sizeDelta;
                    //size.x = 0f;
                    //listItem.GetComponent<RectTransform>().sizeDelta = size;

                    Texture2D texture = Resources.Load(currentItem.imagePath) as Texture2D;
                    NodeListItemComponent node = listItem.GetComponent<NodeListItemComponent>();
                    node.SetImage(texture);
                    node.SetText(currentItem.listName);
                    //node.nodePrefab = AssetDatabase.LoadAssetAtPath(currentItem.prefabPath, typeof(GameObject)) as GameObject;
                    Enum.TryParse(currentItem.nodeType, out node.nodeType);

                    heightOffset -= itemHeight;
                    totalHeight += itemHeight;

                    blockCategory.lists[jx] = node;
                }

                //heightOffset -= offset;
                //totalHeight += offset;

                categoryManager.categories[ix] = blockCategory;
            }

            heightOffset = 0f;
            Vector2 contentSize = leftMenuRoot.GetComponent<RectTransform>().sizeDelta;
            //contentSize.y = totalHeight + offset;
            contentSize.y = totalHeight;
            leftMenuRoot.GetComponent<RectTransform>().sizeDelta = contentSize;
        }

        static NodeList GenerateNodeList()
        {
            NodeList list = new NodeList();

            NodeCategory operators = new NodeCategory();
            operators.categoryName = "Operator";
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Addition", nodeType = PROJECT.NodeType.ADD.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Substitude", nodeType = PROJECT.NodeType.SUB.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Multiply", nodeType = PROJECT.NodeType.MUL.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Divide", nodeType = PROJECT.NodeType.DIV.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "And", nodeType = PROJECT.NodeType.AND.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Or", nodeType = PROJECT.NodeType.OR.ToString()
            });
            operators.AddListItem(new NodeCategory.ListItem() {
                listName = "Not", nodeType = PROJECT.NodeType.NOT.ToString()
            });
            list.AddCategory(operators);

            NodeCategory logics = new NodeCategory();
            logics.categoryName = "Logic";
            logics.AddListItem(new NodeCategory.ListItem() {
                listName = "IF", nodeType = PROJECT.NodeType.IF.ToString()
            });
            logics.AddListItem(new NodeCategory.ListItem() {
                listName = "Switch", nodeType = PROJECT.NodeType.SWITCH.ToString()
            });
            logics.AddListItem(new NodeCategory.ListItem() {
                listName = "While", nodeType = PROJECT.NodeType.WHILE.ToString()
            });
            logics.AddListItem(new NodeCategory.ListItem() {
                listName = "Loop", nodeType = PROJECT.NodeType.LOOP.ToString()
            });
            list.AddCategory(logics);

            NodeCategory perceptions = new NodeCategory();
            perceptions.categoryName = "Perceptions";
            perceptions.AddListItem(new NodeCategory.ListItem() {
                listName = "Say", nodeType = PROJECT.NodeType.SAY.ToString()
            });
            perceptions.AddListItem(new NodeCategory.ListItem() {
                listName = "Expression", nodeType = PROJECT.NodeType.EXPRESSION.ToString()
            });
            list.AddCategory(perceptions);

            NodeCategory intelligentFunctions = new NodeCategory();
            intelligentFunctions.categoryName = "Intelligent Functions";
            intelligentFunctions.AddListItem(new NodeCategory.ListItem() {
                listName = "Gender", nodeType = PROJECT.NodeType.GENDER.ToString()
            });
            intelligentFunctions.AddListItem(new NodeCategory.ListItem() {
                listName = "Emotion", nodeType = PROJECT.NodeType.EMOTION.ToString()
            });
            intelligentFunctions.AddListItem(new NodeCategory.ListItem() {
                listName = "Hering", nodeType = PROJECT.NodeType.SPEECH_REC.ToString()
            });
            list.AddCategory(intelligentFunctions);

            return list;
        }

        [System.Serializable]
        public class NodeList
        {
            public List<NodeCategory> categories = new List<NodeCategory>();

            public void AddCategory(NodeCategory category)
            {
                categories.Add(category);
            }
        }

        [System.Serializable]
        public class NodeCategory
        {
            public string categoryName = string.Empty;
            public List<ListItem> items = new List<ListItem>();

            public void AddListItem(ListItem item)
            {
                items.Add(item);
            }

            [System.Serializable]
            public class ListItem
            {
                public string imagePath = string.Empty;
                public float[] color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
                public string listName = string.Empty;
                //public string prefabPath = string.Empty;
                public string nodeType = string.Empty;
            }
        }
    }
}