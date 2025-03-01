using System;
using System.Collections.Generic;
using UnityEngine;
using World;
using Jobs;
using Economy;

public class Building : StorageEntity // Need to add derived classes to BuildingEditor.
{
    public BuildingGrid buildGrid = new();

    public List<Job> jobs = new List<Job>();

    private bool _firstUpdate = true;
    private bool _underConstruction = true; // New buildings are under construction by default.

    protected virtual void Update()
    {
        // Set blockGrid after first update, because bounding box of renderer is only correct after first render.
        if (_firstUpdate)
        {
            CreateBuildGrid();
            _firstUpdate = false;
        }
    }

    protected virtual void OnDestroy()
    {
        UnregisterJobs();
    }

    public virtual void OnConstructed()
    {
        _underConstruction = true; // Temp until we implement actual construction.

        RegisterJobs();
        ShowBuildGrid(false);
    }

    // Can be done by player-hand, or by villager
    public void DropOffResource(Resource resource)
    {
        inventory.AddResource(resource.type);

        ResourceManager.Instance.RemoveResourceFromWorld(resource); // Call last since it also destroys the object.
    }

    public void RegisterJobs()
    {
        foreach (Job job in jobs)
            JobManager.Instance.RegisterJob(job);
    }
    public void UnregisterJobs()
    {
        foreach (Job job in jobs)
            JobManager.Instance.UnregisterJob(job);
    }
    public void AddJob(JobType type)
    {
        var job = JobFactory.CreateJob(type, this);
        jobs.Add(job);
    }

    public Block GetCurrentBlock()
    {
        return GameManager.Instance.World.GetBlockAt(transform.position + Vector3.down / 2);
    }

    public void ShowBuildGrid(bool show)
    {
        GetComponent<BuildingGridDrawer>().Visible = show;
    }

    private void CreateBuildGrid()
    {
        var bounds = gameObject.GetGridBounds();
        buildGrid.ResizeGrid(Mathf.FloorToInt(bounds.size.x), Mathf.FloorToInt(bounds.size.z));
    }
}
