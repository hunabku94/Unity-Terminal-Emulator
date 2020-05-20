using UnityEngine;

public class TerminalWindow : MonoBehaviour
{
    [SerializeField] GameObject terminalView = null;
    TerminalController terminal = null;

    void Awake()
    {
        if (terminalView)
        {
            terminal = terminalView.GetComponent<TerminalView>().terminal;
        }
        else
        {
            Debug.Log("terminalView n'est pas assigné.", gameObject);
        }
    }

    public void CloseTerminalWindow()
    {
        if (!terminalView)
        {
            return;
        }
        
        terminalView.SetActive(false);
    }

    public void OpenTerminalWindow()
    {
        if (!terminalView)
        {
            return;
        }
        
        terminalView.SetActive(true);
    }

    public void SendALineToPlayer (string line)
    {
        if (!terminalView || terminal == null)
        {
            return;
        }

        terminal.SendALineToPlayer(line);
    }

    public void SendABlockToPlayer (string block)
    {
        if (!terminalView || terminal == null)
        {
            return;
        }

        terminal.SendABlockToPlayer(block);
    }
}
