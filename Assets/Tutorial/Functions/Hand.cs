using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class Hand : FunctionBase
    {
        public GameObject       handObject;
        public GameObject       dragTarget;

        public TMPro.TMP_Text   text;
        public TMPro.TMP_Text   textShadow;

        public UIEffectValue    positionEffect;

        public float dragTime = 0;

        public AnimationCurve   dragCurveAni = new AnimationCurve();


        [Header("Target")]
        public RectTransform    targetObject;

        public string Text
        {
            get
            {
                return text.text;
            }

            set
            {
                text.text = ConvertText(value);
                textShadow.text = text.text;
            }
        }

        public GameObject TargetStart { get; internal set; }
        public GameObject TargetEnd { get; internal set; }


        #region FunctionBase Event

        //스퀀스 처음플레이때 호출 된다.
        public override void OnPlaySequence()
        {

        }

        //스퀀스 종료시 호출 된다.
        // completed = 1 이면 완료 0 이면 중도 취소
        public override void OnEndSequence(bool completed)
        {
            Hide();
            dragTarget = null;
            dragTime = 0.0f;
        }


        public override void OnClear(ObjectFlag clearFlg)
		{
            if(clearFlg.HasFlag(ObjectFlag.Hand) )
            {
                Hide();
                dragTarget = null;
                dragTime = 0.0f;
            }
		}

		#endregion //FunctionBase Event


		public void Hide()
        {
            handObject.SetActive(false);
            Text = "";
        }

        public void Show()
        {
            handObject.SetActive(true);
            Text = "";

            positionEffect.ResetValue();
            positionEffect.enabled = false;
        }

        public RectTransform Show(string targetNodeName)
        {
            var target = FindObject(targetNodeName);

            handObject.SetActive(true);
            Text = "";

            targetObject = target.GetComponent<RectTransform>();

            positionEffect.ResetValue();
            positionEffect.enabled = false;

            return targetObject;
        }

        public void Show(GameObject target)
        {
            handObject.SetActive(true);
            Text = "";

            targetObject = target.GetComponent<RectTransform>();

            positionEffect.ResetValue();
            positionEffect.enabled = false;
        }


        public void ShowClick(string targetNodeName)
        {
            var target = FindObject(targetNodeName);

            handObject.SetActive(true);
            targetObject = target.GetComponent<RectTransform>();

            Text = "Click";

            positionEffect.enabled = true;
        }


        public void DragTarget(GameObject targetObject)
        {
            dragTarget = targetObject;
            dragTime = 0.0f;

            positionEffect.ResetValue();
            positionEffect.enabled = false;
        }


        public void DragTarget(string targetObject)
        {
            dragTarget = FindObject(targetObject);

            if( dragTarget != null )
            {
                TutorialManager.NodeCreatePosition = dragTarget.transform.position;
            }

            dragTime = 0.0f;
        }


        //블록을 드래그 하여 생성 유도 처리
        // 드래그 시작 노드 이름 , 드래그 타겟 위치
        public void NodeCreation(string option)
        {
            string[] opt = option.Split( new char[] { ',' } , StringSplitOptions.RemoveEmptyEntries);
            string start = opt[0];
            string target = "EditorDropTarget";

            EnableBlockList(start);

            if( opt.Length == 2)
            {
                target = opt[1];
            }

            start = string.Format($"Node List Item({start})\\Highlight");

            RectTransform rectTransform = Show(start);

            if(rectTransform !=null)
            {
                float y = rectTransform.parent.localPosition.y + rectTransform.parent.parent.localPosition.y;
                TutorialManager.Instance.SetBlockScrollbar(y);
			}

            Text = "Drag and drop";
            DragTarget(target);

            Highlight.Instance.ShowBoxInner(start);
            Highlight.Instance.ShowBoxInner(dragTarget);
        }


        public void EnableBlockList(string nodeType)
        {
            foreach(var obj in FindObjectsOfType<REEL.D2EEditor.NodeListItemComponent>())
            {
                if( obj.nodeType.ToString() == nodeType)
                {
                    obj.enabled = true;
                }
                else
                {
                    obj.enabled = false;
                }
            }
        }

        public void NodeLink(string option)
        {
            DisableAllNode();

            string[] opt = option.Split(',');
            if(opt.Length != 2)
            {
                Debug.LogError("wrong parameter");
                return;
            }

            string node1 = opt[0];
            string node2 = opt[1];

            if(node1.Contains("\\") == false)
            {
                node1 += "\\EP Right";

                if( FindObject(node1) == false )
                {
                    node1 = opt[0];
				}
            }

            if(node2.Contains("\\") == false)
            {
                node2 += "\\EP Left";

                if(FindObject(node2) == false)
                {
                    node2 = opt[1];
                }
            }

            Show(node1);
            Text = "Drag and drop";
            DragTarget(node2);


			EnableNodeSocket(node1);
			EnableNodeSocket(node2);

			//EnableNodeSocketType(node1 , REEL.D2EEditor.MCNodeSocket.SocketType.EPRight);
			//EnableNodeSocketType(node2 , REEL.D2EEditor.MCNodeSocket.SocketType.EPLeft);

			Highlight.Instance.ShowBox(node1);
            Highlight.Instance.ShowBox(node2);

        }

        public void UnlinkClick(string targetNodeName)
		{
			DisableAllNode();

			ShowClick(targetNodeName);
            Text = "ALT + Click";
            Highlight.Instance.ShowBox(targetNodeName);


			EnableNodeUnlink(targetNodeName);
		}

		public void HighlightClick(string targetNodeName)
        {
            Text = "Click";
            ShowClick(targetNodeName);
            Highlight.Instance.ShowBox(targetNodeName);
        }


        public void ShowClickPlayButton()
        {
            Text = "Click";
            ShowClick("PlayButtonRect");
            dragTarget = null;
            Highlight.Instance.ShowBox("PlayButtonRect");
        }


        private void Update()
		{

            if(targetObject == null) return;

            try
            {
                //드래그 모드 일경우
                if( dragTarget != null )
                {
                    UpdateDrag();
				}
                else
                {
                    handObject.transform.position = targetObject.position;
                }
            }
            catch
            {
                targetObject = null;
                dragTarget = null;
            }

        }

		private void UpdateDrag()
		{
            dragTime += Time.smoothDeltaTime;

            float t = Mathf.Repeat(dragTime , dragCurveAni.keys[dragCurveAni.length-1].time );

            t = dragCurveAni.Evaluate(t);

            handObject.transform.position = Vector2.Lerp(targetObject.position, dragTarget.transform.position, t);
        }
	}
}