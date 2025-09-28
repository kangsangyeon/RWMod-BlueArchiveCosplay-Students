using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAlphaHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var img = GetComponent<Image>();
        if (img)
            img.alphaHitTestMinimumThreshold = 1;
    }
}
