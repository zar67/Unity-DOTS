using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct HealthBarUI : IComponentData
{
    public Entity BarVisual;
    public Entity HealthEntity;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="HealthBarUI"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class HealthBarUIComponent : MonoBehaviour
{
    public GameObject BarHolder;
    public GameObject HealthGameObject;

    /// <summary>
    /// A class to bake a <see cref="HealthBarUIComponent"/> into a <see cref="HealthBarUI"/> DOTs component.
    /// </summary>
    public class Baker : Baker<HealthBarUIComponent>
    {
        public override void Bake(HealthBarUIComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBarUI()
            {
                BarVisual = GetEntity(authoring.BarHolder, TransformUsageFlags.NonUniformScale),
                HealthEntity = GetEntity(authoring.HealthGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}
