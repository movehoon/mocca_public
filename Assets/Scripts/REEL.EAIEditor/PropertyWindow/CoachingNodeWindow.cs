//using UnityEngine;
//using UnityEngine.UI;

//namespace REEL.D2EEditor
//{
//    public class CoachingNodeWindow : MonoBehaviour/*, IShowProperty*/
//    {
//        [SerializeField] private Text nodeTypeText;
//        [SerializeField] private Text nodeIDText;
//        [SerializeField] private InputField nodeTitleInput;
//        [SerializeField] private Text nextIDText;

//        [SerializeField] private GraphItem normalNode;

//        private void OnDisable()
//        {
//            normalNode = null;
//            nodeTitleInput.onValueChanged.RemoveAllListeners();
//        }

//        public void ShowProperty(GraphItem node)
//        {
//            gameObject.SetActive(true);

//            nodeTitleInput.text = node.GetBlockTitle;

//            normalNode = node;
//            nodeTitleInput.onValueChanged.AddListener(normalNode.SetBlockTitle);

//            nodeTypeText.text = node.GetNodeType.ToString();
//            nodeIDText.text = node.BlockID.ToString();

//            nextIDText.text = node.GetNextBlockID.ToString();
//        }
//    }
//}