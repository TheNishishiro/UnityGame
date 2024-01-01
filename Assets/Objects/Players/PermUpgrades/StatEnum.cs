﻿namespace Objects.Players.PermUpgrades
{
    public enum StatEnum
    {
        Health,
        HealthMax,
        SpecialMax,
        SpecialIncrease,
        MagnetSize,
        CooldownReduction,
        [StatType(true)]
        CooldownReductionPercentage,
        AttackCount,
        Damage,
        [StatType(true)]
        Scale,
        [StatType(true)]
        Speed,
        TimeToLive,
        DetectionRange,
        [StatType(true)]
        DamagePercentageIncrease,
        [StatType(true)]
        ExperienceIncreasePercentage,
        MovementSpeed,
        [StatType(true)]
        SkillCooldownReductionPercentage,
        HealthRegen,
        [StatType(true)]
        CritRate,
        [StatType(true)]
        CritDamage,
        PassThroughCount,
        Armor,
        [StatType(true)]
        EnemySpeedIncreasePercentage,
        [StatType(true)]
        EnemySpawnRateIncreasePercentage,
        [StatType(true)]
        EnemyHealthIncreasePercentage,
        [StatType(true)]
        EnemyMaxCountIncreasePercentage,
        [StatType(true)]
        ItemRewardIncrease,
        Revives,
        [StatType(true)]
        ProjectileLifeTimeIncreasePercentage,
        [StatType(true)]
        DodgeChance,
        [StatType(true)]
        DamageTakenIncreasePercentage,
        [StatType(true)]
        HealingIncreasePercentage,
        [StatType(true)]
        Luck,
        DamageOverTime,
        Rerolls,
        Skips,
        [StatType(true)]
        DamageOverTimeFrequencyReduction,
        [StatType(true)]
        DamageOverTimeDurationIncrease,
        [StatType(true)]
        LifeSteal
    }
}