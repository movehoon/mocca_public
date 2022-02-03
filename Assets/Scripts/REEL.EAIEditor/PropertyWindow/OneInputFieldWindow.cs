using TMPro;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class OneInputFieldWindow : MCNodeWindow
    {
        [SerializeField] protected TMP_InputField nodeValueInput;
        [SerializeField] protected TMP_InputField targetNodeInput;

        protected override void OnDisable()
        {
            base.OnDisable();

            nodeValueInput.onValueChanged.RemoveAllListeners();
            if (targetNodeInput != null)
            {
                targetNodeInput.onValueChanged.RemoveAllListeners();
                targetNodeInput = null;
            }
        }

        protected void SyncTwoInputField()
        {
            //nodeValueInput.text = targetNodeInput.text;
            nodeValueInput.onValueChanged.AddListener((text) => 
            {
                //Utils.LogGreen($"[SyncTwoInputField] targetNodeInput.text: {targetNodeInput.text} / text: {text}");
                targetNodeInput.text = text;
            });

            targetNodeInput.onValueChanged.AddListener((text) => 
            {
                //Utils.LogGreen($"[SyncTwoInputField] nodeValueInput.text: {nodeValueInput.text} / text: {text}");
                nodeValueInput.text = text;
            });
        }
    }
}