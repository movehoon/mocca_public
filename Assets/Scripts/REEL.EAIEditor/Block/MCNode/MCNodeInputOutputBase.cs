using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCNodeInputOutputBase : MCNodeSocket
    {
        protected enum ParameterPosition { Left, Right }

        public DataType parameterType = DataType.BOOL;

        public Sprite setSprite;

        protected Sprite normalSprite;
        protected Image refImage;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            if (refImage == null)
            {
                refImage = GetComponent<Image>();
            }

            normalSprite = refImage.sprite;

            lineColor = Utils.GetParameterColor(parameterType);
            refImage.color = lineColor;
        }

        // Dummy.
        public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        {
            return false;
        }

        public void ChangeSpriteToNomal()
        {
            if (refImage == null)
            {
                if (line != null)
                {
                    MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);
                    LineDeleted();
                }

                return;
            }

            refImage.sprite = normalSprite;
        }

        public void ChangeSpriteToSet()
        {
            if (refImage == null)
            {
                if (line != null)
                {
                    MCWorkspaceManager.Instance.RequestLineDelete(line.LineID);
                    LineDeleted();
                }

                return;
            }

            refImage.sprite = setSprite;
        }

        public override void LineDeleted()
        {
            base.LineDeleted();
            ChangeSpriteToNomal();
        }

        public override void LineSet()
        {
            base.LineSet();
            ChangeSpriteToSet();

            //----------------------------------------
            // 입력/출력 파라미터에 선 연결됐을 때 발생하는 튜토리얼이벤트 -kjh
            //----------------------------------------
            TutorialManager.SendEvent(Tutorial.CustomEvent.ParameterLinked);
        }

        public virtual void SetLineColor(Color color)
        {
            if (refImage == null)
            {
                refImage = GetComponent<Image>();
            }

            refImage.color = color;
            lineColor = color;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // Test.
            shouldOverCheck = false;

            if (HasLine || line == null)
            {
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                MCNodeInputOutputBase targetSocket = result.gameObject.GetComponent<MCNodeInputOutputBase>();
                MagneticBound magneticBound = result.gameObject.GetComponent<MagneticBound>();
                if (targetSocket != null || (magneticBound != null && magneticBound.inputoutputBase != null))
                {
                    targetSocket = targetSocket != null ? targetSocket : magneticBound.inputoutputBase;

                    //소켓의 enabled 이 꺼져있으면 연결되지 않는다.-kjh
                    if(targetSocket.enabled == false) continue;

                    if (!targetSocket.Node.NodeID.Equals(Node.NodeID)
                        && CheckTargetSocketType(targetSocket.socketType, targetSocket))
                    {
                        //if (socketPosition == SocketPosition.Left)
                        //{
                        //    line.SetLinePoint(targetSocket, this);
                        //}

                        //else if (socketPosition == SocketPosition.Right)
                        //{
                        //    line.SetLinePoint(this, targetSocket);
                        //}

                        //MCWorkspaceManager.Instance.AddLine(line);
                        //LineSet();
                        //targetSocket.LineSet();

                        MCAddLineCommand command = null;
                        if (socketPosition == SocketPosition.Left)
                        {
                            command = new MCAddLineCommand(line, line.LineID, targetSocket, this);
                            //line.SetLinePoint(targetSocket, this);
                        }

                        else if (socketPosition == SocketPosition.Right)
                        {
                            command = new MCAddLineCommand(line, line.LineID, this, targetSocket);
                            //line.SetLinePoint(this, targetSocket);
                        }

                        MCUndoRedoManager.Instance.AddCommand(command);

                        //MCWorkspaceManager.Instance.AddLine(line);
                        //LineSet();
                        //targetSocket.LineSet();

                        return;
                    }
                }
            }

            HasLine = false;
            Destroy(line.gameObject);
            line = null;
        }

        // 비교할 두 파라미터의 타입을 비교할 때 사용할 도움 함수.
        protected virtual bool CheckTwoParameterType<T>(ParameterPosition myPosition, DataType leftCompareDataType, MCNodeSocket targetSocket, DataType rightCompareDataType) where T : MCNodeInputOutputBase
        {
            DataType leftDataType
                = myPosition == ParameterPosition.Left ? parameterType : targetSocket.GetComponent<T>().parameterType;
            DataType rightDataType
                = myPosition == ParameterPosition.Right ? parameterType : targetSocket.GetComponent<T>().parameterType;

            if (leftDataType == leftCompareDataType && rightDataType == rightCompareDataType)
            {
                return true;
            }

            return false;
        }

        // 입력/출력 연결 시 두 파라미터가 같은지, 다른 경우 예외처리해줘야할 경우가 있는지 확인하는 함수.
        protected virtual bool IsParametersConnectable<T>(ParameterPosition myPosition, MCNodeSocket targetSocket, string targetValue) where T : MCNodeInputOutputBase
        {
            if (targetSocket.GetComponent<T>() == null)
            {
                return false;
            }

            if (parameterType == targetSocket.GetComponent<T>().parameterType)
            {
                //if (targetSocket.Node.nodeData.type == NodeType.FUNCTION_OUTPUT)
                //{
                //    Utils.LogRed($"{parameterType } / {targetSocket.GetComponent<T>().parameterType} / {parameterType == targetSocket.GetComponent<T>().parameterType}");
                //}

                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.STRING, targetSocket, DataType.NUMBER))
            {
                // 값 변환 시도.
                // Number -> String.
                //if (int.TryParse(targetValue, out int retValue))
                //{
                //    return true;
                //}

                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.NUMBER, targetSocket, DataType.STRING))
            {
                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.BOOL, targetSocket, DataType.NUMBER))
            {
                // 값 변환 시도.
                // Bool -> Number.
                //if (bool.TryParse(targetValue, out bool retValue))
                //{
                //    return true;
                //}

                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.NUMBER, targetSocket, DataType.BOOL))
            {
                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.BOOL, targetSocket, DataType.STRING))
            {
                return true;
            }
            else if (CheckTwoParameterType<T>(myPosition, DataType.STRING, targetSocket, DataType.BOOL))
            {
                return true;
            }

            return false;
        }
    }
}