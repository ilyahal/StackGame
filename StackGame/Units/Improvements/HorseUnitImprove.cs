﻿using System;
using StackGame.Core.Configs;
using StackGame.Units.Abilities;

namespace StackGame.Units.Improvements
{
    /// <summary>
	/// Лошадь
	/// </summary>
	public class HorseUnitImprove<T> : UnitImprove<T> where T : IUnit, IImprovable, IClonable
	{
        #region Свойства

        /// <summary>
        /// Жизнь лошади
        /// </summary>
        private readonly int horseDefence;
        /// <summary>
        /// Сила лошади
        /// </summary>
        private readonly int horseStrength;

		public override int Defence => base.Defence + horseDefence;

		public override int Strength => base.Strength + horseStrength;

		#endregion

		#region Инициализация

		public HorseUnitImprove(T unit) : base(unit)
		{
            var parameters = Configs.UnitImproves[UnitImproveType.Horse];
            horseDefence = parameters.Defence;
            horseStrength = parameters.Strength;
        }

		#endregion

		#region Методы

		public override IUnit Clone()
		{
			var clonedUnit = (T)unit.Clone();
			var improvedClonedUnit = new HorseUnitImprove<T>(clonedUnit);

			return improvedClonedUnit;
		}

		public override void TakeDamage(int damage)
		{
			if (horseHealth > 0)
			{
				horseHealth -= damage;
				if (horseHealth < 0)
				{
					base.TakeDamage(Math.Abs(horseHealth));
					horseHealth = 0;
				}
			}
			else
			{
				base.TakeDamage(damage);
			}
		}

		public override string ToString()
		{
            return $"{ base.ToString() } |лошадь - защита { horseDefence }, атака { horseStrength }|";
		}

		#endregion
	}
}
