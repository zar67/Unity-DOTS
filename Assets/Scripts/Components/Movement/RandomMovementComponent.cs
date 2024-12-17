using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct RandomMovement : IComponentData
{
    public float MinMoveDistance;
    public float MaxMoveDistance;
    public float MovementDelayMin;
    public float MovementDelayMax;
    public float MovementDelay;
    public float MovementWaitTimer;
    public Unity.Mathematics.Random Random;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="RandomMovement"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class RandomMovementComponent : MonoBehaviour
{
    public float MinMoveDistance = 1f;
    public float MaxMoveDistance = 5f;
    public float MovementDelayMin = 0.2f;
    public float MovementDelayMax = 2f;

    /// <summary>
    /// A class to bake a <see cref="RandomMovementComponent"/> into a <see cref="RandomMovement"/> DOTs component.
    /// </summary>
    public class Baker : Baker<RandomMovementComponent>
    {
        public override void Bake(RandomMovementComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomMovement()
            {
                MinMoveDistance = authoring.MinMoveDistance,
                MaxMoveDistance = authoring.MaxMoveDistance,
                MovementDelayMin = authoring.MovementDelayMin,
                MovementDelayMax = authoring.MovementDelayMax,
                Random = new Unity.Mathematics.Random((uint)entity.Index),
            });
        }
    }
}
