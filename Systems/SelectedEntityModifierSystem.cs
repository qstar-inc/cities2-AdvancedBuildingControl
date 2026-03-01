using System;
using System.Linq;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Objects;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SelectedEntityModifierSystem : GameSystemBase
    {
        Utils utils;

        protected override void OnCreate()
        {
            utils = WorldHelper.GetSystem<Utils>();
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

                    garbageProducer.m_Garbage = Math.Min(garbageValue, 0);
                    utils.SetAndUpdate(entity, garbageProducer);
                    break;

                case BldgCleanupType.Crime:
                    if (!EntityManager.TryGetComponent(entity, out CrimeProducer crimeProducer))
                        return;

                    if (!float.TryParse(value, out float crimeValue))
                        return;

                    crimeProducer.m_Crime = Math.Min(crimeValue, 0f);
                    utils.SetAndUpdate(entity, crimeProducer);
                    break;

                case BldgCleanupType.OutgoingMail:
                    if (!EntityManager.TryGetComponent(entity, out MailProducer mailProducer))
                        return;

                    if (!ushort.TryParse(value, out ushort mailValue))
                        return;

                    mailProducer.m_SendingMail = Math.Min(mailValue, (ushort)0);

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
                        damaged.m_Damage.x = damageValue;
                    else if (resetType == BldgCleanupType.FireDamage)
                        damaged.m_Damage.y = damageValue;
                    else if (resetType == BldgCleanupType.WaterDamage)
                        damaged.m_Damage.z = damageValue;

                    utils.SetAndUpdate(entity, damaged);
                    break;

                default:
                    break;
            }
        }
    }
}
