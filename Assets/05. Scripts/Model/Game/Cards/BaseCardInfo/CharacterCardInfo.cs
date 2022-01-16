using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public enum CharacterType {
        Guard,
        Warrior,
        Shooter,
        Doctor,
        Engineer
    }

    public enum BuffType {
        Taunt
    }
    [ES3Serializable]
    public abstract class CharacterCardInfo : CardInfo, IAttackable
    {
        [ES3Serializable]
        public CharacterType CharacterType { get; set; }
        [ES3Serializable]
        public CardAlterableProperty<int> Strength { get; set; } //primitive: maximum strength
        [ES3Serializable]
        public CardAlterableProperty<int> Health { get; set; } //primitive: maximum health
        [ES3Serializable]
        public CardAlterableProperty<int> Attack { get; set; }
       
        public CharacterCardInfo() { }
        public CharacterCardInfo(CardProperties attributes) : base(attributes) {
            
            this.CharacterType = attributes.CharacterProperties.CharacterType;
            this.Strength = new CardAlterableProperty<int>(attributes.CharacterProperties.Strength);
            this.Health = new CardAlterableProperty<int>(attributes.CharacterProperties.Health);
            this.Attack = new CardAlterableProperty<int>(attributes.CharacterProperties.Attack);

        }

        public override void OnResetToPrimitive() {
            base.OnResetToPrimitive();
            Attack.ResetToPrimitive();
            
        }
    }
}
