using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct TargetPositionMovement : IComponentData
{
    public float MoveSpeed;
    public float TurnSpeed;
    public float ReachedDistance;
    public float3 Target;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="TargetPositionMovement"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class TargetPositionMovementComponent : MonoBehaviour
{
    public float MoveSpeed = 3f;
    public float TurnSpeed = 10f;
    public float ReachedDistance = 0.1f;

    /// <summary>
    /// A class to bake a <see cref="TargetPositionMovementComponent"/> into a <see cref="TargetPositionMovement"/> DOTs component.
    /// </summary>
    public class Baker : Baker<TargetPositionMovementComponent>
    {
        public override void Bake(TargetPositionMovementComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetPositionMovement()
            {
                MoveSpeed = authoring.MoveSpeed,
                TurnSpeed = authoring.TurnSpeed,
                ReachedDistance = authoring.ReachedDistance,
            });
        }
    }
}
