using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void CommandHandler(string[] args);

public class TerminalController
{
	
	#region Event declarations

	public delegate void LogChangedHandler(string[] log);
	public event LogChangedHandler logChanged;

	#endregion Event declarations

	class CommandRegistration
	{
		public string command
		{
			get;
			private set;
		}
		public CommandHandler handler
		{
			get;
			private set;
		}
		public string help
		{
			get;
			private set;
		}
		
		public CommandRegistration(string command, CommandHandler handler, string help)
		{
			this.command = command;
			this.handler = handler;
			this.help = help;
		}
	}
	
	const int scrollbackSize = 20;

	Queue<string> scrollback = new Queue<string>(scrollbackSize);
	List<string> commandHistory = new List<string>();
	Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

	public string[] log
	{
		get;
		private set;
	}
	
	const string repeatCmdName = "!!";
	
	public TerminalController()
	{
		//Il faut ajouter les commandes ici et les methodes correspondantes dans la region "Command handlers"
		//registerCommand("babble", babble, "Exemple de commande qui montre comment analyser des arguments. babble [mot] [nombre de répétitions]");
		//registerCommand("echo", echo, "Renvoie les arguments sous forme de tableau (pour tester l'analyseur d'arguments)");
		registerCommand("help", help, "Affiche cette aide.");
		//registerCommand(repeatCmdName, repeatCommand, "Répétez la dernière commande.");
		//registerCommand("reload", reload, "Recharger la scene.");
		//registerCommand("resetprefs", resetPrefs, "Réinitialise et enregistre les PlayerPrefs.");
	}
	
	void registerCommand(string command, CommandHandler handler, string help)
	{
		commands.Add(command, new CommandRegistration(command, handler, help));
	}
	
	public void appendLogLine(string line)
	{
		Debug.Log(line);
		
		if (scrollback.Count >= TerminalController.scrollbackSize)
		{
			scrollback.Dequeue();
		}
		scrollback.Enqueue(line);
		
		log = scrollback.ToArray();

		if (logChanged != null)
		{
			logChanged(log);
		}
	}
	
	public void runCommandString(string commandString)
	{
		appendLogLine("$ " + commandString);
		
		string[] commandSplit = parseArguments(commandString);
		string[] args = new string[0];
		if (commandSplit.Length < 1)
		{
			appendLogLine(string.Format("Impossible de traiter la commande '{0}'. Utilisez help pour la liste des commandes", commandString));
			return;
			
		}
		else if (commandSplit.Length >= 2)
		{
			int numArgs = commandSplit.Length - 1;
			args = new string[numArgs];
			Array.Copy(commandSplit, 1, args, 0, numArgs);
		}
		runCommand(commandSplit[0].ToLower(), args);
		commandHistory.Add(commandString);
	}
	
	public void runCommand(string command, string[] args)
	{
		CommandRegistration reg = null;
		if (!commands.TryGetValue(command, out reg))
		{
			appendLogLine(string.Format("Commande inconnue '{0}', tapez 'help' pour la liste.", command));
		}
		else
		{
			if (reg.handler == null)
			{
				appendLogLine(string.Format("Impossible de traiter la commande '{0}', le gestionnaire était nul.", command));
			}
			else
			{
				reg.handler(args);
			}
		}
	}
	
	static string[] parseArguments(string commandString)
	{
		LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
		bool inQuote = false;
		var node = parmChars.First;
		while (node != null)
		{
			var next = node.Next;
			if (node.Value == '"')
			{
				inQuote = !inQuote;
				parmChars.Remove(node);
			}
			if (!inQuote && node.Value == ' ')
			{
				node.Value = '\n';
			}
			node = next;
		}
		char[] parmCharsArr = new char[parmChars.Count];
		parmChars.CopyTo(parmCharsArr, 0);

		return (new string(parmCharsArr)).Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
	}

	#region Command handlers
	void babble(string[] args)
	{
		if (args.Length < 2)
		{
			appendLogLine("2 arguments attendus.");
			return;
		}
		string text = args[0];
		if (string.IsNullOrEmpty(text))
		{
			appendLogLine("arg1 attendu comme texte.");
		}
		else
		{
			int repeat = 0;
			if (!Int32.TryParse(args[1], out repeat))
			{
				appendLogLine("arg2 attendu comme entier.");
			}
			else
			{
				for(int i = 0; i < repeat; ++i)
				{
					appendLogLine(string.Format("{0} {1}", text, i));
				}
			}
		}
	}

	void echo(string[] args)
	{
		StringBuilder sb = new StringBuilder();
		foreach (string arg in args)
		{
			sb.AppendFormat("{0},", arg);
		}
		sb.Remove(sb.Length - 1, 1);
		appendLogLine(sb.ToString());
	}

    void help(string[] args)
	{
		foreach(CommandRegistration reg in commands.Values)
		{
			appendLogLine(string.Format("{0}: {1}", reg.command, reg.help));
		}
	}
	
	void repeatCommand(string[] args)
	{
		for (int cmdIdx = commandHistory.Count - 1; cmdIdx >= 0; --cmdIdx)
		{
			string cmd = commandHistory[cmdIdx];
			if (String.Equals(repeatCmdName, cmd))
			{
				continue;
			}
			runCommandString(cmd);
			break;
		}
	}
	
	void reload(string[] args)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	void resetPrefs(string[] args)
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public void SendALineToPlayer (string line)
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("{0},", line);
		sb.Remove(sb.Length - 1, 1);
		appendLogLine(sb.ToString());
	}

	/// <summary>
	/// Les lignes sont séparées par un "¤" (AltGr + $)
	/// </summary>
	/// <param name="block"></param>
	public void SendABlockToPlayer (string block)
	{
		string[] lines = null;
		lines = block.Split("¤"[0]);

		for (int i = 0; i < lines.Length; i++)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0},", lines[i].Split("-"[0]));
			sb.Remove(sb.Length - 1, 1);
			appendLogLine(sb.ToString());
		}
	}

	#endregion Command handlers
}
