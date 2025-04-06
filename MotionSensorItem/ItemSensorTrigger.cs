using UnityEngine;

public class ItemSensorTrigger : MonoBehaviour
{
    private enum TargetType
    {
        None = 0,
        Enemy = 1
    }

    private PhysGrabObject parentPhysGrabObject;

    private ItemSensor itemSensor;

    public bool enemyTrigger;

    private bool targetAcquired;

    private float visionCheckTimer;

    private void Start()
    {
        parentPhysGrabObject = GetComponentInParent<PhysGrabObject>();
        itemSensor = GetComponentInParent<ItemSensor>();
        if (!SemiFunc.IsMasterClientOrSingleplayer())
        {
            Object.Destroy(this);
        }
    }
    
    /// <summary>
    /// Once the trigger hitbox is entered, calls OnDetect function when it meets these conditions: ItemSensor exists, the state is armed, passes trigger checks
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if ((bool)itemSensor && itemSensor.state == ItemSensor.States.Armed && PassesTriggerChecks(other))
        {
            MotionSensorItem.MotionSensorItem.Logger.LogMessage($"[{Time.deltaTime}] motion sensor trigger enter called");
            OnDetect(other);
        }
    }

    /// <summary>
    /// while the trigger hitbox has something in it, calls OnDetect function when it meets these conditions: ItemSensor exists, the state is armed, passes trigger checks, and the timer is above 2f
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if ((bool)itemSensor && itemSensor.state == ItemSensor.States.Armed && PassesTriggerChecks(other))
        {
            visionCheckTimer += Time.deltaTime;
            if (visionCheckTimer > 2f)
            {
                MotionSensorItem.MotionSensorItem.Logger.LogMessage($"[{Time.deltaTime}] trigger stay called");
                visionCheckTimer = 0f;
                OnDetect(other);
            }
        }
    }

    private bool PassesTriggerChecks(Collider other)
    {
        PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
        if (enemyTrigger)
        {
            if (!componentInParent || !componentInParent.isEnemy)
            {
                return false;
            }
        }

        if ((bool)componentInParent && !componentInParent.isEnemy)
        {
            return false;
        }

        if ((bool)componentInParent && !componentInParent.isEnemy && !componentInParent.grabbed && componentInParent.rb.velocity.magnitude < 0.1f && componentInParent.rb.angularVelocity.magnitude < 0.1f)
        {
            return false;
        }
        return true;
    }

    private void OnDetect(Collider other)
    {
        itemSensor.SetTriggered();
    }

    //private void TryAcquireTarget(Collider other)
    //{
    //    if (targetAcquired)
    //    {
    //        return;
    //    }
    //    PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
    //    PlayerAvatar componentInParent2 = other.GetComponentInParent<PlayerAvatar>();
    //    PlayerAccess componentInParent3 = other.GetComponentInParent<PlayerAccess>();
    //    PlayerController playerController = (componentInParent3 ? componentInParent3.GetComponentInChildren<PlayerController>() : null);
    //    Vector3 position = itemSensor.transform.position;
    //    if ((bool)componentInParent)
    //    {
    //        Vector3 midPoint = componentInParent.midPoint;
    //        if (!VisionObstruct(position, midPoint, componentInParent))
    //        {
    //            if (componentInParent.isEnemy)
    //            {
    //                LockOnTarget(TargetType.Enemy, componentInParent, componentInParent2, playerController);
    //                return;
    //            }
    //        }
    //    }
    //    if (!playerController)
    //    {
    //        return;
    //    }
    //}

    //private void LockOnTarget(TargetType type, PhysGrabObject physObj, PlayerAvatar playerAvatar, PlayerController playerController)
    //{
    //    if (!itemMine)
    //    {
    //        return;
    //    }
    //    switch (type)
    //    {
    //        case TargetType.Enemy:
    //            itemMine.wasTriggeredByEnemy = true;
    //            itemMine.triggeredPhysGrabObject = physObj;
    //            itemMine.triggeredTransform = physObj.transform;
    //            itemMine.triggeredPosition = physObj.transform.position;
    //            break;
    //        case TargetType.RigidBody:
    //            itemMine.wasTriggeredByRigidBody = true;
    //            itemMine.triggeredPhysGrabObject = physObj;
    //            itemMine.triggeredTransform = physObj.transform;
    //            itemMine.triggeredPosition = physObj.transform.position;
    //            break;
    //        case TargetType.Player:
    //            itemMine.wasTriggeredByPlayer = true;
    //            if ((bool)playerAvatar)
    //            {
    //                itemMine.triggeredPlayerAvatar = playerAvatar;
    //                PlayerTumble tumble = playerAvatar.tumble;
    //                if ((bool)tumble)
    //                {
    //                    itemMine.triggeredPlayerTumble = tumble;
    //                    itemMine.triggeredPhysGrabObject = tumble.physGrabObject;
    //                }
    //                itemMine.triggeredTransform = playerAvatar.PlayerVisionTarget.VisionTransform;
    //                itemMine.triggeredPosition = playerAvatar.PlayerVisionTarget.VisionTransform.position;
    //            }
    //            else if ((bool)physObj)
    //            {
    //                PlayerTumble componentInParent = physObj.GetComponentInParent<PlayerTumble>();
    //                if ((bool)componentInParent)
    //                {
    //                    itemMine.triggeredPlayerAvatar = componentInParent.playerAvatar;
    //                    itemMine.triggeredPlayerTumble = componentInParent;
    //                    itemMine.triggeredPhysGrabObject = componentInParent.physGrabObject;
    //                    itemMine.triggeredTransform = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform;
    //                    itemMine.triggeredPosition = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform.position;
    //                }
    //            }
    //            break;
    //    }
    //    targetAcquired = true;
    //    itemMine.SetTriggered();
    //}

    private bool VisionObstruct(Vector3 start, Vector3 end, PhysGrabObject targetPhysObj)
    {
        int layerMask = SemiFunc.LayerMaskGetVisionObstruct();
        Vector3 normalized = (end - start).normalized;
        float maxDistance = Vector3.Distance(start, end);
        RaycastHit[] array = Physics.RaycastAll(start, normalized, maxDistance, layerMask);
        for (int i = 0; i < array.Length; i++)
        {
            RaycastHit raycastHit = array[i];
            if (raycastHit.collider.CompareTag("Wall") || raycastHit.collider.CompareTag("Ceiling"))
            {
                return true;
            }
        }
        return false;
    }
}
