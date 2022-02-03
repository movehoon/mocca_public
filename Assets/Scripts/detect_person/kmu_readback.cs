//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.Rendering;

//public class kmu_readback : MonoBehaviour{

//    RenderTexture renderTexture;
//    Vector2 scale = new Vector2(1, 1);
//    Vector2 offset = Vector2.zero;
//    byte[] pixels;

//    public void ScaleAndCropImage(WebCamTexture webCamTexture, int size_w, int size_h)
//    {

//        if (renderTexture == null)
//        {
//            renderTexture = new RenderTexture(size_w, size_h, 0, RenderTextureFormat.ARGB32);
//        }

//        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
//        offset.x = (1 - scale.x) / 2f;
//        Graphics.Blit(webCamTexture, renderTexture);
//        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, Process.OnCompleteReadBack);
//    }
//}