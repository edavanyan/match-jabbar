using System;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Image directionDebug;

    private Element _element;
    public Element Element => _element;
    public bool Empty => Element == null;
    public bool CanAccept {get; set;}

    public Direction CurrentDirection {get; private set;}

    public int Col {get; set;}
    public int Row {get; set;}

    private void Awake() {
        ResetDirection();
    }

    public void SetElement(Element element, bool instant = false, Action onComplete = null) {
        this._element = element;
        if (instant) {
            element.SetToTile(this);
        } else {
            if (Row < 5 && !element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
            }
            element.MoveToTile(this, onComplete);
        }
    }

    public void SetDirectionTo(Tile other) {
        if (other.Col == Col && other.Row == Row) {
            directionDebug.sprite = GameController.Instance.GameService.Textures.GetSpriteByName(name: "circle_arrow");
            CurrentDirection = Direction.Self;
        } else if (other.Row < Row) {
            directionDebug.sprite = GameController.Instance.GameService.Textures.GetSpriteByName(name: "down_arrow");
            SetEmpty();
            CurrentDirection = Direction.Down;            
        } else if (other.Col > Col) {
            directionDebug.sprite = GameController.Instance.GameService.Textures.GetSpriteByName(name: "right_arrow");
            SetEmpty();
            CurrentDirection = Direction.Right;
        } else if (other.Col < Col) {
            directionDebug.sprite = GameController.Instance.GameService.Textures.GetSpriteByName(name: "left_arrow");
            SetEmpty();
            CurrentDirection = Direction.Left;
        } else {
        }
    }

    public void ResetDirection () {
        CurrentDirection = Direction.None;
    }

    public void SetEmpty() {
        this._element = null;
    }

    public enum Direction {
        Left,
        Right,
        Down, 
        Self,
        None
    }
}
