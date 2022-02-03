using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class FileWindow : MonoBehaviour
    {
        [SerializeField] protected string fileListItemName = "FileListItem";
        [SerializeField] protected FileExplorer fileExplorer;
        [SerializeField] protected RectTransform fileListContent;
        [SerializeField] protected float contentBaseHeight = 60f;
        [SerializeField] protected TabManager tabManager;
        [SerializeField] protected string dataPath;

        [SerializeField] protected string selectedProjectFileName = string.Empty;
        [SerializeField] protected List<FileListItem> fileList = new List<FileListItem>();
        [SerializeField] protected InputField inputField;

        public InputField GetInputField { get { return inputField; } }

        protected virtual void Awake()
        {
            dataPath = Application.dataPath + "/Data";
        }

        protected virtual void OnEnable()
        {
            if (inputField != null) inputField.text = string.Empty;
        }

        public virtual void SetSelectedProjectFileName(Text fileNameText)
        {
            selectedProjectFileName = fileNameText.text;
        }

        protected virtual void ResetList()
        {
            if (fileList.Count == 0) return;

            for (int ix = 0; ix < fileList.Count; ++ix)
            {
                ReturnObject(fileListItemName, fileList[ix].gameObject, ObjectPool.Instance.transform);
            }

            fileList = new List<FileListItem>();
            inputField.text = string.Empty;
        }

        protected virtual void UpdateFileContentHeight()
        {
            if (fileList.Count == 0) return;

            float offset = (fileList.Count - 1) * contentBaseHeight;
            fileListContent.sizeDelta = new Vector2(fileListContent.sizeDelta.x, fileListContent.sizeDelta.y + offset);
        }

        protected virtual void ReturnObject(string objName, GameObject listObj, Transform parent)
        {
            ObjectPool.Instance.PushToPool(objName, listObj, parent);
        }
    }
}