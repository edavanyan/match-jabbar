using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBoard : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    private Tile[,] _board;
    private int _width;
    private int _height;

    [SerializeField] private Element elementPrefab;
    [SerializeField] private Tile tilePrefab;

    private Pool<Element> _elementPool;

    private readonly List<Tile> _matchingList = new List<Tile>();
    private readonly List<Tile> _matchingTiles = new List<Tile>();
    private readonly List<Tile> _tempListForNeighbours = new List<Tile>();

    private Vector2 _pressedTile = new Vector2(-1, -1);

    public void CreateGameBoard()
    {
        _elementPool = new Pool<Element>(elementPrefab);

        _width = GameController.Instance.GameService.Data.BoardData.BoardWidth;
        _height = GameController.Instance.GameService.Data.BoardData.BoardHeight;

        _board = new Tile [_width, _height * 2];

        var offset = 5;
        var startX = 720 - offset * 2;
        var startY = 120;
        var tileSize = 120;
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height * 2; j++)
            {
                var tile = GameObject.Instantiate<Tile>(tilePrefab, transform, false);
                tile.Col = i;
                tile.Row = j;
                var pos = new Vector3(x: startX + ((tileSize + offset) * i), startY + (tileSize + offset) * j, 0);
                tile.transform.position = pos;
                _board[i, j] = tile;

                var item = _elementPool.NewItem();
                item.Sprite =
                    GameController.Instance.GameService.Textures.GetSpriteByIndex(UnityEngine.Random.Range(0, 5));
                SetElementOnTile(item, tile);
                if (j >= _height)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        StartCoroutine(StartGame());

    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(MoveElements());
    }

    private void SetElementOnTile(Element element, Tile tile)
    {
        SetElementOnTile(element, tile.Col, tile.Row);
    }

    private void SetElementOnTile(Element element, int col, int row)
    {
        if (!_board[col, row].Empty)
        {
            _elementPool.DestoryItem(_board[col, row].Element);
        }

        _board[col, row].SetElement(element, true);
    }

    private void RemoveElementFromTile(Tile tile)
    {
        if (tile.Empty)
        {
            //cross match
            return;
        }

        _elementPool.DestoryItem(tile.Element);
        tile.SetDirectionTo(tile);
        tile.SetEmpty();
    }

    private void FindHorizontalMatches()
    {
        for (var i = 0; i < _height; i++)
        {
            for (var j = 0; j < _width; j++)
            {
                _board[j, i].ResetDirection();
                if (_board[j, i].Empty)
                {
                    continue;
                }

                if (_matchingList.Count == 0)
                {
                    _matchingList.Add(_board[j, i]);
                }
                else if (_board[j, i].Element.Sprite.name == _matchingList[0].Element.Sprite.name)
                {
                    _matchingList.Add(_board[j, i]);
                }
                else
                {
                    CheckIfMatchDetected(j, i);
                }
            }

            CheckIfMatchDetected();
        }

        _matchingList.Clear();
    }

    private void FindVerticalMatches()
    {
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                if (_board[i, j].Empty)
                {
                    continue;
                }

                if (_matchingList.Count == 0)
                {
                    _matchingList.Add(_board[i, j]);
                }
                else if (_board[i, j].Element.Sprite.name == _matchingList[0].Element.Sprite.name)
                {
                    _matchingList.Add(_board[i, j]);
                }
                else
                {
                    CheckIfMatchDetected(i, j);
                }
            }

            CheckIfMatchDetected();
        }

        _matchingList.Clear();
    }

    private void CheckIfMatchDetected(int x = -1, int y = -1)
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

        _matchingList.Add(_board[x, y]);
    }

    private void MatchDetected(List<Tile> match)
    {
        foreach (var tile in match)
        {
            _matchingTiles.Add(tile);
        }
    }

    private IEnumerator MoveElements()
    {
        yield return new WaitForSeconds(seconds: 0.5f);
        while (DetermineTileDirections())
        {
            yield return new WaitForSeconds(seconds: 0.25f);
        }

        CheckForMatch();
    }

    private bool CheckForMatch()
    {
        FindHorizontalMatches();
        FindVerticalMatches();
        for (var i = 0; i < _matchingTiles.Count; i++)
        {
            RemoveElementFromTile(_matchingTiles[i]);
        }

        if (_matchingTiles.Count > 0)
        {
            _matchingTiles.Clear();
            StartCoroutine(MoveElements());
            return true;
        }

        return false;
    }

    private void DebugGame()
    {
        DetermineTileDirections();
        CheckForMatch();
    }

    private bool DetermineTileDirections()
    {
        var moveFound = false;
        for (var j = 1; j < _height * 2; j++)
        {
            for (var i = 0; i < _width; i++)
            {
                if (_board[i, j - 1].Empty)
                {
                    if (!_board[i, j].Empty)
                    {
                        if (j <= _height)
                        {
                            moveFound = true;
                        }

                        _board[i, j - 1].SetElement(_board[i, j].Element);
                    }

                    _board[i, j].SetDirectionTo(_board[i, j - 1]);
                    _board[i, j].SetEmpty();
                }
            }
        }

        for (var j = 1; j < _height * 2; j++)
        {
            for (var i = 0; i < _width; i++)
            {
                var neighbours = GetNeighbours(_board[i, j]);
                foreach (var tile in neighbours)
                {
                    if (tile.Empty && tile.CurrentDirection == Tile.Direction.Down)
                    {
                        if (!_board[i, j].Empty)
                        {
                            if (j <= _height)
                            {
                                moveFound = true;
                            }

                            tile.SetElement(_board[i, j].Element);
                        }

                        _board[i, j].SetEmpty();
                        break;
                    }
                }
            }
        }

        for (var j = _height; j < _height * 2; j++)
        {
            for (var i = 0; i < _width; i++)
            {
                if (_board[i, j].Empty)
                {
                    var item = _elementPool.NewItem();
                    item.Sprite =
                        GameController.Instance.GameService.Textures.GetSpriteByIndex(UnityEngine.Random.Range(0, 5));
                    SetElementOnTile(item, _board[i, j]);
                    item.gameObject.SetActive(false);
                }
            }
        }

        return moveFound;
    }

    private List<Tile> GetNeighbours(Tile tile)
    {
        _tempListForNeighbours.Clear();
        if (tile.Col > 0)
        {
            _tempListForNeighbours.Add(_board[tile.Col - 1, tile.Row]);
        }

        if (tile.Col < _width - 1)
        {
            _tempListForNeighbours.Add(_board[tile.Col + 1, tile.Row]);
        }

        if (_tempListForNeighbours.Count > 1)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                var tmpForSwap = _tempListForNeighbours[0];
                _tempListForNeighbours[index: 0] = _tempListForNeighbours[1];
                _tempListForNeighbours[1] = tmpForSwap;
            }
        }

        return _tempListForNeighbours;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (Math.Abs(eventData.delta.x) > 2 || Math.Abs(eventData.delta.y) > 2)
        {
            if (_pressedTile.x >= 0)
            {
                if (Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                {
                    if (eventData.delta.x > 0)
                    {
                        SwapRight(_board[(int)_pressedTile.x, (int)_pressedTile.y]);
                    }
                    else
                    {
                        SwapLeft(_board[(int)_pressedTile.x, (int)_pressedTile.y]);
                    }
                }
                else
                {
                    if (eventData.delta.y > 0)
                    {
                        SwapUp(_board[(int)_pressedTile.x, (int)_pressedTile.y]);
                    }
                    else
                    {
                        SwapDown(_board[(int)_pressedTile.x, (int)_pressedTile.y]);
                    }
                }

                _pressedTile.Set(-1, -1);
            }
        }
    }

    private void SwapRight(Tile tile, bool checkMatch = true)
    {
        if (tile.Col < _width - 1)
        {
            var element = tile.Element;
            var tileToSwap = _board[tile.Col + 1, tile.Row];
            var elementToSwap = tileToSwap.Element;
            tileToSwap.SetEmpty();
            tile.SetElement(elementToSwap);
            tileToSwap.SetElement(element, false, () =>
            {
                if (checkMatch && !CheckForMatch())
                {
                    SwapLeft(tileToSwap, false);
                }

            });
        }
    }

    private void SwapLeft(Tile tile, bool checkMatch = true)
    {
        if (tile.Col > 0)
        {
            var element = tile.Element;
            var tileToSwap = _board[tile.Col - 1, tile.Row];
            tile.SetElement(tileToSwap.Element);
            tileToSwap.SetElement(element, false, () =>
            {
                if (checkMatch && !CheckForMatch())
                {
                    SwapRight(tileToSwap, false);
                }

            });
        }
    }

    private void SwapDown(Tile tile, bool checkMatch = true)
    {
        if (tile.Row > 0)
        {
            var element = tile.Element;
            var tileToSwap = _board[tile.Col, tile.Row - 1];
            tile.SetElement(tileToSwap.Element);
            tileToSwap.SetElement(element, false, () =>
            {
                if (checkMatch && !CheckForMatch())
                {
                    SwapUp(tileToSwap, false);
                }

            });
        }
    }

    private void SwapUp(Tile tile, bool checkMatch = true)
    {
        if (tile.Row < _height - 1)
        {
            var element = tile.Element;
            var tileToSwap = _board[tile.Col, tile.Row + 1];
            tile.SetElement(tileToSwap.Element);
            tileToSwap.SetElement(element, false, () =>
            {
                if (checkMatch && !CheckForMatch())
                {
                    SwapDown(tileToSwap, false);
                }

            });
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var pressedObject = eventData.pointerCurrentRaycast.gameObject;
        if (pressedObject.CompareTag("Element"))
        {
            var tileCol = pressedObject.GetComponent<Element>().Tile.Col;
            var tileRow = pressedObject.GetComponent<Element>().Tile.Row;
            _pressedTile.Set(tileCol, tileRow);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressedTile.Set(-1, -1);
    }
}
