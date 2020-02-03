Imports System.Web
Imports System.Xml
Imports System.Text

Public Class Invoices
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim func As New Functions
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

    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectChangeInvoice, PopupToPrint, TaxEnabled, AllowPrintEmptyDose, UseJSWhenViewInvoice As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, InvoiceModificationLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim p_Prepare, p_Sales, p_Cashier As Boolean

    Dim AllowCancel, AllowReturn, AllowReopen, IsAdmin As Boolean

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
        loadUserSettings()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName)
        AllowReturn = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Return_Items", "User", strUserName)
        AllowReopen = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Reopen_Invoice", "User", strUserName)
        IsAdmin = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Pharmacy/Admin", "User", strUserName)
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
        If application.SelectSingleNode("DirectChangeInvoice") Is Nothing Then DirectChangeInvoice = False Else DirectChangeInvoice = application.SelectSingleNode("DirectChangeInvoice").InnerText
        SusbendMax = application.SelectSingleNode("SusbendMax").InnerText
        If application.SelectSingleNode("UseJSWhenViewInvoice") Is Nothing Then UseJSWhenViewInvoice = False Else UseJSWhenViewInvoice = application.SelectSingleNode("UseJSWhenViewInvoice").InnerText


        'byteInvoicesLimitDay = application.SelectSingleNode("byteInvoicesLimitDay").InnerText
        OrdersLimitDays = application.SelectSingleNode("OrdersLimitDays").InnerText
        If application.SelectSingleNode("InvoiceModificationLimitDays") Is Nothing Then InvoiceModificationLimitDays = 4 Else InvoiceModificationLimitDays = application.SelectSingleNode("InvoiceModificationLimitDays").InnerText
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

    Private Sub loadUserSettings()
        p_Prepare = False
        p_Sales = False
        p_Cashier = False

        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim Actions As XmlNodeList = doc.SelectNodes("//Actions/Application[@ID = 1]/User[@ID=""" & strUserName & """]/Action")
        For Each nod As XmlNode In Actions
            Select Case nod.InnerText
                Case "1"
                    p_Prepare = True
                Case "2"
                    p_Sales = True
                Case "3"
                    p_Cashier = True
                Case "*"
                    p_Prepare = True
                    p_Sales = True
                    p_Cashier = True
            End Select
        Next
    End Sub

    Public Function fillInvoices(ByVal dateInvoice As Date, ByVal byteStatus As Byte, ByVal strSearch As String) As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance, View As String
        Dim Paid, Cancelled, Returned As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colUser, colStatus As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "آجل"
                View = "عرض"
                Paid = "مدفوعة"
                Cancelled = "ملغاة"
                Returned = "مرتجعة"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colUser = "المستخدم"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Credit"
                View = "View"
                Paid = "Paid"
                Cancelled = "Cancelled"
                Returned = "Returned"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colUser = "User"
                colStatus = "Status"
        End Select

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th><th>" & colUser & "</th><th>" & colStatus & "</th><th></th></tr></thead>")

        Dim func As String = ""
        Dim divStatus As String = ""
        Dim Where As String = ""
        Where = Where & " AND CONVERT(varchar(10), TI.dateTransaction, 120)='" & dateInvoice.ToString("yyyy-MM-dd") & "'"
        Select Case byteStatus
            Case 0
                Where = Where & " AND T.byteBase IN (18, 40) AND T.byteStatus IN (0, 1, 2)"
            Case 1
                Where = Where & " AND T.byteBase = 40 AND T.byteStatus IN (1,2)"
            Case 2
                Where = Where & " AND T.byteBase = 40 AND T.byteStatus = 0"
            Case 3
                Where = Where & " AND T.byteBase = 18 AND T.byteStatus = 1"
        End Select
        If strSearch <> "" Then
            If IsNumeric(strSearch) = True Then
                Where = Where & " AND (P.strID='" & strSearch & "' OR T.strTransaction='" & strSearch & "' OR T.strReference='" & strSearch & "' OR T.lngTransaction='" & strSearch & "' OR P.strPhone1='" & strSearch & "' OR T.lngTransaction IN (SELECT lngTransaction FROM Stock_Xlink INNER JOIN Stock_Xlink_Items ON Stock_Xlink_Items.lngXlink=Stock_Xlink.lngXlink WHERE strItem='" & strSearch & "'))"
            Else
                Where = Where & " AND (P.strFirst" & DataLang & " LIKE '%" & strSearch & "%' OR P.strSecond" & DataLang & " LIKE '%" & strSearch & "%' OR P.strThird" & DataLang & " LIKE '%" & strSearch & "%' OR P.strLast" & DataLang & " LIKE '%" & strSearch & "%')"
            End If
        End If
        Dim Disabled, btnClick As String
        Try
            'Dim temp As String = "SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus,ST.byteStatus,ST.byteBase FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON ST.lngTransaction=ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient LEFT JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment LEFT JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE Year(ST.dateTransaction) = 2019 " & Where & " ORDER BY ST.lngTransaction DESC"
            ds = dcl.GetDS("SELECT T.lngTransaction AS TransactionNo, T.dateTransaction AS TransactionDate, T.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, T.strTransaction AS InvoiceNo, T.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContactEn AS DoctorName, T.strReference AS ClinicInvoiceNo, CASE WHEN T.bCash = 1 THEN 'Cash' ELSE 'Credit' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyName, TI.strUserName AS UserName,T.byteStatus,T.byteBase FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN Hw_Departments AS D ON T.byteDepartment = D.byteDepartment LEFT JOIN Hw_Contacts AS C1 ON T.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON T.lngContact = C2.lngContact WHERE T.lngTransaction <> 0 " & Where & " ORDER BY T.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(I).Item("byteBase") = 18 Then
                    func = "viewVoucher"
                    divStatus = "<span class=""tag tag-sm tag-warning"">" & Returned & "</span>"
                Else
                    func = "viewInvoice"
                    Select Case ds.Tables(0).Rows(I).Item("byteStatus")
                        Case 0
                            divStatus = "<span class=""tag tag-sm tag-danger"">" & Cancelled & "</span>"
                        Case Else
                            divStatus = "<span class=""tag tag-sm tag-success"">" & Paid & "</span>"
                    End Select
                End If
                If p_Cashier = False And p_Sales = False Then
                    Disabled = "disabled=""disabled"""
                    btnClick = ""
                Else
                    Disabled = ""
                    btnClick = " onclick=""javascript:showModal('" & func & "', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & "}', '#mdlAlpha');"""
                End If
                table.Append("<tr class=""cursor-pointer"" " & btnClick & " id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("InvoiceNo").ToString & "</td>")
                table.Append("<td class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("PatientName").ToString & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("DoctorName").ToString & "</td>")
                If IsDBNull(ds.Tables(0).Rows(I).Item("InvoiceDate")) Then table.Append("<td class=""center"">" & CDate(ds.Tables(0).Rows(I).Item("TransactionDate")).ToString(strDateFormat) & "</td>") Else table.Append("<td class=""center"">" & CDate(ds.Tables(0).Rows(I).Item("InvoiceDate")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("DepartmentName").ToString & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("CompanyName").ToString & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("PaymentType").ToString & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("UserName").ToString & "</td>")
                table.Append("<td>" & divStatus & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showModal('" & func & "', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & "}', '#mdlAlpha');"" class=""btn btn-sm btn-primary"" " & Disabled & "> " & View & " </button></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try

        table.Append("</tbody></table>")
        'table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage});</script>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});</script>")
        Return table.ToString
    End Function

    Public Function viewInvoice(ByVal lngTransaction As Long) As String
        Dim IsCash, Cancellation, Returning, Reopening As Boolean
        Dim PatientNo, PatientName, DoctorNo, DoctorName, PharmacistName, CashierName, InvoiceNo, CompanyName, CompanyNo, VoidName As String
        Dim InvoiceDate, TransactionDate, VoidDate As Date
        Dim Status As Byte

        Dim Status_Cancelled, Status_Paid, Status_Unpaid As String

        Dim InvoiceItems As String = ""
        Dim ReturnItems As String = ""
        Dim CancellationMessage, ReturningMessage, ReopeningMessage, ReturnMessage As String
        Dim FinalMessage As String = ""

        Dim InvoiceDetails, InsuranceInvoice, CashInvoice As String
        Dim lblStatus, lblCashier, lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash, lblTotal As String
        Dim lblInvoice As String
        Dim lblCancelBy, lblCancelDate As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colDiscount, colTax, colTotal As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim btnReturnItem, btnReqReturnItem, btnCancel, btnReqCancel, btnReOpen, btnReqReOpen, btnPrint, btnClose, btnApprove, btnReject As String
        Dim ShowCancel As Boolean = False
        Dim ShowReopen As Boolean = False
        Dim CloseDate As Date

        Dim curCash, curCashVAT, curCredit, curCreditVAT, curTotal, curTotalVAT As Decimal

        Dim body As New StringBuilder("")
        Dim ds, dsItems As DataSet

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                InvoiceDetails = "تفاصيل الفاتورة"
                InsuranceInvoice = "فاتورة الآجل"
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
                colTax = "الضريبة"
                colTotal = "مجموع"
                ' Buttons
                btnReturnItem = "إعادة صنف"
                btnCancel = "إلغاء الفاتورة"
                btnReOpen = "إعادة فتح الفاتورة"
                btnPrint = "طباعة"
                btnClose = "إغلاق"
                btnReqCancel = "طلب إلغاء الفاتورة"
                btnReqReturnItem = "طلب إعادة صنف"
                btnReqReOpen = "طلب إعادة فتح الفاتورة"
                btnApprove = "تعميد"
                btnReject = "رفض"
                ' Variables
                InsuranceExtend = "right"
                CashExtend = "left"
                Status_Cancelled = "<span class=""tag tag-sm tag-danger"">ملغاة</span>"
                Status_Paid = "<span class=""tag tag-sm tag-success"">مدفوعة</span>"
                Status_Unpaid = "<span class=""tag tag-sm tag-primary"">غير مدفوعة</span>"
                CancellationMessage = "تم طلب الإلغاء بواسطة @USER في @DATE"
                ReopeningMessage = "تم طلب إعادة الفتح بواسطة @USER في @DATE"
                ReturningMessage = "تم طلب إعادة أصناف بواسطة @USER في @DATE"
                ReturnMessage = " من الأصناف تمت إعادتها"
            Case Else
                DataLang = "En"
                InvoiceDetails = "Invoice Details"
                InsuranceInvoice = "Credit Invoice"
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
                colTax = "Tax"
                colTotal = "Total"
                ' Buttons
                btnReturnItem = "Return Item"
                btnCancel = "Cancel Invoice"
                btnReOpen = "Re-Open Invoice"
                btnPrint = "Print"
                btnClose = "Close"
                btnReqCancel = "Request Cancel Invoice"
                btnReqReturnItem = "Request Return Item"
                btnReqReOpen = "Request Re-Open Invoice"
                btnApprove = "Approve"
                btnReject = "Reject"
                ' Variables
                InsuranceExtend = "left"
                CashExtend = "right"
                Status_Cancelled = "<span class=""tag tag-sm tag-danger"">Cancelled</span>"
                Status_Paid = "<span class=""tag tag-sm tag-success"">Paid</span>"
                Status_Unpaid = "<span class=""tag tag-sm tag-primary"">Unpaid</span>"
                CancellationMessage = "Cancellation request made by @USER in @DATE"
                ReopeningMessage = "Re-opening request made by @USER in @DATE"
                ReturningMessage = "Returning request made by @USER in @DATE"
                ReturnMessage = " Items has returned"
        End Select

        If lngTransaction > 0 Then
            ' Insurance
            Try
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, STI.dateTransaction AS CloseDate, STI.dateTransaction, STI.strUserName, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS PharmacistName, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.byteStatus AS [Status],STI.curCash,STI.curCashVAT,STI.curCredit,STI.curCreditVAT FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction=ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.lngTransaction = " & lngTransaction)
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
                    If IsDBNull(ds.Tables(0).Rows(0).Item("CloseDate")) Then CloseDate = Today Else CloseDate = ds.Tables(0).Rows(0).Item("CloseDate")
                    Dim InvoiceUser = ds.Tables(0).Rows(0).Item("strUserName").ToString

                    If UseJSWhenViewInvoice = True Then
                        curCash = 0
                        curCashVAT = 0
                        curCredit = 0
                        curCreditVAT = 0
                        curTotal = 0
                        curTotalVAT = 0
                    Else
                        curCash = ds.Tables(0).Rows(0).Item("curCash")
                        curCashVAT = ds.Tables(0).Rows(0).Item("curCashVAT")
                        curCredit = ds.Tables(0).Rows(0).Item("curCredit")
                        curCreditVAT = ds.Tables(0).Rows(0).Item("curCreditVAT")
                        curTotal = curCash + curCredit
                        curTotalVAT = curCashVAT + curCreditVAT
                    End If
                    

                    If (InvoiceUser = strUserName) And (DateDiff(DateInterval.Day, Today, CloseDate) = 0) Then
                        ShowCancel = True
                        ShowReopen = True
                    Else
                        ShowCancel = False
                        ShowReopen = False
                    End If

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
                    Reopening = False
                    If Status = 0 Then
                        FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center""><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelBy & ":</div><div class=""col-md-4 teal"">" & VoidName & "</div><div class=""col-md-2 text-md-right text-bold-900"">" & lblCancelDate & ":</div><div class=""col-md-4 teal"">" & VoidDate.ToString(strDateFormat & " " & strTimeFormat) & "</div></div></div>"
                    Else
                        Dim dc As New DCL.Conn.XMLData
                        If dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = True Then
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
                        ElseIf dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = True Then
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
                        ElseIf dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Reopen_Invoice", "Transaction", lngTransaction, "@status=0") = True Then
                            Dim doc As New XmlDocument
                            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Reopen_Invoice[@status=0]")
                            For Each node As XmlNode In nodes
                                If node.SelectSingleNode("Transaction").InnerText = lngTransaction Then
                                    Reopening = True
                                    ReopeningMessage = Replace(ReopeningMessage, "@USER", "<span class=""teal"">" & node.Attributes("user").Value & "</span>")
                                    ReopeningMessage = Replace(ReopeningMessage, "@DATE", "<span class=""teal"">" & CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value).ToString(strDateFormat & " " & strTimeFormat) & "</span>")
                                    FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReopeningMessage & "</div></div>"
                                End If
                            Next
                        Else
                            If ReturnedCount > 0 Then
                                FinalMessage = "<div class=""pr-2 pl-2""><div style=""margin-top:-25px;"" class=""col-md-12 border-red round text-md-center"">" & ReturnedCount & ReturnMessage & "</div></div>"
                            End If
                        End If
                    End If

                    ' build invoice header
                    'body.Append("<div class=""row""><div class=""col-md-12""><div class=""card""><div class=""card-body collapse in""><div class=""card-block"" style=""padding:10px; font-size:14px;"">")
                    'body.Append("<div class=""col-md-5 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & PatientName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblDoctor & ":</div><div class=""col-md-9 red"">" & DoctorName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblStatus & ":</div><div class=""col-md-9"">" & InvoiceStatus & "</div></div></div>")
                    'body.Append("<div class=""col-md-3 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 teal"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblPharmacist & ":</div><div class=""col-md-6 red"">" & PharmacistName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblCashier & ":</div><div class=""col-md-6 red"">" & CashierName & "</div></div></div>")
                    'body.Append("<div class=""col-md-4 m-0 p-0""><div class=""col-md-12 p-0""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-5 teal""><span class=""tag tag-sm tag-primary"" style=""width:75px;"" id=""totalCash1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCovered & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalCovered1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotal & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalAll1"">0.00</span></div></div></div>")
                    'body.Append("</div></div></div></div></div>")

                    'body.Append("<div class=""row border-grey lighten-0 m-0 mb-1 font-small-3 box-shadow-1"" style=""padding:5px;"">")
                    'body.Append("<div class=""col-md-5 m-0 p-0"">")
                    'body.Append("<div class=""col-md-2 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-10 teal"">" & PatientName & "</div>")
                    'body.Append("<div class=""col-md-2 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblDoctor & ":</div><div class=""col-md-10 red"" style=""margin-top:5px"">" & DoctorName & "</div>")
                    'body.Append("<div class=""col-md-2 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblStatus & ":</div><div class=""col-md-10"" style=""margin-top:5px"">" & InvoiceStatus & "</div>")
                    'body.Append("</div>")
                    'body.Append("<div class=""col-md-3 m-0 p-0"">")
                    'body.Append("<div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 red"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div>")
                    'body.Append("<div class=""col-md-6 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblPharmacist & ":</div><div class=""col-md-6 teal"" style=""margin-top:5px"">" & PharmacistName & "</div>")
                    'body.Append("<div class=""col-md-6 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblCashier & ":</div><div class=""col-md-6 red"" style=""margin-top:5px"">" & CashierName & "</div>")
                    'body.Append("</div>")
                    'If TaxEnabled = True Then
                    '    body.Append("<div class=""col-md-4 m-0 p-0"">")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-7 teal""><span class=""tag tag-sm tag-primary"" style=""width:60px;"" id=""totalCash1"">0.00</span><span class=""tag tag-sm tag-primary"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalVat1"">0.00</span></div>")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotalCovered & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalCovered1"">0.00</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalCoveredVat1"">0.00</span></div>")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotal & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalAll1"">0.00</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalTotalVat1"">0.00</span></div>")
                    '    body.Append("</div>")
                    'Else
                    '    body.Append("<div class=""col-md-4 m-0 p-0"">")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-7 teal""><span class=""tag tag-sm tag-primary"" style=""width:80px;"" id=""totalCash1"">0.00</span><span  style=""display:none;"" id=""totalVat1"">0.00</span></div>")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotalCovered & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalCovered1"">0.00</span><span  style=""display:none;"" id=""totalCoveredVat1"">0.00</span></div>")
                    '    body.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotal & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalAll1"">0.00</span><span  style=""display:none;"" id=""totalTotalVat1"">0.00</span></div>")
                    '    body.Append("</div>")
                    'End If
                    'body.Append("</div>")
                    Dim divCash, divCovered, divTotal As String
                    If TaxEnabled = True Then
                        divCash = "<div class=""teal""><span class=""tag tag-sm tag-primary"" style=""width:60px;"" id=""totalCash"">" & Math.Round(curCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span><span class=""tag tag-sm tag-primary"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalVat"">" & Math.Round(curCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span></div>"
                        divCovered = "<div class=""red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalCovered"">" & Math.Round(curCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalCoveredVat"">" & Math.Round(curCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span></div>"
                        divTotal = "<div class=""red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalAll"">" & Math.Round(curTotal, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalTotalVat"">" & Math.Round(curTotalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span></div>"
                    Else
                        divCash = "<div class=""teal""><span class=""tag tag-sm tag-primary"" style=""width:80px;"" id=""totalCash1"">" & Math.Round(curCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span><span  style=""display:none;"" id=""totalVat1"">0.00</span></div>"
                        divCovered = "<div class=""red""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalCovered1"">" & Math.Round(curCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</span><span  style=""display:none;"" id=""totalCoveredVat1"">0.00</span></div>"
                        divTotal = "<div class=""red""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalAll1"">0.00</span><span  style=""display:none;"" id=""totalTotalVat1"">0.00</span></div>"
                    End If
                    body.Append("<div class=""row border-grey lighten-0 m-0 mb-1 font-small-3 box-shadow-1"" style=""padding:5px;"">")
                    body.Append("<table class=""full-width"" cellpadding=""3"">")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblPatient & ":</div></td><td><div class=""teal"">" & PatientName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblDate & ":</div></td><td><div class=""red"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotalCash & ":</div></td><td>" & divCash & "</td></tr>")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblDoctor & ":</div></td><td><div class=""red"">" & DoctorName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblPharmacist & ":</div></td><td><div class=""teal"">" & PharmacistName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotalCovered & ":</div></td><td>" & divCovered & "</tr>")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblStatus & ":</div></td><td><div class="""">" & InvoiceStatus & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblCashier & ":</div></td><td><div class=""red"">" & CashierName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotal & ":</div></td><td>" & divTotal & "</td></tr>")
                    body.Append("</table>")
                    body.Append("</div>")

                    ' get general information
                    Dim MaxP, CICov, MICov As Decimal
                    If IsCash = False Then
                        Dim result As String() = func.getCoverage(PatientNo, CompanyNo)
                        If Left(result(0), 4) <> "Err:" Then
                            MaxP = result(2)
                        Else
                            Return result(0)
                        End If
                        CICov = func.getTotalClinicInvoices(PatientNo, DoctorNo, CloseDate)
                        MICov = func.getTotalPharmacyInvoices(PatientNo, DoctorNo, CloseDate, lngTransaction, False)
                    Else
                        MaxP = 0
                        CICov = 0
                        MICov = 0
                    End If

                    body.Append("<input type=""hidden"" id=""trans1"" name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter1"" value=""0"" /><input type=""hidden"" id=""coveredCash1"" name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash1"" name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash1"" name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice1"" value=""0"" /><input type=""hidden"" id=""cashOnly1"" name=""cashOnly"" value=""" & CInt(IsCash) & """ /><input type=""hidden"" id=""Limit_1"" value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_1"" value=""" & CICov & """ /><input type=""hidden"" id=""MICov_1"" value=""" & MICov & """ /><input type=""hidden"" id=""coveredVat1"" name=""coveredVat"" value=""0"" /><input type=""hidden"" id=""deductionVat1"" name=""deductionVat"" value=""0"" /><input type=""hidden"" id=""nonCoveredVat1"" name=""nonCoveredVat"" value=""0"" />")

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
                        'Dim byteUnit As Byte = dsItems.Tables(0).Rows(I).Item("byteUnit")
                        'Dim curBasePrice As Decimal = dsItems.Tables(0).Rows(I).Item("curBasePrice")
                        'Dim curBaseDiscount As Decimal = dsItems.Tables(0).Rows(I).Item("curBaseDiscount")
                        'Dim curCoverage As Decimal = dsItems.Tables(0).Rows(I).Item("curCoverage")
                        'Dim intService As Integer = dsItems.Tables(0).Rows(I).Item("intService")
                        'Dim byteWarehouse As Byte = dsItems.Tables(0).Rows(I).Item("byteWarehouse")
                        InvoiceItems = InvoiceItems & func.createItemRow(lngTransaction, 1, IsCash, CheckBox, dsItems.Tables(0).Rows(I).Item("strBarCode"), dsItems.Tables(0).Rows(I).Item("strItem"), dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), dsItems.Tables(0).Rows(I).Item("byteUnit"), dsItems.Tables(0).Rows(I).Item("dateExpiry"), dsItems.Tables(0).Rows(I).Item("curBasePrice"), dsItems.Tables(0).Rows(I).Item("curDiscount"), dsItems.Tables(0).Rows(I).Item("curQuantity"), CDec("0" & dsItems.Tables(0).Rows(I).Item("curBaseDiscount").ToString), dsItems.Tables(0).Rows(I).Item("curCoverage"), CDec("0" & dsItems.Tables(0).Rows(I).Item("curVAT").ToString), dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False, Returned)
                    Next

                    ' build invoice body
                    Dim TaxHeader_C As String = ""
                    Dim TaxHeader_I As String = ""
                    Dim TaxFooter_C As String = ""
                    Dim TaxFooter_I As String = ""
                    If TaxEnabled = True Then
                        TaxHeader_I = "<th style=""width:80px;"" class=""dynInsurance"">" & colTax & "</th>"
                        TaxHeader_C = "<th style=""width:80px;"" class=""dynCash"">" & colTax & "</th>"
                        TaxFooter_I = "<th style=""width:80px;"" class=""dynInsurance"" id=""vat_I_1"">0.00</th>"
                        TaxFooter_C = "<th style=""width:80px;"" class=""dynCash"" id=""vat_C_1"">0.00</th>"
                    End If
                    body.Append("<div class=""row"">")
                    If IsCash = False Then
                        body.Append("<div class=""col-md-12 insurance1"" id=""divInsurance1""><div class=""card border-" & InsuranceColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & InsuranceColor & " lighten-3""><span class=""icon-clipboard4 text-muted""></span> " & InsuranceInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_I & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_1"">0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_1"">0.00</th>" & TaxFooter_I & "<th></th></tr></thead></table></div></div></div></div>")
                    Else
                        body.Append("<div class=""col-md-12 cash1"" id=""divCash1""><div class=""card border-" & CashColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & CashColor & " lighten-3""><span class=""icon-money text-muted""></span> " & CashInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_C & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_1"">0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_1"">0.00</th>" & TaxFooter_C & "<th></th></tr></thead></table></div></div></div></div>")
                    End If
                    body.Append("</div>")
                    'message
                    body.Append(FinalMessage)
                    ' add scripts
                    body.Append("<script type=""text/javascript"">")
                    'If UseJSWhenViewInvoice = True Then
                    If IsCash = False Then
                        body.Append("InsuranceOn[1]=false;changeToInsurance(1);calculateInsurance(1);")
                    Else
                        body.Append("cashOn[1]=false;changeToCash(1);calculateCash(1);")
                    End If
                    'End If
                    'body.Append("function cancelThis(){cancelInvoice(" & lngTransaction & ");} function requestToCancel(){requestCancelInvoice(" & lngTransaction & ");}")
                    body.Append("function cancelThis(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: """", IsCancel: true, NextFunction: ""cancelInvoice""}','#mdlConfirm');} function requestToCancel(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: """", IsCancel: true, NextFunction: ""requestCancelInvoice""}','#mdlConfirm');}")
                    'body.Append("function returnThis(){returnItems(" & lngTransaction & ", collectChecked());} function requestToReturn(){requestReturnItems(" & lngTransaction & ", collectChecked());}")
                    body.Append("function returnThis(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: ""' + collectChecked() + '"", IsCancel: false, NextFunction: ""returnItems""}','#mdlConfirm');} function requestToReturn(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: ""' + collectChecked() + '"", IsCancel: false, NextFunction: ""requestReturnItems""}','#mdlConfirm');}")
                    body.Append("function reopenThis(){reopenInvoice(" & lngTransaction & ");} function requestToReopen(){requestReopenInvoice(" & lngTransaction & ");}")
                    If (DateDiff(DateInterval.Day, InvoiceDate, Today) > InvoiceModificationLimitDays) Or p_Cashier = False Then
                        If IsAdmin = False Then body.Append("var btnDisabled=false;") Else body.Append("var btnDisabled=true;")
                    Else
                        body.Append("var btnDisabled=true;")
                    End If
                    body.Append("var itemCount=$('.chkItem').length;var CheckedCount=0; function checkSelectedItems(){if(CheckedCount>0 && btnDisabled!=false) $('#btnReturn').prop('disabled', false); else $('#btnReturn').prop('disabled', true);}checkSelectedItems();")
                    body.Append("var countChecked = function() {var n = $('input:checked').length;CheckedCount=n;checkSelectedItems();};countChecked();$('input[type=checkbox]').on('click', countChecked );")
                    body.Append("function collectChecked(){var str='';jQuery.each( $('input:checked'), function( i, val ) {str += val.value + ','});return str.substring(0, str.length-1);}")
                    body.Append("$('#mdlConfirm').on('keyup', function () {if (event.which == 13) {proceed();}});")
                    body.Append("</script>")

                    Dim ActionDisabled As String = ""
                    Dim CancelButton As String = ""
                    Dim ReturnButton As String = ""
                    Dim ReOpenButton As String = ""
                    Dim ApproveButton As String = ""
                    Dim RejectButton As String = ""
                    'show buttons only if not cancelled or returned
                    If (Status > 0) And (ReturnedCount = 0) Then
                        If Cancellation = True Then
                            If AllowCancel = True Or IsAdmin = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveCancelRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectCancelRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        ElseIf Returning = True Then
                            If AllowReturn = True Or IsAdmin = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveReturnRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectReturnRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        ElseIf Reopening = True Then
                            If AllowReopen = True Or IsAdmin = True Then
                                ApproveButton = "<button type=""button"" class=""btn btn-success"" id=""btnApprove"" onclick=""javascript:approveReopenRequest(" & lngTransaction & ");"" " & ActionDisabled & "><i class=""icon-check""></i> " & btnApprove & "</button>"
                                RejectButton = "<button type=""button"" class=""btn btn-danger ml-1 mr-3"" id=""btnReject"" onclick=""javascript:rejectReopenRequest(" & lngTransaction & ");""><i class=""icon-cross""></i> " & btnReject & "</button>"
                            End If
                        Else
                            If (DateDiff(DateInterval.Day, InvoiceDate, Today) > InvoiceModificationLimitDays) Or p_Cashier = False Then If IsAdmin = False Then ActionDisabled = "disabled=""disabled"""
                            If ShowCancel = True Or IsAdmin = True Then
                                If DirectChangeInvoice = True Or AllowCancel Or IsAdmin = True Then
                                    CancelButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:cancelThis();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                                Else
                                    CancelButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:requestToCancel();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                                End If
                            End If
                            If DirectChangeInvoice = True Or AllowReturn Or IsAdmin = True Then
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red ml-1"" id=""btnReturn"" onclick=""javascript:returnThis();"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReturnItem & "</button>"
                            Else
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red ml-1"" id=""btnReturn"" onclick=""javascript:requestToReturn();"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReturnItem & "</button>"
                            End If
                            If ShowReopen = True Or IsAdmin = True Then
                                If DirectChangeInvoice = True Or AllowReopen Or IsAdmin = True Then
                                    ReOpenButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnReOpen"" onclick=""javascript:confirm('', 'Are you sure to re-open this invoice?', reopenThis)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReOpen & "</button>"
                                Else
                                    ReOpenButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnReOpen"" onclick=""javascript:confirm('', 'Do you want to request to re-open this invoice?', requestToReopen)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReOpen & "</button>"
                                End If
                            End If
                        End If
                    Else
                        If (DateDiff(DateInterval.Day, InvoiceDate, Today) > InvoiceModificationLimitDays) Or p_Cashier = False Then If IsAdmin = False Then ActionDisabled = "disabled=""disabled"""
                        If ShowReopen = True Then
                            If DirectChangeInvoice = True Or AllowReopen Or IsAdmin = True Then
                                ReOpenButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnReOpen"" onclick=""javascript:confirm('', 'Are you sure to re-open this invoice?', reopenThis)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReOpen & "</button>"
                            Else
                                ReOpenButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnReOpen"" onclick=""javascript:confirm('', 'Do you want to request to re-open this invoice?', requestToReopen)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReOpen & "</button>"
                            End If
                        End If
                    End If
                    Dim buttons As String = "<div class=""text-md-center"">" & CancelButton & ReturnButton & ReOpenButton & ApproveButton & RejectButton & "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & InvoicePrinter & """ app-url=""p_invoice.aspx?t=" & lngTransaction & """><a href=""p_invoice.aspx?t=" & lngTransaction & """ target=""_blank"" class=""btn btn-blue-grey ml-1 printInvoice""><i class=""icon-printer""></i> " & btnPrint & "</a></span><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

                    Dim sh As New Share.UI
                    Dim str As String = sh.drawModal(func.getHeaderSubMenu(InvoiceDetails, False), body.ToString, buttons, Share.UI.ModalSize.Large)
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

    Public Function viewReturnAmount(ByVal lngTransaction As Long, ByVal lstItems As String, ByVal IsCancel As Boolean, ByVal NextFunction As String) As String
        Dim DataLang As String
        Dim btnYes, btnNo As String
        Dim lblCash, lblVAT, lblTotal As String
        Dim msgCount, msgReturn, msgProceed As String
        Dim Header, cbStatus(2) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "قيمة الإرجاع"
                cbStatus(0) = "مفتوح"
                cbStatus(1) = "مغلق"
                lblCash = "النقدي"
                lblVAT = "الضريبة"
                lblTotal = "الإجمالي"
                msgCount = "ستقوم بإعادة عدد <b>@COUNT@</b> أصناف"
                msgReturn = "يتوجب عليك إعادة"
                msgProceed = "هل أنت متأكد من المتابعة؟"
                btnYes = "نعم"
                btnNo = "لا"
            Case Else
                DataLang = "En"
                Header = "Returning Amount"
                cbStatus(0) = "Opened"
                cbStatus(1) = "Closed"
                lblCash = "Cash"
                lblVAT = "VAT"
                lblTotal = "Total"
                msgCount = "You are returning <b>@COUNT@</b> items"
                msgReturn = "You have to return"
                msgProceed = "Are you sure to proceed?"
                btnYes = "Yes"
                btnNo = "No"
        End Select

        Dim ds As DataSet
        Dim dsItems As DataSet
        Dim totalItems As Integer = 0
        Dim items() As String = Split(lstItems, ",")
        'Validation
        If IsCancel = False And items.Length = 0 Then Return "Err:No items selected.."
        dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
        totalItems = dsItems.Tables(0).Rows.Count
        If IsCancel = False And totalItems = 0 Then Return "Err:No items in this invoice.."
        Dim Content As New StringBuilder("")
        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND Year(ST.dateTransaction) = " & intYear & " AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    ' Get listItems exact amount
                    Dim JSFunction As String
                    Dim cashAmount, creditAmount, cashVAT, creditVAT, Total As Decimal
                    If IsCancel = True Then
                        Dim dsInvoice As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Invoices WHERE lngTransaction=" & lngTransaction)
                        creditAmount = dsInvoice.Tables(0).Rows(0).Item("curCredit")
                        cashAmount = dsInvoice.Tables(0).Rows(0).Item("curCash")
                        creditVAT = dsInvoice.Tables(0).Rows(0).Item("curCreditVAT")
                        cashVAT = dsInvoice.Tables(0).Rows(0).Item("curCashVAT")
                        Total = cashAmount + cashVAT

                        JSFunction = NextFunction & "(" & lngTransaction & ");"
                    Else
                        Dim res As Decimal()
                        res = func.getReturnedItemsValues(lngTransaction, lstItems)
                        creditAmount = res(0)
                        cashAmount = res(1)
                        creditVAT = res(2)
                        cashVAT = res(3)
                        Total = cashAmount + cashVAT

                        JSFunction = NextFunction & "(" & lngTransaction & ",'" & lstItems & "');"
                    End If

                    Content.Append("<div class=""row"">")
                    If IsCancel = False Then Content.Append("<div class=""col-md-12 text-md-center""><h4>" & Replace(msgCount, "@COUNT@", items.Length) & "</h4></div><br /><br />")
                    Content.Append("<div class=""col-md-12 text-md-center"">" & msgReturn & ":</div>")
                    If TaxEnabled = True Then
                        Content.Append("<div class=""col-md-12 p-0 m-0""><div class=""col-md-3 m-0 p-0 text-md-center"">" & lblCash & "</div><div class=""col-md-1 m-0""></div><div class=""col-md-3 m-0 text-md-center"">" & lblVAT & "</div><div class=""col-md-1 m-0""></div><div class=""col-md-4 m-0 text-md-center""><b>" & lblTotal & "</b></div></div>")
                        Content.Append("<div class=""col-md-12 p-0 m-0""><div class=""col-md-3 m-0""><input type=""text"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Math.Round(cashAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></div><div class=""col-md-1 m-0 text-md-center""><b>+</b></div><div class=""col-md-3 m-0""><input type=""text"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Math.Round(cashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></div><div class=""col-md-1 m-0 text-md-center""><b>=</b></div><div class=""col-md-4 m-0""><input type=""text"" class=""form-control form-control-sm text-md-center border border-red text-bold-900"" readonly=""readonly"" value=""" & Math.Round(Total, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></div></div>")
                    Else
                        Content.Append("<div class=""col-md-12 p-0 m-0""><div class=""col-md-4 m-0 p-0 text-md-center""></div><div class=""col-md-4 m-0 p-0 text-md-center""><b>" & lblCash & "</b></div><div class=""col-md-4 m-0 p-0 text-md-center""></div></div>")
                        Content.Append("<div class=""col-md-12 p-0 m-0""><div class=""col-md-4 m-0 p-0 text-md-center""></div><div class=""col-md-4 m-0""><input type=""text"" class=""form-control form-control-sm text-md-center border border-red text-bold-900"" readonly=""readonly"" value=""" & Math.Round(cashAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></div><div class=""col-md-4 m-0 p-0 text-md-center""></div></div>")
                    End If
                    Content.Append("<div class=""col-md-12 text-md-center""><h4><b>" & msgProceed & "</b><h4></div>")
                    Content.Append("</div>")
                    Content.Append("<script type=""text/javascript"">function proceed() {$('#mdlConfirm').modal('hide');" & JSFunction & "}</script>")

                    Dim buttons As String = "<button type=""button"" class=""btn btn-success""id=""btnProceed"" onclick=""javascript:proceed();""><i class=""icon-check""></i> " & btnYes & "</button> <button type=""button"" class=""btn btn-danger"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnNo & "</button>"
                    Dim mdl As New Share.UI
                    Return mdl.drawModal(Header, Content.ToString, buttons, Share.UI.ModalSize.Small, "bg-teal bg-lighten-4")
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

        Dim VoucherDetails, InsuranceInvoice, CashInvoice As String
        Dim lblStatus, lblCashier, lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash, lblTotal As String
        Dim lblInvoice As String
        Dim lblCancelBy, lblCancelDate As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colTax, colDiscount, colTotal As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim btnReturnItem, btnReqReturnItem, btnCancel, btnReqCancel, btnPrint, btnClose, btnApprove, btnReject As String

        Dim body As New StringBuilder("")
        Dim ds, dsItems As DataSet

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                VoucherDetails = "تفاصيل السند"
                InsuranceInvoice = "فاتورة الآجل"
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
                colTax = "الضريبة"
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
                VoucherDetails = "Voucher Details"
                InsuranceInvoice = "Credit Invoice"
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
                colTax = "Tax"
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
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateTransaction AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS PharmacistName, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.byteStatus AS [Status] FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient LEFT JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment LEFT JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 18 AND Year(ST.dateTransaction) = " & intYear & " AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    ' get general data
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo")
                    PatientName = ds.Tables(0).Rows(0).Item("PatientName").ToString
                    'PatientFirstName = ds.Tables(0).Rows(0).Item("PatientFirstName").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo")
                    CompanyName = ds.Tables(0).Rows(0).Item("CompanyName").ToString
                    If IsDBNull(ds.Tables(0).Rows(0).Item("InvoiceDate")) Then InvoiceDate = ds.Tables(0).Rows(0).Item("TransactionDate") Else InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo").ToString
                    DoctorName = ds.Tables(0).Rows(0).Item("DoctorName").ToString
                    TransactionDate = ds.Tables(0).Rows(0).Item("TransactionDate")
                    IsCash = ds.Tables(0).Rows(0).Item("bCash")
                    PharmacistName = ds.Tables(0).Rows(0).Item("PharmacistName").ToString
                    CashierName = ds.Tables(0).Rows(0).Item("CashierName").ToString
                    'InvoiceDate = ds.Tables(0).Rows(0).Item("InvoiceDate")
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
                        If dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "Transaction", lngTransaction, "@status=0") = True Then
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
                        ElseIf dc.CheckExistElement(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "Transaction", lngTransaction, "@status=0") = True Then
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
                    Dim divCash, divCovered, divTotal As String
                    If TaxEnabled = True Then
                        divCash = "<div class=""teal""><span class=""tag tag-sm tag-primary"" style=""width:60px;"" id=""totalCash1"">0.00</span><span class=""tag tag-sm tag-primary"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalVat1"">0.00</span></div>"
                        divCovered = "<div class=""red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalCovered1"">0.00</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalCoveredVat1"">0.00</span></div>"
                        divTotal = "<div class=""red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalAll1"">0.00</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalTotalVat1"">0.00</span></div>"
                    Else
                        divCash = "<div class=""teal""><span class=""tag tag-sm tag-primary"" style=""width:80px;"" id=""totalCash1"">0.00</span><span  style=""display:none;"" id=""totalVat1"">0.00</span></div>"
                        divCovered = "<div class=""red""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalCovered1"">0.00</span><span  style=""display:none;"" id=""totalCoveredVat1"">0.00</span></div>"
                        divTotal = "<div class=""red""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalAll1"">0.00</span><span  style=""display:none;"" id=""totalTotalVat1"">0.00</span></div>"
                    End If
                    body.Append("<div class=""row border-grey lighten-0 m-0 mb-1 font-small-3 box-shadow-1"" style=""padding:5px;"">")
                    body.Append("<table class=""full-width"" cellpadding=""3"">")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblPatient & ":</div></td><td><div class=""teal"">" & PatientName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblDate & ":</div></td><td><div class=""red"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotalCash & ":</div></td><td>" & divCash & "</td></tr>")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblDoctor & ":</div></td><td><div class=""red"">" & DoctorName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblPharmacist & ":</div></td><td><div class=""teal"">" & PharmacistName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotalCovered & ":</div></td><td>" & divCovered & "</tr>")
                    body.Append("<tr><td><div class=""text-md-right text-bold-900"">" & lblStatus & ":</div></td><td><div class="""">" & InvoiceStatus & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblCashier & ":</div></td><td><div class=""red"">" & CashierName & "</div></td><td><div class=""text-md-right text-bold-900"">" & lblTotal & ":</div></td><td>" & divTotal & "</td></tr>")
                    body.Append("</table>")
                    body.Append("</div>")

                    'body.Append("<div class=""row""><div class=""col-md-12""><div class=""card""><div class=""card-body collapse in""><div class=""card-block"" style=""padding:10px; font-size:14px;"">")
                    'body.Append("<div class=""col-md-5 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & PatientName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblDoctor & ":</div><div class=""col-md-9 red"">" & DoctorName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-3 text-md-right text-bold-900"">" & lblStatus & ":</div><div class=""col-md-9"">" & InvoiceStatus & "</div></div></div>")
                    'body.Append("<div class=""col-md-3 m-0 p-0""><div class=""col-md-12 m-0 p-0""><div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 teal"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblPharmacist & ":</div><div class=""col-md-6 red"">" & PharmacistName & "</div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-6 text-md-right text-bold-900"">" & lblCashier & ":</div><div class=""col-md-6 red"">" & CashierName & "</div></div></div>")
                    'body.Append("<div class=""col-md-4 m-0 p-0""><div class=""col-md-12 p-0""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-5 teal""><span class=""tag tag-sm tag-primary"" style=""width:75px;"" id=""totalCash1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotalCovered & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalCovered1"">0.00</span></div></div><div class=""col-md-12 p-0"" style=""margin-top:10px;""><div class=""col-md-7 text-md-right text-bold-900"">" & lblTotal & ":</div><div class=""col-md-5 red""><span class=""tag tag-sm tag-default"" style=""width:75px;"" id=""totalAll1"">0.00</span></div></div></div>")
                    'body.Append("</div></div></div></div></div>")

                    ' get general information
                    Dim MaxP, CICov, MICov As Decimal
                    If IsCash = False Then
                        Dim result As String() = func.getCoverage(PatientNo, CompanyNo)
                        If Left(result(0), 4) <> "Err:" Then
                            MaxP = result(2)
                        Else
                            Return result(0)
                        End If
                        CICov = func.getTotalClinicInvoices(PatientNo, DoctorNo, TransactionDate)
                        MICov = func.getTotalPharmacyInvoices(PatientNo, DoctorNo, TransactionDate, lngTransaction, False)
                    Else
                        MaxP = 0
                        CICov = 0
                        MICov = 0
                    End If

                    'body.Append("<input type=""hidden"" id=""trans1"" name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter1"" value=""0"" /><input type=""hidden"" id=""coveredCash1"" name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash1"" name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash1"" name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice1"" value=""0"" /><input type=""hidden"" id=""cashOnly1"" name=""cashOnly"" value=""" & CInt(IsCash) & """ /><input type=""hidden"" id=""Limit_1"" value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_1"" value=""" & CICov & """ /><input type=""hidden"" id=""MICov_1"" value=""" & MICov & """ />")
                    body.Append("<input type=""hidden"" id=""trans1"" name=""lngTransaction"" value=""" & lngTransaction & """ /><input type=""hidden"" id=""counter1"" value=""0"" /><input type=""hidden"" id=""coveredCash1"" name=""coveredCash"" value=""0"" /><input type=""hidden"" id=""deductionCash1"" name=""deductionCash"" value=""0"" /><input type=""hidden"" id=""nonCoveredCash1"" name=""nonCoveredCash"" value=""0"" /><input type=""hidden"" id=""basePrice1"" value=""0"" /><input type=""hidden"" id=""cashOnly1"" name=""cashOnly"" value=""" & CInt(IsCash) & """ /><input type=""hidden"" id=""Limit_1"" value=""" & MaxP & """ /><input type=""hidden"" id=""CICov_1"" value=""" & CICov & """ /><input type=""hidden"" id=""MICov_1"" value=""" & MICov & """ /><input type=""hidden"" id=""coveredVat1"" name=""coveredVat"" value=""0"" /><input type=""hidden"" id=""deductionVat1"" name=""deductionVat"" value=""0"" /><input type=""hidden"" id=""nonCoveredVat1"" name=""nonCoveredVat"" value=""0"" />")

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
                        InvoiceItems = InvoiceItems & func.createItemRow(lngTransaction, 1, IsCash, CheckBox, dsItems.Tables(0).Rows(I).Item("strBarCode").ToString, dsItems.Tables(0).Rows(I).Item("strItem").ToString, dsItems.Tables(0).Rows(I).Item("strItem" & DataLang).ToString, dsItems.Tables(0).Rows(I).Item("byteUnit"), CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")), CDec("0" & dsItems.Tables(0).Rows(I).Item("curBasePrice").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curDiscount").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curQuantity").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curBaseDiscount").ToString), CDec("0" & dsItems.Tables(0).Rows(I).Item("curCoverage").ToString), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False, Returned)
                    Next

                    ' build invoice body
                    Dim TaxHeader_C As String = ""
                    Dim TaxHeader_I As String = ""
                    Dim TaxFooter_C As String = ""
                    Dim TaxFooter_I As String = ""
                    If TaxEnabled = True Then
                        TaxHeader_I = "<th style=""width:80px;"" class=""dynInsurance"">" & colTax & "</th>"
                        TaxHeader_C = "<th style=""width:80px;"" class=""dynCash"">" & colTax & "</th>"
                        TaxFooter_I = "<th style=""width:80px;"" class=""dynInsurance"" id=""vat_I_1"">0.00</th>"
                        TaxFooter_C = "<th style=""width:80px;"" class=""dynCash"" id=""vat_C_1"">0.00</th>"
                    End If
                    body.Append("<div class=""row"">")
                    If IsCash = False Then
                        body.Append("<div class=""col-md-12 insurance1"" id=""divInsurance1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_I & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_1"">0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_1"">0.00</th>" & TaxFooter_I & "<th></th></tr></thead></table></div></div></div></div>")
                    Else
                        body.Append("<div class=""col-md-12 cash1"" id=""divCash1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                        body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_C & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash1""><tbody>")
                        body.Append(InvoiceItems)
                        body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_1"">0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_1"">0.00</th>" & TaxFooter_C & "<th></th></tr></thead></table></div></div></div></div>")
                    End If
                    body.Append("</div>")

                    'body.Append("<div class=""row"">")
                    'If IsCash = False Then
                    '    body.Append("<div class=""col-md-12 insurance1"" id=""divInsurance1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                    '    body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance1""><tbody>")
                    '    body.Append(InvoiceItems)
                    '    body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_1"">0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_I_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    'Else
                    '    body.Append("<div class=""col-md-12 cash1"" id=""divCash1""><div class=""card border-" & VoucherColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & VoucherColor & " lighten-3""><span class=""icon-mail-forward text-muted""></span> " & ReturnVoucher & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & lblInvoice & ": (<span class=""orange"">" & InvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CompanyName & """>" & CompanyName & "</span></div></div>")
                    '    body.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:100px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th><th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash1""><tbody>")
                    '    body.Append(InvoiceItems)
                    '    body.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:100px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_1"">0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;""></th><th style=""width:80px;"" id=""total_C_1"">0.00</th><th></th></tr></thead></table></div></div></div></div>")
                    'End If
                    'body.Append("</div>")
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
                            If (DateDiff(DateInterval.Day, InvoiceDate, Today) > InvoiceModificationLimitDays) Then ActionDisabled = "disabled=""disabled"""
                            If (DirectChangeInvoice = True) Or (AllowCancel = True) Then
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Are you sure to cancel this invoice?', cancelThis)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Are you sure to return selected items?', returnThis)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReturnItem & "</button>"
                            Else
                                CancelButton = "<button type=""button"" class=""btn btn-outline-red ml-1 mr-3"" id=""btnCancel"" onclick=""javascript:confirm('', 'Do you want to request to cancel this invoice?', requestToCancel)"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnReqCancel & "</button>"
                                ReturnButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnReturn"" onclick=""javascript:confirm('', 'Do you want to request to return selected items?', requestToReturn)"" " & ActionDisabled & "><i class=""icon-share3""></i> " & btnReqReturnItem & "</button>"
                            End If
                        End If
                    End If
                    ' Dim buttons As String = "<div class=""text-md-center""><a href=""#"" class=""btn btn-blue-grey ml-1 printInvoice""><i class=""icon-printer""></i> " & btnPrint & "</a><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"
                    Dim buttons As String = "<div class=""text-md-center""><span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & InvoicePrinter & """ app-url=""p_invoice.aspx?t=" & lngTransaction & """><a href=""p_invoice.aspx?t=" & lngTransaction & """ target=""_blank"" class=""btn btn-blue-grey ml-1 printInvoice""><i class=""icon-printer""></i> " & btnPrint & "</a></span><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

                    Dim sh As New Share.UI
                    Dim str As String = sh.drawModal(func.getHeaderSubMenu(VoucherDetails, False), body.ToString, buttons, Share.UI.ModalSize.Large)
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

    Public Function cancelInvoice(ByVal lngTransaction As Long, Optional ByVal VoidUser As String = "") As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        If VoidUser = "" Then VoidUser = strUserName

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND CONVERT(varchar(10), STI.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * InvoiceModificationLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=0 WHERE lngTransaction=" & lngTransaction)
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strVoidBy='" & VoidUser & "',dateVoid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "',strApprovedBy='" & strUserName & "',dateApproved='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "' WHERE lngTransaction=" & lngTransaction)
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 2, "Cancel Invoice")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','This invoice cancelled successfully!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
    End Function

    Public Function reopenInvoice(ByVal lngTransaction As Long) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND CONVERT(varchar(10), STI.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * InvoiceModificationLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteBase=50, byteStatus=2 WHERE lngTransaction=" & lngTransaction)
                    'dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strVoidBy='" & strUserName & "',dateVoid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "' WHERE lngTransaction=" & lngTransaction)
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Invoice", lngTransaction, 2, "Re-Open invoice")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','This invoice re-opened successfully!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
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
        'If dsItems.Tables(0).Rows.Count <= items.Length Then Return "Err:Cannot return all items, please cancel the invoice.."

        'For I = 0 To dsItems.Tables(0).Rows.Count - 1
        '    'InvoiceItems = InvoiceItems & "<tr id=""tr_1"" class=""Ctr""><td style=""width:32px;""><input type=""hidden"" name=""barcode_C"" value=""" & dsItems.Tables(0).Rows(I).Item("strBarCode") & """/><input type=""hidden"" name=""dose_C"" value=""""/><input type=""hidden"" name=""item_C"" class=""item_C"" value=""" & dsItems.Tables(0).Rows(I).Item("strItem") & """/></td><td style=""width:70px;"" class=""dynCash"">" & dsItems.Tables(0).Rows(I).Item("strItem") & "</td><td class=""itemName width-150"" title=""" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td><td style=""width:100px;"" class=""dynCash red"">" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString(strDateFormat) & "<input type=""hidden"" name=""expire_C"" value=""" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""price_C"" class=""price_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""service_C"" value=""" & dsItems.Tables(0).Rows(I).Item("intService") & """/><input type=""hidden"" name=""warehouse_C"" value=""" & dsItems.Tables(0).Rows(I).Item("byteWarehouse") & """/></td><td style=""width:80px;"" class=""dynCash"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""percent_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curDiscount"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""discount"" name=""discount_C"" class=""discount_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td style=""width:44px;"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""quantity"" name=""quantity_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" name=""unit_C"" value=""" & dsItems.Tables(0).Rows(I).Item("byteUnit") & """/></td><td style=""width:80px;"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""total_C"" class=""total_C"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curCoverage"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & Math.Round(0, byteCurrencyRound, MidpointRounding.AwayFromZero) & """/></td><td class=""text-nowrap""></td></tr>"
        '    InvoiceItems = InvoiceItems & createItemRow(lngTransaction, 1, IsCash, "<input type=""checkbox"" class=""chkItem"" value=""" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & """ />", dsItems.Tables(0).Rows(I).Item("strBarCode"), dsItems.Tables(0).Rows(I).Item("strItem"), dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), dsItems.Tables(0).Rows(I).Item("byteUnit"), dsItems.Tables(0).Rows(I).Item("dateExpiry"), dsItems.Tables(0).Rows(I).Item("curBasePrice"), dsItems.Tables(0).Rows(I).Item("curDiscount"), dsItems.Tables(0).Rows(I).Item("curQuantity"), dsItems.Tables(0).Rows(I).Item("curBaseDiscount"), dsItems.Tables(0).Rows(I).Item("curCoverage"), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False)
        'Next

        If lngTransaction > 0 Then
            Try
                ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus = 1 AND ST.lngTransaction = " & lngTransaction)
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
                    ' Get ReturnDate and cash user
                    Dim dateReturn As Date = Now()
                    Dim strCashBy As String = strUserName
                    Dim doc As New XmlDocument
                    doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
                    Dim node As XmlNode = doc.DocumentElement.SelectSingleNode("Return_Items[@status=0 and Transaction=" & lngTransaction & "]")
                    If Not (node Is Nothing) Then
                        dateReturn = CDate(node.Attributes("date").Value & " " & node.Attributes("time").Value)
                        strCashBy = node.Attributes("user").Value
                    End If
                    ' Create a return invoice (byteDepartment & lngSalesman has been added)
                    lngTransaction_New = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, dateTransaction, lngContact, byteStatus, bCash, dateClosedValid, byteCurrency, byteWarehouse, lngPatient, strRemarks, byteDepartment, lngSalesman) VALUES (18, " & byteTransType & ",'" & ds.Tables(0).Rows(0).Item("strTransaction") & "', '" & dateReturn.ToString("yyyy-MM-dd") & "' , " & ds.Tables(0).Rows(0).Item("lngContact") & ", 1, " & CInt(ds.Tables(0).Rows(0).Item("bCash")) & ", '" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', " & byteLocalCurrency & ", 3, " & ds.Tables(0).Rows(0).Item("lngPatient") & ", '" & ds.Tables(0).Rows(0).Item("strRemarks") & "', " & ds.Tables(0).Rows(0).Item("byteDepartment") & ", " & ds.Tables(0).Rows(0).Item("lngSalesman") & ")")
                    ' Create xlink pointer
                    lngXling_New = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction, lngPointer) VALUES (" & lngTransaction_New & ", " & lngTransaction & ")")
                    ' Add returned items to the return invoice
                    dsSelectedItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND intEntryNumber IN (" & lstItems & ")")
                    For I = 0 To dsSelectedItems.Tables(0).Rows.Count - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,curBasePrice,curDiscount,curUnitPrice,curCoverage,dateExpiry,byteDepartment,intService,byteWarehouse) VALUES (" & lngXling_New & "," & dsSelectedItems.Tables(0).Rows(I).Item("intEntryNumber") & ",'" & dsSelectedItems.Tables(0).Rows(I).Item("strItem") & "'," & dsSelectedItems.Tables(0).Rows(I).Item("byteUnit") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curQuantity") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curBasePrice") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curDiscount") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curUnitPrice") & "," & dsSelectedItems.Tables(0).Rows(I).Item("curCoverage") & ",'" & CDate(dsSelectedItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "'," & dsSelectedItems.Tables(0).Rows(I).Item("byteDepartment") & "," & dsSelectedItems.Tables(0).Rows(I).Item("intService") & ",3)")
                        dcl.ExecSQuery("UPDATE Stock_Xlink_Items SET bCopied=1 WHERE lngXlink=" & lngXlink & " AND intEntryNumber=" & dsSelectedItems.Tables(0).Rows(I).Item("intEntryNumber"))
                    Next
                    ' insert the audit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated, strApprovedBy, dateApproved, strCashBy, dateCash) VALUES (" & lngTransaction_New & ", '" & strCashBy & "', '" & dateReturn.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & strUserName & "', '" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & strCashBy & "', '" & dateReturn.ToString("yyyy-MM-dd HH:mm:ss") & "')")
                    '---------------New Table
                    ' Get listItems exact amount
                    Dim res As Decimal()
                    res = func.getReturnedItemsValues(lngTransaction, lstItems)
                    Dim cashAmount, creditAmount, cashVAT, creditVAT As Decimal
                    creditAmount = res(0) * -1
                    cashAmount = res(1) * -1
                    creditVAT = res(2) * -1
                    cashVAT = res(3) * -1
                    ' insert Invoice
                    Dim dsInvoice As DataSet
                    dsInvoice = dcl.GetDS("SELECT * FROM Stock_Trans_Invoices WHERE lngTransaction=" & lngTransaction_New)
                    If dsInvoice.Tables(0).Rows.Count > 0 Then
                        dcl.ExecScalar("UPDATE Stock_Trans_Invoices SET curCredit=" & creditAmount & ", curCash=" & cashAmount & ",curCreditVAT=" & creditVAT & ", curCashVAT=" & cashVAT & ", dateTransaction='" & dateReturn.ToString("yyyy-MM-dd HH:mm:ss") & "', strUserName='" & strCashBy & "' WHERE lngTransaction=" & lngTransaction_New)
                    Else
                        dcl.ExecSQuery("INSERT INTO Stock_Trans_Invoices VALUES (" & lngTransaction_New & "," & creditAmount & "," & cashAmount & "," & creditVAT & "," & cashVAT & ",1,'" & dateReturn.ToString("yyyy-MM-dd HH:mm:ss") & "','" & strCashBy & "')")
                    End If
                    ' delete payment
                    dcl.ExecSQuery("DELETE FROM Stock_Trans_Payments WHERE lngTransaction=" & lngTransaction_New)
                    ' add payment (Cash only) no return credit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_New & ", 1, " & cashAmount + cashVAT & ", 0)")
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strCashBy, Now, 1, "Invoice", lngTransaction, 2, "Return Items")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct invoice"
        End If

        Return "<script type=""text/javascript"">msg('','The selected items returned successfully!','notice');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
    End Function
End Class
