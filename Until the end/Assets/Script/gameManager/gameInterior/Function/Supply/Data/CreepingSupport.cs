using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepingSupport : Support
{

    public override Vector2 SpawnPos()
    {

        Vector2 size=new Vector2(Mathf.Abs(point[0].x - point[2].x),
                                Mathf.Abs(point[0].y - point[2].y));

        float creeping=size.y/ number;//œ¬¥Œæ‡¿Î—”…Ï

        float spawnY = (-size.y * 0.5f + Random.Range(-creeping, creeping) + (creeping * (currentNumber + 1)));
        spawnY=Mathf.Clamp(spawnY,-size.y*0.5f,size.y*0.5f);

        Vector2 spawnPos = new Vector2(
                        Random.Range(-size.x * 0.5f, size.x * 0.5f),spawnY
                        );
        
        Quaternion angle = Quaternion.Euler(0, 0, rotateAngle);
        Vector2 afterRotatePos = angle * spawnPos;
        return center + afterRotatePos;
    }
}
