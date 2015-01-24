﻿Imports CodeCracker.Design
Imports CodeCracker.Test.TestHelper
Imports Xunit

Namespace Design
    Public Class NameOfTests
        Inherits CodeFixTest(Of NameOfAnalyzer, NameOfCodeFixProvider)

        <Fact>
        Public Async Function IgnoreIfStringLiteralIsWhitespace() As Task
            Dim test = "
Public Class TypeName
    Sub Foo()
        Dim whatever = """"
    End Sub
End Class"

            Await VerifyBasicHasNoDiagnosticsAsync(test)
        End Function

        <Fact>
        Public Async Function IgnoreIfStringLiteralIsNothing() As Task
            Dim test = "
Public Class TypeName
    Sub Foo()
        Dim whatever = Nothing
    End Sub
End Class"

            Await VerifyBasicHasNoDiagnosticsAsync(test)
        End Function

        <Fact>
        Public Async Function IgnoreIfConstructorHasNoParameters() As Task
            Dim test = "
Public Class TypeName
    Public Sub New()
        dim whatever = ""b""
    End Sub
End Class"

            Await VerifyBasicHasNoDiagnosticsAsync(test)
        End Function

        <Fact>
        Public Async Function IgnoreIfMethodHasNoParameters() As Task
            Dim test = "
Public Class TypeName
    Sub Foo()
        Dim whatever = """"
    End Sub
End Class"

            Await VerifyBasicHasNoDiagnosticsAsync(test)
        End Function

        <Fact>
        Public Async Function IgnoreIfMethodHasParametersUnlineOfStringLiteral() As Task
            Dim test = "
Public Class TypeName
    Sub Foo(a As String)
        Dim whatever = ""b""
    End Sub
End Class"

            Await VerifyBasicHasNoDiagnosticsAsync(test)
        End Function

        <Fact>
        Public Async Function WhenUsingStringLiteralEqualsParameterNameReturnAnalyzerCreatesDiagnostic() As Task
            Dim test = "
Public Class TypeName
    Sub Foo(b As String)
        Dim whatever = ""b""
    End Sub
End Class"

            Dim expected = New DiagnosticResult With {
                .Id = NameOfAnalyzer.DiagnosticId,
                .Message = "Use 'NameOf(b)' instead of specifying the parameter name.",
                .Severity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                .Locations = {New DiagnosticResultLocation("Test0.vb", 4, 24)}
            }

            Await VerifyDiagnosticsAsync(test, expected)
        End Function

        <Fact>
        Public Async Function WhenUsingStringLiteralEqualsParameterNameInConstructorFixItToNameof() As Task
            Dim test = "
Public Class TypeName
    Sub New(b As String)
        Dim whatever = ""b""
    End Sub
End Class"

            Dim fixtest = "
Public Class TypeName
    Sub New(b As String)
        Dim whatever = nameof(b)
    End Sub
End Class"

            Await VerifyBasicFixAsync(test, fixtest, 0)
        End Function

        <Fact>
        Public Async Function WhenUsingStringLiteralEqualsParameterNameInConstructorFixItToNameofMustKeepComments() As Task
            Dim test = "
Public Class TypeName
    Sub New(b As String)
        'a
        Dim whatever = ""b"" 'd
        'b
    End Sub
End Class"

            Dim fixtest = "
Public Class TypeName
    Sub New(b As String)
        'a
        Dim whatever = nameof(b) 'd
        'b
    End Sub
End Class"

            Await VerifyBasicFixAsync(test, fixtest, 0)
        End Function

        <Fact>
        Public Async Function WhenUsingStringLiteralEqualsParameterNameInMethodFixItToNameof() As Task
            Dim test = "
Public Class TypeName
    Sub Foo(b As String)
        Dim whatever = ""b""
    End Sub
End Class"

            Dim fixtest = "
Public Class TypeName
    Sub Foo(b As String)
        Dim whatever = nameof(b)
    End Sub
End Class"

            Await VerifyBasicFixAsync(test, fixtest, 0)
        End Function

        <Fact>
        Public Async Function WhenUsingStringLiteralEqualsParameterNameInMethodMustKeepComments() As Task
            Dim test = "
Public Class TypeName
    Sub Foo(b As String)
        'a
        Dim whatever = ""b""'d
        'b
    End Sub
End Class"

            Dim fixtest = "
Public Class TypeName
    Sub Foo(b As String)
        'a
        Dim whatever = nameof(b) 'd
        'b
    End Sub
End Class"

            Await VerifyBasicFixAsync(test, fixtest, 0)
        End Function
    End Class
End Namespace