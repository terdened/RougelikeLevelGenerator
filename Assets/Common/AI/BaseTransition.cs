using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Common.AI
{
    public abstract class BaseTransition
    {
        protected GameObject Self { get; set; }

        protected virtual string TargetState => "Not Defined";

        public BaseTransition(GameObject self)
        {
            Self = self;
        }

        public string Check()
        {
            return IsTriggered() ? TargetState : null;
        }

        protected abstract bool IsTriggered();

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
