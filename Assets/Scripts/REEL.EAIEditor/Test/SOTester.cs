using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

namespace REEL.D2EEditor
{
    public class SOTester : MonoBehaviour
    {
        private static readonly string filePath = "Assets/Resources/PartData/TestSO.asset";

        [SerializeField]
        private TestSO soFile;

        //[MenuItem("SO Tester/Create Test SO File")]
        //public static void CreateSOFile()
        //{
        //    TestSO testSo = ScriptableObject.CreateInstance<TestSO>();
        //    AssetDatabase.CreateAsset(testSo, filePath);
        //}

        private void Awake()
        {
            soFile.mouthpos = 300f;
            soFile.speed = 100000f;
        }
    }
}