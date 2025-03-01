using System.Collections.Generic;
using UnityEngine;
using World;

public class BuildingGridDrawer : MonoBehaviour
{
    public GameObject gridCell;

    private Material _gridCellMaterial;
    public Material gridCellErrorMaterial;

    [SerializeField, GetSet("Visible")]
    private bool _visible = true;
    public bool Visible 
    { 
        get => _visible;
        set
        {
            _visible = value;
            foreach (var cell in _cells)
                cell.SetActive(_visible);
        }
    }

    private Building _building = null;
    private Vector3 _lastBuildingPos = Vector3.zero;

    private List<GameObject> _cells = new();

    private static Vector3 _cellHeightOffset = Vector3.up * 0.1f;

    private void Start()
    {
        _building = gameObject.GetComponent<Building>();
        Debug.Assert(_building != null);

        _gridCellMaterial = gridCell.GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        if (!Visible)
            return;

        BuildingGrid buildGrid = _building.buildGrid;

        int gridSize = buildGrid.width * buildGrid.length;
        if (_cells.Count != gridSize)
        {
            UpdateCellObjects();
        }

        if (_lastBuildingPos != _building.transform.position)
        {
            MoveToBuildingPos();
        }
    }

    private void MoveToBuildingPos()
    {
        _lastBuildingPos = _building.transform.position;

        BuildingGrid buildGrid = _building.buildGrid;
        Bounds bounds = _building.gameObject.GetGridBounds();
        for (int x = 0; x < buildGrid.width; x++)
        {
            for (int z = 0; z < buildGrid.length; z++)
            {
                Vector3 halfBlockOffset = Vector3.forward / 2 + Vector3.right / 2; // Mesh pivot is not in center
                Vector3 offset = x * Vector3.right + z * Vector3.forward + halfBlockOffset;

                var cellObject = _cells[x + z * buildGrid.width];
                cellObject.transform.position = bounds.min + offset + _cellHeightOffset;
                UpdateCellMaterial(cellObject);
            }
        }
    }

    private void UpdateCellObjects()
    {
        Clear();

        foreach (var cell in _building.buildGrid.grid)
        {
            _cells.Add(Instantiate(gridCell, transform, true));
        }
    }

    private void UpdateCellMaterial(GameObject cellObject)
    {
        var cellBlock = GameManager.Instance.World.GetBlockAt(cellObject.transform.position - _cellHeightOffset * 2);

        if (cellBlock.IsBuildable())
            cellObject.GetComponentInChildren<MeshRenderer>().material = _gridCellMaterial;
        else
            cellObject.GetComponentInChildren<MeshRenderer>().material = gridCellErrorMaterial;
    }

    private void Clear()
    {
        foreach (var cell in _cells)
        {
            Destroy(cell);
        }
        _cells.Clear();
        _lastBuildingPos = Vector3.zero;
    }
}
