using Android.Webkit;

using Java.Interop;

using Microsoft.Maui.Handlers;

using WebViewHostExample.Controls;

namespace WebViewHostExample.Handlers
{
    public partial class HybridWebViewHandler : WebViewHandler
    {
        public static PropertyMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewPropertyMapper = new PropertyMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.Mapper);
        public static CommandMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewCommandMapper = new CommandMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.CommandMapper);

        private JSBridge jsBridgeHandler;

        public HybridWebViewHandler() : base(HybridWebViewPropertyMapper, HybridWebViewCommandMapper)
        {
        }

        protected override Android.Webkit.WebView CreatePlatformView()
        {
            Android.Webkit.WebView platformView = base.CreatePlatformView();
            jsBridgeHandler = new JSBridge(this);

            platformView.AddJavascriptInterface(jsBridgeHandler, "jsBridge");

            return platformView;
        }

        protected override void ConnectHandler(Android.Webkit.WebView platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(Android.Webkit.WebView platformView)
        {
            base.DisconnectHandler(platformView);

            (VirtualView as HybridWebView).Cleanup();

            jsBridgeHandler?.Dispose();
            jsBridgeHandler = null;
        }
    }

    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewHandler> hybridWebViewRenderer;

        internal JSBridge(HybridWebViewHandler hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewHandler>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            HybridWebViewHandler hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                (hybridRenderer.VirtualView as HybridWebView)?.InvokeAction(data);
            }
        }
    }

}
