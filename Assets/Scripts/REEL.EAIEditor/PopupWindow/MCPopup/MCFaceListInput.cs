using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCFaceListInput : ListVariableInputBase<MCFaceListInputItem>
	{
        public override void SetValue(string value)
        {
            ResetValues();

            PROJECT.ListValue listValue = JsonConvert.DeserializeObject<PROJECT.ListValue>(value);
            for (int ix = 0; ix < listValue.listValue.Length; ++ix)
            {
                if (currentItemCount == maxCount)
                    break;

                MCFaceListInputItem newItem = AddOneItem();
                newItem.SetValue(listValue.listValue[ix]);

                if (ix != 0)
                {
                    ArrangeSizeAndYPosition();
                }
            }

            //List<string> faces = JsonConvert.DeserializeObject<List<string>>(value);
            //for (int ix = 0; ix < faces.Count; ++ix)
            //{
            //    if (currentItemCount == maxCount)
            //        break;

            //    MCFaceListInputItem newItem = AddOneItem();
            //    newItem.SetValue(faces[ix]);

            //    if (ix != 0)
            //    {
            //        ArrangeSizeAndYPosition();
            //    }
            //}
        }

        private MCFaceListInputItem AddOneItem()
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
            List<MCFaceListInputItem> activeItems = GetActiveItems();
            List<string> faces = new List<string>();
            foreach (var item in activeItems)
            {
                faces.Add(Constants.ParseFacialKoreanToEnglish(item.FaceDropdown));
            }
            
            return JsonConvert.SerializeObject(new PROJECT.ListValue() { listValue = faces.ToArray() });
        }
    }
}