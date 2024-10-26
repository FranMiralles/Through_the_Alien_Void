using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vida : MonoBehaviour
{
    public Image bloodyEfectImage;

    private float maxHealth = 100;
    private float health;
    private float r, g, b, a, d;

    void Start()
    {
        health = maxHealth;
        r = bloodyEfectImage.color.r;
        g = bloodyEfectImage.color.g;
        b = bloodyEfectImage.color.b;
        a = 0f;
        d = 100 - health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health != 100) {
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
        if (dmg > health) health = 0;
        else health -= dmg;
    }
    public void heal(float h) 
    { 
        if (h > (100 - health)) health = 100;
        else health += h;
    }
}
