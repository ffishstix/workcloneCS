using System.Diagnostics;

namespace WorkCloneCS;

public class MessageForm : Form
{
    private readonly TextBox messageBox;

    public string MessageText => messageBox.Text;

    public MessageForm(string title, string initialText = "")
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        Width = 520;
        Height = 200;
        KeyPreview = true;

        messageBox = new TextBox
        {
            Left = 12,
            Top = 12,
            Width = 478,
            Height = 26,
            Text = initialText,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        Button keyboardBtn = new Button
        {
            Left = 12,
            Top = 50,
            Width = 180,
            Height = 28,
            Text = "On-Screen Keyboard",
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        };

        Button okBtn = new Button
        {
            Left = 330,
            Top = 110,
            Width = 78,
            Height = 30,
            Text = "OK",
            DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom
        };

        Button cancelBtn = new Button
        {
            Left = 412,
            Top = 110,
            Width = 78,
            Height = 30,
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom
        };

        keyboardBtn.Click += (_, _) => OpenWindowsKeyboard();
        Shown += (_, _) =>
        {
            messageBox.Focus();
            messageBox.SelectionStart = messageBox.TextLength;
        };

        AcceptButton = okBtn;
        CancelButton = cancelBtn;

        Controls.Add(messageBox);
        Controls.Add(keyboardBtn);
        Controls.Add(okBtn);
        Controls.Add(cancelBtn);
    }

    private static void OpenWindowsKeyboard()
    {
        try
        {
            Process.Start(new ProcessStartInfo("osk.exe")
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Logger.Log($"failed to open on-screen keyboard: {ex.Message}");
        }
    }
}