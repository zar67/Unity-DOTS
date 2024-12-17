using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct TargetEntityMovement : IComponentData
{
    public Entity Target;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="TargetEntityMovement"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class TargetEntityMovementComponent : MonoBehaviour
{
    /// <summary>
    /// A class to bake a <see cref="TargetEntityMovementComponent"/> into a <see cref="TargetEntityMovement"/> DOTs component.
    /// </summary>
    public class Baker : Baker<TargetEntityMovementComponent>
    {
        public override void Bake(TargetEntityMovementComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetEntityMovement()
            {

            });
        }
    }
}
