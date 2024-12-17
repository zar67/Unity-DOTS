using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct RangeAttackTargetLocator : IComponentData
{
    public float AttackRange;
    public EFactions TargetFaction;
    public float TargetUpdateDelay;
    public float TargetUpdateTimer;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="RangeAttackTargetLocator"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class RangeAttackTargetLocatorComponent : MonoBehaviour
{
    public float AttackRange = 5f;
    public EFactions TargetFaction;
    public float TargetUpdateDelay = 0.2f;

    /// <summary>
    /// A class to bake a <see cref="RangeAttackTargetLocatorComponent"/> into a <see cref="RangeAttackTargetLocator"/> DOTs component.
    /// </summary>
    public class Baker : Baker<RangeAttackTargetLocatorComponent>
    {
        public override void Bake(RangeAttackTargetLocatorComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RangeAttackTargetLocator()
            {
                AttackRange = authoring.AttackRange,
                TargetFaction = authoring.TargetFaction,
                TargetUpdateDelay = authoring.TargetUpdateDelay,
            });
        }
    }
}