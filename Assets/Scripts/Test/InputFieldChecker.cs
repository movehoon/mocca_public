using UnityEngine;
using UnityEngine.UI;

namespace REEL.Test
{
	public class InputFieldChecker : MonoBehaviour
	{
        [SerializeField] private InputField inputField;

        private void Awake()
        {
            //inputField.onEndEdit.AddListener(OnChanged);
            inputField.onValueChanged.AddListener(OnChanged);
        }

        public void OnChanged(string input)
        {
            //Debug.Log(input);
        }
    }
}