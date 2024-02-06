using System;

namespace GlobalStructs
{
    [Serializable]
    public struct SoundVolume
    {
        public float Master;
        public float Sfx;
        public float Music;
    }
    [Serializable]
    public struct PlayerSettings
    {
        public SoundVolume soundVolume;
    }
    [Serializable]
    public struct Inventory
    {
        public KayaksSkins KayaksSkins;
    }

    public enum KayaksSkins
    {
        Default,
        Level1,
        Level2
    }
    [Serializable]
    public struct LevelCompleteStats
    {
        public int Level;
        public float Time;
        public int CurrencyEarned;
        public StarsEarned Starts;
    }

    public enum StarsEarned
    {
        Zero,
        One,
        Two,
        Three
    }

}