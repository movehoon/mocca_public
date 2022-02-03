#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class GetSetNodeWindow : MCNodeWindow
    {
        [Header("Value Block")]
        public GameObject valueBlockPrefab;
        public RectTransform valueBlockTopTransform;
        [SerializeField] protected ValueBlock[] valueBlocks;

        [Header("Expression Block")]
        public GameObject expressionBlockPrefab;
        public GameObject ttsBlockPrefab;
        public GameObject facialBlockPrefab;
        public GameObject motionBlockPrefab;
        [SerializeField] protected ExpressionBlock[] expressionBlocks;

        [Space]
        [SerializeField] protected ScrollRect scrollRect;
        [SerializeField] protected RectTransform content;

        protected float defaultHeight = 120f;
        protected float itemHeight = 60f;

        protected int currentVarCount;

        protected readonly int blockCount = 15;
        protected readonly float blockItemHeight = 50f;
        protected readonly float blockItemSpace = 10f;
        protected float contentInitialHeight = 0f;

        protected bool hasInitialized = false;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            contentInitialHeight = content.sizeDelta.y;

            int lastSibilingIndex = 0;
            valueBlocks = new ValueBlock[blockCount];
            for (int ix = 0; ix < blockCount; ++ix)
            {
                GetSetValueBlock newBlock = Instantiate(valueBlockPrefab, content).GetComponent<GetSetValueBlock>();
                if (newBlock != null)
                {
                    newBlock.transform.SetSiblingIndex(valueBlockTopTransform.GetSiblingIndex() + ix + 1);

                    valueBlocks[ix] = new ValueBlock();
                    newBlock.parameterName.text = "Value " + (ix + 1).ToString() + ": ";
                    valueBlocks[ix].block = newBlock.valueBlock.block;
                    valueBlocks[ix].value = newBlock.valueBlock.value;

                    lastSibilingIndex = newBlock.transform.GetSiblingIndex();
                }
            }

            expressionBlocks = new ExpressionBlock[blockCount];
            for (int ix = 0; ix < blockCount; ++ix)
            {
                GetSetExpressionBlock newExp = Instantiate(expressionBlockPrefab, content).GetComponent<GetSetExpressionBlock>();
                GetSetTTSBlock newTts = Instantiate(ttsBlockPrefab, content).GetComponent<GetSetTTSBlock>();
                GetSetFacialBlock newFacial = Instantiate(facialBlockPrefab, content).GetComponent<GetSetFacialBlock>();
                GetSetMotionBlock newMotion = Instantiate(motionBlockPrefab, content).GetComponent<GetSetMotionBlock>();
                if (newExp != null && newTts != null && newFacial != null && newMotion != null)
                {
                    newExp.transform.SetSiblingIndex(lastSibilingIndex + 1);
                    expressionBlocks[ix] = new ExpressionBlock();
                    newExp.parameterName.text = "Express " + (ix + 1).ToString();
                    expressionBlocks[ix].block = newExp.gameObject;
                    expressionBlocks[ix].tts = newTts.text;
                    expressionBlocks[ix].facial = newFacial.text;
                    expressionBlocks[ix].motion = newMotion.text;

                    lastSibilingIndex = newMotion.transform.GetSiblingIndex();
                }
            }

            Vector2 size = Vector2.zero;
            size.y = contentInitialHeight + (blockCount * (blockItemHeight + blockItemSpace)) + (blockCount * 4 * (blockItemHeight + blockItemSpace));
            content.sizeDelta = size;

            hasInitialized = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight);
        }

        protected void SetBlocks(PROJECT.NodeType nodeType, int count)
        {
            TurnOffAll();
            TurnOnWithCount(nodeType, count);
        }

        protected void TurnOffAll()
        {
            for (int ix = 0; ix < expressionBlocks.Length; ix++)
            {
                expressionBlocks[ix].ShowBlock(false);
            }

            for (int ix = 0; ix < valueBlocks.Length; ix++)
            {
                valueBlocks[ix].ShowBlock(false);
            }
        }

        protected void TurnOnWithCount(PROJECT.NodeType nodeType, int count)
        {
            BlockInterface[] targetBlocks = null;

            if (nodeType == PROJECT.NodeType.EXPRESSION)
            {
                targetBlocks = expressionBlocks;

                //for (int ix = 0; ix < count; ix++)
                //{
                //    expressionBlocks[ix].ShowBlock(true);
                //}
            }
            else
            {
                targetBlocks = valueBlocks;
                //for (int ix = 0; ix < count; ix++)
                //{
                //    valueBlocks[ix].ShowBlock(true);
                //}
            }

            for (int ix = 0; ix < count; ++ix)
            {
                targetBlocks[ix].ShowBlock(true);
            }
        }
    }

    public interface BlockInterface
    {
        void ShowBlock(bool isShow);
    }


    [System.Serializable]
    public class ExpressionBlock : BlockInterface
    {
        public GameObject block;

#if USINGTMPPRO
        public TMP_Text tts;
        public TMP_Text facial;
        public TMP_Text motion;
#else
        public Text tts;
        public Text facial;
        public Text motion;
#endif

        public void ShowBlock(bool isShow)
        {
            block.SetActive(isShow);
            tts.transform.parent.gameObject.SetActive(isShow);
            facial.transform.parent.gameObject.SetActive(isShow);
            motion.transform.parent.gameObject.SetActive(isShow);
        }
    }

    [System.Serializable]
    public class ValueBlock : BlockInterface
    {
        public GameObject block;
#if USINGTMPPRO
        public TMP_Text value;
#else
        public Text value;
#endif

        public void ShowBlock(bool isShow)
        {
            block.SetActive(isShow);
        }
    }
}