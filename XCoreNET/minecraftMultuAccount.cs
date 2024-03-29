﻿using Global;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace XCoreNET
{
    public partial class minecraftMultuAccount : Form
    {
        private void setTranslate()
        {
            this.Font = new Font(gb.lang.FONT_FAMILY_CHILD, gb.lang.FONT_SIZE_CHILD);
            this.Text = gb.lang.FORM_TITLE_MULTI_ACCOUNT;
        }
        public minecraftMultuAccount()
        {
            InitializeComponent();
            setTranslate();
        }

        private void DrawingAccountList()
        {
            this.panel.Controls.Clear();

            int x = 5;
            int y = 5;
            int k = 0;
            foreach (var item in gb.account)
            {
                PictureBox avatar = new PictureBox();
                avatar.WaitOnLoad = false;
                avatar.LoadAsync($"https://cravatar.eu/helmavatar/{item.Key}");
                avatar.Location = new System.Drawing.Point(x, y + 40 * k);
                avatar.Width = 32;
                avatar.Height = 32;

                TextBox username = new TextBox();
                username.ReadOnly = true;
                username.Location = new System.Drawing.Point(x + 36, (int)(y * 2.5 + 40 * k));
                username.Text = item.Value.username;
                username.Font = new Font("Verdana", 10);
                username.Width = 130;
                username.BorderStyle = BorderStyle.None;
                toolTip.SetToolTip(username, item.Key);

                Button btnSwitch = new Button();
                btnSwitch.Text = gb.lang.BTN_SWITCH;
                btnSwitch.Width = 60;
                btnSwitch.Height = 24;
                btnSwitch.Location = new System.Drawing.Point(x + 170, y * 2 + 40 * k);

                Button btnDel = new Button();
                btnDel.Text = gb.lang.BTN_DELETE;
                btnDel.Width = 70;
                btnDel.Height = 24;
                btnDel.Location = new System.Drawing.Point(x + 235, y * 2 + 40 * k);

                this.panel.Controls.Add(avatar);
                this.panel.Controls.Add(btnSwitch);
                this.panel.Controls.Add(btnDel);
                this.panel.Controls.Add(username);

                if (item.Key.Equals(gb.minecraftUUID))
                {
                    btnDel.Enabled = false;
                    username.SelectionStart = username.TextLength;
                    username.SelectionLength = 0;

                    btnSwitch.Text = gb.lang.BTN_RELOAD;
                    btnSwitch.Click += (senderx, ex) =>
                    {
                        gb.refreshToken = item.Value.refreshToken;
                        gb.launchToken = item.Value.accessToken;
                        gb.launchTokenExpiresAt = item.Value.expiresAt;
                        gb.minecraftUsername = item.Value.username;
                        gb.minecraftUUID = item.Key;

                        this.DialogResult = DialogResult.Abort;
                    };

                    toolTip.SetToolTip(btnSwitch, gb.lang.TOOLTIP_REFRESH_ACCESS_TOKEN);
                }
                else
                {
                    btnSwitch.Click += (senderx, ex) =>
                    {
                        gb.refreshToken = item.Value.refreshToken;
                        gb.launchToken = item.Value.accessToken;
                        gb.launchTokenExpiresAt = item.Value.expiresAt;
                        gb.minecraftUsername = item.Value.username;
                        gb.minecraftUUID = item.Key;

                        this.DialogResult = DialogResult.OK;
                    };
                    btnDel.Click += (senderx, ex) =>
                    {
                        var reuslt = MessageBox.Show(gb.lang.DIALOG_DELETE_ACCOUNT_CONFIRM, gb.lang.DIALOG_ERROR, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reuslt == DialogResult.Yes)
                        {
                            gb.resetTokens(item.Key);
                            gb.savingSession(true);
                            DrawingAccountList();
                        }
                    };

                    toolTip.SetToolTip(btnSwitch, gb.lang.TOOLTIP_SWITCH_ACCOUNT);
                    toolTip.SetToolTip(btnDel, gb.lang.TOOLTIP_DELETE_ACCOUNT);
                }

                k++;
            }


            if (k == 0) this.Close();
        }

        private void minecraftMultuAccount_Load(object sender, EventArgs e)
        {
            DrawingAccountList();
            this.Focus();
        }
    }
}
