using static System.Net.Mime.MediaTypeNames;

namespace WebViewHostExample;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();

        MyWebView.AddLocalCallback("invokeCSharpAction", ChangeLabelText);
        MyWebView.Source = new HtmlWebViewSource() { Html = htmlSource };
    }

    private void ChangeLabelText(object text)
    {
        Dispatcher.Dispatch(() =>
        {
            ChangeLabel.Text = "The Web Button Was Clicked! Count: " + text;
        });
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
    }

    string htmlSource = @"
<html>
<head></head>
<body>

<script>
    var counter = 1;
    function buttonClicked(e) {		
		invokeCSharpAction(counter++);
    }

    function nativeDemand(data) {
         var el = document.getElementById('webtext');
        var old_text = el.innerText;
         el.innerHTML = data;
        return old_text;
    }

</script>

<div style='display: flex; flex-direction: column; justify-content: center; align-items: center; width: 100%'>
<h2 style='font-family: script'><i>Fancy Web Title</i></h2>
<button style='height:48px; margin-left: 15px; margin-right: 15px; width: 128px; background: lightblue' id='hereBtn' onclick='javascript:buttonClicked(event)'>Click Me!</button>
<div id='webtext' style='font-family: script'><b>This web text will change when you push the native button.</b></div>
</div>
</html>
";

    private async void EvalButton_Clicked(object sender, EventArgs e)
    {
        string old_text = await MyWebView.EvaluateJavaScriptAsync("nativeDemand('" + ChangeText.Text + "')");
        Dispatcher.Dispatch(() =>
        {
            ChangeLabel.Text = old_text;
        });
    }
}

