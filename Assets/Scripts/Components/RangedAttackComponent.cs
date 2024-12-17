using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct RangedAttack : IComponentData
{
    public float AttackDelay;
    public float AttackTimer;
    public int Damage;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="RangedAttack"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class RangedAttackComponent : MonoBehaviour
{
    public float ShootDelay = 0.2f;
    public int Damage = 5;

    /// <summary>
    /// A class to bake a <see cref="RangedAttackComponent"/> into a <see cref="RangedAttack"/> DOTs component.
    /// </summary>
    public class Baker : Baker<RangedAttackComponent>
    {
        public override void Bake(RangedAttackComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RangedAttack()
            {
                AttackDelay = authoring.ShootDelay,
                Damage = authoring.Damage,
            });
        }
    }
}