using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class Element : MonoBehaviour, IPoolable
{
    [FormerlySerializedAs("_image")] [SerializeField]
    private Image image;

    private Dictionary<string, Color> colors = new Dictionary<string, Color>()
    {
        { "red", Color.red },
        { "yellow", Color.yellow },
        { "green", Color.green },
        { "blue", new Color(38f / 256, 172f / 256, 255f / 256) },
        { "orange", new Color(252f / 256, 186f / 256, 3f / 256) }
    };

    public Sprite Sprite
    {
        set
        {
            image.sprite = value;
            var particleMain = particle.main;
            particleMain.startColor = new ParticleSystem.MinMaxGradient(colors[value.name]);
        } 
        get{return image.sprite;}
    }
    
    public Tile Tile { get; private set; }
    private RectTransform _rectTransform;

    [SerializeField] private Animator hintAnimator;

    public bool IsInMotion {get; private set;}

    [SerializeField]private ParticleSystem particle;

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
        IsInMotion = true;
        _rectTransform.DoAnchorPos(Vector2.zero, duration: 0.15f).SetEase(Ease.InOutSine).OnComplete<Tween>(() =>
        {
            IsInMotion = false;
            onComplete?.Invoke();
        });
    }

    public void AnimateHighlighting(Action action)
    {
        particle.Play();
        IsInMotion = true;
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f).OnComplete(() =>
        {
            transform.DOScale(new Vector3(0f, 0f, 1.0f), 0.15f).OnComplete(() =>
            {
                IsInMotion = false;
                action();
            });
        });
    }

    public void New()
    {
        hintAnimator.enabled = false;
        gameObject.SetActive(true);
        transform.localScale.Set(1, 1, 1);
    }

    public void AnimateHint()
    {
        hintAnimator.enabled = true;
    }

    public void StopHintAnimation()
    {
        hintAnimator.enabled = false;
    }

    public void Free()
    {
        IsInMotion = false;
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public void AnimateDrop()
    {
        _rectTransform.DoAnchorPosY(-1, 0.05f).OnComplete<Tween>(() =>
        {
            _rectTransform.DoAnchorPosY(0, 0.05f).SetEase(Ease.OutBack);
        });
        if (_rectTransform.localScale.x > 1.0f)
        {
            _rectTransform.DOScale(1, 0.1f);
        }

        if (_rectTransform.localScale.y > 1.0f)
        {
            _rectTransform.DOScale(1f, 0.1f);
        }
    }

    public void AnimateScaleX()
    {
        if (_rectTransform.localScale.x > 1.0f)
        {
            _rectTransform.DOScale(1.0f, 0.1f).SetEase(Ease.OutSine);
        }

        _rectTransform.DOScaleX(1.1f, 0.1f).SetEase(Ease.OutSine);
    }

    public void AnimateScaleY()
    {
        if (_rectTransform.localScale.y > 1.0f)
        {
            _rectTransform.DOScale(1.0f, 0.1f).SetEase(Ease.OutSine);
        }

        _rectTransform.DOScaleY(1.1f, 0.1f).SetEase(Ease.OutSine);
    }
}
