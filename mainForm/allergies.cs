namespace WorkCloneCS;

public partial class allergiesForm : Form
{
    public allergiesForm()
    {
        InitializeComponent();
    }

    private void selectBtn_Click(object sender, EventArgs e)
    {
        int selectedIndex = selectedBox.SelectedIndex;
        if (selectedIndex == -1) return; // if nothing selected returns.
        string item = selectedBox.SelectedItem.ToString();
        Logger.Log($"selected index: {selectedIndex} , item: {item}");
        if (selectedBox.FindString(item) == -1)selectedBox.Items.Add(item);

    }
    
    private void deselectBtn_Click(object sender, EventArgs e)
    {
        int selectedIndex = selectedBox.SelectedIndex;
        if (selectedIndex == -1) return; // if nothing selected returns.
        Logger.Log("deselect not null");
        string item = selectedBox.SelectedItem.ToString();
        Logger.Log($"selected index: {selectedIndex} , item: {item}");
        try
        {
            selectedBox.Items.RemoveAt(selectedIndex);
            Logger.Log("inside the try");
        }
        catch (Exception ex)
        {
            Logger.Log($"error in deselectBtn_Click: most likely because the item doesnt exist: {ex.Message}");
        }
        
    }
    private void deselectAllBtn_Click(object sender, EventArgs e)
    {
        selectedBox.BeginUpdate();
        while (selectedBox.Items.Count > 0)
        {
            selectedBox.Items.RemoveAt(0);
        }
        selectedBox.EndUpdate();
        Logger.Log("removed all items in select box (deselectAllBtn_Click)");
    }

    
    
}