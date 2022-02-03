using REEL.PROJECT;
using TMPro;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCTeachableMachineNode : MCNode
    {
        public TMP_InputField urlInput;

        protected override void OnEnable()
        {
            if (hasInitialized == true)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;
        }

        public override void SetData(Node node)
        {
            base.SetData(node);

            // 노드 구조가 변경되면서 예외 처리 추가함.
            // 기존에는 입력이 없었는데, 입력이 추가됐음(2021.5.10).
            if (node.inputs != null && node.inputs.Length > 0)
            {
                urlInput.text = node.inputs[0].default_value;
            }
        }

        public void OnPropertyWindowButtonClicked()
        {
            // 현재는 외부 변수는 읽을 수 없어서, InputField에 직접 입력하는 경우만 동작함.
            // Todo: Get 노드를 사용해 변수에 저장된 값을 읽어서 모델 업로드 하는 방안.

            string input = urlInput.text;
            Utils.Log("[Process:Play] TM input is " + input);
            string server_url = "http://" + REEL.D2E.D2EConstants.TEACHABLEMACHINE_IP + ":3000/load";
            StartCoroutine(RestApiUtil.Instance.PostLoadModel(server_url, input, result =>
            {
            }));
        }
    }
}