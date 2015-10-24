using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace firstGame.Scripts
{
    class Wait : Instruction
    {
        float timeToWait;
        Func<float> getTimeToWait;//this can also be a normal float
        public Wait(Func<float> getTimeToWait)
        {
            this.timeToWait = getTimeToWait();
            this.getTimeToWait = getTimeToWait;
        }

        public override InstructionResult Execute(float dt)
        {
            timeToWait -= dt;
            if (timeToWait <= 0.0f)
                return InstructionResult.Done;
            else
                return InstructionResult.Running;
        }

        public override Instruction Reset()
        {
            return new Wait(getTimeToWait);
        }
    }
}
