using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct UnitMovement : IComponentData
{
    public float MoveSpeed;
    public float TurnSpeed;
    public float3 TargetPosition;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="UnitMovement"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class UnitMovementComponent : MonoBehaviour
{
    public float MoveSpeed;
    public float TurnSpeed;

    /// <summary>
    /// A class to bake a <see cref="UnitMovementComponent"/> into a <see cref="UnitMovement"/> DOTs component.
    /// </summary>
    public class Baker : Baker<UnitMovementComponent>
    {
        public override void Bake(UnitMovementComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMovement()
            {
                MoveSpeed = authoring.MoveSpeed,
                TurnSpeed = authoring.TurnSpeed,
            });
        }
    }
}
