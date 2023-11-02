using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision
{
    public Guid IteractionId { get; set; }
    public string WhatColide { get; set; }
    public string WhereColide { get; set; }
    public override bool Equals(object obj)
    {
        return this.IteractionId.Equals(IteractionId);
    }
    public override int GetHashCode()
    {
        return IteractionId.GetHashCode();
    }
}
