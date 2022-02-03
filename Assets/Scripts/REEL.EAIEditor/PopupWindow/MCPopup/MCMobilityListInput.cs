using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCMobilityListInput : ListVariableInputBase<MCMobilityListInputItem>
	{
        public override void SetValue(string value)
        {
            ResetValues();

            PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(value);
            for (int ix = 0; ix < listValue.listValue.Length; ++ix)
            {
                if (currentItemCount == maxCount)
                    break;

                MCMobilityListInputItem newItem = AddOneItem();
                newItem.SetValue(listValue.listValue[ix]);

                if (ix != 0)
                {
                    ArrangeSizeAndYPosition();
                }
            }

            //List<string> motions = JsonConvert.DeserializeObject<List<string>>(value);
            //for (int ix = 0; ix < motions.Count; ++ix)
            //{
            //    if (currentItemCount == maxCount)
            //        break;

            //    MCMobilityListInputItem newItem = AddOneItem();
            //    newItem.SetValue(motions[ix]);

            //    if (ix != 0)
            //    {
            //        ArrangeSizeAndYPosition();
            //    }
            //}
        }

        private MCMobilityListInputItem AddOneItem()
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
            List<MCMobilityListInputItem> activeItems = GetActiveItems();
            List<string> mobility = new List<string>();
            foreach (var item in activeItems)
            {
                //mobility.Add(Constants.ParseMobilityKoreanToEnglish(item.mobilityDropdown.value));
                mobility.Add(Constants.ParseMobilityKoreanToEnglish(item.MobilityDropdown));
            }

            return JsonConvert.SerializeObject(new PROJECT.ListValue() { listValue = mobility.ToArray() });
        }
    }
}