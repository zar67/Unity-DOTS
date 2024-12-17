using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct ProjectileAttacker : IComponentData
{
    public float FiringDistanceFromTarget;
    public float FiringDistanceReachedDistance;
    public float AttackDelay;
    public float AttackTimer;
    public Entity ProjectileEntity;
    public float3 ProjectileOffset;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="ProjectileAttacker"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class ProjectileAttackerComponent : MonoBehaviour
{
    public float FiringDistanceFromTarget = 5f;
    public float FiringDistanceReachedDistance = 1f;
    public float AttackDelay = 0.2f;
    public Transform ProjecileStartTransform;
    public GameObject ProjectilePrefab;

    /// <summary>
    /// A class to bake a <see cref="ProjectileAttackerComponent"/> into a <see cref="ProjectileAttacker"/> DOTs component.
    /// </summary>
    public class Baker : Baker<ProjectileAttackerComponent>
    {
        public override void Bake(ProjectileAttackerComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ProjectileAttacker()
            {
                FiringDistanceFromTarget = authoring.FiringDistanceFromTarget,
                FiringDistanceReachedDistance = authoring.FiringDistanceReachedDistance,
                AttackDelay = authoring.AttackDelay,
                ProjectileEntity = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                ProjectileOffset = authoring.ProjecileStartTransform.localPosition,
            });
        }
    }
}