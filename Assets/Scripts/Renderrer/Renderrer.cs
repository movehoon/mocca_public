using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Renderrer
{
    void Init();
    void Play(string name);
    void Stop();
    bool IsRunning();
}