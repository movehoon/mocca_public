using System;
using UnityEngine;

namespace REEL.Animation
{
    [Serializable]
    public class Paramters
    {
        // Fields
        [SerializeField]
        private float scale = 1f;
        [SerializeField]
        private float defaultValue;
        [SerializeField]
        private float amount;
        [SerializeField]
        private Sprite changeSprite;


        // Properties
        public float DefaultValue
        {
            get { return defaultValue; }
        }

        public float GoalValue
        {
            get { return defaultValue + amount * scale; }
        }

        public Sprite ChangeSprite
        {
            get { return changeSprite; }
        }


        // Methods
        public float GetDelta()
        {
            return GoalValue - DefaultValue;
        }
    }
}