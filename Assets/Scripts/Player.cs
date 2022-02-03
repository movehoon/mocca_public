using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Networking;
using Newtonsoft.Json;
using REEL.PROJECT;
using REEL.D2EEditor;
using REEL.D2E;
using System.Linq;

public class Player : Singleton<Player>
{
    public GameObject processPrefab;
    public bool runOnSimulator = true;

    string tts_feedback = "";
    REEL.PROJECT.ProjectData projData = null;
    Dictionary<string, string> variables_bool = null;
    Dictionary<string, string> variables_number = null;
    Dictionary<string, string> variables_string = null;
    Dictionary<string, string> variables_facial = null;
    Dictionary<string, string> variables_motion = null;
    Dictionary<string, string> variables_mobility = null;
    Dictionary<string, List<string>> lists_bool = null;
    Dictionary<string, List<string>> lists_number = null;
    Dictionary<string, List<string>> lists_string = null;
    Dictionary<string, List<string>> lists_facial = null;
    Dictionary<string, List<string>> lists_motion = null;
    Dictionary<string, List<string>> lists_mobility = null;
    Dictionary<string, List<REEL.PROJECT.Expression>> expressions = null;

    // 작성자 : 장세윤
    // 중단점을 사용할 때 변수 값 확인을 위한 Getter.
    public Dictionary<string, string> Variables_bool { get { return variables_bool; } }
    public Dictionary<string, string> Variables_number { get { return variables_number; } }
    public Dictionary<string, string> Variables_string { get { return variables_string; } }
    public Dictionary<string, string> Variables_facial { get { return variables_facial; } }
    public Dictionary<string, string> Variables_motion { get { return variables_motion; } }
    public Dictionary<string, string> Variables_mobility { get { return variables_mobility; } }
    public Dictionary<string, List<string>> Lists_bool { get { return lists_bool; } }
    public Dictionary<string, List<string>> Lists_number { get { return lists_number; } }
    public Dictionary<string, List<string>> Lists_string { get { return lists_string; } }
    public Dictionary<string, List<string>> Lists_facial { get { return lists_facial; } }
    public Dictionary<string, List<string>> Lists_motion { get { return lists_motion; } }
    public Dictionary<string, List<string>> Lists_mobility { get { return lists_mobility; } }
    public Dictionary<string, List<Expression>> Expressions { get { return expressions; } }

    private Socket socket;
    private Stream stream;
    string yesno_result = "";
    public LocalizationManager.Language language = LocalizationManager.Language.DEFAULT;
    public string LocalVariableName(int func_id, string name) { return "__FUNCTION" + func_id.ToString() + "_LV_" + name; }
    public string DialogOutputName(int node_id) { return "__DIALOG" + node_id.ToString() + "_OUTPUT"; }

    public List<GameObject> goProcesses = new List<GameObject>();

    public FunctionData GetFunctionData(string functionName)
    {
        if (projData == null)
            return null;

        foreach (FunctionData functionData in projData.functions)
        {
            if (functionData.name == functionName)
            {
                return functionData;
            }
        }
        return null;
    }

    public void SetRecognizedSpeech(string speech)
    {
        if (!variables_string.ContainsKey("recognized_speech"))
        {
            variables_string.Add("recognized_speech", "");
        }
        variables_string["recognized_speech"] = speech;
    }

    public string GetRecognizedSpeech()
    {
        if (!variables_string.ContainsKey("recognized_speech"))
        {
            variables_string.Add("recognized_speech", "");
        }
        return variables_string["recognized_speech"];
    }

    public void ResetRecognizedSpeech()
    {
        if (!variables_string.ContainsKey("recognized_speech"))
        {
            variables_string.Add("recognized_speech", "");
        }
        variables_string["recognized_speech"] = "";
    }

