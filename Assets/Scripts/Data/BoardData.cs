using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardData : Data<BoardConfig>
{
    public int BoardWidth{get; private set;}
    public int BoardHeight{get; private set;}

    public override void SetFromConfig (BoardConfig config)
    {
        BoardWidth = config.BoardWidthInTiles;
        BoardHeight = config.BoardHeightInTiles;
    }
}
