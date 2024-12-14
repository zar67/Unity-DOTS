using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// </summary>
public struct MoveSpeed : IComponentData
{
    public float Value;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="MoveSpeed"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class MoveSpeedComponent : MonoBehaviour
{
    public float Value;

    /// <summary>
    /// A class to bake a <see cref="MoveSpeedComponent"/> <see cref="MonoBehaviour"/> into a <see cref="MoveSpeed"/> DOTs component.
    /// </summary>
    public class Baker : Baker<MoveSpeedComponent>
    {
        public override void Bake(MoveSpeedComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveSpeed()
            {
                Value = authoring.Value
            });
        }
    }
}
