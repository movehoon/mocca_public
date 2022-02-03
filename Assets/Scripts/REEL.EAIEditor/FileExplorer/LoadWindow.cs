using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    using TabAddType = TabManager.TabAddType;

    public class LoadWindow : FileWindow
    {
        public void LoadFileList()
        {
            ResetList();

            string[] directories = Directory.GetDirectories(dataPath);

            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory, "*.json");
                
                for (int ix = 0; ix < files.Length; ++ix)
                {
                    GameObject item = ObjectPool.Instance.PopFromPool(fileListItemName, fileListContent);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;
                    item.SetActive(true);

                    string[] info = files[ix].Split(new char[] { '/', '\\' });
                    string fileName = info[info.Length - 1];
                    fileName = fileName.Split(new char[] { '.' })[0];

                    FileListItem list = item.GetComponent<FileListItem>();
                    list.SetFileName(fileName);
                    list.SetFileWindow(this);
                    list.SetInputField(inputField);

                    fileList.Add(list);

                    EventTrigger trigger = list.GetComponent<EventTrigger>();
                }
            }

            UpdateFileContentHeight();
        }

        public void OnLoadClosed()
        {
            inputField.text = string.Empty;
            ResetList();
        }

        public void OnLoadClicked()
        {
            if (string.IsNullOrEmpty(inputField.text)) return;

            tabManager.AddProjectTab(selectedProjectFileName, TabAddType.Load);
            //WorkspaceManager.Instance.LoadFromFile(selectedProjectFileName);
        }
    }
}