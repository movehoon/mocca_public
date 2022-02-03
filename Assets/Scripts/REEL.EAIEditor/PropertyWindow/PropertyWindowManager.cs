using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class PropertyWindowManager : Singleton<PropertyWindowManager>
    {
        //public MCNodeWindow[] propertyWindows;
        public PropertyInsideWindow propertyInsideWindow;

        [Header("Property Windows")]
        [SerializeField] private StartNodeWindow startNodeWindow = null;
        [SerializeField] private LongOneInputWindow longOneInputWindow = null;
        [SerializeField] private SimpleOneInputWindow simpleOneInputWindow = null;
        [SerializeField] private FacialNodeWindow facialNodeWindow = null;
        [SerializeField] private ChoiceNodeWindow choiceNodeWindow = null;
        [SerializeField] private RegisterNameNodeWindow registerNameNodeWindow = null;
        [SerializeField] private PickNumNodeWindow pickNumNodeWindow = null;
        [SerializeField] private SwitchNodeWindow switchNodeWindow = null;
        [SerializeField] private ForNodeWindow forNodeWindow = null;
        [SerializeField] private InsertNodeWindow insertNodeWindow = null;
        [SerializeField] private OperatorNodeWindow operatorNodeWindow = null;
        [SerializeField] private MatchNodeWindow matchNodeWindow = null;
        [SerializeField] private GetElemNodeWindow getElemNodeWindow = null;
        [SerializeField] private GeneralNodeWindow generalNodeWindow = null;
        [SerializeField] private GetNodeWindow getNodeWindow = null;
        [SerializeField] private SetNodeWindow setNodeWindow = null;
        [SerializeField] private ExpressionNodeWindow expressionNodeWindow = null;
        [SerializeField] private LogNodeWindow logNodeWindow = null;
        [SerializeField] private ConcatenateTextsNodeWindow concatenateTextsWindow = null;
        [SerializeField] private TeachableMachineNodeWindow teachableMachineNodeWindow = null;
        [SerializeField] private DialogueNodeWindow dialogueNodeWindow = null;
        //[SerializeField] private IfNodeWindow ifNodeWindow = null;
        //[SerializeField] private YesNoNodeWindow yesNoNodeWindow = null;
        //[SerializeField] private GenderNodeWindow genderNodeWindow = null;
        //[SerializeField] private EmotionNodeWindow emotionNodeWindow = null;

        private List<GameObject> windows = new List<GameObject>();

        private void OnEnable()
        {
            windows.Add(startNodeWindow.gameObject);
            windows.Add(longOneInputWindow.gameObject);
            windows.Add(simpleOneInputWindow.gameObject);
            windows.Add(facialNodeWindow.gameObject);
            windows.Add(choiceNodeWindow.gameObject);
            windows.Add(registerNameNodeWindow.gameObject);
            windows.Add(pickNumNodeWindow.gameObject);
            windows.Add(switchNodeWindow.gameObject);
            windows.Add(forNodeWindow.gameObject);
            windows.Add(insertNodeWindow.gameObject);
            windows.Add(operatorNodeWindow.gameObject);
            windows.Add(matchNodeWindow.gameObject);
            windows.Add(getElemNodeWindow.gameObject);
            windows.Add(generalNodeWindow.gameObject);
            windows.Add(getNodeWindow.gameObject);
            windows.Add(setNodeWindow.gameObject);
            windows.Add(expressionNodeWindow.gameObject);
            windows.Add(logNodeWindow.gameObject);
            windows.Add(concatenateTextsWindow.gameObject);
            windows.Add(teachableMachineNodeWindow.gameObject);
            windows.Add(dialogueNodeWindow.gameObject);
        }

        public void ShowProperty(MCNode node)
        {
            //foreach (MCNodeWindow window in propertyWindows)
            //{
            //    if (window.targetWindowType == type)
            //    {
            //        window.ShowProperty(node);
            //    }
            //}

            if (propertyInsideWindow.IsActive == false)
            {
                return;
            }

            TurnOffAll();

            switch (node.nodeData.type) //일단 모든 타입 선언.
            {
                case PROJECT.NodeType.START:
                    startNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.DIALOGUE:
                case PROJECT.NodeType.CHATBOT_PIZZA_ORDER:
                    dialogueNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.SAY:
                case PROJECT.NodeType.LOG:
                case PROJECT.NodeType.CHAT:
                case PROJECT.NodeType.USER_API:
                case PROJECT.NodeType.USER_API_CAMERA:
                case PROJECT.NodeType.DELETE_FACE:
                case PROJECT.NodeType.DETECT_OBJECT:
                case PROJECT.NodeType.WIKI_QNA:
                    longOneInputWindow.ShowProperty(node); break;

                case PROJECT.NodeType.YESNO:
                case PROJECT.NodeType.WHILE:
                case PROJECT.NodeType.IF:
                case PROJECT.NodeType.DELAY:
                case PROJECT.NodeType.ABS:
                case PROJECT.NodeType.ROUND:
                case PROJECT.NodeType.ROUND_UP:
                case PROJECT.NodeType.ROUND_DOWN:
                    simpleOneInputWindow.ShowProperty(node); break;

                case PROJECT.NodeType.FACIAL:
                case PROJECT.NodeType.MOTION:
                case PROJECT.NodeType.MOBILITY:
                case PROJECT.NodeType.PAUSE:
                case PROJECT.NodeType.RESUME:
                    facialNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.CHOICE:
                    choiceNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.REGISTER_NAME:
                    registerNameNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.PICKNUM:
                case PROJECT.NodeType.COACHING:
                    pickNumNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.SWITCH:
                    switchNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.LOOP:
                    forNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.INSERT:
                case PROJECT.NodeType.SET_ELEM:
                    insertNodeWindow.ShowProperty(node); break;
                
                case PROJECT.NodeType.STRCAT:
                    concatenateTextsWindow.ShowProperty(node); break;

                case PROJECT.NodeType.ADD:
                case PROJECT.NodeType.SUB:
                case PROJECT.NodeType.MUL:
                case PROJECT.NodeType.DIV:
                case PROJECT.NodeType.AND:
                case PROJECT.NodeType.OR:
                case PROJECT.NodeType.EQUAL:
                case PROJECT.NodeType.NOT_EQUAL:
                case PROJECT.NodeType.LESS:
                case PROJECT.NodeType.LESS_EQUAL:
                case PROJECT.NodeType.GREATER:
                case PROJECT.NodeType.GREATER_EQUAL:
                case PROJECT.NodeType.RANDOM:
                case PROJECT.NodeType.MOD:
                case PROJECT.NodeType.POWER:
                    operatorNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.MATCH:
                    matchNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.GET_ELEM:
                case PROJECT.NodeType.GET_INDEX:
                case PROJECT.NodeType.REMOVE:
                    getElemNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.STOP:
                case PROJECT.NodeType.SAY_STOP:
                case PROJECT.NodeType.EXPRESS_STOP:
                case PROJECT.NodeType.SPEECH_REC:
                case PROJECT.NodeType.GENDER:
                case PROJECT.NodeType.EMOTION:
                case PROJECT.NodeType.COUNT:
                case PROJECT.NodeType.REMOVEALL:
                case PROJECT.NodeType.NOT:
                case PROJECT.NodeType.DETECT_PERSON:
                case PROJECT.NodeType.HANDS_UP:
                case PROJECT.NodeType.RECOG_FACE:
                case PROJECT.NodeType.AGE_GENDER:
                case PROJECT.NodeType.POSE_REC:
                    generalNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.GET:
                    getNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.SET:
                    setNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.EXPRESS:
                    expressionNodeWindow.ShowProperty(node); break;

                case PROJECT.NodeType.TEACHABLE_MACHINE_SERVER:
                    teachableMachineNodeWindow.ShowProperty(node); break;

                default:
                    generalNodeWindow.ShowProperty(node); break;
            }
        }

        public void TurnOffAll()
        {
            foreach (GameObject obj in windows)
            {
                obj.SetActive(false);
            }
        }
    }

    public interface IShowProperty
    {
        void ShowProperty(MCNode node);
    }
}