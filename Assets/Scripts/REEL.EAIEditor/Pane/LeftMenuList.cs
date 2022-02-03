using UnityEngine;

namespace REEL.D2EEditor
{
    public class LeftMenuList : MonoBehaviour
    {
        public Transform content;
        public GameObject listItem;

        private float itemHeight = 40f;
        private float itemSpace = 10f;

        public virtual void AddItem()
        {
            if (MCWorkspaceManager.IsProjectNull)
                return;

            Transform newItem = ((GameObject)Instantiate(listItem)).GetComponent<Transform>();
            newItem.SetParent(content);
            newItem.localScale = Vector3.one;
        }

        public void AdjustContentHeight()
        {
            RectTransform rt = content.GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.y = content.childCount * itemHeight + (content.childCount - 1) * itemSpace;
            rt.sizeDelta = size;
        }

        protected bool IsProjectNullOrOnSimulation
        {
            //get { return MCWorkspaceManager.IsProjectNull || MCWorkspaceManager.Instance.IsSimulation; }
            get { return MCPlayStateManager.IsProjectNull || MCPlayStateManager.Instance.IsSimulation; }
        }
    }
}