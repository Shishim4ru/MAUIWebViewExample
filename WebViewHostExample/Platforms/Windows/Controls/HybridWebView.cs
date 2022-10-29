using System.Text.RegularExpressions;
using WebViewHostExample.Handlers;

namespace WebViewHostExample.Controls
{
    public partial class HybridWebView : WebView, IHybridWebView
    {
        private static partial string JavaScriptFunction(string function_name) => $"function {function_name}(data){{window.chrome.webview.postMessage(JSON.stringify({{FunctionName:\"{function_name}\",Data:data}}));}}";

        public new async Task<string> EvaluateJavaScriptAsync(string script)
        {
            if (script == null)
                return null;

            script = EscapeJsString(script);
            script = "try{eval('" + script + "')}catch(e){'null'};";

            string result;


            // Use the handler command to evaluate the JS
            result = await (Handler as HybridWebViewHandler).ExecuteScriptAsync(script);

            //if the js function errored or returned null/undefined treat it as null
            if (result == "null")
                result = null;

            //JSON.stringify wraps the result in literal quotes, we just want the actual returned result
            //note that if the js function returns the string "null" we will get here and not above
            else if (result != null)
                result = result.Trim('"');

            return result;
        }

        static string EscapeJsString(string js)
        {
            if (js == null)
                return null;

            if (js.IndexOf("'", StringComparison.Ordinal) == -1)
                return js;

            //get every quote in the string along with all the backslashes preceding it
            var singleQuotes = Regex.Matches(js, @"(\\*?)'");

            var uniqueMatches = new List<string>();

            for (var i = 0; i < singleQuotes.Count; i++)
            {
                var matchedString = singleQuotes[i].Value;
                if (!uniqueMatches.Contains(matchedString))
                {
                    uniqueMatches.Add(matchedString);
                }
            }

            uniqueMatches.Sort((x, y) => y.Length.CompareTo(x.Length));

            //escape all quotes from the script as well as add additional escaping to all quotes that were already escaped
            for (var i = 0; i < uniqueMatches.Count; i++)
            {
                var match = uniqueMatches[i];
                var numberOfBackslashes = match.Length - 1;
                var slashesToAdd = (numberOfBackslashes * 2) + 1;
                var replacementStr = "'".PadLeft(slashesToAdd + 1, '\\');
                js = Regex.Replace(js, @"(?<=[^\\])" + Regex.Escape(match), replacementStr);
            }

            return js;
        }
    }
}
