using System.Collections.Generic;

namespace Assets.Common.AI
{
    public interface IState
    {
        List<BaseTransition> Transitions { get; set; }

        string Name { get; }

        void HandleState();

        void OnEnter();

        void OnExit();
    }
}
