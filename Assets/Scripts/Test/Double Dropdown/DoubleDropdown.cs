using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Test
{
    public class DoubleDropdown : MonoBehaviour
    {
        public enum Direction
        {
            None, Left, Right
        }

        [Header("목록 정보")]
        public TextAsset column1Text;
        public TextAsset column2Text;

        [Header("드롭다운 콘텐츠 관련")]
        public GameObject content;
        public GameObject column1ItemPrefab;
        public GameObject column2ItemPrefab;
        public Transform column1Parent;
        public Transform column2Parent;
        public bool isOn = false;

        [Header("이동 관련")]
        public Vector3 startPosition;
        public Vector3 leftMoveTarget;
        public Vector3 rightMoveTarget;
        public float moveSpeed = 10f;
        public Direction moveDirection = Direction.None;

        private List<string> column1List = new List<string>();
        private Dictionary<string, List<string>> column2List = new Dictionary<string, List<string>>();

        private void Awake()
        {
            InitializeColumn1ListData(column1Text.text);
            InitializeColumn2ListData(column2Text.text);

            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    isOn = content.activeSelf;
                    if (isOn == false)
                    {
                        Activate();
                    }
                    else
                    {
                        Deactivate();
                    }
                });
            }
        }

        private void OnEnable()
        {
            isOn = content.activeSelf;
        }

        private void Update()
        {
            switch (moveDirection)
            {
                case Direction.Left:
                    MoveTo(leftMoveTarget);
                    break;
                case Direction.Right:
                    MoveTo(rightMoveTarget);
                    break;
                default: break;
            }
        }

        void MoveTo(Vector3 target)
        {
            content.transform.localPosition = Vector3.Lerp(content.transform.localPosition, target, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(content.transform.localPosition, target) == 0f)
            {
                moveDirection = Direction.None;
                return;
            }
        }

        void Activate()
        {
            content.SetActive(true);
        }

        void Deactivate()
        {
            content.SetActive(false);
            Reset();
        }

        void InitializeColumn1ListData(string column1Text)
        {
            column1List = column1Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            InitializeColumn1Dropdown();
        }

        void InitializeColumn2ListData(string column2Text)
        {
            string[] lines = column2Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                return;
            }

            foreach (string line in lines)
            {
                string[] column1Column2Pair = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (column1Column2Pair.Length == 0)
                {
                    continue;
                }

                string column1 = column1Column2Pair[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                //Debug.Log($"column1: {column1}");
                string column2 = column1Column2Pair[1].Trim();

                if (column2List.TryGetValue(column1, out List<string> list) == true)
                {
                    if (list != null && list.Count > 0)
                    {
                        list.Add(column2);
                    }
                    else if (list == null)
                    {
                        list = new List<string>();
                        list.Add(column2);
                    }
                }
                else
                {
                    column2List.Add(column1, new List<string>() { column2 });
                }
            }
        }

        void InitializeColumn1Dropdown()
        {
            for (int ix = 0; ix < column1List.Count; ++ix)
            {
                GameObject go = Instantiate(column1ItemPrefab, column1Parent);
                TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                if (text == null)
                {
                    continue;
                }

                // index0: eng / index1: kor.
                text.text = column1List[ix].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];

                Vector3 pos = go.transform.localPosition;
                pos.y = -50f * ix;
                go.transform.localPosition = pos;

                Button button = go.GetComponent<Button>();
                if (button == null)
                {
                    continue;
                }

                button.onClick.AddListener(() =>
                {
                    if (column2List.TryGetValue(text.text, out List<string> list) == true)
                    {
                        InitializeColumn2Dropdown(text.text, list);
                    }
                });
            }
        }

        void InitializeColumn2Dropdown(string key, List<string> list)
        {
            if (list.Count > 0)
            {
                moveDirection = Direction.Left;

                int childCount = column2Parent.childCount;
                for (int jx = childCount - 1; jx >= 0; --jx)
                {
                    Destroy(column2Parent.GetChild(jx).gameObject);
                }

                RectTransform rect = column2Parent.GetComponent<RectTransform>();
                Vector2 sizeDelta = rect.sizeDelta;
                sizeDelta.y = (list.Count + 1) * 50f;
                rect.sizeDelta = sizeDelta;

                for (int jx = 0; jx < list.Count + 1; ++jx)
                {
                    string column2 = jx == 0 ? "Auto" : list[jx - 1];

                    GameObject go2 = Instantiate(column2ItemPrefab, column2Parent);
                    TextMeshProUGUI text2 = go2.GetComponentInChildren<TextMeshProUGUI>();
                    if (text2 == null)
                    {
                        continue;
                    }

                    text2.text = column2;

                    Vector2 pos2 = go2.transform.localPosition;
                    pos2.y = -50f * jx;
                    go2.transform.localPosition = pos2;

                    Button[] buttons = go2.GetComponentsInChildren<Button>();
                    if (buttons.Length == 2)
                    {
                        buttons[0].onClick.AddListener(() =>
                        {
                            Utils.LogRed($"{key}/{buttons[0].GetComponentInChildren<TextMeshProUGUI>().text} selected.");
                            Deactivate();
                        });

                        buttons[1].onClick.AddListener(() =>
                        {
                            moveDirection = Direction.Right;
                        });
                    }
                }
            }
        }

        private void Reset()
        {
            content.transform.localPosition = startPosition;
            moveDirection = Direction.None;
        }
    }
}