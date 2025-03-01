using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using World;

public class UnitComponentMove : UnitComponentBase
{
    private List<Block> _path = new List<Block>();
    private int _pathIndex = 0;
    private LineRenderer _pathVisualization = null;

    public delegate void ArrivedAtLocation();
    private ArrivedAtLocation _onArrived;

    public bool lookingForPath { get; private set; } = false;
  
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

#if DEBUG
        Debug.Assert(_pathVisualization == null);

        var line = new GameObject("Path");
        _pathVisualization = line.AddComponent<LineRenderer>();
        _pathVisualization.widthMultiplier = 0.2f;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_path.Count > 0)
        {
            if (_pathIndex >= _path.Count - 1) // We reached end of path
            {
                Stop();
                _onArrived?.Invoke(); // TODO Ideally, we "null" the delegate after this, but we can't until we guarantee that no new task will start during the calling of this event. A new task can trigger a new move event, and then it will NULL the wrong delegate
            }
            else
            {
                FollowPath();
            }
        }
    }

    public void MoveToBlock(Block targetBlock, ArrivedAtLocation onArrived)
    {
        Debug.Assert(Owner.GetCurrentBlock() != null);
        Debug.Assert(targetBlock != null);

        _onArrived = onArrived;
        lookingForPath = true;

        PathfindMaster.Instance.RequestPathfind(Owner.GetCurrentBlock(), targetBlock, SetPath);
    }

    public void Stop()
    {
        ClearPath();
    }

    public void SetPath(List<Block> path)
    {
        lookingForPath = false;

        if (path.Count == 0)
        {
            Debug.LogError("Could not find path");
            return;
        }

        _path = path;
        _path.RemoveAt(_path.Count - 1); // Remove last block, since we don't need to move inside the target object.

        VisualizePath();
    }

    private void FollowPath()
    {
        if (_path.Count == 0)
            return;

        Block targetBlock = _path[_pathIndex + 1];

        Vector3 targetPos = targetBlock.GetSurfaceWorldPos() + gameObject.GetPivotYOffset();
        Vector3 characterToTarget = targetPos - transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * Owner.moveSpeed * Time.fixedDeltaTime;

        transform.position += move;
        direction.y = 0; // For rotation.
        transform.rotation = Quaternion.LookRotation(direction);

        float distanceToTarget = characterToTarget.magnitude;
        if (distanceToTarget < 0.1f) // If distance to center of target block is this small, we're good.
            _pathIndex++;
    }

    private void ClearPath()
    {
        _pathIndex = 0;
        if (_path.Count > 0)
        {
            _path.Clear();
        }

        _pathVisualization.positionCount = 0;
    }

    private void VisualizePath()
    {
        _pathVisualization.positionCount = _path.Count;
        for (int i = 0; i < _path.Count; i++)
        {
            _pathVisualization.SetPosition(i, _path[i].worldPosition + Vector3.up);
        }
    }
}
