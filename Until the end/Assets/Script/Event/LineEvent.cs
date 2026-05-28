using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineEvent 
{
    public static Action<ShowUnitVisualEvent> ShowUnitVisualEvent;
    public static Action<UnitAttribute> UpdateDynamicMovePathEvent;
    public static Action<UnitAttribute> UpdateMovePathEvent;
    public static Action<UnitAttribute> HideDestroyUnitEvent;
    public static Action<ShowExplosionEvent> ShowExplosEvent;
    public static Action ClearAllShowLine;
}

