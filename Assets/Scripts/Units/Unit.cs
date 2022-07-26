using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public float speed = 5;

    public Job job = null;

    protected void Update()
    {
        if (job == null)
            ApplyForJob();
        else
            job.Tick();
    }

    public BlockData GetCurrentBlock()
    {
        Vector3 posUnderBlock = GameObjectUtil.GetObjectBottomPosition(gameObject) - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(posUnderBlock);
    }

    public void ApplyForJob()
    {
        job = JobManager.Instance.TakeAvailableJob();

        if (job != null)
        {
            job.unit = this;
            job.Start();
        }
    }

    public void StopJob()
    {
        job = null;
    }

    public void FireFromJob()
    {
        JobManager.Instance.GiveJobBackToLaborMarket(job);
        job = null;
    }
}
