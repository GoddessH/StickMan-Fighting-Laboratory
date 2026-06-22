using System;
using UnityEngine;

public class ProductContext
{
    // 
    public Vector2 Direction { get; set; }
    public Action<HurtPoint> OnDoDamages { get; set; }

    public ProductContext() { }

    public ProductContext(ProductContext other)
    {
        this.Direction = other.Direction;
        this.OnDoDamages = other.OnDoDamages;
    }
}
