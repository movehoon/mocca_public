using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class TwoInputFieldWindow : OneInputFieldWindow
    {
        [SerializeField] protected TMP_InputField nodeValueInput2;
        [SerializeField] protected TMP_InputField targetNodeInput2;

        protected override void OnDisable()
        {
            base.OnDisable();

            nodeValueInput2.onValueChanged.RemoveAllListeners();
            if (targetNodeInput2 != null)
            {
                targetNodeInput2.onValueChanged.RemoveAllListeners();
                targetNodeInput2 = null;
            }
        }

        protected void SyncSecondInputField(string text)
        {
            text = nodeValueInput2.text;
            if (refNode != null)
            {
                refNode.GetComponentsInChildren<TMP_InputField>()[1].text = text;
            }
        }

        protected void SyncTwoInputFieldSecond()
        {
            //nodeValueInput2.text = targetNodeInput2.text;
            nodeValueInput2.onValueChanged.AddListener((text) => { targetNodeInput2.text = text; });
            targetNodeInput2.onValueChanged.AddListener((text) =>  
            {
                nodeValueInput2.text = text;
            });
        }
    }
}