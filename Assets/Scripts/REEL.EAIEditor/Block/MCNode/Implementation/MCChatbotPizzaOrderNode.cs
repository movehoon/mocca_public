using REEL.PROJECT;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCChatbotPizzaOrderNode : MCNode
    {
        public TMP_InputField input;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = new Body();

            // 기본 값 설정.
            //if (Utils.IsNullOrEmptyOrWhiteSpace(nodeData.body.value) == true)
            //{
            //    LocalizationManager.LocalText text = LocalizationManager.GetLocalText("[ID_Dialog_Default_Value]");
            //    nodeData.body.value = LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR ?
            //        text.kor : text.eng;
            //}
        }

        public void SetBodyValue(string value)
        {
            if (nodeData.body == null)
            {
                nodeData.body = new Body();
            }

            nodeData.body.type = DataType.STRING;
            nodeData.body.value = value;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            input.text = GetNodeInputWithIndex(0).input.default_value;
        }
    }
}