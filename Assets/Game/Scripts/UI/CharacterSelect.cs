using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] RectTransform[] charBtn;
    [SerializeField] RectTransform indicator;
    [SerializeField] float moveDelay;

    int indicatorPos;
    float moveTimer;

    // Update is called once per frame
    void Update()
    {
        if (moveTimer < moveDelay)
        {
            moveTimer += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            // indicator is visible when using keyboard
            if (indicator != null)
            {
                indicator.gameObject.SetActive(true);
            }

            if (moveTimer < moveDelay)
            {
                if (indicatorPos < charBtn.Length - 1)
                {
                    indicatorPos++;
                    indicator.position = charBtn[indicatorPos].position;
                }
                else
                {
                    indicatorPos = 0;
                    indicator.position = charBtn[indicatorPos].position;
                }
                moveTimer = 0;
            }
            
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            // indicator is visible when using keyboard
            if (indicator != null)
            {
                indicator.gameObject.SetActive(true);
            }

            if (moveTimer < moveDelay)
            {
                if (indicatorPos > 0)
                {
                    indicatorPos--;
                    indicator.position = charBtn[indicatorPos].position;
                }

                else
                {
                    indicatorPos = charBtn.Length - 1;
                    indicator.position = charBtn[indicatorPos].position;
                }
                moveTimer = 0;
            }
        }

        indicator.localPosition = charBtn[indicatorPos].localPosition;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Selected Character " + indicatorPos);
        }
    }

    public void HoverOnButton(int btnPos)
    {
        indicatorPos = btnPos;
        if (indicator != null)
        {
            indicator.gameObject.SetActive(true);
            indicator.localPosition = charBtn[indicatorPos].localPosition;
        }
    }

    // hide the arrow when the mouse leaves
    public void HoverOffButton()
    {
        if (indicator != null)
        {
            indicator.gameObject.SetActive(false);
        }
    }
}
