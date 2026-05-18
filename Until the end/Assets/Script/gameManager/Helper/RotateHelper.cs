using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotateHelper
{ 
    /// <summary>
    /// 旋转到单位
    /// </summary>
    /// <param name="owner">需要旋转的对象</param>
    /// <param name="target">转向的目标</param>
    /// <param name="speed">旋转的速度</param>
    /// <returns></returns>
    public static Quaternion RotateToUnit(Transform owner,Transform target,float speed)
    {
        if (owner == null || target == null) { Debug.Log("没有旋转对象"); return Quaternion.identity; }

        Vector3 ownerPos=owner.position;
        Vector3 targetPos=target.position;

        return Quaternion.RotateTowards(owner.rotation,
                                        GetRotateAngle(ownerPos, targetPos), speed*Time.deltaTime);
    }
    /// <summary>
    /// 旋转到位置
    /// </summary>
    /// <param name="owner">需要旋转的对象</param>
    /// <param name="pos">需要旋转到的位置</param>
    /// <param name="speed">旋转速度</param>
    /// <param name="extraAngle">额外角度</param>
    /// <returns></returns>
    public static Quaternion RotateToPos(Transform owner,Vector3 pos,float speed,float extraAngle=0)
    {
        if(owner == null) { Debug.Log("无法旋转到目标点"); return Quaternion.identity; }

        Vector3 ownerPos=owner.position;

        return Quaternion.RotateTowards(owner.rotation,GetRotateAngle(ownerPos,pos,extraAngle)
                                            ,speed*Time.deltaTime);
    }
    /// <summary>
    /// 旋转角度
    /// </summary>
    /// <param name="_object">需要旋转的对象</param>
    /// <param name="angle">旋转的角度</param>
    /// <param name="speed">旋转速度</param>
    /// <returns></returns>
    public static Quaternion ObjectRotate(Transform _object,float angle,float speed)
    {
        Quaternion r = Quaternion.Euler(0, 0, angle);
        return Quaternion.RotateTowards(_object.rotation, r, speed*Time.deltaTime);
    }
    /// <summary>
    /// 获取自己到目标的角度
    /// </summary>
    /// <param name="owner">自己</param>
    /// <param name="target">目标</param>
    /// <param name="extraAngle">额外角度</param>
    /// <returns></returns>
    public static Quaternion GetRotateAngle(Vector3 owner,Vector3 target,float extraAngle=0)
    {

        float angle=Mathf.Atan2(target.y-owner.y,target.x-owner.x)*Mathf.Rad2Deg;

        return Quaternion.Euler(0,0,angle+extraAngle);
    }
}
