﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynSecurityGuard.Analyzers;
using TestHelper;

namespace RoslynSecurityGuard.Tests
{
    [TestClass]
    public class WeakHashingAnalyzerTest : DiagnosticVerifier
    {

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzers()
        {
            return new WeakHashingAnalyzer();
        }

        [TestMethod]
        public void WeakHashingFalsePositive()
        {
            var test = @"
using System;
using System.Text;
using System.Security.Cryptography;

class Sha256OK
{
    static String generateSecureHashing()
    {
        string source = ""Hello World!"";
        SHA256 sha256 = SHA256.Create();
        byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));

        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString(""x2""));
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WeakHashingVulnerableMd5()
        {
            var test = @"
using System;
using System.Text;
using System.Security.Cryptography;

class WeakHashing
{

    static String generateWeakHashingMD5()
    {
        string source = ""Hello World!"";
        MD5 md5 = MD5.Create();
        byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(source));

        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString(""x2""));
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "SG0006",
                Severity = DiagnosticSeverity.Warning
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WeakHashingVulnerableSha1()
        {
            var test = @"
using System;
using System.Text;
using System.Security.Cryptography;

class WeakHashing
{

    static String generateWeakHashingSHA1()
    {
        string source = ""Hello World!"";
        SHA1 sha1 = SHA1.Create();
        byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));

        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString(""x2""));
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "SG0006",
                Severity = DiagnosticSeverity.Warning
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}