    public String GetVariableName(int node_id)
    {
        foreach (REEL.PROJECT.Node node in projData.variables)
        {
            if (node.id == node_id)
            {
                return node.body.name;
            }
        }
        return "";
    }
    public REEL.PROJECT.DataType GetVariableType(string name)
    {
        if (variables_bool.ContainsKey(name))
            return REEL.PROJECT.DataType.BOOL;
        else if (variables_number.ContainsKey(name))
            return REEL.PROJECT.DataType.NUMBER;
        else if (variables_string.ContainsKey(name))
            return REEL.PROJECT.DataType.STRING;
        else if (variables_facial.ContainsKey(name))
            return REEL.PROJECT.DataType.FACIAL;
        else if (variables_motion.ContainsKey(name))
            return REEL.PROJECT.DataType.MOTION;
        else if (variables_mobility.ContainsKey(name))
            return REEL.PROJECT.DataType.MOBILITY;
        else if (lists_bool.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (lists_number.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (lists_string.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (lists_facial.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (lists_motion.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (lists_mobility.ContainsKey(name))
            return REEL.PROJECT.DataType.LIST;
        else if (expressions.ContainsKey(name))
            return REEL.PROJECT.DataType.EXPRESSION;
        else
            return REEL.PROJECT.DataType.NONE;
    }
    public REEL.PROJECT.DataType GetListType(string name)
    {
        if (lists_bool.ContainsKey(name))
            return REEL.PROJECT.DataType.BOOL;
        else if (lists_number.ContainsKey(name))
            return REEL.PROJECT.DataType.NUMBER;
        else if (lists_string.ContainsKey(name))
            return REEL.PROJECT.DataType.STRING;
        else if (lists_facial.ContainsKey(name))
            return REEL.PROJECT.DataType.FACIAL;
        else if (lists_motion.ContainsKey(name))
            return REEL.PROJECT.DataType.MOTION;
        else if (lists_mobility.ContainsKey(name))
            return REEL.PROJECT.DataType.MOBILITY;
        else
            return REEL.PROJECT.DataType.NONE;
    }
    public string GetVariable(string name)
    {
        if (variables_bool.ContainsKey(name))
            return variables_bool[name];
        else if (variables_number.ContainsKey(name))
            return variables_number[name];
        else if (variables_string.ContainsKey(name))
            return variables_string[name];
        else if (variables_facial.ContainsKey(name))
            return variables_facial[name];
        else if (variables_motion.ContainsKey(name))
            return variables_motion[name];
        else if (variables_mobility.ContainsKey(name))
            return variables_mobility[name];
        else if (lists_bool.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_bool[name]);
        else if (lists_number.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_number[name]);
        else if (lists_string.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_string[name]);
        else if (lists_facial.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_facial[name]);
        else if (lists_motion.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_motion[name]);
        else if (lists_mobility.ContainsKey(name))
            return JsonConvert.SerializeObject(lists_mobility[name]);
        else if (expressions.ContainsKey(name))
            return JsonConvert.SerializeObject(expressions[name]);
        return "";
    }

    public bool SetVariable(string name, string value)
    {
        if (variables_bool.ContainsKey(name))
        {
            variables_bool[name] = value;
            return true;
        }
        else if (variables_number.ContainsKey(name))
        {
            variables_number[name] = value;
            return true;
        }
        else if (variables_string.ContainsKey(name))
        {
            variables_string[name] = value;
            return true;
        }
        else if (variables_facial.ContainsKey(name))
        {
            variables_facial[name] = value;
            return true;
        }
        else if (variables_motion.ContainsKey(name))
        {
            variables_motion[name] = value;
            return true;
        }
        else if (variables_mobility.ContainsKey(name))
        {
            variables_mobility[name] = value;
            return true;
        }
        else if (lists_bool.ContainsKey(name))
        {
            lists_bool[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (lists_number.ContainsKey(name))
        {
            lists_number[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (lists_string.ContainsKey(name))
        {
            lists_string[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (lists_facial.ContainsKey(name))
        {
            lists_facial[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (lists_motion.ContainsKey(name))
        {
            lists_motion[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (lists_mobility.ContainsKey(name))
        {
            lists_mobility[name] = JsonConvert.DeserializeObject<List<string>>(value);
            return true;
        }
        else if (expressions.ContainsKey(name))
        {
            expressions[name] = JsonConvert.DeserializeObject<List<REEL.PROJECT.Expression>>(value);
            return true;
        }
        return false;
    }

    public void NewVariableBool(string name, string value)
    {
        variables_bool[name] = value;
    }
    public void NewVariableNumber(string name, string value)
    {
        variables_number[name] = value;
    }
    public void NewVariableString(string name, string value)
    {
        variables_string[name] = value;
    }
    public void NewVariablefacial(string name, string value)
    {
        variables_facial[name] = value;
    }
    public void NewVariableMotion(string name, string value)
    {
        variables_motion[name] = value;
    }
    public void NewVariableMobility(string name, string value)
    {
        variables_mobility[name] = value;
    }
    public void NewListBool(string name, List<string> value)
    {
        if (lists_bool.ContainsKey(name))
        {
            lists_bool.Remove(name);
        }
        List<string> list = new List<string>();
        for (int i = 0; i < value.Count; i++)
        {
            list.Add(value[i]);
        }
        lists_bool.Add(name, list);
    }
    public void NewListNumber(string name, List<string> value)
    {
        if (lists_number.ContainsKey(name))
        {
            lists_number.Remove(name);
        }
        List<string> list = new List<string>();
        for (int i = 0; i < value.Count; i++)
        {
            list.Add(value[i]);
        }
        lists_number.Add(name, list);
    }
    public void NewListString(string name, List<string> value)
    {
        if (lists_string.ContainsKey(name))
        {
            lists_string.Remove(name);
        }
        List<string> list = new List<string>();
        for (int i = 0; i < value.Count; i++)
        {
            list.Add(value[i]);
        }
        lists_string.Add(name, list);
    }

    public int GetCount(string name)
    {
        if (lists_bool.ContainsKey(name))
            return lists_bool[name].Count;
        else if (lists_number.ContainsKey(name))
            return lists_number[name].Count;
        else if (lists_string.ContainsKey(name))
            return lists_string[name].Count;
        else if (lists_facial.ContainsKey(name))
            return lists_facial[name].Count;
        else if (lists_motion.ContainsKey(name))
            return lists_motion[name].Count;
        else if (lists_mobility.ContainsKey(name))
            return lists_mobility[name].Count;
        else if (expressions.ContainsKey(name))
            return expressions[name].Count;
        else
            return 0;
    }
    public int GetCount(int node_id)
    {
        foreach (REEL.PROJECT.Node node in projData.variables)
        {
            if (node.id == node_id)
            {
                return GetCount(node.body.name);
            }
        }
        return 0;
    }
    public bool ExistElement(string name, string value)
    {
        if (lists_bool.ContainsKey(name))
        {
            if (value=="1" || value=="true" || value=="True")
            {
                if (lists_bool[name].Contains("1") || lists_bool[name].Contains("true") || lists_bool[name].Contains("True"))
                    return true;
            }
            else if (value=="0" || value=="false" || value=="False")
            {
                if (lists_bool[name].Contains("0") || lists_bool[name].Contains("false") || lists_bool[name].Contains("False"))
                    return true;
            }
            return false;
            //return lists_bool[name].Contains(value);
        }
        else if (lists_number.ContainsKey(name))
            return lists_number[name].Contains(value);
        else if (lists_string.ContainsKey(name))
            return lists_string[name].Contains(value);
        else if (lists_facial.ContainsKey(name))
            return lists_facial[name].Contains(value);
        else if (lists_motion.ContainsKey(name))
            return lists_motion[name].Contains(value);
        else if (lists_mobility.ContainsKey(name))
            return lists_mobility[name].Contains(value);
        else
            return false;
    }
    public string GetElement(string name, int index)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                return lists_bool[name][index];
            else if (lists_number.ContainsKey(name))
                return lists_number[name][index];
            else if (lists_string.ContainsKey(name))
                return lists_string[name][index];
            else if (lists_facial.ContainsKey(name))
                return lists_facial[name][index];
            else if (lists_motion.ContainsKey(name))
                return lists_motion[name][index];
            else if (lists_mobility.ContainsKey(name))
                return lists_mobility[name][index];
            return "";
        }
        catch
        {
            if (runOnSimulator)
            {
                LogWindow.Instance.PrintError("GetElement", "out of index");
            }
            return "";
        }
    }
    public string GetElement(int node_id, int index)
    {
        foreach (REEL.PROJECT.Node node in projData.variables)
        {
            if (node.id == node_id)
            {
                return GetElement(node.body.name, index);
            }
        }
        return "";
    }
    public string GetIndex(string name, string value)
    {
        if (lists_bool.ContainsKey(name))
        {
            for (int i = 0; i < lists_bool[name].Count; i++)
                if (lists_bool[name][i] == value)
                    return i.ToString();
        }
        else if (lists_number.ContainsKey(name))
        {
            for (int i = 0; i < lists_number[name].Count; i++)
                if (lists_number[name][i] == value)
                    return i.ToString();
        }
        else if (lists_string.ContainsKey(name))
        {
            for (int i = 0; i < lists_string[name].Count; i++)
                if (lists_string[name][i] == value)
                    return i.ToString();
        }
        else if (lists_facial.ContainsKey(name))
        {
            for (int i = 0; i < lists_facial[name].Count; i++)
                if (lists_facial[name][i] == value)
                    return i.ToString();
        }
        else if (lists_motion.ContainsKey(name))
        {
            for (int i = 0; i < lists_motion[name].Count; i++)
                if (lists_motion[name][i] == value)
                    return i.ToString();
        }
        else if (lists_mobility.ContainsKey(name))
        {
            for (int i = 0; i < lists_mobility[name].Count; i++)
                if (lists_mobility[name][i] == value)
                    return i.ToString();
        }
        return "-1";
    }
    public void InsertElement(string name, int index, string value)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name].Insert(index, value);
            else if (lists_number.ContainsKey(name))
                lists_number[name].Insert(index, value);
            else if (lists_string.ContainsKey(name))
                lists_string[name].Insert(index, value);
            else if (lists_facial.ContainsKey(name))
                lists_facial[name].Insert(index, value);
            else if (lists_motion.ContainsKey(name))
                lists_motion[name].Insert(index, value);
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name].Insert(index, value);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public void SetElement(string name, int index, string value)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name][index] = value;
            else if (lists_number.ContainsKey(name))
                lists_number[name][index] = value;
            else if (lists_string.ContainsKey(name))
                lists_string[name][index] = value;
            else if (lists_facial.ContainsKey(name))
                lists_facial[name][index] = value;
            else if (lists_motion.ContainsKey(name))
                lists_motion[name][index] = value;
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name][index] = value;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public void PushElement(string name, string value)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name].Add(value);
            else if (lists_number.ContainsKey(name))
                lists_number[name].Add(value);
            else if (lists_string.ContainsKey(name))
                lists_string[name].Add(value);
            else if (lists_facial.ContainsKey(name))
                lists_facial[name].Add(value);
            else if (lists_motion.ContainsKey(name))
                lists_motion[name].Add(value);
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name].Add(value);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public void RemoveIndex(string name, int index)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name].RemoveRange(index, 1);
            else if (lists_number.ContainsKey(name))
                lists_number[name].RemoveRange(index, 1);
            else if (lists_string.ContainsKey(name))
                lists_string[name].RemoveRange(index, 1);
            else if (lists_facial.ContainsKey(name))
                lists_facial[name].RemoveRange(index, 1);
            else if (lists_motion.ContainsKey(name))
                lists_motion[name].RemoveRange(index, 1);
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name].RemoveRange(index, 1);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public void RemoveElement(string name, string value)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name].Remove(value);
            else if (lists_number.ContainsKey(name))
                lists_number[name].Remove(value);
            else if (lists_string.ContainsKey(name))
                lists_string[name].Remove(value);
            else if (lists_facial.ContainsKey(name))
                lists_facial[name].Remove(value);
            else if (lists_motion.ContainsKey(name))
                lists_motion[name].Remove(value);
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name].Remove(value);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public void RemoveAll(string name)
    {
        try
        {
            if (lists_bool.ContainsKey(name))
                lists_bool[name].Clear();
            else if (lists_number.ContainsKey(name))
                lists_number[name].Clear();
            else if (lists_string.ContainsKey(name))
                lists_string[name].Clear();
            else if (lists_facial.ContainsKey(name))
                lists_facial[name].Clear();
            else if (lists_motion.ContainsKey(name))
                lists_motion[name].Clear();
            else if (lists_mobility.ContainsKey(name))
                lists_mobility[name].Clear();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    public bool IsList(string name)
    {
        if (lists_bool.ContainsKey(name))
            return true;
        else if (lists_number.ContainsKey(name))
            return true;
        else if (lists_string.ContainsKey(name))
            return true;
        else if (lists_facial.ContainsKey(name))
            return true;
        else if (lists_motion.ContainsKey(name))
            return true;
        else if (lists_mobility.ContainsKey(name))
            return true;
        return false;
    }

    public void GetTtsFeedback(string feedback)
    {
        //        Debug.Log("[GetTtsFeedback]" + feedback);
        tts_feedback = feedback;
        foreach (GameObject go in goProcesses)
        {
            go.GetComponent<Process>().SetTtsFeedback(feedback);
        }
    }

    public void SetExpressCmd(string cmd)
    {
        foreach (GameObject go in goProcesses)
        {
            go.GetComponent<Process>().SetExpressCmd(cmd);
        }
    }

    public void GetInputEvent(string input)
    {
//        Debug.Log("[Player::GetInputEvent]Get: " + input);
        SetRecognizedSpeech(input);
    }

    public string GetVariable(int node_id)
    {
        foreach (REEL.PROJECT.Node node in projData.variables)
        {
            if (node.id == node_id)
            {
                string value = GetVariable(node.body.name);
                return value;
            }
        }
        return "";
    }
    private bool _isPlaying = false;
    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
    }
    public void Stop()
    {
        Debug.Log("Stop");
        _isPlaying = false;
        // Stop Renderer
        foreach (GameObject go in goProcesses)
        {
            go.GetComponent<Process>().Stop();
            Destroy(go);
        }
        goProcesses.Clear();
        StopAllBehavior();
    }
    public void StopAllBehavior()
    {
        StopTts();
        StopFacial();
        StopMotion();
        StopMobility();
    }
    public void StopTts()
    {
        Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_TTS, "_STOP_");
//        GetTtsFeedback("done");
    }
    public void StopFacial()
    {
        Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_FACIAL, "_STOP_");
    }
    public void StopMotion()
    {
        Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_MOTION, "_STOP_");
    }
    public void StopMobility()
    {
        Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_MOBILITY, "_STOP_");
    }

