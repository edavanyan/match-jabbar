
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardHelper
{
    private readonly int _width;
    private readonly int _height;
    private readonly Tile[,] _pseudoBoard;

    private readonly List<Tile> _matchingTiles = new List<Tile>();
    private readonly List<Tile> _matchingList = new List<Tile>();
    
    private List<Vector2> allTiles = new List<Vector2>();
    private List<int> freeIndices = new List<int>();
    public BoardHelper(Tile[,] board, int width, int height)
    {
        _width = width;
        _height = height;
        _pseudoBoard = new Tile[_width, _height];
        CopyBoard(board);
    }

    public bool ShuffleBoard(Tile[,] board)
    {
        bool shuffled = false;
        while (!CheckForHints(board))
        {
            ShuffleElements(board);
            shuffled = true;
        }

        return shuffled;
    }

    private void ShuffleElements(Tile[,] board)
    {
        allTiles.Clear();
        freeIndices.Clear();
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                freeIndices.Add(allTiles.Count);
                allTiles.Add(new Vector2(i, j));
            }
        }

        while (freeIndices.Count > 1)
        {
            var randomIndex = Random.Range(0, freeIndices.Count);
            var tile1 = allTiles[freeIndices[randomIndex]];
            freeIndices.RemoveAt(randomIndex);

            randomIndex = Random.Range(0, freeIndices.Count);
            var tile2 = allTiles[freeIndices[randomIndex]];
            freeIndices.RemoveAt(randomIndex);
            
            var element = board[(int)tile1.x, (int)tile1.y].Element;
            var tileToSwap = board[(int)tile2.x, (int)tile2.y];
            board[(int)tile1.x, (int)tile1.y].SetElement(tileToSwap.Element);
            tileToSwap.SetElement(element);
            
        }
    }

    private void CopyBoard(Tile[,] board)
    {
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                _pseudoBoard[i, j] = board[i, j];
                _pseudoBoard[i, j].Element.StopHintAnimation();
            }
        }
    }

    public bool CheckMatchesOnTile(Tile tile, Tile[,] board)
    {
        var matches = 0;
        for (int i = Math.Max(0, tile.Col - 2); i < Math.Min(_width, tile.Col + 3); i++)
        {
            if (i == tile.Col)
            {
                continue;
            }
            var tileToCheck = board[i, tile.Row];
            if (!tileToCheck.Empty && tileToCheck.Element.Sprite.name == tile.Element.Sprite.name)
            {
                matches++;
                if (matches == 2)
                {
                    break;
                }
            } 
            else
            {
                matches = 0;
            }
        }

        if (matches > 1)
        {
            return true;
        }
        
        matches = 0;
        for (int i = Math.Max(0, tile.Row - 2); i < Math.Min(_height, tile.Row + 3); i++)
        {
            if (i == tile.Row)
            {
                continue;
            }
            var tileToCheck = board[tile.Col, i];
            if (!tileToCheck.Empty && tileToCheck.Element.Sprite.name == tile.Element.Sprite.name)
            {
                matches++;
                if (matches == 2)
                {
                    break;
                }
            } 
            else
            {
                matches = 0;
            }
        }

        if (matches > 1)
        {
            return true;
        }

        return false;
    }

    private bool CheckForHints(Tile[,] board)
    {
        CopyBoard(board);
        
        for (var i = 0; i < _pseudoBoard.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < _pseudoBoard.GetLength(1) - 1; j++)
            {
                if (PseudoSwapRight(_pseudoBoard[i, j]))
                {
                    return true;
                }
                
                if (PseudoSwapUp(_pseudoBoard[i, j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool AnimateIfMatch()
    {
        var matchList = CheckForMatch(_pseudoBoard);
        if (matchList.Count > 0)
        {
            foreach (var matchTile in matchList)
            {
                matchTile.Element.AnimateHint();
            }

            return true;
        }

        return false;
    }

    private bool PseudoSwapRight(Tile tile)
    {
        if (tile.Col < _width - 1)
        {
            var element = tile.Element;
            var tileToSwap = _pseudoBoard[tile.Col + 1, tile.Row];
            _pseudoBoard[tile.Col, tile.Row].SetElement(tileToSwap.Element, true);
            tileToSwap.SetElement(element, true);

            var animateIfMatch = AnimateIfMatch();

            PseudoSwapLeft(tileToSwap);
            return animateIfMatch;
        }

        return false;
    }

    private void PseudoSwapLeft(Tile tile)
    {
        if (tile.Col > 0)
        {
            var element = tile.Element;
            var tileToSwap = _pseudoBoard[tile.Col - 1, tile.Row];
            _pseudoBoard[tile.Col, tile.Row].SetElement(tileToSwap.Element, true);
            tileToSwap.SetElement(element, true);
        }
    }

    private bool PseudoSwapUp(Tile tile)
    {
        if (tile.Row < _height - 1)
        {
            var element = tile.Element;
            var tileToSwap = _pseudoBoard[tile.Col, tile.Row + 1];
            _pseudoBoard[tile.Col, tile.Row].SetElement(tileToSwap.Element, true);
            tileToSwap.SetElement(element, true);
            
            var animateIfMatch = AnimateIfMatch();
            
            PseudoSwapDown(tileToSwap);
            return animateIfMatch;
        }

        return false;
    }

    private void PseudoSwapDown(Tile tile)
    {
        if (tile.Row > 0)
        {
            var element = tile.Element;
            var tileToSwap = _pseudoBoard[tile.Col, tile.Row - 1];
            _pseudoBoard[tile.Col, tile.Row].SetElement(tileToSwap.Element, true);
            tileToSwap.SetElement(element, true);
        }
    }

    private void FindHorizontalMatches(Tile[,] board)
    {
        for (var i = 0; i < _height; i++)
        {
            for (var j = 0; j < _width; j++)
            {
                board[j, i].ResetDirection();
                if (board[j, i].Empty)
                {
                    continue;
                }

                if (_matchingList.Count == 0)
                {
                    _matchingList.Add(board[j, i]);
                }
                else if (board[j, i].Element.Sprite.name == _matchingList[0].Element.Sprite.name)
                {
                    _matchingList.Add(board[j, i]);
                }
                else
                {
                    CheckIfMatchDetected(board, j, i);
                }
            }

            CheckIfMatchDetected(board);
        }

        _matchingList.Clear();
    }

    private void FindVerticalMatches(Tile[,] board)
    {
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                if (board[i, j].Empty)
                {
                    continue;
                }

                if (_matchingList.Count == 0)
                {
                    _matchingList.Add(board[i, j]);
                }
                else if (board[i, j].Element.Sprite.name == _matchingList[0].Element.Sprite.name)
                {
                    _matchingList.Add(board[i, j]);
                }
                else
                {
                    CheckIfMatchDetected(board, i, j);
                }
            }

            CheckIfMatchDetected(board);
        }

        _matchingList.Clear();
    }

    private void MatchDetected(List<Tile> match)
    {
        foreach (var tile in match)
        {
            _matchingTiles.Add(tile);
        }
    }

    private void CheckIfMatchDetected(Tile[,] board, int x = -1, int y = -1)
    {
        if (_matchingList.Count > 2)
        {
            MatchDetected(_matchingList);
        }

        _matchingList.Clear();
        if (x == -1 || y == -1)
        {
            return;
        }

        _matchingList.Add(board[x, y]);
    }
    
    public List<Tile> CheckForMatch(Tile[,] board)
    {
        _matchingTiles.Clear();
        FindHorizontalMatches(board);
        FindVerticalMatches(board);

        return _matchingTiles;
    }
}