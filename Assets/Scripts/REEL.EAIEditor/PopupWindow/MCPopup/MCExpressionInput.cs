using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCExpressionInput : ListVariableInputBase<MCExpressionInputItem>
	{
        public override void SetValue(string value)
        {
            ResetValues();

            PROJECT.Expression[] expressions = JsonConvert.DeserializeObject<PROJECT.Expression[]>(value);
            for (int ix = 0; ix < expressions.Length; ++ix)
            {
                if (currentItemCount == maxCount)
                    break;

                MCExpressionInputItem newItem = AddOneItem();
                newItem.SetValue(expressions[ix]);

                if (ix != 0)
                {
                    ArrangeSizeAndYPosition();
                }
            }
        }

        private MCExpressionInputItem AddOneItem()
        {
            activeItems.Add(items[items.Count - 1]);
            items.RemoveAt(items.Count - 1);

            MCExpressionInputItem item = activeItems[activeItems.Count - 1];
            item.gameObject.SetActive(true);

            ++currentItemCount;

            return item;
        }

        public override string GetValue()
        {
            List<MCExpressionInputItem> activeItems = GetActiveItems();
            List<PROJECT.Expression> expressions = new List<PROJECT.Expression>();
            foreach (var item in activeItems)
            {
                PROJECT.Expression expression = new PROJECT.Expression();
                expression.tts = item.ttsInput.text;
                //expression.facial = Constants.ParseFacialExpressionKoreanToEnglish(item.faceDropdown.value);
                //expression.motion = Constants.ParseMotionExpressionKoreanToEnglish(item.motionDropdown.value);
                expression.facial = Constants.ParseFacialExpressionKoreanToEnglish(item.FaceDropdown);
                expression.motion = Constants.ParseMotionExpressionKoreanToEnglish(item.MotionDropdown);

                expressions.Add(expression);
            }

            return JsonConvert.SerializeObject(expressions);
        }
	}
}