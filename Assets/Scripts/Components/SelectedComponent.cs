using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct Selected : IComponentData, IEnableableComponent
{
    public Entity SelectedVisual;
    public float SelectedShowScale;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="Selected"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class SelectedComponent : MonoBehaviour
{
    public GameObject SelectedVisual;
    public float SelectedShowScale;

    /// <summary>
    /// A class to bake a <see cref="SelectedComponent"/> into a <see cref="Selected"/> DOTs component.
    /// </summary>
    public class Baker : Baker<SelectedComponent>
    {
        public override void Bake(SelectedComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected()
            {
                SelectedVisual = GetEntity(authoring.SelectedVisual, TransformUsageFlags.Dynamic),
                SelectedShowScale = authoring.SelectedShowScale,
            });
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}
