using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct Attackable : IComponentData
{
    public EFactions Faction;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="Attackable"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class AttackableComponent : MonoBehaviour
{
    public EFactions Faction;

    /// <summary>
    /// A class to bake a <see cref="AttackableComponent"/> into a <see cref="Attackable"/> DOTs component.
    /// </summary>
    public class Baker : Baker<AttackableComponent>
    {
        public override void Bake(AttackableComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Attackable() 
            { 
                Faction = authoring.Faction,
            });
        }
    }
}