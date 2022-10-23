using System.Text.Json;


namespace WebViewHostExample.Controls
{
    public class CallbackData
    {
        public string FunctionName { get; set; }
        public object Data { get; set; }
    }

    public interface IHybridWebView : IWebView
    {
        void Cleanup();
        void InvokeAction(string data);
    }
        

    public partial class HybridWebView : WebView, IHybridWebView
    {
        private Dictionary<string, Action<object>> JSFunctions = new();
        private bool PageLoaded = false;

        public HybridWebView()
        {
            Navigated += HybridWebView_Navigated;
        }

        private void HybridWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            foreach (var action in JSFunctions)
            {
                InjectCallback(action.Key);
            }
            PageLoaded = true;
        }

        public void Cleanup()
        {
            PageLoaded = false;
            JSFunctions.Clear();
        }

        /// <summary>
        /// Call this method to link the javascript function <paramref name="function_name"/> to the C# action <paramref name="callback"/>
        /// </summary>
        /// <param name="function_name">Name of the javascript function</param>
        /// <param name="callback">C# action to call</param>
        public void AddLocalCallback(string function_name, Action<object> callback)
        {
            if (PageLoaded)
            {
                InjectCallback(function_name);
            }
            JSFunctions[function_name] = callback;
        }

        private void InjectCallback(string function_name)
        {
            Eval(JavaScriptFunction(function_name));
        }

        public void InvokeAction(string data)
        {
            CallbackData callbackData = JsonSerializer.Deserialize<CallbackData>(data);
            if (JSFunctions.ContainsKey(callbackData.FunctionName))
            {
                JSFunctions[callbackData.FunctionName]?.Invoke(callbackData.Data);
            }
        }

        private static partial string JavaScriptFunction(string function_name);
    }
}
