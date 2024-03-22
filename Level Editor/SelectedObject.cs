// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using UnityEngine;

public class SelectedObject : MonoBehaviour
{
	bool pressed;
	public EditorControls ec;

	float add, add2;
	public Object obj;



	public enum Object
	{
		positionVertical = 0,
		positionHorizontal = 1,
		positionXY = 2,
		scaleVectical = 3,
		scaleHorizontal = 4,
		scaleXY = 5,
		rotation = 6
	}

	public void OnMouseDown()
	{
		ec.cantContinue = true;
	}
	public void OnMouseDrag()
	{//
		if (MainEditorComponent.Instance.editorControls.nuhUh) return;
		
		float x = Input.GetAxisRaw("Mouse X") / 1.5f;
		float y = Input.GetAxisRaw("Mouse Y") / 1.5f;
		
		GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.5f);
		if (Input.GetKey(KeyCode.LeftControl))
		{
			add += Input.GetAxisRaw("Mouse X") / 2;
			add2 += Input.GetAxisRaw("Mouse Y") / 2;
			if (add >= 1 || add <= -1)
			{
				if (add <= -1) x = -1;
				else x = 1;
				add = 0;
			}
			else
			{
				x = 0;
			}
			if (add2 >= 1 || add2 <= -1)
			{
				if (add2 <= -1) y = -1;
				else y = 1;

				add2 = 0;
			}
			else
			{
				y = 0;
			}
		}
        
        for (int i = 0; i < ec.lastSelectedArray.Length; i++)
		{
			switch (obj)
			{
				case Object.positionVertical:
					ec.lastSelectedArray[i].transform.position += new Vector3(0, y, 0);
					break;
				case Object.positionHorizontal:
					ec.lastSelectedArray[i].transform.position += new Vector3(x, 0, 0);
					break;
				case Object.positionXY:
					ec.lastSelectedArray[i].transform.position += new Vector3(x, y, 0);
					break;
				case Object.scaleVectical:
					if (ec.lastSelectedArray[i].transform.localScale.y <= 10)
						ec.lastSelectedArray[i].transform.localScale += new Vector3(0, y, 0);
					break;
				case Object.scaleHorizontal:
					if (ec.lastSelectedArray[i].transform.localScale.x <= 10)
						ec.lastSelectedArray[i].transform.localScale = ec.lastSelectedArray[i].transform.localScale += new Vector3(x, 0, 0);
					break;
				case Object.scaleXY:
					if (ec.lastSelectedArray[i].transform.localScale.y <= 10 && ec.lastSelectedArray[i].transform.localScale.x <= 10)
						ec.lastSelectedArray[i].transform.localScale += new Vector3(x + y, x + y, 0);
					break;
				case Object.rotation:
					ec.lastSelectedArray[i].transform.rotation *= Quaternion.Inverse(Quaternion.Euler(0, 0, -y * 2 + x * 2));

					break;
			}
			ec.lastSelectedArray[i].transform.localScale = new Vector3(
				Mathf.Clamp(ec.lastSelectedArray[i].transform.localScale.x, -10, 10)
				, Mathf.Clamp(ec.lastSelectedArray[i].transform.localScale.y, -10, 10), ec.lastSelectedArray[i].transform.localScale.z);



			ec.objProperties.InitializeFields(ec.lastSelectedArray);

		}
	}

	public void OnMouseUp()
	{
		if (add > 1) add = 0;
		ec.cantContinue = false;
		MainEditorComponent.Instance.beat = false;
		GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);
	}
}
