/*
  This is a very generic weapon
  behavior that I can use to quickly
  make weapon attack behaviors that
  don't have any specialized behaviors...

  NOTE: this is really to do with melee weapons,
  and there's only a few weapons that have special behaviors.
*/

using UnityEngine;

public class GenericWeaponAttackBehavior : MonoBehaviour {
    private WeaponDataScript weapon_data_script;
    public int physical_damage = 5;

    // NOTE: weapons can theoretically "heal"
    // and this makes -1 not a valid value. Which is
    // fine I guess.
    public int soul_damage = -1;
    public bool must_go_through_body = true;

    void Start() {
        if (soul_damage == -1) soul_damage = physical_damage;
        weapon_data_script = GetComponent<WeaponDataScript>();

        weapon_data_script.on_attack_damage += OnDamageHitEntity;
        weapon_data_script.on_attack_hit    += OnDamageHitAny;
    }

    void OnDestroy() {
        weapon_data_script.on_attack_damage -= OnDamageHitEntity;
        weapon_data_script.on_attack_hit    -= OnDamageHitAny;
    }

    void OnDamageHitAny(GameObject victim) {
        // ?
    }

    void OnDamageHitEntity(GameObject victim) {
        print("I'm going to hurt someone!");
        var victim_controller =
            victim.GetComponent<GenericActorController>();

        victim_controller.HurtBody(physical_damage);
        victim_controller.HurtSoul(soul_damage);
        if (weapon_data_script.holder.GetComponent<GenericActorController>().form == ActorState.Soul) {
            // this makes soul mode much better at killing things.
            victim_controller.HurtSoul(physical_damage);
        }
    }
}
