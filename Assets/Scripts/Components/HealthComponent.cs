using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct Health : IComponentData
{
    public int MaxHealth;
    public int CurrentHealth;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="Health"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    public int MaxHealth = 100;

    /// <summary>
    /// A class to bake a <see cref="HealthComponent"/> into a <see cref="Health"/> DOTs component.
    /// </summary>
    public class Baker : Baker<HealthComponent>
    {
        public override void Bake(HealthComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health()
            {
                MaxHealth = authoring.MaxHealth,
                CurrentHealth = authoring.MaxHealth,
            });
        }
    }
}