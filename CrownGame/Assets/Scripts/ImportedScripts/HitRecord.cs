using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRecord {
    public bool reflected;
    public IHittable hitObject;
    public int hitObjectID;

    public HitRecord() {
        reflected = false;
        hitObject = null;
        hitObjectID = -1;
    }
}
