namespace WebViewHostExample.Controls
{
    public partial class HybridWebView : WebView, IHybridWebView
    {
        private static partial string JavaScriptFunction(string function_name) => $"function {function_name}(data){{jsBridge.invokeAction(JSON.stringify({{FunctionName:\"{function_name}\",Data:data}}));}}";
    }
}
