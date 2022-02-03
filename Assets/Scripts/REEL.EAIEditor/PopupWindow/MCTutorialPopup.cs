using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class TutorialInfo
    {
        public string title;
        public string content;
    }

    public class MCTutorialPopup : MonoBehaviour
    {
        //[Header("ShortCut Informations")]
        //public TutorialInfo[] tutorialInfos;

        [Header("References for instantiation")]
        public McTutorialListItem itemPrefab;
        public RectTransform tutorialInfoParent;

        [Header("References for Reset")]
        public ScrollRect scrollRect;

        [Header("References for Tutorial Start")]
        public Button blockTabButton;

        public GameObject windowRoot;

        private float itemHeight = 40f;
        private bool hasInit = false;

        private void OnEnable()
        {
            if(TutorialManager.Instance == null) return;

            if(hasInit == false)
            {
                InitializeShortCuts();
                hasInit = true;
            }

            Invoke("ResetScroll", 0.1f);
        }




        private void InitializeShortCuts()
		{
			int count = AddUserTutorial();      //일반 사용자 튜토리얼 추가
            count += AddVideoTutorial();        //비디오 플레이 튜토리얼 추가

            // Set content recttransform's height.
            float height = itemHeight * count + (10f * (count - 1));
			tutorialInfoParent.sizeDelta = new Vector2(tutorialInfoParent.sizeDelta.x, height);
		}

        //일반 사용자 튜토리얼
        private int AddUserTutorial()
        {
            var sequences = TutorialManager.Instance.GetSequences();
            int count = 0;

            foreach(var tutorial in sequences)
            {
                if(tutorial.hide) continue;

                var item = Instantiate(itemPrefab, tutorialInfoParent);

                item.titleText.text = string.Format("{0}", tutorial.title);
                item.contentText.text = string.Format("{0}", tutorial.content);

                if(tutorial.active == true)
                {
                    item.button.onClick.AddListener(() =>
                    {
                        PlayTutorial(tutorial.numbering);
                    });
                }
                else
                {
                    item.titleText.color = Color.gray;
                    item.contentText.color = Color.gray;

                    item.button.interactable = false;
                }

                count++;
            }

            return count;
        }


        //비디오 플레이 튜토리얼
        private int AddVideoTutorial()
		{
            var sequences =  FindObjectsOfType<VideoSequence>();
            if(sequences == null || sequences.Count() == 0) return 0;

            foreach(var tutorial in sequences.OrderBy(x=>x.numbering) )
            {
                if(tutorial.hide) continue;

                var item = Instantiate(itemPrefab, tutorialInfoParent);

                item.titleText.text = string.Format("{0}", tutorial.title);
                item.contentText.text = string.Format("{0}", tutorial.content);

                if(tutorial.active == true)
                {
                    item.button.onClick.AddListener(() =>
                    {
                        PlayVideoTutorial(tutorial.numbering);
                        CloseButtonClicked();
                    });
                }
                else
                {
                    item.titleText.color = Color.gray;
                    item.contentText.color = Color.gray;

                    item.button.interactable = false;
                }
            }

            return sequences.Count();
        }

		bool IsDirty()
        {
            var mgr = FindObjectOfType<REEL.D2EEditor.TabManager>();

            if(mgr == null) return false;

            return mgr.CurrentTabCount > 0;
        }


        void CloseCurrentProject()
        {
            if(REEL.D2EEditor.MCWorkspaceManager.Instance != null)
            {
                REEL.D2EEditor.MCWorkspaceManager.Instance.CloseCurrentProject();
            }
        }

        private void PlayTutorial(int numbering)
		{
            if(IsDirty() == true)
            {
                MessageBox.ShowYesNo("[ID_MSG_CLOSE_CONTINUE]현재 프로젝트가 닫히고 저장되지 않은 정보는 사라집니다.\n계속 진행합니까?"    // local 추가완료
                    , res =>
                    {
                        if(res == true)
                        {
                            // 작성자: 장세윤.
                            // 항상 블록탭 열고 시작하도록.
                            blockTabButton.onClick.Invoke();

                            windowRoot.SetActive(false);
                            CloseCurrentProject();

                            TutorialManager.Instance.PlaySequence(numbering);
                        }
                    });
            }
            else
            {
                // 작성자: 장세윤.
                // 항상 블록탭 열고 시작하도록.
                blockTabButton.onClick.Invoke();

                windowRoot.SetActive(false);
                CloseCurrentProject();

                TutorialManager.Instance.PlaySequence(numbering);
            }
        }


        private void PlayVideoTutorial(int numbering)
        {
			TutorialManager.Instance.PlayVideo(numbering);
		}

        public void CloseButtonClicked()
        {
            ResetScroll();
            windowRoot.SetActive(false);
        }

        private void ResetScroll()
        {
            scrollRect.verticalScrollbar.value = 1.0f;
        }
    }
}