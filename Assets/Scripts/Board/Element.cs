using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    [FormerlySerializedAs("_image")] [SerializeField]
    private Image image;
    public Sprite Sprite{set{image.sprite = value;} get{return image.sprite;}}
    [HideInInspector]public Tile Tile { get; private set; }
    private RectTransform _transform;

    public bool IsInMotion {get; private set;}

    private void Awake() {
        _transform = transform as RectTransform;
    }

    public void SetToTile(Tile tile) {
        this.Tile = tile;
        transform.SetParent(tile.transform, false);
        ((RectTransform)transform).anchoredPosition.Set(0, 0);
    }

    public void MoveToTile(Tile tile, Action onComplete) {
        this.Tile = tile;
        transform.SetParent(tile.transform);
        IsInMotion = true;
        ((RectTransform)transform).DoAnchorPos(Vector2.zero, duration: 0.25f).SetEase(Ease.Linear).OnComplete<Tween>(() =>
        {
            IsInMotion = false;
            onComplete?.Invoke();
        });
    }

    public void Move() {
        var rectPos = _transform.anchoredPosition;
        if (Tile.CurrentDirection == Tile.Direction.Down) {
            rectPos.y -= 10;
        } else if (Tile.CurrentDirection == Tile.Direction.Right) {
            rectPos.x += 10;
        } else if (Tile.CurrentDirection == Tile.Direction.Left) {
            rectPos.x -= 10;
        }
        _transform.anchoredPosition = rectPos;
    }
}
