using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<LocalTransform> transform, RefRW<RandomMovement> randomMovement, RefRW<TargetPositionMovement> positionMovement) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<RandomMovement>, RefRW<TargetPositionMovement>>())
        {
            if (!positionMovement.ValueRO.IsMovingToTarget)
            {
                Random random = randomMovement.ValueRO.Random;

                if (randomMovement.ValueRO.MovementDelay <= 0)
                {
                    // Generate new random wait time
                    float newMovementDelay = random.NextFloat(randomMovement.ValueRO.MovementDelayMin, randomMovement.ValueRO.MovementDelayMax);
                    randomMovement.ValueRW.MovementDelay = newMovementDelay;
                    randomMovement.ValueRW.MovementWaitTimer = 0f;
                    continue;
                }

                randomMovement.ValueRW.MovementWaitTimer += SystemAPI.Time.DeltaTime;

                if (randomMovement.ValueRO.MovementWaitTimer >= randomMovement.ValueRO.MovementDelay)
                {
                    // Update to new target
                    var randomDirection = new float3(random.NextFloat(-1f, 1f), 0f, random.NextFloat(-1f, 1f));
                    randomDirection = math.normalize(randomDirection);

                    float randomDistance = random.NextFloat(0f, randomMovement.ValueRO.MaxMoveDistance);

                    positionMovement.ValueRW.Target = transform.ValueRO.Position + (randomDirection * randomDistance);

                    randomMovement.ValueRW.MovementDelay = 0f;
                    randomMovement.ValueRW.MovementWaitTimer = 0f;
                }

                randomMovement.ValueRW.Random = random;
            }
        }
    }
}
