using PartPartWhole.ViewModels;

namespace PartPartWhole;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        this.BindingContext = new ViewModels.PartPartWholeViewModel();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        //count++;

        /*if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";*/
        //int sum = (int)(Sum.GetValue().ToString());

        //SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void addent2_TextChanged(object sender, TextChangedEventArgs e)
    {
        
    }
}

