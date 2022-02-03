using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCFunctionInputList : FunctionIOListBase<MCFunctionInputListItem>
	{
        public override void OnMinusClicked(MCFunctionInputListItem item)
        {
            base.OnMinusClicked(item);

            popup.OnInputMinusClicked();
        }

        public void SetInputs(PROJECT.Input[] inputs)
        {
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                PROJECT.Input input = inputs[ix];
                OnAddClicked();
                activeItems[activeItems.Count - 1].nameInput.text = input.name;
                activeItems[activeItems.Count - 1].typeDropdown.value
                    = Constants.ConvertDataTypeIndexToVariableTypeIndex(input.type);
            }
        }

        public PROJECT.Input[] Inputs
        {
            get
            {
                List<PROJECT.Input> inputs = new List<PROJECT.Input>();
                //foreach (MCFunctionInputListItem function in activeItems)
                if (activeItems.Count > 0)
                {
                    for (int ix = 0; ix < activeItems.Count; ++ix)
                    {
                        var function = activeItems[ix];
                        inputs.Add(new PROJECT.Input()
                        {
                            id = ix,
                            name = function.InputName,
                            type = (PROJECT.DataType)function.DataTypeIndex
                        });
                    }
                }

                return inputs.ToArray();
            }
        }
    }
}