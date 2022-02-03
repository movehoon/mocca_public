using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    [System.Serializable]
	public class PooledObject
	{
        public string poolItemName = string.Empty;
        [SerializeField] private GameObject prefab = null;
        public Transform parentTransform;
        public int poolCount = 0;

        [SerializeField] private Stack<GameObject> poolList = new Stack<GameObject>();

        public void Initialize(Transform parent = null)
        {
            for (int ix = 0; ix < poolCount; ++ix)
            {
                poolList.Push(CreateItem(parent));
            }
        }

        public void PushToPool(GameObject item, Transform parent = null)
        {
            item.transform.SetParent(parent);
            item.SetActive(false);
            poolList.Push(item);
        }

        public GameObject PopFromPool(Transform parent = null)
        {
            if (poolList.Count == 0) poolList.Push(CreateItem(parent));

            GameObject item = poolList.Pop();
            item.transform.SetParent(parent);

            return item;
        }

        private GameObject CreateItem(Transform parent = null)
        {
            GameObject item = Object.Instantiate(prefab) as GameObject;
            item.name = poolItemName;
            item.transform.SetParent(parent);
            item.SetActive(false);

            return item;
        }
    }
}