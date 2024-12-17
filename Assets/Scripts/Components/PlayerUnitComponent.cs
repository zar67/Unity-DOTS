using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct PlayerUnit : IComponentData
{

}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="PlayerUnit"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class PlayerUnitComponent : MonoBehaviour
{
    /// <summary>
    /// A class to bake a <see cref="PlayerUnitComponent"/> into a <see cref="PlayerUnit"/> DOTs component.
    /// </summary>
    public class Baker : Baker<PlayerUnitComponent>
    {
        public override void Bake(PlayerUnitComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerUnit() { });
        }
    }
}