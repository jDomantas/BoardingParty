using BoardingParty.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.AI
{
    interface FighterAI
    {
        Vector? Move(Fighter fighter);
        Vector? Strike(Fighter fighter);
    }
}
