using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyProject
{
    [CreateAssetMenu]
    public class RPSConfig : ScriptableObject
    {
        public List<RPSElementData> elements;

        public Sprite GetSpriteByType(RPSElementType elementType)
        {
            return elements.Find(element => element.type == elementType)?.sprite;
        }
    }

    [Serializable]
    public class RPSElementData
    {
        public RPSElementType type;
        [PreviewField] public Sprite sprite;
    }

}
