using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class FightState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        private FightToCompleteTransition _fightToCompleteTransition;

        public string Name => "Fight";

        private RoomController _roomController;

        public FightState(GameObject self, RoomController roomController)
        {
            _roomController = roomController;
            
            Transitions = new List<BaseTransition>();

            _fightToCompleteTransition = new FightToCompleteTransition(self);
            Transitions.Add(_fightToCompleteTransition);
        }

        public void HandleState() { }

        public void OnEnter() 
        {
            _roomController.CloseDoor();
        }

        public void OnExit()
        {
            _roomController.OpenDoor();
        }
    }
}
