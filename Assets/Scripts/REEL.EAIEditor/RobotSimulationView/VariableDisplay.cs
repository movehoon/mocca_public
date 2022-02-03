using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
	public class VariableDisplay : MonoBehaviour
	{
        [SerializeField] private Transform variableParentTransform;
        private List<VariableListItem> variableItems = new List<VariableListItem>();

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void AddVariable(string name, string value)
        {
            GameObject newVariableObject = ObjectPool.Instance.PopFromPool("VariableListItem", variableParentTransform);
            newVariableObject.SetActive(true);
            VariableListItem newVariable = newVariableObject.GetComponent<VariableListItem>();
            newVariable.UpdateName(name);
            newVariable.UpdateValue(value);
            variableItems.Add(newVariable);
        }

        public void UpdateVariable(SimulationManager.Variable variable)
        {
            UpdateVariable(variable.name, variable.value);
            if (!gameObject.activeSelf) SetActive(true);
        }

        public void UpdateVariable(string name, string value)
        {
            foreach (VariableListItem item in variableItems)
            {
                if (item.VariableName.Equals(name))
                {
                    item.UpdateValue(value);
                    return;
                }
            }

            AddVariable(name, value);
        }

        public void QuitVariableDisplay()
        {
            for (int ix = 0; ix < variableItems.Count; ++ix)
            {
                variableItems[0].ResetVariable(variableParentTransform);
                variableItems.RemoveAt(0);
            }

            SetActive(false);
        }
    }
}