using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCProjectTab : TabComponent
    {
        //private bool shouldValidateProject = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            ownerGroup = Constants.OwnerGroup.PROJECT;
        }

        protected override void OnSelected()
        {
            //Debug.LogWarning("ProjectTab.OnSelected");
            base.OnSelected();

            // 함수 -> 프로젝트 탭 전환.
            if (MCWorkspaceManager.Instance.HasCompiled)
            {
                // 컴파일된 프로젝트 정보를 사용해 노드 배치 복원.
                MCWorkspaceManager.Instance.ReloadFromCompiledProject();

                // 함수 탭이 선택된 상태에서 변수나 함수가 삭제됐었는지 확인.
                // 배치된 노드가 유효한지 확인.
                // 바로 검사하면, UI가 다 로드되지 않은 상태에서 처리되기 때문에,
                // LateUpdate에서 처리하도록 bool 변수 true로 설정.
                MCWorkspaceManager.Instance.DelayValidateProject();

                //MCWorkspaceManager.Instance.DelayValidateProjectCall();
                //MCWorkspaceManager.Instance.ValidateProject();

                // 로컬 변수 제거.
                MCVariableFunctionManager manager = FindObjectOfType<MCVariableFunctionManager>();
                if (manager != null)
                {
                    manager.RemoveAllLocalVariables();
                }
            }
        }

        public override void CloseTab()
        {
            //if (MCWorkspaceManager.Instance.IsSimulation == true)
            if (MCPlayStateManager.Instance.IsSimulation == true)
            {
                return;
            }

            if (TutorialManager.IsPlaying == true)
                return;

            //MCFunctionTable.Instance.DeleteAllFunctionNodes();
            //MCFunctionTable.Instance.DeleteAllFunctions();

            MCWorkspaceManager.Instance.OnCloseCurrentProjectClicked();
        }
    }
}