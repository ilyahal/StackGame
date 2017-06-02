﻿using System;
using StackGame.Units.Abilities;

namespace StackGame.Units.Improvements
{
    /// <summary>
	/// Щит
	/// </summary>
	public class ShieldUnitImprove<T> : UnitImprove<T> where T : IUnit, IImprovable, IClonable
	{
		#region Свойства

		/// <summary>
		/// Защита щита
		/// </summary>
		private int shieldDefence = 30;

		public override int Defence
		{
			get
			{
				return base.Defence + shieldDefence;
			}
		}

		#endregion

		#region Инициализация

		public ShieldUnitImprove(T unit) : base(unit)
		{ }

		#endregion

		#region Методы

		public override IUnit Clone()
		{
			var clonedUnit = (T)unit.Clone();
			var improvedClonedUnit = new ShieldUnitImprove<T>(clonedUnit);

			return improvedClonedUnit;
		}

		public override void GetDamage(int damage)
		{
			if (shieldDefence > 0)
			{
				shieldDefence -= damage;
				if (shieldDefence < 0)
				{
					base.GetDamage(Math.Abs(shieldDefence));
					shieldDefence = 0;
				}
			}
			else
			{
				base.GetDamage(damage);
			}
		}

		public override string ToString()
		{
            return $"{ base.ToString() } |щит { shieldDefence }|";
		}

		#endregion
	}
}
