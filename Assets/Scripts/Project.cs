using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using REEL.PROJECT;
using REEL.D2EEditor;

namespace REEL.PROJECT
{
    public enum NodeType
    {
        VARIABLE = 100,
        LIST,
        EXPRESSION,     // Expression Data(noun)
        LOCAL_VARIABLE,
        LOCAL_LIST,
        LOCAL_EXPRESSION,

        START = 200,
        STOP,
        PAUSE,
        RESUME,
        DELAY,
        LOG = 250,

        GET = 300,
        SET,
        BYPASS,

        COUNT = 400,
        GET_ELEM,
        REMOVE,
        REMOVEALL,
        INSERT,
        SET_ELEM,
        GET_INDEX,
        PUSH_ELEM,          // Added 2020-09-09
        EXIST_ELEM,         // Added 2020-09-09
        REMOVE_ELEM,        // Added 2020-09-09

        // Operation
        ADD = 500,
        SUB,
        MUL,
        DIV,
        RANDOM,
        SIN,                // Added 2020-09-09
        COS,                // Added 2020-09-09
        TAN,                // Added 2020-09-09
        ABS,                // Added 2020-09-09
        SQRT,               // Added 2020-09-09
        ROUND,              // Added 2020-09-09
        ROUND_UP,           // Added 2020-09-09
        ROUND_DOWN,         // Added 2020-09-09
        MOD,
        POWER,

        STRCAT = 520,
        STRLEN,             // Added 2020-09-09
        STRCPY,             // Added 2020-09-09
        CONTAINS,           // Added 2020-09-09

        // Bit Operation
        AND = 600,
        OR,
        NOT,
        XOR,                // Added 2020-09-09

        // Comparator
        EQUAL = 700,
        NOT_EQUAL,
        LESS,
        LESS_EQUAL,
        GREATER,
        GREATER_EQUAL,

        // Branch & Loop
        IF = 1000,
        SWITCH,
        WHILE,
        LOOP,
        MATCH,
        BREAK,              // Added 2020-09-09
        CONTINUE,           // Added 2020-09-09

        // RobotAction
        SAY = 2000,
        EXPRESS,        // Express (verb.)
        FACIAL,
        MOTION,
        MOBILITY,
        SAY_STOP,
        EXPRESS_STOP,

        // Recognition
        SPEECH_REC = 3000,

        // Intelligent
        YESNO = 4000,
        GENDER,
        EMOTION,
        AGE_GENDER,
        DETECT_PERSON,
        HANDS_UP,
        RECOG_FACE,
        REGISTER_NAME,
        COACHING,
        PICKNUM,
        CHOICE,
        NUANCE,
        CHAT,           // Chatbot.
        DELETE_FACE,
        POSE_REC,
        DETECT_OBJECT,      // Added 2021-07-02: 장세윤.

        DIALOGUE,           // Added 2021-06-24: 장세윤.
        WIKI_QNA,
        CHATBOT_PIZZA_ORDER,

        USER_API = 4500,
        USER_API_CAMERA,
        TEACHABLE_MACHINE_SERVER,

        FUNCTION = 5000,
        FUNCTION_INPUT,
        FUNCTION_OUTPUT

    }

    public enum DataType
    {
        NONE,
        BOOL,
        NUMBER,
        STRING,
        VARIABLE,
        LIST,
        EXPRESSION,
        // 아래는 UI용.
        // 실제 데이터는 UI에서 string으로 변환해서 사용함.
        FACIAL,
        MOTION,
        MOBILITY,
        FUNCTION,
    }

    [Serializable]
    public class FunctionLastNodeInfo
    {
        public int lineID = -1;
        public int nodeID = 0;
        public int socketIndex = 0;
    }

    [Serializable]
    public class FunctionData
    {
        public int functionID;
        public string name;
        public string description = "";
        public VariableInfomation[] variableReferences;
        public Node[] variables;
        public Input[] inputs;
        public Output[] outputs;
        public Node[] nodes;
        public int startID;
        //public FunctionLastNodeInfo lastNode = new FunctionLastNodeInfo();
        public List<FunctionLastNodeInfo> lastNodes = new List<FunctionLastNodeInfo>();
        //public int lastID;

        // Contructors.
        public FunctionData() { }
        public FunctionData(FunctionData other)
        {
            functionID = other.functionID;
            name = other.name;
            description = other.description;

            SetInputData(other.inputs);
            SetOutputData(other.outputs);
            SetNodesData(other.nodes);
            SetVariableReferences(other.variableReferences);

            startID = other.startID;
            SetLastNodesData(other.lastNodes);
        }

        public void SetInputData(Input[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
            {
                return;
            }

            if (this.inputs == null || this.inputs.Length != inputs.Length)
            {
                this.inputs = new Input[inputs.Length];
            }

            Array.Copy(inputs, this.inputs, inputs.Length);
        }

        public void SetOutputData(Output[] outputs)
        {
            if (outputs == null || outputs.Length == 0)
            {
                return;
            }

            if (this.outputs == null || this.outputs.Length != outputs.Length)
            {
                this.outputs = new Output[outputs.Length];
            }

            Array.Copy(outputs, this.outputs, outputs.Length);
        }

        public void SetLastNodesData(List<FunctionLastNodeInfo> lastNodes)
        {
            if (lastNodes == null || lastNodes.Count == 0)
            {
                return;
            }

            if (this.lastNodes == null || this.lastNodes.Count != lastNodes.Count)
            {
                this.lastNodes = new List<FunctionLastNodeInfo>();
            }

            foreach (FunctionLastNodeInfo info in lastNodes)
            {
                this.lastNodes.Add(info);
            }
        }

        public void SetNodesData(Node[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
            {
                return;
            }

            if (this.nodes == null || this.nodes.Length != nodes.Length)
            {
                this.nodes = new Node[nodes.Length];
            }

            Array.Copy(nodes, this.nodes, nodes.Length);
        }

        public void SetVariableReferences(VariableInfomation[] variableInfomations)
        {
            if (variableInfomations == null || variableInfomations.Length == 0)
            {
                return;
            }

            if (variableReferences == null || variableReferences.Length != variableInfomations.Length)
            {
                variableReferences = new VariableInfomation[variableInfomations.Length];
            }
            
            Array.Copy(variableInfomations, variableReferences, variableInfomations.Length);
        }

        //public void AddLocalVariable(Node variable)
        //{
        //    if (variables == null)
        //    {
        //        variables = new Node[] { variable };
        //        return;
        //    }

        //    if (CanAddVariable(variable) == false)
        //    {
        //        MessageBox.Show("[ID_SAME_LOCAL_VARIABLE]같은(동일한) 이름을 가진 지역 변수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료.
        //        return;
        //    }

        //    Array.Resize(ref variables, variables.Length + 1);
        //    variables[variables.Length - 1] = variable;
        //}

        //public bool DeleteLocalVariable(string variableName)
        //{
        //    if (variables == null || variables.Length == 0)
        //    {
        //        return false;
        //    }

        //    if (variables.Length == 1)
        //    {
        //        variables = null;
        //        return false;
        //    }

        //    List<Node> tempList = new List<Node>();
        //    foreach (Node node in variables)
        //    {
        //        if (node.body.name.Equals(variableName) == true)
        //        {
        //            continue;
        //        }

        //        tempList.Add(node);
        //    }

        //    variables = new Node[tempList.Count];
        //    Array.Copy(tempList.ToArray(), variables, tempList.Count);

        //    return true;
        //}

        //public bool CanAddVariable(Node variable)
        //{
        //    foreach (Node node in variables)
        //    {
        //        if (node.body.name.Equals(variable.body.name) == true)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public string[] LocalVariableNameList
        //{
        //    get
        //    {
        //        List<string> names = new List<string>();
        //        foreach (Node variable in variables)
        //        {
        //            names.Add(variable.body.name);
        //        }

        //        return names.ToArray();
        //    }
        //}

        //public int GetLocalVariableIndexWithName(string name)
        //{
        //    string[] names = LocalVariableNameList;
        //    for (int ix = 0; ix < names.Length; ++ix)
        //    {
        //        if (names[ix].Equals(name))
        //        {
        //            return ix;
        //        }
        //    }

        //    return -1;
        //}

        public FunctionDesc GetFunctionDesc()
        {
            return new FunctionDesc()
            {
                name = this.name,
                description = this.description
            };
        }
    }

    // 작성자: 장세윤.
    // 주석(Comment) 정보를 프로젝트에 저장/로드하기 위한 클래스.
    [Serializable]
    public class Comment
    {
        public string title;
        public Vector2 position;
        public Vector2 size;
    }

    [Serializable]
    public class ProjectData
    {
        public int version;
        public string title;
        public string owner;
        public Node[] variables;
        public FunctionData[] functions;
        //public Node[] nodes;
        public Process[] processes;
        public LocalizationManager.Language language = LocalizationManager.Language.DEFAULT;

        // 작성자: 장세윤.
        // 주석(Comment) 정보.
        public Comment[] comments;
    }

    

    [Serializable]
    public class Process
    {
        public int id;
        public int priority;
        public Node[] nodes;
        public string name;
        public string description;
    }

    [Serializable]
    public class Node
    {
        public NodeType type;
        public int id;
        public Input[] inputs;
        public Body body;
        public Output[] outputs;
        public Next[] nexts;
        public Vector2 nodePosition;
        public string name;
        public string description;

        // 작성자 : 장세윤.
        // 중단점(Break Point) 설정 여부.
        public bool hasBreakPoint = false;

        // 중단점 처리 여부.
        public bool hasProcessed = false;
    }

    [Serializable]
    public class Input
    {
        public int id;
        public DataType type;
        public int source;
        public int subid;
        public string default_value;
        public string name;
    }

    [Serializable]
    public class Body
    {
        public string name;
        public DataType type;
        public bool isLocalVariable;
        public string value;
    }

    [Serializable]
    public class Output
    {
        public int id;
        public DataType type;
        public string value;
        public string name;
    }

    [Serializable]
    public class Next
    {
        public string value;
        public int next;
    }

    [Serializable]
    public class Expression
    {
        public string tts;
        public string facial;
        public string motion;
    }

    [Serializable]
    public class ListValue : IDisposable
    {
        public string[] listValue;

        public void Dispose()
        {
            if (listValue != null && listValue.Length > 0)
            {
                Array.Clear(listValue, 0, listValue.Length);
            }
        }
    }

    [Serializable]
    public class AI_MESSAGE
    {
        public string method;
        public string input;
        public string result;
    }
    [Serializable]
    public class CHOICE_MESSAGE : IDisposable
    {
        public string method;
        public CHOICE_INPUT input;

        public void Dispose()
        {
            method = string.Empty;
            input.Dispose();
        }
    }
    [Serializable]
    public class CHOICE_INPUT : IDisposable
    {
        public string[] choice;
        public string answer;

        public void Dispose()
        {
            if (choice != null && choice.Length > 0)
            {
                Array.Clear(choice, 0, choice.Length);
            }

            answer = string.Empty;
        }
    }
}

public class Project : MonoBehaviour
{


