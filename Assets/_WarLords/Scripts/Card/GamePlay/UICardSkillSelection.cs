using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardSkillSelection : MonoBehaviour
{
	[BoxGroup("Components"), SerializeField] Canvas canvas;
	[BoxGroup("Components"), SerializeField] EnableSkillButton skillButtonPrefab;
	[BoxGroup("Components"), SerializeField] Button applyButton;
	[BoxGroup("Components"), SerializeField] Slider timerSlider;
	[BoxGroup("Components"), SerializeField] float awaitTime = 2;


	private Dictionary<SkillData, bool> skills;
	private Action<Dictionary<SkillData, bool>> onApply;
	private Dictionary<SkillData, EnableSkillButton> skillObjects;
	private uint totalManaCost;
	private Player player;
	private float startTime;
	private float targetTime;
	void Awake()
    {
		canvas.enabled = false;
    }
	void Update(){
		UpdateSlider();
	}
	void UpdateSlider()
    {
		var isActive = canvas.enabled == true;

		if (!isActive)
			return;

		var deltaTime = Mathf.InverseLerp(targetTime, startTime, Time.time);

		if(deltaTime > 0)
        {
			timerSlider.value = deltaTime;
			return;
        }

		OnClickApply();
	}
	public void Show(Player player, SkillData[] skills, Action<Dictionary<SkillData, bool>> onApply)
    {
		EraseAll();

		this.startTime = Time.time;
		this.targetTime = startTime + awaitTime;
		this.player = player;
		this.onApply = onApply;
		this.skills = new Dictionary<SkillData, bool>();
		skillObjects = new Dictionary<SkillData, EnableSkillButton>();

		int i = 0;
		var canActiveAnySkill = false;
		foreach (var skill in skills)
		{
			this.skills.Add(skill, false);

			var skillObj = ElementFactory.CreateObject<EnableSkillButton>(skillButtonPrefab, transform);
			skillObj.Setup(skill, OnClickSkill);
			skillObj.transform.SetAsFirstSibling();

			if (player.HasAvailableMana(skill.GetManaCost()))
				canActiveAnySkill = true;

			skillObjects.Add(skill, skillObj);
		}

        if (!canActiveAnySkill)
        {
			onApply?.Invoke(this.skills);
			return;
        }

        RefreshButtonStatus();

		canvas.enabled = true;
	}
	void EraseAll()
    {
		if (skillObjects == null || skillObjects.Count <= 0)
			return;

        foreach (var skillObj in skillObjects)
			Destroy(skillObj.Value.gameObject);

		skillObjects = null;
    }
	void OnClickSkill(SkillData skill, bool value)
    {
		int delta = (int)skill.GetManaCost() * (value ? 1 : -1);

		totalManaCost = (uint)Mathf.Clamp(totalManaCost + delta, 0, int.MaxValue);

		skills[skill] = value;

		player.PreviewMana(totalManaCost);
		RefreshButtonStatus();
    }

	void RefreshButtonStatus()
    {
		foreach (var skillObj in skillObjects)
			if(this.skills[skillObj.Key] == false)
				skillObj.Value.SetActive(player.HasAvailableMana(totalManaCost + skillObj.Key.GetManaCost()));

	}

	public void OnClickApply()
    {
		canvas.enabled = false;
		onApply?.Invoke(skills);
	}
}