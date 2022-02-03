using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class FunctionIOListBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public MCFunctionListPopup popup;
        public List<T> items = new List<T>();
        public float itemHeight = 45f;

        protected List<T> activeItems = new List<T>();
        protected RectTransform refRT;
        protected const int maxCount = 5;
        protected int currentItemCount = 0;

        public List<T> ActiveItems { get { return activeItems; } }

        protected virtual void OnEnable()
        {
            if (refRT == null)
            {
                refRT = popup.GetComponent<RectTransform>();
            }
        }

        protected virtual void OnDisable()
        {
            ResetValues();
        }

        public virtual void ResetValues()
        {
            for (int ix = activeItems.Count - 1; ix >= 0; --ix)
            {
                items.Add(activeItems[ix]);
                activeItems.RemoveAt(activeItems.Count - 1);
            }

            foreach (var item in items)
            {
                item.Invoke("ResetValue", 0f);
                item.gameObject.SetActive(false);
            }

            currentItemCount = 0;
        }

        public virtual void OnAddClicked()
        {
            if (currentItemCount == maxCount)
            {
                return;
            }

            ++currentItemCount;

            activeItems.Add(items[items.Count - 1]);
            items.RemoveAt(items.Count - 1);

            activeItems[activeItems.Count - 1].gameObject.SetActive(true);

            //Vector2 size = refRT.sizeDelta;
            //size.y += itemHeight;
            //refRT.sizeDelta = size;
            popup.UpdatePopupSize();

            ArrangeItemsYPosition();
        }

        public virtual void OnMinusClicked(T item)
        {
            if (currentItemCount == 0)
            {
                return;
            }

            int index = 0;
            for (int ix  =0; ix < activeItems.Count; ++ix)
            {
                var listItem = activeItems[ix];
                if (listItem.GetInstanceID().Equals(item.GetInstanceID()))
                {
                    index = ix;
                    break;
                }
            }

            --currentItemCount;

            items.Add(activeItems[index]);
            items[items.Count - 1].Invoke("ResetValue", 0f);
            activeItems[index].gameObject.SetActive(false);
            activeItems.RemoveAt(index);

            Vector2 size = refRT.sizeDelta;
            size.y -= itemHeight;
            refRT.sizeDelta = size;

            ArrangeItemsYPosition();
        }

        protected virtual void ArrangeItemsYPosition()
        {
            for (int ix = 0; ix < activeItems.Count; ++ix)
            {
                var item = activeItems[ix];
                Vector2 position = item.GetComponent<RectTransform>().anchoredPosition;
                position.y = -ix * itemHeight;
                item.GetComponent<RectTransform>().anchoredPosition = position;
            }
        }

        //public void AddFunctionDataState(FunctionDataState state)
        //{
        //    dataStates.Enqueue(state);
        //}

        public int GetItemIndex(T item)
        {
            if (currentItemCount == 0)
            {
                return -1;
            }

            for (int ix = 0; ix < activeItems.Count; ++ix)
            {
                var listItem = activeItems[ix];
                if (listItem.GetInstanceID().Equals(item.GetInstanceID()))
                {
                    return ix;
                }
            }

            return -1;
        }
    }
}