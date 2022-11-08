using UnityEngine;
using World;

namespace Jobs
{
    public class HarvestResourceTask : Task
    {
        private ResourceType _type;
        private ResourceNode _node;

        private float _timeSinceLastHarvest = 0;

        public HarvestResourceTask(Job job, ResourceType type, bool oneTime = false) : base(job, oneTime)
        {
            this._type = type;
        }

        public override void Start()
        {
            base.Start();

            // This task assumes the character is already next to the node.
            // Otherwise we will finish the task unsuccesfully.
            _node = FindResourceNode();
            if (_node == null)
            {
                Debug.LogError("No node found next to unit! Task failed.");
                Finish();
            }

            _timeSinceLastHarvest = 0;
        }

        public override void Tick()
        {
            base.Tick();


            float timeToHarvest = 1.0f / job.UnitJobComponent.harvestSpeed;
            if (_timeSinceLastHarvest >= timeToHarvest)
            {
                if (TryHarvest())
                {
                    Finish();
                }
            }
            else
                _timeSinceLastHarvest += Time.deltaTime;
        }

        private bool TryHarvest()
        {
            if (_node.Harvest(job.UnitJobComponent.harvestDamage))
            {
                job.GetAssignedUnit().inventory.AddResource(_node.type);
                return true;
            }

            return false;
        }

        private ResourceNode FindResourceNode()
        {
            Debug.Assert(_type != ResourceType.RESOURCE_INVALID);
            if (_type == ResourceType.RESOURCE_INVALID) return null;

            string nodeTag = Conversions.ResourceNodeTagForType(_type);

            foreach (var node in GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag))
            {
                if (node.name.Contains(nodeTag))
                {
                    var resourceNode = node.GetComponent<ResourceNode>();

                    float sqr_distance = job.GetAssignedUnit().gameObject.GetSqrBBDistanceToObject(resourceNode.gameObject);
                    if (sqr_distance <= MathUtil.SQRD_DIAG_DIST_BETWEEN_BLOCKS)
                        return resourceNode;
                }
            }

            return null;
        }

        public override string GetTaskDebugDescription()
        {
            return "I am harvesting a resource";
        }
    }
}
