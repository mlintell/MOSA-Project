﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Utility.DebugEngine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Mosa.Tool.Debugger
{
	public partial class MainForm : Form
	{
		public DebugServerEngine DebugEngine = new DebugServerEngine();

		private ConnectionProperties connectionProperties = new ConnectionProperties();
		private DispatchOutput dispatchOutput = new DispatchOutput();
		private MethodCaller methodCaller = new MethodCaller();
		private DisplayView displayView = new DisplayView();

		public string Status { set { toolStripStatusLabel1.Text = value; } }

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			dockPanel.SuspendLayout(true);
			connectionProperties.Show(dockPanel, DockState.DockLeft);
			dispatchOutput.Show(connectionProperties.Pane, DockAlignment.Bottom, 0.40);

			methodCaller.Show(dockPanel, DockState.DockRight);

			dockPanel.ResumeLayout(true, true);
			DebugEngine.SetGlobalDispatch(Dispatch);
		}

		public void SignalConnect()
		{
			foreach (var content in dockPanel.Contents)
			{
				var debugContent = content as DebuggerDockContent;

				if (debugContent != null)
				{
					debugContent.Connect();
				}
			}
		}

		public void SignalDisconnect()
		{
			foreach (var content in dockPanel.Contents)
			{
				var debugContent = content as DebuggerDockContent;

				if (debugContent != null)
				{
					debugContent.Disconnect();
				}
			}
		}

		public void Dispatch(DebugMessage response)
		{
			if (response == null)
				return;

			dispatchOutput.BeginInvoke((CallBack)dispatchOutput.ProcessResponses, new object[] { response });
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			connectionProperties.Show(dockPanel, DockState.DockLeft);
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			toolStripStatusLabel1.Text = "Sent Ping!";
			DebugEngine.SendCommand(new DebugMessage(DebugCode.Ping, new List<byte>()));
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			var memoryView = new MemoryView();
			memoryView.Show(dockPanel, DockState.Document);
		}

		private void toolStripButton2_Click_1(object sender, EventArgs e)
		{
			displayView.Show(dockPanel, DockState.Document);
		}
	}
}
