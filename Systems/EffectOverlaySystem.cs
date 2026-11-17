using System.Collections.Generic;
using AdvancedBuildingControl;
using Colossal.Entities;
using Game;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Color = UnityEngine.Color;

public partial class EffectOverlaySystem : GameSystemBase
{
    private OverlayRenderSystem overlayRenderSystem;
    private ToolSystem toolSystem;
    private EntityQuery samePrefabQuery;

    private Entity entity;
    private int local = -1;
    private int type = -1;
    private float radius;
    private bool radiusCommon = false;
    private Color color = Color.blue;
    private readonly List<float3> cachedPositions = new();
    private Entity cachedFor = Entity.Null;
    private int framesSinceRefresh;
    private const int RefreshInterval = 30;

    protected override void OnCreate()
    {
        base.OnCreate();
        overlayRenderSystem = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
        toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();

        samePrefabQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<PrefabRef>(),
                    ComponentType.ReadOnly<Game.Objects.Transform>(),
                },
                None = new[] { ComponentType.ReadOnly<Deleted>(), ComponentType.ReadOnly<Temp>() },
            }
        );

        UpdateColor();
    }

    protected override void OnUpdate()
    {
        if (type < 0 || local < 0)
            return;

        if (
            entity == Entity.Null
            || toolSystem.selected != entity
            || !EntityManager.TryGetComponent(entity, out Game.Objects.Transform transform)
            || radius <= 0f
        )
        {
            if (toolSystem.selected != entity)
            {
                type = -1;
                local = -1;
            }
            return;
        }

        OverlayRenderSystem.Buffer buffer = overlayRenderSystem.GetBuffer(out JobHandle deps);
        deps.Complete();

        Color fill = GetColor();
        Color outline = new(fill.r, fill.g, fill.b, 1);

        DrawRadius(buffer, outline, fill, transform.m_Position);

        if (radiusCommon && EntityManager.TryGetComponent(entity, out PrefabRef selectedPrefab))
        {
            if (cachedFor != entity || ++framesSinceRefresh >= RefreshInterval)
                RefreshCache(selectedPrefab);

            for (int i = 0; i < cachedPositions.Count; i++)
                DrawRadius(buffer, outline, fill, cachedPositions[i]);
        }
    }

    private void RefreshCache(PrefabRef selectedPrefab)
    {
        cachedFor = entity;
        framesSinceRefresh = 0;
        cachedPositions.Clear();

        using NativeArray<Entity> entities = samePrefabQuery.ToEntityArray(Allocator.Temp);
        using NativeArray<PrefabRef> prefabs = samePrefabQuery.ToComponentDataArray<PrefabRef>(
            Allocator.Temp
        );
        using NativeArray<Game.Objects.Transform> transforms =
            samePrefabQuery.ToComponentDataArray<Game.Objects.Transform>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i] == entity || prefabs[i].m_Prefab != selectedPrefab.m_Prefab)
                continue;

            cachedPositions.Add(transforms[i].m_Position);
        }
    }

    private void DrawRadius(
        OverlayRenderSystem.Buffer buffer,
        Color outline,
        Color fill,
        float3 position
    )
    {
        buffer.DrawCircle(
            outline,
            fill,
            1.0f,
            OverlayRenderSystem.StyleFlags.Projected,
            new float2(0f, 1f),
            position,
            radius * 2f
        );
    }

    public void Show(Entity entity, int local, int type, float radius)
    {
        this.entity = entity;
        this.local = local;
        this.type = type;
        this.radius = radius;
        cachedFor = Entity.Null;
    }

    public void Hide()
    {
        type = -1;
        local = -1;
        cachedFor = Entity.Null;
        cachedPositions.Clear();
    }

    public Color GetColor() => color;

    public void SetColor(Color color)
    {
        Mod.m_Setting.EffectOverlayR = color.r;
        Mod.m_Setting.EffectOverlayG = color.g;
        Mod.m_Setting.EffectOverlayB = color.b;
        Mod.m_Setting.EffectOverlayA = color.a;
        UpdateColor();
    }

    public void UpdateColor()
    {
        color = new(
            Mod.m_Setting.EffectOverlayR,
            Mod.m_Setting.EffectOverlayG,
            Mod.m_Setting.EffectOverlayB,
            Mod.m_Setting.EffectOverlayA
        );
        Update();
    }

    public void SetRadiusCommon(bool radiusCommon)
    {
        this.radiusCommon = radiusCommon;
        Update();
    }
}
