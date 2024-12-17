using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct RangeAttackTargetLocatorSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physics = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        var distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        var collisionFilter = new CollisionFilter()
        {
            BelongsTo = ~0u, // Any layer
            CollidesWith = ~0u, // Any layer
            GroupIndex = 0
        };

        foreach ((RefRO<LocalTransform> transform, RefRW<RangeAttackTargetLocator> attacker, RefRW<AttackTarget> target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<RangeAttackTargetLocator>, RefRW<AttackTarget>>())
        {
            attacker.ValueRW.TargetUpdateTimer += SystemAPI.Time.DeltaTime;

            if (attacker.ValueRO.TargetUpdateTimer >= attacker.ValueRO.TargetUpdateDelay)
            {
                distanceHitList.Clear();
                if (physics.CollisionWorld.OverlapSphere(transform.ValueRO.Position, attacker.ValueRO.AttackRange, ref distanceHitList, collisionFilter))
                {
                    foreach (DistanceHit distanceHit in distanceHitList)
                    {
                        if (SystemAPI.HasComponent<FactionObject>(distanceHit.Entity))
                        {
                            FactionObject targetObject = SystemAPI.GetComponent<FactionObject>(distanceHit.Entity);
                            if (attacker.ValueRO.TargetFaction == targetObject.Faction)
                            {
                                target.ValueRW.ActiveTarget = distanceHit.Entity;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    target.ValueRW.ActiveTarget = Entity.Null;
                }

                attacker.ValueRW.TargetUpdateTimer = 0f;
            }
        }
    }
}
