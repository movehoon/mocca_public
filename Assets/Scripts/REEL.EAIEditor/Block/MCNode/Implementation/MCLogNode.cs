#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using REEL.PROJECT;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCLogNode : MCNode
    {
#if USINGTMPPRO
        public TMP_InputField input;
#else
        public InputField input;
#endif

        public Toggle[] logOptions;

        // 0:log, 1:warning, 2:error.
        protected int logLevel = 0;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;
        }

        protected override void UpdateNodeData()
        {
            //base.UpdateNodeData();

            nodeData.inputs = new PROJECT.Input[2];
            nodeData.inputs[0] = new PROJECT.Input();
            nodeData.inputs[0].id = inputs[0].input.id;
            nodeData.inputs[0].type = inputs[0].input.type;
            nodeData.inputs[0].source = inputs[0].input.source;
            nodeData.inputs[0].subid = inputs[0].input.subid;
            nodeData.inputs[0].name = inputs[0].input.name;

            nodeData.inputs[0].default_value = inputs[0].input.default_value;

            if (inputs[0].HasLine)
            {
                nodeData.inputs[0].default_value = string.Empty;
            }

            // Log 레벨 설정 (Log/Warning/Error).
            nodeData.inputs[1] = new PROJECT.Input();
            nodeData.inputs[1].id = 1;
            nodeData.inputs[1].type = DataType.NUMBER;
            nodeData.inputs[1].source = -1;
            nodeData.inputs[1].subid = -1;
            nodeData.inputs[1].name = string.Empty;
            nodeData.inputs[1].default_value = logLevel.ToString();

            nodeData.outputs = new Output[outputs.Length];
            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                nodeData.outputs[ix] = new Output();
                nodeData.outputs[ix].id = outputs[ix].output.id;
                nodeData.outputs[ix].type = outputs[ix].output.type;
                nodeData.outputs[ix].value = outputs[ix].output.value;
                nodeData.outputs[ix].name = outputs[ix].output.name;
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = new Next();
                nodeData.nexts[ix].value = nexts[ix].next.value;
                nodeData.nexts[ix].next = nexts[ix].next.next;
            }

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }

        public override void SetData(Node node)
        {
            //base.SetData(node);

            nodeData.inputs = new Input[node.inputs.Length];

            for (int ix = 0; ix < node.inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new Input();
                nodeData.inputs[ix].id = node.inputs[ix].id;
                nodeData.inputs[ix].type = node.inputs[ix].type;
                nodeData.inputs[ix].source = node.inputs[ix].source;
                nodeData.inputs[ix].subid = node.inputs[ix].subid;
                nodeData.inputs[ix].default_value = node.inputs[ix].default_value;

                //Debug.Log($"[MCSayNode.SetData] node.inputs[ix].default_value: {node.inputs[ix].default_value}");

                if (ix == 0 && inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }

                if (ix == 0)
                {
                    inputs[ix].SetInputData(node.inputs[ix]);
                }   
            }

            input.text = node.inputs[0].default_value;

            if (node.inputs.Length > 1)
            {
                int logLevelValue = int.Parse(node.inputs[1].default_value);
                logOptions[logLevelValue].isOn = true;
            }
            else
            {
                logLevel = 0;
                logOptions[0].isOn = true;
            }

            if (node.nexts != null && node.nexts.Length > 0)
            {
                nodeData.nexts = new Next[node.nexts.Length];
                for (int ix = 0; ix < node.nexts.Length; ++ix)
                {
                    nodeData.nexts[ix] = new Next();
                    nodeData.nexts[ix].next = node.nexts[ix].next;
                    nodeData.nexts[ix].value = node.nexts[ix].value;
                }
            }

            if (node.body != null)
            {
                nodeData.body = new Body();
                nodeData.body.name = node.body.name;
                nodeData.body.isLocalVariable = node.body.isLocalVariable;
                nodeData.body.type = node.body.type;
                nodeData.body.value = node.body.value;
            }

            // Test.
            // 중단점(BreakPoint) 설정 옵션.
            hasBreakPoint = node.hasBreakPoint;
            if (hasBreakPoint == true && breakPoint != null)
            {
                breakPoint.gameObject.SetActive(hasBreakPoint);
            }
        }

        public virtual void ChangeLogLevel(bool isActive, MCLogLevel level)
        {
            if (isActive)
            {
                //Utils.LogGreen($"isActive: {isActive}, level: {level.logLevel}");

                switch (level.logLevel)
                {
                    case LogLevel.Log:
                        logLevel = 0;
                        break;
                    case LogLevel.Warning:
                        logLevel = 1;
                        break;
                    case LogLevel.Error:
                        logLevel = 2;
                        break;
                    default: break;
                }
            }
        }
    }
}