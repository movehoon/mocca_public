using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCFunctionOutputList : FunctionIOListBase<MCFunctionOutputListItem>
	{
        public void SetOutputs(PROJECT.Output[] outputs)
        {
            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                PROJECT.Output output = outputs[ix];
                OnAddClicked();
                activeItems[activeItems.Count - 1].nameInput.text = output.name;
                activeItems[activeItems.Count - 1].typeDropdown.value
                    = Constants.ConvertDataTypeIndexToVariableTypeIndex(output.type);
            }
        }

        public PROJECT.Output[] Outputs
        {
            get
            {
                List<PROJECT.Output> outputs = new List<PROJECT.Output>();
                //foreach (MCFunctionOutputListItem output in activeItems)
                for (int ix = 0; ix < activeItems.Count; ++ix)
                {
                    var output = activeItems[ix];
                    outputs.Add(new PROJECT.Output()
                    {
                        id = ix,
                        name = output.OutputName,
                        type = (PROJECT.DataType)output.DataTypeIndex
                    });
                }

                return outputs.ToArray();
            }
        }
	}
}