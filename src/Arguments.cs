using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace asmref
{
    public class Arguments
    {
        private static readonly Regex OptionRegex = new Regex(@"[\-\\/](\w{1})", RegexOptions.Compiled);

        private static readonly Dictionary<string, Action<Arguments>> OptionActions = new Dictionary<string, Action<Arguments>>(StringComparer.OrdinalIgnoreCase)
        {
            {"u", a => a.IsUpward = true},
            {"d", a => a.IsDownward = true},
            {"i", a => a.IsInteractiveMode = true}
        };

        public string AssemblyOrFileName { get; private set; }

        public bool IsUpward { get; private set; }

        public bool IsDownward { get; private set; }

        public bool IsInteractiveMode { get; private set; }

        public string ErrorMessage { get; private set; }

        private Arguments()
        {
        }

        public static Arguments Parse(string[] args)
        {
            var arguments = new Arguments();

            foreach (var arg in args)
            {
                var match = OptionRegex.Match(arg);
                if (match.Success)
                {
                    var option = match.Groups[1].Value;
                    Action<Arguments> action;
                    if (OptionActions.TryGetValue(option, out action))
                    {
                        action(arguments);
                    }
                    else
                    {
                        return BuildFailedArguments($"Unexpected option: {arg}");
                    }
                }
                else if (arguments.AssemblyOrFileName != null)
                {
                    return BuildFailedArguments($"Unexpected argument: {arg}");
                }
                else
                {
                    arguments.AssemblyOrFileName = arg;
                }
            }

            if (arguments.AssemblyOrFileName == null)
            {
                return BuildFailedArguments("Please specify short assembly name or assembly file name.");
            }

            if (arguments.IsUpward && arguments.IsDownward)
            {
                return BuildFailedArguments(@"""-u"" and ""-d"" cannot be used together.");
            }

            if (!arguments.IsUpward && !arguments.IsDownward)
            {
                arguments.IsUpward = true;
            }

            if (arguments.IsDownward && arguments.IsInteractiveMode)
            {
                return BuildFailedArguments("Downward inspection cannot be run in interactive mode.");
            }

            return arguments;
        }

        public static string BuildHelpMessage()
        {
            return $@"
Usage:

{Assembly.GetEntryAssembly().GetModules()[0].Name.ToLower()} [-u [-i]|-d] <short assembly name>|<assembly file name>

-u   : Show assemblies that reference the given assembly (upward links).
-d   : Show assemblies that are referenced the given assembly (downward links).
-i   : Interactive mode (for upward inspection only).

""-u"" and ""-d"" cannot be used together. If both are omitted, the default is ""-u"".";
        }

        private static Arguments BuildFailedArguments(string errorMessage)
        {
            return new Arguments { ErrorMessage = errorMessage };
        }
    }
}