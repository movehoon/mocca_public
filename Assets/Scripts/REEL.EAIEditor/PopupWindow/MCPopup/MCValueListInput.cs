using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCValueListInput : ListVariableInputBase<MCValueListInputItem>
	{
        public void SetDataType(PROJECT.DataType dataType)
        {
            foreach (var activeItem in activeItems)
            {
                activeItem.SetDataType(dataType);
            }
        }

        public override void SetValue(string value)
        {
            ResetValues();

            PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(value);
            //Debug.Log(listValue.listValue.Length);
            for (int ix = 0; ix < listValue.listValue.Length; ++ix)
            {
                if (currentItemCount == maxCount)
                    break;

                MCValueListInputItem newItem = AddOneItem();
                newItem.SetValue(listValue.listValue[ix]);

                if (ix != 0)
                {
                    ArrangeSizeAndYPosition();
                }
            }
        }

        public override void OnAddButtonClicked()
        {
            base.OnAddButtonClicked();
            activeItems[activeItems.Count - 1].SetDataType(popup.currentVariableType);
        }

        private MCValueListInputItem AddOneItem()
        {
            activeItems.Add(items[items.Count - 1]);
            items.RemoveAt(items.Count - 1);

            var item = activeItems[activeItems.Count - 1];
            item.gameObject.SetActive(true);

            ++currentItemCount;

            return item;
        }

        public override string GetValue()
        {
            List<MCValueListInputItem> activeItems = GetActiveItems();
            List<string> values = new List<string>();
            foreach (var item in activeItems)
            {
                values.Add(item.Input);
            }

            //Debug.Log("ListInput.GetValue() : " + JsonConvert.SerializeObject(values));
            return JsonConvert.SerializeObject(new PROJECT.ListValue() { listValue = values.ToArray() });
        }
    }
}