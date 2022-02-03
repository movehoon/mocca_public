using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace REEL.Test
{
    public class NestedDropdown : MonoBehaviour
    {
        [Header("드롭다운 속성")]
        public Transform parentTransform1;
        public GameObject buttonPrefab1;
        public Transform parentTransform2;
        public GameObject buttonPrefab2;

        [Header("모션 데이터")]
        public TextAsset motionGroupText;
        public TextAsset motionListText;

        public List<MotionGroup> motionGroups = null;

        private Dictionary<string, List<string>> motionListDictionary = new Dictionary<string, List<string>>();

        private bool isOn = false;

        private void OnEnable()
        {
            if (motionGroups == null || motionGroups.Count == 0)
            {
                InitializeMotionGroupData(motionGroupText.text);
                InitializeMotionListDictionary(motionListText.text);
                InitializeDropdownButtons();
            }
        }

        public void OnDropdownButtonClicked()
        {
            isOn = !isOn;
            parentTransform1.gameObject.SetActive(isOn);
        }

        void InitializeMotionGroupData(string motionGroupText)
        {
            string[] lines = motionGroupText.Split(new string[] { "\n" }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 0)
            {
                motionGroups = new List<MotionGroup>();
                foreach (string line in lines)
                {
                    motionGroups.Add(new MotionGroup(line));
                }
            }
        }

        void InitializeMotionListDictionary(string motionListText)
        {
            string[] lines = motionListText.Split(new string[] { "\n" }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 0)
            {
                foreach (string line in lines)
                {
                    string[] motionGroupPair = line.Split(new string[] { "," }, 
                        StringSplitOptions.RemoveEmptyEntries);

                    if (motionGroupPair.Length > 0)
                    {
                        string group = motionGroupPair[0].Trim();
                        string motion = motionGroupPair[1].Trim();

                        if (motionListDictionary.TryGetValue(group, out List<string> motions) == true)
                        {
                            if (motions != null && motions.Count > 0)
                            {
                                motions.Add(motion);
                            }
                            else if (motions == null)
                            {
                                motions = new List<string>();
                                motions.Add(motion);
                            }
                        }
                        else
                        {
                            motionListDictionary.Add(group, new List<string>() { motion });
                        }
                    }
                }
            }
        }

        void InitializeDropdownButtons()
        {
            for (int ix = 0; ix < motionGroups.Count; ++ix)
            {
                GameObject newButton = Instantiate(buttonPrefab1, parentTransform1);
                TextMeshProUGUI text = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    //text.text = motionGroups[ix].groupEng + "(" + motionGroups[ix].groupKor + ")";
                    text.text = motionGroups[ix].groupEng;
                }

                Vector3 pos = newButton.transform.localPosition;
                pos.y = -50f * ix;
                newButton.transform.localPosition = pos;

                newButton.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        if (motionListDictionary.TryGetValue(text.text, out List<string> motions) == true)
                        {
                            //parentTransform1.gameObject.SetActive(false);
                            isOn = false;

                            if (motions.Count > 0)
                            {
                                DestroyParent2ChildAll();

                                for (int jx = 0; jx < motions.Count + 1; ++jx)
                                {
                                    string motion = jx == 0 ? "Auto" : motions[jx - 1];
                                    //Utils.LogRed($"group: {text.text} / motion: {motion}");

                                    GameObject newButton2 = Instantiate(buttonPrefab2, parentTransform2);
                                    newButton2.GetComponentInChildren<TextMeshProUGUI>().text = motion;

                                    Vector3 pos2 = newButton2.transform.localPosition;
                                    pos2.y = -50f * jx;
                                    newButton2.transform.localPosition = pos2;

                                    Button[] buttons = newButton2.GetComponentsInChildren<Button>();
                                    if (buttons.Length == 2)
                                    {
                                        buttons[0].onClick.AddListener(() =>
                                        {
                                            Utils.LogRed($"{buttons[0].GetComponentInChildren<TextMeshProUGUI>().text} selected.");
                                            //parentTransform2.gameObject.SetActive(false);
                                        });

                                        buttons[1].onClick.AddListener(() =>
                                        {
                                            DestroyParent2ChildAll();

                                            //parentTransform2.gameObject.SetActive(false);
                                            //parentTransform1.gameObject.SetActive(true);
                                            isOn = true;
                                        });
                                    }
                                }

                                parentTransform2.gameObject.SetActive(true);
                            }
                        }
                    }
                );
            }
        }

        void DestroyParent2ChildAll()
        {
            int childCount = parentTransform2.childCount;
            for (int ix = childCount - 1; ix >= 0; --ix)
            {
                Destroy(parentTransform2.GetChild(ix).gameObject);
            }
        }
    }

    [Serializable]
    public class MotionGroupData
    {
        public MotionGroup group;
        public List<string> motions;

        public MotionGroupData()
        {
            motions = new List<string>();
        }

        public MotionGroupData(MotionGroup group)
        {
            this.group = new MotionGroup(group);
            motions = new List<string>();
        }

        public void AddMotion(string motion)
        {
            if (motions.Count == 0)
            {
                motions.Add(motion);
                return;
            }

            if (motions.Contains(motion) == true)
            {
                return;
            }

            motions.Add(motion);
        }
    }

    [Serializable]
    public class MotionGroup
    {
        public string groupEng;
        public string groupKor;

        public MotionGroup()
        {
        }

        public MotionGroup(string groupLine)
        {
            string[] motions = groupLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (motions.Length == 2)
            {
                groupEng = motions[0].Trim();
                groupKor = motions[1].Trim();
            }
        }

        public MotionGroup(MotionGroup group)
        {
            groupEng = group.groupEng;
            groupKor = group.groupKor;
        }
    }    
}