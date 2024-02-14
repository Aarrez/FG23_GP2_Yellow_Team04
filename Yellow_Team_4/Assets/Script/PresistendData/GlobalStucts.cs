using System;
using UnityEngine;
using Sprite = UnityEngine.ProBuilder.Shapes.Sprite;

namespace GlobalStructs
{
    [Serializable]
    public class SoundVolume
    {
        public float Master = 1;
        public float Sfx = 1;
        public float Music = 1;
    }
    [Serializable]
    public struct LevelCompleteStats
    {
        public float Time;
        public int CurrencyEarned;
        public StarsEarned Starts;
    }
    
    [Serializable]
    public class SerializeableItem
    {
        public string id = "2";
        public string name = "Default";
        public ItemType type = ItemType.KayakTexture;
        public string material;
    }

    public enum StarsEarned
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3
    }

}