using UnityEngine;
using System.Collections;


namespace Week04
{
	public class FlattenedInput : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetButtonDown("Submit"))
			{
				var xInput = Input.GetAxisRaw("Horizontal");
				var yInput = Input.GetAxisRaw("Vertical");

				var input = new Vector2(xInput, yInput);
				var output = MathS.CircleToSquare(input);

				Log.Steb(output.x);
				Log.Steb(output.y);
			}

		}
	}
}
