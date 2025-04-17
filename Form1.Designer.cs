namespace WorkCloneCS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            nameBtn = new Button();
            PricingBtn = new Button();
            tableBtn = new Button();
            BackBtn = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            catPan = new FlowLayoutPanel();
            ConfigPannel = new FlowLayoutPanel();
            SignOnBtnConfigPanel = new Button();
            SignOffBtnControlPanel = new Button();
            ConfigBtnControlPanel = new Button();
            syncTillBtnConfigPanel = new Button();
            ReciptToggleBtnConfigPanel = new Button();
            InfoBtnControlPanel = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            miscBtn = new Button();
            OrderBtn = new Button();
            tableBottomBtn = new Button();
            FinalBtn = new Button();
            ConfigBtn = new Button();
            ConfigPannel.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // nameBtn
            // 
            nameBtn.ImageAlign = ContentAlignment.MiddleRight;
            nameBtn.Location = new Point(255, 12);
            nameBtn.Name = "nameBtn";
            nameBtn.Size = new Size(325, 23);
            nameBtn.TabIndex = 0;
            nameBtn.Text = "Name";
            nameBtn.UseVisualStyleBackColor = true;
            nameBtn.Click += nameBtn_Click;
            // 
            // PricingBtn
            // 
            PricingBtn.Location = new Point(12, 12);
            PricingBtn.Name = "PricingBtn";
            PricingBtn.Size = new Size(237, 23);
            PricingBtn.TabIndex = 1;
            PricingBtn.Text = "Standard";
            PricingBtn.UseVisualStyleBackColor = true;
            PricingBtn.Click += PricingBtn_Click;
            // 
            // tableBtn
            // 
            tableBtn.Location = new Point(586, 12);
            tableBtn.Name = "tableBtn";
            tableBtn.Size = new Size(280, 23);
            tableBtn.TabIndex = 2;
            tableBtn.Text = "Table";
            tableBtn.UseVisualStyleBackColor = true;
            tableBtn.Click += tableBtn_Click;
            // 
            // BackBtn
            // 
            BackBtn.Location = new Point(774, 563);
            BackBtn.Name = "BackBtn";
            BackBtn.Size = new Size(92, 47);
            BackBtn.TabIndex = 0;
            BackBtn.Text = "Back";
            BackBtn.UseVisualStyleBackColor = true;
            BackBtn.Click += backBtn_Click;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Location = new Point(13, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(853, 190);
            panel1.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.Location = new Point(12, 239);
            panel2.Name = "panel2";
            panel2.Size = new Size(854, 33);
            panel2.TabIndex = 6;
            // 
            // catPan
            // 
            catPan.Location = new Point(13, 278);
            catPan.Name = "catPan";
            catPan.Size = new Size(853, 279);
            catPan.TabIndex = 7;
            // 
            // ConfigPannel
            // 
            ConfigPannel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ConfigPannel.BackColor = SystemColors.ControlDark;
            ConfigPannel.Controls.Add(SignOnBtnConfigPanel);
            ConfigPannel.Controls.Add(SignOffBtnControlPanel);
            ConfigPannel.Controls.Add(ConfigBtnControlPanel);
            ConfigPannel.Controls.Add(syncTillBtnConfigPanel);
            ConfigPannel.Controls.Add(ReciptToggleBtnConfigPanel);
            ConfigPannel.Controls.Add(InfoBtnControlPanel);
            ConfigPannel.Location = new Point(774, 132);
            ConfigPannel.Name = "ConfigPannel";
            ConfigPannel.Size = new Size(92, 425);
            ConfigPannel.TabIndex = 8;
            ConfigPannel.Visible = false;
            // 
            // SignOnBtnConfigPanel
            // 
            SignOnBtnConfigPanel.Dock = DockStyle.Bottom;
            SignOnBtnConfigPanel.Location = new Point(3, 3);
            SignOnBtnConfigPanel.Name = "SignOnBtnConfigPanel";
            SignOnBtnConfigPanel.Size = new Size(86, 64);
            SignOnBtnConfigPanel.TabIndex = 0;
            SignOnBtnConfigPanel.Text = "SignOn";
            SignOnBtnConfigPanel.UseVisualStyleBackColor = true;
            SignOnBtnConfigPanel.Click += SignOnBtnConfigPanel_Click;
            // 
            // SignOffBtnControlPanel
            // 
            SignOffBtnControlPanel.Dock = DockStyle.Bottom;
            SignOffBtnControlPanel.Location = new Point(3, 73);
            SignOffBtnControlPanel.Name = "SignOffBtnControlPanel";
            SignOffBtnControlPanel.Size = new Size(86, 64);
            SignOffBtnControlPanel.TabIndex = 1;
            SignOffBtnControlPanel.Text = "SignOff";
            SignOffBtnControlPanel.UseVisualStyleBackColor = true;
            // 
            // ConfigBtnControlPanel
            // 
            ConfigBtnControlPanel.Dock = DockStyle.Bottom;
            ConfigBtnControlPanel.Location = new Point(3, 143);
            ConfigBtnControlPanel.Name = "ConfigBtnControlPanel";
            ConfigBtnControlPanel.Size = new Size(86, 64);
            ConfigBtnControlPanel.TabIndex = 2;
            ConfigBtnControlPanel.Text = "Config";
            ConfigBtnControlPanel.UseVisualStyleBackColor = true;
            // 
            // syncTillBtnConfigPanel
            // 
            syncTillBtnConfigPanel.Dock = DockStyle.Bottom;
            syncTillBtnConfigPanel.Location = new Point(3, 213);
            syncTillBtnConfigPanel.Name = "syncTillBtnConfigPanel";
            syncTillBtnConfigPanel.Size = new Size(86, 64);
            syncTillBtnConfigPanel.TabIndex = 3;
            syncTillBtnConfigPanel.Text = "SyncTill";
            syncTillBtnConfigPanel.UseVisualStyleBackColor = true;
            // 
            // ReciptToggleBtnConfigPanel
            // 
            ReciptToggleBtnConfigPanel.Dock = DockStyle.Bottom;
            ReciptToggleBtnConfigPanel.Location = new Point(3, 283);
            ReciptToggleBtnConfigPanel.Name = "ReciptToggleBtnConfigPanel";
            ReciptToggleBtnConfigPanel.Size = new Size(86, 64);
            ReciptToggleBtnConfigPanel.TabIndex = 4;
            ReciptToggleBtnConfigPanel.Text = "ReciptOff";
            ReciptToggleBtnConfigPanel.UseVisualStyleBackColor = true;
            // 
            // InfoBtnControlPanel
            // 
            InfoBtnControlPanel.Dock = DockStyle.Bottom;
            InfoBtnControlPanel.Location = new Point(3, 353);
            InfoBtnControlPanel.Name = "InfoBtnControlPanel";
            InfoBtnControlPanel.Size = new Size(86, 64);
            InfoBtnControlPanel.TabIndex = 5;
            InfoBtnControlPanel.Text = "Info";
            InfoBtnControlPanel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.BackColor = SystemColors.ControlDark;
            flowLayoutPanel2.Controls.Add(miscBtn);
            flowLayoutPanel2.Controls.Add(OrderBtn);
            flowLayoutPanel2.Controls.Add(tableBottomBtn);
            flowLayoutPanel2.Controls.Add(FinalBtn);
            flowLayoutPanel2.Controls.Add(ConfigBtn);
            flowLayoutPanel2.Location = new Point(13, 563);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(728, 47);
            flowLayoutPanel2.TabIndex = 9;
            // 
            // miscBtn
            // 
            miscBtn.Location = new Point(3, 3);
            miscBtn.Name = "miscBtn";
            miscBtn.Size = new Size(137, 44);
            miscBtn.TabIndex = 4;
            miscBtn.Text = "Misc";
            miscBtn.UseVisualStyleBackColor = true;
            // 
            // OrderBtn
            // 
            OrderBtn.Location = new Point(146, 3);
            OrderBtn.Name = "OrderBtn";
            OrderBtn.Size = new Size(137, 44);
            OrderBtn.TabIndex = 5;
            OrderBtn.Text = "Order";
            OrderBtn.UseVisualStyleBackColor = true;
            // 
            // tableBottomBtn
            // 
            tableBottomBtn.Location = new Point(289, 3);
            tableBottomBtn.Name = "tableBottomBtn";
            tableBottomBtn.Size = new Size(137, 44);
            tableBottomBtn.TabIndex = 6;
            tableBottomBtn.Text = "Table";
            tableBottomBtn.UseVisualStyleBackColor = true;
            // 
            // FinalBtn
            // 
            FinalBtn.Location = new Point(432, 3);
            FinalBtn.Name = "FinalBtn";
            FinalBtn.Size = new Size(137, 44);
            FinalBtn.TabIndex = 7;
            FinalBtn.Text = "Final";
            FinalBtn.UseVisualStyleBackColor = true;
            // 
            // ConfigBtn
            // 
            ConfigBtn.Location = new Point(575, 3);
            ConfigBtn.Name = "ConfigBtn";
            ConfigBtn.Size = new Size(137, 44);
            ConfigBtn.TabIndex = 8;
            ConfigBtn.Text = "Config";
            ConfigBtn.UseVisualStyleBackColor = true;
            ConfigBtn.Click += ConfigBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(878, 622);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(ConfigPannel);
            Controls.Add(catPan);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(BackBtn);
            Controls.Add(tableBtn);
            Controls.Add(PricingBtn);
            Controls.Add(nameBtn);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ConfigPannel.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button nameBtn;
        private Button PricingBtn;
        private Button tableBtn;
        private Button BackBtn;
        private Panel panel1;
        private Panel panel2;
        private Label leftLabel;
        private Label rightLabel;
        private decimal priceTotal;
        private FlowLayoutPanel scrollPanel;
        private FlowLayoutPanel catPan;
        private FlowLayoutPanel ConfigPannel;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button miscBtn;
        private Button OrderBtn;
        private Button tableBottomBtn;
        private Button FinalBtn;
        private Button ConfigBtn;
        private Button SignOnBtnConfigPanel;
        private Button SignOffBtnControlPanel;
        private Button ConfigBtnControlPanel;
        private Button syncTillBtnConfigPanel;
        private Button ReciptToggleBtnConfigPanel;
        private Button InfoBtnControlPanel;
        private FlowLayoutPanel basePanel;
    }
}
