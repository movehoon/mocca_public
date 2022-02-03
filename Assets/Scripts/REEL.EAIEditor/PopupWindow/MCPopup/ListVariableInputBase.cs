using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class ListVariableInputBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public MCVariableListPopup popup;
        public List<T> items;
        public float itemHeight = 45f;

        [SerializeField]
        protected List<T> activeItems = new List<T>();
        protected RectTransform refRT;
        protected const int minCount = 1;
        protected const int maxCount = 15;
        protected int currentItemCount = 0;

        protected virtual void OnEnable()
        {
            if (refRT == null)
            {
                refRT = popup.GetComponent<RectTransform>();
            }

            if (currentItemCount == 0)
            {
                activeItems = new List<T>();
                activeItems.Add(items[0]);
                items[0].gameObject.SetActive(true);
                items.RemoveAt(0);

                ArrangeItemsYPosition();

                ++currentItemCount;
            }
        }

        protected virtual void OnDisable()
        {
            ResetValues();
        }

        public List<T> GetActiveItems()
        {
            return activeItems;
        }

        public virtual string GetValue()
        {
            return "";
        }

        public virtual void SetValue(string value)
        {

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

        public virtual void OnAddButtonClicked()
        {
            if (currentItemCount == maxCount)
                return;

            ++currentItemCount;

            activeItems.Add(items[items.Count - 1]);
            items.RemoveAt(items.Count - 1);

            activeItems[activeItems.Count - 1].gameObject.SetActive(true);

            Vector2 size = refRT.sizeDelta;
            size.y += itemHeight;
            refRT.sizeDelta = size;

            ArrangeItemsYPosition();
        }

        public virtual void OnMinusButtonClicked(T item)
        {
            if (currentItemCount == minCount)
                return;

            int index = 0;
            for (int ix = 0; ix < activeItems.Count; ++ix)
            {
                var expItem = activeItems[ix];
                if (expItem.GetInstanceID().Equals(item.GetInstanceID()))
                {
                    index = ix;
                    break;
                }
            }

            //Debug.Log(index);
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

        protected virtual void ArrangeSizeAndYPosition()
        {
            Vector2 size = refRT.sizeDelta;
            size.y += itemHeight;
            refRT.sizeDelta = size;

            ArrangeItemsYPosition();
        }
    }
}