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
    private RectTransform _rectTransform;

    public bool IsInMotion {get; private set;}

    private void Awake() {
        _rectTransform = transform as RectTransform;
    }

    public void SetToTile(Tile tile) {
        this.Tile = tile;
        transform.SetParent(tile.transform, false);
        _rectTransform.anchoredPosition.Set(0, 0);
    }

    public void MoveToTile(Tile tile, TweenCallback onComplete) {
        Tile = tile;
        transform.SetParent(tile.transform);
        _rectTransform.DoAnchorPos(Vector2.zero, duration: 0.2f).SetEase(Ease.InOutSine).OnComplete<Tween>(onComplete);
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

    public void AnimateDrop()
    {
        _rectTransform.DoAnchorPosY(-1, 0.05f).OnComplete<Tween>(() =>
        {
            _rectTransform.DoAnchorPosY(0, 0.05f).SetEase(Ease.OutBack);
        });
        if (_rectTransform.localScale.x > 1.0f)
        {
            _rectTransform.DOScaleX(1f, 0.1f);
        }

        if (_rectTransform.localScale.y > 1.0f)
        {
            _rectTransform.DOScaleY(1f, 0.1f);
        }
    }

    public void AnimateScaleX()
    {
        if (_rectTransform.localScale.x > 1.0f)
        {
            _rectTransform.DOScaleY(1.0f, 0.1f).SetEase(Ease.OutSine);
        }

        _rectTransform.DOScaleX(1.1f, 0.1f).SetEase(Ease.OutSine);
    }

    public void AnimateScaleY()
    {
        if (_rectTransform.localScale.y > 1.0f)
        {
            _rectTransform.DOScaleX(1.0f, 0.1f).SetEase(Ease.OutSine);
        }

        _rectTransform.DOScaleY(1.1f, 0.1f).SetEase(Ease.OutSine);
    }
}
