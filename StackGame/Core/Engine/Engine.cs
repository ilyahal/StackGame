﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using StackGame.Commands;
using StackGame.Strategy;
using StackGame.Army;
using StackGame.Units;
using StackGame.Units.Abilities;

namespace StackGame.Core.Engine
{
	/// <summary>
    /// Движок игры
    /// </summary>
    public class Engine
    {
        #region Свойства

        /// <summary>
        /// Стратегия боя
        /// </summary>
        public IStrategy Strategy = new Strategy1Vs1();

        /// <summary>
        /// Менеджер команд
        /// </summary>
        public readonly CommandManager CommandManager = new CommandManager();

        /// <summary>
        /// Экземпляр класса
        /// </summary>
        private static Engine instance;

        /// <summary>
        /// Первая армия
        /// </summary>
        private readonly IArmy firstArmy = new Army.Army("Белая");
        /// <summary>
        /// Вторая армия
        /// </summary>
        private readonly IArmy secondArmy = new Army.Army("Черная");

        #endregion

        #region Инициализация

        private Engine()
        { }

        #endregion

        #region Методы

        /// <summary>
        /// Получить экземпляр класса
        /// </summary>
        public static Engine GetInstance()
        {
            if (instance == null)
            {
                instance = new Engine();
            }

            return instance;
        }

        /// <summary>
        /// Следующий ход
        /// </summary>
        public bool NextStep() {
            if (firstArmy.IsAllDead || secondArmy.IsAllDead) 
            {
                return false;
            }

            Console.WriteLine("Состояние \"до\":");
            Console.WriteLine(firstArmy);
            Console.WriteLine(secondArmy);

            MeleeAttack();
            Console.WriteLine();
            SpecialAbilityAttack();
            Console.WriteLine();
            CollectDeadUnits();

			Console.WriteLine("Состояние \"после\":");
            Console.WriteLine(firstArmy);
			Console.WriteLine(secondArmy);

            CommandManager.EndTurn();

            return true;
        }

        /// <summary>
        /// Рукопашная атака
        /// </summary>
        private void MeleeAttack()
        {
            var queue = Strategy.GetOpponentsQueue(firstArmy, secondArmy);
			foreach (var opponents in queue)
			{
				Hit(opponents.Unit, opponents.EnemyUnit);
			}
        }

		/// <summary>
		/// Атаковать противника
		/// </summary>
		private void Hit(IUnit unit, IUnit enemyUnit)
		{
			if (unit.IsAlive && unit.Strength > 0 && enemyUnit.IsAlive)
			{
                var command = new HitCommand(unit, enemyUnit, unit.Strength);
				CommandManager.Execute(command);
			}
		}

        /// <summary>
        /// Атака с применением специальных возможностей
        /// </summary>
        private void SpecialAbilityAttack()
        {
            var firstArmyUnitsCount = firstArmy.Units.Count;
            var firstArmyUnitIndex = 0;

            var secondArmyUnitsCount = secondArmy.Units.Count;
            var secondArmyUnitIndex = 0;

            while (firstArmyUnitIndex < firstArmyUnitsCount || secondArmyUnitIndex < secondArmyUnitsCount)
            {
                var _components = new List<SpecialAbilityComponents>();

                var firstArmyComponents = TryGetSpecialAbilityComponents(firstArmy, secondArmy, firstArmyUnitsCount, ref firstArmyUnitIndex);
                if (firstArmyComponents.HasValue)
                {
                    var specialAbilityComponents = firstArmyComponents.Value;
                    _components.Add(specialAbilityComponents);
                }

				var secondArmyComponents = TryGetSpecialAbilityComponents(secondArmy, firstArmy, secondArmyUnitsCount, ref secondArmyUnitIndex);
				if (secondArmyComponents.HasValue)
				{
					var specialAbilityComponents = secondArmyComponents.Value;
					_components.Add(specialAbilityComponents);
				}

                if (_components.Count == 0)
                {
                    continue;
                }
                if (_components.Count > 1)
                {
                    _components = _components.Randomize().ToList();
                }

                foreach (var components in _components)
                {
                    ApplySpecialAbility(components.Army, components.Range, components.Unit, components.Position);
                }
            }
        }

        /// <summary>
        /// Пытаемся получить компоненты для применения специальных возможностей
        /// </summary>
        private SpecialAbilityComponents? TryGetSpecialAbilityComponents(IArmy army, IArmy enemyArmy, int armyUnitsCount, ref int armyUnitIndex)
        {
            if (armyUnitIndex < armyUnitsCount)
            {
                var tmpArmyUnitIndex = armyUnitIndex;
                armyUnitIndex++;

				var specialUnit = TryGetSpecialAbilityUnit(army, tmpArmyUnitIndex);
                if (specialUnit != null)
                {
                    var range = Strategy.GetUnitsRangeForSpecialAbility(army, enemyArmy, specialUnit, tmpArmyUnitIndex);
					if (range != null)
					{
                        var targetArmy = specialUnit.IsFriendly ? army : enemyArmy;

						var components = new SpecialAbilityComponents(specialUnit, targetArmy, range, tmpArmyUnitIndex);
						return components;
					}
                }
            }

            return null;
        }

        /// <summary>
        /// Пытаемся получить единицу армии, обладающую специальными возможностями
        /// </summary>
        private ISpecialAbility TryGetSpecialAbilityUnit(IArmy army, int unitPosition)
        {
            var unit = army.Units[unitPosition];
            if (unit.IsAlive && unit is ISpecialAbility specialUnit)
            {
                return specialUnit;
            }

            return null;
        }

        /// <summary>
        /// Применить специальную возможность
        /// </summary>
        private void ApplySpecialAbility(IArmy targetArmy, IEnumerable<int> range, ISpecialAbility unit, int unitPosition)
        {
			Console.WriteLine($"\ud83d\udd75️ #{ unit }# проверяет unit с индексами { string.Join(", ", range.ToArray()) }");
			unit.DoSpecialAction(targetArmy, range, unitPosition);
        }

		/// <summary>
		/// Удалить мертвые единицы армии
		/// </summary>
        private void CollectDeadUnits()
		{
			CollectDeadUnits(firstArmy);
			CollectDeadUnits(secondArmy);
		}

        /// <summary>
        /// Удалить мертвые единицы армии
        /// </summary>
        private void CollectDeadUnits(IArmy army)
        {
            var command = new CollectDeadCommand(army);
            CommandManager.Execute(command);
        }

		#endregion
	}
}
