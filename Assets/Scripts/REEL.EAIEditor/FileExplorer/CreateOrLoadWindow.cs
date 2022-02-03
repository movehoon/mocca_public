using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class CreateOrLoadWindow : MonoBehaviour
	{
        [SerializeField] private TabManager tabManager = null;
        [SerializeField] private GameObject newAndLoadWindow = null;
        [SerializeField] private FileExplorer fileExplorerWindow = null;

        private event Action OnFileExplorerOpenEvent;
        private event Action OnFileExplorerCloseEvent;

        public void OnCloseClicked()
        {
            newAndLoadWindow.SetActive(false);
            fileExplorerWindow.gameObject.SetActive(false);

            gameObject.SetActive(false);

            RaiseFileExplorerCloseEvent();
        }

        public void OnOpenClicked()
        {
            if (!tabManager.CanAddTab) return;

            gameObject.SetActive(true);
            newAndLoadWindow.SetActive(true);

            RaiseFileExplorerOpenEvent();
        }

        public void OpenSaveWindow()
        {
            if (tabManager.CurrentTab == null)
                return;

            gameObject.SetActive(true);
            fileExplorerWindow.gameObject.SetActive(true);
            fileExplorerWindow.OpenSaveWindow(tabManager.CurrentTab.TabName);

            RaiseFileExplorerOpenEvent();
        }

        public void OpenLoadWindow()
        {
            gameObject.SetActive(true);
            fileExplorerWindow.gameObject.SetActive(true);
            fileExplorerWindow.OpenLoadWindow();

            RaiseFileExplorerOpenEvent();
        }

        public void OnLoadClicked()
        {
            newAndLoadWindow.SetActive(false);
            fileExplorerWindow.gameObject.SetActive(true);

            RaiseFileExplorerOpenEvent();
        }


		public void OnWebLoadClicked()
		{
			gameObject.SetActive(false);
			newAndLoadWindow.SetActive(false);
			fileExplorerWindow.gameObject.SetActive(false);
		}

		public void SubscribeFileExplorerOpenEvent(Action listener)
        {
            OnFileExplorerOpenEvent += listener;
        }

        public void SubscribeFileExplorerCloseEvent(Action listener)
        {
            OnFileExplorerCloseEvent += listener;
        }

        private void RaiseFileExplorerOpenEvent()
        {
            OnFileExplorerOpenEvent?.Invoke();
        }

        private void RaiseFileExplorerCloseEvent()
        {
            OnFileExplorerCloseEvent?.Invoke();
        }
	}
}