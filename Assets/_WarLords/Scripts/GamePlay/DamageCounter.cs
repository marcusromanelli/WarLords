using UnityEngine;

public class DamageCounter : MonoBehaviour 
{
	[SerializeField] new Animation animation;
	[SerializeField] TextMesh text;


	public void Show(uint value)
    {
		text.text = value.ToString();

		animation.Play();
    }
}
