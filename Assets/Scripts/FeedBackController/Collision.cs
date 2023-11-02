using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions
{
    public string WhatColide { get; set; }
    public string HandCollision { get; set; }
    public bool IsActive { get; set; }
    public override bool Equals(object obj)
    {
        return this.WhatColide.Equals(WhatColide);
    }
    public override int GetHashCode()
    {
        return WhatColide.GetHashCode();
    }
}
