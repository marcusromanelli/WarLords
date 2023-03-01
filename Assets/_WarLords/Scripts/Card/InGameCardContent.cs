using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameCardContent : MonoBehaviour
{
	[SerializeField] TMP_Text lifeText;
	[SerializeField] TMP_Text attackText;
	[SerializeField] TMP_Text speedText;

	[SerializeField] Transform skillContainer;
	[SerializeField] Transform activeSkillPrefab;


	Transform[] activeSkillObjects;
	public void UpdateData(RuntimeCardData runtimeCardData)
    {
		lifeText.text = runtimeCardData.CalculateDefense().ToString();
		attackText.text = runtimeCardData.CalculateAttack().ToString();
		speedText.text = runtimeCardData.CalculateWalkSpeed().ToString();

		UpdateSkills(runtimeCardData.ActiveSkills);
    }

	void UpdateSkills(List<SkillData> skilldata)
    {
		if(activeSkillObjects != null)
			foreach (var obj in activeSkillObjects)
				Destroy(obj.gameObject);

		activeSkillObjects = new Transform[skilldata.Count];

		int i = 0;
		foreach(var skill in skilldata)
        {
			var skillObj = ElementFactory.CreateObject<Transform>(activeSkillPrefab, skillContainer);

			skillObj.GetComponentInChildren<TMP_Text>().text = skill.ToString();
			activeSkillObjects[i] = skillObj;

			i++;
        }
    }
}