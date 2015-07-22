using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuriNET.CoreServer;
using YuriNET.Utils;

namespace YuriNET {
    public partial class Form1 : Form {
        private Server server;

        public Form1() {
            InitializeComponent();
            rbDbgNone.Tag = Logger.DisplayLevels.All ^ Logger.DisplayLevels.Debug;
            rbDbgDebug.Tag = Logger.DisplayLevels.All;
            rbDbgWarn.Tag = Logger.DisplayLevels.Warn | Logger.DisplayLevels.Info;
            rbDbgError.Tag = Logger.DisplayLevels.Error | Logger.DisplayLevels.Info;

            EventHandler eventSetDbgLvl = new EventHandler(
                (sender, e) => Logger.DisplayLevel = getSelectedLogLevel()
            );
            rbDbgNone.Click += eventSetDbgLvl;
            rbDbgDebug.Click += eventSetDbgLvl;
            rbDbgWarn.Click += eventSetDbgLvl;
            rbDbgError.Click += eventSetDbgLvl;

        }

        private Logger.DisplayLevels getSelectedLogLevel() {
            Logger.DisplayLevels logDispLvl = Logger.DisplayLevels.All;
            if (rbDbgNone.Checked) {
                logDispLvl = (Logger.DisplayLevels) rbDbgNone.Tag;
            } else if (rbDbgDebug.Checked) {
                logDispLvl = (Logger.DisplayLevels) rbDbgDebug.Tag;
            } else if (rbDbgWarn.Checked) {
                logDispLvl = (Logger.DisplayLevels) rbDbgWarn.Tag;
            } else if (rbDbgError.Checked) {
                logDispLvl = (Logger.DisplayLevels) rbDbgError.Tag;
            }

            return logDispLvl;
        }

        private void button1_Click(object sender, EventArgs e) {
            Logger.DisplayLevel = getSelectedLogLevel();
            server = Server.Holder.getServer();
            server.SocketPort = int.Parse(txtPort.Text);
            server.MaxClients = 100;
            server.startServer();
            button1.Enabled = false;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e) {
            server.stopServer();
            button1.Enabled = true;
            button3.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e) {
            if (server != null) {
                txtOnlineCount.Text = server.getClientsCount().ToString();
            }
        }

        private void btn_loadState_Click(object sender, EventArgs e) {
            //storage.Load();
            //server = (Server) storage.get_Item("server");
            server.startServer();
            button1.Enabled = false;
            button3.Enabled = true;
        }

        private void btn_saveState_Click(object sender, EventArgs e) {
            //storage.set_Item("server", server);
            //storage.Save();
            server.stopServer();
            button1.Enabled = true;
            button3.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            button3.PerformClick();
        }



    }
}
