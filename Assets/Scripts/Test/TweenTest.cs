using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenTest : MonoBehaviour
{
    public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartMoving());
    }
    
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(1);
        rectTransform.DoAnchorPosY(100, 1).SetEase(Ease.Linear);
        StartCoroutine(Move());
    }

    IEnumerator Move() {
        yield return new WaitForSeconds(1);
        rectTransform.DoAnchorPosY(200, 1).SetEase(Ease.Linear);
        
        StartCoroutine(Move());
    }
}
