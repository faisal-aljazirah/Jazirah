Imports System.Web
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml

Public Class Orders
    Dim dcl As New DCL.Conn.DataClassLayer
    Public byteLocalCurrency As Byte
    Public intStartupFY As Integer
    Public intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Public byteCurrencyRound As Byte

    Const TableHeight As Integer = 149
    Const InsuranceColor As String = "blue"
    Const CashColor As String = "red"

    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim AllowCancel As Boolean

    Private Class NameValue
        Public Property name As String
        Public Property value As String
    End Class

    Sub New()
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
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

        ' User (Application) Options and Permissions
        'ChangeQuantity_Cash = False ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        'AddDiscount_Cash = False
        'ChangeQuantity_Insurance = False ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        'AddDiscount_Insurance = False
        'AllowExtraItem_Insurance = False ' True = Allow more amount of approved item, False = Allow Exact amount or less
        'AutoMoveRejectedToCash_Insurance = False 'True = move automaticily, False = Popup a confirm message
        'AutoMoveExtraToCash_Insurance = False ' 'True = move extra items to cash automaticily, False = Popup a confirm message
        'SusbendMax = 5 ' Set a maximum number of suspend invoices
        'AskBeforeSend = True 'True = Ask before send invoice to cashier, False = Send Directly
        'AskBeforeReturn = True 'True = Ask before return invoice to sales, False = Return Directly
        'OnePaymentForCashier = False ' True = Cashier Accept (Cash-Credit) in button, False = Cash and Credit have separated buttons
        'ForcePaymentOnCloseInvoice = True ' True = Cashier cannot close invoice without add how much paid, False = Invoice closed without add any payments
        'OneQuantityPerItem = True ' True = Set Quantity to 1 each time to calculate SUM in the end, False = Set Quantity as doctor record
        'DaysToCalculateMedicalInvoices = 7
        'DaysToCalculateMedicineInvoices = 7
        'OrdersLimitDays = 7
        'DirectCancelInvoice = False
        'CancelLimitDays = 4

        'lngContact_Cash = 27
        'byteDepartment_Cash = 15
        'lngSalesman_Cash = 395
        'lngPatient_Cash = 16

        'Dim lang As String
        'If ByteLanguage = 2 Then lang = "Ar" Else lang = "En"
        'Dim ds As DataSet
        'ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & lang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & lang & ",'') + ' ') + LTRIM(ISNULL(strThird" & lang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & lang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        'If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & lang).ToString Else strContact_Cash = ""
        'If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & lang).ToString Else strSalesman_Cash = ""
        'If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        'If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & lang).ToString Else strDepartment_Cash = ""
        loadSettings()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName)
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

        If ByteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""
    End Sub
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
            ds = dcl.GetDS("SELECT DO.lngContact AS DoctorNo, DO.strContact" & DataLang & " AS DoctorName, DO.intSpeciality AS intSpeciality,CO.lngContact AS CompanyNo, CO.strContact" & DataLang & " AS CompanyName,CO.bytePriceType AS bytePriceType,* FROM Stock_Trans AS ST INNER JOIN Hw_Patients AS HP ON ST.lngPatient=HP.lngPatient INNER JOIN Hw_Contacts AS CO ON ST.lngContact=CO.lngContact INNER JOIN Hw_Contacts AS DO ON ST.lngSalesman=DO.lngContact WHERE ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim strReference As String = ds.Tables(0).Rows(0).Item("strReference").ToString
                Dim lngPatient As Long = ds.Tables(0).Rows(0).Item("lngPatient")
                Dim lngSalesman As Long = ds.Tables(0).Rows(0).Item("lngSalesman")
                Dim dateTransaction As Date = ds.Tables(0).Rows(0).Item("dateTransaction")
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
                Dim dsClinic As DataSet = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' And '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                If dsClinic.Tables(0).Rows.Count > 0 Then
                    Content.Append("<table class=""full-width border"">")
                    Content.Append("<tr><td>lngSalesman:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("lngSalesman").ToString & "</td><td>Amount:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("SumOfAmount").ToString & "</td><td>Coverage:</td><td class=""blue-grey"">" & dsClinic.Tables(0).Rows(0).Item("Coverage").ToString & "</td><td>Days:</td><td class=""blue-grey"">" & DaysToCalculateMedicalInvoices & "</td></tr>")
                    Content.Append("</table>")
                Else
                    Content.Append("<div class=""full-width border"">No data</div>")
                End If

                Content.Append("<h5><b>Stock_Trans:</b> Total</h5>")
                Dim dsTrans As DataSet = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
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

    Public Function viewOrder(ByVal TransNo As Long) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim OrderInformation, InvoiceDetails As String
        Dim lblDoctorName, lblInvoiceDate, lblInsuranceCompany, lblPatientName As String
        Dim colMedicineName, colTypeNo, colAmount, colDescription, colRemarkes, colStatus As String
        Dim Approved, Rejected, Processing As String
        Dim btnPrepare, btnClose As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                OrderInformation = "معلومات الطلب"
                InvoiceDetails = "تفاصيل الفاتورة"
                Approved = "مقبولة"
                Rejected = "مرفوضة"
                Processing = "معالجة"
                lblDoctorName = "اسم الطبيب"
                lblInvoiceDate = "تاريخ الفاتورة"
                lblInsuranceCompany = "نظام التأمين"
                lblPatientName = "اسم المريض"
                colMedicineName = "اسم الدواء"
                colTypeNo = "رقم الصنف"
                colAmount = "الكمية"
                colDescription = "الوصفة"
                colRemarkes = "الملاحظات"
                colStatus = "الحالة"
                btnPrepare = "تحضير الأدوية"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                OrderInformation = "Order Information"
                InvoiceDetails = "Invoice Details"
                Approved = "Approved"
                Rejected = "Rejected"
                Processing = "Processing"
                lblDoctorName = "Doctor Name"
                lblInvoiceDate = "Invoice Date"
                lblInsuranceCompany = "Insurance Company"
                lblPatientName = "Patient Name"
                colMedicineName = "Medicine Name"
                colTypeNo = "Type No"
                colAmount = "Amount"
                colDescription = "Description"
                colRemarkes = "Remarks"
                colStatus = "Status"
                btnPrepare = "Prepare Medicines"
                btnClose = "Close"
        End Select

        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & TransNo)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                Dim str As String = ""
                str = str & "<div class=""modal-dialog modal-lg"" role=""document""><div class=""modal-content""><div class=""modal-header""><button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button> <button type=""button"" class=""close font-small-3 ml-1 mr-1"" onclick=""javascript:showInfo(" & TransNo & ")"" style=""margin-top:5px;""><i class=""icon icon-info-circle""></i></button><h4 class=""modal-title"" id="""">" & OrderInformation & "</h4></div><div class=""modal-body""><div class=""card-body collapse in"" style=""margin-top:-30px""><div class=""card-block"">"

                str = str & "<div class=""row""><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblPatientName & ":</div><div class=""teal"">" & ds.Tables(0).Rows(0).Item("PatientName") & "</div></div></div><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblInvoiceDate & ":</div><div class=""teal"">" & CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString(strDateFormat) & "</div></div></div></div>"
                str = str & "<div class=""row""><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblDoctorName & ":</div><div class=""red"">" & ds.Tables(0).Rows(0).Item("DoctorName") & "</div></div></div><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblInsuranceCompany & ":</div><div><span class=""tag tag-info"">" & ds.Tables(0).Rows(0).Item("CompanyName") & " </span></div></div></div></div>"

                str = str & "<h4 class=""form-section""><i class=""icon-clipboard4""></i> " & InvoiceDetails & "</h4><div class=""row""><table class=""table table-bordered mb-0""><thead><tr><th>" & colMedicineName & "</th><th>" & colTypeNo & "</th><th>" & colAmount & "</th><th>" & colDescription & "</th><th>" & colRemarkes & "</th><th>" & colStatus & "</th></tr></thead><tbody>"

                'old
                'ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, SI.strItem AS ServiceNo, SI.strItemEn AS ServiceName, HTP.curQuantity AS Quantity, HTP.strDose AS Usage, HMA.curUnitPrice AS Price, HMA.dateExpiry AS Expiry, CASE WHEN HMA.byteCheck = 1 THEN 'Done' END AS [Status], HTP.Moredetails, HTP.Notes, HTP.intVisit, CASE WHEN HMA.bApproval = 1 THEN (CASE WHEN NOT(HMA.strApprovedBy IS NULL) THEN 1 ELSE (CASE WHEN bRejected = 1 THEN 2 ELSE 0 END) END) ELSE 1 END AS Approval, PQM.strQtyEn + ' ' + PDM.strDoseEn + ' ' + PRM.strRepetitionEn + ' ' + PTM.strTimeEn + ' ' + PPM.strPeriodEn AS Dose FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON HTP.strReference = ST.strReference INNER JOIN Hw_Medicines_Approval AS HMA ON HMA.intVisit = HTP.intVisit AND HMA.lngPatient = HTP.lngPatient AND HMA.strItem = HTP.strItem INNER JOIN Stock_Items AS SI ON HMA.strItem = SI.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,0,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction = '" & TransNo & "'")
                'for testing
                'SELECT HTP.strReference, HTP.intVisit, SI.strItemEn AS ItemName, SI.strItem AS ItemNo, HTP.curQuantity AS Quantity, HTP.strDose AS Usage,HTP.curUnitPrice,HTP.dateExpiry, CASE WHEN HDA.byteCheck = 1 THEN 'Done' END AS [Status], HTP.Moredetails, HTP.Notes, CASE WHEN HDA.bApproval=1 THEN (CASE WHEN HDA.strApprovedBy IS NOT NULL THEN 1 ELSE (CASE WHEN HDA.bRejected=1 THEN 2 ELSE 0 END) END) ELSE 1 END AS Approval, ISNULL(PQM.strQtyEn,'') + ' ' + ISNULL(PDM.strDoseEn,'') + ' ' + ISNULL(PRM.strRepetitionEn,'') + ' ' + ISNULL(PTM.strTimeEn,'') + ' ' + ISNULL(PPM.strPeriodEn,'') AS Dose FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON ST.strReference=HTP.strReference AND ST.lngPatient=HTP.lngPatient INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem LEFT JOIN Hw_Medicines_Approval AS HDA ON ST.lngPatient=HDA.lngPatient AND ST.strReference=HDA.strReference AND HTP.strItem=HDA.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,0,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction=
                ds = dcl.GetDS("SELECT HTP.strReference, HTP.intVisit, SI.strItem" & DataLang & " AS ItemName, SI.strItem AS ItemNo, HTP.curQuantity AS Quantity, HTP.strDose AS Usage,HTP.curUnitPrice,HTP.dateExpiry, CASE WHEN HDA.byteCheck = 1 THEN 'Done' END AS [Status], HTP.Moredetails, HTP.Notes, CASE WHEN HDA.bApproval=1 THEN (CASE WHEN HDA.strApprovedBy IS NOT NULL THEN 1 ELSE (CASE WHEN HDA.bRejected=1 THEN 2 ELSE 0 END) END) ELSE 1 END AS Approval, ISNULL(PQM.strQty" & DataLang & ",'') + ' ' + ISNULL(PDM.strDose" & DataLang & ",'') + ' ' + ISNULL(PRM.strRepetition" & DataLang & ",'') + ' ' + ISNULL(PTM.strTime" & DataLang & ",'') + ' ' + ISNULL(PPM.strPeriod" & DataLang & ",'') AS Dose FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON ST.strReference=HTP.strReference AND ST.lngPatient=HTP.lngPatient INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem LEFT JOIN Hw_Medicines_Approval AS HDA ON ST.lngPatient=HDA.lngPatient AND ST.strReference=HDA.strReference AND HTP.strItem=HDA.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,0,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction=" & TransNo)

                Dim StatusText, StatusColor As String
                For I = 0 To ds.Tables(0).Rows.Count - 1
                    Select Case ds.Tables(0).Rows(I).Item("Approval")
                        Case 1
                            StatusText = Approved
                            StatusColor = "success"
                        Case 2
                            StatusText = Rejected
                            StatusColor = "danger"
                        Case Else
                            StatusText = Processing
                            StatusColor = "warning"
                    End Select
                    str = str & "<tr><td>" & ds.Tables(0).Rows(I).Item("ItemName") & "</td><td>" & ds.Tables(0).Rows(I).Item("ItemNo") & "</td><td>" & Math.Round(ds.Tables(0).Rows(I).Item("Quantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td>" & ds.Tables(0).Rows(I).Item("Dose") & "</td><td>" & Trim(ds.Tables(0).Rows(I).Item("Moredetails") & " " & ds.Tables(0).Rows(I).Item("Notes")) & "</td><td><span class=""tag tag-" & StatusColor & """ >" & StatusText & "</span></td></tr>"
                Next
                str = str & "</tbody></table></div></div></div></div><div class=""modal-footer""><button type=""button"" onclick=""javascript:prepareOrder(" & TransNo & ");"" class=""btn btn-success"">" & btnPrepare & "</button> <button type=""button"" class=""btn grey btn-secondary"" data-dismiss=""modal"">" & btnClose & "</button></div></div></div>"
                Return str
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function

    Public Function prepareOrder(ByVal lngTransaction As Long, Optional ByVal CashOnly As Boolean = False) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim InvoiceDetails, InsuranceInvoiceNo, CashInvoiceNo As String
        Dim lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash As String
        Dim btnPrint, btnDelete, btnPrintAll, btnPayNow, btnSuspend, btnUnsuspend, btnToCashier, btnClose As String
        Dim plcBarcode As String
        Dim rowCounter As Integer = 1

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                InvoiceDetails = "تفاصيل الفاتورة"
                ' Labels
                lblDoctor = "الطبيب"
                lblDate = "التاريخ"
                lblPatient = "المريض"
                lblTotalCovered = "إجمالي المغطى"
                lblTotalCash = "إجمالي النقدي"
                lblPharmacist = "الصيدلي"
                ' Buttons
                btnPrint = "طباعة"
                btnDelete = "حذف"
                btnPrintAll = "طباعة الكل"
                btnSuspend = "تعليق"
                btnUnsuspend = "إلغاء التعليق"
                btnPayNow = "الدفع الآن"
                btnToCashier = "إلى الصندوق"
                btnClose = "إغلاق"
                ' Placeholders
                plcBarcode = "استخدم الباركود أو ادخل اسم أو رقم الدواء..."
            Case Else
                DataLang = "En"
                InvoiceDetails = "Invoice Details"
                ' Labels
                lblDoctor = "Doctor"
                lblDate = "Date"
                lblPatient = "Patient"
                lblTotalCovered = "Total Covered"
                lblTotalCash = "Total Cash"
                lblPharmacist = "Pharmacist"
                ' Buttons
                btnPrint = "Print"
                btnDelete = "Delete"
                btnPrintAll = "Print All"
                btnSuspend = "Suspend"
                btnUnsuspend = "Unsuspend"
                btnPayNow = "Pay Now"
                btnToCashier = "To Cashier"
                btnClose = "Close"
                ' Placeholders
                plcBarcode = "Use barcode or Enter the item name or number..."
        End Select

        Dim body As New StringBuilder("")
        Dim tabHeader As New StringBuilder("")
        Dim tabContent As New StringBuilder("")
        Dim tabCounter As Integer = 1

        '1> check invoice not in other service

        '2> get main data
        'old
        'ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, ISNULL(P.strFirst" & DataLang & ",'') + ' ' + ISNULL(P.strSecond" & DataLang & ",'') + ' ' + ISNULL(P.strThird" & DataLang & " ,'') + ' ' + ISNULL(P.strLast" & DataLang & ",'') AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.lngTransaction = " & lngTransaction)
        'for testing
        'SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirstEn, P.lngPatient) AS PatientFirstName, ISNULL(P.strFirstEn,'') + ' ' + ISNULL(P.strSecondEn,'') + ' ' + ISNULL(P.strThirdEn ,'') + ' ' + ISNULL(P.strLastEn,'') AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContactEn AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = 
        Dim PatientNo, DoctorNo, CompanyNo As Long
        Dim PatientFirstName, PatientName, DoctorName, CompanyName As String
        Dim TransactionDate, InvoiceDate As Date
        Dim InvoiceNo As String
        Dim bCash As Boolean
        Dim bCreatCash As Boolean
        Dim cashItems As String = ""
        Dim insuranceItems As String = ""
        Dim MaxP, CICov, MICov As Decimal

        If lngTransaction > 0 Then
            ' Insurance
            Try
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.bCreatCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo")
                    PatientName = ds.Tables(0).Rows(0).Item("PatientName").ToString
                    PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("ClinicInvoiceNo").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo")
                    CompanyName = ds.Tables(0).Rows(0).Item("CompanyName").ToString
                    InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo")
                    DoctorName = ds.Tables(0).Rows(0).Item("DoctorName").ToString
                    TransactionDate = ds.Tables(0).Rows(0).Item("TransactionDate")
                    CashOnly = ds.Tables(0).Rows(0).Item("bCash")
                    CashInvoiceNo = ""
                    InsuranceInvoiceNo = ""
                    If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) Then bCreatCash = False Else bCreatCash = CBool(ds.Tables(0).Rows(0).Item("bCreatCash"))

                    Dim dsInsurance, dsCash As DataSet
                    If CashOnly = True Then
                        CashInvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                        dsCash = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                        For I = 0 To dsCash.Tables(0).Rows.Count - 1
                            'cashItems = cashItems & "<tr id=""tr_" & rowCounter & """ class=""Ctr_' + curTab + '""><td style=""width:32px;""><input type=""hidden"" name=""barcode_C"" value=""" & dsCash.Tables(0).Rows(I).Item("strBarCode") & """/><input type=""hidden"" name=""dose_C"" value=""""/><input type=""hidden"" name=""item_C"" class=""item_C"" value=""" & dsCash.Tables(0).Rows(I).Item("strItem") & """/></td><td style=""width:70px;"" class=""dynCash"">" & dsCash.Tables(0).Rows(I).Item("strItem") & "</td><td class=""itemName width-150"" title=""" & dsCash.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsCash.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td><td style=""width:100px;"" class=""dynCash red"">" & CDate(dsCash.Tables(0).Rows(I).Item("dateExpiry")).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_C"" value=""" & CDate(dsCash.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""price_C"" class=""price_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""service_C"" value=""" & dsCash.Tables(0).Rows(I).Item("intService") & """/><input type=""hidden"" name=""warehouse_C"" value=""" & dsCash.Tables(0).Rows(I).Item("byteWarehouse") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""percent_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curDiscount"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""discount"" name=""discount_C"" class=""discount_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td style=""width:44px;"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""quantity"" name=""quantity_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""unit_C"" value=""" & dsCash.Tables(0).Rows(I).Item("byteUnit") & """/></td><td style=""width:80px;"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""total_C"" class=""total_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & Math.Round(0, byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeCItems(this);removeThis(this);calculateCash(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a></td></tr>"
                            cashItems = cashItems & createItemRow(lngTransaction, rowCounter, True, "", dsCash.Tables(0).Rows(I).Item("strBarCode"), dsCash.Tables(0).Rows(I).Item("strItem"), dsCash.Tables(0).Rows(I).Item("strItem" & DataLang), dsCash.Tables(0).Rows(I).Item("byteUnit"), dsCash.Tables(0).Rows(I).Item("dateExpiry"), dsCash.Tables(0).Rows(I).Item("curBasePrice"), dsCash.Tables(0).Rows(I).Item("curDiscount"), dsCash.Tables(0).Rows(I).Item("curQuantity"), dsCash.Tables(0).Rows(I).Item("curBaseDiscount"), dsCash.Tables(0).Rows(I).Item("curCoverage"), 0, dsCash.Tables(0).Rows(I).Item("intService"), dsCash.Tables(0).Rows(I).Item("byteWarehouse"), "", True)
                        Next
                    Else
                        InsuranceInvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                        dsInsurance = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                        For I = 0 To dsInsurance.Tables(0).Rows.Count - 1
                            'insuranceItems = insuranceItems & "<tr id=""tr_" & rowCounter & """ class=""Itr""><td style=""width:32px;""><input type=""hidden"" name=""barcode_I"" value=""" & dsInsurance.Tables(0).Rows(I).Item("strBarCode") & """/><input type=""hidden"" name=""dose_I"" value=""""/><input type=""hidden"" name=""item_I"" class=""item_I"" value=""" & dsInsurance.Tables(0).Rows(I).Item("strItem") & """/></td><td style=""width:70px;"" class=""dynInsurance"">" & dsInsurance.Tables(0).Rows(I).Item("strItem") & "</td><td class=""itemName width-150"" title=""" & dsInsurance.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsInsurance.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td><td style=""width:100px;"" class=""dynInsurance red"">" & CDate(dsInsurance.Tables(0).Rows(I).Item("dateExpiry")).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_I"" value=""" & CDate(dsInsurance.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """/></td><td style=""width:80px;"" class=""dynInsurance"">" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""price_I"" class=""price_I"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""service_I"" value=""" & dsInsurance.Tables(0).Rows(I).Item("intService") & """/><input type=""hidden"" name=""warehouse_I"" value=""" & dsInsurance.Tables(0).Rows(I).Item("byteWarehouse") & """/></td><td style=""width:80px;"" class=""dynInsurance"">" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""percent_I"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curDiscount"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""discount"" name=""discount_I"" class=""discount_I"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td style=""width:44px;"">" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""quantity"" name=""quantity_I"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""unit_I"" value=""" & dsInsurance.Tables(0).Rows(I).Item("byteUnit") & """/></td><td style=""width:80px;"">" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""total_I"" class=""total_I"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & Math.Round(dsInsurance.Tables(0).Rows(I).Item("curUnitPrice") - dsInsurance.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeIItems(this);removeThis(this);calculateInsurance(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a></td></tr>"
                            insuranceItems = insuranceItems & createItemRow(lngTransaction, rowCounter, False, "", dsInsurance.Tables(0).Rows(I).Item("strBarCode"), dsInsurance.Tables(0).Rows(I).Item("strItem"), dsInsurance.Tables(0).Rows(I).Item("strItem" & DataLang), dsInsurance.Tables(0).Rows(I).Item("byteUnit"), dsInsurance.Tables(0).Rows(I).Item("dateExpiry"), dsInsurance.Tables(0).Rows(I).Item("curBasePrice"), dsInsurance.Tables(0).Rows(I).Item("curDiscount"), dsInsurance.Tables(0).Rows(I).Item("curQuantity"), dsInsurance.Tables(0).Rows(I).Item("curBaseDiscount"), dsInsurance.Tables(0).Rows(I).Item("curCoverage"), 0, dsInsurance.Tables(0).Rows(I).Item("intService"), dsInsurance.Tables(0).Rows(I).Item("byteWarehouse"), "", True)
                        Next
                        If bCreatCash = True Then
                            Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & InvoiceNo & "' AND lngPatient=" & PatientNo & " AND bSubCash=1 AND byteBase=50")
                            If dsTemp.Tables(0).Rows.Count > 0 Then
                                CashInvoiceNo = dsTemp.Tables(0).Rows(0).Item("strTransaction").ToString
                                dsCash = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & dsTemp.Tables(0).Rows(0).Item("lngTransaction"))
                                For I = 0 To dsCash.Tables(0).Rows.Count - 1
                                    'cashItems = cashItems & "<tr id=""tr_" & rowCounter & """ class=""Ctr""><td style=""width:32px;""><input type=""hidden"" name=""barcode_C"" value=""" & dsCash.Tables(0).Rows(I).Item("strBarCode") & """/><input type=""hidden"" name=""dose_C"" value=""""/><input type=""hidden"" name=""item_C"" class=""item_C"" value=""" & dsCash.Tables(0).Rows(I).Item("strItem") & """/></td><td style=""width:70px;"" class=""dynCash"">" & dsCash.Tables(0).Rows(I).Item("strItem") & "</td><td class=""itemName width-150"" title=""" & dsCash.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsCash.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td><td style=""width:100px;"" class=""dynCash red"">" & CDate(dsCash.Tables(0).Rows(I).Item("dateExpiry")).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_C"" value=""" & CDate(dsCash.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""price_C"" class=""price_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""service_C"" value=""" & dsCash.Tables(0).Rows(I).Item("intService") & """/><input type=""hidden"" name=""warehouse_C"" value=""" & dsCash.Tables(0).Rows(I).Item("byteWarehouse") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""percent_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curDiscount"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""discount"" name=""discount_C"" class=""discount_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td style=""width:44px;"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""quantity"" name=""quantity_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""unit_C"" value=""" & dsCash.Tables(0).Rows(I).Item("byteUnit") & """/></td><td style=""width:80px;"">" & Math.Round(dsCash.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""total_C"" class=""total_C"" value=""" & Math.Round(dsCash.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & Math.Round(0, byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeCItems(this);removeThis(this);calculateCash(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a></td></tr>"
                                    cashItems = cashItems & createItemRow(lngTransaction, rowCounter, True, "", dsCash.Tables(0).Rows(I).Item("strBarCode"), dsCash.Tables(0).Rows(I).Item("strItem"), dsCash.Tables(0).Rows(I).Item("strItem" & DataLang), dsCash.Tables(0).Rows(I).Item("byteUnit"), dsCash.Tables(0).Rows(I).Item("dateExpiry"), dsCash.Tables(0).Rows(I).Item("curBasePrice"), dsCash.Tables(0).Rows(I).Item("curDiscount"), dsCash.Tables(0).Rows(I).Item("curQuantity"), dsCash.Tables(0).Rows(I).Item("curBaseDiscount"), dsCash.Tables(0).Rows(I).Item("curCoverage"), 0, dsCash.Tables(0).Rows(I).Item("intService"), dsCash.Tables(0).Rows(I).Item("byteWarehouse"), "", True)
                                Next
                            End If
                        End If
                    End If


                    If CompanyNo <> lngContact_Cash Then
                        Dim curCoverage As Decimal
                        Dim lngContract As Long
                        Dim byteScheme, bytePrimaryDep As Byte
                        Dim bPercentValue As Boolean

                        Dim dsGuar As DataSet
                        dsGuar = dcl.GetDS("SELECT * FROM Ins_Contracts WHERE lngGuarantor = " & CompanyNo)
                        If dsGuar.Tables(0).Rows.Count > 0 Then
                            Dim dsIns As DataSet
                            dsIns = dcl.GetDS("SELECT HP.bytePrimaryDep, HP.lngGuarantor, IC.lngContract, IC.byteScheme, IC.curDeductionValueP, IC.curDeductionPercentP, IC.curDeductionValueD, IC.curDeductionPercentD FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & PatientNo)
                            If dsIns.Tables(0).Rows.Count > 0 Then
                                lngContract = dsIns.Tables(0).Rows(0).Item("lngContract")
                                byteScheme = dsIns.Tables(0).Rows(0).Item("byteScheme")
                                bytePrimaryDep = dsIns.Tables(0).Rows(0).Item("bytePrimaryDep")
                                If bytePrimaryDep = 1 Then
                                    If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentP")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                                    bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                                Else
                                    If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentD")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                                    bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                                End If

                                Dim dsGroup As DataSet
                                dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & dsIns.Tables(0).Rows(0).Item("lngContract") & " AND byteScheme=" & dsIns.Tables(0).Rows(0).Item("byteScheme"))
                                MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)

                                'Validate Coverage data, [sometimes wrong entry - not match contract]
                                If curCoverage > 0 And MaxP = 0 Then Return "Err:Max deduction is missing, Please correct the contract information.."
                                If curCoverage = 0 And MaxP > 0 Then Return "Err:Deduction percent is missing, Please correct the contract information.."

                                Dim dsClinic As DataSet
                                dsClinic = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), TransactionDate).ToString("yyyy-MM-dd") & "' And '" & TransactionDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & PatientNo & " AND lngSalesMan=" & DoctorNo & " GROUP BY lngSalesman")
                                If dsClinic.Tables(0).Rows.Count > 0 Then
                                    CICov = CDec("0" & dsClinic.Tables(0).Rows(0).Item("Coverage").ToString)
                                Else
                                    CICov = 0
                                End If
                                Dim dsMidicine As DataSet
                                dsMidicine = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), TransactionDate).ToString("yyyy-MM-dd") & "' AND '" & TransactionDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & PatientNo & " AND lngSalesMan=" & DoctorNo & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                                If dsMidicine.Tables(0).Rows.Count > 0 Then
                                    MICov = CDec("0" & dsMidicine.Tables(0).Rows(0).Item("Cov").ToString)
                                Else
                                    MICov = 0
                                End If
                            Else
                                Return "Err: Credit invoice,Complete insurance information"
                            End If
                        Else
                            Return "Err: There are no contracts with this company."
                        End If
                    Else
                        MaxP = 0
                        CICov = 0
                        MICov = 0
                    End If
                Else
                    Return "Err:This record is unavailable, please refresh the orders again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            ' Cash
            PatientNo = 16
            ds = dcl.GetDS("SELECT ISNULL(strFirst" & DataLang & ", lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",'') + ' ')) AS PatientName,* FROM Hw_Patients WHERE lngPatient=" & PatientNo)
            PatientName = ds.Tables(0).Rows(0).Item("PatientName")
            PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName")
            InvoiceDate = Today
            InvoiceNo = ""
            CompanyNo = 27
            ds.Clear()
            ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & CompanyNo)
            CompanyName = ds.Tables(0).Rows(0).Item("strContact" & DataLang)
            DoctorNo = 395
            ds.Clear()
            ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & DoctorNo)
            DoctorName = ds.Tables(0).Rows(0).Item("strContact" & DataLang)
            CashOnly = True
            CashInvoiceNo = ""
            InsuranceInvoiceNo = ""
            MaxP = 0
            CICov = 0
            MICov = 0
        End If

        '3> add new tab (to join later with other tabs) [MUST BY ACTIVE IN CLASS]
        tabHeader.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab" & tabCounter & """ data-toggle=""tab"" aria-controls=""tab" & tabCounter & """ href=""#tab" & tabCounter & """ aria-expanded=""true"">" & PatientFirstName & "</a></li>")

        ' ''>4 add content of current tab
        tabContent.Append(createTabContent(tabCounter, True, lngTransaction, PatientName, DoctorName, InvoiceDate, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, insuranceItems, cashItems, MaxP, CICov, MICov, CashOnly))

        ' ''==>4.4 add script

        ''>5 add any other tabs in here, using [tabCounter]
        If Not (HttpContext.Current.Session("Suspend_Trans") Is Nothing) Then
            Dim trans As String() = Split(HttpContext.Current.Session("Suspend_Trans"), "×")
            Dim cashO As String() = Split(HttpContext.Current.Session("Suspend_CashOnly"), "×")
            Dim IItem As String() = Split(HttpContext.Current.Session("Suspend_IItems"), "×")
            Dim CItem As String() = Split(HttpContext.Current.Session("Suspend_CItems"), "×")
            For I = 0 To trans.Length - 1
                If trans(I) <> "" Then
                    Dim dsTemp As DataSet
                    Dim build As Boolean = False
                    If trans(I) > 0 Then
                        ' Insurance
                        Try
                            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & trans(I))
                            If ds.Tables(0).Rows.Count > 0 Then
                                PatientNo = ds.Tables(0).Rows(0).Item("PatientNo")
                                PatientName = ds.Tables(0).Rows(0).Item("PatientName")
                                PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName")
                                InvoiceNo = ds.Tables(0).Rows(0).Item("ClinicInvoiceNo")
                                CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo")
                                CompanyName = ds.Tables(0).Rows(0).Item("CompanyName")
                                InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                                DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo")
                                DoctorName = ds.Tables(0).Rows(0).Item("DoctorName")
                                CashOnly = ds.Tables(0).Rows(0).Item("bCash")
                                build = True
                            Else
                                'Return "Err:This record is unavailable, please refresh the orders again.."
                            End If
                        Catch ex As Exception
                            'Return "Err:" & ex.Message
                        End Try
                    Else
                        ' Cash
                        PatientNo = 16
                        ds = dcl.GetDS("SELECT ISNULL(strFirst" & DataLang & ", lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",'') + ' ')) AS PatientName,* FROM Hw_Patients WHERE lngPatient=" & PatientNo)
                        PatientName = ds.Tables(0).Rows(0).Item("PatientName")
                        PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName")
                        InvoiceDate = Today
                        InvoiceNo = ""
                        CompanyNo = 27
                        ds.Clear()
                        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & CompanyNo)
                        CompanyName = ds.Tables(0).Rows(0).Item("strContact" & DataLang)
                        DoctorNo = 395
                        ds.Clear()
                        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & DoctorNo)
                        DoctorName = ds.Tables(0).Rows(0).Item("strContact" & DataLang)
                        CashOnly = True
                        build = True
                    End If

                    If build = True Then
                        tabCounter = tabCounter + 1
                        tabHeader.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab" & tabCounter & """ data-toggle=""tab"" aria-controls=""tab" & tabCounter & """ href=""#tab" & tabCounter & """ aria-expanded=""false"">" & PatientFirstName & "</a></li>")
                        Dim InsItems As String = Replace(IItem(I), "|", "'")
                        InsItems = Replace(InsItems, "^", """")
                        Dim ISplt As String() = Split(InsItems, "!!")
                        InsItems = ""
                        For S = 0 To ISplt.Length - 1
                            If ISplt(S) <> "" Then InsItems = InsItems & "<tr id=""tr_" & rowCounter & """ class=""Itr"">" & ISplt(S) & "</tr>"
                        Next
                        Dim CshItems As String = Replace(CItem(I), "|", "'")
                        CshItems = Replace(CshItems, "^", """")
                        Dim CSplt As String() = Split(CshItems, "!!")
                        CshItems = ""
                        For S = 0 To CSplt.Length - 1
                            If CSplt(S) <> "" Then CshItems = CshItems & "<tr id=""tr_" & rowCounter & """ class=""Ctr"">" & CSplt(S) & "</tr>"
                        Next
                        tabContent.Append(createTabContent(tabCounter, False, trans(I), PatientName, DoctorName, InvoiceDate, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, InsItems, CshItems, 0, 0, 0, CBool(cashO(I))))
                        tabContent.Append("<script type=""text/javascript"">calculateInsurance(" & tabCounter & ");calculateCash(" & tabCounter & ");</script>")
                    End If
                End If
            Next
        End If

        ''>6 add barcode input
        Dim barcode As String = "<div class=""row""><div class=""col-md-12"" style=""margin-top:-20px; margin-bottom:-10px;""><div class=""position-relative has-icon-left""><input type=""text"" class=""form-control round  border-primary text-md-center"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ /><div class=""form-control-position""><i class=""icon-barcode""></i></div></div></div></div>"

        ''>7 add buttons
        Dim sendFunction As String
        Dim printAllButton As String = ""
        If (PrintDose = 2) Or (PrintDose = 3) Then printAllButton = "<span id=""divPrintAll" & tabCounter & """ app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-url=""p_dose.aspx""><button type=""button"" id=""btnPrintAll" & tabCounter & """ onclick=""javascript:printAllDose(" & tabCounter & ");"" class=""btn btn-blue-grey ml-1"" disabled=""disabled""><i class=""icon-printer""></i> " & btnPrintAll & "</button></span>"
        If AskBeforeSend = True Then sendFunction = "confirm('','Send this invoice to cashier?',sendToCashier);" Else sendFunction = "sendToCashier();"
        Dim buttons As String = "<div class=""text-md-center""><button type=""button"" class=""btn btn-outline-success mr-1"" onclick=""javascript:" & sendFunction & """><i class=""icon-coin-dollar""></i> " & btnToCashier & "</button><button type=""button"" class=""btn btn-success mr-3"" onclick=""javascript:showCashier($('#trans' + curTab).val());""><i class=""icon-money""></i> " & btnPayNow & "</button><button type=""button"" class=""btn btn-primary ml-3"" id=""btnSuspend"" onclick=""javascript:suspendInvoice($('#trans'+curTab).val(), $('#cashOnly'+curTab).val(), collectIItems(curTab), collectCItems(curTab), $('#suspend'+curTab).val());""><i class=""icon-clock5""></i> " & btnSuspend & "</button>" & printAllButton & "<button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

        ''>7 close everything

        ''>8 create footer

        ''>9 add scripts
        Dim scripts As String = ""
        scripts = scripts & "<script type=""text/javascript"">"
        scripts = scripts & "var curTab = 1;$('a[data-toggle=""tab""]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); if(curTab!=1) {$('#btnSuspend').attr('disabled', false); $('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnUnsuspend & "');} else {$('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnSuspend & "');if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true); else $('#btnSuspend').attr('disabled', false);}});"
        scripts = scripts & "var counter" & tabCounter & "=" & rowCounter & ";var tabCount=" & tabCounter & ";if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true);"
        'scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {completeBarcode(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
        scripts = scripts & "$('#txtBarcode').on('change paste keyup', function () {var barcode = $(this).val();if (barcode.length != 0) {if ($.isNumeric(barcode) == true) {if (event.which == 13 || barcode.length >= 14) {event.preventDefault();$(this).val('');getItemInfo(barcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val());}}}});"
        scripts = scripts & "function showCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { TabCounter: curTab, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/viewCashier1',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#mdlMessage').html(response.d);$('#mdlMessage').modal('show');}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
        scripts = scripts & "function sendToCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/SendToCashier',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
        scripts = scripts & "calculateInsurance(curTab);calculateCash(curTab);$('#txtBarcode').focus();"
        scripts = scripts & "</script>"

        ''>9 Build everything in modal
        Dim mdl As New Share.UI
        Dim tabHead As String = "<ul class=""nav nav-tabs"" id=""tabCashier"">" & tabHeader.ToString & "</ul>"
        Dim tabBody As String = "<div class=""tab-content px-1 pt-1"">" & tabContent.ToString & "</div>"
        tabBody = tabBody & " <button type=""button"" class=""close font-small-3"" style=""position:absolute; right:0px; top:0px;"" onclick=""javascript:showInfo(" & lngTransaction & ")"" style=""margin-top:5px;""><i class=""icon icon-info-circle""></i></button>"
        body.Append(mdl.drawModal(InvoiceDetails, tabHead & tabBody & barcode & scripts, buttons, Share.UI.ModalSize.Large, "", "p-0"))

        Return body.ToString
    End Function
    Private Function getCoverage(ByVal lngPatient As Long, ByVal lngContact As Long) As String() 'return (0) bPercentValue, (1) curCoverage, (2) MaxLimit
        Dim curCoverage As Decimal
        Dim lngContract As Long
        Dim byteScheme, bytePrimaryDep As Byte
        Dim bPercentValue As Boolean
        Dim MaxP As Decimal
        Try
            Dim dsGuar As DataSet
            dsGuar = dcl.GetDS("SELECT * FROM Ins_Contracts WHERE lngGuarantor = " & lngContact)
            If dsGuar.Tables(0).Rows.Count > 0 Then
                Dim dsIns As DataSet
                dsIns = dcl.GetDS("SELECT HP.bytePrimaryDep, HP.lngGuarantor, IC.lngContract, IC.byteScheme, IC.curDeductionValueP, IC.curDeductionPercentP, IC.curDeductionValueD, IC.curDeductionPercentD FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & lngPatient)
                If dsIns.Tables(0).Rows.Count > 0 Then
                    lngContract = dsIns.Tables(0).Rows(0).Item("lngContract")
                    byteScheme = dsIns.Tables(0).Rows(0).Item("byteScheme")
                    bytePrimaryDep = dsIns.Tables(0).Rows(0).Item("bytePrimaryDep")
                    If bytePrimaryDep = 1 Then
                        If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentP")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                        bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                    Else
                        If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentD")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                        bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                    End If

                    Dim dsGroup As DataSet
                    dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & dsIns.Tables(0).Rows(0).Item("lngContract") & " AND byteScheme=" & dsIns.Tables(0).Rows(0).Item("byteScheme"))
                    MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)

                    Return {bPercentValue, curCoverage, MaxP}
                Else
                    Return {"Err: Credit invoice,Complete insurance information"}
                End If
            Else
                Return {"Err: There are no contracts with this company."}
            End If
        Catch ex As Exception
            Return {"Err:" & ex.Message}
        End Try
    End Function

    Private Function getTotalClinicInvoices(ByVal lngPatient As Long, ByVal lngSalesMan As Long, ByVal dateTransaction As Date) As Decimal
        Dim dsClinic As DataSet
        dsClinic = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' And '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesMan & " GROUP BY lngSalesman")
        If dsClinic.Tables(0).Rows.Count > 0 Then
            Return CDec("0" & dsClinic.Tables(0).Rows(0).Item("Coverage").ToString)
        Else
            Return 0
        End If
    End Function

    Private Function getTotalPharmacyInvoices(ByVal lngPatient As Long, ByVal lngSalesMan As Long, ByVal dateTransaction As Date, ByVal lngTransaction As Long, ByVal InculdeCurrent As Boolean) As Decimal
        Dim dsMidicine As DataSet
        Dim TrnasSQL As String = ""
        If InculdeCurrent = False Then TrnasSQL = " AND ST.lngTransaction<>" & lngTransaction
        dsMidicine = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesMan & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 " & TrnasSQL & " GROUP BY ST.lngSalesman")
        If dsMidicine.Tables(0).Rows.Count > 0 Then
            Return CDec("0" & dsMidicine.Tables(0).Rows(0).Item("Cov").ToString)
        Else
            Return 0
        End If
    End Function

    Private Function createTabContent(ByVal tabCounter As Integer, ByVal IsActive As Boolean, ByVal lngTransaction As Long, ByVal PatientName As String, ByVal DoctorName As String, ByVal InvoiceDate As Date, ByVal CompanyName As String, ByVal InsuranceInvoiceNo As String, ByVal CashInvoiceNo As String, ByVal InsuranceRows As String, ByVal CashRows As String, ByVal MaxP As Decimal, ByVal CICov As Decimal, ByVal MICov As Decimal, Optional ByVal CashOnly As Boolean = False) As String
        Dim InvoiceDetails, InsuranceInvoice, CashInvoice As String
        Dim lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash As String
        Dim tabContent As New StringBuilder("")

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                InvoiceDetails = "تفاصيل الفاتورة"
                InsuranceInvoice = "فاتورة التأمين"
                CashInvoice = "فاتورة النقدي"
                ' Labels
                lblDoctor = "الطبيب"
                lblDate = "التاريخ"
                lblPatient = "المريض"
                lblTotalCovered = "إجمالي المغطى"
                lblTotalCash = "إجمالي النقدي"
                lblPharmacist = "الصيدلي"
            Case Else
                DataLang = "En"
                InvoiceDetails = "Invoice Details"
                InsuranceInvoice = "Insurance Invoice"
                CashInvoice = "Cash Invoice"
                ' Labels
                lblDoctor = "Doctor"
                lblDate = "Date"
                lblPatient = "Patient"
                lblTotalCovered = "Total Covered"
                lblTotalCash = "Total Cash"
                lblPharmacist = "Pharmacist"
        End Select

        Dim active As String = ""
        Dim suspend As Integer
        If IsActive = True Then active = "active"
        If IsActive = True Then suspend = 0 Else suspend = 1

        tabContent.Append("<div role=""tabpanel"" class=""tab-pane " & active & """ id=""tab" & tabCounter & """ aria-expanded=""true"" aria-labelledby=""base-tab" & tabCounter & """>")
        tabContent.Append("<form id=""frmInvoice" & tabCounter & """>")
        ''==>4.2 hidden inputs
        tabContent.Append("<input type=""hidden"" id=""trans" & tabCounter & """ name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter" & tabCounter & """ value=""0"" /><input type=""hidden"" id=""coveredCash" & tabCounter & """ name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash" & tabCounter & """ name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash" & tabCounter & """ name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice" & tabCounter & """ value=""0"" /><input type=""hidden"" id=""cashOnly" & tabCounter & """ name=""cashOnly"" value=""" & CInt(CashOnly) & """ /><input type=""hidden"" id=""suspend" & tabCounter & """ value=""" & suspend & """ /><input type=""hidden"" id=""items_I_" & tabCounter & """ value="""" /><input type=""hidden"" id=""items_C_" & tabCounter & """ value="""" /><input type=""hidden"" id=""Limit_" & tabCounter & """ value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_" & tabCounter & """ value=""" & CICov & """ /><input type=""hidden"" id=""MICov_" & tabCounter & """ value=""" & MICov & """ /><input type=""hidden"" id=""coveredVat" & tabCounter & """ name=""coveredVat"" value=""0"" /><input type=""hidden"" id=""deductionVat" & tabCounter & """ name=""deductionVat"" value=""0"" /><input type=""hidden"" id=""nonCoveredVat" & tabCounter & """ name=""nonCoveredVat"" value=""0"" />")
        ''==>4.3 header start
        tabContent.Append("<div class=""row""><div class=""col-md-12""><div class=""card""><div class=""card-body collapse in""><div class=""card-block"" style=""padding:10px; font-size:14px;"">")
        tabContent.Append("<div class=""col-md-5 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & PatientName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblDoctor & ":</div><div class=""col-md-9 red"">" & DoctorName & "</div></div></div>")
        tabContent.Append("<div class=""col-md-3 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-6 text-md-right text-bold-900"">" & lblPharmacist & ":</div><div class=""col-md-6 teal"">" & strUserName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 red"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></div></div>")
        tabContent.Append("<div class=""col-md-4 m-0 p-0""><div class=""col-md-12 p-0""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-5 teal""><span class=""tag tag-sm tag-primary"" style=""width:75px;"" id=""totalCash" & tabCounter & """>0.00</span><span class=""tag tag-sm tag-primary"" style=""width:25px;"" id=""totalVat" & tabCounter & """>0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCovered & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalCovered" & tabCounter & """>0.00</span><span class=""tag tag-sm tag-default"" style=""width:25px;"" id=""totalCoveredVat" & tabCounter & """>0.00</span></div></div></div>")
        tabContent.Append("</div></div></div></div></div>")
        ''==>4.3 tables start
        tabContent.Append(createItemsTables(tabCounter, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, InsuranceRows, CashRows, CashOnly))
        tabContent.Append("</form></div>")
        Return tabContent.ToString
    End Function

    Private Function createItemsTables(ByVal Identifier As Integer, ByVal InsuranceCompany As String, ByVal InsuranceInvoiceNo As String, ByVal CashInvoiceNo As String, ByVal InsuranceRows As String, ByVal CashRows As String, Optional ByVal CashOnly As Boolean = False) As String
        Dim str As New StringBuilder("")

        Dim InsuranceInvoice, CashInvoice, Invoice As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colDiscount, colTotal As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim CashCompany As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Invoice = "فاتورة"
                InsuranceInvoice = "فاتورة التأمين"
                CashInvoice = "فاتورة النقدي"
                colBarcode = "الباركود"
                colItemNo = "رقم"
                colItemName = "الصنف"
                colAmount = "كم"
                colExpireDate = "الانتهاء"
                colUnitPrice = "السعر"
                colDiscount = "الخصم"
                colTotal = "مجموع"
                InsuranceExtend = "right"
                CashExtend = "left"
            Case Else
                DataLang = "En"
                Invoice = "Invoice"
                InsuranceInvoice = "Insurance Invoice"
                CashInvoice = "Cash Invoice"
                colBarcode = "Barcode"
                colItemNo = "No"
                colItemName = "Item Name"
                colAmount = "Qty"
                colExpireDate = "Expire"
                colUnitPrice = "Price"
                colDiscount = "Discount"
                colTotal = "Total"
                InsuranceExtend = "left"
                CashExtend = "right"
        End Select

        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact=27")
        CashCompany = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString

        If CashOnly = False Then CashBtnExtend = "<a class=""cursor-pointer " & CashColor & " lighten-3"" href=""javascript:changeToCash(" & Identifier & ")""><i id=""btnCashIcon"" class=""icon-circle-" & CashExtend & """></i></a>"

        str.Append("<div class=""row"">")
        If CashOnly = False Then
            'Insurance
            str.Append("<div class=""col-md-6 insurance" & Identifier & """ id=""divInsurance" & Identifier & """><div class=""card border-" & InsuranceColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & InsuranceColor & " lighten-3""><span class=""icon-clipboard4 text-muted""></span> " & InsuranceInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & Invoice & ": (<span class=""orange"">" & InsuranceInvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & InsuranceCompany & """>" & InsuranceCompany & "</span></div></div>")
            str.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance" & Identifier & """><tbody>")
            str.Append(InsuranceRows)
            str.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""><a class=""cursor-pointer " & InsuranceColor & " lighten-3"" href=""javascript:changeToInsurance(" & Identifier & ")""><i id=""btnInsuranceIcon"" class=""icon-circle-" & InsuranceExtend & """></i></a></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_" & Identifier & """>0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_" & Identifier & """>0.00</th><th><span class=""dynInsurance grey"" id=""covInfo" & Identifier & """></span></th></tr></thead></table></div></div></div></div>")
        End If
        'Cash
        str.Append("<div class=""col-md-6 cash" & Identifier & """ id=""divCash" & Identifier & """><div class=""card border-" & CashColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & CashColor & " lighten-3""><span class=""icon-money text-muted""></span> " & CashInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & Invoice & ": (<span class=""orange"">" & CashInvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CashCompany & """>" & CashCompany & "</span></div></div>")
        str.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash" & Identifier & """><tbody>")
        str.Append(CashRows)
        str.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""><a class=""cursor-pointer " & CashColor & " lighten-3"" href=""javascript:changeToCash(" & Identifier & ")""><i id=""btnCashIcon"" class=""icon-circle-" & CashExtend & """></i></a></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_" & Identifier & """>0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_" & Identifier & """>0.00</th><th></th></tr></thead></table></div></div></div></div>")
        'str.Append("<div class=""col-md-6 cash" & Identifier & """ id=""divCash" & Identifier & """><div class=""card border-" & CashColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & CashColor & " lighten-3""><span class=""icon-money text-muted""></span> " & CashInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & Invoice & ": (<span class=""orange"">0</span>)</span> <span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CashCompany & """>" & CashCompany & "</span></div></div>")
        'str.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table able-bordered mb-0 mt-0"" id=""tblCash" & Identifier & """><tbody>")
        'str.Append(CashRows)
        'str.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;"">" & CashBtnExtend & "</th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_" & Identifier & """>0.00</th><th style=""width:80px;"" class=""dynCash"" id=""discount_C_" & Identifier & """>0.00</th><th style=""width:40px;""></th><th style=""width:80px;"" id=""total_C_" & Identifier & """>0.00</th><th></th></tr></thead></table></div></div></div></div>")

        str.Append("</div>")
        If CashOnly = False Then
            str.Append("<script type=""text/javascript"">cashOn[" & Identifier & "] = true;changeToCash(" & Identifier & ");InsuranceOn[" & Identifier & "] = true;changeToInsurance(" & Identifier & ");</script>")
        Else
            str.Append("<script type=""text/javascript"">cashOn[" & Identifier & "] = false;changeToCash(" & Identifier & ");</script>")
        End If
        Return str.ToString
    End Function

    Public Function viewVoucher(ByVal lngTransaction As Long) As String
        Dim IsCash, Cancellation, Returning As Boolean
        Dim PatientNo, PatientName, DoctorNo, DoctorName, PharmacistName, CashierName, InvoiceNo, CompanyName, CompanyNo, VoidName As String
        Dim InvoiceDate, TransactionDate, VoidDate As Date
        Dim Status As Byte

        Dim Status_Cancelled, Status_Paid, Status_Unpaid As String

        Dim InvoiceItems As String = ""
        Dim ReturnItems As String = ""
        Dim CancellationMessage, ReturningMessage, ReturnMessage As String
        Dim FinalMessage As String = ""
        Dim VoucherColor As String = "green"
        Dim ReturnVoucher As String

        Dim InvoiceDetails, InsuranceInvoice, CashInvoice As String
        Dim lblStatus, lblCashier, lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash, lblTotal As String
        Dim lblInvoice As String
        Dim lblCancelBy, lblCancelDate As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colDiscount, colTotal As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim btnReturnItem, btnReqReturnItem, btnCancel, btnReqCancel, btnPrint, btnClose, btnApprove, btnReject As String

        Dim body As New StringBuilder("")
        Dim ds, dsItems As DataSet

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                InvoiceDetails = "تفاصيل الفاتورة"
                InsuranceInvoice = "فاتورة التأمين"
                ReturnVoucher = "سند مرتجع"
                CashInvoice = "فاتورة النقدي"
                ' Labels
                lblDoctor = "الطبيب"
                lblDate = "التاريخ"
                lblPatient = "المريض"
                lblTotalCovered = "إجمالي المغطى"
                lblTotalCash = "إجمالي النقدي"
                lblPharmacist = "الصيدلي"
                lblInvoice = "فاتورة"
                lblStatus = "الحالة"
                lblCashier = "الصندوق"
                lblTotal = "الإجمالي"
                lblCancelBy = "ألغيت بواسطة"
                lblCancelDate = "ألغيت في"
                ' Columns
                colBarcode = "الباركود"
                colItemNo = "رقم"
                colItemName = "الصنف"
                colAmount = "كم"
                colExpireDate = "الانتهاء"
                colUnitPrice = "السعر"
                colDiscount = "الخصم"
                colTotal = "مجموع"
                ' Buttons
                btnReturnItem = "إعادة صنف"
                btnCancel = "إلغاء الفاتورة"
                btnPrint = "طباعة"
                btnClose = "إغلاق"
                btnReqCancel = "طلب إلغاء الفاتورة"
                btnReqReturnItem = "طلب إعادة صنف"
                btnApprove = "تعميد"
                btnReject = "رفض"
                ' Variables
                InsuranceExtend = "right"
                CashExtend = "left"
                Status_Cancelled = "<span class=""tag tag-sm red"">ملغاة</span>"
                Status_Paid = "<span class=""tag tag-sm blue"">مدفوعة</span>"
                Status_Unpaid = "<span class=""tag tag-sm grey"">غير مدفوعة</span>"
                CancellationMessage = "تم طلب الإلغاء بواسطة @USER في @DATE"
                ReturningMessage = "تم طلب إعادة أصناف بواسطة @USER في @DATE"
                ReturnMessage = " من الأصناف تمت إعادتها"
            Case Else
                DataLang = "En"
                InvoiceDetails = "Invoice Details"
                InsuranceInvoice = "Insurance Invoice"
                CashInvoice = "Cash Invoice"
                ReturnVoucher = "Return Voucher"
                ' Labels
                lblDoctor = "Doctor"
                lblDate = "Date"
                lblPatient = "Patient"
                lblTotalCovered = "Total Covered"
                lblTotalCash = "Total Cash"
                lblPharmacist = "Pharmacist"
                lblInvoice = "Invoice"
                lblStatus = "Status"
                lblCashier = "Cashier"
                lblTotal = "Total"
                lblCancelBy = "Cancelled By"
                lblCancelDate = "Cancel Date"
                ' Columns
                colBarcode = "Barcode"
                colItemNo = "No"
                colItemName = "Item Name"
                colAmount = "Qty"
                colExpireDate = "Expire"
                colUnitPrice = "Price"
                colDiscount = "Discount"
                colTotal = "Total"
                ' Buttons
                btnReturnItem = "Return Item"
                btnCancel = "Cancel Invoice"
                btnPrint = "Print"
                btnClose = "Close"
                btnReqCancel = "Request Cancel Invoice"
                btnReqReturnItem = "Request Return Item"
                btnApprove = "Approve"
                btnReject = "Reject"
                ' Variables
                InsuranceExtend = "left"
                CashExtend = "right"
                Status_Cancelled = "<span class=""tag tag-sm red"">Cancelled</span>"
                Status_Paid = "<span class=""tag tag-sm blue"">Paid</span>"
                Status_Unpaid = "<span class=""tag tag-sm grey"">Unpaid</span>"
                CancellationMessage = "Cancellation request made by @USER in @DATE"
                ReturningMessage = "Returning request made by @USER in @DATE"
                ReturnMessage = " Items has returned"
        End Select

        If lngTransaction > 0 Then
            ' Insurance
            Try
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateTransaction AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS PharmacistName, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.byteStatus AS [Status] FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 18 AND Year(ST.dateTransaction) = 2019 AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    ' get general data
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo")
                    PatientName = ds.Tables(0).Rows(0).Item("PatientName").ToString
                    'PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo")
                    CompanyName = ds.Tables(0).Rows(0).Item("CompanyName").ToString
                    InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo")
                    DoctorName = ds.Tables(0).Rows(0).Item("DoctorName").ToString
                    TransactionDate = ds.Tables(0).Rows(0).Item("TransactionDate")
                    IsCash = ds.Tables(0).Rows(0).Item("bCash")
                    PharmacistName = ds.Tables(0).Rows(0).Item("PharmacistName").ToString
                    CashierName = ds.Tables(0).Rows(0).Item("CashierName").ToString
                    InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    Status = ds.Tables(0).Rows(0).Item("Status")
                    VoidName = ds.Tables(0).Rows(0).Item("VoidName").ToString
                    If IsDBNull(ds.Tables(0).Rows(0).Item("VoidDate")) Then VoidDate = Today Else VoidDate = ds.Tables(0).Rows(0).Item("VoidDate")
                    'UserName = ds.Tables(0).Rows(0).Item("UserName").ToString
                    'CashInvoiceNo = ""
                    'InsuranceInvoiceNo = ""
                    Dim InvoiceStatus As String
                    Select Case Status
                        Case 0
                            InvoiceStatus = Status_Cancelled
                        Case 1
                            InvoiceStatus = Status_Paid
                        Case 2
                            InvoiceStatus = Status_Unpaid
                        Case Else
                            InvoiceStatus = ""
                    End Select

                    Dim dsRItems As DataSet
                    dsRItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND XI.bCopied=1")
                    Dim ReturnedCount As Integer = dsRItems.Tables(0).Rows.Count
                    Cancellation = False
                    Returning = False
                    If Status = 0 Then
                        FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center""><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelBy & ":</div><div class=""col-md-4 teal"">" & VoidName & "</div><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelDate & ":</div><div class=""col-md-4 teal"">" & VoidDate.ToString(strDateFormat & " " & strTimeFormat) & "</div></div></div>"
                    Else
                        Dim dc As New DCL.Conn.XMLData
                        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = True Then
                            Dim doc As New XmlDocument
                            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
                            For Each node As XmlNode In nodes
                                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                                    Cancellation = True
                                    CancellationMessage = Replace(CancellationMessage, "@USER", "<span class=""teal"">" & node.Attributes("user").Value & "</span>")
                                    CancellationMessage = Replace(CancellationMessage, "@DATE", "<span class=""teal"">" & CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value).ToString(strDateFormat & " " & strTimeFormat) & "</span>")
                                    FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & CancellationMessage & "</div></div>"
                                End If
                            Next
                        ElseIf dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = True Then
                            Dim doc As New XmlDocument
                            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items[@status=0]")
                            For Each node As XmlNode In nodes
                                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                                    Returning = True
                                    ReturnItems = node.SelectSingleNode("Items").InnerText
                                    ReturningMessage = Replace(ReturningMessage, "@USER", "<span class=""teal"">" & node.Attributes("user").Value & "</span>")
                                    ReturningMessage = Replace(ReturningMessage, "@DATE", "<span class=""teal"">" & CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value).ToString(strDateFormat & " " & strTimeFormat) & "</span>")
                                    FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReturningMessage & "</div></div>"
                                End If
                            Next
                        Else
                            If ReturnedCount > 0 Then
                                FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReturnedCount & ReturnMessage & "</div></div>"
                            End If
                        End If
                    End If

                    ' build invoice header
                    body.Append("<div class=""row""><div class=""col-md-12""><div class=""card""><div class=""card-body collapse in""><div class=""card-block"" style=""padding:10px; font-size:14px;"">")
                    body.Append("<div class=""col-md-5 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & PatientName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblDoctor & ":</div><div class=""col-md-9 red"">" & DoctorName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblStatus & ":</div><div class=""col-md-9"">" & InvoiceStatus & "</div></div></div>")
                    body.Append("<div class=""col-md-3 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 teal"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblPharmacist & ":</div><div class=""col-md-6 red"">" & PharmacistName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblCashier & ":</div><div class=""col-md-6 red"">" & CashierName & "</div></div></div>")
                    body.Append("<div class=""col-md-4 m-0 p-0""><div class=""col-md-12 p-0""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-5 teal""><span class=""tag tag-sm tag-primary"" style=""width:75px;"" id=""totalCash1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCovered & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalCovered1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotal & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalAll1"">0.00</span></div></div></div>")
                    body.Append("</div></div></div></div></div>")

                    ' get general information
                    Dim MaxP, CICov, MICov As Decimal
                    If IsCash = False Then
                        Dim result As String() = getCoverage(PatientNo, CompanyNo)
                        If Left(result(0), 4) <> "Err:" Then
                            MaxP = result(2)
                        Else
                            Return result(0)
                        End If
                        CICov = getTotalClinicInvoices(PatientNo, DoctorNo, TransactionDate)
                        MICov = getTotalPharmacyInvoices(PatientNo, DoctorNo, TransactionDate, lngTransaction, True)
                    Else
                        MaxP = 0
                        CICov = 0
                        MICov = 0
                    End If

                    body.Append("<input type=""hidden"" id=""trans1"" name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter1"" value=""0"" /><input type=""hidden"" id=""coveredCash1"" name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash1"" name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash1"" name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice1"" value=""0"" /><input type=""hidden"" id=""cashOnly1"" name=""cashOnly"" value=""" & CInt(IsCash) & """ /><input type=""hidden"" id=""Limit_1"" value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_1"" value=""" & CICov & """ /><input type=""hidden"" id=""MICov_1"" value=""" & MICov & """ />")

                    ' get invoice items
                    Dim Returned As Boolean
                    Dim CheckBox As String
                    Dim RItem() As String = Split(ReturnItems, ",")
                    dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                    For I = 0 To dsItems.Tables(0).Rows.Count - 1
                        If (Status > 0) And (Cancellation = False) And (Returning = False) And (ReturnedCount = 0) Then
                            CheckBox = "<input type=""checkbox"" class=""chkItem"" value=""" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & """ />"
                        Else
                            CheckBox = ""
                        End If
                        For Each item As String In RItem
                            If item = dsItems.Tables(0).Rows(I).Item("intEntryNumber").ToString Then CheckBox = "<i class=""icon-check red""></i>"
                        Next
                        If CBool(dsItems.Tables(0).Rows(I).Item("bCopied")) = True Then
                            Returned = True
                            CheckBox = "<i class=""icon-cross red""></i>"
                        Else
                            Returned = False
                            CheckBox = ""
                        End If
                        InvoiceItems = InvoiceItems & createItemRow(lngTransaction, 1, IsCash, CheckBox, dsItems.Tables(0).Rows(I).Item("strBarCode").ToString, dsItems.Tables(0).Rows(I).Item("strItem").ToString, dsItems.Tables(0).Rows(I).Item("strItem" & DataLang).ToString, dsItems.Tables(0).Rows(I).Item("byteUnit"), CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")), CDec("0" & dsItems.Tables(0).Rows(I).Item("curBasePrice").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curDiscount").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curQuantity").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curBaseDiscount").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curCoverage").ToString), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False, Returned)
                    Next

                    ' build invoice body
                    body.Append("<div class=""row"">")
                    If IsCash = False Then
                        body.Append("<div class=""col-md-12 insurance1"" id=""divInsurance1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_1"">0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    Else
                        body.Append("<div class=""col-md-12 cash1"" id=""divCash1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_1"">0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    End If
                    body.Append("</div>")
                    'message
                    body.Append(FinalMessage)
                    ' add scripts
                    body.Append("<script type=""text/javascript"">")
                    If IsCash = False Then
                        body.Append("InsuranceOn[1]=false;changeToInsurance(1);calculateInsurance(1);")
                    Else
                        body.Append("cashOn[1]=false;changeToCash(1);calculateCash(1);")
                    End If
                    body.Append("function cancelThis(){cancelInvoice(" & lngTransaction & ");} function requestToCancel(){requestCancelInvoice(" & lngTransaction & ");}")
                    body.Append("function returnThis(){returnItems(" & lngTransaction & ", collectChecked());} function requestToReturn(){requestReturnItems(" & lngTransaction & ", collectChecked());}")
                    body.Append("var itemCount=$('.chkItem').length;var CheckedCount=0; function checkSelectedItems(){if(CheckedCount>0 && CheckedCount!=itemCount) $('#btnReturn').prop('disabled', false); else $('#btnReturn').prop('disabled', true);}checkSelectedItems();")
                    body.Append("var countChecked = function() {var n = $('input:checked').length;CheckedCount=n;checkSelectedItems();};countChecked();$('input[type=checkbox]').on('click', countChecked );")
                    body.Append("function collectChecked(){var str='';jQuery.each( $('input:checked'), function( i, val ) {str += val.value + ','});return str.substring(0, str.length-1);}")
                    body.Append("</script>")

                    Dim ActionDisabled As String = ""
                    Dim CancelButton As String = ""
                    Dim ReturnButton As String = ""
                    Dim ApproveButton As String = ""
                    Dim RejectButton As String = ""
                    'show buttons only if not cancelled or returned
                    If (Status > 0) And (ReturnedCount = 0) Then
                        If Cancellation = True Then
                            If AllowCancel = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveCancelRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectCancelRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        ElseIf Returning = True Then
                            If AllowCancel = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveReturnRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectReturnRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        Else
                            If (DateDiff(DateInterval.Day, InvoiceDate, Today) > CancelLimitDays) Then ActionDisabled = "disabled=""disabled"""
                            If (DirectCancelInvoice = True) Or (AllowCancel = True) Then
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Are you sure to cancel this invoice?', cancelThis)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Are you sure to return selected items?', returnThis)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReturnItem & "</button>"
                            Else
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Do you want to request to cancel this invoice?', requestToCancel)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnReqCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Do you want to request to return selected items?', requestToReturn)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReqReturnItem & "</button>"
                            End If
                        End If
                    End If
                    Dim buttons As String = "<div class=""text-md-center""><a href=""#"" class=""btn btn-blue-grey ml-1 printInvoice""><i class=""icon-printer""></i> " & btnPrint & "</a><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

                    Dim sh As New Share.UI
                    Dim str As String = sh.drawModal(InvoiceDetails, body.ToString, buttons, Share.UI.ModalSize.Large)
                    Return str
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If
    End Function

    Public Function viewInvoice(ByVal lngTransaction As Long) As String
        Dim IsCash, Cancellation, Returning As Boolean
        Dim PatientNo, PatientName, DoctorNo, DoctorName, PharmacistName, CashierName, InvoiceNo, CompanyName, CompanyNo, VoidName As String
        Dim InvoiceDate, TransactionDate, VoidDate As Date
        Dim Status As Byte

        Dim Status_Cancelled, Status_Paid, Status_Unpaid As String

        Dim InvoiceItems As String = ""
        Dim ReturnItems As String = ""
        Dim CancellationMessage, ReturningMessage, ReturnMessage As String
        Dim FinalMessage As String = ""

        Dim InvoiceDetails, InsuranceInvoice, CashInvoice As String
        Dim lblStatus, lblCashier, lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash, lblTotal As String
        Dim lblInvoice As String
        Dim lblCancelBy, lblCancelDate As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colDiscount, colTotal As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim btnReturnItem, btnReqReturnItem, btnCancel, btnReqCancel, btnPrint, btnClose, btnApprove, btnReject As String

        Dim body As New StringBuilder("")
        Dim ds, dsItems As DataSet

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                InvoiceDetails = "تفاصيل الفاتورة"
                InsuranceInvoice = "فاتورة التأمين"
                CashInvoice = "فاتورة النقدي"
                ' Labels
                lblDoctor = "الطبيب"
                lblDate = "التاريخ"
                lblPatient = "المريض"
                lblTotalCovered = "إجمالي المغطى"
                lblTotalCash = "إجمالي النقدي"
                lblPharmacist = "الصيدلي"
                lblInvoice = "فاتورة"
                lblStatus = "الحالة"
                lblCashier = "الصندوق"
                lblTotal = "الإجمالي"
                lblCancelBy = "ألغيت بواسطة"
                lblCancelDate = "ألغيت في"
                ' Columns
                colBarcode = "الباركود"
                colItemNo = "رقم"
                colItemName = "الصنف"
                colAmount = "كم"
                colExpireDate = "الانتهاء"
                colUnitPrice = "السعر"
                colDiscount = "الخصم"
                colTotal = "مجموع"
                ' Buttons
                btnReturnItem = "إعادة صنف"
                btnCancel = "إلغاء الفاتورة"
                btnPrint = "طباعة"
                btnClose = "إغلاق"
                btnReqCancel = "طلب إلغاء الفاتورة"
                btnReqReturnItem = "طلب إعادة صنف"
                btnApprove = "تعميد"
                btnReject = "رفض"
                ' Variables
                InsuranceExtend = "right"
                CashExtend = "left"
                Status_Cancelled = "<span class=""tag tag-sm red"">ملغاة</span>"
                Status_Paid = "<span class=""tag tag-sm blue"">مدفوعة</span>"
                Status_Unpaid = "<span class=""tag tag-sm grey"">غير مدفوعة</span>"
                CancellationMessage = "تم طلب الإلغاء بواسطة @USER في @DATE"
                ReturningMessage = "تم طلب إعادة أصناف بواسطة @USER في @DATE"
                ReturnMessage = " من الأصناف تمت إعادتها"
            Case Else
                DataLang = "En"
                InvoiceDetails = "Invoice Details"
                InsuranceInvoice = "Insurance Invoice"
                CashInvoice = "Cash Invoice"
                ' Labels
                lblDoctor = "Doctor"
                lblDate = "Date"
                lblPatient = "Patient"
                lblTotalCovered = "Total Covered"
                lblTotalCash = "Total Cash"
                lblPharmacist = "Pharmacist"
                lblInvoice = "Invoice"
                lblStatus = "Status"
                lblCashier = "Cashier"
                lblTotal = "Total"
                lblCancelBy = "Cancelled By"
                lblCancelDate = "Cancel Date"
                ' Columns
                colBarcode = "Barcode"
                colItemNo = "No"
                colItemName = "Item Name"
                colAmount = "Qty"
                colExpireDate = "Expire"
                colUnitPrice = "Price"
                colDiscount = "Discount"
                colTotal = "Total"
                ' Buttons
                btnReturnItem = "Return Item"
                btnCancel = "Cancel Invoice"
                btnPrint = "Print"
                btnClose = "Close"
                btnReqCancel = "Request Cancel Invoice"
                btnReqReturnItem = "Request Return Item"
                btnApprove = "Approve"
                btnReject = "Reject"
                ' Variables
                InsuranceExtend = "left"
                CashExtend = "right"
                Status_Cancelled = "<span class=""tag tag-sm red"">Cancelled</span>"
                Status_Paid = "<span class=""tag tag-sm blue"">Paid</span>"
                Status_Unpaid = "<span class=""tag tag-sm grey"">Unpaid</span>"
                CancellationMessage = "Cancellation request made by @USER in @DATE"
                ReturningMessage = "Returning request made by @USER in @DATE"
                ReturnMessage = " Items has returned"
        End Select

        If lngTransaction > 0 Then
            ' Insurance
            Try
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS PharmacistName, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.byteStatus AS [Status] FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND Year(ST.dateTransaction) = 2019 AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    ' get general data
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo")
                    PatientName = ds.Tables(0).Rows(0).Item("PatientName").ToString
                    'PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo")
                    CompanyName = ds.Tables(0).Rows(0).Item("CompanyName").ToString
                    InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo")
                    DoctorName = ds.Tables(0).Rows(0).Item("DoctorName").ToString
                    TransactionDate = ds.Tables(0).Rows(0).Item("TransactionDate")
                    IsCash = ds.Tables(0).Rows(0).Item("bCash")
                    PharmacistName = ds.Tables(0).Rows(0).Item("PharmacistName").ToString
                    CashierName = ds.Tables(0).Rows(0).Item("CashierName").ToString
                    InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    Status = ds.Tables(0).Rows(0).Item("Status")
                    VoidName = ds.Tables(0).Rows(0).Item("VoidName").ToString
                    If IsDBNull(ds.Tables(0).Rows(0).Item("VoidDate")) Then VoidDate = Today Else VoidDate = ds.Tables(0).Rows(0).Item("VoidDate")
                    'UserName = ds.Tables(0).Rows(0).Item("UserName").ToString
                    'CashInvoiceNo = ""
                    'InsuranceInvoiceNo = ""
                    Dim InvoiceStatus As String
                    Select Case Status
                        Case 0
                            InvoiceStatus = Status_Cancelled
                        Case 1
                            InvoiceStatus = Status_Paid
                        Case 2
                            InvoiceStatus = Status_Unpaid
                        Case Else
                            InvoiceStatus = ""
                    End Select

                    Dim dsRItems As DataSet
                    dsRItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND XI.bCopied=1")
                    Dim ReturnedCount As Integer = dsRItems.Tables(0).Rows.Count
                    Cancellation = False
                    Returning = False
                    If Status = 0 Then
                        FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center""><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelBy & ":</div><div class=""col-md-4 teal"">" & VoidName & "</div><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelDate & ":</div><div class=""col-md-4 teal"">" & VoidDate.ToString(strDateFormat & " " & strTimeFormat) & "</div></div></div>"
                    Else
                        Dim dc As New DCL.Conn.XMLData
                        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = True Then
                            Dim doc As New XmlDocument
                            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
                            For Each node As XmlNode In nodes
                                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                                    Cancellation = True
                                    CancellationMessage = Replace(CancellationMessage, "@USER", "<span class=""teal"">" & node.Attributes("user").Value & "</span>")
                                    CancellationMessage = Replace(CancellationMessage, "@DATE", "<span class=""teal"">" & CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value).ToString(strDateFormat & " " & strTimeFormat) & "</span>")
                                    FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & CancellationMessage & "</div></div>"
                                End If
                            Next
                        ElseIf dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = True Then
                            Dim doc As New XmlDocument
                            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items[@status=0]")
                            For Each node As XmlNode In nodes
                                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                                    Returning = True
                                    ReturnItems = node.SelectSingleNode("Items").InnerText
                                    ReturningMessage = Replace(ReturningMessage, "@USER", "<span class=""teal"">" & node.Attributes("user").Value & "</span>")
                                    ReturningMessage = Replace(ReturningMessage, "@DATE", "<span class=""teal"">" & CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value).ToString(strDateFormat & " " & strTimeFormat) & "</span>")
                                    FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReturningMessage & "</div></div>"
                                End If
                            Next
                        Else
                            If ReturnedCount > 0 Then
                                FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReturnedCount & ReturnMessage & "</div></div>"
                            End If
                        End If
                    End If

                    ' build invoice header
                    body.Append("<div class=""row""><div class=""col-md-12""><div class=""card""><div class=""card-body collapse in""><div class=""card-block"" style=""padding:10px; font-size:14px;"">")
                    body.Append("<div class=""col-md-5 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & PatientName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblDoctor & ":</div><div class=""col-md-9 red"">" & DoctorName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblStatus & ":</div><div class=""col-md-9"">" & InvoiceStatus & "</div></div></div>")
                    body.Append("<div class=""col-md-3 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 teal"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblPharmacist & ":</div><div class=""col-md-6 red"">" & PharmacistName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblCashier & ":</div><div class=""col-md-6 red"">" & CashierName & "</div></div></div>")
                    body.Append("<div class=""col-md-4 m-0 p-0""><div class=""col-md-12 p-0""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-5 teal""><span class=""tag tag-sm tag-primary"" style=""width:75px;"" id=""totalCash1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCovered & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalCovered1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotal & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalAll1"">0.00</span></div></div></div>")
                    body.Append("</div></div></div></div></div>")

                    ' get general information
                    Dim MaxP, CICov, MICov As Decimal
                    If IsCash = False Then
                        Dim result As String() = getCoverage(PatientNo, CompanyNo)
                        If Left(result(0), 4) <> "Err:" Then
                            MaxP = result(2)
                        Else
                            Return result(0)
                        End If
                        CICov = getTotalClinicInvoices(PatientNo, DoctorNo, TransactionDate)
                        MICov = getTotalPharmacyInvoices(PatientNo, DoctorNo, TransactionDate, lngTransaction, True)
                    Else
                        MaxP = 0
                        CICov = 0
                        MICov = 0
                    End If

                    body.Append("<input type=""hidden"" id=""trans1"" name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter1"" value=""0"" /><input type=""hidden"" id=""coveredCash1"" name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash1"" name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash1"" name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice1"" value=""0"" /><input type=""hidden"" id=""cashOnly1"" name=""cashOnly"" value=""" & CInt(IsCash) & """ /><input type=""hidden"" id=""Limit_1"" value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_1"" value=""" & CICov & """ /><input type=""hidden"" id=""MICov_1"" value=""" & MICov & """ />")

                    ' get invoice items
                    Dim Returned As Boolean
                    Dim CheckBox As String
                    Dim RItem() As String = Split(ReturnItems, ",")
                    dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                    For I = 0 To dsItems.Tables(0).Rows.Count - 1
                        If (Status > 0) And (Cancellation = False) And (Returning = False) And (ReturnedCount = 0) Then
                            CheckBox = "<input type=""checkbox"" class=""chkItem"" value=""" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & """ />"
                        Else
                            CheckBox = ""
                        End If
                        For Each item As String In RItem
                            If item = dsItems.Tables(0).Rows(I).Item("intEntryNumber").ToString Then CheckBox = "<i class=""icon-check red""></i>"
                        Next
                        If CBool(dsItems.Tables(0).Rows(I).Item("bCopied")) = True Then
                            Returned = True
                            CheckBox = "<i class=""icon-cross red""></i>"
                        Else
                            Returned = False
                        End If
                        InvoiceItems = InvoiceItems & createItemRow(lngTransaction, 1, IsCash, CheckBox, dsItems.Tables(0).Rows(I).Item("strBarCode"), dsItems.Tables(0).Rows(I).Item("strItem"), dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), dsItems.Tables(0).Rows(I).Item("byteUnit"), dsItems.Tables(0).Rows(I).Item("dateExpiry"), dsItems.Tables(0).Rows(I).Item("curBasePrice"), dsItems.Tables(0).Rows(I).Item("curDiscount"), dsItems.Tables(0).Rows(I).Item("curQuantity"), dsItems.Tables(0).Rows(I).Item("curBaseDiscount"), dsItems.Tables(0).Rows(I).Item("curCoverage"), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False, Returned)
                    Next

                    ' build invoice body
                    body.Append("<div class=""row"">")
                    If IsCash = False Then
                        body.Append("<div class=""col-md-12 insurance1"" id=""divInsurance1""><div class=""card border-" & InsuranceColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & InsuranceColor & " lighten-3""><span class=""icon-clipboard4 text-muted""></span> " & InsuranceInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_1"">0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    Else
                        body.Append("<div class=""col-md-12 cash1"" id=""divCash1""><div class=""card border-" & CashColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & CashColor & " lighten-3""><span class=""icon-money text-muted""></span> " & CashInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_1"">0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    End If
                    body.Append("</div>")
                    'message
                    body.Append(FinalMessage)
                    ' add scripts
                    body.Append("<script type=""text/javascript"">")
                    If IsCash = False Then
                        body.Append("InsuranceOn[1]=false;changeToInsurance(1);calculateInsurance(1);")
                    Else
                        body.Append("cashOn[1]=false;changeToCash(1);calculateCash(1);")
                    End If
                    body.Append("function cancelThis(){cancelInvoice(" & lngTransaction & ");} function requestToCancel(){requestCancelInvoice(" & lngTransaction & ");}")
                    body.Append("function returnThis(){returnItems(" & lngTransaction & ", collectChecked());} function requestToReturn(){requestReturnItems(" & lngTransaction & ", collectChecked());}")
                    body.Append("var itemCount=$('.chkItem').length;var CheckedCount=0; function checkSelectedItems(){if(CheckedCount>0 && CheckedCount!=itemCount) $('#btnReturn').prop('disabled', false); else $('#btnReturn').prop('disabled', true);}checkSelectedItems();")
                    body.Append("var countChecked = function() {var n = $('input:checked').length;CheckedCount=n;checkSelectedItems();};countChecked();$('input[type=checkbox]').on('click', countChecked );")
                    body.Append("function collectChecked(){var str='';jQuery.each( $('input:checked'), function( i, val ) {str += val.value + ','});return str.substring(0, str.length-1);}")
                    body.Append("</script>")

                    Dim ActionDisabled As String = ""
                    Dim CancelButton As String = ""
                    Dim ReturnButton As String = ""
                    Dim ApproveButton As String = ""
                    Dim RejectButton As String = ""
                    'show buttons only if not cancelled or returned
                    If (Status > 0) And (ReturnedCount = 0) Then
                        If Cancellation = True Then
                            If AllowCancel = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveCancelRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectCancelRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        ElseIf Returning = True Then
                            If AllowCancel = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveReturnRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectReturnRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        Else
                            If (DateDiff(DateInterval.Day, InvoiceDate, Today) > CancelLimitDays) Then ActionDisabled = "disabled=""disabled"""
                            If (DirectCancelInvoice = True) Or (AllowCancel = True) Then
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Are you sure to cancel this invoice?', cancelThis)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Are you sure to return selected items?', returnThis)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReturnItem & "</button>"
                            Else
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Do you want to request to cancel this invoice?', requestToCancel)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnReqCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Do you want to request to return selected items?', requestToReturn)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReqReturnItem & "</button>"
                            End If
                        End If
                    End If
                    Dim buttons As String = "<div class=""text-md-center"">" & ReturnButton & ApproveButton & CancelButton & RejectButton & "<a href=""p_invoice.aspx?t=" & lngTransaction & """ target=""_blank"" class=""btn btn-blue-grey ml-1 printInvoice""><i class=""icon-printer""></i> " & btnPrint & "</a><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

                    Dim sh As New Share.UI
                    Dim str As String = sh.drawModal(InvoiceDetails, body.ToString, buttons, Share.UI.ModalSize.Large)
                    Return str
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If
    End Function

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
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dc As New DCL.Conn.XMLData
                    If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = False Then
                        Dim res As String = createRequest("Cancel", lngTransaction, "")
                        If Left(res, 4) = "Err:" Then Return res
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

        Return "<script type=""text/javascript"">msg('','You have requested to cancel this invoice successfully!','notice');$('#mdlProcess').modal('hide');updateUI();</script>"

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
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dc As New DCL.Conn.XMLData
                    If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = False Then
                        Dim res As String = createRequest("Return", lngTransaction, lstItems)
                        If Left(res, 4) = "Err:" Then Return res
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

        Return "<script type=""text/javascript"">msg('','You have requested to return items successfully!','notice');$('#mdlProcess').modal('hide');updateUI();</script>"
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
            Return "<script type=""text/javascript"">msg('','The request has been rejected!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
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
            Return "<script type=""text/javascript"">msg('','The request has been rejected!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveCancelRequest(ByVal lngTransaction As Long) As String
        Try
            Dim res As String = cancelInvoice(lngTransaction)
            If Left(res, 4) <> "Err:" Then
                Dim doc As New XmlDocument
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                'get count
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
                Dim count As Integer = nodes.Count
                For Each node As XmlNode In nodes
                    If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "1"
                Next
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                Return "<script type=""text/javascript"">msg('','The request has been appreved!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
            Else
                Return res
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
            Dim items As String = ""
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items[@status=0]")
            For Each node As XmlNode In nodes
                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then items = node.SelectSingleNode("Items").InnerText
            Next
            Dim res As String = returnItems(lngTransaction, items)
            If Left(res, 4) <> "Err:" Then
                For Each node As XmlNode In nodes
                    If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then node.Attributes("status").Value = "1"
                Next
                doc.Save(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                Return "<script type=""text/javascript"">msg('','The request has been appreved!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
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
        End If
        Return ""
    End Function

    Public Function cancelInvoice(ByVal lngTransaction As Long) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=0 WHERE lngTransaction=" & lngTransaction)
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strVoidBy='" & strUserName & "',dateVoid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "' WHERE lngTransaction=" & lngTransaction)
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','This invoice cancelled successfully!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
    End Function

    Public Function returnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet
        Dim dsItems As DataSet
        Dim items() As String = Split(lstItems, ",")
        'Validation
        If items.Length = 0 Then Return "Err:No items selected.."
        dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
        If dsItems.Tables(0).Rows.Count = 0 Then Return "Err:No items in this invoice.."
        If dsItems.Tables(0).Rows.Count <= items.Length Then Return "Err:Cannot return all items, please cancel the invoice.."

        'For I = 0 To dsItems.Tables(0).Rows.Count - 1
        '    'InvoiceItems = InvoiceItems & "<tr id=""tr_1"" class=""Ctr""><td style=""width:32px;""><input type=""hidden"" name=""barcode_C"" value=""" & dsItems.Tables(0).Rows(I).Item("strBarCode") & """/><input type=""hidden"" name=""dose_C"" value=""""/><input type=""hidden"" name=""item_C"" class=""item_C"" value=""" & dsItems.Tables(0).Rows(I).Item("strItem") & """/></td><td style=""width:70px;"" class=""dynCash"">" & dsItems.Tables(0).Rows(I).Item("strItem") & "</td><td class=""itemName width-150"" title=""" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td><td style=""width:100px;"" class=""dynCash red"">" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_C"" value=""" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""price_C"" class=""price_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""service_C"" value=""" & dsItems.Tables(0).Rows(I).Item("intService") & """/><input type=""hidden"" name=""warehouse_C"" value=""" & dsItems.Tables(0).Rows(I).Item("byteWarehouse") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""percent_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curDiscount"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""discount"" name=""discount_C"" class=""discount_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td style=""width:44px;"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""quantity"" name=""quantity_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""unit_C"" value=""" & dsItems.Tables(0).Rows(I).Item("byteUnit") & """/></td><td style=""width:80px;"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""total_C"" class=""total_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & Math.Round(0, byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td class=""text-nowrap""></td></tr>"
        '    InvoiceItems = InvoiceItems & createItemRow(lngTransaction, 1, IsCash, "<input type=""checkbox"" class=""chkItem"" value=""" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & """ />", dsItems.Tables(0).Rows(I).Item("strBarCode"), dsItems.Tables(0).Rows(I).Item("strItem"), dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), dsItems.Tables(0).Rows(I).Item("byteUnit"), dsItems.Tables(0).Rows(I).Item("dateExpiry"), dsItems.Tables(0).Rows(I).Item("curBasePrice"), dsItems.Tables(0).Rows(I).Item("curDiscount"), dsItems.Tables(0).Rows(I).Item("curQuantity"), dsItems.Tables(0).Rows(I).Item("curBaseDiscount"), dsItems.Tables(0).Rows(I).Item("curCoverage"), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False)
        'Next

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * CancelLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    ' Get XLink
                    Dim dsXLink, dsTransType, dsSelectedItems As DataSet
                    Dim lngXlink, lngTransaction_New, lngXling_New As Long
                    dsXLink = dcl.GetDS("SELECT * FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction)
                    lngXlink = dsXLink.Tables(0).Rows(0).Item("lngXlink")
                    ' Get byteTransType
                    Dim byteTransType As Byte
                    dsTransType = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteBase=18")
                    byteTransType = dsTransType.Tables(0).Rows(0).Item("byteTransType")
                    ' Get ReturnDate and created user
                    Dim dateReturn As Date = Now()
                    Dim strCreateBy As String = strUserName
                    Dim doc As New XmlDocument
                    doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                    Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
                    For Each node As XmlNode In nodes
                        If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                            dateReturn = CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value)
                            strCreateBy = node.Attributes("user").Value
                        End If
                    Next
                    ' Create a return invoice (byteDepartment & lngSalesman has been added)
                    lngTransaction_New = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, dateTransaction, lngContact, byteStatus, bCash, dateClosedValid, byteCurrency, byteWarehouse, lngPatient, strRemarks, byteDepartment, lngSalesman) VALUES (18, " & byteTransType & ",'" & ds.Tables(0).Rows(0).Item("strTransaction") & "', '" & dateReturn.ToString("yyyy-MM-dd") & "' , " & ds.Tables(0).Rows(0).Item("lngContact") & ", 1, " & CInt(ds.Tables(0).Rows(0).Item("bCash")) & ", '" & Today.ToString("yyyy-MM-dd") & "', " & byteLocalCurrency & ", 3, " & ds.Tables(0).Rows(0).Item("lngPatient") & ", '" & ds.Tables(0).Rows(0).Item("strRemarks") & "', " & ds.Tables(0).Rows(0).Item("byteDepartment") & ", " & ds.Tables(0).Rows(0).Item("lngSalesman") & ")")
                    ' Create xlink pointer
                    lngXling_New = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction, lngPointer) VALUES (" & lngTransaction_New & ", " & lngTransaction & ")")
                    ' Add returned items to the return invoice
                    dsSelectedItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND intEntryNumber IN (" & lstItems & ")")
                    For I = 0 To dsSelectedItems.Tables(0).Rows.Count - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,curBasePrice,curDiscount,curUnitPrice,curCoverage,dateExpiry,byteDepartment,intService,byteWarehouse) VALUES (" & lngXling_New & "," & dsSelectedItems.Tables(0).Rows(I).Item("intEntryNumber") & ",'" & dsSelectedItems.Tables(0).Rows(I).Item("strItem") & "'," & dsSelectedItems.Tables(0).Rows(I).Item("byteUnit") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curQuantity") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curBasePrice") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curDiscount") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curUnitPrice") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curCoverage") & "," & dsSelectedItems.Tables(0).Rows(I).Item("dateExpiry") & "," & dsSelectedItems.Tables(0).Rows(I).Item("byteDepartment") & "," & dsSelectedItems.Tables(0).Rows(I).Item("intService") & ",3)")
                        dcl.ExecSQuery("UPDATE Stock_Xlink_Items SET bCopied=1 WHERE lngXlink=" & lngXlink & " AND intEntryNumber=" & dsSelectedItems.Tables(0).Rows(I).Item("intEntryNumber"))
                    Next
                    ' insert the audit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated, strApprovedBy, dateApproved, strCashBy, dateCash) VALUES (" & lngTransaction_New & ", '" & strCreateBy & "', '" & dateReturn.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & strUserName & "', '" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & strUserName & "', '" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "')")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','The selected items returned successfully!','notice');$('#mdlProcess').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
    End Function

    Public Function getItemInfo(ByVal strBarcode As String, ByVal lngTransaction As Long, ByVal curCoverage As Decimal, ByVal curBasePriceTotal As Decimal, ByVal RowCounter As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String) As String
        Dim ds As DataSet
        Dim Ret As String = ""
        Dim btnPrint, btnDelete, btnMove As String
        Dim bApproval As Boolean
        Dim strItem, strItemName, strDose As String
        Dim curBasePrice, curUnitPrice As String
        Dim dateExpiry As String
        Dim curQuantity, curOrderQuantity As Decimal
        Dim curBaseDiscount As Decimal
        Dim thisCoverage As Decimal
        Dim lngPatient As Long
        Dim dateTransaction As Date
        Dim strReference As String
        Dim bCash As Boolean
        Dim itemFlag As String = ""
        Dim bytePriceType, intService As Integer
        Dim byteWarehouse As Byte
        Dim intGroup As Integer
        Dim confirm As Boolean = False 'enabled this to popup a confirm message to complete this proccess, required (confirm_text and script)
        Dim confirm_text As String = ""
        Dim Script As String = "" ' add here any script that will run after this process finished
        Dim lngContact, lngSalesman As Long
        Dim lngContract As Long
        Dim byteScheme, bytePrimaryDep, byteUnit As Byte
        Dim curDiscount As Decimal
        Dim bPercentValue As Boolean
        Dim bTax As Boolean
        Dim curTax, curVAT, curVATI As Decimal

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                btnPrint = "طباعة"
                btnDelete = "حذف"
                btnMove = "نقدي"
            Case Else
                DataLang = "En"
                btnPrint = "Print"
                btnDelete = "Delete"
                btnMove = "Cash"
        End Select

        '0====> Get Transaction Information
        If lngTransaction > 0 Then
            ' Insurance
            ds = dcl.GetDS("SELECT HC.bytePriceType,* FROM Stock_Trans AS ST INNER JOIN Hw_Contacts AS HC ON ST.lngContact = HC.lngContact WHERE lngTransaction = " & lngTransaction)
            lngPatient = ds.Tables(0).Rows(0).Item("lngPatient")
            dateTransaction = ds.Tables(0).Rows(0).Item("dateTransaction")
            strReference = ds.Tables(0).Rows(0).Item("strReference").ToString
            bytePriceType = ds.Tables(0).Rows(0).Item("bytePriceType")
            lngContact = ds.Tables(0).Rows(0).Item("lngContact")
            lngSalesman = ds.Tables(0).Rows(0).Item("lngSalesman")
            bCash = ds.Tables(0).Rows(0).Item("bCash")
        Else
            ' Cash
            lngPatient = 16
            dateTransaction = Today
            strReference = ""
            bytePriceType = 0
            lngContact = 27
            lngSalesman = 395
            bCash = True
        End If

        If strBarcode.Length = 5 Then Return "<script type=""text/javascript"">completeBarcode('" & strBarcode & "');</script>"
        '1====> Filter Barcode
        Dim returnedArray As String() = FilterBarcode(strBarcode)
        If Left(returnedArray(0), 4) = "Err:" Then
            Return returnedArray(0)
        End If

        '2====> Correct Expiry Date and Base Price
        Try
            strItem = returnedArray(0)
            curBasePrice = CDec(returnedArray(1))
            dateExpiry = CDate(returnedArray(2))
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        '4====> Get Item General Information
        Dim dsGInfo As DataSet
        dsGInfo = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem LEFT JOIN Hw_Department_Items AS HDI ON SI.strItem = HDI.strItem AND HDI.byteDepartment = 15 LEFT JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment WHERE SI.strItem='" & strItem & "'")
        strItemName = dsGInfo.Tables(0).Rows(0).Item("strItem" & DataLang)
        intGroup = dsGInfo.Tables(0).Rows(0).Item("intGroup")
        byteWarehouse = dsGInfo.Tables(0).Rows(0).Item("byteWarehouse")
        byteUnit = dsGInfo.Tables(0).Rows(0).Item("byteUnit")
        intService = dsGInfo.Tables(0).Rows(0).Item("intService")
        bTax = dsGInfo.Tables(0).Rows(0).Item("bTax")
        curTax = dsGInfo.Tables(0).Rows(0).Item("curTax")

        '4====> Get Item Medical Information
        Dim dsMInfo As DataSet
        dsMInfo = dcl.GetDS("SELECT * FROM Hw_Treatments_Pharmacy WHERE lngPatient=" & lngPatient & " AND strReference='" & strReference & "' AND strItem='" & strItem & "'")
        If dsMInfo.Tables(0).Rows.Count > 0 Then
            strDose = dsMInfo.Tables(0).Rows(0).Item("strDose").ToString
            curOrderQuantity = dsMInfo.Tables(0).Rows(0).Item("curQuantity").ToString
        Else
            strDose = "0000000000"
            curOrderQuantity = 1
        End If

        '4====> Get Insurance Information
        If bCash = True Then
            'Cash
            lngContract = 0
            byteScheme = 0
            curBaseDiscount = 0
        Else
            'Insurance
            ds = dcl.GetDS("SELECT * FROM Ins_Contracts WHERE lngGuarantor = " & lngContact)
            If ds.Tables(0).Rows.Count > 0 Then
                ds.Clear()
                ds = dcl.GetDS("SELECT HP.bytePrimaryDep, HP.lngGuarantor, IC.lngContract, IC.byteScheme, IC.curDeductionValueP, IC.curDeductionPercentP, IC.curDeductionValueD, IC.curDeductionPercentD FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & lngPatient)
                If ds.Tables(0).Rows.Count > 0 Then
                    lngContract = ds.Tables(0).Rows(0).Item("lngContract")
                    byteScheme = ds.Tables(0).Rows(0).Item("byteScheme")
                    bytePrimaryDep = ds.Tables(0).Rows(0).Item("bytePrimaryDep")
                    If bytePrimaryDep = 1 Then
                        If IsDBNull(ds.Tables(0).Rows(0).Item("curDeductionValueP")) Then curBaseDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curDeductionPercentP")) Else curBaseDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curDeductionValueP"))
                        bPercentValue = IsDBNull(ds.Tables(0).Rows(0).Item("curDeductionValueP"))
                    Else
                        If IsDBNull(ds.Tables(0).Rows(0).Item("curDeductionValueD")) Then curBaseDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curDeductionPercentD")) Else curBaseDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curDeductionValueD"))
                        bPercentValue = IsDBNull(ds.Tables(0).Rows(0).Item("curDeductionValueD"))
                    End If
                    'Must Insert this to sync with old software
                    'CurrentDb.Execute "INSERT INTO Stock_Trans_Insurance (lngTransaction, lngContact, lngContract, byteScheme, bytePrimaryDep, curCoverage, bPercentValue) VALUES (" & [lngTransaction] & ", " & rs!lngGuarantor & ", " & rs!lngContract & ", " & rs!byteScheme & ", " & Nz(rs!bytePrimaryDep, 1) & ", " & [sfrm1]![curCoverage] & "," & [sfrm1]![bPercentValue] & ")"
                Else
                    Return "Err: Credit invoice,Complete insurance information"
                End If
            Else
                Return "Err: There are no contracts with this company."
            End If
        End If

        '2===> Get Quantity (Insurance = From Doctor, Cash = 1) Depend on settings
        If OneQuantityPerItem = True Then
            curQuantity = 1
        Else
            curQuantity = curOrderQuantity
        End If

        '2===>
        If checkStock(strItem, curQuantity, dateTransaction, byteWarehouse, SelectedInsuranceItems, SelectedCashItems) = False Then
            Return "Err:No balance of this item."
        End If

        '3====> Check Approvals
        If bCash = True Then
            'Cash
            bApproval = True
            bCash = True ' convert to Cash invoice
        Else
            'Insurance
            Dim result As Integer = getItemApprovalStatus(strItem, lngPatient, dateTransaction, strReference)
            If result < 0 Then
                'send errors and exit
                Select Case result
                    Case -1
                        Return "Err:intVisit not found"
                    Case -2
                        Return "Err:no record"
                    Case -3
                        'Return "Err:not approved or rejected yet"
                        bCash = True
                        bApproval = True
                        confirm_text = "Order is not approved or rejected yet, do you want to move it to cash invoice?"
                        confirm = True
                End Select
            Else
                Select Case result
                    Case 0
                        itemFlag = "<i class=""icon-cross danger"" data-toggle=""tooltip"" data-delay=""500"" data-original-title=""Rejected"" title=""Rejected""></i>"
                        bApproval = False
                        bCash = True ' convert to Cash invoice
                        If AutoMoveRejectedToCash_Insurance = True Then
                            Script = "msg('','Order is rejected, moved to cash!','notice')"
                        Else
                            confirm_text = "Order is rejected, do you want to move it to cash invoice?"
                            confirm = True
                        End If
                    Case 1
                        itemFlag = "<i class=""icon-checkmark success"" data-toggle=""tooltip"" data-delay=""500"" data-original-title=""Approved"" title=""Approved""></i>"
                        bApproval = True
                        bCash = False ' convert to Insurance invoice
                    Case Else
                        itemFlag = ""
                        bApproval = False
                        bCash = True ' convert to Cash invoice
                End Select
            End If
        End If

        '2.1===> Stop Duplicate if required
        If AllowExtraItem_Insurance = False And bCash = False Then
            If checkDuplicateItem(curOrderQuantity, strItem, SelectedInsuranceItems) = False Then
                bApproval = False
                bCash = True
                If AutoMoveExtraToCash_Insurance = False Then
                    confirm_text = "Quantity for this item is " & Math.Round(curOrderQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<br>Move it to cash?"
                    confirm = True
                End If
            End If
        End If

        '5====> Get Price
        curBasePrice = getPrice(curBasePrice, strItem)
        'curUnitPrice = curBaseDiscount

        '6====> Get Discount (Contact Discount=> Insurance) Or (Promotion Discount=> Cash)
        curDiscount = getDiscount(bCash, strItem, bytePriceType, intGroup, lngPatient, dateTransaction)

        '7====> Get Insurance Coverage
        'Dim thisPrice, thisDiscount, PatientCash, InsuranceCoverage As Decimal
        If bCash = True Then
            'Cash
            thisCoverage = 0
            'thisPrice = curBasePrice
            'PatientCash = curBasePrice
            'InsuranceCoverage = 0
            'thisDiscount = curBaseDiscount
        Else
            'Insurance
            If bApproval = True Then
                curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
                Dim re As String = CheckItemCoverage(lngTransaction, lngContract, byteScheme, strItem, curBasePrice, curQuantity, dateTransaction, lngPatient, curBasePriceTotal)
                If Left(re, 4) = "Err:" Then
                    Return re
                Else
                    thisCoverage = Math.Round(CalculateCoverage(lngTransaction, strItem, curUnitPrice, curQuantity, curCoverage, lngContract, byteScheme, bytePriceType, bytePrimaryDep, lngPatient, lngSalesman, dateTransaction, intGroup), byteCurrencyRound, MidpointRounding.AwayFromZero)
                    'thisPrice = curBasePrice
                    'PatientCash = thisCoverage
                    'InsuranceCoverage = curBasePrice - thisCoverage
                    'thisDiscount = InsuranceCoverage
                End If
            End If
        End If

        '7====> Get Tax (VAT)
        If bTax = False Then curTax = 0

        '7====> Build record
        Dim Quantity, Discount, ExpireDate, Typ, Func As String
        If bCash = True Then
            Typ = "C"
            Func = "calculateCash"
            If ChangeQuantity_Cash = True Then Quantity = "<input type=""text"" class=""form-control input-xs width-10 text-md-center"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """ /><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>" Else Quantity = curQuantity & "<input type=""hidden"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """/><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>"
            'If AddDiscount_Cash = True Then Discount = "<input type=""text"" class=""form-control form-filter input-xs width-10 text-md-center"" name=""discount"" value=""" & thisDiscount & """ />" Else Discount = thisDiscount
        Else
            Typ = "I"
            Func = "calculateInsurance"
            If ChangeQuantity_Insurance = True Then Quantity = "<input type=""text"" class=""form-control input-xs width-10 text-md-center"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """ /><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>" Else Quantity = curQuantity & "<input type=""hidden"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """/><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>"
            'If AddDiscount_Insurance = True Then Discount = "<input type=""text"" class=""form-control form-filter input-xs width-50 text-md-center"" name=""discount"" value=""" & thisDiscount & """><div class=""form-control-position""><i class=""icon-percent""></i></div>" Else Discount = thisDiscount
        End If

        'If dateExpiry <= Today Then ExpireDate = "<span class=""tag tag-danger tag-xs"">" & CDate(dateExpiry).ToString(strDateFormat) & "</span>" Else ExpireDate = CDate(dateExpiry).ToString(strDateFormat)
        If dateExpiry <= Today Then
            Return "Err:Item Expired"
            'Else
            '    If dateExpiry <= DateAdd(DateInterval.Month, 3, Today) Then
            '        ExpireDate = "<span class=""tag tag-danger tag-xs"">" & CDate(dateExpiry).ToString(strDateFormat) & "</span><input type=""hidden"" name=""expire_" & Typ & """ value=""" & CDate(dateExpiry).ToString(strDateFormat) & """/>"
            '    Else
            '        ExpireDate = CDate(dateExpiry).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_" & Typ & """ value=""" & CDate(dateExpiry).ToString(strDateFormat) & """/>"
            '    End If
        End If

        Ret = Ret & "<script type=""text/javascript"">"

        'Ret = Ret & "function addThis(){"
        'If bCash = False Then
        '    Ret = Ret & "var item = '<tr id=""tr_" & RowCounter & """ class=""Itr""><td style=""width:32px;"">" & itemFlag & "<input type=""hidden"" name=""barcode_" & Typ & """ value=""" & strBarcode & """/><input type=""hidden"" name=""dose_" & Typ & """ value=""" & strDose & """/><input type=""hidden"" name=""item_" & Typ & """ class=""item_" & Typ & """ value=""" & strItem & """/></td><td style=""width:70px;"" class=""dynInsurance"">" & strItem & "</td><td class=""itemName width-150"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px;"" class=""dynInsurance red"">" & ExpireDate & "</td><td style=""width:80px;"" class=""dynInsurance"">" & curBasePrice & "<input type=""hidden"" id=""price"" name=""price_" & Typ & """ class=""price_" & Typ & """ value=""" & curBasePrice & """/><input type=""hidden"" name=""service_" & Typ & """ value=""" & intService & """/><input type=""hidden"" name=""warehouse_" & Typ & """ value=""" & byteWarehouse & """/></td><td style=""width:80px;"" class=""dynInsurance"">" & Discount & "<input type=""hidden"" name=""percent_" & Typ & """ value=""" & curBaseDiscount & """/><input type=""hidden"" id=""discount"" name=""discount_" & Typ & """ class=""discount_" & Typ & """ value=""" & thisDiscount & """/></td><td style=""width:44px;"">" & Quantity & "</td><td style=""width:80px;"">" & PatientCash & "<input type=""hidden"" id=""total"" name=""total_" & Typ & """ class=""total_" & Typ & """ value=""" & PatientCash & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & InsuranceCoverage & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeIItems(this);removeThis(this);" & Func & "(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a> <a href=""javascript:"" onclick=""javascript:moveThis(this);" & Func & "(curTab);"" class=""tag btn-info btn-lighten-3 tag-xs dynInsurance"">" & btnMove & "</a></td></tr>';"
        '    Ret = Ret & "$('#tblInsurance' + curTab + ' > tbody:last-child').append(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);InsuranceOn[curTab]=!(InsuranceOn[curTab]);changeToInsurance(curTab);calculateInsurance(curTab);$('#items_I_' + curTab).val($('#items_I_' + curTab).val()+'" & strItem & ",');"
        'Else
        '    Ret = Ret & "var item = '<tr id=""tr_" & RowCounter & """ class=""Ctr""><td style=""width:32px;"">" & itemFlag & "<input type=""hidden"" name=""barcode_" & Typ & """ value=""" & strBarcode & """/><input type=""hidden"" name=""dose_" & Typ & """ value=""" & strDose & """/><input type=""hidden"" name=""item_" & Typ & """ class=""item_" & Typ & """ value=""" & strItem & """/></td><td style=""width:70px;"" class=""dynCash"">" & strItem & "</td><td class=""itemName width-150"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px;"" class=""dynCash red"">" & ExpireDate & "</td><td style=""width:80px;"" class=""dynCash"">" & curBasePrice & "<input type=""hidden"" id=""price"" name=""price_" & Typ & """ class=""price_" & Typ & """ value=""" & curBasePrice & """/><input type=""hidden"" name=""service_" & Typ & """ value=""" & intService & """/><input type=""hidden"" name=""warehouse_" & Typ & """ value=""" & byteWarehouse & """/></td><td style=""width:80px;"" class=""dynCash"">" & Discount & "<input type=""hidden"" name=""percent_" & Typ & """ value=""" & curBaseDiscount & """/><input type=""hidden"" id=""discount"" name=""discount_" & Typ & """ class=""discount_" & Typ & """ value=""" & thisDiscount & """/></td><td style=""width:44px;"">" & Quantity & "</td><td style=""width:80px;"">" & PatientCash & "<input type=""hidden"" id=""total"" name=""total_" & Typ & """ class=""total_" & Typ & """ value=""" & PatientCash & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & InsuranceCoverage & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeCItems(this);removeThis(this);" & Func & "(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a></td></tr>';"
        '    Ret = Ret & "$('#tblCash' + curTab + ' > tbody:last-child').append(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);cashOn[curTab]=!(cashOn[curTab]);changeToCash(curTab);calculateCash(curTab);$('#items_C_' + curTab).val($('#items_C_' + curTab).val()+'" & strItem & ",');"
        'End If
        'Ret = Ret & "}"

        Ret = Ret & "function addThis(){"
        If bCash = False Then
            Ret = Ret & "var item = '" & createItemRow(lngTransaction, RowCounter, False, itemFlag, strBarcode, strItem, strItemName, byteUnit, dateExpiry, curBasePrice, curDiscount, curQuantity, curBaseDiscount, thisCoverage, curTax, intService, byteWarehouse, strDose, True, False, True) & "';"
            Ret = Ret & "$('#tblInsurance' + curTab + ' > tbody:last-child').append(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);InsuranceOn[curTab]=!(InsuranceOn[curTab]);changeToInsurance(curTab);calculateInsurance(curTab);$('#items_I_' + curTab).val($('#items_I_' + curTab).val()+'" & strItem & ",');"
        Else
            Ret = Ret & "var item = '" & createItemRow(lngTransaction, RowCounter, True, itemFlag, strBarcode, strItem, strItemName, byteUnit, dateExpiry, curBasePrice, curDiscount, curQuantity, curBaseDiscount, thisCoverage, curTax, intService, byteWarehouse, strDose, True, False, True) & "';"
            Ret = Ret & "$('#tblCash' + curTab + ' > tbody:last-child').append(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);cashOn[curTab]=!(cashOn[curTab]);changeToCash(curTab);calculateCash(curTab);$('#items_C_' + curTab).val($('#items_C_' + curTab).val()+'" & strItem & ",');"
        End If
        Ret = Ret & "refreshListeners();createPrintDoseLink(curTab);" & Script
        Ret = Ret & "}"

        If confirm = True Then Ret = Ret & "confirm('','" & confirm_text & "', addThis);" Else Ret = Ret & "addThis();"
        Ret = Ret & "</script>"
        Return Ret
    End Function

    Public Function completeBarcode(ByVal strBarcode As String) As String
        Dim str As String = ""
        Dim strItem As String
        Dim ExpireDate As String
        Dim Price As String
        Dim ds As DataSet

        Dim Header As String
        Dim btnSave, btnClose As String
        Dim lblExpiryDate, lblBasePrice As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "تاريخ الإنتهاء والسعر"
                lblExpiryDate = "تاريخ الإنتهاء"
                lblBasePrice = "سعر الصنف"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Expire Date & Price"
                lblExpiryDate = "Expire Date"
                lblBasePrice = "Item Price"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        strItem = strBarcode
        ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            Dim dsTemp As DataSet
            dsTemp = dcl.GetDS("SELECT MAX(curBasePrice) AS curBasePrice, MAX(dateExpiry) AS dateExpiry FROM Stock_Xlink_Items WHERE strItem='" & strItem & "' AND dateExpiry > GETDATE() AND curBasePrice IS NOT NULL AND curBasePrice > 0")
            If dsTemp.Tables(0).Rows.Count > 0 Then
                If Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("curBasePrice"))) And Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("dateExpiry"))) Then
                    Price = CDec(dsTemp.Tables(0).Rows(0).Item("curBasePrice")).ToString("F2")
                    ExpireDate = CDate(dsTemp.Tables(0).Rows(0).Item("dateExpiry")).ToString("yyyy-MM")
                Else
                    Return "Err:item not defined"
                End If
            Else
                Return "Err:item not defined"
            End If
        Else
            Return "Err:item not defined"
        End If

        str = str & "<div class=""row pl-1 pr-1"">"
        str = str & "<div class=""form-group""><label>" & lblExpiryDate & ":</label><input type=""month"" id=""dateExpiry"" class=""form-control"" value=""" & ExpireDate & """></div>"
        str = str & "<div class=""form-group""><label>" & lblBasePrice & ":</label><input class=""form-control"" id=""curBasePrice"" type=""number"" value=""" & Price & """ /></div>"
        str = str & "</div>"
        str = str & "<script type=""text/javascript"">"
        str = str & "var eYear=$('#dateExpiry').val().substr(2,2); var eMonth=$('#dateExpiry').val().substr(5,2);"
        str = str & "var iPrice=pad(parseFloat($('#curBasePrice').val()).toFixed(2).replace('.',''),6);"
        str = str & "function completeIt(){var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; getItemInfo(newbarcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val()); $('#mdlMessage').modal('hide');}"
        str = str & "</script>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" class=""btn btn-success ml-1"" onclick=""javascript:completeIt();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.Small, "bg-grey bg-lighten-2")
    End Function

    Private Function createItemRow(ByVal lngTransaction As Long, ByVal Counter As Integer, ByVal IsCash As Boolean, ByVal Flag As String, ByVal strBarcode As String, ByVal strItem As String, ByVal strItemName As String, ByVal byteUnit As Byte, ByVal dateExpiry As Date, ByVal curBasePrice As Decimal, ByVal curDiscount As Decimal, ByVal curQuantity As Decimal, ByVal curBaseDiscount As Decimal, ByVal curCoverage As Decimal, ByVal curVAT As Decimal, ByVal intService As Integer, ByVal byteWarehouse As Byte, ByVal strDose As String, ByVal Editable As Boolean, Optional ByVal Returned As Boolean = False, Optional ByVal AutoPrintDose As Boolean = False) As String
        Dim item As String
        Dim btnPrint, btnDelete, btnMove As String
        Dim Typ, Cls, Func, ExpireDate, moveButton, printButton, allButtons As String
        Dim curTotal, curUnitPrice As Decimal

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                btnPrint = "طباعة"
                btnDelete = "حذف"
                btnMove = "نقدي"
            Case Else
                DataLang = "En"
                btnPrint = "Print"
                btnDelete = "Delete"
                btnMove = "Cash"
        End Select

        If IsCash = False Then
            Typ = "I"
            Cls = "dynInsurance"
            Func = "calculateInsurance"
            curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curTotal = curCoverage
            curBaseDiscount = curBaseDiscount
            moveButton = "<button type=""button"" onclick=""javascript:moveThis(this);" & Func & "(curTab);"" class=""btn btn-info btn-lighten-3 btn-xs " & Cls & """>" & btnMove & "</button>"
        Else
            Typ = "C"
            Cls = "dynCash"
            Func = "calculateCash"
            curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curTotal = curUnitPrice
            curBaseDiscount = 0
            moveButton = ""
        End If

        If dateExpiry <= DateAdd(DateInterval.Month, 3, Today) Then
            ExpireDate = "<span class=""tag tag-danger tag-xs"">" & CDate(dateExpiry).ToString(strDateFormat) & "</span><input type=""hidden"" name=""expire_" & Typ & """ class=""expire"" value=""" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """/>"
        Else
            ExpireDate = CDate(dateExpiry).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_" & Typ & """ class=""expire"" value=""" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """/>"
        End If

        If (lngTransaction > 0) And ((PrintDose = 2) Or (PrintDose = 3)) And (strDose <> "0000000000") And (strDose <> "") Then
            printButton = "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-url=""p_dose.aspx?t=" & lngTransaction & "&i=" & strItem & "&e=" & dateExpiry.ToString("yyyy-MM-dd") & """><button type=""button"" class=""btn btn-blue-grey btn-xs printDose"">" & btnPrint & "</button></span>"
        Else
            printButton = ""
        End If

        Dim AutoPrint As String = ""
        If (AutoPrintDose = True) And ((PrintDose = 1) Or (PrintDose = 3)) And (strDose <> "0000000000") And (strDose <> "") Then AutoPrint = "<i><span id=""directPrint"" app-url=""p_dose.aspx?t=" & lngTransaction & "&i=" & strItem & "&e=" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """></span></i>"

        If Editable = True Then allButtons = printButton & " <button type=""button"" onclick=""javascript:remove" & Typ & "Items(this);removeThis(this);" & Func & "(curTab);"" class=""btn btn-red btn-lighten-3 btn-xs"">" & btnDelete & "</button> " & moveButton & AutoPrint Else allButtons = """"
        Dim DelStart, DelEnd, Lit As String
        If Returned = True Then
            DelStart = "<del>"
            DelEnd = "</del>"
            Lit = "bg-grey bg-lighten-4"
        Else
            DelStart = ""
            DelEnd = ""
            Lit = ""
        End If
        'add row
        item = "<tr id=""tr_" & Counter & """ class=""" & Typ & "tr " & Lit & """>"
        'add flag + barcode + item + dose + unit
        item = item & "<td style=""width:32px;"">" & Flag & "<input type=""hidden"" name=""barcode_" & Typ & """ value=""" & strBarcode & """/><input type=""hidden"" name=""dose_" & Typ & """ class=""dose"" value=""" & strDose & """/><input type=""hidden"" name=""item_" & Typ & """ class=""item"" value=""" & strItem & """/><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/></td>"
        'add item
        item = item & "<td style=""width:70px;"" class=""" & Cls & """>" & DelStart & strItem & DelEnd & "</td>"
        'add item name
        item = item & "<td class=""itemName width-150"" title=""" & strItemName & """>" & DelStart & strItemName & DelEnd & "</td>"
        'add expire date
        item = item & "<td style=""width:100px;"" class=""" & Cls & " red"">" & ExpireDate & "</td>"
        'add baseprice + service + warehouse
        item = item & "<td style=""width:80px;"" class=""" & Cls & """>" & Math.Round(curBasePrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""baseprice_" & Typ & """ class=""price_" & Typ & """ value=""" & curBasePrice & """/><input type=""hidden"" name=""service_" & Typ & """ value=""" & intService & """/><input type=""hidden"" name=""warehouse_" & Typ & """ value=""" & byteWarehouse & """/></td>"
        'add unitprice + unitdicsount
        item = item & "<td style=""width:80px;"" class=""" & Cls & """>" & Math.Round(curDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & " %<input type=""hidden"" name=""basediscount_" & Typ & """ value=""" & curBaseDiscount & """/><input type=""hidden"" id=""discount"" name=""discount_" & Typ & """ class=""discount_" & Typ & """ value=""" & curDiscount & """/></td>"
        'add quantity
        item = item & "<td style=""width:44px;"">" & Math.Round(curQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""quantity_" & Typ & """ value=""" & curQuantity & """/></td>"
        'add total + coverage + vat
        item = item & "<td style=""width:80px;"">" & Math.Round(curUnitPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""unitprice_" & Typ & """ class=""total_" & Typ & """ value=""" & curUnitPrice & """/><input type=""hidden"" id=""coverage_" & Typ & """ name=""coverage_" & Typ & """ class=""coverage_" & Typ & """ value=""" & curCoverage & """/><input type=""hidden"" class=""vat_" & Typ & """ name=""vat_" & Typ & """ value=""" & curVAT & """/></td>"
        'add buttons
        item = item & "<td class=""text-nowrap"">" & allButtons & "</td>"
        'close row
        item = item & "</tr>"

        Return item
    End Function

    Private Class InvoiceItem
        Dim _Barcode, _Item, _Dose As String
        Dim _BasePrice, _UnitPrice, _Discount, _BaseDiscount, _Quantity, _Coverage, _VAT As Decimal
        Dim _Service, _Unit, _Warehouse As Integer
        Dim _Expire As Date
        Property Barcode As String
            Set(value As String)
                _Barcode = value
            End Set
            Get
                Return _Barcode
            End Get
        End Property
        Property Item As String
            Set(value As String)
                _Item = value
            End Set
            Get
                Return _Item
            End Get
        End Property
        Property Expire As Date
            Set(value As Date)
                _Expire = value
            End Set
            Get
                Return _Expire
            End Get
        End Property
        Property Service As Integer
            Set(value As Integer)
                _Service = value
            End Set
            Get
                Return _Service
            End Get
        End Property
        Property Warehouse As Byte
            Set(value As Byte)
                _Warehouse = value
            End Set
            Get
                Return _Warehouse
            End Get
        End Property
        Property BasePrice As Decimal
            Set(value As Decimal)
                _BasePrice = value
            End Set
            Get
                Return _BasePrice
            End Get
        End Property
        Property Discount As Decimal
            Set(value As Decimal)
                _Discount = value
            End Set
            Get
                Return _Discount
            End Get
        End Property
        Property UnitPrice As Decimal
            Set(value As Decimal)
                _UnitPrice = value
            End Set
            Get
                Return _UnitPrice
            End Get
        End Property
        Property BaseDiscount As Decimal
            Set(value As Decimal)
                _BaseDiscount = value
            End Set
            Get
                Return _BaseDiscount
            End Get
        End Property
        Property Quantity As Decimal
            Set(value As Decimal)
                _Quantity = value
            End Set
            Get
                Return _Quantity
            End Get
        End Property
        Property Unit As Integer
            Set(value As Integer)
                _Unit = value
            End Set
            Get
                Return _Unit
            End Get
        End Property
        Property Coverage As Decimal
            Set(value As Decimal)
                _Coverage = value
            End Set
            Get
                Return _Coverage
            End Get
        End Property
        Property VAT As Decimal
            Set(value As Decimal)
                _VAT = value
            End Set
            Get
                Return _VAT
            End Get
        End Property
        Property Dose As String
            Set(value As String)
                _Dose = value
            End Set
            Get
                Return _Dose
            End Get
        End Property

        Sub New(ByVal strBarcode As String, ByVal strItem As String, ByVal dateExpire As Date, ByVal intService As Integer, ByVal byteWarehouse As Integer, ByVal curBasePrice As Decimal, ByVal curDiscount As Decimal, ByVal curUnitPrice As Decimal, ByVal curQuantity As Decimal, ByVal byteUnit As Byte, ByVal curBaseDiscount As Decimal, ByVal curCoverage As Decimal, ByVal curVAT As Decimal, ByVal strDose As String)
            Me._Barcode = strBarcode
            Me._Item = strItem
            Me._Expire = dateExpire
            Me._Service = intService
            Me._Warehouse = byteWarehouse
            Me._BasePrice = curBasePrice
            Me._Discount = curDiscount
            Me._UnitPrice = curUnitPrice
            Me._Quantity = curQuantity
            Me._Unit = byteUnit
            Me._BaseDiscount = curBaseDiscount
            Me._Coverage = curCoverage
            Me._VAT = curVAT
            Me._Dose = strDose
        End Sub

    End Class

    Private Function createCashBox(ByVal strPatientName As String, ByVal curAmount As Decimal, ByVal IsCashier As Boolean) As String()
        Dim body As New StringBuilder("")
        Dim btnPayment, btnCash, btnCredit, btnSplit, btnJoin, btnCancel As String
        Dim lblPatient, lblCash, lblPaid, lblRemind, lblCredit, lblAmount, btnCalculator As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'labels
                lblPatient = "المريض"
                lblAmount = "المبلغ"
                lblCash = "النقدي"
                lblPaid = "المدفوع"
                lblRemind = "المتبقي"
                lblCredit = "الشبكة"
                'buttons
                btnCalculator = "الحاسبة"
                btnPayment = "الدفع"
                btnCash = "نقدي"
                btnCredit = "بطاقة بنكية"
                btnSplit = "تقسيم الدفع"
                btnJoin = "ضم الدفع"
                btnCancel = "إلغاء الأمر"
            Case Else
                DataLang = "En"
                'labels
                btnCalculator = "Calculator"
                lblPatient = "Patient"
                lblAmount = "Amount"
                lblCash = "Cash"
                lblPaid = "Paid"
                lblRemind = "Remind"
                lblCredit = "SPAN"
                'buttons
                btnPayment = "Payment"
                btnCash = "Cash"
                btnCredit = "Credit Card"
                btnSplit = "Split Payment"
                btnJoin = "Join Payment"
                btnCancel = "Cancel"
        End Select

        body.Append("<div class=""row""><div class=""col-md-12""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & strPatientName & "</div></div><div class=""col-md-12""><hr /></div><div class=""col-md-6"">")
        'left part
        body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblAmount & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_total"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""" & curAmount & """ /></div></div>")
        body.Append("<div class=""col-md-12"" id=""divTotalPaid""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblPaid & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_paid"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12"" id=""divTotalCash""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblCredit & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_credit"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12"" id=""divTotalCredit""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblCash & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_cash"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label lass=""col-form-label""><h5 class=""text-md-right"">" & lblRemind & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_remind"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""0"" /></div></div>")

        body.Append("</div><div class=""col-md-6"">")
        'right part
        body.Append("<table style=""width:100%""><tr><td colspan=""3""><button type=""button"" class=""btn btn-teal col-md-12"">" & btnCalculator & "</button></td></tr><tr><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">5</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">10</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">50</button></td></tr><tr><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">100</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">200</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">500</button></td></tr><tr><td colspan=""3""><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">" & curAmount & "</button></td></tr></table>")
        body.Append("</div></div>")

        Dim btns As String
        If OnePaymentForCashier = True Then
            btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment"" onclick=""javascript:if(validatePayment()) updatePayment(3);""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        Else
            btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment"" onclick=""javascript:if(validatePayment()) updatePayment(3);""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCash"" onclick=""javascript:if(validatePayment()) updatePayment(1);""><i class=""icon-cash""></i> " & btnCash & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCredit"" onclick=""javascript:if(validatePayment()) updatePayment(2);""><i class=""icon-credit-card""></i> " & btnCredit & "</button><button type=""button"" class=""btn btn-primary ml-1"" id=""btnSplit""><i class=""icon-calculator""></i> " & btnSplit & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        End If
        'btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCash""><i class=""icon-cash""></i> " & btnCash & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCredit""><i class=""icon-credit-card""></i> " & btnCredit & "</button><button type=""button"" class=""btn btn-primary ml-1"" id=""btnSplit""><i class=""icon-calculator""></i> " & btnSplit & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        body.Append("<script type=""text/javascript"">")
        If OnePaymentForCashier = True Then body.Append("var onePayment = true; var byTotal = false;") Else body.Append("var onePayment = false; var byTotal = false;")
        body.Append("btnSplit='<i class=""icon-calculator""></i> " & btnSplit & "';btnJoin='<i class=""icon-calculator""></i> " & btnJoin & "';")
        body.Append("$('#net_paid').on('change paste keyup', calcRemind);$('#net_cash').on('change paste keyup', calcRemind);$('#net_credit').on('change paste keyup', calcRemind);$('.calcNum').click(function () {if (onePayment == true) $('#net_paid').val($(this).text());  else { if (byTotal == true) $('#net_paid').val($(this).text()); else $('#net_cash').val($(this).text());}calcRemind();});$('#btnSplit').click(changePayment);changePayment();")
        If ForcePaymentOnCloseInvoice = True Then body.Append("function validatePayment(){if($('#net_remind').val()<=0) return true; else return false;}") Else body.Append("function validatePayment(){return true}")
        body.Append("</script>")
        Return {body.ToString, btns}
    End Function

    Public Function viewCashier(ByVal lngTransaction As Long) As String
        Dim ds As DataSet
        Dim lngPatient As Long
        Dim coveredCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim bCreatCash As Boolean
        Dim strPatientName As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        'analize data and verify
        If lngTransaction > 0 Then
            ' Insurance
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus,bCreatCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND ST.lngTransaction = " & lngTransaction)
            lngPatient = ds.Tables(0).Rows(0).Item("PatientNo")
            strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
            Dim dsTemp, dsCovered, dsNonCovered As DataSet
            dsCovered = dcl.GetDS("SELECT SUM(XI.curCoverage) AS Total FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction)
            coveredCash = CDec("0" & dsCovered.Tables(0).Rows(0).Item("Total").ToString)
            If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
            If bCreatCash = True Then
                Dim lngXlink As Long = 0
                dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & ds.Tables(0).Rows(0).Item("ClinicInvoiceNo") & "' AND lngPatient=" & lngPatient & " AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND bSubCash=1")
                If dsTemp.Tables(0).Rows.Count > 0 Then
                    dsNonCovered = dcl.GetDS("SELECT SUM(XI.curCoverage) AS Total FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & dsTemp.Tables(0).Rows(0).Item("lngTransaction"))
                    nonCoveredCash = CDec("0" & dsNonCovered.Tables(0).Rows(0).Item("Total").ToString)
                Else
                    nonCoveredCash = 0
                End If
            Else
                nonCoveredCash = 0
            End If
        Else
            Return "Err:"
        End If
        TotalCashAmount = Math.Round((coveredCash + nonCoveredCash), byteCurrencyRound, MidpointRounding.AwayFromZero)

        'Dim source As String
        Dim res As String() = createCashBox(strPatientName, TotalCashAmount, True)
        Dim body As String = res(0)
        Dim btns As String = res(1)
        'If OnePaymentForCashier Then source = "$('#net_total').val()" Else source = "" ' ====> for later work
        Dim script As String = "<script type=""text/javascript"">function updatePayment(type){getPaid1(" & lngTransaction & ",$('#net_total').val(),$('#net_credit').val(),type)};</script>"

        Dim mdl As New Share.UI
        Return mdl.drawModal("Cashier", body & script, btns, Share.UI.ModalSize.Medium, "bg-grey bg-lighten-3", "", "text-md-center")
    End Function

    Public Function viewCashier(ByVal TabCounter As Integer, ByVal Fields As String) As String
        Dim ds As DataSet
        Dim lngTransaction, lngPatient As Long
        Dim coveredCash, deductionCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim cashOnly As Boolean
        Dim strPatientName As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "lngTransaction"
                    lngTransaction = field(I).value()
                Case "cashOnly"
                    cashOnly = field(I).value()
                Case "coveredCash"
                    coveredCash = field(I).value()
                Case "deductionCash"
                    deductionCash = field(I).value()
                Case "nonCoveredCash"
                    nonCoveredCash = field(I).value()
            End Select
        Next

        'analize data and verify
        TotalCashAmount = coveredCash + nonCoveredCash
        If lngTransaction > 0 Then
            ' Insurance
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
            lngPatient = ds.Tables(0).Rows(0).Item("PatientNo")
            strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
        Else
            ' Cash
            lngPatient = 16
            ds = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(strFirstEn,'') + ' ') + LTRIM(ISNULL(strSecondEn,'') + ' ') + LTRIM(ISNULL(strThirdEn,'') + ' ') + LTRIM(ISNULL(strLastEn,'') + ' ')) AS PatientName,* FROM Hw_Patients WHERE lngPatient=" & lngPatient)
            strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
        End If

        Dim res As String() = createCashBox(strPatientName, TotalCashAmount, False)
        Dim body As String = res(0)
        Dim btns As String = res(1)
        Dim script As String = "<script type=""text/javascript"">function updatePayment(type){getPaid2(" & TabCounter & ",'" & Replace(Fields, """", "|") & "',$('#net_total').val(),$('#net_credit').val(),type)};</script>"

        Dim mdl As New Share.UI
        Return mdl.drawModal("Cashier", body & script, btns, Share.UI.ModalSize.Medium, "bg-grey bg-lighten-3", "", "text-md-center")
    End Function

    Public Function GetPaid(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim coveredCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim CashItem, InsuranceItem As Integer
        Dim bCreatCash As Boolean
        Dim lngTransaction_2 As Long = 0 'For the second invoice if any

        If lngTransaction > 0 Then
            ' Insurance
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus,bCreatCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.lngTransaction = " & lngTransaction)
            Dim dsTemp, dsCovered, dsNonCovered As DataSet
            dsCovered = dcl.GetDS("SELECT SUM(XI.curCoverage) AS Total, COUNT(XI.lngXlink) AS ItemCount FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction)
            coveredCash = dsCovered.Tables(0).Rows(0).Item("Total")
            InsuranceItem = dsCovered.Tables(0).Rows(0).Item("ItemCount")
            If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
            If bCreatCash = True Then
                Dim lngXlink As Long = 0
                dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & ds.Tables(0).Rows(0).Item("ClinicInvoiceNo") & "' AND lngPatient=" & ds.Tables(0).Rows(0).Item("PatientNo") & " AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND bSubCash=1")
                lngTransaction_2 = dsTemp.Tables(0).Rows(0).Item("lngTransaction")
                dsNonCovered = dcl.GetDS("SELECT SUM(XI.curCoverage) AS Total, COUNT(XI.lngXlink) AS ItemCount FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction_2)
                nonCoveredCash = dsNonCovered.Tables(0).Rows(0).Item("Total")
                CashItem = dsNonCovered.Tables(0).Rows(0).Item("ItemCount")
            Else
                nonCoveredCash = 0
            End If
        Else
            Return "Err:"
        End If
        TotalCashAmount = Math.Round((coveredCash + nonCoveredCash), byteCurrencyRound, MidpointRounding.AwayFromZero)

        'validate payment with invoice
        If CashItem + InsuranceItem = 0 Then Return "Err:No items in this invoice!"
        If TotalCashAmount <> (coveredCash + nonCoveredCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        If TotalCashAmount < (P_SPAN + P_Cash) Then Return "Err:Payment dose not match the invoice total!"
        ' Payment type
        '==> here must decide if the payment is CASH or SPAN or BOTH (do it later)
        'convert trans to invoice
        Dim res As String = closeInvoice(lngTransaction)
        If Left(res, 4) = "Err:" Then Return res

        Return "<script type=""text/javascript"">$('#mdlMessage').modal('hide');$('#large').modal('hide');$('#row" & lngTransaction & "').remove();msg('','Invoice close successfully!','info');" & res & "</script>"
    End Function

    Public Function GetPaid(ByVal tabCounter As Integer, ByVal Fields As String, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim ds As DataSet
        Dim body As New StringBuilder("")
        Dim lngTransaction As Long
        Dim coveredCash, deductionCash, nonCoveredCash As Decimal
        Dim cashOnly As Boolean

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        'if sales is cashier in same time don't skip cashier
        Dim result As String = ""
        'If SkipCashier = False Then
        '    result = SendToCashier(Fields, True)
        '    If Left(result, 4) = "Err:" Then Return result
        'End If

        ' get form values
        Fields = Replace(Fields, "|", """")

        result = SendToCashier(Fields, True)
        If Left(result, 4) = "Err:" Then Return result

        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "lngTransaction"
                    lngTransaction = field(I).value()
                Case "cashOnly"
                    cashOnly = field(I).value()
                Case "coveredCash"
                    coveredCash = field(I).value()
                Case "deductionCash"
                    deductionCash = field(I).value()
                Case "nonCoveredCash"
                    nonCoveredCash = field(I).value()
            End Select
        Next
        If lngTransaction = -1 Then lngTransaction = CLng("0" & result)

        ' Collect items
        Dim Items_Insurance, Items_Cash As New List(Of InvoiceItem)
        Dim Total_Insurance, Total_Cash, Total_All As Decimal
        If cashOnly = False Then
            Items_Insurance = getInvoiceItems(Fields, False)
            For Each item As InvoiceItem In Items_Insurance
                Total_Insurance = Total_Insurance + item.UnitPrice
            Next
        End If
        Items_Cash = getInvoiceItems(Fields, True)
        For Each item As InvoiceItem In Items_Cash
            Total_Cash = Total_Cash + item.UnitPrice
        Next
        Total_All = Total_Insurance + Total_Cash
        'validate payment with invoice
        If Items_Cash.Count + Items_Insurance.Count = 0 Then Return "Err:No items in this invoice!"
        If Total_All <> (coveredCash + nonCoveredCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        If Total_All < (P_SPAN + P_Cash) Then Return "Err:Payment dose not match the invoice total!"
        ' Payment type
        '==> here must decide if the payment is CASH or SPAN or BOTH (do it later)
        'convert trans to invoice
        Dim res As String = closeInvoice(lngTransaction)
        If Left(res, 4) = "Err:" Then Return res

        Return "<script type=""text/javascript"">$('#mdlMessage').modal('hide');$('#large').modal('hide');$('#row" & lngTransaction & "').remove();msg('','Invoice close successfully!','info');" & res & "</script>"
    End Function

    Private Function createPrintBox(ByVal lngTransaction As Long, ByVal lngTransaction_2 As Long) As String
        Dim Header, btnPrintInvoice, btnPrintCash, btnPrintInsurance, btnClose As String
        Dim strBox As String = ""

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "طباعة الفاتورة.."
                btnPrintInvoice = "'طباعة"
                btnPrintCash = "فاتورة نقدي"
                btnPrintInsurance = "فاتورة آجل"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Print Invoice.."
                btnPrintInvoice = "Print"
                btnPrintCash = "Cash Invoice"
                btnPrintInsurance = "Credit Invoice"
                btnClose = "Close"
        End Select

        strBox = strBox & "<div class=""row""><div class=""col-md-12 text-md-center"">"
        If lngTransaction_2 = 0 Then
            strBox = strBox & "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & InvoicePrinter & """ app-url=""p_invoice.aspx?t=" & lngTransaction & """><button type=""button"" id=""zag"" class=""btn btn-success mr-1"" onclick=""javascript:printInvoice(this);""><i class=""icon-print""></i> " & btnPrintInvoice & "</button></span>"
        Else
            strBox = strBox & "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & InvoicePrinter & """ app-url=""p_invoice.aspx?t=" & lngTransaction & """><button type=""button"" id=""zag"" class=""btn btn-success mr-1"" onclick=""javascript:printInvoice(this);""><i class=""icon-print""></i> " & btnPrintInsurance & "</button></span> <span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & InvoicePrinter & """ app-url=""p_invoice.aspx?t=" & lngTransaction_2 & """><button type=""button"" class=""btn btn-success mr-1"" onclick=""javascript:printInvoice(this);""><i class=""icon-print""></i> " & btnPrintCash & "</button></span>"
        End If
        strBox = strBox & "<button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"
        strBox = strBox & "</div></div>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, strBox, "", Share.UI.ModalSize.Medium, "bg-grey bg-lighten-2 mt-4")
    End Function

    Private Function closeInvoice(ByVal lngTransaction As Long) As String
        If lngTransaction > 0 Then
            Dim lngTransaction_2 As Long = 0
            Try
                ' Get last invoice number
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = 2019 AND byteBase = 40")
                Dim strTransaction As String = CLng(dsLast.Tables(0).Rows(0).Item("LastNo")) + 1
                ' Get byteTransType
                Dim dsTransType As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteBase = 40")
                Dim byteTransType As Byte = dsTransType.Tables(0).Rows(0).Item("byteTransType")
                ' Update transaction
                dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction & "', byteBase=40, byteTransType=" & byteTransType & ", dateClosedValid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1 WHERE lngTransaction = " & lngTransaction)
                ' Update Audit
                Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction)
                If dsAudit.Tables(0).Rows.Count > 0 Then
                    Dim CreateSQL As String = ""
                    Dim LastSaveSQL As String = ""
                    Dim ApproveSQL As String = ""
                    Dim CashSQL As String = ""
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCreatedBy")) Then CreateSQL = ",strCreatedBy='" & strUserName & "', dateCreated='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strLastSavedBy")) Then LastSaveSQL = ",strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strApprovedBy")) Then ApproveSQL = ",strApprovedBy='" & strUserName & "', dateApproved='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction)
                Else
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                End If
                'Get Related Cash Invoice
                Dim ds As DataSet
                Dim bCreatCash As Boolean
                ds = dcl.GetDS("SELECT * FROM Stock_Trans WHERE lngTransaction=" & lngTransaction)
                If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
                If bCreatCash = True Then
                    Dim dsTemp As DataSet
                    dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & ds.Tables(0).Rows(0).Item("strReference") & "' AND lngPatient=" & ds.Tables(0).Rows(0).Item("lngPatient") & " AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "' AND bSubCash=1")
                    If dsTemp.Tables(0).Rows.Count > 0 Then
                        lngTransaction_2 = dsTemp.Tables(0).Rows(0).Item("lngTransaction")
                        ' Get last invoice number
                        Dim dsLast2 As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = 2019 AND byteBase = 40")
                        Dim strTransaction2 As String = CLng(dsLast2.Tables(0).Rows(0).Item("LastNo")) + 1
                        ' Update transaction
                        dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction2 & "', byteBase=40, byteTransType=" & byteTransType & ", dateClosedValid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1 WHERE lngTransaction = " & lngTransaction_2)
                        ' Update Audit
                        Dim dsAudit2 As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction_2)
                        If dsAudit2.Tables(0).Rows.Count > 0 Then
                            Dim CreateSQL As String = ""
                            Dim LastSaveSQL As String = ""
                            Dim ApproveSQL As String = ""
                            Dim CashSQL As String = ""
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCreatedBy")) Then CreateSQL = ",strCreatedBy='" & strUserName & "', dateCreated='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strLastSavedBy")) Then LastSaveSQL = ",strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strApprovedBy")) Then ApproveSQL = ",strApprovedBy='" & strUserName & "', dateApproved='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction_2 & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction_2)
                        Else
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction_2 & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                        End If
                    End If
                End If

                Dim PrintScript As String = "$('#mdlSales').modal('hide');"
                Select Case PrintInvoice
                    Case 0 'No Print
                        ' Nothing
                    Case 1 'Auto Print
                        PrintScript = ""
                    Case 2 'Ask To Print
                        PrintScript = "$('#mdlSales').html('" & createPrintBox(lngTransaction, lngTransaction_2) & "');$('#mdlSales').modal('show');"
                    Case 3 'User Defined
                        '
                End Select
                Return PrintScript
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err: Transaction Lost"
        End If
    End Function

    Public Function ReturnToSales(ByVal lngTransaction As Long) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        If lngTransaction > 0 Then
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Try
                    Dim lngTranslink As Long
                    Dim dsTranslink As DataSet

                    dsTranslink = dcl.GetDS("SELECT * FROM Stock_Trans WHERE byteBase=50 AND bSubCash=1 AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "' AND strReference='" & ds.Tables(0).Rows(0).Item("strReference").ToString & "'")
                    If dsTranslink.Tables(0).Rows.Count > 0 Then lngTranslink = dsTranslink.Tables(0).Rows(0).Item("lngTransaction") Else lngTranslink = 0
                    dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus = 1, bCollected1 = 1,bApproved1 = 0 WHERE lngTransaction=" & lngTransaction)
                    dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus = 1, bCollected1 = 1,bApproved1 = 0 WHERE lngTransaction=" & lngTranslink)
                Catch ex As Exception
                    Return "Err:Cannot return this invoice"
                End Try
            Else
                Return "Err:This record is unavailable, please refresh the orders again.."
            End If
        Else
            Return "Err:There is no invoice to return.."
        End If
        Return "<script type=""text/javascript"">msg('','Invoice has been returned to sales!','notice');$('#row" & lngTransaction & "').remove();</script>"
    End Function

    Public Function SendToCashier(ByVal Fields As String, Optional ForPayment As Boolean = False) As String
        Dim ds As DataSet
        Dim body As New StringBuilder("")
        Dim lngTransaction, returnTransaction As Long
        Dim coveredCash, deductionCash, nonCoveredCash As Decimal
        Dim cashOnly As Boolean

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "lngTransaction"
                    lngTransaction = field(I).value()
                Case "cashOnly"
                    cashOnly = field(I).value()
                Case "coveredCash"
                    coveredCash = field(I).value()
                Case "deductionCash"
                    deductionCash = field(I).value()
                Case "nonCoveredCash"
                    nonCoveredCash = field(I).value()
            End Select
        Next
        ' Collect items
        Dim Items_Insurance, Items_Cash As New List(Of InvoiceItem)
        Dim Total_Insurance, Total_Cash, Total_All As Decimal
        If cashOnly = False Then
            Items_Insurance = getInvoiceItems(Fields, False)
            For Each item As InvoiceItem In Items_Insurance
                Total_Insurance = Total_Insurance + item.UnitPrice
            Next
        End If
        Items_Cash = getInvoiceItems(Fields, True)
        For Each item As InvoiceItem In Items_Cash
            Total_Cash = Total_Cash + item.UnitPrice
        Next
        Total_All = Total_Insurance + Total_Cash

        'validateion
        If Items_Cash.Count + Items_Insurance.Count = 0 Then Return "Err:No items in this invoice!"
        If Total_All <> (coveredCash + nonCoveredCash + deductionCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"

        'send to cashier
        If lngTransaction > 0 Then
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                returnTransaction = lngTransaction

                'get Transaction info
                Dim PatientName As String = ds.Tables(0).Rows(0).Item("PatientName").ToString
                Dim InvoiceNo As String = ds.Tables(0).Rows(0).Item("ClinicInvoiceNo").ToString
                Dim DoctorNo As Long = ds.Tables(0).Rows(0).Item("DoctorNo").ToString
                Dim CompanyNo As Long = ds.Tables(0).Rows(0).Item("CompanyNo").ToString
                Dim DepartmentNo As Long = ds.Tables(0).Rows(0).Item("DepartmentNo").ToString
                Dim PatientNo As Long = ds.Tables(0).Rows(0).Item("PatientNo").ToString
                Dim TransactionDate As Date = ds.Tables(0).Rows(0).Item("TransactionDate")

                'insurance
                If Items_Insurance.Count > 0 Then
                    Try
                        ' update transaction
                        Dim bCreateCash As String
                        If Items_Cash.Count > 0 Then bCreateCash = ",bCreatCash=1" Else bCreateCash = ""
                        dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus=2, bCollected1 = 1, bApproved1 = 1 " & bCreateCash & " WHERE lngTransaction=" & lngTransaction)
                        ' insert audit
                        Dim dsAudit As DataSet
                        dsAudit = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction)
                        If dsAudit.Tables(0).Rows.Count > 0 Then
                            dcl.ExecScalar("UPDATE Stock_Trans_Audit SET strCreatedBy='" & strUserName & "', dateCreated='" & Today.ToString("yyyy-MM-dd HH:mm") & "' WHERE lngTransaction=" & lngTransaction)
                        Else
                            dcl.ExecScalar("INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & lngTransaction & ", '" & strUserName & "', '" & Today.ToString("yyyy-MM-dd HH:mm") & "')")
                        End If
                        ' insert xlink
                        Dim lngXlink As Long
                        Dim dsXlink As DataSet
                        dsXlink = dcl.GetDS("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction)
                        If dsXlink.Tables(0).Rows.Count > 0 Then
                            lngXlink = dsXlink.Tables(0).Rows(0).Item("lngXlink")
                        Else
                            lngXlink = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransaction & "," & lngTransaction & ")")
                        End If
                        'insert xlink items
                        dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlink)
                        Dim intEntryNumber As Integer = 1
                        For Each item As InvoiceItem In Items_Insurance
                            'Dim dsXlinkItem As DataSet
                            'dsXlinkItem = dcl.GetDS("SELECT * FROM Stock_Xlink_Items WHERE lngXlink = (SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction & ") AND strItem='" & item.Item & "'")
                            'If dsXlinkItem.Tables(0).Rows.Count > 0 Then
                            'dcl.ExecScalar("UPDATE Stock_Xlink_Items SET bApproval=1 WHERE lngXlink=" & lngXlink & " AND strItem='" & item.Item & "'")
                            'Else
                            dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1)")
                            'End If
                            intEntryNumber = intEntryNumber + 1
                        Next
                    Catch ex As Exception
                        Return "Err:" & ex.Message
                    End Try
                End If
                'Cash
                If Items_Cash.Count > 0 Then
                    Try
                        ' find any related transaction or insert a new transaction 
                        Dim dsTrans As DataSet
                        Dim lngTransaction_New As Long
                        If cashOnly = True Then
                            lngTransaction_New = lngTransaction
                            dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus=2, bCollected1=1, bApproved1=1 WHERE lngTransaction=" & lngTransaction_New)
                        Else
                            dsTrans = dcl.GetDS("SELECT * FROM Stock_Trans WHERE byteBase=50 AND bSubCash=1 AND dateTransaction='" & TransactionDate.ToString("yyyy-MM-dd") & "' AND strReference='" & InvoiceNo & "'")
                            If dsTrans.Tables(0).Rows.Count > 0 Then
                                ' get transaction no
                                lngTransaction_New = dsTrans.Tables(0).Rows(0).Item("lngTransaction")
                                dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus=2, bCollected1=1, bApproved1=1 WHERE lngTransaction=" & lngTransaction_New)
                            Else
                                ' get MAX strTransaction
                                Dim LastTrans As Long
                                ds = dcl.GetDS("SELECT Max(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction)=YEAR(GETDATE()) AND byteTransType=20 AND byteBase=40")
                                LastTrans = ds.Tables(0).Rows(0).Item("Last") + 1
                                ' insert a new transaction 
                                lngTransaction_New = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, strUserPrint, bCash, bSubCash, dateEntry) VALUES (50, 20,'" & LastTrans & "', " & byteDepartment_Cash & " , " & lngContact_Cash & ", '" & Today.ToString("yyyy-MM-dd") & "', 2, " & byteLocalCurrency & ", " & lngSalesman_Cash & ", " & PatientNo & ", '" & PatientName & "','" & InvoiceNo & "', 1, 1, '" & strUserName & "',1,1,'" & Today.ToString("yyyy-MM-dd") & "')")
                            End If
                        End If
                        ' insert audit
                        Dim dsAudit As DataSet
                        dsAudit = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction_New)
                        If dsAudit.Tables(0).Rows.Count > 0 Then
                            dcl.ExecScalar("UPDATE Stock_Trans_Audit SET strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd HH:mm") & "' WHERE lngTransaction=" & lngTransaction_New)
                        Else
                            dcl.ExecScalar("INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & lngTransaction_New & ", '" & strUserName & "', '" & Today.ToString("yyyy-MM-dd HH:mm") & "')")
                        End If
                        ' insert xlink
                        Dim lngXlink As Long
                        Dim dsXlink As DataSet
                        dsXlink = dcl.GetDS("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction_New)
                        If dsXlink.Tables(0).Rows.Count > 0 Then
                            lngXlink = dsXlink.Tables(0).Rows(0).Item("lngXlink")
                        Else
                            lngXlink = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransaction_New & "," & lngTransaction & ")")
                        End If
                        'insert xlink items
                        'remove all items
                        dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlink)
                        Dim intEntryNumber As Integer = 1
                        For Each item As InvoiceItem In Items_Cash
                            'find item and update it, or add a new one
                            'Dim dsXlinkItem As DataSet
                            'dsXlinkItem = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE lngTransaction=" & lngTransaction_New & " AND XI.strItem='" & item.Item & "'")
                            'If dsXlinkItem.Tables(0).Rows.Count > 0 Then
                            'dcl.ExecScalar("UPDATE Stock_Xlink_Items SET intEntryNumber=" & intEntryNumber & ", curQuantity=" & item.Quantity & ", dateExpiry='" & item.Expire.ToString("yyyy-MM-dd") & "', curBasePrice=" & item.BasePrice & ", curUnitPrice=" & item.UnitPrice & ", curUnitNetPrice=NULL, curDiscount=" & item.Discount & ", curCoverage=" & item.Coverage & ", curBaseDiscount=" & item.Discount & ", bApproval=1 WHERE lngXlink=" & lngXlink & " AND strItem='" & item.Item & "'")
                            'Else
                            dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1)")
                            'End If
                            intEntryNumber = intEntryNumber + 1
                        Next
                    Catch ex As Exception
                        Return "Err:" & ex.Message
                    End Try
                End If
                returnTransaction = lngTransaction '<=====
            Else
                Return "Err:This record is unavailable, please refresh the orders again.."
            End If
        Else
            If Items_Cash.Count > 0 Then
                Try
                    ' get MAX strTransaction
                    Dim LastTrans As Long
                    ds = dcl.GetDS("SELECT Max(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction)=YEAR(GETDATE()) AND byteTransType=20 AND byteBase=40")
                    LastTrans = ds.Tables(0).Rows(0).Item("Last") + 1
                    ' insert a transaction
                    Dim lngTransaction_New As Long
                    lngTransaction_New = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1,bCash,dateEntry) VALUES (50, 20,'" & LastTrans & "', " & byteDepartment_Cash & " , " & lngContact_Cash & ", '" & Today.ToString("yyyy-MM-dd") & "', 2, " & byteLocalCurrency & ", " & lngSalesman_Cash & ", " & lngPatient_Cash & ", '" & strPatient_Cash & "',NULL, 1, 1,1,'" & Today.ToString("yyyy-MM-dd") & "')")
                    ' insert audit
                    Dim dsAudit As DataSet
                    dsAudit = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction_New)
                    If dsAudit.Tables(0).Rows.Count > 0 Then
                        dcl.ExecScalar("UPDATE Stock_Trans_Audit SET strCreatedBy='" & strUserName & "', dateCreated='" & Today.ToString("yyyy-MM-dd HH:mm") & "' WHERE lngTransaction=" & lngTransaction_New)
                    Else
                        dcl.ExecScalar("INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & lngTransaction_New & ", '" & strUserName & "', '" & Today.ToString("yyyy-MM-dd HH:mm") & "')")
                    End If
                    ' insert xlink
                    Dim lngXlink As Long
                    Dim dsXlink As DataSet
                    dsXlink = dcl.GetDS("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction_New)
                    If dsXlink.Tables(0).Rows.Count > 0 Then
                        lngXlink = dsXlink.Tables(0).Rows(0).Item("lngXlink")
                    Else
                        lngXlink = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransaction_New & "," & lngTransaction_New & ")")
                    End If
                    'insert xlink items
                    'remove all items
                    dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlink)
                    Dim intEntryNumber As Integer = 1
                    For Each item As InvoiceItem In Items_Cash
                        'Dim dsXlinkItem As DataSet
                        'dsXlinkItem = dcl.GetDS("SELECT * FROM Stock_Xlink_Items WHERE lngXlink = (SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction_New & ") AND strItem='" & item.Item & "'")
                        'If dsXlinkItem.Tables(0).Rows.Count > 0 Then
                        'dcl.ExecScalar("UPDATE Stock_Xlink_Items SET bApproval=1 WHERE lngXlink=" & lngXlink & " AND strItem='" & item.Item & "'")
                        'Else
                        dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & byteDepartment_Cash & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1)")
                        'End If
                        intEntryNumber = intEntryNumber + 1
                    Next
                    returnTransaction = lngTransaction_New '<=====
                Catch ex As Exception
                    Return "Err:" & ex.Message
                End Try
            End If
        End If
        If ForPayment = True Then
            Return returnTransaction
        Else
            Return "<script type=""text/javascript"">msg('','Invoice has been sent to cashier!','notice');$('#row" & lngTransaction & "').remove();</script>"
        End If
    End Function

    Private Function getInvoiceItems(ByVal Fields As String, ByVal CashInvoice As Boolean) As List(Of InvoiceItem)
        Dim BasePrice_I, BasePrice_C, Discount_I, Discount_C, UnitPrice_I, UnitPrice_C, Item_I, Item_C, Barcode_I, Barcode_C, BaseDiscount_I, BaseDiscount_C, Quantity_I, Quantity_C, Coverage_I, Coverage_C, Unit_I, Unit_C, Service_I, Service_C, Expire_I, Expire_C, Dose_I, Dose_C, Warehouse_I, Warehouse_C, VAT_I, VAT_C As String

        Barcode_I = ""
        Barcode_C = ""
        Item_I = ""
        Item_C = ""
        BasePrice_I = ""
        BasePrice_C = ""
        Discount_I = ""
        Discount_C = ""
        UnitPrice_I = ""
        UnitPrice_C = ""
        BaseDiscount_I = ""
        BaseDiscount_C = ""
        Quantity_I = ""
        Quantity_C = ""
        Coverage_I = ""
        Coverage_C = ""
        Unit_I = ""
        Unit_C = ""
        Service_I = ""
        Service_C = ""
        Expire_I = ""
        Expire_C = ""
        Dose_I = ""
        Dose_C = ""
        Warehouse_I = ""
        Warehouse_C = ""
        VAT_I = ""
        VAT_C = ""
        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "baseprice_I"
                    BasePrice_I = BasePrice_I & field(I).value() & ","
                Case "baseprice_C"
                    BasePrice_C = BasePrice_C & field(I).value() & ","
                Case "item_I"
                    Item_I = Item_I & field(I).value() & ","
                Case "item_C"
                    Item_C = Item_C & field(I).value() & ","
                Case "barcode_I"
                    Barcode_I = Barcode_I & field(I).value() & ","
                Case "barcode_C"
                    Barcode_C = Barcode_C & field(I).value() & ","
                Case "discount_I"
                    Discount_I = Discount_I & field(I).value() & ","
                Case "discount_C"
                    Discount_C = Discount_C & field(I).value() & ","
                Case "unitprice_I"
                    UnitPrice_I = UnitPrice_I & field(I).value() & ","
                Case "unitprice_C"
                    UnitPrice_C = UnitPrice_C & field(I).value() & ","
                Case "basediscount_I"
                    BaseDiscount_I = BaseDiscount_I & field(I).value() & ","
                Case "basediscount_C"
                    BaseDiscount_C = BaseDiscount_C & field(I).value() & ","
                Case "quantity_I"
                    Quantity_I = Quantity_I & field(I).value() & ","
                Case "quantity_C"
                    Quantity_C = Quantity_C & field(I).value() & ","
                Case "coverage_I"
                    Coverage_I = Coverage_I & field(I).value() & ","
                Case "coverage_C"
                    Coverage_C = Coverage_C & field(I).value() & ","
                Case "unit_I"
                    Unit_I = Unit_I & field(I).value() & ","
                Case "unit_C"
                    Unit_C = Unit_C & field(I).value() & ","
                Case "expire_I"
                    Expire_I = Expire_I & field(I).value() & ","
                Case "expire_C"
                    Expire_C = Expire_C & field(I).value() & ","
                Case "service_I"
                    Service_I = Service_I & field(I).value() & ","
                Case "service_C"
                    Service_C = Service_C & field(I).value() & ","
                Case "dose_I"
                    Dose_I = Dose_I & field(I).value() & ","
                Case "dose_C"
                    Dose_C = Dose_C & field(I).value() & ","
                Case "warehouse_I"
                    Warehouse_I = Warehouse_I & field(I).value() & ","
                Case "warehouse_C"
                    Warehouse_C = Warehouse_C & field(I).value() & ","
                Case "vat_I"
                    VAT_I = VAT_I & field(I).value() & ","
                Case "vat_C"
                    VAT_C = VAT_C & field(I).value() & ","
            End Select
        Next

        Dim barcode, item, baseprice, unitprice, quantity, discount, basediscount, coverage, unit, expire, service, dose, warehouse, vat As String()
        Dim Items As New List(Of InvoiceItem)
        If CashInvoice = False Then
            If Len(Barcode_I) > 0 Then
                barcode = Split(Left(Barcode_I, Len(Barcode_I) - 1), ",")
                item = Split(Left(Item_I, Len(Item_I) - 1), ",")
                baseprice = Split(Left(BasePrice_I, Len(BasePrice_I) - 1), ",")
                unitprice = Split(Left(UnitPrice_I, Len(UnitPrice_I) - 1), ",")
                quantity = Split(Left(Quantity_I, Len(Quantity_I) - 1), ",")
                discount = Split(Left(Discount_I, Len(Discount_I) - 1), ",")
                basediscount = Split(Left(BaseDiscount_I, Len(BaseDiscount_I) - 1), ",")
                coverage = Split(Left(Coverage_I, Len(Coverage_I) - 1), ",")
                unit = Split(Left(Unit_I, Len(Unit_I) - 1), ",")
                service = Split(Left(Service_I, Len(Service_I) - 1), ",")
                expire = Split(Left(Expire_I, Len(Expire_I) - 1), ",")
                dose = Split(Left(Dose_I, Len(Dose_I) - 1), ",")
                warehouse = Split(Left(Warehouse_I, Len(Warehouse_I) - 1), ",")
                vat = Split(Left(VAT_I, Len(VAT_I) - 1), ",")
                For I = 0 To barcode.Length - 1
                    Items.Add(New InvoiceItem(barcode(I), item(I), expire(I), service(I), warehouse(I), baseprice(I), discount(I), unitprice(I), quantity(I), unit(I), basediscount(I), coverage(I), vat(I), dose(I)))
                Next
            End If
        Else
            If Len(Barcode_C) > 0 Then
                barcode = Split(Left(Barcode_C, Len(Barcode_C) - 1), ",")
                item = Split(Left(Item_C, Len(Item_C) - 1), ",")
                baseprice = Split(Left(BasePrice_C, Len(BasePrice_C) - 1), ",")
                unitprice = Split(Left(UnitPrice_C, Len(UnitPrice_C) - 1), ",")
                quantity = Split(Left(Quantity_C, Len(Quantity_C) - 1), ",")
                discount = Split(Left(Discount_C, Len(Discount_C) - 1), ",")
                basediscount = Split(Left(BaseDiscount_C, Len(BaseDiscount_C) - 1), ",")
                coverage = Split(Left(Coverage_C, Len(Coverage_C) - 1), ",")
                unit = Split(Left(Unit_C, Len(Unit_C) - 1), ",")
                service = Split(Left(Service_C, Len(Service_C) - 1), ",")
                expire = Split(Left(Expire_C, Len(Expire_C) - 1), ",")
                dose = Split(Left(Dose_C, Len(Dose_C) - 1), ",")
                warehouse = Split(Left(Warehouse_C, Len(Warehouse_C) - 1), ",")
                vat = Split(Left(VAT_C, Len(VAT_C) - 1), ",")
                For I = 0 To barcode.Length - 1
                    Items.Add(New InvoiceItem(barcode(I), item(I), expire(I), service(I), warehouse(I), baseprice(I), discount(I), unitprice(I), quantity(I), unit(I), basediscount(I), coverage(I), vat(I), dose(I)))
                Next
            End If
        End If
        Return Items
    End Function


    Private Function getItemApprovalStatus(ByVal strItem As String, ByVal lngPatient As Long, ByVal dateTransaction As Date, ByVal strReference As String) As Integer
        Dim ds As DataSet
        Dim intVisit As Integer
        Dim returnValue As Integer
        Dim bApproval As Boolean

        ' => Get intVisit
        ds = dcl.GetDS("SELECT intVisit,bApproval FROM Hw_Treatments_Pharmacy WHERE strReference='" & strReference & "' AND dateTransaction='" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            intVisit = ds.Tables(0).Rows(0).Item("intVisit")
            bApproval = ds.Tables(0).Rows(0).Item("bApproval")

            If bApproval = True Then
                ' => Get Approvals
                ds = dcl.GetDS("SELECT * FROM Hw_Medicines_Approval Where intVisit=" & intVisit & " AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
                ' => Check Status (Approved  or Rejected)
                If ds.Tables(0).Rows.Count > 0 Then
                    If Not (IsDBNull(ds.Tables(0).Rows(0).Item("strRejectedBy")) And IsDBNull(ds.Tables(0).Rows(0).Item("strApprovedBy"))) Then
                        'TODO: assign here if rejected or approved
                        If ds.Tables(0).Rows(0).Item("strApprovedBy").ToString <> "" Then
                            returnValue = 1
                        Else
                            If ds.Tables(0).Rows(0).Item("strRejectedBy").ToString <> "" Then
                                returnValue = 0
                                'CheckItemCoverage()
                            Else
                                returnValue = 1
                            End If
                        End If
                    Else
                        returnValue = -3 '"Err:not approved or rejected yet"
                    End If
                Else
                    'returnValue = -2 '"Err:no record"
                    returnValue = 2 'Change to cash
                End If
            Else
                'Already Approved (bApproval=false means not required approvals)
                returnValue = 1
            End If
        Else
            'returnValue = -1 '"Err:intVisit not found"
            returnValue = 2 'Change to cash
        End If
        Return returnValue
    End Function

    Private Function FilterBarcode(ByVal strBarcode As String) As String()
        Dim ds As DataSet
        Dim strItem As String
        Dim str(3) As String

        If strBarcode <> "" Then
            Select Case strBarcode.Length
                Case 5
                    strItem = strBarcode
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        Dim dsTemp As DataSet
                        dsTemp = dcl.GetDS("SELECT MAX(curBasePrice) AS curBasePrice, MAX(dateExpiry) AS dateExpiry FROM Stock_Xlink_Items WHERE strItem='" & strItem & "' AND dateExpiry > GETDATE() AND curBasePrice IS NOT NULL AND curBasePrice > 0")
                        If dsTemp.Tables(0).Rows.Count > 0 Then
                            If Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("curBasePrice"))) And Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("dateExpiry"))) Then
                                str(1) = CDec(dsTemp.Tables(0).Rows(0).Item("curBasePrice")).ToString("F2")
                                str(2) = CDate(dsTemp.Tables(0).Rows(0).Item("dateExpiry")).ToString("yyyy-MM-dd")
                            Else
                                str(0) = "Err:item not defined"
                            End If
                        Else
                            str(0) = "Err:item not defined"
                        End If
                        'strItem_AfterUpdate()
                        'curBasePrice = Mid(strBarcode, 9, 4) & "." & Mid([strBarcode], 13, 2)
                        'dateExpiry = DateSerial(Mid(strBarcode, 17, 2), Mid(strBarcode, 15, 2), 1)
                        'ret = "OK => Less"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case Is < 12
                    strItem = Mid(strBarcode, 1, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = Mid(strBarcode, 9, 4) & "." & Mid(strBarcode, 13, 2)
                        str(2) = "20" & Mid(strBarcode, 17, 2) & "-" & Mid(strBarcode, 15, 2) & "-01"
                        'strItem_AfterUpdate()
                        'curBasePrice = Mid(strBarcode, 9, 4) & "." & Mid([strBarcode], 13, 2)
                        'dateExpiry = DateSerial(Mid(strBarcode, 17, 2), Mid(strBarcode, 15, 2), 1)
                        'ret = "OK => Less"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case 12
                    strItem = Left(strBarcode, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Item_Info WHERE strOldReference='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = ""
                        str(2) = "20" & Mid(strBarcode, 12, 1) & "-" & Mid(strBarcode, 10, 2) & "-01"
                        'strItem_AfterUpdate()
                        'dateExpiry = DateSerial(Mid(strBarcode, 12, 1), Mid(strBarcode, 10, 2), 1)
                        'ret = "OK => 12"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case Is >= 14 ' New Barcode
                    strItem = Left(strBarcode, 5)
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        If strBarcode.Length = 14 Then
                            str(1) = Mid(strBarcode, 10, 3) & "." & Mid(strBarcode, 13, 2)
                            str(2) = "20" & Mid(strBarcode, 8, 2) & "-" & Mid(strBarcode, 6, 2) & "-01"
                        Else
                            Dim restCount As Integer = strBarcode.Length - 9
                            str(1) = Mid(strBarcode, 10, restCount - 2) & "." & Mid(strBarcode, strBarcode.Length - 1, 2)
                            str(2) = "20" & Mid(strBarcode, 8, 2) & "-" & Mid(strBarcode, 6, 2) & "-01"
                        End If

                        'dateExpiry = DateSerial(Mid(strBarcode, 8, 2), Mid(strBarcode, 6, 2), 1)
                        'curBasePrice = Mid(strBarcode, 10, 3) & "." & Mid(strBarcode, 13, 2)
                        'Me.Parent![cmdSendCash].Enabled = True
                        'strItem_AfterUpdate()

                        ''                    If [Forms]![Ph_Prepearing_Med].[Flag] = True Then
                        'rsItems = CurrentDb.OpenRecordset("SELECT * FROM Hw_Treatments_Pharmacy WHERE strReference='" & [Forms]![Ph_Prepearing_Med].[strReference] & "' AND year(dateTransaction)=year(date())", dbOpenDynaset)
                        'rsItems.MoveFirst()
                        'rsItems.MoveLast()
                        'rsItems.MoveFirst()
                        'If DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = False Then
                        '    For I = 1 To rsItems.RecordCount
                        '        If [strItem] = rsItems!strItem Then
                        '            db = CurrentDb
                        '            ws = CreateWorkspace("ODBC", "", "", dbUseODBC)
                        '            con = ws.OpenConnection("ODBC", dbDriverNoPrompt, False, CurrentDb.TableDefs(0).Connect)
                        '            ws.BeginTrans()
                        '            CurrentDb.Execute "UPDATE Stock_Xlink_Items SET strDose1='" & rsItems!strDose & "'" & " WHERE lngXlink=" & [lngXlink] & " AND strItem='" & [strItem] & "'"
                        '            '                                    con.Execute "UPDATE Healthware..Hw_Treatments_Pharmacy SET byteCheck=1 WHERE strreference=" & [Forms]![Ph_Prepearing_Med].[strReference] & " AND strItem='" & [strItem] & "' AND Year(dateTransaction)=year(date())"
                        '            con.Close()
                        '            [strDose1] = rsItems!strDose
                        '            [curQuantity] = 1 'rsItems!curQuantity
                        '            [bApproval] = rsItems!bApproval
                        '            rsItems.MoveNext()
                        '            ws.Close()
                        '            Exit Function
                        '        End If

                        '        '    I = I + 1
                        '        rsItems.MoveNext()
                        '    Next I
                        '    If Not [Forms]![Ph_Prepearing_Med].[bCash] Or DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = False Then
                        '        Undo()
                        '    End If
                        '    MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãæÌæÏ ÈÇáØáÈ .", "Item out of order."), vbOKOnly)
                        '    End
                        'Else
                        '    [bApproval] = True
                        '    CurrentDb.Execute "UPDATE Stock_Xlink_Items SET bApproval=True WHERE lngXlink=" & [lngXlink] & " AND strItem='" & [strItem] & "'"
                        'End If
                        ' ''                    End If
                        'If [strDose1] & "" = "" Then
                        '    [strDose1] = "0000000000"
                        'End If
                        'strItem_AfterUpdate()
                        'Dose_Click()
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case Else
                        str(0) = "Err:item not defined"
            End Select
        Else
            str(0) = "Err:no barcode"
        End If
        Return str
    End Function

    Private Function getPrice(ByVal curBasePrice As Decimal, ByVal strItem As String) As Decimal
        Dim ds As DataSet
        Dim byteUnit As Byte
        Dim ret, curPrice, curFactor As Decimal

        If curBasePrice = 0 Then
            ds = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem WHERE SI.strItem='" & strItem & "'")
            byteUnit = CByte("0" & ds.Tables(0).Rows(0).Item("byteUnit").ToString)
            curPrice = CDec("0" & ds.Tables(0).Rows(0).Item("curPrice").ToString)
            curFactor = CDec("0" & ds.Tables(0).Rows(0).Item("curFactor").ToString)
            If curPrice <> 0 Then
                If byteUnit <> 0 Then
                    ret = CDec((curPrice * curFactor) / curFactor)
                Else
                    ret = curPrice
                End If
            Else
                ret = 0
            End If
        Else
            ret = curBasePrice
        End If
        Return ret
    End Function

    Private Function getDiscount(ByVal bCash As Boolean, ByVal strItem As String, ByVal bytePriceType As Integer, ByVal intGroup As Integer, ByVal lngPatient As Long, ByVal dateTransaction As Date) As Decimal 'bytePriceType from contacts table (company)
        Dim dsDiscount, dsPromotion As DataSet
        'Dim intSerice, intGroup As Integer
        'Dim byteWarehouse As Byte
        Dim curDiscount As Decimal
        Dim bytePromotionClass As Byte

        'ds = dcl.GetDS("SELECT * FROM Hw_Department_Items AS HDI INNER JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment INNER JOIN Stock_Items ON HDI.strItem = Stock_Items.strItem WHERE HDI.byteDepartment=" & byteDepartment & " AND HDI.strItem='" & strItem & "'")
        'intSerice = ds.Tables(0).Rows(0).Item("intService")
        'byteWarehouse = ds.Tables(0).Rows(0).Item("byteWarehouse")
        'intGroup = ds.Tables(0).Rows(0).Item("intGroup")
        '==>[intService] = DLookup("intService", "Hw_Department_Items", "byteDepartment=15 AND strItem='" & [strItem] & "'")
        '==>[byteWarehouse] = [intService].Column(2)
        'enabled
        '[dateExpiry].Enabled = Nz([strItemDesc].Column(3)) And Nz([byteWarehouse].Column(1), True) And [strItem].Column(3) = "0"

        If bCash = False Then
            dsDiscount = dcl.GetDS("SELECT * FROM Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
            If dsDiscount.Tables(0).Rows.Count > 0 Then
                curDiscount = Math.Abs(dsDiscount.Tables(0).Rows(0).Item("curPercent"))
            Else
                curDiscount = 0
            End If
        Else
            bytePromotionClass = DeterminePromotionClass(lngPatient, dateTransaction)
            If bytePromotionClass <> 0 Then
                dsPromotion = dcl.GetDS("SELECT curDiscount FROM Hw_Promotion_Groups WHERE byteClass=" & bytePromotionClass & " AND intGroup=" & intGroup)
                If dsPromotion.Tables(0).Rows.Count > 0 Then
                    curDiscount = dsPromotion.Tables(0).Rows(0).Item("curDiscount")
                    '[curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
                Else
                    curDiscount = 0
                End If
            Else
                curDiscount = 0
            End If
        End If
        '==>[curDiscount] = Abs(Nz(DLookup("curPercent", "Stock_Price_Groups", "bytePriceType=" & Nz(Me.Parent![lngContact].Column(2), 0) & " AND intGroup=" & Nz([strItem].Column(2), 0)), 0))
        '==>[curBaseDiscount] = [curDiscount]

        '    CheckItemCoverage

        'If Me.Parent![bCash] And Me.Parent![bytePromotionClass] & "" <> "" Then
        '    [curDiscount] = Nz(DLookup("curDiscount", "Hw_Promotion_Groups", "byteClass=" & Me.Parent![bytePromotionClass] & " AND intGroup=" & Val([strItemDesc].Column(6))), 0)
        '    [curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
        'End If

        '==>Dim curNet As Currency
        '==>curNet = ([curBasePrice] * [curQuantity]) - ([curBasePrice] * [curQuantity] * [curDiscount] / 100)
        Return curDiscount
    End Function
    Private Function getDiscount2(ByVal strItem As String, ByVal lngPatient As Long, ByVal dateTransaction As Date) As Decimal
        Dim ds As DataSet
        Dim curDiscount As Decimal
        Dim bytePromotionClass As Byte
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts AS HC LEFT JOIN Stock_Price_Groups AS SPG ON HC.bytePriceType=SPG.bytePriceType WHERE lngContact=27 AND bResident=1")
        If ds.Tables(0).Rows.Count > 0 Then
            curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString)
        Else
            curDiscount = 0
        End If

        bytePromotionClass = DeterminePromotionClass(lngPatient, dateTransaction)
        If bytePromotionClass <> 0 Then
            ds = dcl.GetDS(" SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
            Dim intGroup As Integer = ds.Tables(0).Rows(0).Item("intGroup")
            ds = dcl.GetDS("SELECT curDiscount FROM Hw_Promotion_Groups WHERE byteClass=" & bytePromotionClass & " AND intGroup=" & intGroup)
            If ds.Tables(0).Rows.Count > 0 Then
                curDiscount = ds.Tables(0).Rows(0).Item("curDiscount")
                '[curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
            End If
        End If
        Return curDiscount
    End Function

    Private Function getTax(ByVal curNet As Decimal, ByVal strItem As String) As Decimal
        'Dim ds As DataSet
        'Dim bTax As Boolean
        'Dim curTax As Decimal

        'ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
        'bTax = ds.Tables(0).Rows(0).Item("bTax")
        'curTax = ds.Tables(0).Rows(0).Item("curTax")

        ''Dim curNet As Currency
        ''curNet = ([curBasePrice] * [curQuantity]) - ([curBasePrice] * [curQuantity] * [curDiscount] / 100)
        ''If Not Me.Parent![bCash] Then
        'If bTax = True And curTax <> 0 Then

        '    If Me.Parent!bPercentValue = True And Me.Parent!curCoverage <> 0 Then
        '        curVAT = (curNet * Me.Parent!curCoverage / 100) * curTax / 100
        '        curVATI = (curNet * (100 - Me.Parent!curCoverage) / 100) * curTax / 100
        '    Else
        '        curVAT = 0
        '        curVATI = 0
        '    End If
        'Else
        '    Return 0
        '    'curVAT = 0
        '    'curVATI = 0
        'End If
        ''End If
    End Function

    Private Function CalculateAmount() As Decimal
        'Dim rsValue As Recordset
        'Dim curValue As Currency, curDiscount As Currency, curTotal As Currency

        'If [sfrm].Form![lngXlink] & "" = "" Then Exit Function
        'curTotal = Nz(DSum("curUnitPrice*curQuantity", "Stock_Xlink_Items", "lngXlink=" & [sfrm].Form![lngXlink]), 0)
        'curDiscount = curTotal
        'rsValue = CurrentDb().OpenRecordset("SELECT X.*, V.intValueSign, V.bytePercentCalculation FROM Stock_Xlink_Values AS X INNER JOIN Stock_Value_Types AS V ON (X.byteValueType=V.byteValueType) WHERE lngXlink=" & [sfrm].Form![lngXlink] & " ORDER BY X.byteValueType", dbOpenSnapshot)
        'Do Until rsValue.EOF
        '    Select Case rsValue!bytePercentCalculation
        '        Case 1 ' Prime Cost
        '            If rsValue!bPercentValue Then
        '                curValue = rsValue!curValue * curTotal / 100
        '            Else
        '                curValue = rsValue!curValue
        '            End If
        '            If rsValue!byteValueType = 1 Then curDiscount = curTotal - curValue

        '        Case 2 ' Prime - Discount
        '            If rsValue!bPercentValue Then
        '                curValue = rsValue!curValue * curDiscount / 100
        '            Else
        '                curValue = rsValue!curValue
        '            End If
        '    End Select
        '    curTotal = curTotal + rsValue!intValueSign * curValue
        '    rsValue.MoveNext()
        'Loop
        'rsValue.Close()
        'CalculateAmount = curTotal
    End Function

    Private Function CalculateCoverage(ByVal lngTransaction As Long, ByVal strItem As String, ByVal curUnitPrice As Decimal, ByVal curQuantity As Decimal, ByVal Cov As Decimal, ByVal lngContract As Long, ByVal byteScheme As Byte, ByVal bytePriceType As Integer, ByVal bytePrimaryDep As Byte, ByVal lngPatient As Long, ByVal lngSalesman As Long, ByVal dateTransaction As Date, ByVal intGroup As Integer) As Decimal
        Dim curCoverage As Decimal
        Dim ds, dsCovPlans, dsGroup, dsPHinvs As DataSet
        Dim curUnitPrice1, curCoverage1, curCoverage2, curCoverage3 As Decimal
        ' ???
        Dim bCash As Boolean = False
        Dim strBatch As String = ""
        Dim curUnitNetPrice, curDiscount, curCovAmount As Decimal

        'Get required data

        ' === > Check Coverage <===
        dsCovPlans = dcl.GetDS("SELECT ICP.curCoveragePercent,ICP.curCoverageValue, ICP.curYearlyLimit, ICP.curMonthlyLimit, IMI.strItem FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND IMI.strItem='" & strItem & "'")
        If dsCovPlans.Tables(0).Rows.Count > 0 Then
            If dsCovPlans.Tables(0).Rows(0).Item("curCoveragePercent").ToString <> "" Then
                curCoverage = curUnitPrice * curQuantity * dsCovPlans.Tables(0).Rows(0).Item("curCoveragePercent") / 100
            ElseIf dsCovPlans.Tables(0).Rows(0).Item("curCoverageValue").ToString <> "" Then
                curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curCoverageValue")
            Else
                dsCovPlans = dcl.GetDS("SELECT curDeductionValueP, curDeductionPercentP, curDeductionValueD, curDeductionPercentD FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                If dsCovPlans.Tables(0).Rows.Count > 0 Then
                    If bytePrimaryDep = 1 Then ' Primary
                        If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP").ToString <> "" Then
                            If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") = 100 Then
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") / 100
                            Else
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") / 100
                            End If
                        ElseIf dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueP").ToString <> "" Then
                            curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueP")
                        Else
                            curCoverage = 0
                        End If
                    Else ' Dependent
                        If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD").ToString <> "" Then
                            If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") = 100 Then
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") / 100
                            Else
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") / 100
                            End If
                        ElseIf dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueD").ToString <> "" Then
                            curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueD")
                        Else
                            curCoverage = 0
                        End If
                    End If
                Else
                    curCoverage = 0
                End If
            End If
        End If

        ' === > Check Deduction Limit <===
        If bCash = False Then
            Dim MaxP As Decimal = 0
            Dim CICov As Decimal = 0
            Dim MICov As Decimal = 0
            '1> Get Contract & Scheme Info (Case Limit - Year Limit - Max Deduction)
            dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
            If dsGroup.Tables(0).Rows.Count > 0 Then
                MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)
                '2> Get Clinic Invoices (Sum of Amount - Sum of Coverage)
                ds = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' And '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                If ds.Tables(0).Rows.Count > 0 Then
                    CICov = CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString)
                    '3> Get Medicins Invoices  (Sum of Amount - Sum of Coverage) ===== MUST NOT INCLUDE CURRENT TRANSACTION ======
                    dsPHinvs = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                    If dsPHinvs.Tables(0).Rows.Count > 0 Then MICov = CDec("0" & dsPHinvs.Tables(0).Rows(0).Item("Cov").ToString)
                    '4> Check if reach MAX or Limit
                    If CICov + MICov + Cov + curCoverage > MaxP Then
                        '5> Split coverage or cover all
                        'This is my code
                        'curCoverage = curCoverage - ((CICov + MICov + Cov + curCoverage) - MaxP)
                        'If curCoverage < 0 Then curCoverage = 0


                        'curCoverage = (CICov + MICon + Cov) - MaxP
                        'If curCoverage < 0 Then curCoverage = 0

                        ' '' '' ''MsgBox(IIf(gblanguage, "ÍÏ ÇáÇÞÊØÇÚ ááãÑíÖ åæ: " & rsGroup!curDeductionMaxP & " ÑíÇá", "Max Deduction limit for this Patient is " & rsGroup!curDeductionMaxP & " Ryials") & vbCrLf & IIf(gblanguage, "æãÌãæÚ ãÇ ÏÝÚå åæ: " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] + rsPHinvs!Cov & " ÑíÇá", "?And the total is " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] + rsPHinvs!Cov & " Ryials"), vbCritical)
                        '' '' ''curUnitPrice1 = (curUnitPrice * curQuantity) + (CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + Cov - dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP"))
                        '' '' ''curCoverage2 = Cov
                        '' '' ''curCoverage3 = Cov - ((curCoverage2 + CDec("0" & dsPHinvs.Tables(0).Rows(0).Item("Cov").ToString))) - (dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") - CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString))
                        '' '' ''curCoverage1 = curUnitPrice - (curCoverage3 - Cov)
                        '' '' ''If curCoverage > curCoverage3 Then
                        '' '' ''    curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    If curCoverage = 0 Then
                        '' '' ''        curUnitNetPrice = curUnitPrice - curCoverage2
                        '' '' ''        ds = dcl.GetDS("SELECT ABS(curPercent) AS curPercent FROM Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
                        '' '' ''        If ds.Tables(0).Rows.Count > 0 Then curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString) Else curDiscount = 0
                        '' '' ''    End If
                        '' '' ''    curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    'MsgBox(IIf(gblanguage, "Êã ÊÍæíá ÇáÝÇÆÖ Úä ÍÏ ÇáÇÞÊØÇÚ Úáì ÍÓÇÈ ÇáÔÑßÉ", "Flow Over Deduction Was Transfered To Company Account"), vbApplicationModal, "Al-Jazira Clinic")
                        '' '' ''    curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''End If
                    End If
                Else
                    curCoverage = -1 'Err:There Is No Clinic Invoice For THis PATIENT Please Go to Reception
                End If
            Else
                If CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + (Cov + curCovAmount) > dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") Then
                    If strBatch = "" Then
                        'MsgBox(IIf(gblanguage, "ÍÏ ÇáÇÞÊØÇÚ ááãÑíÖ åæ : " & rsGroup!curDeductionMaxP & " ÑíÇá", "Max Deduction limit for this Patient is " & rsGroup!curDeductionMaxP & " Ryials") & vbCrLf & IIf(gblanguage, "æãÌãæÚ ãÇ ÇÎÐå åæ " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] & " ÑíÇá", "?And the total is " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] & " Ryials"), vbCritical)
                        curUnitPrice1 = curUnitPrice * curQuantity + (CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + Cov - dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP"))
                        curCoverage2 = Cov
                        curCoverage3 = Cov - ((curCoverage2) - (dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") - CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString)))
                        If curCoverage3 <= 0 Then
                            curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                            GoTo Label1
                        End If
                        curCoverage1 = curUnitPrice - (curCoverage2 - Cov)
                        If curCoverage <= curCoverage3 Then Exit Function
                        curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        If curCoverage = 0 Then
                            curUnitNetPrice = curUnitPrice - curCoverage2
                            ds = dcl.GetDS("SELECT ABS(curPercent) AS curPercent FROM Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
                            curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString)
                        End If
Label1:
                        curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        'MsgBox(IIf(gblanguage, "Êã ÊÍæíá ÇáÝÇÆÖ ãä ÇáÇÞÊØÇÚ Úáì ÍÓÇÈ ÇáÔÑßÉ", "Flow Over Deduction Was Transfered To Company Account"), vbApplicationModal, "Al-Jazira Clinic")
                        curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                    End If
                End If
            End If
        End If
        Return curCoverage
    End Function

    ''' <summary>
    ''' Check the item not exceeded the <para /> limitation of coverage of insurance company
    ''' </summary>
    ''' <remarks>
    ''' Created by Faisal Al-Aseery 08-2019 Version: 1.0
    ''' </remarks>
    ''' <returns>Nothing if not exceeded, Otherwise will return Error as string</returns>
    ''' <param name="lngTransaction">The transaction ID.</param>
    ''' <param name="strItem">The item ID.</param>
    ''' <param name="curBasePrice">The base price of the item</param>
    Private Function CheckItemCoverage(ByVal lngTransaction As Long, ByVal lngContract As Long, ByVal byteScheme As Byte, ByVal strItem As String, ByVal curBasePrice As Decimal, ByVal curQuantity As Decimal, ByVal dateTransaction As Date, ByVal lngPatient As Long, ByVal curBasePriceTotal As Decimal) As String

        Dim ds, dsGroup, dsTrans As DataSet
        Dim strBatch As String = ""

        ' Get Information

        '1> Check in (Ins_Coverage_Items) Table for ????
        ds = dcl.GetDS("SELECT bAuthRequired, intWaitDays, curMaxQty, bVisits, bCovered FROM Ins_Coverage_Items WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme & " AND strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count = 0 Then ' No record
            '>2 Check in contracts for the service
            dsGroup = dcl.GetDS("SELECT ICG.bCovered, ICG.bAutorization FROM Ins_Coverage_Groups AS ICG INNER JOIN Ins_Med_Items AS IMI ON ICG.lngGroup = IMI.lngGroup AND ICG.lngMed = IMI.lngMed WHERE ICG.lngContract=" & lngContract & " AND ICG.byteScheme=" & byteScheme & " AND ICG.byteScope=2 AND IMI.strItem='" & strItem & "'")
            If dsGroup.Tables(0).Rows.Count = 0 Then ' No record
                Return "Err: This service is not included the contracts."
            Else
                '3> Check if is coverd
                If dsGroup.Tables(0).Rows(0).Item("bCovered") = False Then
                    Return "Err: This service is not covered"
                Else
                    '4> Check if authorized
                    If dsGroup.Tables(0).Rows(0).Item("bAutorization") = True Then
                        Return "Err: Autherization required for this service"
                    End If
                    '5> Check if (Yearly limit) Exist => By Item
                    dsGroup = dcl.GetDS("SELECT curYearlyLimit, ICP.lngMed FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND strItem = '" & strItem & "'")
                    If dsGroup.Tables(0).Rows(0).Item("curYearlyLimit").ToString <> "" Then
                        '5.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                        dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND lngMed= " & dsGroup.Tables(0).Rows(0).Item("lngMed") & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                        If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                            Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                        End If
                    End If
                    '6> Check if (Yearly limit) Exist => By Contract
                    dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP,curMonthlyLimitP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                    If dsGroup.Tables(0).Rows(0).Item("curYearLimitP").ToString <> "" Then
                        '6.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                        dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                        If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                            Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                        End If
                    End If
                    '7> Check if (Monthly Limit) Exist
                    If dsGroup.Tables(0).Rows.Count > 0 Then
                        If dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP").ToString <> "" Then
                            '7.1> Check if (Monthly Amount) is Greeter Than (Monthly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                            dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction BETWEEN '" & DateAdd(DateInterval.Month, 1, dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                            If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP") Then
                                Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                            End If
                        End If
                    End If
                    '8> Check if (Case Limit) Exist
                    If dsGroup.Tables(0).Rows.Count > 0 Then
                        If dsGroup.Tables(0).Rows(0).Item("curCaseLimitP").ToString <> "" Then
                            '8.1> Check if (Current Amount) is Greeter Than (Case Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                            dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction = '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                            Dim total As Decimal = CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString)
                            If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curCaseLimitP") Then
                                Return "Err: Max limit for the case is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curCaseLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                            End If
                        End If
                    End If
                End If
            End If
        Else
            '2> Check if is coverd
            If ds.Tables(0).Rows(0).Item("bCovered") = True Then
                '3> Check if authorized
                If ds.Tables(0).Rows(0).Item("bAutorization") = True Then
                    If strBatch = "" Then
                        '4> Check if still in waitting
                        If ds.Tables(0).Rows(0).Item("intWaitDays").ToString <> "" And ds.Tables(0).Rows(0).Item("intWaitDays").ToString <> "0" Then
                            '5> Check if (Max Quantity) Exist
                            If ds.Tables(0).Rows(0).Item("curMaxQty").ToString <> "" And ds.Tables(0).Rows(0).Item("curMaxQty").ToString <> "0" Then
                                '5.1> Check if (Current Amount) is Greeter Than (Max Quantity) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curBasePrice*curQuantity) AS curAmount, SUM(SXI.curQuantity) AS curQty FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND SXI.strItem='" & strItem & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If CDec("0" & dsTrans.Tables(0).Rows(0).Item("curQty").ToString) + curQuantity > 1 Then
                                    Return "Err: Max yearly limit for this service is " & Math.Round(ds.Tables(0).Rows(0).Item("curMaxQty"), byteCurrencyRound, MidpointRounding.AwayFromZero)
                                End If
                            End If
                            '6> Check if (Yearly limit) Exist => By Item
                            dsGroup = dcl.GetDS("SELECT curYearlyLimit, ICP.lngMed FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND strItem = '" & strItem & "'")
                            If dsGroup.Tables(0).Rows(0).Item("curYearlyLimit").ToString <> "" Then
                                '6.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND lngMed= " & dsGroup.Tables(0).Rows(0).Item("lngMed") & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                                    Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                End If
                            End If
                            '7> Check if (Yearly limit) Exist => By Contract
                            dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP,curMonthlyLimitP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                            If dsGroup.Tables(0).Rows(0).Item("curYearLimitP").ToString <> "" Then
                                '7.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                                    Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                End If
                            End If
                            '8> Check if (Case Limit) Exist
                            If dsGroup.Tables(0).Rows.Count > 0 Then
                                If dsGroup.Tables(0).Rows(0).Item("curCaseLimitP").ToString <> "" Then
                                    '8.1> Check if (Current Amount) is Greeter Than (Case Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                    dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction = '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                    If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curCaseLimitP") Then
                                        Return "Err: Max limit for the case is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curCaseLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                    End If
                                End If
                            End If
                        Else
                            Return "Err: Service coverage starts after " & ds.Tables(0).Rows(0).Item("intWaitDays").ToString
                        End If
                    End If
                Else
                    Return "Err: Autherization required for this service"
                End If
            Else
                Return "Err: This service is not covered"
            End If
        End If
        Return True

        'notCoverage:

        '        If Me.OpenArgs = 1 Then
        '            If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Medicine_Orders]![lngTransaction]) = True Then
        '                [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Medicine_Orders]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Medicine_Orders]![strReference] & "'"))
        '                [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                [lstCashOrder].Requery()
        '            End If
        '        Else
        '            If Me.OpenArgs = 0 Then
        '                On Error Resume Next
        '                If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Medicine_Cash]![lngTransaction]) = True Or DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then
        '                    [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Medicine_Cash]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Medicine_Cash]![strReference] & "'"))
        '                    [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                    [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                    [lstCashOrder].Requery()
        '                End If
        '            Else
        '                On Error Resume Next
        '                If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then 'Or DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then
        '                    [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Doctor_Medicine_Orders]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Doctor_Medicine_Orders]![strReference] & "'"))
        '                    [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                    [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                    [lstCashOrder].Requery()
        '                End If
        '            End If
        '        End If

        '        If bApprCheck = True Then
        '            [strBatch] = 1
        '            Exit Function
        '        Else
        '            db = CurrentDb
        '            ws = CreateWorkspace("ODBC", "", "", dbUseODBC)
        '            con = ws.OpenConnection("ODBC", dbDriverNoPrompt, False, CurrentDb.TableDefs(0).Connect)

        '            rs = con.OpenRecordset("SELECT Max(Convert(int, strTransaction)) AS Last FROM Stock_Trans WHERE DatePart(Year, dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " AND byteBase=50", dbOpenSnapshot)
        '            strTrans = Nz(rs!Last, 0) + 1
        '            rs.Close()
        '            [strItem] = DLookup("strItem", "Stock_Items", "strItem=""" & Left([strBarCode], 5) & """")
        '            [dateExpiry] = DateSerial(Mid$([strBarCode], 8, 2), Mid$([strBarCode], 6, 2), 1)
        '            [byteUnit] = 1
        '            If DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = True Then
        '                If MsgBox(IIf(gblanguage, "åá ÊæÏ ÇÖÇÝÉ ÇáÏæÇÁ Ýí ÇáÝÇÊæÑÉ ÇáäÞÏíÉ ¿ ", "Do You want Add Item In Cash Invoice ?"), vbQuestion + vbYesNo + vbDefaultButton2, "Warning...") = vbYes Then
        '                    rs = CurrentDb.OpenRecordset("SELECT T.lngTransaction, T.byteBase, T.byteTransType, T.lngContact, T.bCreatCash, T.strReference FROM Stock_Trans AS T WHERE T.byteBase=50 AND T.byteTransType=38 AND T.lngContact=27 AND T.bSubCash=True AND T.strReference='" & Forms![Ph_Prepearing_Med]![strReference] & "'", dbOpenSnapshot)
        '                    If rs.EOF Then
        '                    CurrentDb.Execute ("UPDATE Stock_Trans SET bCreatCash=True WHERE lngTransaction=" & Me.Parent![lngTransaction]), dbSeeChanges
        '                        strSQL = "INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, bCash, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, strUserPrint, bSubCash)"
        '                        strSQL = strSQL & " SELECT 50, 38,'" & strTrans & "', " & [byteDepartment] & " , 27, #" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#, 1, 1, 3, " & Forms![Ph_Prepearing_Med]![lngSalesmanNo] & ", " & Forms![Ph_Prepearing_Med]![lngPatient] & ", """ & Forms![Ph_Prepearing_Med]![strPatient] & """,'" & Forms![Ph_Prepearing_Med]![strReference] & "', 1, 0, """ & Forms![Cmn_Defaults]![strUserName] & """,True  "
        '                        CurrentDb.Execute strSQL
        '                        CurrentDb.Execute "INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & DLookup("lngTransaction", "stock_Trans", "strTransaction='" & strTrans & "'" & " AND dateTransaction=#" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#") & " , """ & Forms![Cmn_Defaults]![strUserName] & """, #" & Format(Now, "mm\/dd\/yyyy hh:nn") & "#)"
        '                        lngTransactionNew = DLookup("lngTransaction", "Stock_Trans", "strTransaction='" & strTrans & "' AND Year(dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " And byteBase=50")
        '                        rsXlink = con.OpenRecordset("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransactionNew, dbOpenDynaset)
        '                        If rsXlink.EOF Then
        '                            CurrentDb().Execute "INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionNew & "," & lngTransactionNew & ")"
        '                        End If
        '                    Else
        '                        lngTransactionNew = rs!lngTransaction
        '                    End If
        '                    If Forms![Ph_Prepearing_Med]![lngXlink1] & "" = "" Then Forms![Ph_Prepearing_Med]![lngXlink1] = DLookup("lngXlink", "Stock_Xlink", "lngPointer=" & lngTransactionNew)
        '                    CurrentDb.Execute "INSERT INTO Stock_Xlink_Items ( lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1 )" _
        '                     & " VALUES( " & Forms![Ph_Prepearing_Med]![lngXlink1] & ", " & Nz(DMax("intEntryNumber", "Stock_Xlink_Items", "lngXlink=" & Forms![Ph_Prepearing_Med]![lngXlink1]), 0) + 1 & ", " & [byteDepartment] & ", " & [intService] & ", " & [strItem] & ", " & [byteUnit] & ", 1, " & Forms![Ph_Prepearing_Med]![sfrm0].Form![curQuantity] & ", #" & dateExpiry & "#, " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", 0, " & [byteWarehouse] & ", #" & Format(Now(), "mm\/dd\/yyyy hh:nn:ss AmPm") & "#, '" & strBarCode & "','" & [strDose] & "')"
        '                    Undo()
        '                    '             Me.Requery
        '                    Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                    con.Close()
        '                    ws.Close()
        '                    '            CheckItemCoverage = False
        '                    [lstCashOrder].Requery()
        '                    bConvertedCash = True
        '                    CheckItemCoverage = True
        '                    Exit Function
        '                Else
        '                    '                    [bApproval] = False
        '                    [strBatch] = 1
        '                    Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                    CheckItemCoverage()
        '                End If
        '            Else
        '                rs = CurrentDb.OpenRecordset("SELECT T.lngTransaction, T.byteBase, T.byteTransType, T.lngContact, T.bCreatCash, T.strReference FROM Stock_Trans AS T WHERE T.byteBase=50 AND T.byteTransType=38 AND T.lngContact=27 AND T.bSubCash=True AND T.strReference='" & Forms![Ph_Prepearing_Med]![strReference] & "'", dbOpenSnapshot)
        '                If rs.EOF Then
        '                CurrentDb.Execute ("UPDATE Stock_Trans SET bCreatCash=True WHERE lngTransaction=" & Me.Parent![lngTransaction]), dbSeeChanges
        '                    strSQL = "INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, bCash, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, strUserPrint, bSubCash)"
        '                    strSQL = strSQL & " SELECT 50, 38,'" & strTrans & "', " & [byteDepartment] & " , 27, #" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#, 1, 1, 3, " & Forms![Ph_Prepearing_Med]![lngSalesmanNo] & ", " & Forms![Ph_Prepearing_Med]![lngPatient] & ", """ & Forms![Ph_Prepearing_Med]![strPatient] & """,'" & Forms![Ph_Prepearing_Med]![strReference] & "', 1, 0, """ & Forms![Cmn_Defaults]![strUserName] & """,True  "
        '                    CurrentDb.Execute strSQL
        '                    CurrentDb.Execute "INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & DLookup("lngTransaction", "stock_Trans", "strTransaction='" & strTrans & "'" & " AND dateTransaction=#" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#") & " , """ & Forms![Cmn_Defaults]![strUserName] & """, #" & Format(Now, "mm\/dd\/yyyy hh:nn") & "#)"
        '                    lngTransactionNew = DLookup("lngTransaction", "Stock_Trans", "strTransaction='" & strTrans & "' AND Year(dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " And byteBase=50")
        '                    rsXlink = con.OpenRecordset("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransactionNew, dbOpenDynaset)
        '                    If rsXlink.EOF Then
        '                        CurrentDb().Execute "INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionNew & "," & lngTransactionNew & ")"
        '                    End If
        '                Else
        '                    lngTransactionNew = rs!lngTransaction
        '                End If
        '                If Forms![Ph_Prepearing_Med]![lngXlink1] & "" = "" Then Forms![Ph_Prepearing_Med]![lngXlink1] = DLookup("lngXlink", "Stock_Xlink", "lngPointer=" & lngTransactionNew)
        '                CurrentDb.Execute "INSERT INTO Stock_Xlink_Items ( lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1 )" _
        '                 & " VALUES( " & Forms![Ph_Prepearing_Med]![lngXlink1] & ", " & Nz(DMax("intEntryNumber", "Stock_Xlink_Items", "lngXlink=" & Forms![Ph_Prepearing_Med]![lngXlink1]), 0) + 1 & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![byteDepartment] & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![intService] & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![strItem] & ", " & Nz(Forms![Ph_Prepearing_Med]![sfrm0].Form![byteUnit], 1) & ", 1, " & Forms![Ph_Prepearing_Med]![sfrm0].Form![curQuantity] & ", #" & dateExpiry & "#, " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", 0, " & [byteWarehouse] & ", #" & Format(Now(), "mm\/dd\/yyyy hh:nn:ss AmPm") & "#, '" & strBarCode & "', '" & [strDose] & "')"
        '                Undo()
        '                '             Me.Requery
        '                Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                con.Close()
        '                ws.Close()
        '                '            CheckItemCoverage = False
        '                [lstCashOrder].Requery()
        '            End If
        '            rs.Close()
        '        End If
    End Function

    Private Function DeterminePromotionClass(ByVal lngPatient As Long, ByVal dateTransaction As Date) As Byte
        'Dim rs As Recordset
        'Dim rsAmount As Recordset
        'Dim strAmount As String
        'Dim rsPromotions As Recordset
        Dim dsPromotions, ds As DataSet

        'dsPromotions = dcl.GetDS("SELECT * FROM Hw_Promotions")
        ds = dcl.GetDS("SELECT dateParticipation, byteClass, dateClass, lngFamily FROM Hw_Patients WHERE lngPatient=" & lngPatient)
        If ds.Tables(0).Rows(0).Item("lngFamily").ToString = "" Then
            ' Single patient
            Select Case ds.Tables(0).Rows(0).Item("byteClass").ToString
                Case "1" ' VIP Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'End If
                    End If
                Case "2" ' A Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'End If
                    End If
                Case Else
                    Return CDec("0" & ds.Tables(0).Rows(0).Item("byteClass").ToString)
            End Select
        Else
            ' Family
            Dim lngFamily As Long = ds.Tables(0).Rows(0).Item("lngFamily")
            Dim intPatients As Integer
            Dim dsTemp As DataSet = dcl.GetDS("SELECT lngPatient FROM Hw_Patients WHERE lngFamily=" & lngFamily)
            If dsTemp.Tables(0).Rows.Count > 0 Then
                intPatients = dsTemp.Tables(0).Rows(0).Item("lngPatient")
            Else
                intPatients = 0
            End If
            Select Case ds.Tables(0).Rows(0).Item("byteClass").ToString
                Case "1" ' VIP Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
                        'End If
                    End If
                Case "2" ' A Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
                        'End If
                    End If
                Case Else
                    Return CDec("0" & ds.Tables(0).Rows(0).Item("byteClass").ToString)
            End Select
        End If

    End Function

    ' Get ByteTransType
    Private Function GetTransType(ByVal lngGuarantor As Long) As Byte
        ' Parameter: lngGuarantor Equals 0, no guarantor
        Dim dsTransTypes As DataSet
        ' This code by me
        dsTransTypes = dcl.GetDS("SELECT * FROM Stock_Trans_Types AS STT INNER JOIN Stock_TransType_Departments AS STTD ON STT.byteTransType = STTD.byteTransType INNER JOIN Cmn_User_Departments AS CUD ON STTD.byteDepartment = CUD.byteDepartment WHERE STT.byteBase=" & byteBase & " AND CUD.strUserName = '" & strUserName & "'")
        If dsTransTypes.Tables(0).Rows.Count > 0 Then
            Return dsTransTypes.Tables(0).Rows(0).Item("byteTransType")
        Else
            Return 0
        End If

        'If lngGuarantor <> 0 Then
        '    rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=2 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '    If Not rsTransTypes.EOF Then
        '        [byteTransType] = rsTransTypes!byteTransType
        '    Else
        '        rsTransTypes.Close()
        '        rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=3 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '        If Not rsTransTypes.EOF Then
        '            [byteTransType] = rsTransTypes!byteTransType
        '            [bCash] = False
        '        End If
        '    End If
        '    byteTransType_AfterUpdate()
        '    [lngContact] = lngGuarantor
        '    [lngContactNo] = [lngContact]
        '    rsTransTypes.Close()
        'Else
        '    rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=1 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '    If Not rsTransTypes.EOF Then
        '        [byteTransType] = rsTransTypes!byteTransType
        '        byteTransType_AfterUpdate()
        '    Else
        '        rsTransTypes.Close()
        '        rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=3 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '        [byteTransType] = rsTransTypes!byteTransType
        '        [bCash] = True
        '        byteTransType_AfterUpdate()
        '    End If
        '    rsTransTypes.Close()
        'End If
        ''SetInsInfo Nz(lngGuarantor, 0)
    End Function

    ' Send To Cashier
    Private Sub cmdCash_Click()
        'Dim lngTranslink As Long

        '[lngTranslink] = CLng(Nz(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & [strReference] & "'"), 0))
        'CurrentDb.Execute("UPDATE Stock_Trans SET byteStatus=2, bCollected1 = True, bApproved1 = True WHERE lngTransaction=" & [lngTransaction], dbSeeChanges)
        'CurrentDb.Execute("UPDATE Stock_Trans SET byteStatus=2, bCollected1 = True, bApproved1 = True WHERE lngTransaction=" & [lngTranslink], dbSeeChanges)
        'MsgBox IIf(gblanguage, "¡Êã ÇáÇÑÓÇá Çáì ÇáßÇÔíÑ", "Order Was Send To Cash Succesfuly" & vbOKOnly)

        'DoCmd.Close(acForm, "PH_Prepearing_Med")
        'On Error Resume Next
        'If Me.OpenArgs = 1 Then
        '    [Forms]![PH_Medicine_Orders]![lstInvoices].Requery()
        '    [Forms]![PH_Medicine_Orders]![lstInvoicesAll].Requery()
        '    [Forms]![PH_Medicine_Orders]![LstServices].Requery()
        'Else
        '    If Me.OpenArgs = 2 Then
        '        [Forms]![PH_Doctor_Medicine_Orders]![lstInvoices].Requery()
        '        [Forms]![PH_Doctor_Medicine_Orders]![lstInvoicesAll].Requery()
        '        [Forms]![PH_Doctor_Medicine_Orders]![LstServices].Requery()
        '    Else
        '        [Forms]![PH_Medicine_Cash]![lstInvoices].Requery()
        '        [Forms]![PH_Medicine_Cash]![lstInvoices].Requery()
        '        [Forms]![PH_Medicine_Cash]![LstServices].Requery()
        '    End If
        'End If
    End Sub

    Private Sub cmdClose_Click()
        '    Dim strSQL As String
        '    Dim byteDepartment As Byte, byteWarehouse As Byte
        '    Dim intService As Integer, intLastService As Integer
        '    Dim lngTrans As Long, lngXlink As Long
        '    Dim rsTrans As Recordset, rsXlink As Recordset, rsXlink_Items As Recordset, rsInvoice As Recordset
        '    Dim lngTranslink As Long, lngXlinkLink As Long
        '    Dim rsTAudit As Recordset

        '    If DLookup("lngTransSales", "Stock_Trans", "lngTransaction=" & [lngTransaction]) & "" = "" Then
        '        strSQL = "SELECT X.lngXlink, T.lngTransaction, T.bCash, T.byteDepartment, T.strRemarks, T.strReference, T.lngSalesman, T.lngPatient, T.dateEntry, T.lngContact, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '               " FROM (Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink" & _
        '               " WHERE X.lngTransaction =" & [lngTransaction] & _
        '               " GROUP BY X.lngXlink, T.lngTransaction, T.bCash, T.byteDepartment, T.strRemarks, T.strReference, T.lngSalesman, T.lngPatient, T.dateEntry, T.lngContact, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '               " ORDER BY XI.intEntryNumber"
        '        rsInvoice = CurrentDb.OpenRecordset(strSQL, dbOpenSnapshot)
        '        If rsInvoice.EOF Then Exit Sub
        '        If Not rsInvoice.EOF Then
        '            rsInvoice.MoveLast()
        '            rsInvoice.MoveFirst()
        '            ProgressMeter(udInitMeter, IIf(gblanguage, "ÌÇÑí äÓÎ ÇáØáÈ ááÝÇÊæÑÉ...", "Copying data..."), rsInvoice.RecordCount)
        '        Else
        '            MsgBox(IIf(gblanguage, "áÇ ÊæÌÏ ÈíÇäÇÊ ááäÓÎ.", "There is no data to be Copy."), vbInformation)
        '            Exit Sub
        '        End If
        '        rsTrans = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans WHERE lngTransaction=" & rsInvoice!lngTransaction, dbOpenDynaset, dbSeeChanges)
        '        With rsTrans
        '            .Edit()
        '            !strTransaction = Nz(DMax("Clng(strTransaction)", "Stock_Trans", "Year(dateTransaction)=" & Year(dateTransaction) & " AND byteBase=40"), 0) + 1
        '            !byteBase = 40
        '            !byteTransType = DLookup("byteTransType", "Stock_Trans_Types", "byteBase=40")
        '            !lngContact = rsInvoice!lngContact
        '        !dateTransaction = Format(Date, "dd\/mm\/yyyy")
        '            !byteDepartment = rsInvoice!byteDepartment
        '            !dateClosedValid = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '            !dateEntry = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '            !byteStatus = 1
        '            !bCash = rsInvoice!bCash
        '            !byteCurrency = rsInvoice!byteCurrency
        '            !strRemarks = rsInvoice!strRemarks
        '            !strReference = rsInvoice!strReference
        '            !lngSalesman = rsInvoice!lngSalesman
        '            !lngPatient = rsInvoice!lngPatient
        '            !byteWarehouse = 3
        '            .Update()
        '            .MoveLast()
        '            lngTrans = !lngTransaction
        '        End With
        '        rsTAudit = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTrans, dbOpenDynaset, dbSeeChanges)
        '        With rsTAudit
        '            .Edit()
        '            !lngTransaction = lngTrans
        '            !strCreatedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateCreated = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strLastSavedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateLastSaved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strApprovedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateApproved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strCashBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateCash = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            .Update()
        '            .MoveLast()
        '        End With
        '        '''        DoCmd.OpenReport "rStock_OutPAtientInv_Jazirah_Casher"
        '        '''''

        '        ''''''
        '        MsgBox(IIf(gblanguage, "Êã ÇäÔÇÁ ÇáÝÇÊæÑÉ, ", "Invoice Was Created Successfuly "), vbOKOnly)
        '        ''''''
        '        rsTrans.Close()
        '        rsInvoice.Close()
        '        ProgressMeter udremovemeter
        '        MsgBox IIf(gblanguage, "ÊãÊ ÇáÚãáíÉ ÈäÌÇÍ", "Operation completed successfully")

        '        If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & [lngTransaction]) = True Then
        '            On Error Resume Next
        '            [lngTranslink] = Nz(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & [strReference] & "'"), 0)
        '            [lngXlinkLink] = Nz(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]), 0)

        '            strSQL = "SELECT X.lngXlink, T.lngTransaction, T.byteDepartment, T.strReference, T.strRemarks, T.lngSalesman, T.lngPatient, T.dateEntry, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '                   " FROM (Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink" & _
        '                   " WHERE (((X.lngTransaction) =" & [lngTranslink] & ") And ((X.lngPointer) =" & [lngTranslink] & "))" & _
        '                   " GROUP BY X.lngXlink, T.lngTransaction, T.byteDepartment, T.strReference, T.strRemarks, T.lngSalesman, T.lngPatient, T.dateEntry, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '                   " ORDER BY XI.intEntryNumber"
        '            rsInvoice = CurrentDb.OpenRecordset(strSQL, dbOpenSnapshot)
        '            If rsInvoice.EOF Then Exit Sub
        '            If Not rsInvoice.EOF Then
        '                rsInvoice.MoveLast()
        '                rsInvoice.MoveFirst()
        '                ProgressMeter(udInitMeter, IIf(gblanguage, "ÌÇÑí äÓÎ ÇáØáÈ ÇáäÞÏí ááÝÇÊæÑÉ...", "Copying Cash data..."), rsInvoice.RecordCount)
        '            Else
        '                MsgBox(IIf(gblanguage, "áÇ ÊæÌÏ ÈíÇäÇÊ ááäÓÎ.", "There is no data to be Copy."), vbInformation)
        '                Exit Sub
        '            End If
        '            rsTrans = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans WHERE lngTransaction=" & rsInvoice!lngTransaction, dbOpenDynaset, dbSeeChanges)
        '            With rsTrans
        '                .Edit()
        '                !strTransaction = Nz(DMax("Clng(strTransaction)", "Stock_Trans", "Year(dateTransaction)=" & Year(dateTransaction) & " AND byteBase=40"), 0) + 1
        '                !byteBase = 40
        '                !byteTransType = DLookup("byteTransType", "Stock_Trans_Types", "byteBase=40")
        '            !dateTransaction = Format(Date, "dd\/mm\/yyyy")
        '                !dateClosedValid = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '                !dateEntry = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '                !byteDepartment = rsInvoice!byteDepartment
        '                !lngContact = 27
        '                !byteStatus = 1
        '                !bCash = True
        '                !strRemarks = rsInvoice!strRemarks
        '                !strReference = rsInvoice!strReference
        '                !lngSalesman = rsInvoice!lngSalesman
        '                !lngPatient = rsInvoice!lngPatient
        '                !byteCurrency = rsInvoice!byteCurrency
        '                !byteWarehouse = 3
        '                .Update()
        '                .MoveLast()
        '                lngTrans = !lngTransaction
        '            End With
        '            rsTAudit = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTrans, dbOpenDynaset, dbSeeChanges)
        '            With rsTAudit
        '                .Edit()
        '                !lngTransaction = lngTrans
        '                !strCreatedBy = Forms![Cmn_Defaults]![strUserName]
        '                !dateCreated = Format(Now, "mm\/dd\/yyyy hh:nn")
        '                !strLastSavedBy = Forms![Cmn_Defaults]![strUserName]
        '                !dateLastSaved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '                !strApprovedBy = Forms![Cmn_Defaults]![strUserName]
        '                !dateApproved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '                !strCashBy = Forms![Cmn_Defaults]![strUserName]
        '                !dateCash = Format(Now, "mm\/dd\/yyyy hh:nn")
        '                .Update()
        '                .MoveLast()
        '            End With
        '            '''            DoCmd.OpenReport "rStock_OutPAtientInv_Jazirah_Casher", , "lngTransaction=" & rsInvoice!lngTransaction
        '            '''''
        '            MsgBox(IIf(gblanguage, "Êã ÇäÔÇÁ ÇáÝÇÊæÑÉ, ", "Invoice Was Created Successfuly "), vbOKOnly)
        '            ''''''
        '            rsTrans.Close()
        '            rsInvoice.Close()
        '            ProgressMeter udremovemeter
        '            MsgBox IIf(gblanguage, "ÊãÊ ÇáÚãáíÉ ÈäÌÇÍ", "Operation completed successfully")
        '        End If
        '    Else
        '        lngTrans = DLookup("lngTransSales", "Stock_Trans", "lngTransaction=" & [lngTransaction])
        '    CurrentDb.Execute ("UPDATE Stock_Trans SET Stock_Trans.byteStatus =1 WHERE Stock_Trans.lngTransaction=" & [lngTransaction]), dbSeeChanges
        '        MsgBox IIf(gblanguage, "Êã ÇäÔÇÁ ÝÇÊæÑÉ ÓÇíÞÇ", "Invoice Recently Added")
        '    End If
        '    [lstInvoices].Requery()
        '    [LstServices].Requery()

    End Sub

    ' Close Invoice
    Private Sub cmdOrder_Click()
        'Dim strSQL As String
        'Dim byteDepartment As Byte, byteWarehouse As Byte
        'Dim intService As Integer, intLastService As Integer
        'Dim lngTrans As Long, lngXlink As Long
        'Dim rsTrans As Recordset, rsXlink As Recordset, rsXlink_Items As Recordset, rsInvoice As Recordset
        'Dim lngTranslink As Long, lngXlinkLink As Long
        'Dim rsTAudit As Recordset

        'If DLookup("lngTransSales", "Stock_Trans", "lngTransaction=" & [lngTransaction]) & "" = "" Then
        '    strSQL = "SELECT X.lngXlink, T.lngTransaction, T.bCash, T.byteDepartment, T.strRemarks, T.strReference, T.lngSalesman, T.lngPatient, T.dateEntry, T.lngContact, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '           " FROM (Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink" & _
        '           " WHERE X.lngTransaction =" & [lngTransaction] & _
        '           " GROUP BY X.lngXlink, T.lngTransaction, T.bCash, T.byteDepartment, T.strRemarks, T.strReference, T.lngSalesman, T.lngPatient, T.dateEntry, T.lngContact, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '           " ORDER BY XI.intEntryNumber"
        '    rsInvoice = CurrentDb.OpenRecordset(strSQL, dbOpenSnapshot)
        '    If rsInvoice.EOF Then Exit Sub
        '    If Not rsInvoice.EOF Then
        '        rsInvoice.MoveLast()
        '        rsInvoice.MoveFirst()
        '        ProgressMeter(udInitMeter, IIf(gblanguage, "ÌÇÑí äÓÎ ÇáØáÈ ááÝÇÊæÑÉ...", "Copying data..."), rsInvoice.RecordCount)
        '    Else
        '        MsgBox(IIf(gblanguage, "áÇ ÊæÌÏ ÈíÇäÇÊ ááäÓÎ.", "There is no data to be Copy."), vbInformation)
        '        Exit Sub
        '    End If
        '    rsTrans = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans WHERE lngTransaction=" & rsInvoice!lngTransaction, dbOpenDynaset, dbSeeChanges)
        '    With rsTrans
        '        .Edit()
        '        !strTransaction = Nz(DMax("Clng(strTransaction)", "Stock_Trans", "Year(dateTransaction)=" & Year(dateTransaction) & " AND byteBase=40"), 0) + 1
        '        !byteBase = 40
        '        !byteTransType = DLookup("byteTransType", "Stock_Trans_Types", "byteBase=40")
        '        !lngContact = rsInvoice!lngContact
        '    !dateTransaction = Format(Date, "dd\/mm\/yyyy")
        '        !byteDepartment = rsInvoice!byteDepartment
        '        !dateClosedValid = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '        !dateEntry = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '        !byteStatus = 1
        '        !bCash = rsInvoice!bCash
        '        !byteCurrency = rsInvoice!byteCurrency
        '        !strRemarks = rsInvoice!strRemarks
        '        !strReference = rsInvoice!strReference
        '        !lngSalesman = rsInvoice!lngSalesman
        '        !lngPatient = rsInvoice!lngPatient
        '        !byteWarehouse = 3
        '        .Update()
        '        .MoveLast()
        '        lngTrans = !lngTransaction
        '    End With
        '    rsTAudit = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTrans, dbOpenDynaset, dbSeeChanges)
        '    With rsTAudit
        '        .Edit()
        '        !lngTransaction = lngTrans
        '        !strCreatedBy = Forms![Cmn_Defaults]![strUserName]
        '        !dateCreated = Format(Now, "mm\/dd\/yyyy hh:nn")
        '        !strLastSavedBy = Forms![Cmn_Defaults]![strUserName]
        '        !dateLastSaved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '        !strApprovedBy = Forms![Cmn_Defaults]![strUserName]
        '        !dateApproved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '        !strCashBy = Forms![Cmn_Defaults]![strUserName]
        '        !dateCash = Format(Now, "mm\/dd\/yyyy hh:nn")
        '        .Update()
        '        .MoveLast()
        '    End With
        '    If rsInvoice!bCash = True Then
        '        If MsgBox(IIf(gblanguage, "åá ÊÑíÏ ØÈÇÚÉ ÇáÅíÕÇáð¿", "Do you want print invoice?"), vbYesNo) = vbYes Then
        '            ChoosePrinter()
        '            DoCmd.OpenReport "rStock_OutPAtientInv_Jazirah_Casher"
        '        End If
        '    End If
        '    If rsInvoice!bCash = False Then
        '        ChoosePrinter()
        '        DoCmd.OpenReport "rStock_OutPAtientInv_Jazirah_Casher"
        '    End If
        '    '''''

        '    ''''''
        '    '            MsgBox IIf(gblanguage, "Êã ÇäÔÇÁ ÇáÝÇÊæÑÉ, ", "Invoice Was Created Successfuly "), vbOKOnly
        '    ''''''
        '    rsTrans.Close()
        '    rsInvoice.Close()
        '    ProgressMeter udremovemeter
        '    '            MsgBox IIf(gblanguage, "ÊãÊ ÇáÚãáíÉ ÈäÌÇÍ", "Operation completed successfully")

        '    If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & [lngTransaction]) = True Then
        '        On Error Resume Next
        '        [lngTranslink] = Nz(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & [strReference] & "'"), 0)
        '        [lngXlinkLink] = Nz(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]), 0)

        '        strSQL = "SELECT X.lngXlink, T.lngTransaction, T.byteDepartment, T.strReference, T.strRemarks, T.lngSalesman, T.lngPatient, T.dateEntry, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '               " FROM (Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink" & _
        '               " WHERE (((X.lngTransaction) =" & [lngTranslink] & ") And ((X.lngPointer) =" & [lngTranslink] & "))" & _
        '               " GROUP BY X.lngXlink, T.lngTransaction, T.byteDepartment, T.strReference, T.strRemarks, T.lngSalesman, T.lngPatient, T.dateEntry, T.byteCurrency, XI.intEntryNumber, XI.intService, XI.strItem, XI.byteUnit, XI.byteQuantityType, XI.curQuantity, XI.dateExpiry, XI.curUnitPrice, XI.curBasePrice, XI.curDiscount, XI.curBaseDiscount, XI.curCoverage, XI.strBarcode" & _
        '               " ORDER BY XI.intEntryNumber"
        '        rsInvoice = CurrentDb.OpenRecordset(strSQL, dbOpenSnapshot)
        '        If rsInvoice.EOF Then Exit Sub
        '        If Not rsInvoice.EOF Then
        '            rsInvoice.MoveLast()
        '            rsInvoice.MoveFirst()
        '            ProgressMeter(udInitMeter, IIf(gblanguage, "ÌÇÑí äÓÎ ÇáØáÈ ÇáäÞÏí ááÝÇÊæÑÉ...", "Copying Cash data..."), rsInvoice.RecordCount)
        '        Else
        '            MsgBox(IIf(gblanguage, "áÇ ÊæÌÏ ÈíÇäÇÊ ááäÓÎ.", "There is no data to be Copy."), vbInformation)
        '            Exit Sub
        '        End If
        '        rsTrans = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans WHERE lngTransaction=" & rsInvoice!lngTransaction, dbOpenDynaset, dbSeeChanges)
        '        With rsTrans
        '            .Edit()
        '            !strTransaction = Nz(DMax("Clng(strTransaction)", "Stock_Trans", "Year(dateTransaction)=" & Year(dateTransaction) & " AND byteBase=40"), 0) + 1
        '            !byteBase = 40
        '            !byteTransType = DLookup("byteTransType", "Stock_Trans_Types", "byteBase=40")
        '        !dateTransaction = Format(Date, "dd\/mm\/yyyy")
        '            !dateClosedValid = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '            !dateEntry = Format(Now(), "dd\/mm\/yyyy hh:nn:ss AmPm")
        '            !byteDepartment = rsInvoice!byteDepartment
        '            !lngContact = 27
        '            !byteStatus = 1
        '            !bCash = True
        '            !strRemarks = rsInvoice!strRemarks
        '            !strReference = rsInvoice!strReference
        '            !lngSalesman = rsInvoice!lngSalesman
        '            !lngPatient = rsInvoice!lngPatient
        '            !byteCurrency = rsInvoice!byteCurrency
        '            !byteWarehouse = 3
        '            .Update()
        '            .MoveLast()
        '            lngTrans = !lngTransaction
        '        End With
        '        rsTAudit = CurrentDb.OpenRecordset("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTrans, dbOpenDynaset, dbSeeChanges)
        '        With rsTAudit
        '            .Edit()
        '            !lngTransaction = lngTrans
        '            !strCreatedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateCreated = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strLastSavedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateLastSaved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strApprovedBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateApproved = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            !strCashBy = Forms![Cmn_Defaults]![strUserName]
        '            !dateCash = Format(Now, "mm\/dd\/yyyy hh:nn")
        '            .Update()
        '            .MoveLast()
        '        End With
        '        '            DoCmd.OpenReport "rStock_OutpatientInv_Jazirah_Cash_Casher", , "lngTransaction=" & rsInvoice!lngTransaction
        '        '''''
        '        '            MsgBox IIf(gblanguage, "Êã ÇäÔÇÁ ÇáÝÇÊæÑÉ, ", "Invoice Was Created Successfuly "), vbOKOnly
        '        ''''''
        '        rsTrans.Close()
        '        rsInvoice.Close()
        '        ProgressMeter udremovemeter
        '        '            MsgBox IIf(gblanguage, "ÊãÊ ÇáÚãáíÉ ÈäÌÇÍ", "Operation completed successfully")
        '    End If
        'Else
        '    lngTrans = DLookup("lngTransSales", "Stock_Trans", "lngTransaction=" & [lngTransaction])
        'CurrentDb.Execute ("UPDATE Stock_Trans SET Stock_Trans.byteStatus =1 WHERE Stock_Trans.lngTransaction=" & [lngTransaction]), dbSeeChanges
        '    MsgBox IIf(gblanguage, "Êã ÇäÔÇÁ ÝÇÊæÑÉ ÓÇíÞÇ", "Invoice Recently Added")
        'End If
        '[lstInvoices].Requery()
        '[LstServices].Requery()
        'cmdOrder.Visible = False
    End Sub

    Public Function SuspendInvoice(ByVal lngTransaction As Long, ByVal CashOnly As Integer, ByVal InsuranceItems As String, ByVal CashItems As String) As String
        If HttpContext.Current.Session("Suspend_Trans") Is Nothing Then
            HttpContext.Current.Session("Suspend_Trans") = lngTransaction
            HttpContext.Current.Session("Suspend_CashOnly") = CashOnly
            HttpContext.Current.Session("Suspend_IItems") = InsuranceItems
            HttpContext.Current.Session("Suspend_CItems") = CashItems
        Else
            Dim found As Boolean = False
            Dim trans As String() = Split(HttpContext.Current.Session("Suspend_Trans"), "×")
            For I = 0 To trans.Length - 1
                If trans(I) <> "" Then If CLng(trans(I)) = lngTransaction Then found = True
            Next
            If found = False Then
                HttpContext.Current.Session("Suspend_Trans") = HttpContext.Current.Session("Suspend_Trans") & "×" & lngTransaction
                HttpContext.Current.Session("Suspend_CashOnly") = HttpContext.Current.Session("Suspend_CashOnly") & "×" & CashOnly
                HttpContext.Current.Session("Suspend_IItems") = HttpContext.Current.Session("Suspend_IItems") & "×" & InsuranceItems
                HttpContext.Current.Session("Suspend_CItems") = HttpContext.Current.Session("Suspend_CItems") & "×" & CashItems
            End If
        End If

        Return "<script type=""text/javascript"">$('#mdlSales').modal('hide');</script>"
    End Function

    Public Function UnsuspendInvoice(ByVal lngTransaction As Long) As String
        If Not (HttpContext.Current.Session("Suspend_Trans") Is Nothing) Then
            Dim trans As String() = Split(HttpContext.Current.Session("Suspend_Trans"), "×")
            Dim cashO As String() = Split(HttpContext.Current.Session("Suspend_CashOnly"), "×")
            Dim IItem As String() = Split(HttpContext.Current.Session("Suspend_IItems"), "×")
            Dim CItem As String() = Split(HttpContext.Current.Session("Suspend_CItems"), "×")

            HttpContext.Current.Session("Suspend_Trans") = ""
            HttpContext.Current.Session("Suspend_CashOnly") = ""
            HttpContext.Current.Session("Suspend_IItems") = ""
            HttpContext.Current.Session("Suspend_CItems") = ""
            For I = 0 To trans.Length - 1
                If trans(I) <> "" Then
                    If CLng(trans(I)) <> lngTransaction Then
                        HttpContext.Current.Session("Suspend_Trans") = HttpContext.Current.Session("Suspend_Trans") & "×" & trans(I)
                        HttpContext.Current.Session("Suspend_CashOnly") = HttpContext.Current.Session("Suspend_CashOnly") & "×" & cashO(I)
                        HttpContext.Current.Session("Suspend_IItems") = HttpContext.Current.Session("Suspend_IItems") & "×" & IItem(I)
                        HttpContext.Current.Session("Suspend_CItems") = HttpContext.Current.Session("Suspend_CItems") & "×" & CItem(I)
                    End If
                End If
            Next
        End If

        Return "<script type=""text/javascript"">closeCurrentTab();</script>" '"<script>if(tabCount>1) {$('#tabCashier li:nth-child('+curTab+')').remove(); tabCount--;$('#tabCashier li:first-child a').tab('show');} else $('#mdlSales').modal('hide');</script>"
    End Function

    Private Function checkDuplicateItem(ByVal curOrderQuantity As Decimal, ByVal strItem As String, ByVal SelectedInsuranceItems As String) As Boolean
        Dim Count As Integer = 0
        Dim items As String() = Split(SelectedInsuranceItems, ",")
        For Each item As String In items
            If item = strItem Then Count = Count + 1
        Next
        ' Count + 1 => Old Items + Current
        If (Count + 1) > curOrderQuantity Then Return False Else Return True
    End Function

    Private Function checkStock(ByVal strItem As String, ByVal curQuantity As Decimal, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String) As Boolean
        Dim bNegative As Boolean = False
        Dim curBalance As Decimal
        Dim ds As DataSet

        Dim Count As Integer = 0
        Dim I_items As String() = Split(SelectedInsuranceItems, ",")
        For Each item As String In I_items
            If item = strItem Then Count = Count + 1
        Next

        Dim C_items As String() = Split(SelectedCashItems, ",")
        For Each item As String In C_items
            If item = strItem Then Count = Count + 1
        Next

        'If DLookup("bNegative", "Hw_Firm") = False Then
        If bNegative = False Then '=> TODO: I don't know what is that for!
            Dim strSQL As String = "SELECT SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem='" & strItem & "' AND ST.dateTransaction <= '" & dateTransaction.ToString("yyyy-MM-dd") & "'"

            ds = dcl.GetDS(strSQL)
            If ds.Tables(0).Rows.Count > 0 Then
                curBalance = CDec("0" & ds.Tables(0).Rows(0).Item("curBalance").ToString)
            Else
                curBalance = 0
            End If

            If curBalance - (curQuantity + Count) < 0 Then
                Return False
            End If
        End If
        Return True
    End Function

    Private Function getQuantity(ByVal strReference As String, ByVal lngPatient As Long, ByVal strItem As String) As Decimal
        If strReference <> "" Then
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Hw_Treatments_Pharmacy WHERE strReference='" & strReference & "' AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0).Rows(0).Item("curQuantity")
            Else
                Return 1
            End If
        Else
            Return 1
        End If
    End Function

    Public Function fillOrders() As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance, View As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "تأمين"
                View = "عرض"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Insurance"
                View = "View"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = "Status"
        End Select

        table.Append("<table class=""table tablelist table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th>" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th><th>" & colStatus & "</th></tr></thead><tbody>")

        Try
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * OrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' ORDER BY ST.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                table.Append("<tr id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("InvoiceNo") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("PatientName") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("DoctorName") & "</td>")
                table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("InvoiceDate")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("DepartmentName") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("CompanyName") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("PaymentType") & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ");"" class=""btn btn-sm btn-primary""> " & View & " </button></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try


        table.Append("</tbody></table>")
        '             <asp:Repeater ID="repInvoices" runat="server">
        '             <ItemTemplate>
        '             <tr id="row<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>">
        '<td><%#DataBinder.Eval(Container.DataItem, "InvoiceNo")%></td>
        '<td><%#DataBinder.Eval(Container.DataItem, "PatientName")%></td>
        '<td><%#DataBinder.Eval(Container.DataItem, "DoctorName")%></td>
        '<td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "InvoiceDate")).ToString(strDateFormat)%></td>
        '<td><%#DataBinder.Eval(Container.DataItem, "DepartmentName")%></td>
        '<td><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></td>
        '<td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
        '<td><%#showStatus(DataBinder.Eval(Container.DataItem, "TransactionNo"), DataBinder.Eval(Container.DataItem, "TransactionStatus"))%></td>
        '             </tr>
        '             </ItemTemplate>
        '             </asp:Repeater>

        table.Append("<script>$('table.tablelist').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});</script>")
        Return table.ToString
    End Function
End Class
