using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class RandomizeIcons : MonoBehaviour
    {
        public List<Image> IconList;
        public List<Sprite> SpriteList;
        public float minSize;
        public float maxSize;

        [ContextMenu("Do Something")]
        public void Do()
        {
            foreach(var icon in IconList)
            {
                int randomIndex = Random.Range(0, SpriteList.Count);
                icon.sprite = SpriteList[randomIndex];
                float randomSize = Random.Range(minSize, maxSize);
                icon.transform.localScale = new Vector3((Random.Range(0, 2) == 0 ? -1 : 1) * randomSize, randomSize, 0);
                icon.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            }
        }
    }
}
