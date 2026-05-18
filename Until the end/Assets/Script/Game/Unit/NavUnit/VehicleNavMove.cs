using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleNavMove : UnitNavMove
{
    protected override void Update()
    {
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return;//没有路径||路径数==0||目标路径序列大于路径数

        NavMove();
    }
    public override void Move()
    {
        
        Quaternion angle=RotateHelper.GetRotateAngle(transform.position, currentPathPos);
        float rotate = Quaternion.Angle(transform.rotation, angle);
        switch(rotate)
        {
            case float i when i < 15:
                Forward();
                break;
            case float i when i < 110:
                Rotate();
                break;
            default:
                Back();
                break;
        };
        
    }
    private void Forward()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentPathPos,
                                                moveSpeed*Time.deltaTime);
        transform.rotation = RotateHelper.RotateToPos(transform, currentPathPos, rotateSpeed);
    }
    private void Rotate()
    {
        transform.Translate(Vector2.right*moveSpeed*Time.deltaTime*0.4f);
        transform.rotation=RotateHelper.RotateToPos(transform, currentPathPos, rotateSpeed);
    }
    private void Back()
    {
        transform.Translate(Vector2.left*moveSpeed*Time.deltaTime*0.3f);
        transform.rotation=RotateHelper.RotateToPos(transform, currentPathPos, rotateSpeed,180);
    }
}
