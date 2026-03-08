namespace WorkCloneCS;

public partial class allergiesForm : Form
{
    private readonly HashSet<string> selectedAllergies = new(StringComparer.OrdinalIgnoreCase);

    public List<string> SelectedAllergies => selectedAllergies.OrderBy(x => x).ToList();

    public allergiesForm()
    {
        InitializeComponent();
        wireEvents();
    }

    public allergiesForm(IEnumerable<string> availableAllergies, IEnumerable<string> preSelectedAllergies)
    {
        InitializeComponent();
        wireEvents();
        populateLists(availableAllergies, preSelectedAllergies);
    }

    private void wireEvents()
    {
        selectBtn.Click -= selectBtn_Click;
        deselectBtn.Click -= deselectBtn_Click;
        deselectAllBtn.Click -= deselectAllBtn_Click;
        selectBtn.Click += selectBtn_Click;
        deselectBtn.Click += deselectBtn_Click;
        deselectAllBtn.Click += deselectAllBtn_Click;
    }

    private void populateLists(IEnumerable<string> availableAllergies, IEnumerable<string> preSelectedAllergies)
    {
        selectedBox.Items.Clear();
        selectableItems.Items.Clear();
        selectedAllergies.Clear();

        HashSet<string> preSelected = new(
            preSelectedAllergies?.Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>(),
            StringComparer.OrdinalIgnoreCase
        );

        foreach (string allergyName in preSelected.OrderBy(x => x))
        {
            selectedAllergies.Add(allergyName);
            selectedBox.Items.Add(allergyName);
        }

        IEnumerable<string> available =
            availableAllergies?.Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>();

        foreach (string allergyName in available.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x))
        {
            if (preSelected.Contains(allergyName)) continue;
            selectableItems.Items.Add(allergyName);
        }
    }

    private void selectBtn_Click(object sender, EventArgs e)
    {
        int selectedIndex = selectableItems.SelectedIndex;
        if (selectedIndex == -1) return;

        string item = selectableItems.SelectedItem?.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(item)) return;

        if (selectedAllergies.Add(item))
        {
            selectedBox.Items.Add(item);
        }

        selectableItems.Items.RemoveAt(selectedIndex);
    }

    private void deselectBtn_Click(object sender, EventArgs e)
    {
        int selectedIndex = selectedBox.SelectedIndex;
        if (selectedIndex == -1) return;

        string item = selectedBox.SelectedItem?.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(item)) return;

        selectedBox.Items.RemoveAt(selectedIndex);
        selectedAllergies.Remove(item);
        if (selectableItems.FindStringExact(item) == -1) selectableItems.Items.Add(item);
    }

    private void deselectAllBtn_Click(object sender, EventArgs e)
    {
        selectedBox.BeginUpdate();
        while (selectedBox.Items.Count > 0)
        {
            string item = selectedBox.Items[0]?.ToString() ?? "";
            if (!string.IsNullOrWhiteSpace(item))
            {
                selectedAllergies.Remove(item);
                if (selectableItems.FindStringExact(item) == -1) selectableItems.Items.Add(item);
            }

            selectedBox.Items.RemoveAt(0);
        }

        selectedBox.EndUpdate();
    }
}