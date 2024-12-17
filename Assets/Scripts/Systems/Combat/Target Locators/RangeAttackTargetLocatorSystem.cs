using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
                    float smallestDistance = float.MaxValue;
                    Entity closestEntity = Entity.Null;
                    foreach (DistanceHit distanceHit in distanceHitList)
                    {
                        if (SystemAPI.HasComponent<Attackable>(distanceHit.Entity))
                        {
                            Attackable targetObject = SystemAPI.GetComponent<Attackable>(distanceHit.Entity);
                            if (attacker.ValueRO.TargetFaction == targetObject.Faction)
                            {
                                float distance = math.distancesq(transform.ValueRO.Position, distanceHit.Position);
                                if (closestEntity == Entity.Null || smallestDistance > distance)
                                {
                                    closestEntity = distanceHit.Entity;
                                    smallestDistance = distance;
                                }
                                break;
                            }
                        }
                    }

                    if (closestEntity != Entity.Null)
                    {
                        target.ValueRW.ActiveTarget = closestEntity;
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
