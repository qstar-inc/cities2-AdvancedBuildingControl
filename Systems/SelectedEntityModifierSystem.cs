using System;
using System.Linq;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Events;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SelectedEntityModifierSystem : GameSystemBase
    {
        public IconCommandSystem iconCommandSystem;
        public IconCommandBuffer iconCommandBuffer;

        public Utils utils;

        protected override void OnCreate()
        {
            utils = WorldHelper.GetSystem<Utils>();
            iconCommandSystem = WorldHelper.IconCommandSystem;
        }

        protected override void OnUpdate() { }

        public void RandomizeStyle(Entity entity)
        {
            try
            {
                if (!EntityManager.TryGetComponent(entity, out PseudoRandomSeed pseudoRandomSeed))
                    return;

                Random random = new();
                ushort randomUShort = (ushort)random.Next(0, 65536);
                pseudoRandomSeed.m_Seed = randomUShort;

                utils.AddAndUpdate(entity, pseudoRandomSeed);
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void ChangeBrand(Entity entity, string replaceBrand)
        {
            try
            {
                var match = DataRetriever.brandDataInfos.FirstOrDefault(b =>
                    b.PrefabName == replaceBrand
                );
                if (!EntityManager.Exists(entity) || replaceBrand == string.Empty || match == null)
                    return;

                EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Renter> renters);

                for (int i = 0; i < renters.Length; i++)
                {
                    Renter renter = renters[i];
                    Entity renterEntity = renter.m_Renter;

                    if (
                        !EntityManager.TryGetComponent(renterEntity, out CompanyData companyData)
                        && companyData.m_Brand.Equals(Entity.Null)
                    )
                        continue;

                    companyData.m_Brand = match.Entity;

                    utils.SetAndUpdate(renterEntity, entity, companyData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void ChangeCleanupValue(Entity entity, string value, BldgCleanupType resetType)
        {
            switch (resetType)
            {
                case BldgCleanupType.Garbage:
                    if (!EntityManager.TryGetComponent(entity, out GarbageProducer garbageProducer))
                        return;

                    if (!int.TryParse(value, out int garbageValue))
                        return;

                    garbageProducer.m_Garbage = Math.Clamp(garbageValue, 0, 20000);

                    if (garbageValue == 0)
                    {
                        EntityQuery garbageParamQuery = SystemAPI
                            .QueryBuilder()
                            .WithAll<GarbageParameterData>()
                            .Build();
                        GarbageParameterData garbageParams =
                            garbageParamQuery.GetSingleton<GarbageParameterData>();

                        iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                        iconCommandBuffer.Remove(entity, garbageParams.m_GarbageNotificationPrefab);

                        Entity garbageRequest = garbageProducer.m_CollectionRequest;
                        if (garbageRequest != null)
                            EntityManager.AddComponent<Deleted>(garbageRequest);
                    }
                    WorldHelper.GetSystem<GarbageAccumulationSystem>().Update();

                    utils.SetAndUpdate(entity, garbageProducer);

                    break;

                case BldgCleanupType.Crime:
                    if (!EntityManager.TryGetComponent(entity, out CrimeProducer crimeProducer))
                        return;

                    if (!float.TryParse(value, out float crimeValue))
                        return;

                    crimeProducer.m_Crime = Math.Clamp(crimeValue, 0f, 25000f);
                    if (crimeValue == 0)
                    {
                        EntityQuery crimeParamQuery = SystemAPI
                            .QueryBuilder()
                            .WithAll<PoliceConfigurationData>()
                            .Build();
                        PoliceConfigurationData crimeParams =
                            crimeParamQuery.GetSingleton<PoliceConfigurationData>();

                        iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                        iconCommandBuffer.Remove(
                            entity,
                            crimeParams.m_CrimeSceneNotificationPrefab
                        );

                        if (EntityManager.TryGetComponent(entity, out AccidentSite accidentSite))
                        {
                            Entity policeRequest = accidentSite.m_PoliceRequest;
                            if (policeRequest != null)
                                EntityManager.AddComponent<Deleted>(policeRequest);
                        }
                    }

                    utils.SetAndUpdate(entity, crimeProducer);
                    break;

                case BldgCleanupType.OutgoingMail:
                    if (!EntityManager.TryGetComponent(entity, out MailProducer mailProducer))
                        return;

                    if (!ushort.TryParse(value, out ushort mailValue))
                        return;

                    mailProducer.m_SendingMail = Math.Max(mailValue, (ushort)0);

                    utils.SetAndUpdate(entity, mailProducer);
                    break;

                case BldgCleanupType.PhysicalDamage:
                case BldgCleanupType.FireDamage:
                case BldgCleanupType.WaterDamage:
                    if (!EntityManager.TryGetComponent(entity, out Damaged damaged))
                        return;

                    if (!float.TryParse(value, out float damageValue))
                        return;

                    damageValue = Math.Clamp(damageValue / 100f, 0f, 1f);

                    if (resetType == BldgCleanupType.PhysicalDamage)
                    {
                        damaged.m_Damage.x = damageValue;

                        if (damageValue == 0)
                        {
                            EntityQuery disasterParamQuery = SystemAPI
                                .QueryBuilder()
                                .WithAll<DisasterConfigurationData>()
                                .Build();
                            DisasterConfigurationData disasterParams =
                                disasterParamQuery.GetSingleton<DisasterConfigurationData>();

                            iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                            iconCommandBuffer.Remove(
                                entity,
                                disasterParams.m_WeatherDamageNotificationPrefab
                            );
                        }
                    }
                    else if (resetType == BldgCleanupType.FireDamage)
                    {
                        damaged.m_Damage.y = damageValue;

                        if (damageValue == 0)
                        {
                            EntityQuery disasterParamQuery = SystemAPI
                                .QueryBuilder()
                                .WithAll<FireConfigurationData>()
                                .Build();
                            FireConfigurationData disasterParams =
                                disasterParamQuery.GetSingleton<FireConfigurationData>();

                            iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                            iconCommandBuffer.Remove(
                                entity,
                                disasterParams.m_FireNotificationPrefab
                            );
                        }
                    }
                    else if (resetType == BldgCleanupType.WaterDamage)
                    {
                        damaged.m_Damage.z = damageValue;

                        if (damageValue == 0)
                        {
                            EntityQuery disasterParamQuery = SystemAPI
                                .QueryBuilder()
                                .WithAll<DisasterConfigurationData>()
                                .Build();
                            DisasterConfigurationData disasterParams =
                                disasterParamQuery.GetSingleton<DisasterConfigurationData>();

                            iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();

                            iconCommandBuffer.Remove(
                                entity,
                                disasterParams.m_WaterDamageNotificationPrefab
                            );
                        }
                    }

                    utils.SetAndUpdate(entity, damaged);
                    break;

                default:
                    break;
            }
        }

        public void TriggerCleanup(Entity entity, BldgCleanupType resetType)
        {
            switch (resetType)
            {
                case BldgCleanupType.ClearAbandonment:
                    RemoveAbandonment(entity);
                    break;

                case BldgCleanupType.ClearNotification:
                    RemoveNotifications(entity);
                    break;

                default:
                    break;
            }
        }

        // Original from Plop The Growable
        public void RemoveAbandonment(Entity entity)
        {
            EntityQuery bcQuery = SystemAPI
                .QueryBuilder()
                .WithAll<BuildingConfigurationData>()
                .Build();

            BuildingConfigurationData bcData = bcQuery.GetSingleton<BuildingConfigurationData>();
            Entity abandonedNotification = bcData.m_AbandonedNotification;

            EntityManager.RemoveComponent<Abandoned>(entity);

            EntityManager.AddComponent<PropertyToBeOnMarket>(entity);
            if (EntityManager.HasComponent<PropertyOnMarket>(entity))
                EntityManager.RemoveComponent<PropertyOnMarket>(entity);

            if (EntityManager.HasComponent<BuildingCondition>(entity))
                EntityManager.SetComponentData(entity, new BuildingCondition { m_Condition = 0 });

            EntityManager.AddComponentData(entity, default(GarbageProducer));
            EntityManager.AddComponentData(entity, default(MailProducer));
            EntityManager.AddComponentData(entity, default(ElectricityConsumer));
            EntityManager.AddComponentData(entity, default(WaterConsumer));

            iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();
            iconCommandBuffer.Remove(entity, abandonedNotification);

            if (
                EntityManager.TryGetComponent(entity, out Building building)
                && building.m_RoadEdge != Entity.Null
            )
                EntityManager.AddComponent<Updated>(building.m_RoadEdge);
        }

        public void RemoveNotifications(Entity entity)
        {
            if (
                !EntityManager.TryGetBuffer(
                    entity,
                    false,
                    out DynamicBuffer<IconElement> iconBuffer
                )
            )
                return;

            EntityQuery tcQuery = SystemAPI
                .QueryBuilder()
                .WithAll<TrafficConfigurationData>()
                .Build();
            TrafficConfigurationData m_TrafficConfigurationData =
                tcQuery.GetSingleton<TrafficConfigurationData>();

            iconCommandBuffer = iconCommandSystem.CreateCommandBuffer();
            using var icons = iconBuffer.ToNativeArray(Allocator.Temp);
            foreach (var iconElement in icons)
            {
                if (!EntityManager.TryGetComponent(iconElement.m_Icon, out PrefabRef prefabRef))
                    continue;
                Entity prefab = prefabRef.m_Prefab;
                if (
                    (
                        prefab == m_TrafficConfigurationData.m_CarConnectionNotification
                        || prefab == m_TrafficConfigurationData.m_ShipConnectionNotification
                        || prefab == m_TrafficConfigurationData.m_PedestrianConnectionNotification
                        || prefab == m_TrafficConfigurationData.m_TrainConnectionNotification
                        || prefab == m_TrafficConfigurationData.m_RoadConnectionNotification
                        || prefab == m_TrafficConfigurationData.m_BicycleConnectionNotification
                    ) && EntityManager.TryGetComponent(iconElement.m_Icon, out Target target)
                )
                {
                    if (
                        !EntityManager.TryGetComponent(
                            iconElement.m_Icon,
                            out Game.Notifications.Icon icon
                        )
                    )
                        continue;
                    IconFlags iconFlags = icon.m_Flags & IconFlags.SecondaryLocation;
                    iconCommandBuffer.Remove(entity, prefab, target.m_Target, iconFlags);
                }
                else
                {
                    iconCommandBuffer.Remove(entity, prefab);
                }

                LogHelper.SendLog(
                    $"Removed notification '{WorldHelper.PrefabSystem.GetPrefabName(prefab)}'",
                    LogLevel.DEVD
                );
            }
        }
    }
}