    //----- START
    REEL.PROJECT.Node MakeStart(int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.START;
        node.id = 0;
        node.inputs = null;
        node.body = null;
        node.outputs = null;
        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeStop(int id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.STOP;
        node.id = id;
        node.body = null;
        node.inputs = null;
        node.outputs = null;
        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakePause(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.PAUSE;
        node.id = id;
        node.body = null;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.NUMBER;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeResume(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.RESUME;
        node.id = id;
        node.body = null;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.NUMBER;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeLog(int id, REEL.PROJECT.DataType type, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.LOG;
        node.id = id;
        node.body = null;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = type;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }

    //----- VARIABLE
    REEL.PROJECT.Node MakeList(int id, string name, REEL.PROJECT.DataType type, string value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();
        REEL.PROJECT.Body body = new REEL.PROJECT.Body();

        node.type = REEL.PROJECT.NodeType.LIST;
        node.id = id;
        node.inputs = null;

        body.name = name;
        body.type = type;
        body.value = value;
        node.body = body;

        node.outputs = null;
        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeVariable(int id, string name, REEL.PROJECT.DataType type, string value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();
        REEL.PROJECT.Body body = new REEL.PROJECT.Body();

        node.type = REEL.PROJECT.NodeType.VARIABLE;
        node.id = id;
        node.inputs = null;

        body.name = name;
        body.type = type;
        body.value = value;
        node.body = body;

        node.outputs = null;
        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeExpression(int id, string name, REEL.PROJECT.DataType type, string value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();
        REEL.PROJECT.Body body = new REEL.PROJECT.Body();

        node.type = REEL.PROJECT.NodeType.EXPRESSION;
        node.id = id;
        node.inputs = null;

        body.name = name;
        body.type = type;
        body.value = value;
        node.body = body;

        node.outputs = null;
        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeGet(int id, REEL.PROJECT.DataType type, string variable_name)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GET;
        node.id = id;

        node.inputs = null;

        REEL.PROJECT.Body body = new REEL.PROJECT.Body();
        body.type = type;
        body.name = variable_name;
        node.body = body;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = type;
        output.value = variable_name;
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeSet(int id, REEL.PROJECT.DataType type, int source, int subid, string default_value, string variable_name, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SET;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = type;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        REEL.PROJECT.Body body = new REEL.PROJECT.Body();
        body.type = type;
        body.name = variable_name;
        node.body = body;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = type;
        output.value = variable_name;
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeCount(int id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.COUNT;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.LIST;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.VARIABLE;
        output.value = "_count";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeGetElem(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GET_ELEM;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.STRING;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeSetElem(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2, int source3, int subid3, string default_value3, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SET_ELEM;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[3];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        input[2] = new REEL.PROJECT.Input();
        input[2].id = 2;
        input[2].type = REEL.PROJECT.DataType.STRING;
        input[2].source = source3;
        input[2].subid = subid3;
        input[2].default_value = default_value3;
        node.inputs = input;

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeGetIndex(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GET_INDEX;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.STRING;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeInsert(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2, int source3, int subid3, string default_value3, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.INSERT;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[3];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        input[2] = new REEL.PROJECT.Input();
        input[2].id = 2;
        input[2].type = REEL.PROJECT.DataType.STRING;
        input[2].source = source3;
        input[2].subid = subid3;
        input[2].default_value = default_value3;
        node.inputs = input;

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeRemove(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.REMOVE;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeRemoveAll(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.REMOVEALL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[1];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.BOOL;
        input[0].source = source;
        input[0].subid = subid;
        input[0].default_value = default_value;
        node.inputs = input;

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }

    //----- OPERATOR
    REEL.PROJECT.Node MakeAdd(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.ADD;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeSub(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SUB;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeMul(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.MUL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeDiv(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.DIV;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeRandom(int id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.RANDOM;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[1];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source;
        input[0].subid = subid;
        input[0].default_value = default_value;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeAnd(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.AND;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.BOOL;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.BOOL;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeOr(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.OR;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.BOOL;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.BOOL;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeNot(int id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.AND;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[1];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.BOOL;
        input[0].source = source;
        input[0].subid = subid;
        input[0].default_value = default_value;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }


    //----- COMPARE
    REEL.PROJECT.Node MakeEqual(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.EQUAL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeNotEqual(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.NOT_EQUAL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeLess(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.LESS;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeLessEqual(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.LESS_EQUAL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeGreater(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GREATER;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeGreaterEqual(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GREATER_EQUAL;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.NUMBER;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.NUMBER;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.BOOL;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }
    REEL.PROJECT.Node MakeStrCat(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.STRCAT;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.STRING;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.STRING;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.STRING;
        output.value = "_result";
        node.outputs = new REEL.PROJECT.Output[] { output };

        node.nexts = null;

        return node;
    }

    //----- CONTROL
    REEL.PROJECT.Node MakeIf(int id, int source, int subid, string default_value, int true_id, int false_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.IF;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.BOOL;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next true_next = new REEL.PROJECT.Next();
        true_next.value = "TRUE";
        true_next.next = true_id;
        REEL.PROJECT.Next false_next = new REEL.PROJECT.Next();
        false_next.value = "FALSE";
        false_next.next = false_id;
        node.nexts = new REEL.PROJECT.Next[] { true_next, false_next };

        return node;
    }
    REEL.PROJECT.Node MakeSwitch(int id, REEL.PROJECT.DataType type, int source, int subid, string default_value, REEL.PROJECT.Next[] nexts)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SWITCH;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = type;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        node.nexts = nexts;

        return node;
    }
    REEL.PROJECT.Node MakeLoop(int id, int source, int subid, string default_value, int next_id, int exit_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.LOOP;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.NUMBER;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.VARIABLE;
        output.value = "loop" + id + "_i";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        REEL.PROJECT.Next exit = new REEL.PROJECT.Next();
        exit.value = "EXIT";
        exit.next = exit_id;
        node.nexts = new REEL.PROJECT.Next[] { next, exit };

        return node;
    }
    REEL.PROJECT.Node MakeWhile(int id, int source, int subid, string default_value, int next_id, int exit_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.WHILE;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.BOOL;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        REEL.PROJECT.Next exit = new REEL.PROJECT.Next();
        exit.value = "EXIT";
        exit.next = exit_id;
        node.nexts = new REEL.PROJECT.Next[] { next, exit };

        return node;
    }
    REEL.PROJECT.Node MakeMatch(int id, REEL.PROJECT.DataType type, int source, int subid, string default_value, REEL.PROJECT.Next[] nexts)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.MATCH;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = type;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        node.nexts = nexts;

        return node;
    }

    // ROBOT
    REEL.PROJECT.Node MakeSay(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SAY;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeSayStop(int id, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SAY_STOP;
        node.id = id;

        node.inputs = null;
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeFacial(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.FACIAL;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeMotion(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.MOTION;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeMobility(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.MOBILITY;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeExpress(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.EXPRESS;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.EXPRESSION;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeExpressStop(int id, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.EXPRESS_STOP;
        node.id = id;

        node.inputs = null;
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    // INTELLIGENT
    REEL.PROJECT.Node MakeSpeechRec(int id, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.SPEECH_REC;
        node.id = id;
        node.inputs = null;
        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.VARIABLE;
        output.value = "recognized_speech";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeYesNo(int id, int source, int subid, string default_value, int yes_id, int no_id, int other_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.YESNO;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next yes = new REEL.PROJECT.Next();
        yes.value = "YES";
        yes.next = yes_id;
        REEL.PROJECT.Next no = new REEL.PROJECT.Next();
        no.value = "NO";
        no.next = no_id;
        REEL.PROJECT.Next other = new REEL.PROJECT.Next();
        other.value = "OTHER";
        other.next = other_id;
        node.nexts = new REEL.PROJECT.Next[] { yes, no, other };

        return node;
    }
    REEL.PROJECT.Node MakeGender(int id, int male_id, int female_id, int other_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.GENDER;
        node.id = id;

        node.inputs = null;
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next male = new REEL.PROJECT.Next();
        male.value = "MALE";
        male.next = male_id;
        REEL.PROJECT.Next female = new REEL.PROJECT.Next();
        female.value = "FEMALE";
        female.next = female_id;
        REEL.PROJECT.Next other = new REEL.PROJECT.Next();
        other.value = "OTHER";
        other.next = other_id;
        node.nexts = new REEL.PROJECT.Next[] { male, female, other };

        return node;
    }
    REEL.PROJECT.Node MakeEmotion(int id, int calm_id, int sad_id, int disgusted_id, int happy_id, int confused_id, int angry_id, int surprised_id, int other_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.EMOTION;
        node.id = id;

        node.inputs = null;
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next calm = new REEL.PROJECT.Next();
        calm.value = "CALM";
        calm.next = calm_id;
        REEL.PROJECT.Next sad = new REEL.PROJECT.Next();
        sad.value = "SAD";
        sad.next = sad_id;
        REEL.PROJECT.Next disgusted = new REEL.PROJECT.Next();
        disgusted.value = "DISGUSTED";
        disgusted.next = disgusted_id;
        REEL.PROJECT.Next happy = new REEL.PROJECT.Next();
        happy.value = "HAPPY";
        happy.next = happy_id;
        REEL.PROJECT.Next confused = new REEL.PROJECT.Next();
        confused.value = "CONFUSED";
        confused.next = confused_id;
        REEL.PROJECT.Next angry = new REEL.PROJECT.Next();
        angry.value = "ANGRY";
        angry.next = angry_id;
        REEL.PROJECT.Next surprised = new REEL.PROJECT.Next();
        surprised.value = "SURPRISED";
        surprised.next = surprised_id;
        REEL.PROJECT.Next other = new REEL.PROJECT.Next();
        other.value = "OTHER";
        other.next = other_id;
        node.nexts = new REEL.PROJECT.Next[] { calm, sad, disgusted, happy, confused, angry, surprised, other };

        return node;
    }
    REEL.PROJECT.Node MakeDetectPerson(int id, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.DETECT_PERSON;
        node.id = id;

        node.inputs = null;
        node.body = null;

        REEL.PROJECT.Output output1 = new REEL.PROJECT.Output();
        output1.id = 0;
        output1.type = REEL.PROJECT.DataType.VARIABLE;
        output1.value = "_NUM_PERSON_";
        REEL.PROJECT.Output output2 = new REEL.PROJECT.Output();
        output2.id = 1;
        output2.type = REEL.PROJECT.DataType.LIST;
        output2.value = "_DETECTED_PERSON_";
        node.outputs = new REEL.PROJECT.Output[] { output1, output2 };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeRecogFace(int id, int next_id, int fail_id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.RECOG_FACE;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };
        node.body = null;


        REEL.PROJECT.Output output1 = new REEL.PROJECT.Output();
        output1.id = 0;
        output1.type = REEL.PROJECT.DataType.VARIABLE;
        output1.value = "_PERSON_NAME_";
        node.outputs = new REEL.PROJECT.Output[] { output1 };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        REEL.PROJECT.Next fail = new REEL.PROJECT.Next();
        fail.value = "FAIL";
        fail.next = fail_id;
        node.nexts = new REEL.PROJECT.Next[] { next, fail };

        return node;
    }
    REEL.PROJECT.Node MakeRegisterName(int id, int next_id, int fail_id, int name_source, int name_subid, string name_default_value, int image_source, int image_subid, string image_default_valu)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.REGISTER_NAME;
        node.id = id;

        REEL.PROJECT.Input input1 = new REEL.PROJECT.Input();
        input1.id = 0;
        input1.type = REEL.PROJECT.DataType.STRING;
        input1.source = name_source;
        input1.subid = name_subid;
        input1.default_value = name_default_value;
        REEL.PROJECT.Input input2 = new REEL.PROJECT.Input();
        input2.id = 1;
        input2.type = REEL.PROJECT.DataType.STRING;
        input2.source = image_source;
        input2.subid = image_subid;
        input2.default_value = image_default_valu;
        node.inputs = new REEL.PROJECT.Input[] { input1, input2 };
        node.body = null;

        REEL.PROJECT.Output output1 = new REEL.PROJECT.Output();
        output1.id = 0;
        output1.type = REEL.PROJECT.DataType.VARIABLE;
        output1.value = "_PERSON_NAME_";
        node.outputs = new REEL.PROJECT.Output[] { output1 };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeDeleteName(int id, int next_id, int name_source, int name_subid, string name_default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.DELETE_FACE;
        node.id = id;

        REEL.PROJECT.Input input1 = new REEL.PROJECT.Input();
        input1.id = 0;
        input1.type = REEL.PROJECT.DataType.STRING;
        input1.source = name_source;
        input1.subid = name_subid;
        input1.default_value = name_default_value;
        node.inputs = new REEL.PROJECT.Input[] { input1 };

        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeAgeGender(int id, int next_id, int fail_id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.AGE_GENDER;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };
        node.body = null;

        REEL.PROJECT.Output output1 = new REEL.PROJECT.Output();
        output1.id = 0;
        output1.type = REEL.PROJECT.DataType.VARIABLE;
        output1.value = "_RECOGNIZED_AGE_";
        REEL.PROJECT.Output output2 = new REEL.PROJECT.Output();
        output2.id = 0;
        output2.type = REEL.PROJECT.DataType.VARIABLE;
        output2.value = "_RECOGNIZED_GENDER_";
        node.outputs = new REEL.PROJECT.Output[] { output1, output2 };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        REEL.PROJECT.Next fail = new REEL.PROJECT.Next();
        fail.value = "fail";
        fail.next = fail_id;
        node.nexts = new REEL.PROJECT.Next[] { next, fail };

        return node;
    }
    REEL.PROJECT.Node MakeHandsUP(int id, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.HANDS_UP;
        node.id = id;


        node.inputs = null;
        node.body = null;

        REEL.PROJECT.Output output1 = new REEL.PROJECT.Output();
        output1.id = 0;
        output1.type = REEL.PROJECT.DataType.VARIABLE;
        output1.value = "_HANDSUP_X_";
        REEL.PROJECT.Output output2 = new REEL.PROJECT.Output();
        output2.id = 1;
        output2.type = REEL.PROJECT.DataType.VARIABLE;
        output2.value = "_HANDSUP_PERSON_";
        node.outputs = new REEL.PROJECT.Output[] { output1, output2 };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }

    REEL.PROJECT.Node MakePoseRecognition(int id, int next_id, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.POSE_REC;
        node.id = id;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.STRING; //string for pose
        output.value = "_RECOGNIZED_POSE_";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;

        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }

    REEL.PROJECT.Node MakeCoaching(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.COACHING;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.STRING;
        output.value = "_COACHING_RESULT_";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakePickNum(int id, int source, int subid, string default_value, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.PICKNUM;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_PICKNUM_RESULT_";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeChoice(int id, int source1, int subid1, string default_value1, int source2, int subid2, string default_value2, int next_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.CHOICE;
        node.id = id;

        REEL.PROJECT.Input[] input = new REEL.PROJECT.Input[2];
        input[0] = new REEL.PROJECT.Input();
        input[0].id = 0;
        input[0].type = REEL.PROJECT.DataType.LIST;
        input[0].source = source1;
        input[0].subid = subid1;
        input[0].default_value = default_value1;
        input[1] = new REEL.PROJECT.Input();
        input[1].id = 1;
        input[1].type = REEL.PROJECT.DataType.STRING;
        input[1].source = source2;
        input[1].subid = subid2;
        input[1].default_value = default_value2;
        node.inputs = input;

        node.body = null;

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "_CHOICE_RESULT_";
        node.outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = next_id;
        node.nexts = new REEL.PROJECT.Next[] { next };

        return node;
    }
    REEL.PROJECT.Node MakeNuance(int id, int source, int subid, string default_value, int pos_id, int neg_id, int other_id)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.NUANCE;
        node.id = id;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.STRING;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };
        node.body = null;
        node.outputs = null;

        REEL.PROJECT.Next pos = new REEL.PROJECT.Next();
        pos.value = "POS";
        pos.next = pos_id;
        REEL.PROJECT.Next neg = new REEL.PROJECT.Next();
        neg.value = "NEG";
        neg.next = neg_id;
        REEL.PROJECT.Next other = new REEL.PROJECT.Next();
        other.value = "OTHER";
        other.next = other_id;
        node.nexts = new REEL.PROJECT.Next[] { pos, neg, other };

        return node;
    }

    REEL.PROJECT.Node MakeFunction(int id, int start_id, REEL.PROJECT.Input[] inputs, REEL.PROJECT.Output[] outputs, REEL.PROJECT.Next[] nexts, string name = "TestFunction")
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.FUNCTION;
        node.id = id;

        REEL.PROJECT.Body body = new REEL.PROJECT.Body();
        body.name = "Function";
        body.type = REEL.PROJECT.DataType.FUNCTION;
        body.value = "{\"start_id\": " + start_id + "}";
        node.body = body;

        node.inputs = inputs;
        node.outputs = outputs;
        node.nexts = nexts;

        return node;
    }
    REEL.PROJECT.Node MakeBypass(int id, REEL.PROJECT.DataType type, int source, int subid, string default_value)
    {
        REEL.PROJECT.Node node = new REEL.PROJECT.Node();

        node.type = REEL.PROJECT.NodeType.BYPASS;
        node.id = id;
        node.body = null;

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = type;
        input.source = source;
        input.subid = subid;
        input.default_value = default_value;
        node.inputs = new REEL.PROJECT.Input[] { input };

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = type;
        output.value = "";
        node.outputs = new REEL.PROJECT.Output[] { output };
        node.nexts = null;

        return node;
    }

    public void SaveIntelligenceExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();

        project.title = "intelligence";
        project.owner = "reel";

        nodes.Add(MakeStart(1));

        nodes.Add(MakeSay(1, -1, -1, "안녕하세요. 저를 바라봐주세요.", 2));
        nodes.Add(MakeGender(2, 3, 4, 5));
        nodes.Add(MakeSay(3, -1, -1, "남성이시군요", 6));
        nodes.Add(MakeSay(4, -1, -1, "여성이시군요", 6));
        nodes.Add(MakeSay(5, -1, -1, "성별을 잘 모르겠어요", 6));
        nodes.Add(MakeEmotion(6, 7, 8, 9, 10, 11, 12, 13, 14));
        nodes.Add(MakeSay(7, -1, -1, "평온하시네요", 15));
        nodes.Add(MakeSay(8, -1, -1, "슬퍼보여요", 15));
        nodes.Add(MakeSay(9, -1, -1, "불편해보이시네요", 15));
        nodes.Add(MakeSay(10, -1, -1, "행복해보이시네요", 15));
        nodes.Add(MakeSay(11, -1, -1, "혼란스러워보이네요", 15));
        nodes.Add(MakeSay(12, -1, -1, "화나보이세요", 15));
        nodes.Add(MakeSay(13, -1, -1, "놀라워보이시네요", 15));
        nodes.Add(MakeSay(14, -1, -1, "어떤 표정인지 잘 모르겠네요", 15));
        nodes.Add(MakeSay(15, -1, -1, "진단을 마치겠습니다", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);

        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveOxQuizExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "ox_quiz";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<string> questions = new List<string>();
        questions.Add("토사구팽이란 고사성어와 연관이 있는 동물은 개와 토끼이다");
        questions.Add("사람의 세포는 개미의 세포보다 크다");
        questions.Add("벼룩은 간을 가지고 있다");
        REEL.PROJECT.ListValue lv = new REEL.PROJECT.ListValue();
        lv.listValue = questions.ToArray();
        variables.Add(MakeList(-100, "Questions", REEL.PROJECT.DataType.STRING, JsonUtility.ToJson(lv)));

        lv.listValue = new string[] {
            "O", "X", "X"
        };
        variables.Add(MakeList(-101, "Answers", REEL.PROJECT.DataType.STRING, JsonUtility.ToJson(lv)));

        variables.Add(MakeVariable(-102, "Answer", REEL.PROJECT.DataType.STRING, ""));
        variables.Add(MakeVariable(-103, "Score", REEL.PROJECT.DataType.NUMBER, "0"));

        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));
        nodes.Add(MakeSay(1, -1, -1, "세 문제를 풀어보세요", 2));

        nodes.Add(MakeLoop(2, 4, 0, null, 5, 21));

        nodes.Add(MakeGet(3, REEL.PROJECT.DataType.LIST, "Questions"));

        nodes.Add(MakeCount(4, 3, 0, null));

        nodes.Add(MakeSay(5, 7, 0, null, 8));

        nodes.Add(MakeGet(6, REEL.PROJECT.DataType.LIST, "Questions"));

        nodes.Add(MakeGetElem(7, 6, 0, null, 2, 0, null));

        nodes.Add(MakeSpeechRec(8, 9));

        nodes.Add(MakeYesNo(9, 8, 0, null, 10, 11, 12));

        nodes.Add(MakeSet(10, REEL.PROJECT.DataType.STRING, -1, -1, "O", "Answer", 13));
        nodes.Add(MakeSet(11, REEL.PROJECT.DataType.STRING, -1, -1, "X", "Answer", 13));

        nodes.Add(MakeSay(12, -1, -1, "다시 말해주세요", 8));

        nodes.Add(MakeIf(13, 17, 0, null, 18, -1));

        nodes.Add(MakeGet(14, REEL.PROJECT.DataType.LIST, "Answers"));

        nodes.Add(MakeGetElem(15, 14, 0, null, 2, 0, null));

        nodes.Add(MakeGet(16, REEL.PROJECT.DataType.STRING, "Answer"));

        nodes.Add(MakeEqual(17, 16, 0, null, 15, 0, null));

        nodes.Add(MakeSet(18, REEL.PROJECT.DataType.STRING, 20, 0, "0", "Score", 2));

        nodes.Add(MakeGet(19, REEL.PROJECT.DataType.NUMBER, "Score"));

        nodes.Add(MakeAdd(20, 19, 0, null, -1, -1, "1"));

        REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[4];
        nexts[0] = new REEL.PROJECT.Next();
        nexts[0].value = "1";
        nexts[0].next = 23;
        nexts[1] = new REEL.PROJECT.Next();
        nexts[1].value = "2";
        nexts[1].next = 24;
        nexts[2] = new REEL.PROJECT.Next();
        nexts[2].value = "3";
        nexts[2].next = 25;
        nexts[3] = new REEL.PROJECT.Next();
        nexts[3].value = "DEFAULT";
        nexts[3].next = 26;
        nodes.Add(MakeSwitch(21, REEL.PROJECT.DataType.NUMBER, 22, 0, null, nexts));

        nodes.Add(MakeGet(22, REEL.PROJECT.DataType.NUMBER, "Score"));

        nodes.Add(MakeSay(23, -1, -1, "한 문제 맞추셨네요", 27));
        nodes.Add(MakeSay(24, -1, -1, "두 문제 맞추셨네요", 27));
        nodes.Add(MakeSay(25, -1, -1, "세 문제 맞추셨네요", 27));
        nodes.Add(MakeSay(26, -1, -1, "하나도 못맞췄어요", 27));

        nodes.Add(MakeSay(27, -1, -1, "그럼 안녕", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

        //        string path = Path.Combine(Application.persistentDataPath, "ox_quiz.json");

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveTestAIBlock()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "test_AI";
        project.owner = "KDY";


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));
        nodes.Add(MakeSay(1, -1, -1, "카메라를 봐라봐주세요", 2));

        nodes.Add(MakeDetectPerson(2, 3));

        nodes.Add(MakeStrCat(4, 2, 0, null, -1, -1, "명 입니다"));

        nodes.Add(MakeSay(3, 4, 0, null, 6));

        nodes.Add(MakeCount(5, 2, 1, null));

        nodes.Add(MakeLoop(6, 5, 0, null, 7, 15));

        nodes.Add(MakeGetElem(8, 2, 1, null, 6, 0, null));

        nodes.Add(MakeAgeGender(7, 9, -1, 8, 0, null));

        nodes.Add(MakeSay(9, 7, 0, null, 10));

        nodes.Add(MakeSay(10, 7, 1, null, 11));

        nodes.Add(MakeRecogFace(11, 12, 19, 8, 0, null));

        nodes.Add(MakeSay(19, -1, -1, "이름을 입력해주세요", 13));

        nodes.Add(MakeSpeechRec(13, 14));

        nodes.Add(MakeRegisterName(14, 12, -1, 13, 0, null, 8, 0, null));

        nodes.Add(MakeSay(12, 11, 0, null, -1));

        nodes.Add(MakeHandsUP(15, 16));

        nodes.Add(MakeSay(16, 15, 0, null, 17));

        nodes.Add(MakeRecogFace(17, 18, -1, 15, 1, null));

        nodes.Add(MakeSay(18, 17, 0, null, -1));

        //


        //nodes.Add(MakeGet(3, REEL.PROJECT.DataType.LIST, "Questions"));

        //nodes.Add(MakeCount(4, 3, 0, null));

        //nodes.Add(MakeSay(5, 7, 0, null, 8));

        //nodes.Add(MakeGet(6, REEL.PROJECT.DataType.LIST, "Questions"));

        //nodes.Add(MakeGetElem(7, 6, 0, null, 2, 0, null));

        //nodes.Add(MakeSpeechRec(8, 9));

        //nodes.Add(MakeYesNo(9, 8, 0, null, 10, 11, 12));

        //nodes.Add(MakeSet(10, REEL.PROJECT.DataType.STRING, -1, -1, "O", "Answer", 13));
        //nodes.Add(MakeSet(11, REEL.PROJECT.DataType.STRING, -1, -1, "X", "Answer", 13));

        //nodes.Add(MakeSay(12, -1, -1, "다시 말해주세요", 8));

        //nodes.Add(MakeIf(13, 17, 0, null, 18, -1));

        //nodes.Add(MakeGet(14, REEL.PROJECT.DataType.LIST, "Answers"));

        //nodes.Add(MakeGetElem(15, 14, 0, null, 2, 0, null));

        //nodes.Add(MakeGet(16, REEL.PROJECT.DataType.STRING, "Answer"));

        //nodes.Add(MakeEqual(17, 16, 0, null, 15, 0, null));

        //nodes.Add(MakeSet(18, REEL.PROJECT.DataType.STRING, 20, 0, "0", "Score", 2));

        //nodes.Add(MakeGet(19, REEL.PROJECT.DataType.NUMBER, "Score"));

        //nodes.Add(MakeAdd(20, 19, 0, null, -1, -1, "1"));

        //REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[4];
        //nexts[0] = new REEL.PROJECT.Next();
        //nexts[0].value = "1";
        //nexts[0].next = 23;
        //nexts[1] = new REEL.PROJECT.Next();
        //nexts[1].value = "2";
        //nexts[1].next = 24;
        //nexts[2] = new REEL.PROJECT.Next();
        //nexts[2].value = "3";
        //nexts[2].next = 25;
        //nexts[3] = new REEL.PROJECT.Next();
        //nexts[3].value = "DEFAULT";
        //nexts[3].next = 26;
        //nodes.Add(MakeSwitch(21, REEL.PROJECT.DataType.NUMBER, 22, 0, null, nexts));

        //nodes.Add(MakeGet(22, REEL.PROJECT.DataType.NUMBER, "Score"));

        //nodes.Add(MakeSay(23, -1, -1, "한 문제 맞추셨네요", 27));
        //nodes.Add(MakeSay(24, -1, -1, "두 문제 맞추셨네요", 27));
        //nodes.Add(MakeSay(25, -1, -1, "세 문제 맞추셨네요", 27));
        //nodes.Add(MakeSay(26, -1, -1, "하나도 못맞췄어요", 27));

        //nodes.Add(MakeSay(27, -1, -1, "그럼 안녕", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

        //        string path = Path.Combine(Application.persistentDataPath, "ox_quiz.json");

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }


    public void SaveExpressonExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "expression";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<REEL.PROJECT.Expression> expressions = new List<REEL.PROJECT.Expression>();
        REEL.PROJECT.Expression express1 = new REEL.PROJECT.Expression();
        express1.tts = "안녕하세요";
        express1.facial = "smile";
        express1.motion = "hello";
        expressions.Add(express1);
        REEL.PROJECT.Expression express2 = new REEL.PROJECT.Expression();
        express2.tts = "제 이름은 모카예요";
        express2.facial = "tell";
        express2.motion = "hi";
        expressions.Add(express2);
        REEL.PROJECT.Expression express3 = new REEL.PROJECT.Expression();
        express3.tts = "만나서 정말 반가워요";
        express3.facial = "smile";
        express3.motion = "hello";
        expressions.Add(express3);
        string exp = JsonConvert.SerializeObject(expressions);
        //Debug.Log(exp);
        variables.Add(MakeExpression(-100, "Greet", REEL.PROJECT.DataType.EXPRESSION, exp));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeExpress(1, 2, 0, null, 3));
        nodes.Add(MakeGet(2, REEL.PROJECT.DataType.EXPRESSION, "Greet"));
        nodes.Add(MakeSay(3, -1, -1, "웃어볼께요", 4));
        nodes.Add(MakeFacial(4, -1, -1, "smile", 5));
        nodes.Add(MakeSay(5, -1, -1, "안녕 인사입니다", 6));
        nodes.Add(MakeMotion(6, -1, -1, "hi", 7));
        nodes.Add(MakeSay(7, -1, -1, "앞으로 이동합니다", 8));
        nodes.Add(MakeMobility(8, -1, -1, "forward", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveFunctionExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "function";
        project.owner = "reel";

        // Make function contents
        List<REEL.PROJECT.Node> function_nodes = new List<REEL.PROJECT.Node>();
        function_nodes.Add(MakeSay(101, -1, -1, "입력하신 값은", 102));
        function_nodes.Add(MakeSay(102, 103, 0, null, -2));
        function_nodes.Add(MakeBypass(103, REEL.PROJECT.DataType.FUNCTION, 0, 0, ""));  
        // BYPASS의 입력일 경우 TYPE을 FUNCTION으로 하여 FUNCTION의 입력 index(subid)를 가리킴
        function_nodes.Add(MakeAdd(104, 103, 0, null, -1, -1, "3"));
        function_nodes.Add(MakeBypass(105, REEL.PROJECT.DataType.NUMBER, 104, 0, ""));
        //project.nodes = function_nodes.ToArray();
        project.functions = new FunctionData[] { new FunctionData() };
        project.functions[0].nodes = function_nodes.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeLoop(1, -1, -1, "3", 2, 5));

        nodes.Add(MakeSay(3, -1, -1, "출력 값은", 4));
        nodes.Add(MakeSay(4, 105, 0, null, -1));
        nodes.Add(MakeSay(5, -1, -1, "수고하셨습니다", -1));

        REEL.PROJECT.Input input = new REEL.PROJECT.Input();
        input.id = 0;
        input.type = REEL.PROJECT.DataType.NUMBER;
        input.source = 1;
        input.subid = 0;
        input.default_value = null;
        REEL.PROJECT.Input[] inputs = new REEL.PROJECT.Input[] { input };

        REEL.PROJECT.Output output = new REEL.PROJECT.Output();
        output.id = 0;
        output.type = REEL.PROJECT.DataType.NUMBER;
        output.value = "0";
        REEL.PROJECT.Output[] outputs = new REEL.PROJECT.Output[] { output };

        REEL.PROJECT.Next next = new REEL.PROJECT.Next();
        next.value = "NEXT";
        next.next = 3;
        REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[] { next };

        nodes.Add(MakeFunction(2, 101, inputs, outputs, nexts));
        project.functions[0].inputs = inputs;
        project.functions[0].outputs = outputs;


        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveIntroduceKoExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "introduce_ko";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<REEL.PROJECT.Expression> expressions = new List<REEL.PROJECT.Expression>();
        REEL.PROJECT.Expression express1 = new REEL.PROJECT.Expression();
        express1.tts = "안녕하세요. 저는 소셜로봇 모카입니다.";
        express1.facial = "smile";
        express1.motion = "hello";
        expressions.Add(express1);
        REEL.PROJECT.Expression express2 = new REEL.PROJECT.Expression();
        express2.tts = "저는 사회적 상호작용을 기반으로 사람들을 돕기위해 한성대학교 교육로봇연구실에서 만들어졌습니다.";
        express2.facial = "gazeup";
        express2.motion = "clap";
        expressions.Add(express2);
        REEL.PROJECT.Expression express3 = new REEL.PROJECT.Expression();
        express3.tts = "저는 사람들과의 공감을 위해 스마트폰의 카메라로 감정을 인식하여 상황을 파악하고";
        express3.facial = "happy";
        express3.motion = "wrong";
        expressions.Add(express3);
        REEL.PROJECT.Expression express4 = new REEL.PROJECT.Expression();
        express4.tts = "스마트폰 화면의 표정과";
        express4.facial = "winkleft";
        express4.motion = "nodLeft";
        expressions.Add(express4);
        REEL.PROJECT.Expression express5 = new REEL.PROJECT.Expression();
        express5.tts = "양 팔을 사용한 여러가지 동작으로 사람들과 상호작용을 할 수 있습니다.";
        express5.facial = "surprised";
        express5.motion = "angry";
        expressions.Add(express5);
        string exp = JsonConvert.SerializeObject(expressions);
        //Debug.Log(exp);
        variables.Add(MakeExpression(-100, "Greet", REEL.PROJECT.DataType.EXPRESSION, exp));


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeExpress(1, 2, 0, null, -1));
        nodes.Add(MakeGet(2, REEL.PROJECT.DataType.EXPRESSION, "Greet"));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveIntroduceEnExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "introduce_en";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<REEL.PROJECT.Expression> expressions = new List<REEL.PROJECT.Expression>();
        REEL.PROJECT.Expression express1 = new REEL.PROJECT.Expression();
        express1.tts = "Hello, I’m social robot mocha.";
        express1.facial = "smile";
        express1.motion = "hello";
        expressions.Add(express1);
        REEL.PROJECT.Expression express2 = new REEL.PROJECT.Expression();
        express2.tts = "I made in Robots in Education and entertainment laboratory of Hansung university for helping people by social interaction.";
        express2.facial = "gazeup";
        express2.motion = "clap";
        expressions.Add(express2);
        REEL.PROJECT.Expression express3 = new REEL.PROJECT.Expression();
        express3.tts = "I can recognize environment by using camera of smart phone";
        express3.facial = "happy";
        express3.motion = "wrong";
        expressions.Add(express3);
        REEL.PROJECT.Expression express4 = new REEL.PROJECT.Expression();
        express4.tts = "and communicate with people by facial on smart phone";
        express4.facial = "winkleft";
        express4.motion = "nodLeft";
        expressions.Add(express4);
        REEL.PROJECT.Expression express5 = new REEL.PROJECT.Expression();
        express5.tts = "and gesture with both arms";
        express5.facial = "surprised";
        express5.motion = "angry";
        expressions.Add(express5);
        string exp = JsonConvert.SerializeObject(expressions);
        //Debug.Log(exp);
        variables.Add(MakeExpression(-100, "Greet", REEL.PROJECT.DataType.EXPRESSION, exp));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));
        nodes.Add(MakeExpress(1, 2, 0, null, -1));
        nodes.Add(MakeGet(2, REEL.PROJECT.DataType.EXPRESSION, "Greet"));


        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveMultiProcessExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "multi_process";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<REEL.PROJECT.Expression> expressions = new List<REEL.PROJECT.Expression>();
        REEL.PROJECT.Expression express1 = new REEL.PROJECT.Expression();
        express1.tts = "안녕하세요. 저는 소셜로봇 모카입니다.";
        express1.facial = "smile";
        express1.motion = "hello";
        expressions.Add(express1);
        REEL.PROJECT.Expression express2 = new REEL.PROJECT.Expression();
        express2.tts = "저는 사회적 상호작용을 기반으로 사람들을 돕기위해 한성대학교 교육로봇연구실에서 만들어졌습니다.";
        express2.facial = "gazeup";
        express2.motion = "clap";
        expressions.Add(express2);
        REEL.PROJECT.Expression express3 = new REEL.PROJECT.Expression();
        express3.tts = "저는 사람들과의 공감을 위해 스마트폰의 카메라로 감정을 인식하여 상황을 파악하고";
        express3.facial = "happy";
        express3.motion = "wrong";
        expressions.Add(express3);
        REEL.PROJECT.Expression express4 = new REEL.PROJECT.Expression();
        express4.tts = "스마트폰 화면의 표정과";
        express4.facial = "winkleft";
        express4.motion = "nodLeft";
        expressions.Add(express4);
        REEL.PROJECT.Expression express5 = new REEL.PROJECT.Expression();
        express5.tts = "양 팔을 사용한 여러가지 동작으로 사람들과 상호작용을 할 수 있습니다.";
        express5.facial = "surprised";
        express5.motion = "angry";
        expressions.Add(express5);
        string exp = JsonConvert.SerializeObject(expressions);
        //Debug.Log(exp);
        variables.Add(MakeExpression(-100, "Greet", REEL.PROJECT.DataType.EXPRESSION, exp));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();

        List<REEL.PROJECT.Node> nodes1 = new List<REEL.PROJECT.Node>();
        nodes1.Add(MakeStart(2));
        nodes1.Add(MakeSay(1, 0, 0, "'2019 로보월드' 개막식이 유정열 산업통상자원부 산업정책실장,한국로봇산업진흥원, 한국로봇산업협회,한국로봇융합연구원,제어로봇시스템학회 등 120여명의 관계자들이 참석한 가운데 10일 킨텍스 '2019 로보월드' 전시장에서 열렸다.", 2));
        nodes1.Add(MakeExpress(2, -100, 0, null, 4));
        //nodes1.Add(MakeGet(3, REEL.PROJECT.DataType.EXPRESSION, "Greet"));
        nodes1.Add(MakeSay(4, 0, 0, "종료합니다.", 5));
        nodes1.Add(MakeStop(5));

        REEL.PROJECT.Process process1 = new REEL.PROJECT.Process();
        process1.id = 0;
        process1.priority = 5;
        process1.nodes = nodes1.ToArray();
        processes.Add(process1);


        List<REEL.PROJECT.Node> nodes2 = new List<REEL.PROJECT.Node>();
        nodes2.Add(MakeStart(1));
        nodes2.Add(MakeLog(1, REEL.PROJECT.DataType.STRING, 0, 0, "정지, 다시, 종료라고 말하세요", 2));
        nodes2.Add(MakeWhile(2, -1, -1, "true", 3, -1));
        nodes2.Add(MakeSpeechRec(3, 4));

        REEL.PROJECT.Next next1 = new REEL.PROJECT.Next();
        next1.value = "정지";
        next1.next = 5;
        REEL.PROJECT.Next next2 = new REEL.PROJECT.Next();
        next2.value = "다시";
        next2.next = 6;
        REEL.PROJECT.Next next3 = new REEL.PROJECT.Next();
        next3.value = "종료";
        next3.next = 7;
        REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[] { next1, next2, next3 };
        nodes2.Add(MakeMatch(4, REEL.PROJECT.DataType.STRING, 3, 0, null, nexts));
        nodes2.Add(MakePause(5, 0, 0, "0", -1));
        nodes2.Add(MakeResume(6, 0, 0, "0", -1));
//        nodes2.Add(MakeSayStop(6, -1));
        nodes2.Add(MakeExpressStop(7, -1));

        REEL.PROJECT.Process process2 = new REEL.PROJECT.Process();
        process2.id = 1;
        process2.priority = 7;
        process2.nodes = nodes2.ToArray();
        processes.Add(process2);

        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveListOpExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "list_operation";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<string> fruits = new List<string>();
        fruits.Add("사과");
        fruits.Add("배");
        REEL.PROJECT.ListValue lv = new REEL.PROJECT.ListValue();
        lv.listValue = fruits.ToArray();
        variables.Add(MakeList(-100, "Fruits", REEL.PROJECT.DataType.STRING, JsonUtility.ToJson(lv)));
        List<string> fruits_source = new List<string>();
        REEL.PROJECT.ListValue lv2 = new REEL.PROJECT.ListValue();
        lv2.listValue = fruits_source.ToArray();
        variables.Add(MakeList(-101, "FruitsSource", REEL.PROJECT.DataType.STRING, JsonUtility.ToJson(lv2)));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Node> function_nodes = new List<REEL.PROJECT.Node>();
        function_nodes.Add(MakeSay(101, 0, 0, "과일은", 102));
        function_nodes.Add(MakeLoop(102, 103, 0, null, 105, 108));
        function_nodes.Add(MakeCount(103, -101, 0, ""));
        function_nodes.Add(MakeSay(105, 106, 0, "", -1));
        function_nodes.Add(MakeGetElem(106, -101, 0, null, 102, 0, null));
        function_nodes.Add(MakeSay(108, 111, 0, "", -2));
        function_nodes.Add(MakeCount(109, -101, 0, ""));
        function_nodes.Add(MakeStrCat(111, 109, 0, null, 0, 0, "개 입니다"));
        //project.nodes = function_nodes.ToArray();
        project.functions = new FunctionData[] { new FunctionData() };
        project.functions[0].nodes = function_nodes.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(50));

        nodes.Add(MakeSet(50, DataType.LIST, -100, 0, "", "FruitsSource", 1));

        REEL.PROJECT.Next next1 = new REEL.PROJECT.Next();
        next1.value = "NEXT";
        next1.next = 2;
        REEL.PROJECT.Next[] nexts1 = new REEL.PROJECT.Next[] { next1 };
        nodes.Add(MakeFunction(1, 101, null, null, nexts1));

        nodes.Add(MakeInsert(2, -101, 0, "", 0, 0, "1", 0, 0, "포도", 4));

        REEL.PROJECT.Next next2 = new REEL.PROJECT.Next();
        next2.value = "NEXT";
        next2.next = 5;
        REEL.PROJECT.Next[] nexts2 = new REEL.PROJECT.Next[] { next2 };
        nodes.Add(MakeFunction(4, 101, null, null, nexts2));

        nodes.Add(MakeRemove(5, -101, 0, "", 0, 0, "2", 7));

        REEL.PROJECT.Next next3 = new REEL.PROJECT.Next();
        next3.value = "NEXT";
        next3.next = 8;
        REEL.PROJECT.Next[] nexts3 = new REEL.PROJECT.Next[] { next3 };
        nodes.Add(MakeFunction(7, 101, null, null, nexts3));

        nodes.Add(MakeSetElem(8, -101, 0, "", 0, 0, "1", 0, 0, "키위", 10));

        REEL.PROJECT.Next next4 = new REEL.PROJECT.Next();
        next4.value = "NEXT";
        next4.next = 11;
        REEL.PROJECT.Next[] nexts4 = new REEL.PROJECT.Next[] { next4 };
        nodes.Add(MakeFunction(10, 101, null, null, nexts4));

        nodes.Add(MakeRemoveAll(11, -101, 0, "", 13));

        REEL.PROJECT.Next next5 = new REEL.PROJECT.Next();
        next5.value = "NEXT";
        next5.next = 14;
        REEL.PROJECT.Next[] nexts5 = new REEL.PROJECT.Next[] { next5 };
        nodes.Add(MakeFunction(13, 101, null, null, nexts5));

        nodes.Add(MakeStop(14));


        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }


    public void SaveUpDownExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "updown_game";
        project.owner = "reel";

        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        variables.Add(MakeVariable(-102, "limit", REEL.PROJECT.DataType.NUMBER, "0"));
        variables.Add(MakeVariable(-103, "number", REEL.PROJECT.DataType.NUMBER, "0"));
        variables.Add(MakeVariable(-104, "answer", REEL.PROJECT.DataType.NUMBER, "0"));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeSay(1, -1, -1, "숫자 맞추는 게임을 시작하겠습니다. 1부터 무슨 숫자까지 할지 한계 값을 정해주세요.", 2));

        nodes.Add(MakeSpeechRec(2, 3));

        nodes.Add(MakeSet(3, REEL.PROJECT.DataType.NUMBER, 2, 0, null, "limit", 4));

        nodes.Add(MakeSay(4, -1, -1, "지금부터 게임을 시작할게요.", 7));

        nodes.Add(MakeGet(5, REEL.PROJECT.DataType.NUMBER, "limit"));

        nodes.Add(MakeRandom(6, 5, 0, null));

        nodes.Add(MakeSet(7, REEL.PROJECT.DataType.NUMBER, 6, 0, null, "number", 8));

        nodes.Add(MakeSay(8, -1, -1, "생각하신 숫자를 입력해주세요.", 14));

        nodes.Add(MakeIf(12, 11, 0, null, 13, 16));

        nodes.Add(MakeGet(9, REEL.PROJECT.DataType.NUMBER, "number"));

        nodes.Add(MakeGet(10, REEL.PROJECT.DataType.NUMBER, "answer"));

        nodes.Add(MakeEqual(11, 9, 0, "number", 10, 0, "answer"));

        nodes.Add(MakeSpeechRec(14, 15));

        nodes.Add(MakeSet(15, REEL.PROJECT.DataType.NUMBER, 14, 0, null, "answer", 12));

        nodes.Add(MakeGet(17, REEL.PROJECT.DataType.NUMBER, "number"));

        nodes.Add(MakeGet(18, REEL.PROJECT.DataType.NUMBER, "answer"));

        nodes.Add(MakeGreater(19, 17, 0, null, 18, 0, null));

        nodes.Add(MakeIf(16, 19, 0, null, 20, 21));

        nodes.Add(MakeSay(20, -1, -1, "더 큰 숫자를 입력해주세요.", 14));

        nodes.Add(MakeSay(21, -1, -1, "더 작은 숫자를 입력해주세요.", 14));

        nodes.Add(MakeSay(13, -1, -1, "축하합니다. 답을 맞추셨습니다. 게임을 종료하겠습니다.", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 1;
        process.priority = 7;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveRPSExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "Rock_Paper_Scissors";
        project.owner = "reel";

        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        variables.Add(MakeVariable(-101, "RPS", REEL.PROJECT.DataType.NUMBER, "3"));
        variables.Add(MakeVariable(-102, "User", REEL.PROJECT.DataType.NUMBER, "0"));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeSay(1, -1, -1, "저랑 가위바위보 해요", 2));
        nodes.Add(MakeSpeechRec(2, 3));

        REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[4];
        nexts[0] = new REEL.PROJECT.Next();
        nexts[0].value = "가위";
        nexts[0].next = 17;
        nexts[1] = new REEL.PROJECT.Next();
        nexts[1].value = "바위";
        nexts[1].next = 18;
        nexts[2] = new REEL.PROJECT.Next();
        nexts[2].value = "보";
        nexts[2].next = 19;
        nexts[3] = new REEL.PROJECT.Next();
        nexts[3].value = "DEFAULT";
        nexts[3].next = 2;
        nodes.Add(MakeMatch(3, REEL.PROJECT.DataType.STRING, 2, 0, null, nexts));

        nodes.Add(MakeGet(4, REEL.PROJECT.DataType.NUMBER, "RPS"));
        nodes.Add(MakeGet(29, REEL.PROJECT.DataType.NUMBER, "User"));
        nodes.Add(MakeRandom(5, 4, -1, null));

        nodes.Add(MakeSet(17, REEL.PROJECT.DataType.NUMBER, 0, 0, "1", "User", 6));      //사용자 가위
        nodes.Add(MakeSet(18, REEL.PROJECT.DataType.NUMBER, 0, 0, "2", "User", 7));      //사용자 바위
        nodes.Add(MakeSet(19, REEL.PROJECT.DataType.NUMBER, 0, 0, "3", "User", 8));      //사용자 보

        nodes.Add(MakeIf(6, 20, 0, null, 10, 7));      //사용자 가위 if
        nodes.Add(MakeIf(7, 21, 0, null, 11, 8));
        nodes.Add(MakeIf(8, 22, 0, null, 12, 13));

        nodes.Add(MakeIf(23, 20, 0, null, 12, 24));      //사용자 바위 if
        nodes.Add(MakeIf(24, 21, 0, null, 10, 25));
        nodes.Add(MakeIf(25, 22, 0, null, 11, 13));

        nodes.Add(MakeIf(26, 20, -1, null, 11, 27));      //사용자 보 if
        nodes.Add(MakeIf(27, 21, -1, null, 12, 28));
        nodes.Add(MakeIf(28, 22, -1, null, 10, 13));

        nodes.Add(MakeSay(10, -1, -1, "비겼어요.", 1));
        nodes.Add(MakeSay(11, -1, -1, "제가 이겼어요.", 1));
        nodes.Add(MakeSay(12, -1, -1, "제가 졌어요.", 1));
        nodes.Add(MakeSay(13, -1, -1, "가위, 바위, 보 중에서 하나를 골라주세요", 2));

        nodes.Add(MakeEqual(20, 5, 0, null, 29, 0, "1"));        //사용자 가위
        nodes.Add(MakeEqual(21, 5, 0, null, 29, 0, "2"));        //사용자 바위
        nodes.Add(MakeEqual(22, 5, 0, null, 29, 0, "3"));        //사용자 보

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 1;
        process.priority = 7;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void Save31GameExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "31game";
        project.owner = "woojin";

        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        variables.Add(MakeVariable(-100, "thirtyOne", REEL.PROJECT.DataType.NUMBER, "7"));
        variables.Add(MakeVariable(-101, "three", REEL.PROJECT.DataType.NUMBER, "3"));
        variables.Add(MakeVariable(-102, "one", REEL.PROJECT.DataType.NUMBER, "1"));
        variables.Add(MakeVariable(-103, "turn", REEL.PROJECT.DataType.NUMBER, "1"));
        variables.Add(MakeVariable(-104, "num", REEL.PROJECT.DataType.NUMBER, "1"));
        variables.Add(MakeVariable(-105, "user1", REEL.PROJECT.DataType.STRING, "user1"));
        variables.Add(MakeVariable(-106, "user2", REEL.PROJECT.DataType.STRING, "user2"));
        variables.Add(MakeVariable(-107, "total", REEL.PROJECT.DataType.NUMBER, "0"));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeSay(1, -1, -1, "안녕하세요. 제 이름은 모카에요. 지금부터 써리원게임을 같이 해보아요!" +
            " " + "두 분 이상이 계셔야 게임을 진행할 수 있어요. 현재 두 분 이상 계시나요?", 2));
        nodes.Add(MakeSpeechRec(2, 3));
        nodes.Add(MakeYesNo(3, 2, 0, null, 4, 5, 6));
        nodes.Add(MakeSay(5, -1, -1, "친구를 초대해 보아요!", 1));
        nodes.Add(MakeSay(6, -1, -1, "다시 말해주세요", 2));

        nodes.Add(MakeSay(4, -1, -1, "사용자1의 이름을 입력해주세요.", 7));
        nodes.Add(MakeSpeechRec(7, 9));
        nodes.Add(MakeSet(9, REEL.PROJECT.DataType.STRING, 7, 0, "user1", "user1", 10));
        nodes.Add(MakeSay(10, -1, -1, "사용자2의 이름을 입력해주세요.", 11));
        nodes.Add(MakeSpeechRec(11, 12));
        nodes.Add(MakeSet(12, REEL.PROJECT.DataType.STRING, 11, 0, "user2", "user2", 13));

        nodes.Add(MakeSay(13, -1, -1, "지금부터 게임을 시작하겠습니다.", 17));

        nodes.Add(MakeGet(14, REEL.PROJECT.DataType.NUMBER, "thirtyOne"));
        nodes.Add(MakeGet(15, REEL.PROJECT.DataType.NUMBER, "total"));
        nodes.Add(MakeGreater(16, 14, 0, null, 15, 0, null));

        nodes.Add(MakeWhile(17, 16, 0, null, 19, 60));

        nodes.Add(MakeGet(18, REEL.PROJECT.DataType.NUMBER, "turn"));

        REEL.PROJECT.Next[] nexts = new REEL.PROJECT.Next[3];
        nexts[0] = new REEL.PROJECT.Next();
        nexts[0].value = "1";
        nexts[0].next = 20;
        nexts[1] = new REEL.PROJECT.Next();
        nexts[1].value = "2";
        nexts[1].next = 30;
        nexts[2] = new REEL.PROJECT.Next();
        nexts[2].value = "3";
        nexts[2].next = 29;
        nodes.Add(MakeSwitch(19, REEL.PROJECT.DataType.NUMBER, 18, 0, null, nexts));

        nodes.Add(MakeSay(20, -1, -1, "이번엔 모카차례에요.", 23));
        nodes.Add(MakeGet(21, REEL.PROJECT.DataType.NUMBER, "one"));
        nodes.Add(MakeRandom(22, -1, -1, "3"));
        nodes.Add(MakeSet(23, REEL.PROJECT.DataType.NUMBER, 21, 0, null, "num", 24));
        nodes.Add(MakeGet(71, REEL.PROJECT.DataType.NUMBER, "num"));
        nodes.Add(MakeSay(24, 71, 0, null, 25));
        nodes.Add(MakeSay(25, -1, -1, "을 선택할거에요.", 38));

        //        nodes.Add(MakeGet(26, REEL.PROJECT.DataType.STRING, "user1"));
        nodes.Add(MakeStrCat(27, -105, 0, null, -1, -1, "님의 차례에요. 1에서 3사이의 숫자를 선택하세요."));

        nodes.Add(MakeGet(28, REEL.PROJECT.DataType.STRING, "user2"));
        nodes.Add(MakeSay(29, 28, 0, null, 30));

        nodes.Add(MakeSay(30, 27, 0, "", 31));
        nodes.Add(MakeSpeechRec(31, 32));
        nodes.Add(MakeSet(32, REEL.PROJECT.DataType.NUMBER, 31, 0, null, "num", 33));
        nodes.Add(MakeGet(72, REEL.PROJECT.DataType.NUMBER, "num"));
        nodes.Add(MakeSay(33, 72, 0, null, 34));
        nodes.Add(MakeSay(34, -1, -1, "을 선택했습니다.", 38));

        nodes.Add(MakeGet(35, REEL.PROJECT.DataType.NUMBER, "total"));
        nodes.Add(MakeGet(36, REEL.PROJECT.DataType.NUMBER, "num"));
        nodes.Add(MakeAdd(37, 35, 0, null, 36, 0, null));
        nodes.Add(MakeSet(38, REEL.PROJECT.DataType.NUMBER, 37, 0, null, "total", 39));
        nodes.Add(MakeGet(70, REEL.PROJECT.DataType.NUMBER, "total"));
        nodes.Add(MakeSay(39, 70, 0, null, 40));
        nodes.Add(MakeSay(40, -1, -1, "까지 진행됐습니다.", 44));

        nodes.Add(MakeGet(41, REEL.PROJECT.DataType.NUMBER, "turn"));
        nodes.Add(MakeAdd(42, 41, 0, null, -1, -1, "1"));
        nodes.Add(MakeGreaterEqual(43, 42, 0, null, -1, -1, "4"));
        nodes.Add(MakeIf(44, 43, 0, null, 45, 46));

        nodes.Add(MakeSet(45, REEL.PROJECT.DataType.NUMBER, -1, -1, "1", "turn", 17));
        nodes.Add(MakeSet(46, REEL.PROJECT.DataType.NUMBER, 42, 0, null, "turn", 17));


        //while끝났을 때
        REEL.PROJECT.Next[] nexts2 = new REEL.PROJECT.Next[3];
        nexts2[0] = new REEL.PROJECT.Next();
        nexts2[0].value = "1";
        nexts2[0].next = 47;
        nexts2[1] = new REEL.PROJECT.Next();
        nexts2[1].value = "2";
        nexts2[1].next = 48;
        nexts2[2] = new REEL.PROJECT.Next();
        nexts2[2].value = "3";
        nexts2[2].next = 49;
        nodes.Add(MakeSwitch(60, REEL.PROJECT.DataType.NUMBER, 41, 0, null, nexts2));

        nodes.Add(MakeSay(47, -1, -1, "모카가 졌어요...", 0));
        nodes.Add(MakeSay(48, 27, 0, null, 50));
        nodes.Add(MakeSay(49, 29, 0, null, 50));
        nodes.Add(MakeSay(50, -1, -1, "님이 졌어요.", -1));

        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 1;
        process.priority = 7;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();


        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));

    }

    public void SaveFacialMotionExample()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "facial_motion";
        project.owner = "reel";


        List<REEL.PROJECT.Node> variables = new List<REEL.PROJECT.Node>();
        List<REEL.PROJECT.Expression> expressions = new List<REEL.PROJECT.Expression>();
        REEL.PROJECT.Expression express = new REEL.PROJECT.Expression();
        express.tts = "안녕하세요. 저는 소셜로봇 모카입니다.";
        express.facial = "smile";
        express.motion = "hello";
        expressions.Add(express);
        express = new REEL.PROJECT.Expression();
        express.tts = "저는 사람들의 말을 듣고 행동할 수 있고, 사람들의 성별과 감정을 인식할 수 있습니다.";
        express.facial = "gazeup";
        express.motion = "clap";
        expressions.Add(express);
        express = new REEL.PROJECT.Expression();
        express.tts = "또한 사람의 나이도 알아볼 수 있도록 열심히 공부 중입니다.";
        express.facial = "happy";
        express.motion = "wrong";
        expressions.Add(express);
        express = new REEL.PROJECT.Expression();
        express.tts = "지금부터 저의 표정과 모션 동작을 보여드리겠습니다.";
        express.facial = "winkleft";
        express.motion = "nodLeft";
        expressions.Add(express);
        string exp = JsonConvert.SerializeObject(expressions);
        //Debug.Log(exp);
        variables.Add(MakeExpression(-100, "Greet", REEL.PROJECT.DataType.EXPRESSION, exp));
        project.variables = variables.ToArray();


        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));

        nodes.Add(MakeExpress(1, 2, 0, null, 3));
        nodes.Add(MakeGet(2, REEL.PROJECT.DataType.EXPRESSION, "Greet"));

        nodes.Add(MakeFacial(3, 0, 0, "normal", 4));
        nodes.Add(MakeSay(4, 0, 0, "보통의 표정입니다.", 5));
        nodes.Add(MakeFacial(5, 0, 0, "happy", 6));
        nodes.Add(MakeSay(6, 0, 0, "행복한 표정입니다.", 7));
        nodes.Add(MakeFacial(7, 0, 0, "angry", 8));
        nodes.Add(MakeSay(8, 0, 0, "화난 표정입니다.", 9));
        nodes.Add(MakeFacial(9, 0, 0, "fear", 10));
        nodes.Add(MakeSay(10, 0, 0, "두려운 표정입니다.", 11));
        nodes.Add(MakeFacial(11, 0, 0, "sad", 12));
        nodes.Add(MakeSay(12, 0, 0, "슬픈 표정입니다.", 13));
        nodes.Add(MakeFacial(13, 0, 0, "smile", 14));
        nodes.Add(MakeSay(14, 0, 0, "미소짓는 표정입니다.", 15));
        nodes.Add(MakeFacial(15, 0, 0, "speak", 16));
        nodes.Add(MakeSay(16, 0, 0, "말하는 표정입니다.", 17));
        nodes.Add(MakeFacial(17, 0, 0, "surprised", 18));
        nodes.Add(MakeSay(18, 0, 0, "놀라운 표정입니다.", 19));
        nodes.Add(MakeFacial(19, 0, 0, "winkleft", 20));
        nodes.Add(MakeSay(20, 0, 0, "왼쪽 윙크입니다.", 21));
        nodes.Add(MakeFacial(21, 0, 0, "winkright", 22));
        nodes.Add(MakeSay(22, 0, 0, "오른눈 윔크입니다.", 23));
        nodes.Add(MakeFacial(23, 0, 0, "gazeup", 24));
        nodes.Add(MakeSay(24, 0, 0, "위를 바라봅니다.", 25));
        nodes.Add(MakeFacial(25, 0, 0, "gazedown", 26));
        nodes.Add(MakeSay(26, 0, 0, "아래를 바라봅니다.", 27));
        nodes.Add(MakeFacial(27, 0, 0, "gazeleft", 28));
        nodes.Add(MakeSay(28, 0, 0, "왼쪽을 바라봅니다.", 29));
        nodes.Add(MakeFacial(29, 0, 0, "gazeright", 30));
        nodes.Add(MakeSay(30, 0, 0, "오른쪽을 바라봅니다.", 31));


        nodes.Add(MakeSay(31, 0, 0, "이번엔 모션 동작을 보여드리겠습니다.", 32));
        nodes.Add(MakeMotion(32, 0, 0, "hi", 33));
        nodes.Add(MakeSay(33, 0, 0, "안녕이라는 모션을 지금 하는 중입니다. 한 손으로 인사를 합니다.", 34));
        nodes.Add(MakeMotion(34, 0, 0, "hello", 35));
        nodes.Add(MakeSay(35, 0, 0, "안녕하세요라는 모션을 지금 하고 있습니다. 양 손으로 인사를 합니다.", 36));
        nodes.Add(MakeMotion(36, 0, 0, "angry", 37));
        nodes.Add(MakeSay(37, 0, 0, "화난 모션을 지금 하는 중입니다. 어때요? 정말 화가 났을 때 사용합니다", 38));
        nodes.Add(MakeMotion(38, 0, 0, "sad", 39));
        nodes.Add(MakeSay(39, 0, 0, "슬픈 모션을 지금 하는 중입니다. 슬픈 감정이 느껴지시나요?", 40));
        nodes.Add(MakeMotion(40, 0, 0, "ok", 41));
        nodes.Add(MakeSay(41, 0, 0, "오케이 또는 그래라는 모션을 지금 하는 중입니다. 긍정적인 표현을 할 때 사용합니다.", 42));
        nodes.Add(MakeMotion(42, 0, 0, "no", 43));
        nodes.Add(MakeSay(43, 0, 0, "노 아니라는 모션을 지금 하는 중입니다. 부정적인 표현을 할 때 사용합니다.", 44));
        nodes.Add(MakeMotion(44, 0, 0, "wrong", 45));
        nodes.Add(MakeSay(45, 0, 0, "틀렸을 때 하는 모션입니다. 문제 등을 틀렸을 떄 사용합니다.", 46));
        nodes.Add(MakeMotion(46, 0, 0, "clap", 47));
        nodes.Add(MakeSay(47, 0, 0, "박수를 치는 모션을 지금 하는 중입니다. 격려하거나 기분이 아주 좋을 때 사용합니다", 48));
        nodes.Add(MakeMotion(48, 0, 0, "happy", 49));
        nodes.Add(MakeSay(49, 0, 0, "행복한 모션을 지금 하는 중입니다. 기분이 좋을 때 하는 표현입니다", 50));
        nodes.Add(MakeMotion(50, 0, 0, "nodLeft", 51));
        nodes.Add(MakeSay(51, 0, 0, "왼쪽을 바라보는 모션을 지금 하는 중입니다. 왼쪽에 누군가 있거나 왼쪽을 가리킬 때 사용합니다", 52));
        nodes.Add(MakeMotion(52, 0, 0, "nodRight", 53));
        nodes.Add(MakeSay(53, 0, 0, "오른쪽을 바라보는 모션을 지금 하는 중입니다. 오른쪽에 누군가 있거나 오른쪽을 가리킬 때 사용합니다", 54));

        nodes.Add(MakeSay(54, 0, 0, "이것으로 표정과 모션 동작의 설명을 마치도록 하겠습니다. 감사합니다.", -1));


        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }

    public void SaveDoubleOutput()
    {
        REEL.PROJECT.ProjectData project = new REEL.PROJECT.ProjectData();
        project.title = "double_output";
        project.owner = "reel";

        project.variables = null;

        List<REEL.PROJECT.Process> processes = new List<REEL.PROJECT.Process>();
        List<REEL.PROJECT.Node> nodes = new List<REEL.PROJECT.Node>();
        nodes.Add(MakeStart(1));
//        nodes.Add(MakeAge(1, 2));
        nodes.Add(MakeSay(2, 3, 0, "", -1));
        nodes.Add(MakeStrCat(3, 1, 0, "", 1, 1, ""));
        REEL.PROJECT.Process process = new REEL.PROJECT.Process();
        process.id = 0;
        process.priority = 5;
        process.nodes = nodes.ToArray();
        processes.Add(process);
        project.processes = processes.ToArray();

        string jsonString = JsonUtility.ToJson(project, prettyPrint: true);

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + project.title + ".json";
#elif UNITY_ANDROID || UNITY_IOS
            string filepath = Application.persistentDataPath + project.title + ".json";
#else
            string filepath = Application.dataPath + project.title + ".json";
#endif

        using (StreamWriter streamWriter = File.CreateText(filepath))
        {
            streamWriter.Write(jsonString);
        }

        //        Debug.Log("Path: " + path);
        Debug.Log(JsonUtility.ToJson(project, prettyPrint: true));
    }



    public void Save(string filename)
    {
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("[Project::Start]");
        //SaveTestAIBlock();
        //SaveOxQuizExample();
        //SaveIntelligenceExample();
        //SaveExpressonExample();
        //SaveFunctionExample();
        //SaveIntroduceKoExample();
        //SaveIntroduceEnExample();
        //SaveMultiProcessExample();
        //SaveListOpExample();
        //SaveUpDownExample();
        //SaveRPSExample();
        //Save31GameExample();
        //SaveFacialMotionExample();
        //SaveDoubleOutput();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
