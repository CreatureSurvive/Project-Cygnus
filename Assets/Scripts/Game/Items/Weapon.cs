﻿using UnityEngine;
using System.Collections;

public sealed class Weapon : MonoBehaviour {
	private Weapon_Template m_template = null;
	private MeshFilter m_filter;

	public Weapon_Template template {

		get {

			return m_template;
		}
		internal set {

			m_template = value;
			m_template.game_object = gameObject;
			m_template.weapon_object = this;
		}
	}

	public MeshFilter filter {

		get {

			return m_filter;
		}
		internal set {

			m_filter = value;
		}
	}

	#region Ownership

	internal Entity m_NPC_target = null;
	internal NPC m_NPC_owner = null;
	internal Player_Controller m_player_owner = null;

	public Entity NPC_target {

		get {

			return m_NPC_target;
		}
	}

	public NPC NPC_owner {

		get {

			return m_NPC_owner;
		}
		internal set {

			m_NPC_owner = value;
		}
	}

	public Player_Controller player_owner {

		get {

			return m_player_owner;
		}
		internal set {

			m_player_owner = value;
		}
	}

	public Item_Owner ownership {

		get {

			return (m_NPC_owner == null ? (m_player_owner == null ? Item_Owner.NONE : Item_Owner.PLAYER) : Item_Owner.NPC);
		}
	}

	internal void set_owner(Player_Controller player) {

		m_NPC_owner = null;
		m_player_owner = player;
	}

	internal void set_owner(NPC npc) {

		m_player_owner = null;
		m_NPC_owner = npc;
	}

	public void set_target(GameObject target) {

		if (m_NPC_owner == null) {

			Debug.LogError("Cannot set item target because this item is not owned by an NPC.");
			return;
		}

		m_NPC_target = (Entity)target;
	}

	public void set_target(Entity target) {

		if (m_NPC_owner == null) {

			Debug.LogError("Cannot set item target because this item is not owned by an NPC.");
			return;
		}

		m_NPC_target = target;
	}

	internal void drop_owner() {

		player_owner = null;
		NPC_owner = null;
	}

	#endregion

	void Start() {

		filter = GetComponent<MeshFilter>();
		template.spawned();
	}

	void Update() {
		template.exists_update();

		switch (ownership) {
			case Item_Owner.NPC:

				template.passive_update(m_NPC_owner);

				break;
			case Item_Owner.PLAYER:

				template.passive_update(m_player_owner);

				break;
		}
	}

	void FixedUpdate() {

		template.fixed_update();
	}

	void LateUpdate() {

		template.late_update();
	}

	void use() {
		switch (ownership) {
			case Item_Owner.NONE:

				Debug.LogError("Cannot use weapon because no owner was assigned before the event was completed.", this);

				break;
			case Item_Owner.NPC:

				if (template.AI_can_use_on_target(m_NPC_owner, m_NPC_target))
					template.primary_used(m_NPC_owner);

				break;
			case Item_Owner.PLAYER:

				template.primary_used(m_player_owner);

				break;
		}
	}

	public void pick_up() {

		switch (ownership) {
			case Item_Owner.NONE:

				Debug.LogError("Cannot pick up weapon because no owner was assigned before the event was completed.", this);

				break;
			case Item_Owner.NPC:

				template.picked_up(m_NPC_owner);

				break;
			case Item_Owner.PLAYER:

				template.picked_up(m_player_owner);

				break;
		}
	}

	public void drop() {
		switch (ownership) {
			case Item_Owner.NONE:

				Debug.LogError("Cannot drop weapon as no NPC or Player owns this item.", this);

				break;
			case Item_Owner.NPC:

				template.dropped(m_NPC_owner);

				break;
			case Item_Owner.PLAYER:

				template.dropped(m_player_owner);

				break;
		}
	}

	public void equip(int slot) {
		switch (ownership) {
			case Item_Owner.NONE:

				Debug.LogError("Cannot equip weapon as no NPC or Player owns this item.", this);

				break;
			case Item_Owner.NPC:
				
				template.equipped(m_NPC_owner, slot);

				break;
			case Item_Owner.PLAYER:

				template.equipped(m_player_owner, slot);

				break;
		}
	}

	public void assign_template(Weapon_Template template, MeshFilter model) {

		this.template = template;
		if (model != null) {
			filter.mesh = model.sharedMesh;
		}
	}
}