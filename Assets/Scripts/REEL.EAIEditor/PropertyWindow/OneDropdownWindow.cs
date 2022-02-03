#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USINGTMPPRO
using Type = REEL.D2EEditor.MoccaTMPDropdownHelper.Type;
#else
using Type = REEL.D2EEditor.MoccaDropdownHelper.Type;
#endif

namespace REEL.D2EEditor
{
    public class OneDropdownWindow : MCNodeWindow
    {
#if USINGTMPPRO
        [SerializeField] protected TMP_Dropdown nodeValueDropdown;
        [SerializeField] protected TMP_Dropdown targetNodeDropdown;
#else
        [SerializeField] protected Dropdown nodeValueDropdown;
        [SerializeField] protected Dropdown targetNodeDropdown;
#endif

        protected override void OnDisable()
        {
            base.OnDisable();

            nodeValueDropdown.onValueChanged.RemoveAllListeners();
            if (targetNodeDropdown != null)
            {
                targetNodeDropdown.onValueChanged.RemoveAllListeners();
                targetNodeDropdown = null;
            }
        }

        protected Type type;
#if USINGTMPPRO
        protected List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();
#else
        protected List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();
#endif

        protected void SyncDropdown(int value)
        {
            value = nodeValueDropdown.value;
#if USINGTMPPRO
            refNode.GetComponentInChildren<TMP_Dropdown>().value = value;
#else
            normalNode.GetComponentInChildren<Dropdown>().value = value;
#endif
        }

        protected void SyncTwoDropdowns()
        {
            nodeValueDropdown.onValueChanged.AddListener((value) => { targetNodeDropdown.value = value; });
            targetNodeDropdown.onValueChanged.AddListener((value) => { nodeValueDropdown.value = value; });
        }

        protected void SetDropdownList(MCNode node)
        {
#if USINGTMPPRO
            type = node.GetComponentInChildren<MoccaTMPDropdownHelper>().type;
#else
            type = node.GetComponentInChildren<MoccaDropdownHelper>().type;
#endif
            //SetData(type);
            nodeValueDropdown.ClearOptions();
#if USINGTMPPRO
            nodeValueDropdown.AddOptions(Constants.GetTMPOptionDataWithType(type));
#else
            nodeValueDropdown.AddOptions(Constants.GetOptionDataWithType(type));
#endif
        }

        //private void SetData(Type type)
        //{
        //    switch (type)
        //    {
        //        case Type.Facial:
        //            {
        //                optionData = Constants.GetFacialOptionData;
        //            }
        //            break;
        //        case Type.Motion:
        //            {
        //                optionData = Constants.GetMotionOptionData;
        //            }
        //            break;
        //        case Type.Mobility:
        //            {
        //                optionData = Constants.GetMobilityOptionData;
        //            }
        //            break;
        //        case Type.VariableList:
        //            {
        //                optionData = Constants.GetVariableList;
        //            }
        //            break;
        //    }
        //}
    }
}