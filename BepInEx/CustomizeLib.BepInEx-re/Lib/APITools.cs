using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx
{
    #region API 用的结构体
    public struct CustomPlant(
        [NotNull] ID id, [NotNull] GameObject prefab, [NotNull] GameObject preview, [NotNull] List<(ID a, ID b)> fusions,
        float attackInterval, float produceInterval, int attackDamage, int maxHealth, float cd, int sun)
    {
        [NotNull] public ID Id { get; set; } = id;
        [NotNull] public GameObject Prefab { get; set; } = prefab;
        [NotNull] public GameObject Preview { get; set; } = preview;
        [NotNull] public List<(ID a, ID b)> Fusions { get; set; } = fusions;
        [NotNull] public List<(BulletType bullet, GameObject prefab)> BulletSkins { get; set; } = [];
        public float AttackInterval { get; set; } = attackInterval;
        public float ProduceInterval { get; set; } = produceInterval;
        public int AttackDamage { get; set; } = attackDamage;
        public int MaxHealth { get; set; } = maxHealth;
        public float CD { get; set; } = cd;
        public int Sun { get; set; } = sun;

        internal readonly PlantDataManager.PlantData ToPlantData()
        {
            return new()
            {
                attackDamage = AttackDamage,
                attackInterval = AttackInterval,
                cd = CD,
                cost = Sun,
                maxHealth = MaxHealth,
                produceInterval = ProduceInterval,
                thePlantType = Id
            };
        }
    }

    public readonly struct ID(int id)
    {
        // data
        private readonly int id = id;

        // cons
        public ID(PlantType id) : this((int)id) { }
        public ID(ZombieType id) : this((int)id) { }

        // ID -> other
        public static implicit operator int(ID id) => id.id;
        public static implicit operator PlantType(ID id) => id.id.ToEnum<PlantType>();
        public static implicit operator ZombieType(ID id) => id.id.ToEnum<ZombieType>();

        // other -> ID
        public static implicit operator ID(int id) => new(id);
        public static implicit operator ID(PlantType id) => new(id);
        public static implicit operator ID(ZombieType id) => new(id);
    }
    #endregion

    #region API 的工具
    public enum PlantTags
    {
        BigNut,
        DoubleBoxPlants,
        FlyingPlants,
        IsCaltrop,
        IsClassicPlant,
        IsFirePlant,
        IsIcePlant,
        IsLily,
        IsMagnetPlants,
        IsNut,
        IsPickaxeStar,
        IsPlantern,
        IsPot,
        IsPotatoMine,
        IsPuff,
        IsPumpkin,
        IsPurplePlant,
        IsSmallRangePlantern,
        IsSnowPlant,
        IsSpickRock,
        IsTallNut,
        IsTangkelp,
        IsTorch,
        IsVirtualPlant,
        IsWaterPlant,
        UmbrellaPlants,
        RedPlant,
        UncrashablePlants,
        WhiteCardPlants
    }

    public enum ZombieTags
    {
        BannedInRandomZombies,
        BigZombie,
        EliteZombie,
        IsAirShipZombie,
        IsAirZombie,
        IsBossZombie,
        IsDriverZombie,
        IsGargantuar,
        IsLeaderZombie,
        NotRandomBungiZombie,
        UltimateZombie,
        UltiZombie_level,
        UltiZombie_level_water,
        UselessHypnoZombie,
        UltiZombie_level_a,
        UltiZombie_level_b,
        UltiZombie_level_c
    }
    #endregion
}
