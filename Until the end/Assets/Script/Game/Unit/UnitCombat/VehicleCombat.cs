using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCombat : UnitCombat
{
    [SerializeField]protected GameObject barbette;//炮台
    [SerializeField]protected float targetAngle;//炮塔角度
    private void Update()
    {
        if (!unit.isAttack && barbette.transform.rotation != transform.rotation)
        {
            barbette.transform.rotation = Quaternion.RotateTowards(
                 barbette.transform.rotation, transform.rotation, weapon._rotateSpeed * Time.deltaTime);
        }//不在攻击
        Attack();
    }
    protected override void AttackTarget(UnitAttribute target)
    {
        base.AttackTarget(target);
        barbette.transform.rotation=RotateHelper.RotateToUnit(barbette.transform,target.transform,
                                                    weapon._rotateSpeed);
    }
    protected override void TowardsTarget(UnitAttribute target)
    {
        Quaternion Angle = RotateHelper.GetRotateAngle(barbette.transform.position, target.transform.position); ;
        barbette.transform.rotation = RotateHelper.RotateToUnit(barbette.transform, target.transform, weapon._rotateSpeed);
        if (Quaternion.Angle(barbette.transform.rotation, Angle) < 10)
        {
            isTowardsTarget = true;
        }
        else isTowardsTarget = false;

    }
}
