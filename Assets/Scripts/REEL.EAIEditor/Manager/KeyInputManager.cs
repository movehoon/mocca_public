using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class KeyInputManager : Singleton<KeyInputManager>
    {
        public bool shouldMultiSelect = false;
        public bool isShiftPressed = false;
        public bool isAltPressed = false;

        public UnityEvent OnCtrlAKeyDown;
        public UnityEvent OnCtrlCKeyDown;
        public UnityEvent OnCtrlVKeyDown;
        public UnityEvent OnCtrlZKeyDown;
        public UnityEvent OnCtrlRKeyDown;
        public UnityEvent OnCtrlPKeyDown;
        public UnityEvent OnDeleteKeyDown;
        public UnityEvent OnCtrlSKeyDown;
        public UnityEvent OnCtrlOKeyDown;
        public UnityEvent OnCtrlNKeyDown;
        public UnityEvent OnESCKeyDown;
        public UnityEvent OnCKeyDown;

        void Update()
        {
            // 디버깅용.
            // 작성자: 장세윤 (20211118).
            // 프로젝트 string 데이터를 MCWorkspaceManager의 compiledProjectString에 입력한 다음,
            // Alt + F10으로 프로젝트를 강제 로드하는 데 사용하는 단축키.
#if UNITY_EDITOR
            if (isAltPressed && Input.GetKeyDown(KeyCode.F10))
            {
                MCWorkspaceManager.Instance.LoadProjectFromText();
            }
#endif


#if UNITY_STANDALONE_WIN
            if (shouldMultiSelect == true && Input.GetKey(KeyCode.LeftControl) == false)
#elif UNITY_STANDALONE_OSX
            if (shouldMultiSelect == true && Input.GetKey(KeyCode.LeftCommand) == false)
#endif
            {
                shouldMultiSelect = false;
            }

#if UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.LeftControl))
#elif UNITY_STANDALONE_OSX
            if (Input.GetKeyDown(KeyCode.LeftCommand))
#endif
            {
                shouldMultiSelect = true;
            }

#if UNITY_STANDALONE_WIN
            if (Input.GetKeyUp(KeyCode.LeftControl))
#elif UNITY_STANDALONE_OSX
            if (Input.GetKeyUp(KeyCode.LeftCommand))
#endif
            {
                shouldMultiSelect = false;
            }

            if (isShiftPressed == true && Input.GetKey(KeyCode.LeftShift) == false)
            {
                isShiftPressed = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) == true)
            {
                isShiftPressed = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) == true)
            {
                isShiftPressed = false;
            }

            if (isAltPressed == true && Input.GetKey(KeyCode.LeftAlt) == false)
            {
                isAltPressed = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt) == true)
            {
                isAltPressed = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt) == true)
            {
                isAltPressed = false;
            }

            //if (MCWorkspaceManager.Instance.IsSimulation == true)
            if (MCPlayStateManager.Instance.IsSimulation == true)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
            {
                OnCtrlAKeyDown?.Invoke();
                //MCWorkspaceManager.Instance.SetAllSelected();
            }

#else
            if (shouldMultiSelect && Input.GetKeyDown(KeyCode.A)) 
            {
                // InputField가 선택된 상태에서 Ctrl + A키가 눌리면 블록 선택 처리 안하도록.
                if (EventSystem.current.currentSelectedGameObject == null ||
                    EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null &&
                    EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null)
                {
                    MCWorkspaceManager.Instance.SetAllSelected();
                }
            }
#endif


            //튜토리얼 진행때에는 단축키 작동 금지 - kjh
            if(TutorialManager.IsPlaying) return;


            //if (Input.GetKeyDown(KeyCode.Delete) && Input.imeCompositionMode == IMECompositionMode.Auto)
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                // InputField가 선택된 상태에서 Delete 키가 눌리면 블록 삭제 안하도록 처리.
                if (EventSystem.current.currentSelectedGameObject == null ||
                    EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null &&
                    EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null)
                {
                    // 삭제하기 전에 현 상태 저장.
                    //MCUndoRedoManager.Instance.RecordProject();

                    OnDeleteKeyDown?.Invoke();

                    //MCCommentManager.Instance.DeleteSelected();
                    //MCWorkspaceManager.Instance.DeleteSelected();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnESCKeyDown?.Invoke();

                //MCCommentManager.Instance.SetAllUnSelected();
                //MCWorkspaceManager.Instance.SetAllUnSelected();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    OnCtrlSKeyDown?.Invoke();
                }
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    OnCtrlOKeyDown?.Invoke();
                }
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    OnCtrlNKeyDown?.Invoke();
                }
            }

            // Ctrl + C.
            if (Input.GetKeyDown(KeyCode.C))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    OnCtrlCKeyDown?.Invoke();

                    //MCWorkspaceManager.Instance.CopySelectedNodes();
                }
                else
                {
                    //if (!MCWorkspaceManager.Instance.IsSimulation
                    if (!MCPlayStateManager.Instance.IsSimulation
                        && MCProjectManager.ProjectDescription != null
                        && EventSystem.current.currentSelectedGameObject == null ||
                        (EventSystem.current.currentSelectedGameObject != null && 
                        EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null) 
                        && (EventSystem.current.currentSelectedGameObject != null && 
                        EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null))
                    {
                        OnCKeyDown?.Invoke();

                        //MCCommentManager.Instance.AddNewComment("코멘트", Input.mousePosition);
                    }
                }
            }

            // Ctrl + V.
            if (Input.GetKeyDown(KeyCode.V))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    if (EventSystem.current.currentSelectedGameObject == null ||
                        EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null &&
                        EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null)
                    {
                        OnCtrlVKeyDown?.Invoke();
                        //MCWorkspaceManager.Instance.PasteSelectedNodes();
                    }
                }
            }

            // Ctrl + Z.
            if (Input.GetKeyDown(KeyCode.Z))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    if (EventSystem.current.currentSelectedGameObject == null ||
                        EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null &&
                        EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null)
                    {
                        OnCtrlZKeyDown?.Invoke();

                        //MCUndoRedoManager.Instance.UnDo();
                    }
                }
            }

            // Ctrl + R.
            if (Input.GetKeyDown(KeyCode.R))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    if (EventSystem.current.currentSelectedGameObject == null ||
                        EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() == null &&
                        EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null)
                    {
                        OnCtrlRKeyDown?.Invoke();

                        //MCUndoRedoManager.Instance.UnDo();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
#if UNITY_EDITOR
                if (isShiftPressed)
#else
                if (shouldMultiSelect)
#endif
                {
                    OnCtrlPKeyDown?.Invoke();
                }
            }
        }
    }
}