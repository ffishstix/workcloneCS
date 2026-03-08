using System.Drawing.Drawing2D;

namespace WorkCloneCS;

public partial class TableForm
{
    private readonly ToolTip tableSummaryToolTip = new()
    {
        AutomaticDelay = 100,
        InitialDelay = 100,
        ReshowDelay = 100,
        AutoPopDelay = 3000,
        ShowAlways = true
    };

    private Dictionary<int, (int itemCount, decimal totalPrice)> openTableSummaries = new();

    private void clickSection()
    {
        foreach (Button button in getTableButtons()) registerTableButton(button);
    }

    private IEnumerable<Button> getTableButtons()
    {
        // For buttons 1-87
        for (int i = 1; i <= 87; i++)
        {
            Control[] matches = Controls.Find($"button{i}", true);
            if (matches.Length > 0 && matches[0] is Button button) yield return button;
        }

        for (int i = 90; i <= 98; i++)
        {
            Control[] matches = Controls.Find($"button{i}", true);
            if (matches.Length > 0 && matches[0] is Button button) yield return button;
        }

        yield return button300;
        yield return button500;
        yield return button700;
        yield return button800;
    }

    private void registerTableButton(Button button)
    {
        button.Click += tableBtn_Click;
        button.DoubleClick += tableBtn_DoubleClick;
        button.MouseEnter += tableBtn_MouseEnter;
        button.MouseLeave += tableBtn_MouseLeave;
        button.Paint += tableBtn_Paint;
    }

    private void initialiseOpenTables()
    {
        openTableSummaries = database.getOpenTableSummaries();

        foreach (Button button in getTableButtons())
        {
            button.Invalidate();
        }
    }

    private void tableBtn_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is Button button) showTableSummaryPopup(button);
    }

    private void tableBtn_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is Button button) tableSummaryToolTip.Hide(button);
    }

    private void tableBtn_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button button) return;
        if (!tryGetTableId(button, out int tableId)) return;
        if (!openTableSummaries.ContainsKey(tableId)) return;

        Size textSize = TextRenderer.MeasureText(button.Text, button.Font);
        const int ringPadding = 4;
        Rectangle ringRect = new(
            (button.Width - textSize.Width) / 2 - ringPadding,
            (button.Height - textSize.Height) / 2 - ringPadding,
            textSize.Width + (ringPadding * 2) - 2,
            textSize.Height + (ringPadding * 2) - 2
        );

        if (ringRect.Width <= 0 || ringRect.Height <= 0) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using Pen pen = new(Color.Red, 2f);
        e.Graphics.DrawEllipse(pen, ringRect);
    }

    private void showTableSummaryPopup(Button button)
    {
        if (!tryGetTableId(button, out int tableId)) return;

        (int itemCount, decimal totalPrice) summary =
            openTableSummaries.TryGetValue(tableId, out var found)
                ? found
                : (0, 0m);

        string message = $"Items: {summary.itemCount}  Price: {summary.totalPrice:C}";
        tableSummaryToolTip.Show(message, button, button.Width / 2, -28, 2500);
    }

    private static bool tryGetTableId(Button button, out int tableId)
    {
        return int.TryParse(button.Text, out tableId);
    }
}