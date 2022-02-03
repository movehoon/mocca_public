using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCNodeImageCreator : MonoBehaviour
    {
        private MCNode refNode;

        private void Awake()
        {
            if (refNode == null)
            {
                refNode = GetComponent<MCNode>();
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F10))
            {
                StartCoroutine("TakeScreenShot");
            }
#endif
        }

        private IEnumerator TakeScreenShot()
        {
            yield return new WaitForEndOfFrame();

            if (Directory.Exists(Path.Combine(Application.dataPath, "../NodeScreenShot")) == false)
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "../NodeScreenShot"));
            }
            var directory = Path.Combine(Application.dataPath, "../NodeScreenShot");

            int width = Convert.ToInt32(refNode.RefRect.rect.width) + 2;
            int height = Convert.ToInt32(refNode.RefRect.rect.height) + 2;

            Vector2 position = refNode.RefRect.position;
            var startX = position.x - width / 2;
            var startY = position.y - height / 2 + 1;

            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
            texture.Apply();

            var bytes = texture.EncodeToPNG();
            Destroy(texture);

            //Utils.LogRed($"width: {width} / height: {height} / startX: {startX} / startY: {startY} / position: {position}");

            File.WriteAllBytes(Path.Combine(directory, refNode.nodeData.type.ToString() + ".png"), bytes);
        }
    }
}