using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace firstGame.Scripts
{
    class For : Instruction
    {
        int start, end, i;
        Func<int, Instruction> getBody;
        Instruction body;
        public For(int start, int end, Func<int, Instruction> getBody)//func<> is a function, it takes an int and return an Instruction 
        {
            this.i = start;
            this.start = start;
            this.end = end;
            this.getBody = getBody;
            this.body = getBody(i);//this is a Semicolon, made of Wait and CreateAsteroid
        }

        public override InstructionResult Execute(float dt)
        {
            if (i >= end)
                return InstructionResult.Done;
            else
            {
                switch (body.Execute(dt))
                {
                    case InstructionResult.Done:
                        i++;
                        body = getBody(i);
                        return InstructionResult.Running;
                    case InstructionResult.DoneAndCreateAsteroid:
                        i++;
                        body = getBody(i);
                        return InstructionResult.RunningAndCreateAsteroid;
                    case InstructionResult.Running:
                       return InstructionResult.Running;
                    case InstructionResult.RunningAndCreateAsteroid: 
                        return InstructionResult.RunningAndCreateAsteroid;        //code nevers returns RunningAndCreateAsteroid.
                }
                return InstructionResult.Done;
            }
        }

        public override Instruction Reset()
        {
            return new For(start, end, getBody);
        }
    }
}
