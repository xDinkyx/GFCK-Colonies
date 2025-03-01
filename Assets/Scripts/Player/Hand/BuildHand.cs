using System;
using UnityEngine;
using UnityEngine.InputSystem;
using World;

public class BuildHand : InputResolverStep
{
    private GameObject _selectedStructure;

    public Material PreviewMaterial;

    // Properties to restore after placement.
    private Material _oldMaterial;
    private int _oldLayer;


    public static event EventHandler StructurePlaced;
    public static event EventHandler BuildCanceled;

    public override InputResolver.InputResolution ResolveInput()
    {
        if (Input.GetMouseButtonDown(1)) // right mouse btn
            CancelBuilding();

        var hovered_block = GameManager.Instance.World.GetBlockUnderMouse(true);
        if (hovered_block == null
         || !hovered_block.IsBuildable())
            return InputResolver.InputResolution.Block;

        if (_selectedStructure != null)
        {
            MoveBuildingTo(hovered_block);

            RotateStructureOnInput();
        }

        if (Input.GetMouseButtonDown(0)) // left mouse btn
            PlaceStructure();

        return InputResolver.InputResolution.Block;
    }

    private void PlaceStructure()
    {
        if (_selectedStructure.TryGetComponent<Building>(out var building))
        {
            building.OnConstructed();
        }

        RestoreStructureProperties();

        _selectedStructure.isStatic = true;
        _selectedStructure = null;

        StructurePlaced.Invoke(this, null);
    }

    public void SelectStructure(GameObject structure)
    {
        _selectedStructure = GameManager.InstantiateGameObject(structure); // ToDo: Instantiate a temp game object, with scripts removed. And only instantiate proper when placed.

        // Store properties we change to restore them later.
        _oldMaterial = _selectedStructure.GetComponent<MeshRenderer>().material;
        _oldLayer = _selectedStructure.layer;

        SetTemporaryStructureProperties();
    }

    private void RotateStructureOnInput()
    {
        if (Input.GetKeyDown(KeyCode.E)
         || Mouse.current.middleButton.wasPressedThisFrame)
            _selectedStructure.transform.Rotate(Vector3.up, 90, Space.World);
        if (Input.GetKeyDown(KeyCode.Q))
            _selectedStructure.transform.Rotate(Vector3.up, -90, Space.World);
    }

    public void SetTemporaryStructureProperties()
    {
        // Set preview material
        _selectedStructure.GetComponent<MeshRenderer>().material = PreviewMaterial;

        // Set to preview layer, which does not interact with characters.
        _selectedStructure.layer = LayerMask.NameToLayer(GlobalDefines.previewLayerName);
    }
    public void RestoreStructureProperties()
    {
        // Restore material
        _selectedStructure.GetComponent<MeshRenderer>().material = _oldMaterial;

        // Restore layer
        _selectedStructure.layer = _oldLayer;
    }

    private void CancelBuilding()
    {
        GameObject.Destroy(_selectedStructure);
        _selectedStructure = null;

        BuildCanceled.Invoke(this, null);
    }
    private void MoveBuildingTo(Block block)    {        if (_selectedStructure.TryGetComponent<Building>(out var building)         && !DoesStructureFit(building.buildGrid))
            return;

        Vector3 offset = Vector3.right / 2 + Vector3.forward / 2;        _selectedStructure.transform.position = block.GetSurfaceWorldPos() + _selectedStructure.GetPivotYOffset() + offset;    }
    private bool DoesStructureFit(BuildingGrid buildingGrid)    {        //var bounds = _selectedStructure.GetGridBounds();        //var structureBlocks = GameManager.Instance.GameWorld.GetContainedBlocks(bounds);
        //Debug.Assert(structureBlocks.Length == buildingGrid.grid.Length);
        //for (int x = 0; x < structureBlocks.GetLength(0); x++)        //{        //    for (int z = 0; z < structureBlocks.GetLength(1); z++)        //    {        //        var block = structureBlocks[x, z];
        //        if (block == null)        //            return false;
        //        if (buildingGrid.grid[x, z] != BuildingGrid.Cell.FREE        //         && !BlockCode.IsBuildable(block))        //            return false;        //    }        //}
        return true;    }
}
