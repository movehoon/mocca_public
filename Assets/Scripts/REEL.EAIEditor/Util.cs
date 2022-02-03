using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public static class Util
    {
        private static readonly int variableIDForDeclaration = -2;

        public static bool CompareTwoStrings(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.CurrentCultureIgnoreCase);
            //return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static float GetAngleBetween(Vector2 start, Vector2 end)
        {
            Vector2 target = end - start;
            return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }

        public static float GetDistanceBetween(Vector2 start, Vector2 end)
        {
            Vector2 target = end - start;
            return target.magnitude;
        }

        public static T ReadFromJson<T>(TextAsset jsonText)
        {
            return SimpleJson.SimpleJson.DeserializeObject<T>(jsonText.text);
        }

        public static string RemoveAllWhiteSpace(string targetString)
        {
            return targetString.Replace(" ", string.Empty);
        }

        // 두 직선이 서로 교차하는지 확인하는 메소드.
        public static bool CheckLineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float ab = CCW(a, b, c) * CCW(a, b, d);
            float cd = CCW(c, d, a) * CCW(c, d, b);

            return ab <= 0f && cd <= 0f;
        }

        // 직선이 드래그 영역에 포함되는지 확인.
        //public static bool CheckLineIncluded(GraphLine.LinePoint linePoint, DragInfo dragInfo)
        //{
        //    if (linePoint.start.x > dragInfo.topLeft.x && linePoint.start.x < dragInfo.topLeft.x + dragInfo.width &&
        //        linePoint.end.x > dragInfo.topLeft.x && linePoint.end.x < dragInfo.topLeft.x + dragInfo.width &&
        //        linePoint.start.y < dragInfo.topLeft.y && linePoint.start.y > dragInfo.topLeft.y - dragInfo.height &&
        //        linePoint.end.y < dragInfo.topLeft.y && linePoint.end.y > dragInfo.topLeft.y - dragInfo.height)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        private static float CCW(Vector2 a, Vector2 b)
        {
            return a.Cross(b);
        }

        private static float CCW(Vector2 p, Vector2 a, Vector2 b)
        {
            return CCW(a - p, b - p);
        }

        // Vector2 확장 메소드 (외적 계산).
        public static float Cross(this Vector2 myVector, Vector2 otherVector)
        {
            return myVector.x * otherVector.y - myVector.y * otherVector.x;
        }

        // UI RayCaster.
        public static bool GetRaycastResult(BaseRaycaster raycaster, EventSystem eventSystem, Vector2 mousePosition, out RaycastResult result)
        {
            result = new RaycastResult();
            PointerEventData data = new PointerEventData(eventSystem);
            data.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(data, results);

            if (results.Count > 0)
            {
                result = results[0];
                return true;
            }

            else return false;
        }

        public static void XMLSerialize<T>(T node, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, node);
            writer.Close();
        }

        public static void XMLSerialize<T>(T node, string projectPath, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter writer = new StreamWriter(projectPath + "/" + fileName);
            serializer.Serialize(writer, node);
            writer.Close();
        }

        public static T XMLDeserialize<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(path);
            T xmlObject = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return xmlObject;
        }

        // 
        //public static void CompressFolderToZip(string folderPath, string outputPath)
        //{
        //    try
        //    {
        //        FileStream fsOut = File.Create(outputPath);
        //        using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
        //        {
        //            zipStream.SetLevel(3);
        //            ZipConstants.DefaultCodePage = 0;
        //            zipStream.UseZip64 = UseZip64.Off;
        //            int folderOffset = folderPath.Length + (folderPath.EndsWith("\\") ? 0 : 1);
        //            CompressFolder(folderPath, zipStream, folderOffset);

        //            zipStream.IsStreamOwner = true;

        //            zipStream.Finish();
        //            zipStream.Flush();
        //            zipStream.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Log("Exception Zip: " + ex);
        //    }
        //}

        //private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        //{
        //    string[] files = Directory.GetFiles(path);

        //    foreach (string filename in files)
        //    {
        //        // Check whether meta file ornor.
        //        string extension = Path.GetExtension(filename);
        //        if (IsMetaFile(extension))
        //            continue;

        //        FileInfo fi = new FileInfo(filename);

        //        string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
        //        entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
        //        ZipEntry newEntry = new ZipEntry(entryName);
        //        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
        //        newEntry.Size = fi.Length;

        //        zipStream.PutNextEntry(newEntry);

        //        // Zip the file in buffered chunks
        //        // the "using" will close the stream even if an exception occurs
        //        byte[] buffer = new byte[4096];
        //        using (FileStream streamReader = File.OpenRead(filename))
        //        {
        //            StreamUtils.Copy(streamReader, zipStream, buffer);
        //        }
        //        //zipStream.CloseEntry();
        //    }

        //    //string[] folders = Directory.GetDirectories(path);
        //    //foreach (string folder in folders)
        //    //{
        //    //    CompressFolder(folder, zipStream, folderOffset);
        //    //}
        //}

        //private static bool IsMetaFile(string fileExtension)
        //{
        //    return fileExtension.Equals(".meta");
        //}

        //public static ProjectFormat GetSaveFormat(List<GraphItem> locatedItemList, List<GraphLine> locatedLineList, List<LeftMenuVariableItem> variableList, string projectName = "")
        //{
        //    //foreach (var item in variableList)
        //    //{
        //    //    Debug.Log(item);
        //    //}

        //    ProjectFormat project = GetSaveFormat(locatedItemList, locatedLineList, projectName);
        //    project.variableArray = new VariableArray();
        //    for (int ix = 0; ix < variableList.Count; ++ix)
        //    {
        //        VariableFormat variable = new VariableFormat();
        //        variable.name = variableList[ix].name;
        //        variable.type = variableList[ix].dataType.ToString();
        //        variable.value = variableList[ix].value;

        //        project.variableArray.Add(variable);
        //    }

        //    return project;
        //}

        //public static int GetMaxIndexOfNodes(List<GraphItem> locatedItemList)
        //{
        //    int maxIndex = 0;

        //    foreach (var item in locatedItemList)
        //    {
        //        if (maxIndex > item.BlockID)
        //            maxIndex = item.BlockID;
        //    }

        //    return maxIndex;
        //}

        //public static ProjectFormat GetSaveFormat(List<GraphItem> locatedItemList, List<GraphLine> locatedLineList, string projectName = "")
        //{
        //    ProjectFormat project = new ProjectFormat();
        //    project.projectName = projectName;

        //    project.blockArray = new NodeBlockArray();
        //    for (int ix = 0; ix < locatedItemList.Count; ++ix)
        //    {
        //        NodeBlock block = new NodeBlock();
        //        //block.nodeType = locatedItemList[ix].GetNodeType;
        //        block.id = locatedItemList[ix].BlockID;
        //        block.title = locatedItemList[ix].GetBlockTitle;
        //        block.value = locatedItemList[ix].GetItemData() as string;
        //        //block.position = locatedItemList[ix].GetComponent<RectTransform>().position;
        //        block.position = locatedItemList[ix].GetComponent<RectTransform>().anchoredPosition;

        //        //NodeType nodeType = locatedItemList[ix].GetNodeType;

        //        //if (nodeType == NodeType.SWITCH)
        //        //{
        //        //    SwitchBlockItem switchNode = locatedItemList[ix] as SwitchBlockItem;
        //        //    block.switchBlockCount = switchNode.GetBlockCount;
        //        //    //Debug.Log("switchNode.GetBlockCount: " + switchNode.GetBlockCount + " , block.switchBlockCount: " + block.switchBlockCount);
        //        //    block.switchBranchType = switchNode.GetSwitchType;
        //        //    block.name = switchNode.GetSwitchName;
        //        //    for (int jx = 0; jx < switchNode.GetBlockCount; ++jx)
        //        //    {
        //        //        ExecuteCasePoint casePoint = switchNode.executePoints[jx + 1] as ExecuteCasePoint;
        //        //        block.switchBlockValues.Add(casePoint.GetCaseValue);
        //        //    }
        //        //}

        //        //else if (nodeType == NodeType.Emotions)
        //        //{
        //        //    EmotionRecognitionItem emotionNode = locatedItemList[ix] as EmotionRecognitionItem;
        //        //    block.switchBlockCount = emotionNode.GetBlockCount;
        //        //    for (int jx = 0; jx < emotionNode.GetBlockCount - 1; ++jx)
        //        //    {
        //        //        ExecuteCasePoint casePoint = emotionNode.executePoints[jx + 1] as ExecuteCasePoint;
        //        //        block.switchBlockValues.Add(casePoint.GetCaseValue);
        //        //    }
        //        //}

        //        //else if (nodeType == NodeType.API)
        //        //{
        //        //    APIBlockItem apiNode = locatedItemList[ix] as APIBlockItem;
        //        //    block.switchBlockCount = apiNode.GetBlockCount;
        //        //    block.requestURL = apiNode.GetRequestURL;
        //        //    block.apiBranchType = apiNode.GetAPIType;
        //        //    block.name = apiNode.GetAPIName;
        //        //    for (int jx = 0; jx < apiNode.GetBlockCount; ++jx)
        //        //    {
        //        //        ExecuteCasePoint casePoint = apiNode.executePoints[jx + 1] as ExecuteCasePoint;
        //        //        block.switchBlockValues.Add(casePoint.GetCaseValue);
        //        //    }
        //        //}

        //        //else if (nodeType == NodeType.VARIABLE)
        //        //{
        //        //    VariableItem variableNode = locatedItemList[ix] as VariableItem;
        //        //    block.variableOperator = variableNode.GetOperatorType.ToString();
        //        //    block.name = variableNode.GetBlockName;
        //        //}

        //        //else if (nodeType == NodeType.IF)
        //        //{
        //        //    IFBranchItem ifNode = locatedItemList[ix] as IFBranchItem;
        //        //    BranchCondition data = ifNode.GetIFBranchData();
        //        //    block.title = ifNode.GetBlockName;
        //        //    block.name = ifNode.GetVariableName;
        //        //    block.value = data.rParameter;
        //        //    block.ifBranchType = data.lParamType;
        //        //    block.variableOperator = ifNode.GetStringFromOpType(data.opParameter);
        //        //}

        //        //else if (nodeType == NodeType.EXPRESSION)
        //        //{
        //        //    ExpressionBlockItem expNode = locatedItemList[ix] as ExpressionBlockItem;
        //        //    List<ExpressionTTS> ttsDataArray = expNode.GetExpressionTTSData;
        //        //    for (int jx = 0; jx < ttsDataArray.Count; ++jx)
        //        //    {
        //        //        block.expressTTSValues.Add(ttsDataArray[jx]);
        //        //    }
        //        //}

        //        project.BlockAdd(block);
        //    }

        //    project.lineArray = new LineBlockArray();
        //    for (int ix = 0; ix < locatedLineList.Count; ++ix)
        //    {
        //        int leftBlockID = locatedLineList[ix].GetLeftExecutePointInfo.blockID;
        //        int leftExecutePointID = locatedLineList[ix].GetLeftExecutePointInfo.executePointID;
        //        int rightBlockID = locatedLineList[ix].GetRightExecutePointInfo.blockID;

        //        LineBlock line = new LineBlock(leftBlockID, leftExecutePointID, rightBlockID);
        //        project.LineAdd(line);
        //    }

        //    return project;
        //}

        //public static void CompileToXML(ProjectFormat projectFormat, List<GraphItem> locatedItemList, List<LeftMenuVariableItem> variableList, IComparer<GraphItem> comparer)
        //{
        //    XMLProject project = new XMLProject();

        //    //if (locatedItemList.Count < 2)
        //    //{
        //    //    CompileToXML(projectFormat, locatedItemList, comparer, project);
        //    //    return;
        //    //}

        //    //int variableID = GetMaxIndexOfNodes(locatedItemList) + 1;
        //    for (int ix = 0; ix < variableList.Count; ++ix)
        //    {
        //        LeftMenuVariableItem variable = variableList[ix];

        //        XMLVariableNode xmlNode = new XMLVariableNode();
        //        xmlNode.nodeID = variableIDForDeclaration;
        //        xmlNode.nodeName = variable.name;
        //        //xmlNode.nodeType = NodeType.VARIABLE;
        //        xmlNode.nodeValue = variable.value;
        //        xmlNode.operatorType = XMLVariableOperatorType.set;

        //        //xmlNode.nextID = (ix < variableList.Count - 1) ? variableIDForDeclaration : locatedItemList[0].BlockID;
        //        xmlNode.nextID = 0;             // 변수 선언용 XML에는 의미 없음.
        //        project.AddNode(xmlNode);
        //    }

        //    CompileToXML(projectFormat, locatedItemList, comparer, project);
        //}

        //public static void CompileToXML(ProjectFormat projectFormat, List<GraphItem> locatedItemList, IComparer<GraphItem> comparer, XMLProject project = null)
        //{
        //    locatedItemList.Sort(comparer);

        //    if (project == null)
        //        project = new XMLProject();

        //    //XMLNode entryNode = locatedItemList[0].GetXMLNormalData();
        //    //if (project.nodes.Count > 0)
        //    //{
        //    //    XMLVariableNode firstVariable = (XMLVariableNode)project.nodes[0];
        //    //    entryNode.nextID = firstVariable.nodeID;
        //    //}

        //    //project.AddNodeToFirst(entryNode);

        //    //foreach (GraphItem node in locatedItemList)
        //    for (int ix = 0; ix < locatedItemList.Count; ++ix)
        //    {
        //        GraphItem node = locatedItemList[ix];

        //        //if (node.GetNodeType == NodeType.SWITCH)
        //        //{
        //        //    SwitchBlockItem switchNode = node as SwitchBlockItem;
        //        //    project.AddNode(switchNode.GetXMLSwitchData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.VARIABLE)
        //        //{
        //        //    VariableItem variableNode = node as VariableItem;
        //        //    project.AddNode(variableNode.GetXMLVariableData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.IF)
        //        //{
        //        //    IFBranchItem ifNode = node as IFBranchItem;
        //        //    project.AddNode(ifNode.GetXMLIFData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.YESNO)
        //        //{
        //        //    YESNOBranchItem yesNoNode = node as YESNOBranchItem;
        //        //    project.AddNode(yesNoNode.GetXMLYesNoSwitchData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.EXPRESSION)
        //        //{
        //        //    ExpressionBlockItem expNode = node as ExpressionBlockItem;
        //        //    project.AddNode(expNode.GetXMLExpressionData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.API)
        //        //{
        //        //    APIBlockItem apiNode = node as APIBlockItem;
        //        //    project.AddNode(apiNode.GetXMLAPIData());
        //        //}
        //        //else if (node.GetNodeType == NodeType.MOTION)
        //        //{
        //        //    int index = int.Parse(node.GetItemData().ToString());
        //        //    node.SetItemData(Constants.ParseMotionKoreanToEnglish(index));
        //        //    project.AddNode(node.GetXMLNormalData());

        //        //    node.SetItemData(index.ToString());
        //        //}
        //        //else if (node.GetNodeType == NodeType.FACIAL)
        //        //{
        //        //    int index = int.Parse(node.GetItemData().ToString());
        //        //    node.SetItemData(Constants.ParseFacialKoreanToEnglish(index));
        //        //    project.AddNode(node.GetXMLNormalData());

        //        //    node.SetItemData(index.ToString());
        //        //}
        //        //else if (node.GetNodeType == NodeType.MOBILITY)
        //        //{
        //        //    int index = int.Parse(node.GetItemData().ToString());
        //        //    node.SetItemData(((Constants.MobilityType)(index)).ToString());
        //        //    project.AddNode(node.GetXMLNormalData());

        //        //    node.SetItemData(index.ToString());
        //        //}

        //        //else if (node.GetNodeType == NodeType.Emotions)
        //        //{
        //        //    EmotionRecognitionItem yesNoNode = node as EmotionRecognitionItem;
        //        //    project.AddNode(yesNoNode.GetXMLEmotionData());
        //        //}

        //        //else if (node.GetNodeType == NodeType.Gender)
        //        //{
        //        //    GenderBranchItem yesNoNode = node as GenderBranchItem;
        //        //    project.AddNode(yesNoNode.GetXMLGenderSwitchNode());
        //        //}

        //        //else
        //        //{
        //        //    project.AddNode(node.GetXMLNormalData());
        //        //}

        //        project.AddNode(node.GetXMLNormalData());
        //    }

        //    string projectName = PlayerPrefs.GetString(D2EConstants.currentProjectKey);
        //    string projectPath = D2EConstants.BaseProjectPath + projectFormat.projectName;
        //    XMLSerialize<XMLProject>(project, projectPath, projectName + ".xml");

        //    string zipOutputPath = Application.dataPath + "/" + projectName + ".zip";
        //    CompressFolderToZip(projectPath, zipOutputPath);
        //    //XMLSerialize<XMLProject>(project, Application.dataPath + "/Data/" + projectName + ".xml");
        //    //XMLSerialize<XMLProject>(project, Application.dataPath + "/Data/TestProject.xml");
        //}

        //public static void CheckDataDirectory()
        //{
        //    string dataPath = Application.dataPath + "/Data";
        //    if (!Directory.Exists(dataPath))
        //        Directory.CreateDirectory(dataPath);
        //}

        //public static string GetUTF8String(string input)
        //{
        //    byte[] bytes = Encoding.Default.GetBytes(input);
        //    return Encoding.UTF8.GetString(bytes);
        //}
    }
}