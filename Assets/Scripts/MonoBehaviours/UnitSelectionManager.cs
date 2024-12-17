using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviourSingleton<UnitSelectionManager>
{
    public event EventHandler<Vector2> OnSelectionAreaStart;
    public event EventHandler<Vector2> OnSelectionAreaEnd;

    private Vector2 m_selectionStartMousePosition;

    public Rect GetSelectionArea()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        var lowerLeftCorner = new Vector2(
            Mathf.Min(m_selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(m_selectionStartMousePosition.y, selectionEndMousePosition.y));

        var upperRightCorner = new Vector2(
            Mathf.Max(m_selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(m_selectionStartMousePosition.y, selectionEndMousePosition.y));

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, m_selectionStartMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectEntitiesInSelectionArea();
            OnSelectionAreaEnd?.Invoke(this, Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetTargetsOfSelectedEntities();
        }
    }

    private void SelectEntitiesInSelectionArea()
    {
        Rect selectionArea = GetSelectionArea();

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /// <see cref="Allocator"/> determines the lifetime of the memory.
        /// <see cref="Allocator.Temp"/> means the memory will be cleaned up at the end of the frame.
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, PlayerUnit, Selected>().Build(entityManager);
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < entityArray.Length; i++)
        {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
        }

        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, PlayerUnit>().WithPresent<Selected>().Build(entityManager);
        entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> transformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        for (int i = 0; i < transformArray.Length; i++)
        {
            LocalTransform transform = transformArray[i];
            Vector2 entityScreenPosition = Camera.main.WorldToScreenPoint(transform.Position);
            if (selectionArea.Contains(entityScreenPosition))
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
            }
        }
    }

    private void SetTargetsOfSelectedEntities()
    {
        Vector3 mouseWorldPosition = MousePositionManager.Instance.GetWorldPosition();

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /// <see cref="Allocator"/> determines the lifetime of the memory.
        /// <see cref="Allocator.Temp"/> means the memory will be cleaned up at the end of the frame.
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<TargetBasedMovement, Selected>().Build(entityManager);

        NativeArray<TargetBasedMovement> movementArray = entityQuery.ToComponentDataArray<TargetBasedMovement>(Allocator.Temp);
        for (int i = 0; i < movementArray.Length; i++)
        {
            TargetBasedMovement movement = movementArray[i];
            movement.TargetPosition = mouseWorldPosition;
            movementArray[i] = movement;
        }

        // Update the data on all the components.
        // This is needed because the components are structs (a type value) not a reference value so any changes will apply to the copy of the data not the original data.
        entityQuery.CopyFromComponentDataArray(movementArray);
    }
}
