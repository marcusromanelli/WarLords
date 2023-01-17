using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BuySkillButton : MonoBehaviour {

	public int skillNumber;
	CardObject card;
	public Color BlueSelected, BlueUnselected, Green, Red;
	List<Renderer> renderers;

	bool status = false;

	void Start () {
		card = transform.GetComponentInParent<CardObject>();
		renderers = GetComponentsInChildren<Renderer>().ToList();
	}

	void Update(){
		if(card.Character==null){
			if(status){
				renderers.ForEach(delegate(Renderer obj) {
					obj.material.color = BlueSelected;
				});
			}else{
				renderers.ForEach(delegate(Renderer obj) {
					obj.material.color = BlueUnselected;
				});
			}
		}else{
			if(status){
				renderers.ForEach(delegate(Renderer obj) {
					obj.material.color = Green;
				});
			}else{
				renderers.ForEach(delegate(Renderer obj) {
					obj.material.color = Red;
				});
			}
		}


	}

	void OnMouseDown(){
		Debug.LogWarning("CLick");
		if(card.Character==null){
			GameConfiguration.PlaySFX(GameConfiguration.confirmAction);
			if(!status){
				Debug.Log("Add skill");
				card.AddSKill(skillNumber);
			}else{
				Debug.Log("Remove skill");
				card.RemoveSkill(skillNumber);
			}

			status = !status;
		}
	}
}
