using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalView : MonoBehaviour
{
	public TerminalController terminal = new TerminalController();

	public GameObject viewContainer;
	public TextMeshProUGUI logTextArea;
	public TMP_InputField inputField;

	void Start()
	{
		if (terminal != null)
		{
			terminal.logChanged += OnLogChanged;
		}

		updateLogStr(terminal.log);
        inputField.Select();
        inputField.ActivateInputField();
    }
	
	~TerminalView()
	{
		terminal.logChanged -= OnLogChanged;
	}

	void OnLogChanged(string[] newLog)
	{
		updateLogStr(newLog);
	}
	
	void updateLogStr(string[] newLog)
	{
		if (newLog == null)
		{
			logTextArea.text = "";
		}
		else
		{
			logTextArea.text = string.Join("\n", newLog);
		}
	}

	public void runCommand()
	{
		terminal.runCommandString(inputField.text);
		inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

}
