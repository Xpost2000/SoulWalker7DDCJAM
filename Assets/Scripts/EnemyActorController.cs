using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  Enemy Actor holder generally.

  Wraps some common enemy actor logic that's
  needed for the game.
*/
public class EnemyActorController : MonoBehaviour
{
    public delegate void OnIncapacitationRecover();
    public delegate void OnIncapacitation();

    public event GameManagerScript.OnTurnStart on_turn_start;
    public event GameManagerScript.OnTurnEnd on_turn_end;

    public event OnIncapacitation on_incapacitate;
    public event OnIncapacitationRecover on_incapacitate_recover;

    // Set flags to determine which form must die in order to kill the enemy
    public ActorState death_if_form_dies;

    // if an actor has a "form" die, they are incapacitated
    // until this runs out.
    public int max_incapacitation_turns = 2;
    int incapacitation_turns = 0;

    private GenericActorController controller;

    void Start() {
        controller = GetComponent<GenericActorController>();
        controller.on_hurt += OnHurt;
        controller.on_death += OnDeath;
        GameManagerScript.instance().on_turn_start += OnTurnStart;
        GameManagerScript.instance().on_turn_end += OnTurnEnd;
    }

    void OnDestroy() {
        GameManagerScript.instance().on_turn_start -= OnTurnStart;
        GameManagerScript.instance().on_turn_end   -= OnTurnEnd;
    }

    void OnTurnStart() {
        if (incapacitation_turns <= 0)
            on_turn_start?.Invoke();
    }
    
    void OnTurnEnd() {
        if (incapacitation_turns <= 0)
            on_turn_end?.Invoke();

        incapacitation_turns -= 1;

        if (incapacitation_turns == 0) {
            on_incapacitate_recover?.Invoke();
            if ((death_if_form_dies & ActorState.Soul) != 0)
                controller.health = controller.max_health;
            if ((death_if_form_dies & ActorState.Body) != 0)
                controller.soul_health = controller.max_soul_health;
        }
    }

    void OnHurt(int amount, ActorState form) {
        GameManagerScript.instance().MessageLog.NewMessage(
            gameObject.name + " has been hurt for " + amount.ToString(), Color.white
        );
    }

    void OnDeath(ActorState state) {
        if ((state & death_if_form_dies) != 0) {
            // perma death?!
            // play death animation?
            // puff away!
            GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " has died!", Color.white);
            // for now just disappear.
            Destroy(gameObject);
        } else {
            // wrong death form!
            incapacitation_turns = max_incapacitation_turns;
            GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " has incapacitated!", Color.white);
            on_incapacitate?.Invoke();
        }
    }
}
