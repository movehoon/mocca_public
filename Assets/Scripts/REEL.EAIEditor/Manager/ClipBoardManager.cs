using REEL.PROJECT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class VariableInfomation
    {
        public string name = "";
        public DataType type;
        public NodeType nodeType;
        public string value = "";
        public string functionName = "";     // local 변수일 때, 변수가 속한 함수의 이름.

        public VariableInfomation() { }
        public VariableInfomation(VariableInfomation other)
        {
            name = other.name;
            type = other.type;
            nodeType = other.nodeType;
            value = other.value;
            functionName = other.functionName;
        }

        public VariableInfomation(LeftMenuVariableItem variable)
        {
            name = variable.VariableName;
            type = variable.dataType;
            nodeType = variable.nodeType;
            value = variable.value;
            functionName = variable.functionName;
        }

        public override string ToString()
        {
            return $"name: {name}/dataType: {type}/nodeType: {nodeType}/value: {value}/functionName: {functionName}";
        }
    }

    public class ClipBoardManager : Singleton<ClipBoardManager>
    {
        // 클립보드에 저장할 내용 정의.
        [System.Serializable]
        public class ClipBoardContent
        {
            public enum ClipboardType
            {
                Copy, Paste, Undo, Redo, Revove, Add
            }

            public ClipboardType clipboardType;
            //public List<MCNode> copiedNodes = new List<MCNode>();

			public List<Node> copiedNodes = new List<Node>();

            // 작성자: 장세윤.
            // Variable/Funciton 노드 정보.
            // 프로젝트 간 복사할 때 변수/함수가 정의돼있지 않은 경우, 이 정보를 사용해서 추가.
            public List<VariableInfomation> copiedVariables = new List<VariableInfomation>();
            public List<FunctionData> copiedFunction = new List<FunctionData>();
		}

        //public Stack<ClipBoardContent> clipBoardContents = new Stack<ClipBoardContent>();
        
        // 작성자: 장세윤
        // 2020.09.16.
        // 현재는 복사/붙여넣기에만 사용하기 때문에 Stack을 사용할 필요가 없음.
        private ClipBoardContent clipBoardContent;

        public void PushContent(ClipBoardContent newContent)
        {
            if (MCProjectManager.ProjectDescription == null)
            {
                return;
            }

            //clipBoardContents.Push(newContent);

            clipBoardContent = newContent;
        }

        public ClipBoardContent PopContent()
        {
            //if (MCProjectManager.ProjectDescription == null || clipBoardContents.Count == 0)
            //{
            //    return null;
            //}

            //return clipBoardContents.Pop();

            if (MCProjectManager.ProjectDescription == null || clipBoardContent == null)
            {
                return null;
            }

            return clipBoardContent;
        }
    }
}