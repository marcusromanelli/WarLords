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

	[SerializeField] Transform statusContainer;
	[SerializeField] Transform skillContainer;
	[SerializeField] Transform activeSkillPrefab;


	Transform[] activeSkillObjects;
	public void UpdateData(RuntimeCardData runtimeCardData)
    {
		var willShowStatus = runtimeCardData.Summoned;

		statusContainer.gameObject.SetActive(willShowStatus);

		if (!willShowStatus)
			return;

		lifeText.text = runtimeCardData.CalculateDefense().ToString();
		attackText.text = runtimeCardData.CalculateAttack().ToString();
		speedText.text = runtimeCardData.CalculateWalkSpeed().ToString();

		UpdateSkills(runtimeCardData.ActiveSkills);
    }

	void UpdateSkills(List<SkillData> skilldata)
    {
		if(activeSkillObjects != null)
			foreach (var obj in activeSkillObjects)
				if(obj != null)
					Destroy(obj.gameObject);

		skillContainer.gameObject.SetActive(skilldata.Count > 0);

		if (skilldata.Count <= 0)
			return;

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