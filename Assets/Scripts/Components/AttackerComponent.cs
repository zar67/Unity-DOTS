using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct Attacker : IComponentData
{
    public float AttackRange;
    public EFactions TargetFaction;
    public float TargetUpdateDelay;
    public float TargetUpdateTimer;

    public Entity ActiveTarget;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="Attacker"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class AttackerComponent : MonoBehaviour
{
    public float AttackRange = 5f;
    public EFactions TargetFaction;
    public float TargetUpdateDelay = 0.2f;

    /// <summary>
    /// A class to bake a <see cref="AttackerComponent"/> into a <see cref="Attacker"/> DOTs component.
    /// </summary>
    public class Baker : Baker<AttackerComponent>
    {
        public override void Bake(AttackerComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Attacker()
            {
                AttackRange = authoring.AttackRange,
                TargetFaction = authoring.TargetFaction,
                TargetUpdateDelay = authoring.TargetUpdateDelay,
            });
        }
    }
}