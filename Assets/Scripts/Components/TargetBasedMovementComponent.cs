using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct TargetBasedMovement : IComponentData
{
    public float MoveSpeed;
    public float TurnSpeed;
    public float TargetDistance;
    public float3 TargetPosition;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="TargetBasedMovement"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class TargetBasedMovementComponent : MonoBehaviour
{
    public float MoveSpeed;
    public float TurnSpeed;
    public float TargetDistance;

    /// <summary>
    /// A class to bake a <see cref="TargetBasedMovementComponent"/> into a <see cref="TargetBasedMovement"/> DOTs component.
    /// </summary>
    public class Baker : Baker<TargetBasedMovementComponent>
    {
        public override void Bake(TargetBasedMovementComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetBasedMovement()
            {
                MoveSpeed = authoring.MoveSpeed,
                TurnSpeed = authoring.TurnSpeed,
                TargetDistance = authoring.TargetDistance,
            });
        }
    }
}
