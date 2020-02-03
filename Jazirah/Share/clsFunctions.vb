Public Class Functions

#Region "Numers To Letters Function"
    Public Function ToArabicLetter(ByVal givenNumber As Double) As String
        Dim FinalOutput, Number, NumberCurrency, Fractions, FractionsCurrency As String
        Dim Tafkeet = " فقط لا غير"

        Dim WholeNumber() As String = Split(givenNumber, ".")

        NumberCurrency = NumberAsCurrency(WholeNumber(0))
        FinalOutput = NumberCurrency

        If WholeNumber.Length >= 2 Then
            If WholeNumber(1).Length.Equals(1) Then
                WholeNumber(1) = WholeNumber(1) + "0"
            ElseIf WholeNumber(1).Length > 2 Then
                WholeNumber(1) = WholeNumber(1).Substring(0, 2)
            End If

            FractionsCurrency = FractionAsCurrency(WholeNumber(1))
            FinalOutput = FinalOutput + " و" + FractionsCurrency
        End If

        If FinalOutput <> Nothing And FinalOutput <> "" Then
            FinalOutput = FinalOutput + Tafkeet
        End If

        ToArabicLetter = FinalOutput
    End Function

    Private Function SFormatNumber(ByVal X As Double) As String

        Dim Letter1, Letter2, Letter3, Letter4, Letter5, Letter6 As String
        Dim c As String = Format(Math.Floor(X), "000000000000")
        Dim C1 As Double = Val(Mid(c, 12, 1))
        Select Case C1
            Case Is = 1 : Letter1 = "واحد"
            Case Is = 2 : Letter1 = "اثنان"
            Case Is = 3 : Letter1 = "ثلاثة"
            Case Is = 4 : Letter1 = "اربعة"
            Case Is = 5 : Letter1 = "خمسة"
            Case Is = 6 : Letter1 = "ستة"
            Case Is = 7 : Letter1 = "سبعة"
            Case Is = 8 : Letter1 = "ثمانية"
            Case Is = 9 : Letter1 = "تسعة"
        End Select


        Dim C2 As Double = Val(Mid(c, 11, 1))
        Select Case C2
            Case Is = 1 : Letter2 = "عشر"
            Case Is = 2 : Letter2 = "عشرون"
            Case Is = 3 : Letter2 = "ثلاثون"
            Case Is = 4 : Letter2 = "اربعون"
            Case Is = 5 : Letter2 = "خمسون"
            Case Is = 6 : Letter2 = "ستون"
            Case Is = 7 : Letter2 = "سبعون"
            Case Is = 8 : Letter2 = "ثمانون"
            Case Is = 9 : Letter2 = "تسعون"
        End Select


        If Letter1 <> "" And C2 > 1 Then Letter2 = Letter1 + " و" + Letter2
        If Letter2 = "" Or Letter2 Is Nothing Then
            Letter2 = Letter1
        End If
        If C1 = 0 And C2 = 1 Then Letter2 = Letter2 + "ة"
        If C1 = 1 And C2 = 1 Then Letter2 = "احدى عشر"
        If C1 = 2 And C2 = 1 Then Letter2 = "اثنى عشر"
        If C1 > 2 And C2 = 1 Then Letter2 = Letter1 + " " + Letter2
        Dim C3 As Double = Val(Mid(c, 10, 1))
        Select Case C3
            Case Is = 1 : Letter3 = "مائة"
            Case Is = 2 : Letter3 = "مائتان"
            Case Is > 2 : Letter3 = Microsoft.VisualBasic.Left(SFormatNumber(C3), Len(SFormatNumber(C3)) - 1) + "مائة"
        End Select
        If Letter3 <> "" And Letter2 <> "" Then Letter3 = Letter3 + " و" + Letter2
        If Letter3 = "" Then Letter3 = Letter2


        Dim C4 As Double = Val(Mid(c, 7, 3))
        Select Case C4
            Case Is = 1 : Letter4 = "الف"
            Case Is = 2 : Letter4 = "الفان"
            Case 3 To 10 : Letter4 = SFormatNumber(C4) + " آلاف"
            Case Is > 10 : Letter4 = SFormatNumber(C4) + " الف"
        End Select
        If Letter4 <> "" And Letter3 <> "" Then Letter4 = Letter4 + " و" + Letter3
        If Letter4 = "" Then Letter4 = Letter3
        Dim C5 As Double = Val(Mid(c, 4, 3))
        Select Case C5
            Case Is = 1 : Letter5 = "مليون"
            Case Is = 2 : Letter5 = "مليونان"
            Case 3 To 10 : Letter5 = SFormatNumber(C5) + " ملايين"
            Case Is > 10 : Letter5 = SFormatNumber(C5) + " مليون"
        End Select
        If Letter5 <> "" And Letter4 <> "" Then Letter5 = Letter5 + " و" + Letter4
        If Letter5 = "" Then Letter5 = Letter4


        Dim C6 As Double = Val(Mid(c, 1, 3))
        Select Case C6
            Case Is = 1 : Letter6 = "مليار"
            Case Is = 2 : Letter6 = "ملياران"
            Case Is > 2 : Letter6 = SFormatNumber(C6) + " مليار"
        End Select
        If Letter6 <> "" And Letter5 <> "" Then Letter6 = Letter6 + " و" + Letter5
        If Letter6 = "" Then Letter6 = Letter5

        SFormatNumber = Letter6

    End Function

    Private Function NumberAsCurrency(ByVal givenNumber As Double) As String
        Dim Number, Currency As String

        Number = SFormatNumber(givenNumber)

        If Number <> "" And Number <> Nothing And givenNumber <= 2 Then
            If Number.StartsWith("واحد") Then
                Number = Number.Substring(4)
            ElseIf Number.StartsWith("اثنان") Then
                Number = Number.Substring(5)
            End If
        End If

        Select Case CDbl(givenNumber)
            Case Is = Nothing : Currency = ""
            Case Is = 2 : Currency = " ريالان "
            Case 3 To 10 : Currency = " ريالات "
            Case Else : Currency = " ريال "
        End Select

        NumberAsCurrency = Number + " " + Currency

    End Function

    Private Function FractionAsCurrency(ByVal givenNumber As Double) As String
        Dim Fractions, Currency As String

        Fractions = SFormatNumber(givenNumber)

        If Fractions <> "" And Fractions <> Nothing And givenNumber <= 2 Then
            If Fractions.StartsWith("واحد") Then
                Fractions = Fractions.Substring(4)
            ElseIf Fractions.StartsWith("اثنان") Then
                Fractions = Fractions.Substring(5)
            End If
        End If

        Select Case CDbl(givenNumber)
            Case Is = Nothing : Currency = ""
            Case Is = 2 : Currency = " هللتان"
            Case 3 To 10 : Currency = " هللات"
            Case Else : Currency = " هلله"
        End Select

        FractionAsCurrency = Fractions + " " + Currency

    End Function
#End Region
End Class
