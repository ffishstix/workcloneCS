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
            finalPanel = new FlowLayoutPanel();
            CancelBtn = new Button();
            SendToTableBtnFinalPanel = new Button();
            CardBtn = new Button();
            CashBtn = new Button();
            tablePanel = new FlowLayoutPanel();
            openTableBtn = new Button();
            blankBtn = new Button();
            sendToTableBtnTablePanel = new Button();
            printBillBtn = new Button();
            orderPanel = new FlowLayoutPanel();
            setSeatBtn = new Button();
            addItemBtn = new Button();
            subtractItemBtn = new Button();
            lineCorrectBtn = new Button();
            toggleBtn = new Button();
            multiplyBtn = new Button();
            miscPanel = new FlowLayoutPanel();
            currentSeatBtn = new Button();
            textMsgBtn = new Button();
            presetMsgBtn = new Button();
            categoryShiftBtn = new Button();
            discountAmountBtn = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            ConfigPannel.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            finalPanel.SuspendLayout();
            tablePanel.SuspendLayout();
            orderPanel.SuspendLayout();
            miscPanel.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // nameBtn
            // 
            nameBtn.ImageAlign = ContentAlignment.MiddleRight;
            nameBtn.Location = new Point(291, 16);
            nameBtn.Margin = new Padding(3, 4, 3, 4);
            nameBtn.Name = "nameBtn";
            nameBtn.Size = new Size(371, 31);
            nameBtn.TabIndex = 0;
            nameBtn.Text = "Name";
            nameBtn.UseVisualStyleBackColor = true;
            nameBtn.Click += nameBtn_Click;
            // 
            // PricingBtn
            // 
            PricingBtn.Location = new Point(14, 16);
            PricingBtn.Margin = new Padding(3, 4, 3, 4);
            PricingBtn.Name = "PricingBtn";
            PricingBtn.Size = new Size(271, 31);
            PricingBtn.TabIndex = 1;
            PricingBtn.Text = "Standard";
            PricingBtn.UseVisualStyleBackColor = true;
            PricingBtn.Click += PricingBtn_Click;
            // 
            // tableBtn
            // 
            tableBtn.Location = new Point(670, 16);
            tableBtn.Margin = new Padding(3, 4, 3, 4);
            tableBtn.Name = "tableBtn";
            tableBtn.Size = new Size(320, 31);
            tableBtn.TabIndex = 2;
            tableBtn.Text = "Table";
            tableBtn.UseVisualStyleBackColor = true;
            tableBtn.Click += tableBtn_Click;
            // 
            // BackBtn
            // 
            BackBtn.Location = new Point(889, 752);
            BackBtn.Margin = new Padding(3, 4, 3, 4);
            BackBtn.Name = "BackBtn";
            BackBtn.Size = new Size(105, 63);
            BackBtn.TabIndex = 0;
            BackBtn.Text = "Back";
            BackBtn.UseVisualStyleBackColor = true;
            BackBtn.Click += backBtn_Click;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Location = new Point(3, 4);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(975, 253);
            panel1.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.Location = new Point(3, 265);
            panel2.Margin = new Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new Size(975, 44);
            panel2.TabIndex = 6;
            // 
            // catPan
            // 
            catPan.Location = new Point(3, 317);
            catPan.Margin = new Padding(3, 4, 3, 4);
            catPan.Name = "catPan";
            catPan.Size = new Size(975, 372);
            catPan.TabIndex = 1;
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
            ConfigPannel.Location = new Point(889, 180);
            ConfigPannel.Margin = new Padding(3, 4, 3, 4);
            ConfigPannel.Name = "ConfigPannel";
            ConfigPannel.Size = new Size(105, 559);
            ConfigPannel.TabIndex = 8;
            ConfigPannel.Visible = false;
            // 
            // SignOnBtnConfigPanel
            // 
            SignOnBtnConfigPanel.Dock = DockStyle.Bottom;
            SignOnBtnConfigPanel.Location = new Point(3, 4);
            SignOnBtnConfigPanel.Margin = new Padding(3, 4, 3, 4);
            SignOnBtnConfigPanel.Name = "SignOnBtnConfigPanel";
            SignOnBtnConfigPanel.Size = new Size(98, 85);
            SignOnBtnConfigPanel.TabIndex = 0;
            SignOnBtnConfigPanel.Text = "SignOn";
            SignOnBtnConfigPanel.UseVisualStyleBackColor = true;
            SignOnBtnConfigPanel.Click += SignOnBtnConfigPanel_Click;
            // 
            // SignOffBtnControlPanel
            // 
            SignOffBtnControlPanel.Dock = DockStyle.Bottom;
            SignOffBtnControlPanel.Location = new Point(3, 97);
            SignOffBtnControlPanel.Margin = new Padding(3, 4, 3, 4);
            SignOffBtnControlPanel.Name = "SignOffBtnControlPanel";
            SignOffBtnControlPanel.Size = new Size(98, 85);
            SignOffBtnControlPanel.TabIndex = 1;
            SignOffBtnControlPanel.Text = "SignOff";
            SignOffBtnControlPanel.UseVisualStyleBackColor = true;
            // 
            // ConfigBtnControlPanel
            // 
            ConfigBtnControlPanel.Dock = DockStyle.Bottom;
            ConfigBtnControlPanel.Location = new Point(3, 190);
            ConfigBtnControlPanel.Margin = new Padding(3, 4, 3, 4);
            ConfigBtnControlPanel.Name = "ConfigBtnControlPanel";
            ConfigBtnControlPanel.Size = new Size(98, 85);
            ConfigBtnControlPanel.TabIndex = 2;
            ConfigBtnControlPanel.Text = "Config";
            ConfigBtnControlPanel.UseVisualStyleBackColor = true;
            // 
            // syncTillBtnConfigPanel
            // 
            syncTillBtnConfigPanel.Dock = DockStyle.Bottom;
            syncTillBtnConfigPanel.Location = new Point(3, 283);
            syncTillBtnConfigPanel.Margin = new Padding(3, 4, 3, 4);
            syncTillBtnConfigPanel.Name = "syncTillBtnConfigPanel";
            syncTillBtnConfigPanel.Size = new Size(98, 85);
            syncTillBtnConfigPanel.TabIndex = 3;
            syncTillBtnConfigPanel.Text = "SyncTill";
            syncTillBtnConfigPanel.UseVisualStyleBackColor = true;
            // 
            // ReciptToggleBtnConfigPanel
            // 
            ReciptToggleBtnConfigPanel.Dock = DockStyle.Bottom;
            ReciptToggleBtnConfigPanel.Location = new Point(3, 376);
            ReciptToggleBtnConfigPanel.Margin = new Padding(3, 4, 3, 4);
            ReciptToggleBtnConfigPanel.Name = "ReciptToggleBtnConfigPanel";
            ReciptToggleBtnConfigPanel.Size = new Size(98, 85);
            ReciptToggleBtnConfigPanel.TabIndex = 4;
            ReciptToggleBtnConfigPanel.Text = "ReciptOff";
            ReciptToggleBtnConfigPanel.UseVisualStyleBackColor = true;
            // 
            // InfoBtnControlPanel
            // 
            InfoBtnControlPanel.Dock = DockStyle.Bottom;
            InfoBtnControlPanel.Location = new Point(3, 469);
            InfoBtnControlPanel.Margin = new Padding(3, 4, 3, 4);
            InfoBtnControlPanel.Name = "InfoBtnControlPanel";
            InfoBtnControlPanel.Size = new Size(98, 85);
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
            flowLayoutPanel2.Location = new Point(15, 751);
            flowLayoutPanel2.Margin = new Padding(3, 4, 3, 4);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(832, 63);
            flowLayoutPanel2.TabIndex = 9;
            // 
            // miscBtn
            // 
            miscBtn.Location = new Point(3, 4);
            miscBtn.Margin = new Padding(3, 4, 3, 4);
            miscBtn.Name = "miscBtn";
            miscBtn.Size = new Size(157, 59);
            miscBtn.TabIndex = 4;
            miscBtn.Text = "Misc";
            miscBtn.UseVisualStyleBackColor = true;
            miscBtn.Click += miscBtn_Click;
            // 
            // OrderBtn
            // 
            OrderBtn.Location = new Point(166, 4);
            OrderBtn.Margin = new Padding(3, 4, 3, 4);
            OrderBtn.Name = "OrderBtn";
            OrderBtn.Size = new Size(157, 59);
            OrderBtn.TabIndex = 5;
            OrderBtn.Text = "Order";
            OrderBtn.UseVisualStyleBackColor = true;
            OrderBtn.Click += OrderBtn_Click;
            // 
            // tableBottomBtn
            // 
            tableBottomBtn.Location = new Point(329, 4);
            tableBottomBtn.Margin = new Padding(3, 4, 3, 4);
            tableBottomBtn.Name = "tableBottomBtn";
            tableBottomBtn.Size = new Size(157, 59);
            tableBottomBtn.TabIndex = 6;
            tableBottomBtn.Text = "Table";
            tableBottomBtn.UseVisualStyleBackColor = true;
            tableBottomBtn.Click += tableBottomBtn_Click;
            // 
            // FinalBtn
            // 
            FinalBtn.Location = new Point(492, 4);
            FinalBtn.Margin = new Padding(3, 4, 3, 4);
            FinalBtn.Name = "FinalBtn";
            FinalBtn.Size = new Size(157, 59);
            FinalBtn.TabIndex = 7;
            FinalBtn.Text = "Final";
            FinalBtn.UseVisualStyleBackColor = true;
            FinalBtn.Click += FinalBtn_Click;
            // 
            // ConfigBtn
            // 
            ConfigBtn.Location = new Point(655, 4);
            ConfigBtn.Margin = new Padding(3, 4, 3, 4);
            ConfigBtn.Name = "ConfigBtn";
            ConfigBtn.Size = new Size(157, 59);
            ConfigBtn.TabIndex = 8;
            ConfigBtn.Text = "Config";
            ConfigBtn.UseVisualStyleBackColor = true;
            ConfigBtn.Click += ConfigBtn_Click;
            // 
            // finalPanel
            // 
            finalPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            finalPanel.BackColor = SystemColors.ControlDark;
            finalPanel.Controls.Add(CancelBtn);
            finalPanel.Controls.Add(SendToTableBtnFinalPanel);
            finalPanel.Controls.Add(CardBtn);
            finalPanel.Controls.Add(CashBtn);
            finalPanel.Location = new Point(889, 367);
            finalPanel.Margin = new Padding(3, 4, 3, 4);
            finalPanel.Name = "finalPanel";
            finalPanel.Size = new Size(105, 372);
            finalPanel.TabIndex = 8;
            finalPanel.Visible = false;
            // 
            // CancelBtn
            // 
            CancelBtn.Dock = DockStyle.Bottom;
            CancelBtn.Location = new Point(3, 4);
            CancelBtn.Margin = new Padding(3, 4, 3, 4);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(98, 85);
            CancelBtn.TabIndex = 0;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SendToTableBtnFinalPanel
            // 
            SendToTableBtnFinalPanel.Dock = DockStyle.Bottom;
            SendToTableBtnFinalPanel.Location = new Point(3, 97);
            SendToTableBtnFinalPanel.Margin = new Padding(3, 4, 3, 4);
            SendToTableBtnFinalPanel.Name = "SendToTableBtnFinalPanel";
            SendToTableBtnFinalPanel.Size = new Size(98, 85);
            SendToTableBtnFinalPanel.TabIndex = 1;
            SendToTableBtnFinalPanel.Text = "SendToTable";
            SendToTableBtnFinalPanel.UseVisualStyleBackColor = true;
            // 
            // CardBtn
            // 
            CardBtn.Dock = DockStyle.Bottom;
            CardBtn.Location = new Point(3, 190);
            CardBtn.Margin = new Padding(3, 4, 3, 4);
            CardBtn.Name = "CardBtn";
            CardBtn.Size = new Size(98, 85);
            CardBtn.TabIndex = 2;
            CardBtn.Text = "Card";
            CardBtn.UseVisualStyleBackColor = true;
            // 
            // CashBtn
            // 
            CashBtn.Dock = DockStyle.Bottom;
            CashBtn.Location = new Point(3, 283);
            CashBtn.Margin = new Padding(3, 4, 3, 4);
            CashBtn.Name = "CashBtn";
            CashBtn.Size = new Size(98, 85);
            CashBtn.TabIndex = 3;
            CashBtn.Text = "Cash";
            CashBtn.UseVisualStyleBackColor = true;
            // 
            // tablePanel
            // 
            tablePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            tablePanel.BackColor = SystemColors.ControlDark;
            tablePanel.Controls.Add(openTableBtn);
            tablePanel.Controls.Add(blankBtn);
            tablePanel.Controls.Add(sendToTableBtnTablePanel);
            tablePanel.Controls.Add(printBillBtn);
            tablePanel.Location = new Point(889, 367);
            tablePanel.Margin = new Padding(3, 4, 3, 4);
            tablePanel.Name = "tablePanel";
            tablePanel.Size = new Size(105, 372);
            tablePanel.TabIndex = 10;
            tablePanel.Visible = false;
            // 
            // openTableBtn
            // 
            openTableBtn.Dock = DockStyle.Bottom;
            openTableBtn.Location = new Point(3, 4);
            openTableBtn.Margin = new Padding(3, 4, 3, 4);
            openTableBtn.Name = "openTableBtn";
            openTableBtn.Size = new Size(98, 85);
            openTableBtn.TabIndex = 0;
            openTableBtn.Text = "OpenTable";
            openTableBtn.UseVisualStyleBackColor = true;
            // 
            // blankBtn
            // 
            blankBtn.Dock = DockStyle.Bottom;
            blankBtn.Location = new Point(3, 97);
            blankBtn.Margin = new Padding(3, 4, 3, 4);
            blankBtn.Name = "blankBtn";
            blankBtn.Size = new Size(98, 85);
            blankBtn.TabIndex = 1;
            blankBtn.UseVisualStyleBackColor = true;
            // 
            // sendToTableBtnTablePanel
            // 
            sendToTableBtnTablePanel.Dock = DockStyle.Bottom;
            sendToTableBtnTablePanel.Location = new Point(3, 190);
            sendToTableBtnTablePanel.Margin = new Padding(3, 4, 3, 4);
            sendToTableBtnTablePanel.Name = "sendToTableBtnTablePanel";
            sendToTableBtnTablePanel.Size = new Size(98, 85);
            sendToTableBtnTablePanel.TabIndex = 2;
            sendToTableBtnTablePanel.Text = "SendToTable";
            sendToTableBtnTablePanel.UseVisualStyleBackColor = true;
            // 
            // printBillBtn
            // 
            printBillBtn.Dock = DockStyle.Bottom;
            printBillBtn.Location = new Point(3, 283);
            printBillBtn.Margin = new Padding(3, 4, 3, 4);
            printBillBtn.Name = "printBillBtn";
            printBillBtn.Size = new Size(98, 85);
            printBillBtn.TabIndex = 3;
            printBillBtn.Text = "PrintBill";
            printBillBtn.UseVisualStyleBackColor = true;
            // 
            // orderPanel
            // 
            orderPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            orderPanel.BackColor = SystemColors.ControlDark;
            orderPanel.Controls.Add(setSeatBtn);
            orderPanel.Controls.Add(addItemBtn);
            orderPanel.Controls.Add(subtractItemBtn);
            orderPanel.Controls.Add(lineCorrectBtn);
            orderPanel.Controls.Add(toggleBtn);
            orderPanel.Controls.Add(multiplyBtn);
            orderPanel.Location = new Point(890, 180);
            orderPanel.Margin = new Padding(3, 4, 3, 4);
            orderPanel.Name = "orderPanel";
            orderPanel.Size = new Size(105, 559);
            orderPanel.TabIndex = 11;
            orderPanel.Visible = false;
            // 
            // setSeatBtn
            // 
            setSeatBtn.Dock = DockStyle.Bottom;
            setSeatBtn.Location = new Point(3, 4);
            setSeatBtn.Margin = new Padding(3, 4, 3, 4);
            setSeatBtn.Name = "setSeatBtn";
            setSeatBtn.Size = new Size(98, 85);
            setSeatBtn.TabIndex = 0;
            setSeatBtn.Text = "SetSeat";
            setSeatBtn.UseVisualStyleBackColor = true;
            // 
            // addItemBtn
            // 
            addItemBtn.Dock = DockStyle.Bottom;
            addItemBtn.Location = new Point(3, 97);
            addItemBtn.Margin = new Padding(3, 4, 3, 4);
            addItemBtn.Name = "addItemBtn";
            addItemBtn.Size = new Size(98, 85);
            addItemBtn.TabIndex = 1;
            addItemBtn.Text = "AddItem";
            addItemBtn.UseVisualStyleBackColor = true;
            // 
            // subtractItemBtn
            // 
            subtractItemBtn.Dock = DockStyle.Bottom;
            subtractItemBtn.Location = new Point(3, 190);
            subtractItemBtn.Margin = new Padding(3, 4, 3, 4);
            subtractItemBtn.Name = "subtractItemBtn";
            subtractItemBtn.Size = new Size(98, 85);
            subtractItemBtn.TabIndex = 2;
            subtractItemBtn.Text = "SubtractItem";
            subtractItemBtn.UseVisualStyleBackColor = true;
            // 
            // lineCorrectBtn
            // 
            lineCorrectBtn.Dock = DockStyle.Bottom;
            lineCorrectBtn.Location = new Point(3, 283);
            lineCorrectBtn.Margin = new Padding(3, 4, 3, 4);
            lineCorrectBtn.Name = "lineCorrectBtn";
            lineCorrectBtn.Size = new Size(98, 85);
            lineCorrectBtn.TabIndex = 3;
            lineCorrectBtn.Text = "LineCorrect";
            lineCorrectBtn.UseVisualStyleBackColor = true;
            // 
            // toggleBtn
            // 
            toggleBtn.Dock = DockStyle.Bottom;
            toggleBtn.Location = new Point(3, 376);
            toggleBtn.Margin = new Padding(3, 4, 3, 4);
            toggleBtn.Name = "toggleBtn";
            toggleBtn.Size = new Size(98, 85);
            toggleBtn.TabIndex = 4;
            toggleBtn.Text = "Toggle";
            toggleBtn.UseVisualStyleBackColor = true;
            // 
            // multiplyBtn
            // 
            multiplyBtn.Dock = DockStyle.Bottom;
            multiplyBtn.Location = new Point(3, 469);
            multiplyBtn.Margin = new Padding(3, 4, 3, 4);
            multiplyBtn.Name = "multiplyBtn";
            multiplyBtn.Size = new Size(98, 85);
            multiplyBtn.TabIndex = 5;
            multiplyBtn.Text = "Multiply";
            multiplyBtn.UseVisualStyleBackColor = true;
            // 
            // miscPanel
            // 
            miscPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            miscPanel.BackColor = SystemColors.ControlDark;
            miscPanel.Controls.Add(currentSeatBtn);
            miscPanel.Controls.Add(textMsgBtn);
            miscPanel.Controls.Add(presetMsgBtn);
            miscPanel.Controls.Add(categoryShiftBtn);
            miscPanel.Controls.Add(discountAmountBtn);
            miscPanel.Location = new Point(890, 273);
            miscPanel.Margin = new Padding(3, 4, 3, 4);
            miscPanel.Name = "miscPanel";
            miscPanel.Size = new Size(105, 465);
            miscPanel.TabIndex = 12;
            miscPanel.Visible = false;
            // 
            // currentSeatBtn
            // 
            currentSeatBtn.Dock = DockStyle.Bottom;
            currentSeatBtn.Location = new Point(3, 4);
            currentSeatBtn.Margin = new Padding(3, 4, 3, 4);
            currentSeatBtn.Name = "currentSeatBtn";
            currentSeatBtn.Size = new Size(98, 85);
            currentSeatBtn.TabIndex = 1;
            currentSeatBtn.Text = "CurrentSeat";
            currentSeatBtn.UseVisualStyleBackColor = true;
            // 
            // textMsgBtn
            // 
            textMsgBtn.Dock = DockStyle.Bottom;
            textMsgBtn.Location = new Point(3, 97);
            textMsgBtn.Margin = new Padding(3, 4, 3, 4);
            textMsgBtn.Name = "textMsgBtn";
            textMsgBtn.Size = new Size(98, 85);
            textMsgBtn.TabIndex = 2;
            textMsgBtn.Text = "TextMsg";
            textMsgBtn.UseVisualStyleBackColor = true;
            // 
            // presetMsgBtn
            // 
            presetMsgBtn.Dock = DockStyle.Bottom;
            presetMsgBtn.Location = new Point(3, 190);
            presetMsgBtn.Margin = new Padding(3, 4, 3, 4);
            presetMsgBtn.Name = "presetMsgBtn";
            presetMsgBtn.Size = new Size(98, 85);
            presetMsgBtn.TabIndex = 3;
            presetMsgBtn.Text = "PresetMsg";
            presetMsgBtn.UseVisualStyleBackColor = true;
            // 
            // categoryShiftBtn
            // 
            categoryShiftBtn.Dock = DockStyle.Bottom;
            categoryShiftBtn.Location = new Point(3, 283);
            categoryShiftBtn.Margin = new Padding(3, 4, 3, 4);
            categoryShiftBtn.Name = "categoryShiftBtn";
            categoryShiftBtn.Size = new Size(98, 85);
            categoryShiftBtn.TabIndex = 4;
            categoryShiftBtn.Text = "Category Shift";
            categoryShiftBtn.UseVisualStyleBackColor = true;
            // 
            // discountAmountBtn
            // 
            discountAmountBtn.Dock = DockStyle.Bottom;
            discountAmountBtn.Location = new Point(3, 376);
            discountAmountBtn.Margin = new Padding(3, 4, 3, 4);
            discountAmountBtn.Name = "discountAmountBtn";
            discountAmountBtn.Size = new Size(98, 85);
            discountAmountBtn.TabIndex = 5;
            discountAmountBtn.Text = "Discount Amount";
            discountAmountBtn.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(panel1);
            flowLayoutPanel1.Controls.Add(panel2);
            flowLayoutPanel1.Controls.Add(catPan);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(15, 51);
            flowLayoutPanel1.Margin = new Padding(3, 4, 3, 4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(984, 696);
            flowLayoutPanel1.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 825);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(finalPanel);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(ConfigPannel);
            Controls.Add(tablePanel);
            Controls.Add(miscPanel);
            Controls.Add(orderPanel);
            Controls.Add(BackBtn);
            Controls.Add(tableBtn);
            Controls.Add(PricingBtn);
            Controls.Add(nameBtn);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ConfigPannel.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            finalPanel.ResumeLayout(false);
            tablePanel.ResumeLayout(false);
            orderPanel.ResumeLayout(false);
            miscPanel.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
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
        private FlowLayoutPanel finalPanel;
        private Button CancelBtn;
        private Button SendToTableBtnFinalPanel;
        private Button CardBtn;
        private Button CashBtn;
        private FlowLayoutPanel tablePanel;
        private Button openTableBtn;
        private Button blankBtn;
        private Button sendToTableBtnTablePanel;
        private Button printBillBtn;
        private FlowLayoutPanel orderPanel;
        private Button setSeatBtn;
        private Button addItemBtn;
        private Button subtractItemBtn;
        private Button lineCorrectBtn;
        private Button toggleBtn;
        private Button multiplyBtn;
        private FlowLayoutPanel miscPanel;
        private Button currentSeatBtn;
        private Button textMsgBtn;
        private Button presetMsgBtn;
        private Button categoryShiftBtn;
        private Button discountAmountBtn;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}
