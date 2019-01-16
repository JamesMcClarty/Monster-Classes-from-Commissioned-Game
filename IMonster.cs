using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster
{
    void DamageHealth();
    float GetSpeed();
    int ReturnPoints();

    void Idle();
    void Attack();
    void Move();
    void TakeDamage();
    void GetShoved();
    void Die();
}