    public void Pause(int pid = -1)
    {
        if (pid == -1)
        {
            // Pause all process
            foreach (GameObject go in goProcesses)
            {
                go.GetComponentInChildren<Process>().Pause();
            }
        }
        else
        {
            foreach (GameObject go in goProcesses)
            {
                Process process = go.GetComponentInChildren<Process>();
                if (pid == process.GetPID())
                {
                    process.Pause();
                }
            }
        }
    }
    public void Resume(int pid = -1)
    {
        if (pid == -1)
        {
            // Pause all process
            foreach (GameObject go in goProcesses)
            {
                go.GetComponentInChildren<Process>().Resume();
            }
        }
        else
        {
            foreach (GameObject go in goProcesses)
            {
                Process process = go.GetComponentInChildren<Process>();
                if (pid == process.GetPID())
                {
                    process.Resume();
                }
            }
        }
    }
    public void Play()
    {
        Debug.Log("Play");
        _isPlaying = true;

        if (runOnSimulator)
        {
            //MCWorkspaceManager.Instance.IsSimulation = true;
            MCPlayStateManager.Instance.IsSimulation = true;
        }

        //string path = "ox_quiz.json";
        //string path = "intelligence.json";
        //string path = "expression.json";
        //string path = "function.json";
        //string path = "introduce_ko.json";
        //string path = "introduce_en.json";
        //string path = "multi_process.json";
        string path = "list_operation.json";
        //string path = "updown_game.json";
        //string path = "Rock_Paper_Scissors.json";
        //string path = "31game.json";
        //string path = "facial_motion.json";
        //string path = "double_output.json";
        //string path = "test_AI.json";

#if UNITY_EDITOR
        string filepath = Application.dataPath + "/" + path;
#elif UNITY_ANDROID || UNITY_IOS
        Debug.Log("UNITY_IOS");
        string filepath = Application.persistentDataPath + "/" + path;
#else
        string filepath = Application.dataPath + "/" + path;
#endif
        StreamReader reader = new StreamReader(filepath);
        string body = reader.ReadToEnd();
        Debug.Log(body);
        reader.Close();

        Play(body);

    }

