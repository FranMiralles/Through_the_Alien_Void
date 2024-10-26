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
    private float r, g, b, a, dmg;

    void Start()
    {
        health = maxHealth;
        r = bloodyEfectImage.color.r;
        g = bloodyEfectImage.color.g;
        b = bloodyEfectImage.color.b;
        a = 0f;
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
        a = (float) 1 - (health / 100);
        Math.Clamp(a,0,1f);
        changeColor();
    }
    public void heal(float cure) 
    { 
        if (cure > (100 - health)) health = 100;
        else health += cure;
        a = (float) health / 100;
        Math.Clamp(a,0,1f);
        changeColor();
    }
}
