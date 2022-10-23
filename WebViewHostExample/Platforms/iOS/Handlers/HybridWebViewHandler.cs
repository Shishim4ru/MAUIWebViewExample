using CoreGraphics;

using Foundation;

using Microsoft.Maui.Handlers;

using WebKit;

using WebViewHostExample.Controls;

namespace WebViewHostExample.Handlers
{
    public partial class HybridWebViewHandler : WebViewHandler
    {
        public static PropertyMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewPropertyMapper = new PropertyMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.Mapper);
        public static CommandMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewCommandMapper = new CommandMapper<IHybridWebView, HybridWebViewHandler>(WebViewHandler.CommandMapper);

        private WKUserContentController userController;
        private JSBridge jsBridgeHandler;


        public HybridWebViewHandler() : base(HybridWebViewPropertyMapper, HybridWebViewCommandMapper)
        {
        }

        protected override WKWebView CreatePlatformView()
        {
            var platformView = base.CreatePlatformView();
            
            jsBridgeHandler = new JSBridge(this);
            userController = platformView.Configuration.UserContentController ?? new WKUserContentController();

            userController.AddScriptMessageHandler(jsBridgeHandler, "invokeAction");

            return platformView;            
        }

        protected override void ConnectHandler(WKWebView platformView)
        {
            base.ConnectHandler(platformView);
        }


        protected override void DisconnectHandler(WKWebView platformView)
        {
            base.DisconnectHandler(platformView);


            userController.RemoveAllUserScripts();
            userController.RemoveScriptMessageHandler("invokeAction");
        
            jsBridgeHandler?.Dispose();
            jsBridgeHandler = null;
        }


    }

    public class JSBridge : NSObject, IWKScriptMessageHandler
    {
        readonly WeakReference<HybridWebViewHandler> hybridWebViewRenderer;

        internal JSBridge(HybridWebViewHandler hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewHandler>(hybridRenderer);
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            HybridWebViewHandler hybridRenderer;

            if (hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                (hybridRenderer.VirtualView as HybridWebView)?.InvokeAction(message.Body.ToString());
            }
        }
    }


}
