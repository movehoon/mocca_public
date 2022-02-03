using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class BlockCategory : MonoBehaviour
    {
        public NodeListItemComponent[] lists;
        public Transform categoryDivider;
        public Button collapseButton;

        public Sprite collapedSprite;
        public Sprite unCollapsedSprite;

        private BlockCategoryManager manager;
        public bool IsCollapsed { get; set; }

        private RectTransform refRect;
        public RectTransform RefRect
        {
            get
            {
                if(refRect == null)
                {
                    refRect = GetComponent<RectTransform>();
                }

                return refRect;
            }
        }

        private float startYPosition = 0f;
        private float dividerStartYPosition = 0f;


        private bool isLock 
        {   
            get
            {
                return TutorialManager.IsPlaying;
            }
        }


        private void Awake()
        {
            if (manager == null)
            {
                manager = transform.parent.GetComponent<BlockCategoryManager>();
            }

            if (lists == null || lists.Length == 0)
            {
                lists = GetComponentsInChildren<NodeListItemComponent>();
            }

            if (collapseButton == null)
            {
                collapseButton = GetComponentInChildren<Button>();
            }

            startYPosition = RefRect.anchoredPosition.y;
            if (categoryDivider != null)
            {
                dividerStartYPosition = categoryDivider.localPosition.y;
            }

            IsCollapsed = false;

            collapseButton.onClick.AddListener(OnClicked);
            //GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        public void OnClicked()
        {
			if(isLock) return;

			IsCollapsed = !IsCollapsed;
            UpdateState();
            SetAllItemUnselected();
        }


        //튜토리얼 시작시 전부 열어 놓을때 호출됨 - kjh
        public void Open()  
        {
            if(IsCollapsed == false) return;
            
            IsCollapsed = false;
            UpdateState();
            SetAllItemUnselected();
        }


        public void SetAllItemUnselected()
        {
            foreach (NodeListItemComponent item in lists)
            {
                item.SetItemSelected(false);
            }
        }

        public void OnItemSelected(int itemInstanceID)
        {
            foreach (NodeListItemComponent item in lists)
            {
                item.SetItemSelected(item.GetInstanceID().Equals(itemInstanceID));
            }
        }

        private void UpdateState()
        {
            foreach (NodeListItemComponent list in lists)
            {
                if (IsCollapsed)
                {
                    list.gameObject.SetActive(false);
                    collapseButton.image.sprite = collapedSprite;
                }
                    
                else
                {
                    if (Utils.IsNullOrEmptyOrWhiteSpace(manager.SearchWordLowcase) == false)
                    {
                        //if (list.ItemNameLowercase.Contains(manager.SearchWordLowcase))
                        if (list.EngLowerItemName.Contains(manager.SearchWordLowcase) == true
                            || list.KorLowerItemName.Contains(manager.SearchWordLowcase) == true)
                        {
                            list.gameObject.SetActive(true);
                        }

                        collapseButton.image.sprite = unCollapsedSprite;
                        continue;
                    }

                    list.gameObject.SetActive(true);
                    collapseButton.image.sprite = unCollapsedSprite;
                }
            }

            manager.UpdateState(this);
        }

        public void ResetPosition()
        {
            SetPosition(startYPosition);
            if (categoryDivider != null)
            {
                Vector3 pos = categoryDivider.localPosition;
                pos.y = dividerStartYPosition;
                categoryDivider.localPosition = pos;
            }
        }

        public void SetPosition(float yPosition)
        {
            Vector2 position = RefRect.anchoredPosition;
            position.y = yPosition;
            RefRect.anchoredPosition = position;
        }

        public int GetListCount { get { return lists.Length; } }

        public int GetFilteredListCount(string searchWordLowercase)
        {
            int count = 0;
            foreach (NodeListItemComponent item in lists)
            {
                //if (item.ItemNameLowercase.Contains(searchWordLowercase) == true)
                if (item.EngLowerItemName.Contains(searchWordLowercase) == true
                    || item.KorLowerItemName.Contains(searchWordLowercase) == true)
                {
                    ++count;
                    continue;
                }
            }

            return count;
        }
    }
}