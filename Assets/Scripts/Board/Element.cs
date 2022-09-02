using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Element : MonoBehaviour, IPoolable
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
        ((RectTransform)transform).DoAnchorPos(Vector2.zero, duration: 0.15f).SetEase(Ease.InOutSine).OnComplete<Tween>(() =>
        {
            IsInMotion = false;
            onComplete?.Invoke();
        });
    }

    public void AnimateHighlighting(Action action)
    {
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f).OnComplete(() =>
        {
            transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.15f).OnComplete(() => action());
        });
    }

    public void New()
    {
        gameObject.SetActive(true);
    }

    public void Free()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
        transform.localScale.Set(1, 1, 1);
    }
}
