using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Should be refactored using events instead of on update.

        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.SelectedVisual);
            visualTransform.ValueRW.Scale = 0;
        }

        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.SelectedVisual);
            visualTransform.ValueRW.Scale = selected.ValueRO.SelectedShowScale;
        }
    }
}
