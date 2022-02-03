using System;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class SimulationManager : Singleton<SimulationManager>
    {
        enum BlockPlayStatus
        {
            none, play, done
        }

        [Serializable]
        class StatusMessage
        {
            public string nodeid = string.Empty;
            public string nodestatus = string.Empty;
            public Variable[] variable = null;

            public override string ToString()
            {
                return "nodeid: " + nodeid + " , nodestatus: " + nodestatus + " , variable: " + variable;
            }
        }

        [Serializable]
        public class Variable
        {
            public string name;
            public string value;

            public override string ToString()
            {
                return "name: " + name + ", value: " + value;
            }
        }

        [SerializeField] private VariableDisplay variableDisplay;

        [SerializeField] private Text runButtonText;

        [SerializeField] private int currentBlockID = 0;

        public void UpdateBlockStatus(string message)
        {
            Debug.Log(message);
            StatusMessage status = JsonUtility.FromJson<StatusMessage>(message);
            ProcessBlockStatus(status);
        }

        private void ProcessBlockStatus(StatusMessage status)
        {
            //if (!IsEnded(status))
            //{
            //    if (status.nodestatus.Equals(BlockPlayStatus.play.ToString()))
            //    {
            //        int id = int.Parse(status.nodeid);
            //        if (currentBlockID.Equals(id))
            //            return;

            //        currentBlockID = int.Parse(status.nodeid);
            //        //Debug.Log("currentBlockID: " + currentBlockID);

            //        GraphItem currentItem = WorkspaceManager.Instance.GetGraphItem(currentBlockID);
            //        currentItem.SetSelected();

            //        ShowVariableList(status.variable);

            //        //Debug.Log("Set Item Selected: " + currentBlockID);
            //    }
            //    else if (status.nodestatus.Equals(BlockPlayStatus.done.ToString()))
            //    {
            //        GraphItem currentItem = WorkspaceManager.Instance.GetGraphItem(currentBlockID);
            //        currentItem.SetUnselected();
            //    }
            //}
            //else
            //{
            //    currentBlockID = 0;
            //    WorkspaceManager.Instance.SetSimulationMode(false);
            //    variableDisplay.QuitVariableDisplay();
            //}
        }

        private void ShowVariableList(Variable[] variables)
        {
            if (variables != null && variables.Length > 0)
            {
                for (int ix = 0; ix < variables.Length; ++ix)
                {
                    variableDisplay.UpdateVariable(variables[ix]);
                }
            }
        }

        private void ChangeRunButtonStatus(bool isSimulation)
        {
            Debug.Log("ChangeRunButtonStatus: " + isSimulation);
            runButtonText.text = isSimulation ? "Edit" : "Run";
        }

        private bool IsEnded(StatusMessage status)
        {
            return status.nodeid.Equals("-1");
        }
    }
}