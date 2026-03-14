using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    public ThemedButton[] buttons;
    private ThemedButton _selected;

    void Start()
    {
        foreach (var btn in buttons)
        {
            // Capture local ref for the lambda
            var localBtn = btn;
            btn.GetComponent<Button>().onClick.AddListener(() => Select(localBtn));
        }

        // Select first button by default (optional)
        if (buttons.Length > 0) Select(buttons[0]);
    }

    public void Select(ThemedButton target)
    {
        foreach (var btn in buttons)
            btn.SetSelected(btn == target);

        _selected = target;
    }

    public ThemedButton GetSelected() => _selected;
}
