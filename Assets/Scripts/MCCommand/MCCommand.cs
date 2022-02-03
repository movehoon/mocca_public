using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public interface MCCommand
    {
        void Execute();
        void Undo();
    }
}