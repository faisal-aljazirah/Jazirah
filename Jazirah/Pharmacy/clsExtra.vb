Imports System.Text
Imports System.Web
Imports System.Xml

Public Class Extra
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Dim byteLocalCurrency As Byte
    Dim intStartupFY As Integer
    Dim intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Dim byteCurrencyRound As Byte
    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, AllowPrintEmptyDose As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Sub New()
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            Throw New Exception("Login Error")
        End If
        If HttpContext.Current.Session("UserLanguage") Is Nothing Then
            HttpContext.Current.Session("UserLanguage") = 1
        End If
        If HttpContext.Current.Session("UserDTFormat") Is Nothing Then
            HttpContext.Current.Session("UserDTFormat") = "yyyy-MM-dd"
        End If
        If HttpContext.Current.Session("UserTMFormat") Is Nothing Then
            HttpContext.Current.Session("UserTMFormat") = "HH:mm"
        End If

        strUserName = HttpContext.Current.Session("UserName")
        ByteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")

        loadSettings()
    End Sub

    Private Sub loadSettings()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/settings.xml"))
        'get count
        Dim items As String = ""
        Dim application As XmlNode = doc.SelectSingleNode("Settings/Pharmacy")
        intYear = application.SelectSingleNode("intYear").InnerText
        intStartupFY = application.SelectSingleNode("intStartupFY").InnerText
        byteLocalCurrency = application.SelectSingleNode("byteLocalCurrency").InnerText
        byteCurrencyRound = application.SelectSingleNode("byteCurrencyRound").InnerText

        If application.SelectSingleNode("byteDepartment_Cash") Is Nothing Then byteDepartment_Cash = "" Else byteDepartment_Cash = application.SelectSingleNode("byteDepartment_Cash").InnerText
        lngContact_Cash = application.SelectSingleNode("lngContact_Cash").InnerText
        lngSalesman_Cash = application.SelectSingleNode("lngSalesman_Cash").InnerText
        lngPatient_Cash = application.SelectSingleNode("lngPatient_Cash").InnerText

        ChangeQuantity_Cash = application.SelectSingleNode("ChangeQuantity_Cash").InnerText
        ChangeQuantity_Insurance = application.SelectSingleNode("ChangeQuantity_Insurance").InnerText
        AddDiscount_Cash = application.SelectSingleNode("AddDiscount_Cash").InnerText
        AddDiscount_Insurance = application.SelectSingleNode("AddDiscount_Insurance").InnerText
        If application.SelectSingleNode("OneQuantityPerItem") Is Nothing Then OneQuantityPerItem = True Else OneQuantityPerItem = application.SelectSingleNode("OneQuantityPerItem").InnerText
        AllowExtraItem_Insurance = application.SelectSingleNode("AllowExtraItem_Insurance").InnerText
        AutoMoveRejectedToCash_Insurance = application.SelectSingleNode("Auto_MoveRejectedToCash_Insurance").InnerText
        AskBeforeSend = application.SelectSingleNode("AskBeforeSend").InnerText
        AskBeforeReturn = application.SelectSingleNode("AskBeforeReturn").InnerText
        OnePaymentForCashier = application.SelectSingleNode("OnePaymentForCashier").InnerText
        ForcePaymentOnCloseInvoice = application.SelectSingleNode("ForcePaymentOnCloseInvoice").InnerText
        'DirectCancelInvoic = application.SelectSingleNode("DirectCancelInvoic").InnerText
        SusbendMax = application.SelectSingleNode("SusbendMax").InnerText

        'byteInvoicesLimitDay = application.SelectSingleNode("byteInvoicesLimitDay").InnerText
        OrdersLimitDays = application.SelectSingleNode("OrdersLimitDays").InnerText
        CancelLimitDays = application.SelectSingleNode("CancelLimitDays").InnerText
        DaysToCalculateMedicalInvoices = application.SelectSingleNode("DaysToCalculateMedicalInvoices").InnerText
        DaysToCalculateMedicineInvoices = application.SelectSingleNode("DaysToCalculateMedicineInvoices").InnerText

        If application.SelectSingleNode("PopupToPrint") Is Nothing Then PopupToPrint = True Else PopupToPrint = application.SelectSingleNode("PopupToPrint").InnerText
        If application.SelectSingleNode("PrintDose") Is Nothing Then PrintDose = 3 Else PrintDose = application.SelectSingleNode("PrintDose").InnerText
        If application.SelectSingleNode("PrintInvoice") Is Nothing Then PrintInvoice = 2 Else PrintInvoice = application.SelectSingleNode("PrintInvoice").InnerText
        If application.SelectSingleNode("DosePrinter") Is Nothing Then DosePrinter = "ZDesigner GK420t" Else DosePrinter = application.SelectSingleNode("DosePrinter").InnerText
        If application.SelectSingleNode("InvoicePrinter") Is Nothing Then InvoicePrinter = "HP LaserJet Professional P1566" Else InvoicePrinter = application.SelectSingleNode("InvoicePrinter").InnerText
        If application.SelectSingleNode("TaxEnabled") Is Nothing Then TaxEnabled = True Else TaxEnabled = application.SelectSingleNode("TaxEnabled").InnerText
        If application.SelectSingleNode("AllowPrintEmptyDose") Is Nothing Then AllowPrintEmptyDose = True Else AllowPrintEmptyDose = application.SelectSingleNode("AllowPrintEmptyDose").InnerText

        If ByteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""
    End Sub

