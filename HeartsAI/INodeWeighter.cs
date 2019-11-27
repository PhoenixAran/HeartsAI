using Hearts.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartsAI
{
    public interface INodeWeighter
    {
        void WeightNodeTree( List<Card> hand, Trick currentTrick, Node nodeTree );

    }
}
