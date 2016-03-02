using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuriNET.CoreServer;
using YuriNET.Storage;
using YuriNET.Utils;

namespace YuriNET_Dedicate {
    public partial class Form1 : Form {
        private Server server;
        private static NoreStorage storage = new NoreStorage("server.dat");

        public Form1() {
            InitializeComponent();
            rbDbgNone.Tag = Logger.Level.Info;
            rbDbgDebug.Tag = Logger.Level.Debug | Logger.Level.Info;
            rbDbgWarn.Tag = Logger.Level.Warn | Logger.Level.Debug | Logger.Level.Info;
            rbDbgError.Tag = Logger.Level.Error | Logger.Level.Warn | Logger.Level.Debug | Logger.Level.Info;

            EventHandler eventSetDbgLvl = new EventHandler(
                (sender, e) => Logger.DisplayLevel = getSelectedLogLevel()
            );
            rbDbgNone.Click += eventSetDbgLvl;
            rbDbgDebug.Click += eventSetDbgLvl;
            rbDbgWarn.Click += eventSetDbgLvl;
            rbDbgError.Click += eventSetDbgLvl;

        }

        private Logger.Level getSelectedLogLevel() {
            Logger.Level logDispLvl = Logger.Level.All;
            if (rbDbgNone.Checked) {
                logDispLvl = (Logger.Level) rbDbgNone.Tag;
            } else if (rbDbgDebug.Checked) {
                logDispLvl = (Logger.Level) rbDbgDebug.Tag;
            } else if (rbDbgWarn.Checked) {
                logDispLvl = (Logger.Level) rbDbgWarn.Tag;
            } else if (rbDbgError.Checked) {
                logDispLvl = (Logger.Level) rbDbgError.Tag;
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
            
            server.loadClients();
            //server.startServer();
            //button1.Enabled = false;
            //button3.Enabled = true;
        }

        private void btn_saveState_Click(object sender, EventArgs e) {
            //storage.set_Item("server", server);
            //storage.Save();

            server.dumpClients();
            //server.stopServer();
            //button1.Enabled = true;
            //button3.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            button3.PerformClick();
        }



    }
}
