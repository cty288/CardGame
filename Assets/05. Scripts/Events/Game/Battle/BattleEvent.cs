using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame {
    public interface IBattleEvent {

    }
    public struct OnDrawCard: IBattleEvent {
        public CardInfo cardDraw;
    }

    public struct OnEnterBattleScene: IBattleEvent { }

    public struct OnLeaveBattleScene: IBattleEvent{}

    public struct OnEnemyLevelPassed : IBattleEvent {
        public GraphVertex LevelVertex;
    }

    public class OnCardTryDealt : IBattleEvent {
        public bool DealSuccess;
        public CardInfo CardInfo;
    }
    public struct OnCardDealt : IBattleEvent {
        public CardInfo CardDealt;
    }
}
