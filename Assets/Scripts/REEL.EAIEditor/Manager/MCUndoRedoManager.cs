using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCUndoRedoManager : Singleton<MCUndoRedoManager>
    {
        //private Stack<MCCommand> undos = new Stack<MCCommand>();
        //private Stack<MCCommand> redos = new Stack<MCCommand>();

        private Queue<MCCommand> commandBuffer = new Queue<MCCommand>();
        private List<MCCommand> commandHistory = new List<MCCommand>();

        private int counter = 0;

        //private void Update()
        //{
        //    if (commandBuffer.Count > 0)
        //    {
        //        MCCommand command = commandBuffer.Dequeue();
        //        command.Execute();

        //        commandHistory.Add(command);
        //        ++counter;
        //    }
        //}

        // 저장된 기록정보 모두 삭제.
        // 프로젝트 탭 -> 함수 탭으로 이동할 때 사용.
        // Todo: 현재, 함수에서는 Undo를 지원하는 않음. 함수에서도 Undo 동작 가능하도록.
        public void ResetAllRecord()
        {
            //Debug.Log("ResetAllRecord");
            
            //undos.Clear();
            //redos.Clear();

            commandBuffer.Clear();
            commandHistory.Clear();
            counter = 0;
        }

        public void AddCommand(MCCommand command)
        {
            while (commandHistory.Count > counter)
            {
                commandHistory.RemoveAt(counter);
            }
            
            //commandBuffer.Enqueue(command);

            command.Execute();

            commandHistory.Add(command);
            ++counter;

            // Set Project Dirty.
            MCWorkspaceManager.Instance.IsDirty = true;
        }

        // 명령 기록을 위해 추가를 하는데 바로 실행은 안함.
        public void AddCommandDontExecute(MCCommand command)
        {
            while (commandHistory.Count > counter)
            {
                commandHistory.RemoveAt(counter);
            }

            commandHistory.Add(command);
            ++counter;

            // Set Project Dirty.
            MCWorkspaceManager.Instance.IsDirty = true;
        }

        // commandHistroy 리스트에서 counter를 하나 감소시킨 뒤 이전 명령 실행.
        public void UnDo()
        {
            //Debug.Log($"UnDo Count: {undos.Count}");

            //if (undos.Count > 0)
            //{
            //    MCCommand command = undos.Pop();
            //    //Debug.Log($"[MCUndoRedoManager] command type: {command.GetType()}");
            //    redos.Push(command);
            //    command.Undo();
            //}

            if (counter > 0)
            {
                --counter;
                //Utils.LogRed($"commandHistory[counter]: {commandHistory[counter].GetType()}");
                commandHistory[counter].Undo();
            }
        }

        // commandHistroy 리스트에서 counter 번째 명령을 다시 실행.
        public void ReDo()
        {
            //if (redos.Count > 0)
            //{
            //    MCCommand command = redos.Pop();
            //    undos.Push(command);
            //    command.Execute();
            //}

            if (counter < commandHistory.Count)
            {
                commandHistory[counter].Execute();
                ++counter;
            }
        }
    }
}