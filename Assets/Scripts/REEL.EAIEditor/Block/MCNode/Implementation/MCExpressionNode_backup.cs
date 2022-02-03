//using Newtonsoft.Json;
//using REEL.PROJECT;
//using UnityEngine.UI;

//namespace REEL.D2EEditor
//{
//    public class MCExpressionNode : MCNode
//	{
//        //public InputField speechInput;
//        //public Dropdown facialDropdown;
//        //public Dropdown motionDropdown;

//        //private Expression[] expArray;

//        protected override void OnEnable()
//        {
//            if (hasInitialized)
//            {
//                return;
//            }

//            base.OnEnable();

//            nodeData.body = null;
//            nodeData.outputs = null;
//        }

//        protected override void UpdateNodeData()
//        {
//            Input input = new Input();
//            input.id = 0;
//            input.source = inputs[0].input.source;
//            input.subid = inputs[0].input.subid;
//            input.type = REEL.PROJECT.DataType.EXPRESSION;
//            input.default_value = string.Empty;

//			nodeData.inputs = new Input[] { input };

//            nodeData.nexts = new Next[nexts.Length];
//            for (int ix = 0; ix < nexts.Length; ++ix)
//            {
//                nodeData.nexts[ix] = nexts[ix].next;
//            }

//            // Test.
//            // BreakPoint 옵션 설정.
//            // 0:설정 안됨, 1:설정됨.
//            nodeData.hasBreakPoint = hasBreakPoint;
//        }
//    }
//}
