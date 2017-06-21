﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Action SubPhase";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            CanBePaused = true;
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            if (!Selection.ThisShip.IsSkipsActionSubPhase)
            {
                if (!Selection.ThisShip.IsDestroyed)
                {
                    Selection.ThisShip.GenerateAvailableActionsList();
                    Triggers.AddTrigger("Action", TriggerTypes.OnActionSubPhaseStart, Roster.GetPlayer(Phases.CurrentPhasePlayer).PerformAction, Selection.ThisShip, Phases.CurrentPhasePlayer);
                }
                else
                {
                    //Next();
                }
            }
            else
            {
                Selection.ThisShip.IsSkipsActionSubPhase = false;
                //Next();
            }

            Phases.CallOnActionSubPhaseTrigger();
        }

        public override void Next()
        {
            Selection.ThisShip.CallAfterActionIsPerformed(this.GetType());

            if (Phases.CurrentSubPhase.GetType() == this.GetType())
            {
                FinishPhase();
            }
        }

        public override void Pause()
        {
            Actions.CloseActionsPanel();
        }

        public override void Resume()
        {
            Actions.ShowActionsPanel();
        }

        public override void FinishPhase()
        {
            GenericSubPhase activationSubPhase = new ActivationSubPhase();
            Phases.CurrentSubPhase = activationSubPhase;
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
            Phases.CurrentSubPhase.RequiredPlayer = RequiredPlayer;

            Phases.CurrentSubPhase.Next();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}

namespace SubPhases
{

    public class ActionDecisonSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Select action";

            foreach (var action in Selection.ThisShip.GetAvailableActionsList())
            {
                decisions.Add(action.Name, delegate {
                    Tooltips.EndTooltip();
                    Selection.ThisShip.AddAlreadyExecutedAction(action);
                    action.ActionTake();
                    Game.UI.HideNextButton();
                    Phases.FinishSubPhase(this.GetType());
                });
            }

            Game.UI.ShowSkipButton();
        }

    }

}
