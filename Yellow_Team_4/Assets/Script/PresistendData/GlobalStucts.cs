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
    public struct LevelCompleteStats
    {
        public float Time;
        public int CurrencyEarned;
        public StarsEarned Starts;
    }

    public enum StarsEarned
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3
    }

}