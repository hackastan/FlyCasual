﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using Movement;
using UnityEngine;
using Content;

namespace UpgradesList.SecondEdition
{
    public class SeasonedNavigator : GenericUpgrade
    {
        public SeasonedNavigator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seasoned Navigator",
                UpgradeType.Crew,
                cost: 7,
                abilityType: typeof(Abilities.SecondEdition.SeasonedNavigatorAbility),
                seImageNumber: 47,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(369, 24),
                new Vector2(125, 125)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeasonedNavigatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += ActivateSeasonedNavigator;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= ActivateSeasonedNavigator;
        }

        private void ActivateSeasonedNavigator(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToChangeManeuver);
        }

        private void AskToChangeManeuver(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                ShowAvailableManeuver,
                descriptionLong: "Do you want to set your dial to another non-red maneuver of the same speed? (While you execute that maneuver, increase its difficulty)",
                imageHolder: HostUpgrade
            );
        }

        private void ShowAvailableManeuver(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Owner.ChangeManeuver(AssignManeuverWithIncreasedComplexity, Triggers.FinishTrigger, IsSameSpeedNotRed);
        }

        private void AssignManeuverWithIncreasedComplexity(string maneuverCode)
        {
            if (maneuverCode != HostShip.AssignedManeuver.ToString())
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": The difficulty of the maneuver has been increased");

                HostShip.AfterGetManeuverColorIncreaseComplexity += ChangeComplexity;
                HostShip.OnMovementFinish += ClearAbility;
            }

            ShipMovementScript.SendAssignManeuverCommand(maneuverCode);
        }

        private bool IsSameSpeedNotRed(string maneuverString)
        {
            bool result = false;

            if (maneuverString == HostShip.AssignedManeuver.ToString()) return true;

            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed && movementStruct.ColorComplexity != MovementComplexity.Complex)
            {
                result = true;
            }
            return result;
        }

        private void ChangeComplexity(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.ToString() == HostShip.AssignedManeuver.ToString()) return;

            movement.ColorComplexity = GenericMovement.IncreaseComplexity(movement.ColorComplexity);
        }

        private void ClearAbility(GenericShip ship)
        {
            HostShip.AfterGetManeuverColorIncreaseComplexity -= ChangeComplexity;
            HostShip.OnMovementFinish -= ClearAbility;
        }
    }
}