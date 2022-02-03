using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
	public class SaveWindow : FileWindow
	{
        public SaveConfirmWindow confirmWindow;

        public void LoadFileList(string currentTabName = "")
        {
            ResetList();

            string[] directories = Directory.GetDirectories(dataPath);

            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory, "*.json");

                for (int ix = 0; ix < files.Length; ++ix)
                {
                    //Debug.Log(files[ix]);
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

            // Show current tab name on the input field.
            inputField.text = currentTabName;

            UpdateFileContentHeight();
        }

        public void OnSaveClosed()
        {
            inputField.text = string.Empty;
            ResetList();
        }

        public void OnSaveClicked()
        {
            if (IsFileExist)
            {
                //Debug.LogWarning("파일 존재함: " + inputField);
                confirmWindow.ShowWindow();
            }

            else
            {
                //MCWorkspaceManager.Instance.SaveProject(inputField.text);
                //tabManager.SaveProject(inputField.text);
                ResetWindow();
            }
        }

        void ResetWindow()
        {
            inputField.text = string.Empty;
            ResetList();
            fileExplorer.OnCancelClicked();
        }

        public void SaveAnyway()
        {
            //tabManager.SaveProject(inputField.text);
            //MCWorkspaceManager.Instance.SaveProject(inputField.text);
        }

        private bool IsFileExist
        {
            get
            {
                string[] directories = Directory.GetDirectories(dataPath);
                for (int ix = 0; ix < directories.Length; ++ix)
                {
                    string[] files = Directory.GetFiles(directories[ix], "*.json");

                    for (int jx = 0; jx < files.Length; ++jx)
                    {
                        string[] info = files[jx].Split(new char[] { '/', '\\' });
                        string fileName = info[info.Length - 1];
                        fileName = fileName.Split(new char[] { '.' })[0];

                        if (string.Equals(fileName, inputField.text))
                            return true;
                    }
                }

                return false;
            }
        }

        //public void OnSaveClicked()
        //{
        //    if (string.IsNullOrEmpty(inputField.text)) return;

        //    //tabManager.AddTab(selectedProjectFileName);
        //    BlockDiagramManager.Instance.SaveToFile(string.IsNullOrEmpty(selectedProjectFileName) ? "Test" : selectedProjectFileName);
        //}
    }
}