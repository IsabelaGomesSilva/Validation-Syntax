using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using ValidationSyntax;

public class SyntaxValidator
{
    public static void ValidateRuleSyntax(string ruleSyntax)
    {
        string code = @"
        using System;

        public class Rule
        {
          public Guid ProjectId { get; set; }
          public string Name { get; set; }

          public bool Evaluate()
          {
            return (/*syntax*/);
          }
        }
        ";

        code = code.Replace("/*syntax*/", ruleSyntax);

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create("DynamicRule", syntaxTrees: new[] { syntaxTree }, references: references,
        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var diagnostics = compilation.GetDiagnostics();

        if (diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var diagnostic in diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                sb.AppendLine(diagnostic.ToString());
            }
            throw new Exceptions.TimesheetDomainException($"The rule syntax is invalid: {sb}");
        }

        Console.WriteLine("The rule syntax is valid.");
    }   
}

namespace Exceptions
{
    public class TimesheetDomainException : Exception
    {
        public TimesheetDomainException(string message) : base(message) { }
    }
}

public class Program
{
    public static void Main()
    {
        teste teste= new teste();
        try
        {
            string validSyntax = "ProjectId == Guid.NewGuid()";
            string validSyntax1 = "x < y || z == w"; 
            string invalidSyntax = "a == b c > d"; 
            string invalidSyntax1 = "a == b";

            SyntaxValidator.ValidateRuleSyntax(validSyntax);
            SyntaxValidator.ValidateRuleSyntax(validSyntax1);
            SyntaxValidator.ValidateRuleSyntax(invalidSyntax);
            SyntaxValidator.ValidateRuleSyntax(invalidSyntax1);
        }
        catch (Exceptions.TimesheetDomainException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
