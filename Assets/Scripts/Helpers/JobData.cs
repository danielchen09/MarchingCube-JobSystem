using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class JobData<T> {
    public T job;
    public JobHandle handle;

    public JobData(T job, JobHandle handle) {
        this.job = job;
        this.handle = handle;
    }
}
