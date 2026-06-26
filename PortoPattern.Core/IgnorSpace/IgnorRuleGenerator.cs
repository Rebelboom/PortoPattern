#nullable enable
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PortoPattern.Core.IgnorSpace;

public class IgnorRuleGenerator
{
    public IgnorRule? CreateRule(string rawPath, bool isGlobal)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
#if DEBUG
            Console.WriteLine("[DEBUG ERROR]: rawPath is null or empty.");
#endif
            return null;
        }

        string cleanedPath = rawPath.Trim().Replace('/', '\\');
        string folderName = Path.GetFileName(cleanedPath);

        if (string.IsNullOrEmpty(folderName))
        {
            folderName = Path.GetFileName(Path.GetDirectoryName(cleanedPath)) ?? "unknown_folder";
        }

        string safeVariableName = NormalizeVariableName(folderName);
        string variableSuffix = isGlobal ? "all" : "only";
        IgnorMode mode = isGlobal ? IgnorMode.GlobalName : IgnorMode.ExactPath;

        return new IgnorRule
        {
            RealPath = cleanedPath,
            VirtualVariable = $"user.{safeVariableName}.{variableSuffix}",
            Mode = mode
        };
    }

    private string NormalizeVariableName(string folderName)
    {
        string lower = folderName.ToLower();
        string noSpaces = lower.Replace(" ", "_");
        string clean = Regex.Replace(noSpaces, @"[^\p{L}\p{N}_]", "");
        return Regex.Replace(clean, @"_+", "_").Trim('_');
    }
}