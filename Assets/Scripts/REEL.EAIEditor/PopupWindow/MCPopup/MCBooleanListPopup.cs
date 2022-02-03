using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCBooleanListPopup : ListVariableInputBase<MCBooleanListInputItem>
    {
        public override void SetValue(string value)
        {
            ResetValues();

            PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(value);
            for (int ix = 0; ix < listValue.listValue.Length; ++ix)
            {
                if (currentItemCount == maxCount)
                {
                    return;
                }

                MCBooleanListInputItem newItem = AddOneItem();
                newItem.SetValue(listValue.listValue[ix]);

                if (ix != 0)
                {
                    ArrangeSizeAndYPosition();
                }
            }
        }

        private MCBooleanListInputItem AddOneItem()
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
            List<MCBooleanListInputItem> activeItems = GetActiveItems();
            List<string> booleans = new List<string>();
            foreach (var item in activeItems)
            {
                booleans.Add(GetStringValueFromBoolean(item.booleanToggle.isOn));
            }

            return JsonConvert.SerializeObject(new PROJECT.ListValue() { listValue = booleans.ToArray() });
        }

        private string GetStringValueFromBoolean(bool isOn)
        {
            if (isOn == true)
            {
                //return "1";
                return "true";
            }
            else
            {
                //return "0";
                return "false";
            }
        }
    }
}