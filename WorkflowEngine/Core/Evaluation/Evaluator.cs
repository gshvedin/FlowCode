using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using WorkflowEngine.Misc;
using Wmhelp.XPath2;
using WorkflowEngine.Core.Evaluation.XPathExtensions;

namespace WorkflowEngine.Core.Evaluation
{
    public static class Evaluator
    {
        /*
         * Testing of performance difference between two evaluators:
         * Sample count: 1000000
         * PYTHON result: 0 m 28 sec
         * XPath result: 0 m 6 sec
         * WINNER: XPath
         * XPath selected as a default Evaluator. Py willbe challenger and requires setting type "Python" in attribute type of expression
         * Test 1000000 attempts of calculation result:
            XPATH2:00:00:14.1080049
            XPATH:00:00:07.9167005
        int attemptCalc = 1000000;
        DateTime dts = DateTime.Now;
        for (int i = 0; i<attemptCalc; i++)
        {
            Evaluator.EvaluatePython($"{i}*{i}+{i}");
        }
        Console.WriteLine($"Test result:{DateTime.Now.Subtract(dts)}");
        TimeSpan tsi = DateTime.Now.Subtract(dts);
        DateTime dts2 = DateTime.Now;
        for (int i = 0; i<attemptCalc; i++)
        {
            Evaluator.EvaluateXPath($"{i}*{i}+{i}");
        }
        Console.WriteLine($"Test {attemptCalc} attempts of calculation result:\nXPATH:{DateTime.Now.Subtract(dts2)}\nJSCRIPT:{tsi}");
        */

        private static XmlDocument xDoc;
        private static ScriptEngine pythonEval;

        [MethodTimer.Time]
        public static string EvaluateXPath(string expr, bool useXPath2 = false)
        {
            if (xDoc == null)
            {
                xDoc = new XmlDocument();
            }

            // replace , with . in case of not completed expression [121,*2] => [121*2]
            expr = Regex.Replace(expr, @"(?<d1>\d{1})[,](?<d2>[*+-/])", @"${d1}${d2}");
            ////replace , with . in general case  [121,22*2] => [121.22*2]  
            /// expr = Regex.Replace(expr, @"(?<d1>\d{1})[,](?<d2>\d{1})", @"${d1}.${d2}");
            ////replace , with empty in case of not completed expression number('121,')  => [number('121')]
            expr = Regex.Replace(expr, @"(?<d1>\d{1})[,][']", @"${d1}'");
            ////replace empty parameter for contains function that returns always true to make false
            if (expr.Contains("contains"))
            {
                expr = Regex.Replace(expr, @"[contains\(.*?,\s*?]''\)", "'empty parameter')");
            }

            string result;
            try
            {
                if (useXPath2)
                {
                    result = xDoc.CreateNavigator().XPath2Evaluate(expr).ToString();
                }
                else
                {
                    XPathExpression expression = GetCompiledExpression(expr);
                    result = xDoc.CreateNavigator().Evaluate(expression).ToString();
                }
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Error of XPath expression:\n{ex.Message}\nExpression text:{expr}");
            }

            return result;
        }

        [MethodTimer.Time]
        public static string EvaluateXPath(XmlDocument document, string expression, bool useXPath2 = false)
        {
            string result;
            try
            {
                if (!(expression.Contains("./") || expression.StartsWith("count(") || expression.StartsWith("sum(")))
                {
                    expression = ".//" + expression;
                }

                object evalResult = null;
                if (useXPath2)
                {
                    evalResult = document?.CreateNavigator().XPath2Evaluate(expression);
                }
                else
                {
                    evalResult = document?.CreateNavigator().Evaluate(expression);
                }

                if (evalResult is XPathNodeIterator)
                {
                    result = document.SelectSingleNode(expression)?.InnerText;
                }
                else
                {
                    result = evalResult.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Error of XPath expression:\n{ex.Message}\nExpression text:{expression}");
            }

            return result;
        }

        [MethodTimer.Time]
        public static string EvaluatePython(string expr)
        {
            if (pythonEval == null)
            {
                pythonEval = Python.CreateEngine();
            }

            try
            {
                return pythonEval.Execute(expr)?.ToString();
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Error of Python expression:\n{ex.Message}\nExpression text:{expr}");
            }
        }

        public static bool EvaluateRegexp(string expression, string value)
        {
            Match match = Regex.Match(value, expression);
            return match.Success;
        }

        private static XPathExpression GetCompiledExpression(string xpath)
        {
            // Compile the XPath expression and set its context to the XPathContext
            XPathExpression expr = XPathExpression.Compile(xpath);
            if (xpath.Contains("wf:", StringComparison.InvariantCulture))
            {
                CustomXsltContext context = new CustomXsltContext();
                context.AddNamespace("wf", "http://xpathExtensions");
                expr.SetContext(context);
            }

            return expr;
        }


    }
}