#Region "Extra Help"
    Public Function viewInfo(ByVal lngTransaction As Long) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim btnClose As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"

                btnClose = "إغلاق"
            Case Else
                DataLang = "En"

                btnClose = "Close"
        End Select

        Dim Content As New StringBuilder("")
        Content.Append("<div class=""font-small-3 height-400 overflow-auto"">")
        If lngTransaction > 0 Then
            ds = dcl.GetDS("SELECT DO.lngContact AS DoctorNo, STI.dateTransaction AS CloseDate, DO.strContact" & DataLang & " AS DoctorName, DO.intSpeciality AS intSpeciality,CO.lngContact AS CompanyNo, CO.strContact" & DataLang & " AS CompanyName,CO.bytePriceType AS bytePriceType,* FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS HP ON ST.lngPatient=HP.lngPatient INNER JOIN Hw_Contacts AS CO ON ST.lngContact=CO.lngContact INNER JOIN Hw_Contacts AS DO ON ST.lngSalesman=DO.lngContact WHERE ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim strReference As String = ds.Tables(0).Rows(0).Item("strReference").ToString
                Dim lngPatient As Long = ds.Tables(0).Rows(0).Item("lngPatient")
                Dim lngSalesman As Long = ds.Tables(0).Rows(0).Item("lngSalesman")
                Dim dateTransaction As Date = ds.Tables(0).Rows(0).Item("dateTransaction")
                Dim CloseDate As Date
                If IsDBNull(ds.Tables(0).Rows(0).Item("CloseDate")) Then CloseDate = Today Else CloseDate = ds.Tables(0).Rows(0).Item("CloseDate")
                Dim intVisit As Integer = 0
                Dim byteWarehouse As Byte = 3
                Dim lstItems As String = ""

                Content.Append("<h5><b>Stock_Trans:</b></h5>")
                Content.Append("<table class=""full-width border"">")
                Content.Append("<tr><td>lngTransaction:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("lngTransaction") & "</b></td><td>byteBase:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteBase") & "</td><td>byteTransType:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteTransType") & "</td></tr>")
                Content.Append("<tr><td>dateTransaction:</td><td class=""blue-grey""><b>" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "</b></td><td>byteStatus:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteStatus") & "</td><td>strReference:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("strReference") & "</b></td></tr>")
                Content.Append("<tr><td>bCollected1:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bCollected1") & "</td><td>bApproved1:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bApproved1") & "</td><td>bClinicSent:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bClinicSent") & "</td></tr>")
                Content.Append("</table>")
                Content.Append("<h5><b>Hw_Patients:</b></h5>")
                Content.Append("<table class=""full-width border"">")
                Content.Append("<tr><td>lngPatient:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("lngPatient") & "</b></td><td>strFirst" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("strFirst" & DataLang) & "</td><td>lngGuarantor:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("lngGuarantor") & "</td></tr>")
                Content.Append("</table>")
                Content.Append("<h5><b>Hw_Contacts: (Doctor)</b></h5>")
                Content.Append("<table class=""full-width border"">")
                Content.Append("<tr><td>lngContact:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("DoctorNo") & "</b></td><td>strContact" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("DoctorName") & "</td><td>intSpeciality:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("intSpeciality") & "</td></tr>")
                Content.Append("</table>")
                Content.Append("<h5><b>Hw_Contacts: (Company)</b></h5>")
                Content.Append("<table class=""full-width border"">")
                Content.Append("<tr><td>lngContact:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("CompanyNo") & "</b></td><td>strContact" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("CompanyName") & "</td><td>bytePriceType:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bytePriceType") & "</td></tr>")
                Content.Append("</table>")

                Content.Append("<h5><b>Stock_Trans_Audit:</b></h5>")
                Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction)
                If dsAudit.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>strCreatedBy:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("strCreatedBy").ToString & "</td><td>dateCreated:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("dateCreated").ToString & "</td></tr>")
                    Content.Append("<tr><td>strLastSavedBy:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("strLastSavedBy").ToString & "</td><td>dateLastSaved:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("dateLastSaved").ToString & "</td></tr>")
                    Content.Append("<tr><td>strApprovedBy:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("strApprovedBy").ToString & "</td><td>dateApproved:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("dateApproved").ToString & "</td></tr>")
                    Content.Append("<tr><td>strCashBy:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("strCashBy").ToString & "</td><td>dateCash:</td><td class=""blue-grey"">" & dsAudit.Tables(0).Rows(0).Item("dateCash").ToString & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Hw_Treatments_Pharmacy:</b></h5>")
                Dim dsTreatments As DataSet = dcl.GetDS("SELECT SI.strItem AS strItem, SI.strItemEn AS strItemEn, * FROM Hw_Treatments_Pharmacy AS HTP INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem WHERE strReference='" & strReference & "' AND lngPatient=" & lngPatient & " AND dateTransaction='" & dateTransaction.ToString("yyyy-MM-dd") & "'")
                If dsTreatments.Tables(0).Rows.Count > 0 Then
                    intVisit = dsTreatments.Tables(0).Rows(0).Item("intVisit")
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><th>intVisit</th><th>strItem</th><th>strItem" & DataLang & "</th><th>curQuantity</th><th>bApproval</th></tr>")
                    For I = 0 To dsTreatments.Tables(0).Rows.Count - 1
                        Content.Append("<tr><td class=""blue-grey"">" & dsTreatments.Tables(0).Rows(I).Item("intVisit").ToString & "</td><td class=""blue-grey"">" & dsTreatments.Tables(0).Rows(I).Item("strItem").ToString & "</td><td class=""blue-grey"">" & dsTreatments.Tables(0).Rows(I).Item("strItem" & DataLang).ToString & "</td><td class=""blue-grey"">" & dsTreatments.Tables(0).Rows(I).Item("curQuantity").ToString & "</td><td class=""blue-grey"">" & dsTreatments.Tables(0).Rows(I).Item("bApproval").ToString & "</td></tr>")
                        lstItems = lstItems & "'" & dsTreatments.Tables(0).Rows(I).Item("strItem").ToString & "',"
                    Next
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Hw_Medicines_Approval:</b></h5>")
                Dim dsApproval As DataSet = dcl.GetDS("SELECT * FROM Hw_Medicines_Approval WHERE intVisit=" & intVisit & " AND lngPatient=" & lngPatient)
                If dsApproval.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><th>intVisit</th><th>strItem</th><th>byteCheck</th><th>bApproval</th><th>strApprovalNo</th><th>strApprovedBy</th><th>bRejected</th><th>strRejectedBy</th></tr>")
                    For I = 0 To dsApproval.Tables(0).Rows.Count - 1
                        Content.Append("<tr><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("intVisit").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("strItem").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("byteCheck").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("bApproval").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("strApprovalNo").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("strApprovedBy").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("bRejected").ToString & "</td><td class=""blue-grey"">" & dsApproval.Tables(0).Rows(I).Item("strRejectedBy").ToString & "</td></tr>")
                    Next
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Stock_Trans_Insurance:</b></h5>")
                Dim dsInsurance As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Insurance WHERE lngTransaction=" & lngTransaction)
                If dsInsurance.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>bPercentValue:</td><td class=""blue-grey"">" & dsInsurance.Tables(0).Rows(0).Item("bPercentValue").ToString & "</td><td>curCoverage:</td><td class=""blue-grey"">" & dsInsurance.Tables(0).Rows(0).Item("curCoverage").ToString & "</td><td>lngContract:</td><td class=""blue-grey"">" & dsInsurance.Tables(0).Rows(0).Item("lngContract").ToString & "</td><td>byteScheme:</td><td class=""blue-grey"">" & dsInsurance.Tables(0).Rows(0).Item("byteScheme").ToString & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Ins_Coverage:</b></h5>")
                Dim dsCoverage As DataSet = dcl.GetDS("SELECT * FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & lngPatient)
                If dsCoverage.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>strInsuranceNo:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("strInsuranceNo").ToString & "</td><td>bytePrimaryDep:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("bytePrimaryDep").ToString & "</td></tr>")
                    Content.Append("<tr><td>lngContract:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("lngContract").ToString & "</td><td>byteScheme:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("byteScheme").ToString & "</td></tr>")
                    Content.Append("<tr><td>curDeductionValueP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionValueP").ToString & "</td><td>curDeductionPercentD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentD").ToString & "</td></tr>")
                    Content.Append("<tr><td>curDeductionPercentP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentP").ToString & "</td><td>curDeductionValueD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionValueD").ToString & "</td></tr>")
                    Content.Append("<tr><td>curDeductionMaxP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxP").ToString & "</td><td>curDeductionMaxD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxD").ToString & "</td></tr>")
                    Content.Append("<tr><td>curYearLimitP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curYearLimitP").ToString & "</td><td>curYearLimitD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curYearLimitD").ToString & "</td></tr>")
                    Content.Append("<tr><td>curMonthlyLimitP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curMonthlyLimitP").ToString & "</td><td>curMonthlyLimitD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curMonthlyLimitD").ToString & "</td></tr>")
                    Content.Append("<tr><td>curCaseLimitP:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curCaseLimitP").ToString & "</td><td>curCaseLimitD:</td><td class=""blue-grey"">" & dsCoverage.Tables(0).Rows(0).Item("curCaseLimitD").ToString & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Clinic_Invoices:</b> Total</h5>")
                Dim dsClinic As DataSet = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' And '" & CloseDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                If dsClinic.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>lngSalesman:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("lngSalesman").ToString & "</td><td>Amount:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("SumOfAmount").ToString & "</td><td>Coverage:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("Coverage").ToString & "</td><td>Days:</td><td class=""blue-grey"">" & DaysToCalculateMedicalInvoices & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Stock_Trans:</b> Total</h5>")
                Dim dsTrans As DataSet = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' AND '" & CloseDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                If dsTrans.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>Amount:</td><td class=""blue-grey"">" & dsTrans.Tables(0).Rows(0).Item("Amount").ToString & "</td><td>Coverage:</td><td class=""blue-grey"">" & dsTrans.Tables(0).Rows(0).Item("Cov").ToString & "</td><td>Days:</td><td class=""blue-grey"">" & DaysToCalculateMedicineInvoices & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Stock (Balance):</b></h5>")
                Dim dsBalance As DataSet = dcl.GetDS("SELECT SXI.strItem, SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem IN (" & lstItems & "'') AND ST.dateTransaction <= '" & dateTransaction.ToString("yyyy-MM-dd") & "' GROUP BY SXI.strItem")
                If dsBalance.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><th>strItem</th><th>Balance</th></tr>")
                    For I = 0 To dsBalance.Tables(0).Rows.Count - 1
                        Content.Append("<tr><td class=""blue-grey"">" & dsBalance.Tables(0).Rows(I).Item("strItem").ToString & "</td><td class=""blue-grey"">" & dsBalance.Tables(0).Rows(I).Item("curBalance").ToString & "</td></tr>")
                    Next
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If
            Else
                Content.Append("Transaction No:" & lngTransaction & " not available!")
            End If
        Else
            Content.Append("Transaction No is wrong!")
        End If
        Content.Append("</div>")

        Dim mdl As New Share.UI
        Return mdl.drawModal("More Details", Content.ToString, "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "bg-grey bg-lighten-2")
    End Function

    Public Function viewCoverage(ByVal lngTransaction As Long) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim btnClose As String
        Dim Header As String
        Dim divCoverage, divClinicInvoices, divPharmacyInvoices As String
        Dim lblDeductionPercent, lblCaseLimit, lblMaxDeductionValue, lblDeductionValue, lblCompany, lblClass, lblHolder, lblContract, lblPolicy As String
        Dim lblCoverage, lblAmount, lblFor, lblDays As String
        Dim msgNoCoverage, msgNoInvoices As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "معلومات التغطية"
                divCoverage = "معلومات تغطية التأمين"
                divClinicInvoices = "فواتير العيادة"
                divPharmacyInvoices = "فواتير الصيدلية"
                lblDeductionValue = "قيمة الاقتطاع"
                lblDeductionPercent = "نسبة الاقتطاع"
                lblCaseLimit = "الحد الأعلى للحالة"
                lblMaxDeductionValue = "الحد الأعلى للاقتطاع"
                lblCoverage = "التغطية"
                lblAmount = "المجموع"
                lblFor = "لمدة"
                lblDays = "أيام"
                lblCompany = "الشركة"
                lblClass = "الفئة"
                lblHolder = "الفرع"
                lblContract = "رقم العقد"
                lblPolicy = "رقم البوليصة"
                msgNoCoverage = "لا يوجد تغطية"
                msgNoInvoices = "لا توجد فواتير"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Coverage Information"
                divCoverage = "Insurance Coverage Informaiton"
                divClinicInvoices = "Clinic Invoices"
                divPharmacyInvoices = "Phrmacy Invoices"
                lblDeductionValue = "Deduction Value"
                lblDeductionPercent = "Deduction Percent"
                lblCaseLimit = "Case Limit"
                lblMaxDeductionValue = "Max Deduction Value"
                lblCoverage = "Coverage"
                lblAmount = "Total"
                lblFor = "For"
                lblDays = "Days"
                lblCompany = "Company"
                lblClass = "Class"
                lblHolder = "Holder"
                lblContract = "Contract No"
                lblPolicy = "Policy No"
                msgNoCoverage = "No Coverage"
                msgNoInvoices = "No Invoices"
                btnClose = "Close"
        End Select

        Dim Content As New StringBuilder("")
        Content.Append("<div>")
        If lngTransaction > 0 Then
            ds = dcl.GetDS("SELECT DO.lngContact AS DoctorNo, DO.strContact" & DataLang & " AS DoctorName, STI.dateTransaction AS CloseDate, DO.intSpeciality AS intSpeciality,CO.lngContact AS CompanyNo, CO.strContact" & DataLang & " AS CompanyName,CO.bytePriceType AS bytePriceType,* FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS HP ON ST.lngPatient=HP.lngPatient INNER JOIN Hw_Contacts AS CO ON ST.lngContact=CO.lngContact INNER JOIN Hw_Contacts AS DO ON ST.lngSalesman=DO.lngContact WHERE ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim strReference As String = ds.Tables(0).Rows(0).Item("strReference").ToString
                Dim lngPatient As Long = ds.Tables(0).Rows(0).Item("lngPatient")
                Dim lngSalesman As Long = ds.Tables(0).Rows(0).Item("lngSalesman")
                Dim dateTransaction As Date = ds.Tables(0).Rows(0).Item("dateTransaction")
                Dim CloseDate As Date
                If IsDBNull(ds.Tables(0).Rows(0).Item("CloseDate")) Then CloseDate = Today Else CloseDate = ds.Tables(0).Rows(0).Item("CloseDate")
                Dim intVisit As Integer = 0
                Dim byteWarehouse As Byte = 3
                Dim lstItems As String = ""
                Dim Value As Integer

                Content.Append("<h5 class=""text-md-center""><b>" & divCoverage & "</b></h5>")
                Content.Append("<div class=""full-width"">")
                Dim dsCoverage As DataSet = dcl.GetDS("SELECT * FROM Hw_Patients AS HP INNER JOIN Ins_Contracts AS IT ON HP.lngContract=IT.lngContract INNER JOIN Hw_Contacts AS HC ON IT.lngGuarantor=HC.lngContact INNER JOIN Ins_Schemes AS ISC ON HP.lngContract=ISC.lngContract AND HP.byteScheme=ISC.byteScheme INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & lngPatient)
                If dsCoverage.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width"">")
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblCompany & ":</td><td class=""text-bold-700 brown lighten-2"">" & dsCoverage.Tables(0).Rows(0).Item("strContact" & DataLang).ToString & "</td></tr>")
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblHolder & ":</td><td class=""text-bold-700 brown lighten-2"">" & dsCoverage.Tables(0).Rows(0).Item("strHolder" & DataLang).ToString & "</td></tr>")
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblClass & ":</td><td class=""text-bold-700 brown lighten-2"">" & dsCoverage.Tables(0).Rows(0).Item("strScheme").ToString & "</td></tr>")
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblContract & ":</td><td class=""text-bold-700 indigo lighten-2"">" & dsCoverage.Tables(0).Rows(0).Item("lngContract").ToString & "</td></tr>")
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblPolicy & ":</td><td class=""text-bold-700 indigo lighten-2"">" & dsCoverage.Tables(0).Rows(0).Item("strPolicyNo").ToString & "</td></tr>")
                    Content.Append("<tr><td> </td></tr>")
                    If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionValueP"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionValueP") Else If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionValueD"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionValueD") Else Value = 0
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblDeductionValue & ":</td><td class=""text-bold-700 red lighten-2"">" & Math.Round(Value, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                    If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentP"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentP") Else If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentD"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionPercentD") Else Value = 0
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblDeductionPercent & ":</td><td class=""text-bold-700 red lighten-2"">" & Math.Round(Value, byteCurrencyRound, MidpointRounding.AwayFromZero) & "%</td></tr>")
                    If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxP"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxP") Else If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxD"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curDeductionMaxD") Else Value = 0
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1"">" & lblMaxDeductionValue & ":</td><td class=""text-bold-700 red lighten-2"">" & Math.Round(Value, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                    If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curCaseLimitP"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curCaseLimitP") Else If Not (IsDBNull(dsCoverage.Tables(0).Rows(0).Item("curCaseLimitD"))) Then Value = dsCoverage.Tables(0).Rows(0).Item("curCaseLimitD") Else Value = 0
                    Content.Append("<tr><td class=""text-md-right width-40-per pr-1""> " & lblCaseLimit & ":</td><td class=""text-bold-700 red lighten-2"">" & Math.Round(Value, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append(msgNoCoverage)
                End If
                Content.Append("</div>")
                Content.Append("<hr />")

                Content.Append("<h5 class=""text-md-center""><b>" & divClinicInvoices & "</b> [<span class="" text-bold-700 indigo lighten-2"">" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), CloseDate).ToString(strDateFormat) & "</span>] To [<span class="" text-bold-700 indigo lighten-2"">" & CloseDate.ToString(strDateFormat) & "</span>]  " & lblFor & " <span class="" text-bold-700 indigo lighten-2"">" & DaysToCalculateMedicalInvoices & "</span> " & lblDays & "</h5>")
                Content.Append("<div class=""full-width text-md-center"">")
                '''''''''old query (using dateTransaction)''''''Dim dsClinic As DataSet = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateClosedValid).ToString("yyyy-MM-dd") & "' And '" & dateClosedValid.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                'Dim dsClinic As DataSet = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' And '" & CloseDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                'If dsClinic.Tables(0).Rows.Count > 0 Then
                '    Dim Coverage As Decimal
                '    If IsDBNull(dsClinic.Tables(0).Rows(0).Item("Coverage")) Then Coverage = 0 Else Coverage = dsClinic.Tables(0).Rows(0).Item("Coverage")
                '    Content.Append("<table class=""full-width"">")
                '    Content.Append("<tr><td class=""text-md-right"">" & lblAmount & "</td><td class=""text-md-center text-bold-700"">" & Math.Round(CDec("0" & dsClinic.Tables(0).Rows(0).Item("SumOfAmount").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td class=""text-md-right"">" & lblCoverage & "</td><td class=""text-md-center text-bold-700 red lighten-2"">" & Math.Round(Coverage, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                '    Content.Append("</table>")
                'Else
                '    Content.Append(msgNoInvoices)
                'End If
                Dim Amount, Coverage, TotalCoverage As Decimal
                Dim Payment As String
                Amount = 0
                Coverage = 0
                TotalCoverage = 0
                Dim dsClinic As DataSet = dcl.GetDS("SELECT * FROM Clinic_Invoices AS CI INNER JOIN Hw_Contacts AS C ON CI.lngSalesman=C.lngContact WHERE byteBase=41 AND byteStatus>0 AND lngPatient=" & lngPatient & " AND lngSalesman=" & lngSalesman & " AND dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' AND '" & CloseDate.ToString("yyyy-MM-dd") & "'")
                Content.Append("<table class=""full-width border"">")
                For I = 0 To dsClinic.Tables(0).Rows.Count - 1
                    If IsDBNull(dsClinic.Tables(0).Rows(I).Item("Amount")) Then Amount = 0 Else Amount = dsClinic.Tables(0).Rows(I).Item("Amount")
                    If IsDBNull(dsClinic.Tables(0).Rows(I).Item("curCoverage")) Then Coverage = 0 Else Coverage = dsClinic.Tables(0).Rows(I).Item("curCoverage")
                    If dsClinic.Tables(0).Rows(I).Item("bCash") = True Then Payment = "Cash" Else Payment = "Credit"
                    Content.Append("<tr><td>" & dsClinic.Tables(0).Rows(I).Item("strContact" & DataLang).ToString & "</td><td>" & CDate(dsClinic.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td><td>" & Payment & "</td><td class=""text-md-center text-bold-700"">" & Math.Round(Amount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td class=""text-md-center text-bold-700 red lighten-2"">" & Math.Round(Coverage, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                    TotalCoverage = TotalCoverage + Coverage
                Next
                Content.Append("<tr><td></td><td></td><td></td><td class=""text-md-center text-bold-700""></td><td class=""text-md-center text-bold-700 red"">" & Math.Round(TotalCoverage, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                Content.Append("</table>")
                
                Content.Append("</div>")
                Content.Append("<hr />")


                Content.Append("<h5 class=""text-md-center""><b>" & divPharmacyInvoices & "</b> [<span class="" text-bold-700 indigo lighten-2"">" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), CloseDate).ToString(strDateFormat) & "</span>] To [<span class="" text-bold-700 indigo lighten-2"">" & CloseDate.ToString(strDateFormat) & "</span>]  " & lblFor & " <span class="" text-bold-700 indigo lighten-2"">" & DaysToCalculateMedicineInvoices & "</span> " & lblDays & "</h5>")
                Content.Append("<div class=""full-width text-md-center"">")
                'Dim dsTrans As DataSet = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' AND '" & CloseDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                'If dsTrans.Tables(0).Rows.Count > 0 Then
                '    Content.Append("<table class=""full-width border"">")
                '    Content.Append("<tr><td class=""text-md-right"">" & lblAmount & "</td><td class=""text-md-center text-bold-700"">" & Math.Round(CDec("0" & dsTrans.Tables(0).Rows(0).Item("Amount").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td class=""text-md-right"">" & lblCoverage & "</td><td class=""text-md-center text-bold-700 red lighten-2"">" & Math.Round(CDec("0" & dsTrans.Tables(0).Rows(0).Item("Cov").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                '    Content.Append("</table>")
                'Else
                '    Content.Append(msgNoInvoices)
                'End If
                Amount = 0
                Coverage = 0
                TotalCoverage = 0
                Dim dsTrans As DataSet = dcl.GetDS("SELECT C.strContact" & DataLang & ", ST.dateClosedValid, ST.bCash, SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS curCoverage FROM Stock_Trans AS ST INNER JOIN Hw_Contacts AS C ON ST.lngSalesman=C.lngContact INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), CloseDate).ToString("yyyy-MM-dd") & "' AND '" & CloseDate.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngSalesMan=" & lngSalesman & " AND ST.byteBase = 40 AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY C.strContact" & DataLang & ", ST.dateClosedValid, ST.bCash")
                If dsTrans.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    For I = 0 To dsTrans.Tables(0).Rows.Count - 1
                        If IsDBNull(dsTrans.Tables(0).Rows(I).Item("Amount")) Then Amount = 0 Else Amount = dsTrans.Tables(0).Rows(I).Item("Amount")
                        If IsDBNull(dsTrans.Tables(0).Rows(I).Item("curCoverage")) Then Coverage = 0 Else Coverage = dsTrans.Tables(0).Rows(I).Item("curCoverage")
                        If dsTrans.Tables(0).Rows(I).Item("bCash") = 1 Then Payment = "Cash" Else Payment = "Credit"
                        Content.Append("<tr><td>" & dsTrans.Tables(0).Rows(I).Item("strContact" & DataLang).ToString & "</td><td>" & CDate(dsTrans.Tables(0).Rows(I).Item("dateClosedValid")).ToString(strDateFormat) & "</td><td>" & Payment & "</td><td class=""text-md-center text-bold-700"">" & Math.Round(Amount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td class=""text-md-center text-bold-700 red lighten-2"">" & Math.Round(Coverage, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                        TotalCoverage = TotalCoverage + Coverage
                    Next
                    Content.Append("<tr><td></td><td></td><td></td><td class=""text-md-center text-bold-700""></td><td class=""text-md-center text-bold-700 red"">" & Math.Round(TotalCoverage, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append(msgNoInvoices)
                End If
                Content.Append("</div>")
            Else
                Content.Append("Transaction No:" & lngTransaction & " not available!")
            End If
        Else
            Content.Append("Transaction No is wrong!")
        End If
        Content.Append("</div>")

        Dim mdl As New Share.UI
        Return mdl.drawModal(Header, Content.ToString, "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "bg-grey bg-lighten-2")
    End Function

    Public Function viewPatient(ByVal lngTransaction As Long) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim btnClose As String
        Dim Header As String
        Dim lblFile, lblName, lblID, lblBirthDate, lblAge, lblSex, lblPhone1, lblPhone2 As String
        Dim Male, Female As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "معلومات المريض"
                lblFile = "رقم الملف"
                lblName = "اسم المريض"
                lblID = "رقم الهوية"
                lblAge = "العمر"
                lblBirthDate = "تاريخ الميلاد"
                lblSex = "الجنس"
                lblPhone1 = "رقم الهاتف 1"
                lblPhone2 = "رقم الهاتف 2"
                Male = "ذكر"
                Female = "أنثى"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Patient Information"
                lblFile = "File No"
                lblName = "Patient Name"
                lblID = "ID No"
                lblAge = "Age"
                lblBirthDate = "Birth Date"
                lblSex = "Sex"
                lblPhone1 = "Phone 1 No"
                lblPhone2 = "Phone 2 No"
                Male = "Male"
                Female = "Female"
                btnClose = "Close"
        End Select

        Dim Content As New StringBuilder("")
        Content.Append("<div>")
        If lngTransaction > 0 Then
            ds = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, * FROM Stock_Trans AS ST INNER JOIN Hw_Patients AS P ON ST.lngPatient=P.lngPatient WHERE ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim BirthDate, Age, Sex As String
                If IsDBNull(ds.Tables(0).Rows(0).Item("dateBirth").ToString) Then
                    BirthDate = ""
                    Age = ""
                Else
                    BirthDate = CDate(ds.Tables(0).Rows(0).Item("dateBirth")).ToString(strDateFormat)
                    Age = DateDiff(DateInterval.Year, ds.Tables(0).Rows(0).Item("dateBirth"), Today)
                End If
                If ds.Tables(0).Rows(0).Item("bSex") = True Then Sex = Male Else Sex = Female
                Content.Append("<div class=""full-width"">")
                Content.Append("<table class=""full-width"">")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblFile & ":</td><td class=""text-bold-700 brown lighten-2"">" & ds.Tables(0).Rows(0).Item("lngPatient").ToString & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblName & ":</td><td class=""text-bold-700 brown lighten-2"">" & ds.Tables(0).Rows(0).Item("PatientName").ToString & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblName & ":</td><td class=""text-bold-700 brown lighten-2"">" & ds.Tables(0).Rows(0).Item("strID").ToString & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblBirthDate & ":</td><td class=""text-bold-700 indigo lighten-2"">" & BirthDate & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblAge & ":</td><td class=""text-bold-700 indigo lighten-2"">" & Age & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblSex & ":</td><td class=""text-bold-700 indigo lighten-2"">" & Sex & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblPhone1 & ":</td><td class=""text-bold-700 indigo lighten-2"">" & ds.Tables(0).Rows(0).Item("strPhone1").ToString & "</td></tr>")
                Content.Append("<tr><td class=""text-md-right width-30-per pr-1"">" & lblPhone2 & ":</td><td class=""text-bold-700 indigo lighten-2"">" & ds.Tables(0).Rows(0).Item("strPhone2").ToString & "</td></tr>")
                Content.Append("</table>")
                Content.Append("</div>")
            Else
                Content.Append("Transaction No:" & lngTransaction & " not available!")
            End If
        Else
            Content.Append("Transaction No is wrong!")
        End If
        Content.Append("</div>")

        Dim mdl As New Share.UI
        Return mdl.drawModal(Header, Content.ToString, "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "bg-grey bg-lighten-4")
    End Function
#End Region

#Region "Requests"
    Public Function requestCancelInvoice(ByVal lngTransaction As Long) As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = " & intYear & " AND CONVERT(varchar(10), STI.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dc As New DCL.Conn.XMLData
                    If dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = False Then
                        Dim res As String = createRequest("Cancel", lngTransaction, "")
                        If Left(res, 4) = "Err:" Then Return res
                        ' update user logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 0, "Request cancel invoice")
                    Else
                        Return "Err:This invoice has been requested to cancel already.."
                    End If
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','You have requested to cancel this invoice successfully!','notice');$('#mdlAlpha').modal('hide');updateUI();</script>"
    End Function

    Public Function requestReturnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = " & intYear & " AND CONVERT(varchar(10), STI.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dc As New DCL.Conn.XMLData
                    If dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = False Then
                        Dim res As String = createRequest("Return", lngTransaction, lstItems)
                        If Left(res, 4) = "Err:" Then Return res
                        ' update user logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 0, "Request return items")
                    Else
                        Return "Err:This invoice has been requested to return items already.."
                    End If
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','You have requested to return items successfully!','notice');$('#mdlAlpha').modal('hide');updateUI();</script>"
    End Function

    Public Function requestReopenInvoice(ByVal lngTransaction As Long) As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = " & intYear & " AND CONVERT(varchar(10), STI.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dc As New DCL.Conn.XMLData
                    If dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Reopen_Invoice", "Transaction", lngTransaction, "@status=0") = False Then
                        Dim res As String = createRequest("Reopen", lngTransaction, "")
                        If Left(res, 4) = "Err:" Then Return res
                        ' update user logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 0, "Request re-open invoice")
                    Else
                        Return "Err:This invoice has been requested to re-open already.."
                    End If
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','You have requested to re-open this invoice successfully!','notice');$('#mdlAlpha').modal('hide');updateUI();</script>"
    End Function

    Public Function rejectCancelRequest(ByVal lngTransaction As Long) As String
        Try
            Dim doc As New XmlDocument
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            'get count
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
            Dim count As Integer = nodes.Count
            For Each node As XmlNode In nodes
                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "2"
            Next
            doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            ' update user logs
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 5, "Reject cancel request")
            Return "<script type=""text/javascript"">msg('','The request has been rejected!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function rejectReturnRequest(ByVal lngTransaction As Long) As String
        Try
            Dim doc As New XmlDocument
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            'get count
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items")
            Dim count As Integer = nodes.Count
            For Each node As XmlNode In nodes
                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "2"
            Next
            doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            ' update user logs
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 5, "Reject return request")
            Return "<script type=""text/javascript"">msg('','The request has been rejected!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function rejectReopenRequest(ByVal lngTransaction As Long) As String
        Try
            Dim doc As New XmlDocument
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            'get count
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Reopen_Invoice[@status=0]")
            Dim count As Integer = nodes.Count
            For Each node As XmlNode In nodes
                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "2"
            Next
            doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            ' update user logs
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 0, "Reject re-open request")
            Return "<script type=""text/javascript"">msg('','The request has been rejected!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveCancelRequest(ByVal lngTransaction As Long) As String
        Try
            Dim doc As New XmlDocument
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            'get count
            Dim node As XmlNode = doc.DocumentElement.SelectSingleNode("Cancel_Invoice[@status=0 and Transaction=" & lngTransaction & "]")
            If Not (node Is Nothing) Then
                Dim VoidUser As String = node.Attributes("user").Value
                Dim ret As New Invoices
                Dim res As String = ret.cancelInvoice(lngTransaction, VoidUser)
                If Left(res, 4) <> "Err:" Then
                    node.Attributes("status").Value = "1"
                    doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 4, "Approve cancel request")
                    Return "<script type=""text/javascript"">msg('','The request has been appreved!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
                Else
                    Return res
                End If
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveReturnRequest(ByVal lngTransaction As Long) As String
        Try
            Dim doc As New XmlDocument
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            'get count
            Dim node As XmlNode = doc.DocumentElement.SelectSingleNode("Return_Items[@status=0 and Transaction=" & lngTransaction & "]")
            If Not (node Is Nothing) Then
                Dim ReturnUser As String = node.Attributes("user").Value
                Dim items As String = node.SelectSingleNode("Items").InnerText
                Dim ret As New Invoices
                Dim res As String = ret.returnItems(lngTransaction, items)
                If Left(res, 4) <> "Err:" Then
                    node.Attributes("status").Value = "1"
                    doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 4, "Approve return request")
                    Return "<script type=""text/javascript"">msg('','The request has been appreved!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
                Else
                    Return res
                End If
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveReopenRequest(ByVal lngTransaction As Long) As String
        Try
            Dim ret As New Invoices
            Dim res As String = ret.reopenInvoice(lngTransaction)
            If Left(res, 4) <> "Err:" Then
                Dim doc As New XmlDocument
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                'get count
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Reopen_Invoice[@status=0]")
                Dim count As Integer = nodes.Count
                For Each node As XmlNode In nodes
                    If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "1"
                Next
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                ' update user logs
                Dim usr As New Share.User
                usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 4, "Approve re-open request")
                Return "<script type=""text/javascript"">msg('','The request has been appreved!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
            Else
                Return res
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Private Function createRequest(ByVal Type As String, ByVal lngTransaction As Long, ByVal lstItem As String) As String
        If Type = "Cancel" Then
            Try
                Dim doc As New XmlDocument
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                'get count
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice")
                Dim count As Integer = nodes.Count
                'creatr new  node
                Dim CancelNode As XmlNode = doc.CreateNode(XmlNodeType.Element, "Cancel_Invoice", Nothing)
                'create Attribute
                Dim att_date As XmlAttribute = doc.CreateAttribute("date")
                att_date.Value = Now.ToString("yyyy-MM-dd")
                CancelNode.Attributes.Append(att_date)
                Dim att_time As XmlAttribute = doc.CreateAttribute("time")
                att_time.Value = Now.ToString("HH:mm:ss")
                CancelNode.Attributes.Append(att_time)
                Dim att_user As XmlAttribute = doc.CreateAttribute("user")
                att_user.Value = strUserName
                CancelNode.Attributes.Append(att_user)
                Dim att_status As XmlAttribute = doc.CreateAttribute("status")
                att_status.Value = 0
                CancelNode.Attributes.Append(att_status)
                'create Element
                Dim req As XmlElement = doc.CreateElement("Request")
                req.InnerText = count + 1
                CancelNode.AppendChild(req)
                Dim trans As XmlElement = doc.CreateElement("Transaction")
                trans.InnerText = lngTransaction
                CancelNode.AppendChild(trans)
                Dim res As XmlElement = doc.CreateElement("Reason")
                res.InnerText = ""
                CancelNode.AppendChild(res)
                doc.DocumentElement.AppendChild(CancelNode)
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        ElseIf Type = "Return" Then
            Try
                Dim doc As New XmlDocument
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                'get count
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items")
                Dim count As Integer = nodes.Count
                'creatr new  node
                Dim ReturnNode As XmlNode = doc.CreateNode(XmlNodeType.Element, "Return_Items", Nothing)
                'create Attribute
                Dim att_date As XmlAttribute = doc.CreateAttribute("date")
                att_date.Value = Now.ToString("yyyy-MM-dd")
                ReturnNode.Attributes.Append(att_date)
                Dim att_time As XmlAttribute = doc.CreateAttribute("time")
                att_time.Value = Now.ToString("HH:mm:ss")
                ReturnNode.Attributes.Append(att_time)
                Dim att_user As XmlAttribute = doc.CreateAttribute("user")
                att_user.Value = strUserName
                ReturnNode.Attributes.Append(att_user)
                Dim att_status As XmlAttribute = doc.CreateAttribute("status")
                att_status.Value = 0
                ReturnNode.Attributes.Append(att_status)
                'create Element
                Dim req As XmlElement = doc.CreateElement("Request")
                req.InnerText = count + 1
                ReturnNode.AppendChild(req)
                Dim trans As XmlElement = doc.CreateElement("Transaction")
                trans.InnerText = lngTransaction
                ReturnNode.AppendChild(trans)
                Dim items As XmlElement = doc.CreateElement("Items")
                items.InnerText = lstItem
                ReturnNode.AppendChild(items)
                Dim res As XmlElement = doc.CreateElement("Reason")
                res.InnerText = ""
                ReturnNode.AppendChild(res)
                doc.DocumentElement.AppendChild(ReturnNode)
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        ElseIf Type = "Reopen" Then
            Try
                Dim doc As New XmlDocument
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                'get count
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Reopen_Invoice")
                Dim count As Integer = nodes.Count
                'creatr new  node
                Dim ReopenNode As XmlNode = doc.CreateNode(XmlNodeType.Element, "Reopen_Invoice", Nothing)
                'create Attribute
                Dim att_date As XmlAttribute = doc.CreateAttribute("date")
                att_date.Value = Now.ToString("yyyy-MM-dd")
                ReopenNode.Attributes.Append(att_date)
                Dim att_time As XmlAttribute = doc.CreateAttribute("time")
                att_time.Value = Now.ToString("HH:mm:ss")
                ReopenNode.Attributes.Append(att_time)
                Dim att_user As XmlAttribute = doc.CreateAttribute("user")
                att_user.Value = strUserName
                ReopenNode.Attributes.Append(att_user)
                Dim att_status As XmlAttribute = doc.CreateAttribute("status")
                att_status.Value = 0
                ReopenNode.Attributes.Append(att_status)
                'create Element
                Dim req As XmlElement = doc.CreateElement("Request")
                req.InnerText = count + 1
                ReopenNode.AppendChild(req)
                Dim trans As XmlElement = doc.CreateElement("Transaction")
                trans.InnerText = lngTransaction
                ReopenNode.AppendChild(trans)
                Dim res As XmlElement = doc.CreateElement("Reason")
                res.InnerText = ""
                ReopenNode.AppendChild(res)
                doc.DocumentElement.AppendChild(ReopenNode)
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        End If
        Return ""
    End Function
#End Region

    Public Function changeInvoiceType(ByVal TransNo As Long) As String
        Try
            Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans WHERE lngTransaction = " & TransNo)
            Dim lngContact As Long = ds.Tables(0).Rows(0).Item("lngContact")
            Dim bCash As Boolean = ds.Tables(0).Rows(0).Item("bCash")
            'If lngContact = lngContact_Cash Then Return "Err: Cannot change this invoice to credit invoice"
            Dim intType As Integer
            Dim Desc As String
            If bCash = True Then
                intType = 0
                Desc = "from [cash] to [credit]"
            Else
                intType = 1
                Desc = "from [credit] to [cash]"
            End If
            dcl.ExecSQuery("UPDATE Stock_Trans SET bCash = " & intType & "WHERE lngTransaction=" & TransNo)
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 1, "Order", TransNo, 2, "Change invoice type " & Desc)
            HttpContext.Current.Session("SkipCorrection") = "Yes"
            Return "<script>$('#mdlAlpha').modal('hide');msg('','Invoice has been changed..','success')</script>"

            ' Correct the order to cash if its contact is Pharmacy Cash
            'If (CompanyNo = lngContact_Cash) And (CashOnly = False) Then
            '    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=1 WHERE lngTransaction=" & TransNo)
            '    CashOnly = True
            'End If

            ' Correct the order to credit if its contact is not Pharmacy Cash
            'If (CompanyNo <> lngContact_Cash) And (CashOnly = True) Then
            '    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=0 WHERE lngTransaction=" & TransNo)
            '    CashOnly = True
            'End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function
End Class
