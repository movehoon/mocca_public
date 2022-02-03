using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCWindowedModeSetter : MonoBehaviour
    {
        // 프로그램 시작할 때 강제로 창 모드로 실행하도록 설정.
        private void Awake()
        {
            if (Screen.fullScreen == true)
            {
                Screen.fullScreen = false;
            }
        }
    }
}