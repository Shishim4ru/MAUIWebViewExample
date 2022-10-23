using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Handlers;

using WebViewHostExample.Controls;
using System.Net;
using System.Text;

namespace WebViewHostExample.Handlers;

public partial class HybridWebViewHandler : WebViewHandler
{
    public static PropertyMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewPropertyMapper = new PropertyMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.Mapper);
    public static CommandMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewCommandMapper = new CommandMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.CommandMapper);

    public HybridWebViewHandler() : base(HybridWebViewPropertyMapper, HybridWebViewCommandMapper)
    {
        
    }

    protected override WebView2 CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();
        return platformView;
    }

    protected override void ConnectHandler(WebView2 platformView)
    {
        base.ConnectHandler(platformView);
        platformView.WebMessageReceived += PlatformView_WebMessageReceived;
    }

    private void PlatformView_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
    {
        var json = e.TryGetWebMessageAsString();  // always a JSON string
        if (!string.IsNullOrEmpty(json))
        {
            (VirtualView as HybridWebView)?.InvokeAction(json);
        }
    }

    protected override void DisconnectHandler(WebView2 platformView)
    {
        platformView.WebMessageReceived -= PlatformView_WebMessageReceived;
        (VirtualView as HybridWebView).Cleanup();
        base.DisconnectHandler(platformView);
    }
}