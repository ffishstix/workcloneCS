using System.ComponentModel;

namespace WorkCloneCS;

partial class allergiesForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        deselectAllBtn = new System.Windows.Forms.Button();
        deselectBtn = new System.Windows.Forms.Button();
        selectedBox = new System.Windows.Forms.ListBox();
        selectableItems = new System.Windows.Forms.ListBox();
        selectBtn = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // deselectAllBtn
        // 
        deselectAllBtn.Location = new System.Drawing.Point(172, 118);
        deselectAllBtn.Name = "deselectAllBtn";
        deselectAllBtn.Size = new System.Drawing.Size(134, 32);
        deselectAllBtn.TabIndex = 8;
        deselectAllBtn.Text = "deselect All";
        deselectAllBtn.UseVisualStyleBackColor = true;
        // 
        // deselectBtn
        // 
        deselectBtn.Location = new System.Drawing.Point(172, 51);
        deselectBtn.Name = "deselectBtn";
        deselectBtn.Size = new System.Drawing.Size(134, 32);
        deselectBtn.TabIndex = 7;
        deselectBtn.Text = "remove";
        deselectBtn.UseVisualStyleBackColor = true;
        // 
        // selectedBox
        // 
        selectedBox.FormattingEnabled = true;
        selectedBox.ItemHeight = 15;
        selectedBox.Location = new System.Drawing.Point(312, 11);
        selectedBox.Name = "selectedBox";
        selectedBox.Size = new System.Drawing.Size(161, 139);
        selectedBox.TabIndex = 6;
        // 
        // selectableItems
        // 
        selectableItems.FormattingEnabled = true;
        selectableItems.ItemHeight = 15;
        selectableItems.Items.AddRange(new object[] { "peanuts", "dairy", "Eggs", "nuts", "wheat", "soy", "Fish", "Meat", "gluten" });
        selectableItems.Location = new System.Drawing.Point(5, 11);
        selectableItems.Name = "selectableItems";
        selectableItems.Size = new System.Drawing.Size(161, 139);
        selectableItems.TabIndex = 5;
        // 
        // selectBtn
        // 
        selectBtn.Location = new System.Drawing.Point(172, 13);
        selectBtn.Name = "selectBtn";
        selectBtn.Size = new System.Drawing.Size(134, 32);
        selectBtn.TabIndex = 9;
        selectBtn.Text = "select";
        selectBtn.UseVisualStyleBackColor = true;
        
        // 
        // allergies
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(479, 163);
        Controls.Add(selectBtn);
        Controls.Add(deselectAllBtn);
        Controls.Add(deselectBtn);
        Controls.Add(selectedBox);
        Controls.Add(selectableItems);
        Text = "allergies";
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button selectBtn;

    private System.Windows.Forms.Button deselectAllBtn;
    private System.Windows.Forms.Button deselectBtn;
    private System.Windows.Forms.ListBox selectedBox;
    private System.Windows.Forms.ListBox selectableItems;

    #endregion
}