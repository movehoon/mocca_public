using TMPro;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCNodeWindow : MonoBehaviour, IShowProperty
    {
        //public enum PropertyWindowType
        //{
        //    SAY, EXPRESSION, FACIAL, IF, YESNO, GENDER, EMOTION, SWITCH, GET, START, FOR, OPERATOR
        //}

        //public PropertyWindowType windowType;

        //[SerializeField] protected Text ownedProcessIDText;
        //[SerializeField] protected Text nodeIDText;
        //[SerializeField] private Text[] nextIDTexts;

        [SerializeField] protected TMP_Text nodeTypeText;

        private MCNodeNext[] mcNodeNexts;

        [SerializeField] protected MCNode refNode;
        [SerializeField] protected GameObject[] onOffBlocks;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            // 20201015. 
            // PropertyWindow 활성화/비활성화 과정에서 refNode가 null이 될 수 있는데, 
            // 이때 아래 이벤트 해제를 잘 해줘야함.
            // ShowProperty할 때 타겟 PropertyWindow를 참조해야 할 듯 (리턴받아서?).
            if (refNode != null)
            {
                // 블록의 상태(선연결/해제 등)가 변경될 때 발생하는 이벤트에 등록해제.
                refNode.UnsubscribeOnNodeStateChanged(OnNodeStateChanged);
                refNode = null;
            }
        }

        public virtual void ShowProperty(MCNode node)
        {
            gameObject.SetActive(true);

            refNode = node;

            nodeTypeText.text = node.nodeData.type.ToString();

            InitWindow();

            // 블록의 상태(선연결/해제 등)가 변경될 때 발생하는 이벤트에 등록.
            refNode.SubscribeOnNodeStateChanged(OnNodeStateChanged);

            //ownedProcessIDText.text = node.OwnedProcessID.ToString();
            //nodeIDText.text = node.NodeID.ToString();

            //SetNextIDTexts(node);
        }

        // 각 프로퍼티 창 별로 노드 상태가 바뀌었을 떄 필요한 로직 구현해야 함.
        protected virtual void OnNodeStateChanged()
        {
            UpdateProperty();
        }

        // 노드 상태가 바뀌었을 떄 호출되는 메소드. 각 창 별로 override해서 구현해야 함.
        protected virtual void UpdateProperty()
        {
            //refNode.MakeNode();
        }

        //protected void TurnOnOffInputFieldDropdown(InputField input, bool isInput, Dropdown dropdown, bool isDropdown)
        protected void TurnOnOffInputFieldDropdown(GameObject input, bool isInput, GameObject dropdown, bool isDropdown)
        {
            input.SetActive(isInput);
            dropdown.SetActive(isDropdown);
        }

        protected void InitWindow()
        {
            if (onOffBlocks.Length > 0)
            {
                for (int i = 0; i < onOffBlocks.Length; i++)
                {
                    onOffBlocks[i].SetActive(true);
                }
            }
            //TurnOnOffInputFieldDropdown(true, false);
        }

        protected void TurnOffBlocks()
        {
            for (int i = 0; i < onOffBlocks.Length; i++)
            {
                onOffBlocks[i].SetActive(false);
            }
        }

        //protected void SetNextIDTexts(MCNode node)
        //{
        //    mcNodeNexts = node.GetComponentsInChildren<MCNodeNext>();

        //    if (mcNodeNexts.Length != 0)
        //    {
        //        for (int i = 0; i < nextIDTexts.Length; i++)
        //            nextIDTexts[i].text = mcNodeNexts[i].next.next.ToString();
        //    }
        //    else //next가 없을 때
        //        nextIDTexts[0].text = "NONE";
        //}
    }
}