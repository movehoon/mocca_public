using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class VariableListItem : MonoBehaviour
	{
        [SerializeField] private Text nameText;
        [SerializeField] private Text valueText;

        public void UpdateName(string name)
        {
            nameText.text = name;
        }

        public void UpdateValue(string value)
        {
            valueText.text = value;
        }

        public void ResetVariable(Transform parentTransform)
        {
            nameText.text = string.Empty;
            valueText.text = string.Empty;

            gameObject.SetActive(false);
            ObjectPool.Instance.PushToPool("VariableListItem", gameObject, parentTransform);
        }

        public string VariableName { get { return nameText.text; } }
        public string VariableValue { get { return valueText.text; } }
    }
}

