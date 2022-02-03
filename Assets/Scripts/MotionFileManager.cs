using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MotionFileManager : MonoBehaviour
{
    public FirebaseManager firebasManager;

    string MOTION_PATH;

    private Dictionary<string, MotionData> motionNameDataPair = new Dictionary<string, MotionData>();

    public void FirebasePush()
    {
        Debug.Log("FirebasePush");
        MotionManager.RequestAll((res) =>
        {
            if (res == false)
            {
                Debug.Log("Firebase request false");
                return;
            }
            List<string> FB_motionNames = MotionManager.GetNameList();

            // Load files information
            DirectoryInfo directoryInfo = new DirectoryInfo(MOTION_PATH);
            FileInfo[] filesInfo = directoryInfo.GetFiles("*.json");
            foreach (FileInfo fi in filesInfo)
            {
                string motionName = fi.Name.Substring(0, fi.Name.Length - 5);

                MotionDataSet motionDataset = MotionManager.CreateMotion(motionName);
                if (FB_motionNames != null)
                {
                    if (FB_motionNames.Contains(motionName))
                    {
                        var motion = MotionManager.GetByName(motionName);
                        motionDataset = motion;
                    }
                }
                motionDataset.name = motionName;
                motionDataset.jsonData = File.ReadAllText(fi.FullName);
                MotionManager.SaveMotion(motionDataset, (result) =>
                {
                    if (result == MotionManager.ResultType.OK)
                    {
                        MessageBox.Show("[ID_MSG_SAVE_MOTION_FIREBASE]모션 파이어베이스 저장 완료!"); // local 추가완료
                    }
                    else
                    {
                        MessageBox.Show("[ID_MSG_ FAIL_TO_SAVE_MOTION_FIREBASE]모션 파이어베이스 저장 실패 : " + result);   // 추가완료
                    }
                });
            }
        });


    }

    public void FirebasePull()
    {
        Debug.Log("FirebasePull");
        MotionManager.RequestAll((res) =>
        {
            if (res == false)
            {
                Debug.Log("Firebase request false");
                return;
            }
            var uuids = MotionManager.GetUuidList();
            if (uuids != null)
            {
                //Debug.Log("Found motion count " + MotionManager.GetNameList().Count.ToString());

                motionNameDataPair.Clear();
                foreach (var uuid in MotionManager.GetUuidList())
                {
                    var motion = MotionManager.GetByUuid(uuid);
                    //string txt = string.Format("{0}\n({1})", motion.name, motion.uuid);
                    //Debug.Log(txt);

                    // 작성자: 장세윤.
                    // 모션 데이터 받아서 파일로 저장할 때, 나중에 로드하기 쉽도록 Dictionary에 저장.
                    AddToMotionNameDataPair(motion.name, motion.jsonData);

                    string filePath = MOTION_PATH + motion.name + ".json";
                    File.WriteAllText(filePath, motion.jsonData);
                }

                // 작성자: 장세윤.
                // 모션 데이터 받아서 파일로 저장할 때, 나중에 로드하기 쉽도록 Dictionary에 저장.
                //string[] files = Directory.GetFiles(motion_dir, "*.json");
                //foreach (string file in files)
                //{
                //    if (file.Contains(".meta"))
                //    {
                //        continue;
                //    }

                //    string motionName = file.Replace(motion_dir, "");
                //    motionName = motionName.Replace(".json", "");

                //    //Debug.Log($"[motionName] {motionName}");
                //    AddToMotionNameDataPair(motionName, File.ReadAllText(file));
                //}
            }
        });
    }


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        MOTION_PATH = Application.dataPath + "/MotionData/";
#elif UNITY_ANDROID || UNITY_IOS
        MOTION_PATH = Application.persistentDataPath + "/MotionData/";
#else
        //MOTION_PATH = Application.dataPath + "/MotionData/";
        MOTION_PATH = Application.persistentDataPath + "/MotionData/";
#endif

        DirectoryInfo directoryInfo = new DirectoryInfo(MOTION_PATH);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        FirebasePull();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    // 작성자: 장세윤.
    // 모션이름/모션데이터를 딕셔너리에 저장하는 메소드 1.
    // 모션이름, 모션 json 데이터.
    private void AddToMotionNameDataPair(string motionName, string jsonData)
    {
        motionNameDataPair.Add(motionName, JsonUtility.FromJson<MotionData>(jsonData));
    }

    // 작성자: 장세윤.
    // 모션이름/모션데이터를 딕셔너리에 저장하는 메소드 2.
    // 모션이름, 모션 데이터.
    private void AddToMotionNameDataPair(string motionName, MotionData motionData)
    {
        motionNameDataPair.Add(motionName, motionData);
    }

    // 작성자: 장세윤.
    // 모션 이름을 Key로 사용해 모션 데이터를 얻을 때 사용하는 메소드.
    public bool GetMotionData(string motionName, out MotionData motionData)
    {
        // Editor: movehoon
        // 입력한 motionName으로 motionNameDataPair를 검색하여 TAG에 일치하면
        // 모션파일 실행이 아니라 모션 카테고리 실행으로 인식하여
        // 해당 모션들을 motionCandidate에 저장한다.
        List<string> motionCandidate = new List<string>();
        foreach (var kvp in motionNameDataPair)
        {
            MotionData motData = kvp.Value;
            if (motData.GetTag() != null)
            {
                if (motData.GetTag().Equals(motionName))
                {
                    motionCandidate.Add(kvp.Key);
                }
            }
        }
        // 모션 TAG로 검색된 모션이 존재하는 경우
        // motionCandidate에 있는 모션 중 random하게 motionName을 교체한다.
        if (motionCandidate.Count > 0)
        {
            System.Random rnd = new System.Random();
            motionName = motionCandidate[rnd.Next(0, motionCandidate.Count - 1)];
        }

        return motionNameDataPair.TryGetValue(motionName, out motionData);
    }
}