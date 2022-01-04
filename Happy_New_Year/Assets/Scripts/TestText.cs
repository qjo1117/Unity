using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TestText : MonoBehaviour
{
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOAnchorPosY(0, 5.0f).SetDelay(1.5f).SetEase(Ease.InOutElastic);

        Text txt = GetComponent<Text>();
        txt.DOText("DOTween Example", 2.0f, true, ScrambleMode.Numerals, null).SetDelay(3.0f);
    }

    void Update()
    {
        
    }
}
