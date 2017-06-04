﻿using System;
using System.Collections.Generic;
using System.Linq;
using StackGame.Core.Engine;
using StackGame.Army;
using StackGame.Units.Abilities;

namespace StackGame.Strategy
{
    /// <summary>
    /// Стратегия "все со всеми"
    /// </summary>
    public class StrategyAllVsAll : Strategy
    {
		#region Методы

        public override List<MeleeOpponents> GetOpponentsQueue(IArmy firstArmy, IArmy secondArmy)
        {
			var opponents = new MeleeOpponents(firstArmy.Units[0], secondArmy.Units[0]);
			var queue = new List<MeleeOpponents>
			{
				opponents,
				opponents.Reverse()
			};

            queue = queue.Randomize().ToList();

            return queue;
        }

		public override IEnumerable<int> GetUnitsRangeForSpecialAbility(IArmy army, ISpecialAbility unit, int unitPosition)
        {
			var isFirst = unitPosition == 0;
			if (isFirst)
			{
                return null;
			}

			var radius = unit.Range;

			Tuple<int, int> bounds;
			if (unit.IsFriendly)
			{
                bounds = GetBoundsInAllyArmy(army, unitPosition, radius);
			}
			else
			{
				if (unitPosition - radius >= 0)
				{
                    return null;
				}

				bounds = GetBoundsInEmemyArmy(army, unitPosition, radius);
			}

			var startIndex = bounds.Item1;
			var endIndex = bounds.Item2;

			var count = endIndex - startIndex + 1;

			if (count == 0)
			{
				return null;
			}

			var range = Enumerable.Range(startIndex, count);
            return range;
        }

		protected override Tuple<int, int> GetBoundsInAllyArmy(IArmy army, int unitPosition, int unitRange)
		{
			var startIndex = unitPosition - unitRange;
			if (startIndex < 0)
			{
				startIndex = 0;
			}

			var endIndex = unitPosition + unitRange;
			if (endIndex >= army.Units.Count)
			{
				endIndex = army.Units.Count - 1;
			}

			return new Tuple<int, int>(startIndex, endIndex);
		}

		protected override Tuple<int, int> GetBoundsInEmemyArmy(IArmy army, int unitPosition, int unitRange)
        {
			var startIndex = 0;
			var endIndex = Math.Abs(unitPosition - unitRange) - 1;

			if (endIndex >= army.Units.Count)
			{
				endIndex = army.Units.Count - 1;
			}

            return new Tuple<int, int>(startIndex, endIndex);
        }

		#endregion
	}
}