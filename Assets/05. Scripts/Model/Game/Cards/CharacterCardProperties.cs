using System;
using UnityEditor;

namespace MainGame {
    [Serializable]
    public struct CharacterCardProperties {
        public CharacterType CharacterType;
        public int Strength;
        public int Attack;
        public int Health;
    }
}