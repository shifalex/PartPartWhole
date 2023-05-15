namespace PartPartWhole;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
        this.BindingContext = new ViewModels.PartPartWholeViewModel();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
    }

    private void addent2_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}

