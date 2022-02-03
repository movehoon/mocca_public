using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class Highlight : FunctionBase
    {
        public static Highlight Instance;

        public GameObject   prefabBox;
        public GameObject   prefabBoxInner;
        //public GameObject   prefabCircle;

        public Vector3      vec3 { get; set; }
        public Color        color = new Color();

        public List<GameObject> list = new List<GameObject>();


        #region FunctionBase Event
        //스퀀스 처음플레이때 호출 된다.
        public override void OnPlaySequence()
        {

        }

        //스퀀스 종료시 호출 된다.
        // completed = 1 이면 완료 0 이면 중도 취소
        public override void OnEndSequence(bool completed)
        {
            ClearObject();
        }

        public override void OnClear(ObjectFlag clearFlg)
        {
            if(clearFlg.HasFlag(ObjectFlag.Highlight) )
            {
                ClearObject();
            }
        }

        #endregion //FunctionBase Event

        public void Start()
		{
            Instance = this;
        }

        private void ClearObject()
        {
            foreach(var obj in list)
            {
                GameObject.DestroyImmediate(obj);
            }

            list.Clear();
        }


        public void ShowBox(string targetObjectName)
        {
            GameObject obj = FindObject(targetObjectName);

            if(obj == null)
            {
                isWaiting = false;
                return;
            }

            var rect = obj.GetComponent<RectTransform>();
            if(rect == null) return;

            TMPro.TMP_InputField input = rect.GetComponent<TMPro.TMP_InputField>();
            if( input != null )
            {
                input.enabled = true;
			}

            TMPro.TMP_Dropdown  dropDown = rect.GetComponent<TMPro.TMP_Dropdown>();
            if(dropDown != null)
            {
                dropDown.enabled = true;
            }


            GameObject box = GameObject.Instantiate(prefabBox , rect );

            list.Add(box);
        }

        public void ShowBoxInner(string targetObjectName)
        {
            GameObject obj = FindObject(targetObjectName);

            if(obj == null)
            {
                isWaiting = false;
                return;
            }

            var rect = obj.GetComponent<RectTransform>();
            if(rect == null) return;

            GameObject box = GameObject.Instantiate(prefabBoxInner , rect );

            list.Add(box);
        }


        public void ShowBox(GameObject targetObject)
        {
            var rect = targetObject.GetComponent<RectTransform>();
            if(rect == null) return;

            GameObject box = GameObject.Instantiate(prefabBox, rect );

            list.Add(box);
        }


        public void ShowBoxInner(GameObject targetObject)
        {
            GameObject obj = targetObject;

            if(obj == null)
            {
                isWaiting = false;
                return;
            }

            var rect = obj.GetComponent<RectTransform>();
            if(rect == null) return;

            GameObject box = GameObject.Instantiate(prefabBoxInner , rect );

            list.Add(box);
        }

    }
}