    public void CreateGlobalVariable(Node[] variables)
    {
        if (variables != null)
        {
            foreach (REEL.PROJECT.Node node in variables)
            {
                if (node.type == REEL.PROJECT.NodeType.VARIABLE)
                {
                    if (node.body.type == REEL.PROJECT.DataType.BOOL)
                    {
                        variables_bool.Add(node.body.name, node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.NUMBER)
                    {
                        variables_number.Add(node.body.name, node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.STRING)
                    {
                        variables_string.Add(node.body.name, node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.FACIAL)
                    {
                        variables_facial.Add(node.body.name, node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOTION)
                    {
                        variables_motion.Add(node.body.name, node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOBILITY)
                    {
                        variables_mobility.Add(node.body.name, node.body.value);
                    }
                }
                else if (node.type == REEL.PROJECT.NodeType.LIST)
                {
                    if (node.body.type == REEL.PROJECT.DataType.BOOL)
                    {
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        List<string> list = new List<string>(listValue.listValue);
                        lists_bool.Add(node.body.name, list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.NUMBER)
                    {
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        List<string> list = new List<string>(listValue.listValue);
                        lists_number.Add(node.body.name, list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.STRING)
                    {
                        //Debug.Log("Src: " + node.body.value);
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //Debug.Log("listValue: " + listValue.ToString());
                        List<string> list = new List<string>(listValue.listValue);
                        //Debug.Log("list: " + list.ToString());
                        lists_string.Add(node.body.name, list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.FACIAL)
                    {
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        List<string> list = new List<string>(listValue.listValue);
                        lists_facial.Add(node.body.name, list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOTION)
                    {
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        List<string> list = new List<string>(listValue.listValue);
                        lists_motion.Add(node.body.name, list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOBILITY)
                    {
                        REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        List<string> list = new List<string>(listValue.listValue);
                        lists_mobility.Add(node.body.name, list);
                    }
                }
                else if (node.type == REEL.PROJECT.NodeType.EXPRESSION)
                {
                    List<REEL.PROJECT.Expression> expression = JsonConvert.DeserializeObject<List<REEL.PROJECT.Expression>>(node.body.value);
                    expressions.Add(node.body.name, expression);
                }
            }
        }
    }

    public void CreateLocalVariables(int function_id, int start_id)
    {
        Debug.Log("CreateLocalVariables from " + start_id);
        if (projData != null)
        {
            foreach (FunctionData functionData in projData.functions)
            {
//                Debug.Log("Check " + functionData.startID);
                if (functionData.startID == start_id)
                {
                    Debug.Log("Found Function");
                    if (functionData.variables != null)
                    {
                        _CreateLocalVariables(function_id, functionData.variables);
                        foreach (Node node in functionData.variables)
                        {
                            Debug.Log("Make Local variable " + node.body.name + " to " + node.body.value);
                        }
                    }
                }
            }

        }
    }

    void _CreateLocalVariables(int fid, Node[] variables)
    {
        if (variables != null)
        {
            foreach (REEL.PROJECT.Node node in variables)
            {
                string localVariableName = LocalVariableName(fid, node.body.name);

                if (node.type == REEL.PROJECT.NodeType.VARIABLE)
                {
                    if (node.body.type == REEL.PROJECT.DataType.BOOL)
                    {
                        if (Variables_bool.ContainsKey(localVariableName) == false)
                        {
                            variables_bool.Add(localVariableName, node.body.value);
                        }
                        
                        //variables_bool.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.NUMBER)
                    {
                        if (variables_number.ContainsKey(localVariableName) == false)
                        {
                            variables_number.Add(localVariableName, node.body.value);
                        }
                        //variables_number.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.STRING)
                    {
                        if (variables_string.ContainsKey(localVariableName) == false)
                        {
                            variables_string.Add(localVariableName, node.body.value);
                        }
                        //variables_string.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.FACIAL)
                    {
                        if (variables_facial.ContainsKey(localVariableName) == false)
                        {
                            variables_facial.Add(localVariableName, node.body.value);
                        }
                        //variables_facial.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOTION)
                    {
                        if (variables_motion.ContainsKey(localVariableName) == false)
                        {
                            variables_motion.Add(localVariableName, node.body.value);
                        }
                        //variables_motion.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOBILITY)
                    {
                        if (variables_mobility.ContainsKey(localVariableName) == false)
                        {
                            variables_mobility.Add(localVariableName, node.body.value);
                        }
                        //variables_mobility.Add(LocalVariableName(fid, node.body.name), node.body.value);
                    }
                }
                else if (node.type == REEL.PROJECT.NodeType.LIST)
                {
                    if (node.body.type == REEL.PROJECT.DataType.BOOL)
                    {
                        if (lists_bool.ContainsKey(localVariableName) == false)
                        {
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                List<string> list = new List<string>(listValue.listValue);
                                lists_bool.Add(localVariableName, list);
                            }
                        }

                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //List<string> list = new List<string>(listValue.listValue);
                        //lists_bool.Add(LocalVariableName(fid, node.body.name), list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.NUMBER)
                    {
                        if (lists_number.ContainsKey(localVariableName) == false)
                        {
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                List<string> list = new List<string>(listValue.listValue);
                                lists_number.Add(localVariableName, list);
                            }
                        }
                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //List<string> list = new List<string>(listValue.listValue);
                        //lists_number.Add(LocalVariableName(fid, node.body.name), list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.STRING)
                    {
                        if (lists_string.ContainsKey(localVariableName) == false)
                        {
                            //Debug.Log("Src: " + node.body.value);
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                //Debug.Log("listValue: " + listValue.ToString());
                                List<string> list = new List<string>(listValue.listValue);
                                //Debug.Log("list: " + list.ToString());
                                lists_string.Add(localVariableName, list);
                            }
                        }
                        ////Debug.Log("Src: " + node.body.value);
                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        ////Debug.Log("listValue: " + listValue.ToString());
                        //List<string> list = new List<string>(listValue.listValue);
                        ////Debug.Log("list: " + list.ToString());
                        //lists_string.Add(LocalVariableName(fid, node.body.name), list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.FACIAL)
                    {
                        if (lists_facial.ContainsKey(localVariableName) == false)
                        {
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                List<string> list = new List<string>(listValue.listValue);
                                lists_facial.Add(localVariableName, list);
                            }
                        }

                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //List<string> list = new List<string>(listValue.listValue);
                        //lists_facial.Add(LocalVariableName(fid, node.body.name), list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOTION)
                    {
                        if (lists_motion.ContainsKey(localVariableName) == false)
                        {
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                List<string> list = new List<string>(listValue.listValue);
                                lists_motion.Add(localVariableName, list);
                            }
                        }
                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //List<string> list = new List<string>(listValue.listValue);
                        //lists_motion.Add(LocalVariableName(fid, node.body.name), list);
                    }
                    else if (node.body.type == REEL.PROJECT.DataType.MOBILITY)
                    {
                        if (lists_mobility.ContainsKey(localVariableName) == false)
                        {
                            using (ListValue listValue = JsonUtility.FromJson<ListValue>(node.body.value))
                            {
                                List<string> list = new List<string>(listValue.listValue);
                                lists_mobility.Add(localVariableName, list);
                            }
                        }
                        //REEL.PROJECT.ListValue listValue = JsonUtility.FromJson<REEL.PROJECT.ListValue>(node.body.value);
                        //List<string> list = new List<string>(listValue.listValue);
                        //lists_mobility.Add(LocalVariableName(fid, node.body.name), list);
                    }
                }
                else if (node.type == REEL.PROJECT.NodeType.EXPRESSION)
                {
                    if (expressions.ContainsKey(localVariableName) == false)
                    {
                        List<Expression> expression = JsonConvert.DeserializeObject<List<Expression>>(node.body.value);
                        expressions.Add(localVariableName, expression);
                    }
                    //List<REEL.PROJECT.Expression> expression = JsonConvert.DeserializeObject<List<REEL.PROJECT.Expression>>(node.body.value);
                    //expressions.Add(LocalVariableName(fid, node.body.name), expression);
                }
            }
        }
    }


    public int GetBlockCountInProject(string jsonString)
    {
        ProjectData projData = JsonUtility.FromJson<REEL.PROJECT.ProjectData>(jsonString);
        int nBlock = projData.variables.Length;
        foreach (REEL.PROJECT.Process p in projData.processes)
        {
            nBlock += p.nodes.Length;
        }
        Debug.Log("Total block usage: " + nBlock);
        return nBlock;
    }

    public void Play(string jsonString, int nodeId = -1)
    {
        Debug.Log("Play~~~");
        _isPlaying = true;

        //----------------------------------------
        // 프로젝트 실행 튜토리얼 이벤트 - kjh
        //----------------------------------------
        TutorialManager.SendEvent(Tutorial.CustomEvent.ProjectPlayed);

        projData = JsonUtility.FromJson<REEL.PROJECT.ProjectData>(jsonString);
        Debug.Log(jsonString);
        Debug.Log("projData.language: " + projData.language);
//        projData.language = LocalizationManager.Language.ENG;

        Debug.Log("Total block usage: " + GetBlockCountInProject(jsonString));

        // Check content language
        language = LocalizationManager.Language.DEFAULT;
        if (projData.language >= 0)
        {
            language = projData.language;
        }
        Debug.Log("projData.language: " + projData.language);

        // 1. Check Variables
        variables_bool = new Dictionary<string, string>();
        variables_number = new Dictionary<string, string>();
        variables_string = new Dictionary<string, string>();
        variables_facial = new Dictionary<string, string>();
        variables_motion = new Dictionary<string, string>();
        variables_mobility = new Dictionary<string, string>();
        lists_bool = new Dictionary<string, List<string>>();
        lists_number = new Dictionary<string, List<string>>();
        lists_string = new Dictionary<string, List<string>>();
        lists_facial = new Dictionary<string, List<string>>();
        lists_motion = new Dictionary<string, List<string>>();
        lists_mobility = new Dictionary<string, List<string>>();
        expressions = new Dictionary<string, List<REEL.PROJECT.Expression>>();
        CreateGlobalVariable(projData.variables);


        foreach (GameObject go in goProcesses)
        {
            Destroy(go);
        }
        goProcesses.Clear();
        if (_isPlaying)
        {
            foreach (REEL.PROJECT.Process process in projData.processes)
            {
                if (process.id == 0)
                {
                    continue;
                }

                GameObject go = (GameObject)Instantiate(processPrefab, transform);
                go.SetActive(true);
                //if (projData.nodes != null)
                //{
                //    Node[] nodes = new Node[process.nodes.Length + projData.nodes.Length];
                //    process.nodes.CopyTo(nodes, 0);
                //    projData.nodes.CopyTo(nodes, process.nodes.Length);
                //    process.nodes = nodes;
                //}
                if (projData.functions != null)
                {
                    int length = 0;
                    foreach (REEL.PROJECT.FunctionData function in projData.functions)
                    {
                        length += function.nodes.Length;
                    }
                    Node[] nodes = new Node[process.nodes.Length + length];
                    process.nodes.CopyTo(nodes, 0);
                    int offset = process.nodes.Length;
                    foreach (REEL.PROJECT.FunctionData function in projData.functions)
                    {
                        function.nodes.CopyTo(nodes, offset);
                        offset += function.nodes.Length;
                    }
                    //projData.nodes.CopyTo(nodes, process.nodes.Length);
                    process.nodes = nodes;
                }
                goProcesses.Add(go);
                go.GetComponent<Process>().Play(process, nodeId);
            }
        }
    }


	//------------------------------------------------------------------
	//
	// MOCCA_Head 씬파일에서 FireBase 데이타 사용을 위한 함수들 - kjh
	//
	//------------------------------------------------------------------
	static string LAST_LOADED_LOGIC_DATA = string.Empty;

	public void LoadProject(ProjectDesc curSelectedDesc, string logicData)
	{
		LAST_LOADED_LOGIC_DATA = logicData;
        if (!runOnSimulator)
        {
            Play(LAST_LOADED_LOGIC_DATA);
        }
    }

	public void PlayLastLoadedProject()
	{
		if( string.IsNullOrWhiteSpace(LAST_LOADED_LOGIC_DATA) )
		{
			MessageBox.Show("프로젝트를 먼저 불러오세요."); // local 추가 완료
			return;
		}

		Play(LAST_LOADED_LOGIC_DATA);
	}

}
