using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct AttackerTargetSystem : ISystem
{
    [BurstCompile]
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

        foreach ((RefRO<LocalTransform> transform, RefRW<Attacker> attacker) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Attacker>>())
        {
            attacker.ValueRW.TargetUpdateTimer += SystemAPI.Time.DeltaTime;

            if (attacker.ValueRO.TargetUpdateTimer >= attacker.ValueRO.TargetUpdateDelay)
            {
                distanceHitList.Clear();
                if (physics.CollisionWorld.OverlapSphere(transform.ValueRO.Position, attacker.ValueRO.AttackRange, ref distanceHitList, collisionFilter))
                {
                    foreach (DistanceHit distanceHit in distanceHitList)
                    {
                        FactionObject target = SystemAPI.GetComponent<FactionObject>(distanceHit.Entity);
                        if (attacker.ValueRO.TargetFaction == target.Faction)
                        {
                            attacker.ValueRW.ActiveTarget = distanceHit.Entity;
                            break;
                        }
                    }
                }
                else
                {
                    attacker.ValueRW.ActiveTarget = Entity.Null;
                }

                attacker.ValueRW.TargetUpdateTimer = 0f;
            }
        }
    }
}
