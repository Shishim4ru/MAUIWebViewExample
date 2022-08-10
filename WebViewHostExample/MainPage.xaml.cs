﻿namespace WebViewHostExample;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		MyWebView.Source = new HtmlWebViewSource() { Html = htmlSource };

        MyWebView.JavaScriptAction += MyWebView_JavaScriptAction;
	}

    private void MyWebView_JavaScriptAction(object sender, Controls.JavaScriptActionEventArgs e)
    {
		Dispatcher.Dispatch(() =>
		{
            ChangeLabel.Text = "The Web Button Was Clicked! Count: " + e.Payload;
        });
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
</script>

<div style='display: flex; flex-direction: column; justify-content: center; width: 100%'>
<button style='margin-left: 15px; margin-right: 15px; background: lightblue' id='hereBtn' onclick='javascript:buttonClicked(event)'>Click Me!</button>
</div>
</html>
";

	
}

