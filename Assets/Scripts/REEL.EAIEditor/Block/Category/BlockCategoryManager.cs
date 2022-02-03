using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class BlockCategoryManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField searchInputField;
        public BlockCategory[] categories;

        private float itemHeight = 36f;

        private RectTransform refRect;
        public RectTransform RefRect
        {
            get
            {
                if (refRect == null)
                {
                    refRect = GetComponent<RectTransform>();
                }

                return refRect;
            }
        }

        public string SearchWordLowcase
        {
            get
            {
                if (Utils.IsNullOrEmptyOrWhiteSpace(searchInputField.text) == true)
                {
                    return string.Empty;
                }

                return searchInputField.text.ToLower();
            }
        }

        private float startYSize = 0f;

        private void OnEnable()
        {
            if (categories == null || categories.Length == 0)
            {
                categories = GetComponentsInChildren<BlockCategory>();
            }

            Utils.GetGraphPane().SubscribeOnPointDown(SetAllItemUnselected);

            startYSize = RefRect.sizeDelta.y;
        }

        private void OnDisable()
        {
            Utils.GetGraphPane().UnSubscribeOnPointDown(SetAllItemUnselected);
        }

        public void UpdateState(BlockCategory category)
        {
            int index = 0;
            float height = 0f;
            float sign = 1f;
            for (int ix = 0; ix < categories.Length; ++ix)
            {
                if (categories[ix].gameObject.GetInstanceID().Equals(category.gameObject.GetInstanceID()))
                {
                    index = ix + 1;
                    if (Utils.IsNullOrEmptyOrWhiteSpace(SearchWordLowcase) == true)
                    {
                        height = category.GetListCount * itemHeight;
                    }
                    else
                    {
                        height = category.GetFilteredListCount(SearchWordLowcase) * itemHeight;
                        //Debug.Log($"[BlockCategoryManager.UpdateState] height: {height} / FilteredCount: {category.GetFilteredListCount(SearchWordLowcase)}");
                    }
                    
                    sign = category.IsCollapsed ? 1f : -1f;

                    break;
                }
            }

            for (; index < categories.Length; ++index)
            {
                //RectTransform rect = categories[index].categoryDivider.GetComponent<RectTransform>();
                //rect.anchoredPosition += new Vector2(0f, height * sign);
                categories[index].categoryDivider.localPosition += new Vector3(0f, height * sign, 0f);
                categories[index].RefRect.anchoredPosition += new Vector2(0f, height * sign);
            }

            // Collapse여부에 따른 Content의 y크기 조절.
            Vector2 size = RefRect.sizeDelta;
            float yOffset = 0f;
            if (Utils.IsNullOrEmptyOrWhiteSpace(SearchWordLowcase) == true)
            {
                yOffset = category.GetListCount * itemHeight;
            }
            else
            {
                yOffset = category.GetFilteredListCount(SearchWordLowcase) * itemHeight;
            }
            size.y = category.IsCollapsed ? size.y - yOffset : size.y + yOffset;
            RefRect.sizeDelta = size;
        }

        public void SetAllItemUnselected()
        {
            foreach (BlockCategory category in categories)
            {
                category.SetAllItemUnselected();
            }
        }

        public void OnItemSelected(int itemInstanceID)
        {
            foreach (BlockCategory category in categories)
            {
                category.OnItemSelected(itemInstanceID);
            }
        }

        public void Clear()
        {
            searchInputField.SetTextWithoutNotify("");
            FilterListWithWord("");
        }

        bool prevWordEmptyState = false;
        public void FilterListWithWord(string searchWord)
        {
            bool currentWordEmptyState = Utils.IsNullOrEmptyOrWhiteSpace(searchWord);
            if (prevWordEmptyState == true && currentWordEmptyState == true)
            {
                return;
            }

            // Show all.
            //if (Utils.IsNullOrEmptyOrWhiteSpace(searchWord) == true)
            if (currentWordEmptyState == true)
            {
                foreach (BlockCategory category in categories)
                {
                    // reset to original position (category);
                    category.ResetPosition();

                    foreach (NodeListItemComponent item in category.lists)
                    {
                        item.gameObject.SetActive(true);

                        // reset to original position (item);
                        item.ResetPosition();
                    }

                    // Set category's collapse state to false.
                    category.IsCollapsed = false;
                    category.collapseButton.image.sprite = category.unCollapsedSprite;
                }

                // reset to original size (content).
                ResetSize();
                prevWordEmptyState = true;
                return;
            }

            // Filter list with search word (lowercase).
            string searchWordLowercase = searchWord.ToLower();
            float yPosOffset = 0f;
            int totalActiveChildCount = 0;
            for (int ix = 0; ix < categories.Length; ++ix)
            {
                BlockCategory category = categories[ix];

                int activeChildCount = 0;
                List<int> activeChildIndices = new List<int>();
                for (int jx = 0; jx < category.lists.Length; ++jx)
                {
                    NodeListItemComponent item = category.lists[jx];
                    //if (item.ItemNameLowercase.Contains(searchWordLowercase) == true)
                    if (item.EngLowerItemName.Contains(searchWordLowercase) == true
                        || item.KorLowerItemName.Contains(searchWordLowercase) == true)
                    {
                        item.gameObject.SetActive(true);
                        ++activeChildCount;
                        activeChildIndices.Add(jx);
                    }

                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }

                totalActiveChildCount += activeChildCount;

                // Resize blockcategory y size.
                if (ix != 0)
                {
                    if (ix > 0 && ix < categories.Length - 1)
                    {
                        Vector3 pos = category.categoryDivider.localPosition;
                        pos.y = yPosOffset;
                        category.categoryDivider.localPosition = pos;
                    }

                    category.SetPosition(yPosOffset - 25f);
                }

                // Rearrange item's y position.
                for (int index = 0; index < activeChildIndices.Count; ++index)
                {
                    int childIndex = activeChildIndices[index];
                    NodeListItemComponent item = category.lists[childIndex];
                    float height = itemHeight * index;
                    item.SetPosition(-height);
                }

                yPosOffset = category.RefRect.anchoredPosition.y - itemHeight - activeChildCount * itemHeight;

                // Set category's collapse state to false.
                category.IsCollapsed = false;
                category.collapseButton.image.sprite = category.unCollapsedSprite;
            }

            float ySize = (categories.Length + totalActiveChildCount) * itemHeight + ((categories.Length - 1) * 25f);
            SetSize(ySize);

            prevWordEmptyState = false;
        }

        public void ResetSize()
        {
            SetSize(startYSize);
        }

        public void SetSize(float ySize)
        {
            Vector2 size = RefRect.sizeDelta;
            size.y = ySize;
            RefRect.sizeDelta = size;
        }
    }
}