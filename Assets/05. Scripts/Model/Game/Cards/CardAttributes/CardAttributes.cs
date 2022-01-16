using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public interface IPointable {

    }

    public interface IAttackable: IPointable {
        public CardAlterableProperty<int> Health { get; set; }
    }
}
