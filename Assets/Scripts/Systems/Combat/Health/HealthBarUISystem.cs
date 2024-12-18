using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(HealthResetEventsSystem))]
partial struct HealthBarUISystem : ISystem
{
    /// Cannot use <see cref="BurstCompileAttribute"/> with managed (class) data like <see cref="UnityEngine.MonoBehaviour"/>.
    /// This will give an error to Unity if both are used, but will not stop compilation.
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if (Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }

        foreach ((RefRW<LocalTransform> transform, RefRO<HealthBarUI> healthUI) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBarUI>>())
        {
            if (transform.ValueRO.Scale > 0f)
            {
                LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthUI.ValueRO.HealthEntity);
                transform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }

            Health health = SystemAPI.GetComponent<Health>(healthUI.ValueRO.HealthEntity);
            if (health.OnHealthChanged)
            {
                float healthNormalized = health.CurrentHealth / (float)health.MaxHealth;

                transform.ValueRW.Scale = healthNormalized < 1f ? 1f : 0f;

                RefRW<PostTransformMatrix> barTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthUI.ValueRO.BarVisual);
                barTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
            }
        }
    }
}
