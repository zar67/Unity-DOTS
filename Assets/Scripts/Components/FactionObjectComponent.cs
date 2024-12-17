using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct FactionObject : IComponentData
{
    public EFactions Faction;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="global::FactionObject"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class FactionObjectComponent : MonoBehaviour
{
    public EFactions Faction;

    /// <summary>
    /// A class to bake a <see cref="FactionObjectComponent"/> into a <see cref="global::FactionObject"/> DOTs component.
    /// </summary>
    public class Baker : Baker<FactionObjectComponent>
    {
        public override void Bake(FactionObjectComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FactionObject() 
            { 
                Faction = authoring.Faction,
            });
        }
    }
}