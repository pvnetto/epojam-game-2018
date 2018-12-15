using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable  {

    void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce);

    bool isAlly(int id);

}
