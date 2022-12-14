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

    private ComponentPool<Element> _elementPool;

    private CommandPool<SwipeCommand> _commandPool;

    private readonly List<Tile> _tempListForNeighbours = new List<Tile>();

    private Vector2 _pressedTile = new Vector2(-1, -1);
    
    private bool _isInMovingState = false;

    private CommandExecuter _commandExecuter;

    private readonly List<Element> _movingElemets = new List<Element>();
    private BoardHelper _boardHelper;

    public void CreateGameBoard()
    {
        _elementPool = new ComponentPool<Element>(elementPrefab);
        _commandPool = new CommandPool<SwipeCommand>(new SwipeCommand());
        _commandExecuter = new CommandExecuter();
        
        _width = GameController.Instance.Data.BoardData.BoardWidth;
        _height = GameController.Instance.Data.BoardData.BoardHeight;

        _board = new Tile [_width, _height + 2];

        var offset = 0;
        elementPrefab.transform.localScale = Vector3.one;

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height + 2; j++)
            {
                var tile = Instantiate(tilePrefab, transform, false);
                tile.Col = i;
                tile.Row = j;
                
                _board[i, j] = tile;

                var item = _elementPool.NewItem();
                item.Sprite = GameController.Instance.Textures.GetSpriteByIndex(UnityEngine.Random.Range(0, 5));
                SetElementOnTile(item, tile);
                if (j >= _height)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        _boardHelper = new BoardHelper(_board, _width, _height);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.3f);
        
        var canvasTransform = GameController.Instance.Canvas.transform as RectTransform;

        var tileTransform = tilePrefab.transform as RectTransform;
        tileTransform.localScale = Vector3.one;
        var tileSize = tileTransform.rect;
        var startX = -_width * tileSize.width / 2f + tileSize.width / 2f;
        var startY = -_height * tileSize.height / 2f + tileSize.height / 2f;
        
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height + 2; j++)
            {
                var tile = _board[i, j];
                var pos = new Vector3(x: startX + ((tileSize.width ) * i), startY + (tileSize.height) * j, -20);
                ((RectTransform)tile.transform).anchoredPosition = pos;
                transform.localScale = Vector3.one;
            }
        }
        
        yield return new WaitForSeconds(0.2f);
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

    private IEnumerator MoveElements()
    {
        _isInMovingState = true;
        yield return new WaitForSeconds(seconds: 0.33f);
        while (DetermineTileDirections())
        {
            yield return new WaitForSeconds(seconds: 0.15f);
        }

        foreach (var movingElemet in _movingElemets)
        {
            movingElemet.transform.localScale.Set(1, 1, 1);
            movingElemet.AnimateDrop();
        }
        _movingElemets.Clear();
        _isInMovingState = false;
        
        if (!CheckForMatch())
        {
            var shuffleBoard = _boardHelper.ShuffleBoard(_board);
            if (shuffleBoard)
            {
                yield return new WaitForSeconds(0.15f);
                CheckForMatch();
            }
        }
    }

    private bool CheckForMatch()
    {
        var matchingTiles = _boardHelper.CheckForMatch(_board);
        foreach (var tile in matchingTiles)
        {
            tile.Element.AnimateHighlighting(() => RemoveElementFromTile(tile));
        }

        if (matchingTiles.Count > 0)
        {
            matchingTiles.Clear();
            if (!_isInMovingState)
            {
                StartCoroutine(MoveElements());
            }

            return true;
        }

        return false;
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
    
    private bool DetermineTileDirections()
    {
        var moveFound = false;
        for (var j = 1; j < _height + 2; j++)
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

                        var element = _board[i, j].Element;
                        _board[i, j - 1].SetElement(element);
                        element.AnimateScaleY();
                        _movingElemets.Add(element);
                    }

                    _board[i, j].SetDirectionTo(_board[i, j - 1]);
                    _board[i, j].SetEmpty();
                }
            }
        }

        for (var j = 1; j < _height + 2; j++)
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

                            var element = _board[i, j].Element;
                            tile.SetElement(element);
                            element.AnimateScaleX();
                            _movingElemets.Add(element);
                        }

                        _board[i, j].SetEmpty();
                        break;
                    }
                }
            }
        }

        for (var j = _height; j < _height + 2; j++)
        {
            for (var i = 0; i < _width; i++)
            {
                if (_board[i, j].Empty)
                {
                    var item = _elementPool.NewItem();
                    item.Sprite =
                        GameController.Instance.Textures.GetSpriteByIndex(UnityEngine.Random.Range(0, 5));
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
        float minDelta = 1;
        if (Math.Abs(eventData.delta.x) > minDelta || Math.Abs(eventData.delta.y) > minDelta)
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
            var tileToSwap = _board[tile.Col + 1, tile.Row];
            var swipeCommand = _commandPool.NewItem();
            if (tile.Empty || 
                tileToSwap.Empty ||
                tile.Element.IsInMotion || 
                tileToSwap.Element.IsInMotion)
            {
                return;
            }
            
            swipeCommand.Set(tile, tileToSwap, () =>
            {
                GameController.Instance.Events.FireEvent(typeof(CommandExecutionCompleteEvent));
                if (checkMatch)
                {
                    if (!(_boardHelper.CheckMatchesOnTile(tile, _board) ||
                        _boardHelper.CheckMatchesOnTile(tileToSwap, _board)))
                    {
                        SwapLeft(tileToSwap, false);
                    }
                    else if (!_isInMovingState)
                    {
                        CheckForMatch();
                    }
                }
            });
            
            _commandExecuter.RegisterCommand(swipeCommand);
        }
    }

    private void SwapLeft(Tile tile, bool checkMatch = true)
    {
        if (tile.Col > 0)
        {
            var tileToSwap = _board[tile.Col - 1, tile.Row];
            var swipeCommand = _commandPool.NewItem();
            if (tile.Empty || 
                tileToSwap.Empty ||
                tile.Element.IsInMotion || 
                tileToSwap.Element.IsInMotion)
            {
                return;
            }
            
            swipeCommand.Set(tile, tileToSwap, () =>
            {
                GameController.Instance.Events.FireEvent(typeof(CommandExecutionCompleteEvent));
                if (checkMatch)
                {
                    if (!(_boardHelper.CheckMatchesOnTile(tile, _board) ||
                        _boardHelper.CheckMatchesOnTile(tileToSwap, _board)))
                    {
                        SwapRight(tileToSwap, false);
                    }
                    else if (!_isInMovingState)
                    {
                        CheckForMatch();
                    }
                }
            });
            
            _commandExecuter.RegisterCommand(swipeCommand);
        }
    }

    private void SwapDown(Tile tile, bool checkMatch = true)
    {
        if (tile.Row > 0)
        {
            var tileToSwap = _board[tile.Col, tile.Row - 1];
            var swipeCommand = _commandPool.NewItem();
            if (tile.Empty || 
                tileToSwap.Empty ||
                tile.Element.IsInMotion || 
                tileToSwap.Element.IsInMotion)
            {
                return;
            }
            
            swipeCommand.Set(tile, tileToSwap, () =>
            {
                GameController.Instance.Events.FireEvent(typeof(CommandExecutionCompleteEvent));
                if (checkMatch)
                {
                    if (!(_boardHelper.CheckMatchesOnTile(tile, _board) ||
                        _boardHelper.CheckMatchesOnTile(tileToSwap, _board)))
                    {
                        SwapUp(tileToSwap, false);
                    }
                    else if (!_isInMovingState)
                    {
                        CheckForMatch();
                    }
                }
            });
            
            _commandExecuter.RegisterCommand(swipeCommand);
        }
    }

    private void SwapUp(Tile tile, bool checkMatch = true)
    {
        if (tile.Row < _height - 1)
        {
            var tileToSwap = _board[tile.Col, tile.Row + 1];
            var swipeCommand = _commandPool.NewItem();
            if (tile.Empty || 
                tileToSwap.Empty ||
                tile.Element.IsInMotion || 
                tileToSwap.Element.IsInMotion)
            {
                return;
            }
            
            swipeCommand.Set(tile, tileToSwap, () =>
            {
                GameController.Instance.Events.FireEvent(typeof(CommandExecutionCompleteEvent));
                if (checkMatch)
                {

                    if (!(_boardHelper.CheckMatchesOnTile(tile, _board) ||
                        _boardHelper.CheckMatchesOnTile(tileToSwap, _board)))
                    {
                        SwapDown(tileToSwap, false);
                    }
                    else if (!_isInMovingState)
                    {
                        CheckForMatch();
                    }
                }

            });
            
            _commandExecuter.RegisterCommand(swipeCommand);
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
        else if (pressedObject.CompareTag("Tile"))
        {
            var tileCol = pressedObject.GetComponent<Tile>().Col;
            var tileRow = pressedObject.GetComponent<Tile>().Row;
            _pressedTile.Set(tileCol, tileRow);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressedTile.Set(-1, -1);
    }
}
