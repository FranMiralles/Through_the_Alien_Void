using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vida : MonoBehaviour
{
    public Image bloodyEfectImage;

    private float value;
    private float r, g, b, a, d;

    void Start()
    {
        value = 100;
        r = bloodyEfectImage.color.r;
        g = bloodyEfectImage.color.g;
        b = bloodyEfectImage.color.b;
        a = 0f;
        d = 100 - value;
    }

    // Update is called once per frame
    void Update()
    {
        if (value != 100) {
            a = 1 - (0.01f * (d));
        }
        Math.Clamp(a,0,1f);
        changeColor();
    }

    private void changeColor()
    {
        Color c = new Color(r,g,b,a);
        bloodyEfectImage.color = c;
    }

    public void damage(float dmg) 
    { 
        if (dmg > value) value = 0;
        else value -= dmg;
    }
    public void heal(float h) 
    { 
        if (h > (100 - value)) value = 100;
        else value += h;
    }
}
