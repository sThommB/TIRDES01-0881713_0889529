using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace firstGame.Scripts
{
    abstract class Instruction //used by repeat, for, wait, semicolon and createasteroid class
    {
        public abstract InstructionResult Execute(float dt);
        public abstract Instruction Reset();

        static public Instruction operator +(Instruction A, Instruction B)//operator overloading (+), take members of Instruction and return Semicolon(also member of Instruction)
        {
            return new Semicolon(A, B);
        }
    }
}
