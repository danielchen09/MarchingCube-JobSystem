using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkState {
    public const int 
        FILL = 0,
        MESH = 1,
        BAKE = 2,
        DONE = 3;

    public int workState;

    public WorkState() {
        workState = 0;
    }

    public void Next() {
        if (workState < 3)
            workState = workState + 1;
    }

    public void NextInLoop() {
        workState = (workState + 1) % 3;
    }

    public void Restart() {
        workState = 0;
    }
}
