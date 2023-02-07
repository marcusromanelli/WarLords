using UnityEngine;

public class DamageCounter : MonoBehaviour 
{
	[SerializeField] Animation animation;
	[SerializeField] TextMesh text;


	public void Show(uint value)
    {
		text.text = value.ToString();

		animation.Play();
    }
}
