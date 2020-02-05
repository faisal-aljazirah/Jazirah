Imports System.Web
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml

Public Class Orders
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim func As New Functions
    Public byteLocalCurrency As Byte
    Public intStartupFY As Integer
    Public intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Public byteCurrencyRound As Byte
    Dim byteWarehouse As Byte = 3
    Dim PharmacyDB As String = "Pharmacy" 'Default

    Const TableHeight As Integer = 149
    Const InsuranceColor As String = "blue"
    Const CashColor As String = "red"

    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, AllowPrintEmptyDose, LogsEnabled, ErrorLogsEnabled As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, byteOrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice, SalesmanType As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter, CreditContacts As String
    Dim AllowCancel As Boolean
    Dim p_Prepare, p_Sales, p_Cashier As Boolean

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

        If PharmacyDB <> "" Then PharmacyDB = PharmacyDB & ".."

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
        loadAppSettings()
        loadUserSettings()
        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName)
    End Sub

    Private Sub loadAppSettings()
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
        If application.SelectSingleNode("SalesmanType") Is Nothing Then SalesmanType = 0 Else SalesmanType = application.SelectSingleNode("SalesmanType").InnerText

        'byteInvoicesLimitDay = application.SelectSingleNode("byteInvoicesLimitDay").InnerText
        byteOrdersLimitDays = application.SelectSingleNode("OrdersLimitDays").InnerText
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

        If application.SelectSingleNode("CreditContacts") Is Nothing Then CreditContacts = "380,1740" Else CreditContacts = application.SelectSingleNode("CreditContacts").InnerText
        If application.SelectSingleNode("LogsEnabled") Is Nothing Then LogsEnabled = True Else LogsEnabled = application.SelectSingleNode("LogsEnabled").InnerText
        If application.SelectSingleNode("ErrorLogsEnabled") Is Nothing Then ErrorLogsEnabled = True Else ErrorLogsEnabled = application.SelectSingleNode("ErrorLogsEnabled").InnerText

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

    Public Function createOrder() As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim lblSalesman, lblContact, lblPatient As String
        Dim drpContact, drpPatient As String
        Dim btnOpen, btnClose As String


        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                lblSalesman = "الطبيب"
                lblContact = "الشركة"
                lblPatient = "المريض"
                btnOpen = "فتح الطلب"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblSalesman = "Doctor"
                lblContact = "Company"
                lblPatient = "Patient"
                btnOpen = "Open Order"
                btnClose = "Close"
        End Select
        Dim dsContact, dsPatient As DataSet

        drpContact = "<select class=""form-control"" id=""drpContact""  onchange=""javascript:getCreditPatients(this.value)"">"
        dsContact = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact IN (" & CreditContacts & ") ORDER BY lngContact")
        For I = 0 To dsContact.Tables(0).Rows.Count - 1
            drpContact = drpContact & "<option value=""" & dsContact.Tables(0).Rows(I).Item("lngContact") & """>" & dsContact.Tables(0).Rows(I).Item("strContact" & DataLang).ToString & "</option>"
        Next
        drpContact = drpContact & "</select>"

        drpPatient = "<select class=""form-control"" id=""drpPatient"">"
        dsPatient = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients AS P INNER JOIN Hw_Patients_Details AS PD ON P.lngPatient=PD.lngPatient WHERE P.lngGuarantor=380 AND PD.bCredit=1")
        For I = 0 To dsPatient.Tables(0).Rows.Count - 1
            drpPatient = drpPatient & "<option value=""" & dsPatient.Tables(0).Rows(I).Item("lngPatient") & """>" & dsPatient.Tables(0).Rows(I).Item("PatientName").ToString & "</option>"
        Next
        drpPatient = drpPatient & "</select>"
        Dim body As New StringBuilder("")

        body.Append("<div class=""row"">")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-3 text-bold-900 text-md-right"">" & lblSalesman & ":</div><div class=""col-md-9""><input type=""text"" id=""txtSalesmanName"" class=""form-control"" readonly=""readonly"" value=""" & strSalesman_Cash & """ /></div></div>")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-3 text-bold-900 text-md-right"">" & lblContact & ":</div><div class=""col-md-9"">" & drpContact & "</div></div>")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-3 text-bold-900 text-md-right"">" & lblPatient & ":</div><div class=""col-md-9"" id=""lstPatient"">" & drpPatient & "</div></div>")
        body.Append("</div>")

        Dim buttons As String = ""
        buttons = "<button type=""button"" onclick=""javascript:createCreditOrder($('#drpContact').val(), $('#drpPatient').val());"" class=""btn btn-success""><i class=""icon-save""></i> " & btnOpen & "</button> "
        buttons = buttons & "<button type=""button"" class=""btn btn-warning"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"

        Dim sh As New Share.UI
        Return sh.drawModal("Select The Patient...", body.ToString, buttons, Share.UI.ModalSize.Small)
    End Function

    Public Function getCreditPatients(ByVal lngContact As Long) As String
        Dim drpPatient As String
        Dim dsPatient As DataSet


        drpPatient = "<select class=""form-control"" id=""drpPatient"">"
        dsPatient = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients AS P INNER JOIN Hw_Patients_Details AS PD ON P.lngPatient=PD.lngPatient WHERE P.lngGuarantor=" & lngContact & " AND PD.bCredit=1")
        For I = 0 To dsPatient.Tables(0).Rows.Count - 1
            drpPatient = drpPatient & "<option value=""" & dsPatient.Tables(0).Rows(I).Item("lngPatient") & """>" & dsPatient.Tables(0).Rows(I).Item("PatientName").ToString & "</option>"
        Next
        drpPatient = drpPatient & "</select>"

        Return drpPatient
    End Function

    Public Function createCreditOrder(ByVal lngContact As Long, ByVal lngPatient As Long) As String
        Const ErrorType As Integer = 4
        Dim usr As New Share.User
        If lngContact > 0 And lngPatient > 0 Then
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Stock_Trans WHERE lngContact=" & lngContact & " ANd lngPatient=" & lngPatient & " AND lngSalesman=" & lngSalesman_Cash & " AND dateTransaction='" & Today.ToString("yyyy-MM-dd") & "' AND byteStatus=1 AND byteBase=50")
            If ds.Tables(0).Rows.Count = 0 Then
                Try
                    Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = " & intYear & " AND byteBase = 50")
                    Dim strTransaction As String = CLng(dsLast.Tables(0).Rows(0).Item("LastNo")) + 1

                    Dim dsPatient As DataSet = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients AS P WHERE lngPatient=" & lngPatient)
                    Dim strPatient As String = dsPatient.Tables(0).Rows(0).Item("PatientName").ToString

                    Dim lngTrans As Long
                    lngTrans = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, dateTransaction, lngContact, byteStatus, bCash, dateClosedValid, byteCurrency, byteWarehouse, lngPatient, strRemarks, byteDepartment, lngSalesman, dateEntry, bCollected1, bApproved1) VALUES (50, 20,'" & strTransaction & "', '" & Today.ToString("yyyy-MM-dd") & "' , " & lngContact & ", 1, 0, '" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', " & byteLocalCurrency & ", 3, " & lngPatient & ", '" & strPatient & "', " & byteDepartment_Cash & ", " & lngSalesman_Cash & ", '" & Today.ToString("yyyy-MM-dd HH:mm:sss") & "', 1, 0)")

                    Return "<script>javascript:showModal('prepareOrder','{TransNo: " & lngTrans & "}','#mdlAlpha');</script>"
                Catch ex As Exception
                    If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngContact, ErrorType, ex.Message)
                    Return "Err: " & ex.Message
                End Try
            Else
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngContact, ErrorType, "You have already same order in your list...")
                Return "Err: You have already same order in your list..."
            End If
        Else
            If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngContact, ErrorType, "Cannot create this order..")
            Return "Err: Cannot create this order.."
        End If
    End Function

    Public Function viewOrder(ByVal TransNo As Long, ByVal ShowOnly As Boolean, ByVal ToPrepare As Boolean) As String
        Const ErrorType As Integer = 1

        Dim ds As DataSet
        Dim DataLang As String
        Dim OrderInformation, InvoiceDetails As String
        Dim lblDoctorName, lblInvoiceDate, lblInsuranceCompany, lblPatientName As String
        Dim colItemName, colItemNo, colAmount, colDescription, colRemarkes, colStatus As String
        Dim Approved, Rejected, Processing As String
        Dim btnPrepare, btnReturn, btnSend, btnClose As String
        Dim cnfReturnToDoctor As String

        Dim Cash, Credit As String
        Dim AutoCorrectionMsg As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                OrderInformation = "معلومات الطلب"
                InvoiceDetails = "تفاصيل الفاتورة"
                Approved = "مقبولة"
                Rejected = "مرفوضة"
                Processing = "معالجة"
                Cash = "نقدية"
                Credit = "آجلة"
                AutoCorrectionMsg = "هذه الفاتورة تم تغييرها إلى {TYPE} عن طريق التغيير التلقائي, يرجى التأكد من المريض قبل الاستمرار.."
                lblDoctorName = "اسم الطبيب"
                lblInvoiceDate = "تاريخ الفاتورة"
                lblInsuranceCompany = "جهة الدفع"
                lblPatientName = "اسم المريض"
                colItemName = "اسم الصنف"
                colItemNo = "رقم الصنف"
                colAmount = "الكمية"
                colDescription = "الوصفة"
                colRemarkes = "الملاحظات"
                colStatus = "الحالة"
                btnPrepare = "تحضير الأدوية"
                btnReturn = "إرجاع الطلب"
                btnSend = "ارسال للتحضير"
                cnfReturnToDoctor = "هل أنت متأكد من إرجاع الطلب إلى الطبيب؟"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                OrderInformation = "Order Information"
                InvoiceDetails = "Invoice Details"
                Approved = "Approved"
                Rejected = "Rejected"
                Processing = "Processing"
                Cash = "Cash"
                Credit = "Credit"
                AutoCorrectionMsg = "This invoice has changed to {TYPE} by Auto-Correction, please check with the patient before proceed.."
                lblDoctorName = "Doctor Name"
                lblInvoiceDate = "Invoice Date"
                lblInsuranceCompany = "Payment Party"
                lblPatientName = "Patient Name"
                colItemName = "Item Name"
                colItemNo = "Item No"
                colAmount = "Amount"
                colDescription = "Description"
                colRemarkes = "Remarks"
                colStatus = "Status"
                btnPrepare = "Prepare Medicines"
                btnReturn = "Return Order"
                btnSend = "Send To Prepare"
                cnfReturnToDoctor = "Are you sure to return this order to the doctor?"
                btnClose = "Close"
        End Select

        Dim Where As String = ""
        Dim Msg As String = ""
        Dim HasClinic As Boolean = False
        Dim usr As New Share.User
        If ShowOnly = False Then Where = " AND ST.byteBase = 50 AND ST.byteStatus = 1 AND ST.bCollected1 = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL)"
        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, P.lngGuarantor AS PCompanyNo, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.dateClosedValid FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.lngTransaction <> 0 " & Where & " AND ST.lngTransaction = " & TransNo)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                If ShowOnly = False Then
                    '-------------------> Auto-Correction
                    Dim CompanyNo As Long = ds.Tables(0).Rows(0).Item("CompanyNo")
                    Dim CompanyName As String = ds.Tables(0).Rows(0).Item("CompanyName").ToString
                    Dim PCompanyNo As Long
                    If IsDBNull(ds.Tables(0).Rows(0).Item("PCompanyNo")) Then PCompanyNo = 0 Else PCompanyNo = ds.Tables(0).Rows(0).Item("PCompanyNo")
                    Dim DoctorNo As Long = ds.Tables(0).Rows(0).Item("DoctorNo")
                    Dim CashOnly As Boolean = ds.Tables(0).Rows(0).Item("bCash")
                    If ds.Tables(0).Rows(0).Item("ClinicInvoiceNo").ToString <> "" Then HasClinic = True
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

                    If HttpContext.Current.Session("SkipCorrection") Is Nothing Then HttpContext.Current.Session("SkipCorrection") = ""
                    If HttpContext.Current.Session("SkipCorrection").ToString <> "Yes" Then
                        Dim dsClinic As DataSet = dcl.GetDS("select * from Clinic_Invoices where lngPatient=" & ds.Tables(0).Rows(0).Item("PatientNo") & " and dateTransaction between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), CDate(ds.Tables(0).Rows(0).Item("TransactionDate"))).ToString("yyyy-MM-dd") & "' and '" & CDate(ds.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND lngSalesman=" & DoctorNo)
                        If dsClinic.Tables(0).Rows.Count > 0 Then
                            Dim ClinicCash As Boolean
                            ClinicCash = dsClinic.Tables(0).Rows(0).Item("bCash")

                            'If dsClinic.Tables(0).Rows(0).Item("bCash") = 1 And IsDBNull(dsClinic.Tables(0).Rows(1).Item("bCash")) Then ClinicCash = True Else ClinicCash = False

                            If CashOnly <> ClinicCash Then
                                Dim current As String
                                If ClinicCash = True Then
                                    current = Cash
                                    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=1 WHERE lngTransaction=" & TransNo)
                                    If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", TransNo, ErrorType, "Invoice type changed from [credit] to [cash] by AUTO-CORRECTION")
                                    If LogsEnabled Then usr.AddLog("SYSTEM", Now, 1, "Order", TransNo, 2, "Invoice type changed from [credit] to [cash] by AUTO-CORRECTION")
                                Else
                                    current = Credit
                                    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=0 WHERE lngTransaction=" & TransNo)
                                    If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", TransNo, ErrorType, "Invoice type changed from [cash] to [credit] by AUTO-CORRECTION")
                                    If LogsEnabled Then usr.AddLog("SYSTEM", Now, 1, "Order", TransNo, 2, "Invoice type changed from [cash] to [credit] by AUTO-CORRECTION")
                                End If
                                Msg = Replace(AutoCorrectionMsg, "{TYPE}", current)
                                ''Return "Err: This invoice need to be fixed!"
                            End If
                        Else
                            'Return "Err: There is no medical invoices, please go to the receiption.."
                        End If
                    End If
                    HttpContext.Current.Session("SkipCorrection") = ""

                    'Checking Insurance company for the patient
                    If CashOnly = False Then
                        If PCompanyNo <> 0 And CompanyNo <> PCompanyNo Then
                            Dim dsContact As DataSet = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact=" & PCompanyNo)
                            dcl.ExecSQuery("UPDATE Stock_Trans SET lngContact=" & PCompanyNo & " WHERE lngTransaction=" & TransNo)
                            Msg = Replace(AutoCorrectionMsg, "{TYPE}", dsContact.Tables(0).Rows(0).Item("strContact" & DataLang).ToString)
                            If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "Order", TransNo, ErrorType, "Insurance company changed from [" & CompanyName & "] to [" & dsContact.Tables(0).Rows(0).Item("strContact" & DataLang).ToString & "] by AUTO-CORRECTION")
                            If LogsEnabled Then usr.AddLog("SYSTEM", Now, 1, "Order", TransNo, 2, "Insurance company changed from [" & CompanyName & "] to [" & dsContact.Tables(0).Rows(0).Item("strContact" & DataLang).ToString & "] by AUTO-CORRECTION")
                        End If
                    End If
                    '<-------------------

                    '-------------------> Cashier Order [for preparing]
                    Dim doc As New XmlDocument
                    doc.Load(HttpContext.Current.Server.MapPath("../data/xml/ph-cashier.xml"))
                    Dim cashiers As XmlNode = doc.SelectSingleNode("Cashiers/Cashier[User='" & strUserName & "']")
                    If Not (cashiers Is Nothing) Then
                        Dim No As String = cashiers.SelectSingleNode("No").InnerText
                        Dim dsOrder As DataSet = dcl.GetDS("SELECT * FROM Ph_Cashiers WHERE byteNo=" & No)
                        If dsOrder.Tables(0).Rows.Count > 0 Then
                            If dsOrder.Tables(0).Rows(0).Item("lngTransaction") <> TransNo Then
                                dcl.ExecSQuery("UPDATE Ph_Cashiers SET lngTransaction = " & TransNo & ", strUserName='" & strUserName & "' WHERE byteNo = " & No)
                            End If
                        Else
                            If No <> "" Then dcl.ExecSQuery("INSERT INTO Ph_Cashiers VALUES (" & No & ", '" & strUserName & "', " & TransNo & ")")
                        End If
                    End If
                    '<-------------------
                End If

                Dim str As String = ""
                If ShowOnly = False Then
                    str = str & "<div class=""row""><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblPatientName & ":</div><div class=""teal"">" & ds.Tables(0).Rows(0).Item("PatientName") & "</div></div></div><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblInvoiceDate & ":</div><div class=""teal"">" & CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString(strDateFormat) & "</div></div></div></div>"

                    str = str & "<div class=""row""><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblDoctorName & ":</div><div class=""red"">" & ds.Tables(0).Rows(0).Item("DoctorName") & "</div></div></div><div class=""col-md-6""><div><div class=""text-bold-900"">" & lblInsuranceCompany & ":</div><div><span class=""tag tag-info"">" & ds.Tables(0).Rows(0).Item("CompanyName") & " </span></div></div></div></div>"
                    str = str & "<h4 class=""form-section""><i class=""icon-clipboard4""></i> " & InvoiceDetails & "</h4>"
                End If
                str = str & "<div class=""row pl-1 pr-1""><table class=""table table-bordered""><thead><tr><th>" & colItemNo & "</th><th>" & colItemName & "</th><th>" & colAmount & "</th><th>" & colDescription & "</th><th>" & colRemarkes & "</th><th>" & colStatus & "</th></tr></thead><tbody>"

                ds = dcl.GetDS("SELECT HTP.strReference, HTP.intVisit, SI.strItem" & DataLang & " AS ItemName, SI.strItem AS ItemNo, HTP.curQuantity AS Quantity, HTP.strDose AS Usage,HTP.curUnitPrice,HTP.dateExpiry, CASE WHEN HDA.byteCheck = 1 THEN 'Done' END AS [Status], HTP.Moredetails, HTP.Notes, CASE WHEN HDA.bApproval=1 THEN (CASE WHEN HDA.strApprovedBy IS NOT NULL THEN 1 ELSE (CASE WHEN HDA.bRejected=1 THEN 2 ELSE 0 END) END) ELSE 1 END AS Approval, ISNULL(PQM.strQty" & DataLang & ",'') + ' ' + ISNULL(PDM.strDose" & DataLang & ",'') + ' ' + ISNULL(PRM.strRepetition" & DataLang & ",'') + ' ' + ISNULL(PTM.strTime" & DataLang & ",'') + ' ' + ISNULL(PPM.strPeriod" & DataLang & ",'') AS Dose FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON ST.strReference=HTP.strReference AND ST.lngPatient=HTP.lngPatient INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem LEFT JOIN Hw_Medicines_Approval AS HDA ON ST.lngPatient=HDA.lngPatient AND ST.strReference=HDA.strReference AND HTP.strItem=HDA.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,1,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction=" & TransNo & " ORDER BY SI.strItem" & DataLang)

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
                    str = str & "<tr><td>" & ds.Tables(0).Rows(I).Item("ItemNo") & "</td><td class=""text-bold-700 brown darken-1"">" & ds.Tables(0).Rows(I).Item("ItemName") & "</td><td>" & Math.Round(ds.Tables(0).Rows(I).Item("Quantity"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td class=""text-bold-700 brown darken-1"">" & ds.Tables(0).Rows(I).Item("Dose") & "</td><td>" & Trim(ds.Tables(0).Rows(I).Item("Moredetails") & " " & ds.Tables(0).Rows(I).Item("Notes")) & "</td><td><span class=""tag tag-" & StatusColor & """ >" & StatusText & "</span></td></tr>"
                Next

                str = str & "</tbody></table></div>"
                str = str & "<script type=""text/javascript"">"
                str = str & "function returnToDoctor(){returnOrder(" & TransNo & ")}"
                str = str & "function sendToPrapare(transNo) {var dataJsonString = JSON.stringify({ lngTransaction: transNo });$.ajax({type: 'POST',url: 'ajax.aspx/sendToPrapare',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
                If Msg <> "" Then str = str & "msg('','" & Msg & "','info')"
                str = str & "</script>"

                Dim P_disabled As String = ""
                If p_Prepare = False Then P_disabled = "disabled=""disabled"""
                Dim buttons As String = ""
                Dim returnButton As String = ""
                If ToPrepare = True Then
                    buttons = "<button type=""button"" onclick=""javascript:sendToPrapare(" & TransNo & ");"" class=""btn btn-info""><i class=""icon-medkit2""></i> " & btnSend & "</button> "
                Else
                    If HasClinic = True Then returnButton = " <button type=""button"" onclick=""javascript:confirm('','" & cnfReturnToDoctor & "',returnToDoctor);"" class=""btn btn-danger"" " & P_disabled & "><i class=""icon-refresh""></i> " & btnReturn & "</button>"
                    If ShowOnly = False Then buttons = "<button type=""button"" onclick=""javascript:showModal('prepareOrder','{TransNo: " & TransNo & "}','#mdlAlpha');"" class=""btn btn-success""><i class=""icon-medkit2""></i> " & btnPrepare & "</button>" & returnButton & " "
                End If
                buttons = buttons & "<button type=""button"" class=""btn btn-warning"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"
                Dim sh As New Share.UI

                Dim Size As Share.UI.ModalSize = Share.UI.ModalSize.Large
                'If ShowOnly = True Then Size = Share.UI.ModalSize.Medium Else Size = Share.UI.ModalSize.Large
                Dim Background As String = ""
                If ShowOnly = True Then Background = "bg-grey bg-lighten-4"

                Return sh.drawModal(OrderInformation, str, buttons, Size, Background)
            Catch ex As Exception
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "Order", TransNo, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function

    Public Function returnOrder(ByVal TransNo As Long) As String
        Const ErrorType As Integer = 2

        Dim ds As DataSet
        Dim DataLang As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.byteStatus = 1 AND ST.bCollected1 = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & TransNo)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=0 WHERE lngTransaction=" & TransNo)
                Return "<script type=""text/javascript"">msg('','Order has retruned successfully!','info');$('#mdlAlpha').modal('hide');$('#row" & TransNo & "').remove();updateUI();</script>"
            Catch ex As Exception
                Dim usr As New Share.User
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", TransNo, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function

    Public Function prepareOrder(ByVal lngTransaction As Long, Optional ByVal CashOnly As Boolean = False) As String
        Const ErrorType As Integer = 3
        Dim usr As New Share.User

        Dim ds As DataSet
        Dim DataLang As String
        Dim InvoiceDetails, InsuranceInvoiceNo, CashInvoiceNo As String
        Dim lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash As String
        Dim btnPrint, btnDelete, btnPrintAll, btnPayNow, btnSuspend, btnUnsuspend, btnToCashier, btnView, btnClose, btnViewOrder, btnMoreDetails As String
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
                btnView = "إجراء"
                btnViewOrder = "عرض طلب الأدوية"
                btnMoreDetails = "عرض تفاصيل التغطية"
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
                btnView = "Action"
                btnViewOrder = "View Midicnes Order"
                btnMoreDetails = "View Coverage Details"
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
        Dim TransactionDate, InvoiceDate, CloseDate As Date
        Dim InvoiceNo As String
        Dim bCash As Boolean
        Dim bCreatCash As Boolean
        Dim cashItemsRows As String = ""
        Dim insuranceItemsRows As String = ""
        Dim cashItems As String = ""
        Dim insuranceItems As String = ""
        Dim MaxP, CICov, MICov As Decimal
        Dim OrderItemsCount As Decimal

        If lngTransaction > 0 Then
            ' Insurance
            Try
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, STI.dateTransaction AS CloseDate, ST.strTransaction AS InvoiceNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash, ST.bCreatCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction LEFT JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
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
                    If IsDBNull(ds.Tables(0).Rows(0).Item("CloseDate")) Then CloseDate = Today Else CloseDate = ds.Tables(0).Rows(0).Item("CloseDate")
                    CashOnly = ds.Tables(0).Rows(0).Item("bCash")
                    CashInvoiceNo = ""
                    InsuranceInvoiceNo = ""

                    '' Correct the order to cash if its contact is Pharmacy Cash
                    'If (CompanyNo = lngContact_Cash) And (CashOnly = False) Then
                    '    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=1 WHERE lngTransaction=" & lngTransaction)
                    '    CashOnly = True
                    'End If

                    '' Correct the order to credit if its contact is not Pharmacy Cash
                    'If (CompanyNo <> lngContact_Cash) And (CashOnly = True) Then
                    '    dcl.ExecSQuery("UPDATE Stock_Trans SET bCash=0 WHERE lngTransaction=" & lngTransaction)
                    '    CashOnly = True
                    'End If

                    If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) Then bCreatCash = False Else bCreatCash = CBool(ds.Tables(0).Rows(0).Item("bCreatCash"))

                    Dim dsInsurance, dsCash, dsOrders As DataSet
                    If CashOnly = True Then
                        CashInvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                        dsCash = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                        For I = 0 To dsCash.Tables(0).Rows.Count - 1
                            cashItemsRows = cashItemsRows & func.createItemRow(lngTransaction, rowCounter, True, "", dsCash.Tables(0).Rows(I).Item("strBarCode"), dsCash.Tables(0).Rows(I).Item("strItem"), dsCash.Tables(0).Rows(I).Item("strItem" & DataLang), dsCash.Tables(0).Rows(I).Item("byteUnit"), dsCash.Tables(0).Rows(I).Item("dateExpiry"), dsCash.Tables(0).Rows(I).Item("curBasePrice"), dsCash.Tables(0).Rows(I).Item("curDiscount"), dsCash.Tables(0).Rows(I).Item("curQuantity"), dsCash.Tables(0).Rows(I).Item("curBaseDiscount"), dsCash.Tables(0).Rows(I).Item("curCoverage"), CDec("0" & dsCash.Tables(0).Rows(I).Item("curVAT").ToString), dsCash.Tables(0).Rows(I).Item("intService"), dsCash.Tables(0).Rows(I).Item("byteWarehouse"), "", True, False, False, False)
                            cashItems = cashItems & dsCash.Tables(0).Rows(I).Item("strItem") & ","
                            rowCounter = rowCounter + 1
                        Next
                        OrderItemsCount = 0
                    Else
                        InsuranceInvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                        dsInsurance = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                        For I = 0 To dsInsurance.Tables(0).Rows.Count - 1
                            insuranceItemsRows = insuranceItemsRows & func.createItemRow(lngTransaction, rowCounter, False, "", dsInsurance.Tables(0).Rows(I).Item("strBarCode"), dsInsurance.Tables(0).Rows(I).Item("strItem"), dsInsurance.Tables(0).Rows(I).Item("strItem" & DataLang), dsInsurance.Tables(0).Rows(I).Item("byteUnit"), dsInsurance.Tables(0).Rows(I).Item("dateExpiry"), dsInsurance.Tables(0).Rows(I).Item("curBasePrice"), dsInsurance.Tables(0).Rows(I).Item("curDiscount"), dsInsurance.Tables(0).Rows(I).Item("curQuantity"), dsInsurance.Tables(0).Rows(I).Item("curBaseDiscount"), dsInsurance.Tables(0).Rows(I).Item("curCoverage"), CDec("0" & dsInsurance.Tables(0).Rows(I).Item("curVAT").ToString), dsInsurance.Tables(0).Rows(I).Item("intService"), dsInsurance.Tables(0).Rows(I).Item("byteWarehouse"), "", True, False, False, False)
                            insuranceItems = insuranceItems & dsInsurance.Tables(0).Rows(I).Item("strItem") & ","
                            rowCounter = rowCounter + 1
                        Next
                        If bCreatCash = True Then
                            Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & InvoiceNo & "' AND lngPatient=" & PatientNo & " AND bSubCash=1 AND byteBase=50 AND byteStatus>0")
                            If dsTemp.Tables(0).Rows.Count > 0 Then
                                CashInvoiceNo = dsTemp.Tables(0).Rows(0).Item("strTransaction").ToString
                                dsCash = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & dsTemp.Tables(0).Rows(0).Item("lngTransaction"))
                                For I = 0 To dsCash.Tables(0).Rows.Count - 1
                                    cashItemsRows = cashItemsRows & func.createItemRow(lngTransaction, rowCounter, True, "", dsCash.Tables(0).Rows(I).Item("strBarCode"), dsCash.Tables(0).Rows(I).Item("strItem"), dsCash.Tables(0).Rows(I).Item("strItem" & DataLang), dsCash.Tables(0).Rows(I).Item("byteUnit"), dsCash.Tables(0).Rows(I).Item("dateExpiry"), dsCash.Tables(0).Rows(I).Item("curBasePrice"), dsCash.Tables(0).Rows(I).Item("curDiscount"), dsCash.Tables(0).Rows(I).Item("curQuantity"), dsCash.Tables(0).Rows(I).Item("curBaseDiscount"), dsCash.Tables(0).Rows(I).Item("curCoverage"), CDec("0" & dsCash.Tables(0).Rows(I).Item("curVAT").ToString), dsCash.Tables(0).Rows(I).Item("intService"), dsCash.Tables(0).Rows(I).Item("byteWarehouse"), "", True, False, False, False)
                                    cashItems = cashItems & dsCash.Tables(0).Rows(I).Item("strItem") & ","
                                    rowCounter = rowCounter + 1
                                Next
                            End If
                        End If
                        dsOrders = dcl.GetDS("SELECT SUM(curQuantity) AS curQuantity FROM Hw_Treatments_Pharmacy WHERE lngPatient=" & PatientNo & " AND strReference='" & InvoiceNo & "'")
                        OrderItemsCount = CDec("0" & dsOrders.Tables(0).Rows(0).Item("curQuantity").ToString)
                    End If

                    If CompanyNo <> lngContact_Cash And CashOnly = False Then
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
                                dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                                If IsDBNull(dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP")) Then MaxP = 0 Else MaxP = dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP")
                                'MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)

                                'Validate Coverage data, [sometimes wrong entry - not match contract]
                                If curCoverage > 0 And MaxP = 0 Then Return "Err:Max deduction is missing, Please correct the contract information.."
                                '[Sometimes No percent becuse it is less that max - so no need after reciption payments]
                                'If curCoverage = 0 And MaxP > 0 Then Return "Err:Deduction percent is missing, Please correct the contract information.."

                                CICov = func.getTotalClinicInvoices(PatientNo, DoctorNo, CloseDate)
                                'Dim dsClinic As DataSet
                                'dsClinic = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), TransactionDate).ToString("yyyy-MM-dd") & "' And '" & TransactionDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & PatientNo & " AND lngSalesMan=" & DoctorNo & " GROUP BY lngSalesman")
                                'If dsClinic.Tables(0).Rows.Count > 0 Then
                                '    CICov = CDec("0" & dsClinic.Tables(0).Rows(0).Item("Coverage").ToString)
                                'Else
                                '    CICov = 0
                                'End If
                                MICov = func.getTotalPharmacyInvoices(PatientNo, DoctorNo, CloseDate, lngTransaction, False)
                                'Dim dsMidicine As DataSet
                                'dsMidicine = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), TransactionDate).ToString("yyyy-MM-dd") & "' AND '" & TransactionDate.ToString("yyyy-MM-dd") & "' AND lngPatient=" & PatientNo & " AND lngSalesMan=" & DoctorNo & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                                'If dsMidicine.Tables(0).Rows.Count > 0 Then
                                '    MICov = CDec("0" & dsMidicine.Tables(0).Rows(0).Item("Cov").ToString)
                                'Else
                                '    MICov = 0
                                'End If
                            Else
                                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngTransaction, ErrorType, "Credit invoice,Complete insurance information")
                                Return "Err: Credit invoice,Complete insurance information"
                            End If
                        Else
                            If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngTransaction, ErrorType, "There are no contracts with this company.")
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
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "ORDER", lngTransaction, ErrorType, ex.Message)
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
            OrderItemsCount = 0
        End If

        '3> add new tab (to join later with other tabs) [MUST BY ACTIVE IN CLASS]
        tabHeader.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab" & tabCounter & """ data-toggle=""tab"" aria-controls=""tab" & tabCounter & """ href=""#tab" & tabCounter & """ aria-expanded=""true"">" & PatientFirstName & "</a></li>")

        ' ''>4 add content of current tab
        tabContent.Append(createTabContent(tabCounter, rowCounter, True, lngTransaction, PatientName, DoctorName, InvoiceDate, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, insuranceItems, cashItems, insuranceItemsRows, cashItemsRows, MaxP, CICov, MICov, OrderItemsCount, CashOnly))

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
                            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ISNULL(P.strFirst" & DataLang & ", P.lngPatient) AS PatientFirstName, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo,  C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.bCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = " & intYear & " AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & trans(I))
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
                        'tabContent.Append(createTabContent(tabCounter, rowCounter, False, trans(I), PatientName, DoctorName, InvoiceDate, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, InsItems, CshItems, 0, 0, 0, 0, CBool(cashO(I))))
                        tabContent.Append("<script type=""text/javascript"">calculateInsurance(" & tabCounter & ");calculateCash(" & tabCounter & ");</script>")
                    End If
                End If
            Next
        End If

        ''>6 add barcode input
        Dim barcode As String = "<div class=""row pl-1 pr-1""><div class=""col-md-12"" style=""margin: -20px 0 -10px 0;""><div class=""position-relative has-icon-left""><input type=""text"" class=""form-control round  border-primary text-md-center"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ /><div class=""form-control-position""><i class=""icon-barcode""></i></div></div></div></div>"

        ''>7 add buttons
        Dim S_disabled As String = ""
        Dim C_disabled As String = ""
        If p_Sales = False Then S_disabled = "disabled=""disabled"""
        If p_Cashier = False Then C_disabled = "disabled=""disabled"""

        Dim sendFunction As String
        Dim printAllButton As String = ""
        Dim sendButton As String = ""
        Dim paymentButton As String = ""
        If (PrintDose = 2) Or (PrintDose = 3) Then printAllButton = "<span id=""divPrintAll" & tabCounter & """ app-print=""true"" app-printer=""" & DosePrinter & """ app-popup=""" & PopupToPrint.ToString.ToLower & """ app-url=""p_dose.aspx""><button type=""button"" id=""btnPrintAll" & tabCounter & """ onclick=""javascript:printAllDose(" & tabCounter & ");"" class=""btn btn-blue-grey ml-1"" disabled=""disabled""><i class=""icon-printer""></i> " & btnPrintAll & "</button></span>"
        If AskBeforeSend = True Then sendFunction = "confirm('','Send this invoice to cashier?',sendToCashier);" Else sendFunction = "sendToCashier();"
        If SalesmanType = 0 Or SalesmanType = 2 Then sendButton = "<button type=""button"" class=""btn btn-outline-success mr-1"" onclick=""javascript:" & sendFunction & """ " & S_disabled & "><i class=""icon-coin-dollar""></i> " & btnToCashier & "</button>"
        If SalesmanType = 1 Or SalesmanType = 2 Then paymentButton = "<button type=""button"" class=""btn btn-success mr-3"" onclick=""javascript:showCashier($('#trans' + curTab).val());"" " & C_disabled & "><i class=""icon-money""></i> " & btnPayNow & "</button>"
        Dim buttons As String = "<div class=""text-md-center"">" & sendButton & paymentButton & "<button type=""button"" class=""btn btn-primary ml-3"" id=""btnSuspend"" onclick=""javascript:suspendInvoice($('#trans'+curTab).val(), $('#cashOnly'+curTab).val(), collectIItems(curTab), collectCItems(curTab), $('#suspend'+curTab).val());""><i class=""icon-clock5""></i> " & btnSuspend & "</button>" & printAllButton & "<button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button></div>"

        ''>7 close everything

        ''>8 create footer

        ''>9 add scripts
        Dim scripts As String = ""
        scripts = scripts & "<script type=""text/javascript"">"
        scripts = scripts & "var curTab = 1;$('a[data-toggle=""tab""]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); if(curTab!=1) {$('#btnSuspend').attr('disabled', false); $('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnUnsuspend & "');} else {$('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnSuspend & "');if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true); else $('#btnSuspend').attr('disabled', false);}});"
        scripts = scripts & "var counter" & tabCounter & "=" & rowCounter & ";var tabCount=" & tabCounter & ";if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true);"
        'scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {completeBarcode(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
        scripts = scripts & "$('#txtBarcode').on('change paste keyup', function () {var barcode = $(this).val();if (barcode.length != 0) {if ($.isNumeric(barcode) == true) {if (event.which == 13 || barcode.length >= 14) {event.preventDefault();$(this).val('');getItemInfo(barcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val(),0);}}}});"
        scripts = scripts & "function showCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { TabCounter: curTab, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/viewCashier1',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#mdlMessage').html(response.d);$('#mdlMessage').modal('show');}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
        scripts = scripts & "function sendToCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/SendToCashier',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
        scripts = scripts & "calculateInsurance(curTab);calculateCash(curTab);$('#txtBarcode').focus();"
        scripts = scripts & "</script>"

        ''>9 Build everything in modal
        Dim mdl As New Share.UI
        Dim tabHead As String = "<ul class=""nav nav-tabs"" id=""tabCashier"">" & tabHeader.ToString & "</ul>"
        Dim tabBody As String = "<div class=""tab-content px-1 pt-1"">" & tabContent.ToString & "</div>"
        body.Append(mdl.drawModal(func.getHeaderSubMenu(InvoiceDetails, True), tabHead & tabBody & barcode & scripts, buttons, Share.UI.ModalSize.Large, "", "p-0"))

        Return body.ToString
    End Function

    Private Function createTabContent(ByVal tabCounter As Integer, ByVal rowCounter As Integer, ByVal IsActive As Boolean, ByVal lngTransaction As Long, ByVal PatientName As String, ByVal DoctorName As String, ByVal InvoiceDate As Date, ByVal CompanyName As String, ByVal InsuranceInvoiceNo As String, ByVal CashInvoiceNo As String, ByVal InsuranceItemsList As String, ByVal CashItemsList As String, ByVal InsuranceRows As String, ByVal CashRows As String, ByVal MaxP As Decimal, ByVal CICov As Decimal, ByVal MICov As Decimal, ByVal OrderItemsCount As Decimal, Optional ByVal CashOnly As Boolean = False) As String
        Dim InvoiceDetails, InsuranceInvoice, CashInvoice As String
        Dim lblDoctor, lblDate, lblPatient, lblPharmacist, lblTotalCovered, lblTotalCash As String
        Dim tabContent As New StringBuilder("")

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
        End Select

        Dim active As String = ""
        Dim suspend As Integer
        If IsActive = True Then active = "active"
        If IsActive = True Then suspend = 0 Else suspend = 1

        tabContent.Append("<div role=""tabpanel"" class=""tab-pane " & active & """ id=""tab" & tabCounter & """ aria-expanded=""true"" aria-labelledby=""base-tab" & tabCounter & """>")
        tabContent.Append("<form id=""frmInvoice" & tabCounter & """>")
        ''==>4.2 hidden inputs
        Dim hidden As String = ""
        ' static
        hidden = hidden & "<input type=""hidden"" id=""trans" & tabCounter & """ name=""lngTransaction"" value=""" & lngTransaction & """ />" ' Transaction
        hidden = hidden & "<input type=""hidden"" id=""cashOnly" & tabCounter & """ name=""cashOnly"" value=""" & CInt(CashOnly) & """ />" 'cash only (invoice type)
        hidden = hidden & "<input type=""hidden"" id=""Limit_" & tabCounter & """ value=""" & MaxP & """ />" 'Max deduction of credit invoice
        hidden = hidden & "<input type=""hidden"" id=""CICov_" & tabCounter & """ value=""" & CICov & """ />" ' total clinic invoices for credit invoice
        hidden = hidden & "<input type=""hidden"" id=""MICov_" & tabCounter & """ value=""" & MICov & """ />" ' total pharmacy invoices for credit invoice
        hidden = hidden & "<input type=""hidden"" id=""orderItemsCount" & tabCounter & """ value=""" & OrderItemsCount & """ />" ' order items count for credit invoice
        ' changable
        hidden = hidden & "<input type=""hidden"" id=""counter" & tabCounter & """ value=""" & rowCounter & """ />" ' row counter
        hidden = hidden & "<input type=""hidden"" id=""coveredCash" & tabCounter & """ name=""coveredCash"" value=""0"" />" ' covered cash
        hidden = hidden & "<input type=""hidden"" id=""deductionCash" & tabCounter & """ name=""deductionCash"" value=""0"" />" ' deduction cash
        hidden = hidden & "<input type=""hidden"" id=""nonCoveredCash" & tabCounter & """ name=""nonCoveredCash"" value=""0"" />" ' noncovered cash
        hidden = hidden & "<input type=""hidden"" id=""basePrice" & tabCounter & """ value=""0"" />" ' base price
        hidden = hidden & "<input type=""hidden"" id=""suspend" & tabCounter & """ value=""" & suspend & """ />" ' suspend (if this invoice suspended)
        hidden = hidden & "<input type=""hidden"" id=""items_I_" & tabCounter & """ value=""" & InsuranceItemsList & """ />" ' list of credit items
        hidden = hidden & "<input type=""hidden"" id=""items_C_" & tabCounter & """ value=""" & CashItemsList & """ />" ' list of cash items
        hidden = hidden & "<input type=""hidden"" id=""coveredVat" & tabCounter & """ name=""coveredVat"" value=""0"" />" ' covered VAT cash (if enabled)
        hidden = hidden & "<input type=""hidden"" id=""deductionVat" & tabCounter & """ name=""deductionVat"" value=""0"" />" ' deduction VAT cash (if enabled)
        hidden = hidden & "<input type=""hidden"" id=""nonCoveredVat" & tabCounter & """ name=""nonCoveredVat"" value=""0"" />" ' noncovered VAT cash (if enabled)
        tabContent.Append(hidden)
        ''==>4.3 header start
        tabContent.Append("<div class=""row border-grey lighten-0 m-0 mb-1 font-small-3 box-shadow-1"" style=""padding:5px;"">")
        tabContent.Append("<div class=""col-md-5 m-0 p-0"">")
        tabContent.Append("<div class=""col-md-2 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-10 teal"">" & PatientName & "</div>")
        tabContent.Append("<div class=""col-md-2 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblDoctor & ":</div><div class=""col-md-10 red"" style=""margin-top:5px"">" & DoctorName & "</div>")
        tabContent.Append("</div>")
        tabContent.Append("<div class=""col-md-3 m-0 p-0"">")
        tabContent.Append("<div class=""col-md-6 text-md-right text-bold-900"">" & lblDate & ":</div><div class=""col-md-6 red"">" & CDate(InvoiceDate).ToString(strDateFormat) & "</div>")
        tabContent.Append("<div class=""col-md-6 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblPharmacist & ":</div><div class=""col-md-6 teal"" style=""margin-top:5px"">" & strUserName & "</div>")
        tabContent.Append("</div>")
        If TaxEnabled = True Then
            tabContent.Append("<div class=""col-md-4 m-0 p-0"">")
            tabContent.Append("<div class=""col-md-5 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-7 teal""><span class=""tag tag-sm tag-primary"" style=""width:60px;"" id=""totalCash" & tabCounter & """>0.00</span><span class=""tag tag-sm tag-primary"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalVat" & tabCounter & """>0.00</span></div>")
            tabContent.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotalCovered & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:60px;"" id=""totalCovered" & tabCounter & """>0.00</span><span class=""tag tag-sm tag-default"" style=""width:45px; margin-left:2px; margin-right:2px;"" id=""totalCoveredVat" & tabCounter & """>0.00</span></div>")
            tabContent.Append("</div>")
        Else
            tabContent.Append("<div class=""col-md-4 m-0 p-0"">")
            tabContent.Append("<div class=""col-md-5 text-md-right text-bold-900"">" & lblTotalCash & ":</div><div class=""col-md-7 teal""><span class=""tag tag-sm tag-primary"" style=""width:80px;"" id=""totalCash" & tabCounter & """>0.00</span><span  style=""display:none;"" id=""totalVat" & tabCounter & """>0.00</span></div>")
            tabContent.Append("<div class=""col-md-5 text-md-right text-bold-900"" style=""margin-top:5px"">" & lblTotalCovered & ":</div><div class=""col-md-7 red"" style=""margin-top:5px""><span class=""tag tag-sm tag-default"" style=""width:80px;"" id=""totalCovered" & tabCounter & """>0.00</span><span  style=""display:none;"" id=""totalCoveredVat" & tabCounter & """>0.00</span></div>")
            tabContent.Append("</div>")
        End If
        tabContent.Append("</div>")

        ''==>4.3 tables start
        tabContent.Append(createItemsTables(tabCounter, CompanyName, InsuranceInvoiceNo, CashInvoiceNo, InsuranceRows, CashRows, CashOnly))
        tabContent.Append("</form></div>")
        Return tabContent.ToString
    End Function

    Private Function createItemsTables(ByVal Identifier As Integer, ByVal InsuranceCompany As String, ByVal InsuranceInvoiceNo As String, ByVal CashInvoiceNo As String, ByVal InsuranceRows As String, ByVal CashRows As String, Optional ByVal CashOnly As Boolean = False) As String
        Dim str As New StringBuilder("")

        Dim InsuranceInvoice, CashInvoice, Invoice As String
        Dim colBarcode, colItemName, colItemNo, colAmount, colExpireDate, colUnitPrice, colDiscount, colTotal, colTax As String
        Dim InsuranceExtend, CashExtend As String
        Dim CashBtnExtend As String = ""
        Dim CashCompany As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Invoice = "فاتورة"
                InsuranceInvoice = "فاتورة الآجل"
                CashInvoice = "فاتورة النقدي"
                colBarcode = "الباركود"
                colItemNo = "رقم"
                colItemName = "الصنف"
                colAmount = "كم"
                colExpireDate = "الانتهاء"
                colUnitPrice = "السعر"
                colDiscount = "الخصم"
                colTotal = "مجموع"
                colTax = "الضريبة"
                InsuranceExtend = "right"
                CashExtend = "left"
            Case Else
                DataLang = "En"
                Invoice = "Invoice"
                InsuranceInvoice = "Credit Invoice"
                CashInvoice = "Cash Invoice"
                colBarcode = "Barcode"
                colItemNo = "No"
                colItemName = "Item Name"
                colAmount = "Qty"
                colExpireDate = "Expire"
                colUnitPrice = "Price"
                colDiscount = "Discount"
                colTotal = "Total"
                colTax = "Tax"
                InsuranceExtend = "left"
                CashExtend = "right"
        End Select

        Dim TaxHeader_C As String = ""
        Dim TaxHeader_I As String = ""
        Dim TaxFooter_C As String = ""
        Dim TaxFooter_I As String = ""
        If TaxEnabled = True Then
            TaxHeader_I = "<th style=""width:80px;"" class=""dynInsurance"">" & colTax & "</th>"
            TaxHeader_C = "<th style=""width:80px;"" class=""dynCash"">" & colTax & "</th>"
            TaxFooter_I = "<th style=""width:80px;"" class=""dynInsurance"" id=""vat_I_" & Identifier & """>0.00</th>"
            TaxFooter_C = "<th style=""width:80px;"" class=""dynCash"" id=""vat_C_" & Identifier & """>0.00</th>"
        End If

        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact=27")
        CashCompany = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString

        If CashOnly = False Then CashBtnExtend = "<a class=""cursor-pointer " & CashColor & " lighten-3"" href=""javascript:changeToCash(" & Identifier & ")""><i id=""btnCashIcon"" class=""icon-circle-" & CashExtend & """></i></a>"

        str.Append("<div class=""row"">")
        If CashOnly = False Then
            'Insurance
            str.Append("<div class=""col-md-6 insurance" & Identifier & """ id=""divInsurance" & Identifier & """><div class=""card border-" & InsuranceColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & InsuranceColor & " lighten-3""><span class=""icon-clipboard4 text-muted""></span> " & InsuranceInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & Invoice & ": (<span class=""orange"">" & InsuranceInvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynInsurance company"" title=""" & InsuranceCompany & """>" & InsuranceCompany & "</span></div></div>")
            str.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynInsurance"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynInsurance"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_I & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblInsurance" & Identifier & """><tbody>")
            str.Append(InsuranceRows)
            str.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""><a class=""cursor-pointer " & InsuranceColor & " lighten-3"" href=""javascript:changeToInsurance(" & Identifier & ")""><i id=""btnInsuranceIcon"" class=""icon-circle-" & InsuranceExtend & """></i></a></th><td style=""width:70px;"" class=""dynInsurance""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:80px;"" class=""dynInsurance"" id=""price_I_" & Identifier & """>0.00</th><th style=""width:80px;"" class=""dynInsurance""></th><th style=""width:44px;"" id=""quantity_I_" & Identifier & """>0</th><th style=""width:80px;"" id=""total_I_" & Identifier & """>0.00</th>" & TaxFooter_I & "<th><span class=""dynInsurance grey"" id=""covInfo_old" & Identifier & """></span></th></tr></thead></table></div></div></div></div>")
        End If
        'Cash
        str.Append("<div class=""col-md-6 cash" & Identifier & """ id=""divCash" & Identifier & """><div class=""card border-" & CashColor & " border-lighten-3""><div class=""card-header""><h4 class=""card-title " & CashColor & " lighten-3""><span class=""icon-money text-muted""></span> " & CashInvoice & "</h4><div class=""heading-elements""><span class=""font-small-2 text-muted"">" & Invoice & ": (<span class=""orange"">" & CashInvoiceNo & "</span>) </span><span class=""font-small-2 tag tag-xs tag-info dynCash company"" title=""" & CashCompany & """>" & CashCompany & "</span></div></div>")
        str.Append("<div class=""card-body collapse in""><div class=""card-block p-0""><table class=""table table-bordered mb-0""><thead><tr><th style=""width:32px;""></th><th style=""width:70px;"" class=""dynCash"">" & colItemNo & "</th><th class=""itemName width-150"" title=""" & colItemName & """>" & colItemName & "</th><th style=""width:80px;"" class=""dynCash"">" & colExpireDate & "</th><th style=""width:80px;"" class=""dynCash"">" & colUnitPrice & "</th><th style=""width:80px;"" class=""dynCash"">" & colDiscount & "</th><th style=""width:44px;"">" & colAmount & "</th><th style=""width:80px;"">" & colTotal & "</th>" & TaxHeader_C & "<th></th></tr></thead></table><div style=""height:" & TableHeight & "px; overflow-x:auto;"" class="" mb-0 mt-0""><table class=""table table-bordered mb-0 mt-0"" id=""tblCash" & Identifier & """><tbody>")
        str.Append(CashRows)
        str.Append("</tbody></table></div><table class=""table table-bordered mb-0 mt-0""><thead><tr><th style=""width:32px;""><a class=""cursor-pointer " & CashColor & " lighten-3"" href=""javascript:changeToCash(" & Identifier & ")""><i id=""btnCashIcon"" class=""icon-circle-" & CashExtend & """></i></a></th><td style=""width:70px;"" class=""dynCash""></td><th class=""itemName width-150""></th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:80px;"" class=""dynCash"" id=""price_C_" & Identifier & """>0.00</th><th style=""width:80px;"" class=""dynCash""></th><th style=""width:44px;"" id=""quantity_C_" & Identifier & """>0</th><th style=""width:80px;"" id=""total_C_" & Identifier & """>0.00</th>" & TaxFooter_C & "<th></th></tr></thead></table></div></div></div></div>")

        str.Append("</div>")
        If CashOnly = False Then
            str.Append("<script type=""text/javascript"">cashOn[" & Identifier & "] = true;changeToCash(" & Identifier & ");InsuranceOn[" & Identifier & "] = true;changeToInsurance(" & Identifier & ");</script>")
        Else
            str.Append("<script type=""text/javascript"">cashOn[" & Identifier & "] = false;changeToCash(" & Identifier & ");</script>")
        End If
        Return str.ToString
    End Function

    Public Function getItemInfo(ByVal strBarcode As String, ByVal lngTransaction As Long, ByVal curCoverage As Decimal, ByVal curBasePriceTotal As Decimal, ByVal RowCounter As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String, Optional ByVal ItemType As Byte = 0, Optional ByVal AssignedQuantity As Decimal = 0) As String
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
        'Dim byteWarehouse As Byte
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
        Dim bMove As Boolean = False
        Dim bInvoiceType, AutoPrint As Boolean

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
            bInvoiceType = ds.Tables(0).Rows(0).Item("bCash")
        Else
            ' Cash
            lngPatient = 16
            dateTransaction = Today
            strReference = ""
            bytePriceType = 0
            lngContact = 27
            lngSalesman = 395
            bCash = True
            bInvoiceType = True
        End If

        'If strBarcode.Length = 5 Then Return "<script type=""text/javascript"">var modalOn=0;$('#mdlMessage').on('shown.bs.modal', function() {if(modalOn==0){$('#btnSaveBarcode').focus();modalOn++;}});completeBarcode('" & strBarcode & "');</script>"
        If strBarcode.Length = 5 Then Return "<script type=""text/javascript"">$('#mdlMessage').on('shown.bs.modal', function() {$('#dateExpiry').focus().select();});var modalOn=0;$('#mdlMessage').on('keyup', function (event) {if (event.which == 13 && modalOn==0) {event.preventDefault();completeIt();modalOn++;}});completeBarcode('" & strBarcode & "', " & byteWarehouse & ", 0, 'getItemInfo([BARCODE],$(\'#trans\'+curTab).val(),$(\'#deductionCash\'+curTab).val(),$(\'#basePrice\'+curTab).val(),$(\'#counter\'+curTab).val(),$(\'#items_I_\'+curTab).val(),$(\'#items_C_\'+curTab).val(),0);');</script>"
        '1====> Filter Barcode
        Dim returnedArray As String() = func.FilterBarcode(strBarcode)
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

        '4====> Ask for quantity
        If HttpContext.Current.Session("QCounter") Is Nothing Then HttpContext.Current.Session("QCounter") = 0
        Dim QuantityItems As String = "25571,30461,30460,30459,26786,26071"
        Dim QuantityContacts As String = "380,1740"
        Dim QItems() As String = Split(QuantityItems, ",")
        Dim QContacts() As String = Split(QuantityContacts, ",")
        If (Array.IndexOf(QContacts, lngContact.ToString) > -1 Or Array.IndexOf(QItems, strItem) > -1) And AssignedQuantity = 0 And HttpContext.Current.Session("QCounter") = 0 Then
            HttpContext.Current.Session("QCounter") = HttpContext.Current.Session("QCounter") + 1
            Return "<script type=""text/javascript"">$('#mdlConfirm').on('shown.bs.modal', function() {$('#curQuantity').focus().select();});var modalQOn=0;$('#mdlConfirm').on('keydown', function (event) {if (event.which == 13 && modalQOn==0) {event.preventDefault();assignQuantity();modalQOn++;}});getQuantity('" & strBarcode & "');</script>"
        End If
        HttpContext.Current.Session("QCounter") = 0


        '4====> Get Item General Information
        Dim dsGInfo As DataSet
        dsGInfo = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem LEFT JOIN Hw_Department_Items AS HDI ON SI.strItem = HDI.strItem AND HDI.byteDepartment = 15 LEFT JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment WHERE SI.strItem='" & strItem & "'")
        strItemName = Replace(dsGInfo.Tables(0).Rows(0).Item("strItem" & DataLang), "'", "\'")
        intGroup = dsGInfo.Tables(0).Rows(0).Item("intGroup")
        'byteWarehouse = dsGInfo.Tables(0).Rows(0).Item("byteWarehouse")
        byteUnit = dsGInfo.Tables(0).Rows(0).Item("byteUnit")
        'if intService not assigned, then use intGroup to get intService
        If IsDBNull(dsGInfo.Tables(0).Rows(0).Item("intService")) Then
            If intGroup = 21 Then intService = 1 Else intService = 2
        Else
            intService = dsGInfo.Tables(0).Rows(0).Item("intService")
        End If


        If IsDBNull(dsGInfo.Tables(0).Rows(0).Item("bTax")) Then
            bTax = False
            curTax = 0
        Else
            bTax = dsGInfo.Tables(0).Rows(0).Item("bTax")
            curTax = dsGInfo.Tables(0).Rows(0).Item("curTax")
        End If


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
            If lngContact <> 380 Then
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
        End If

        '2===> Get Quantity (Insurance = From Doctor, Cash = 1) Depend on settings
        If OneQuantityPerItem = True Then
            If Array.IndexOf(QContacts, lngContact.ToString) > -1 Or Array.IndexOf(QItems, strItem) > -1 Then
                curQuantity = AssignedQuantity
            Else
                curQuantity = 1
            End If
        Else
            curQuantity = curOrderQuantity
        End If

        '2===>
        Dim curQty As Decimal
        If ItemType > 0 Then curQty = 0 Else curQty = curQuantity ' only when moving item => avoid duplicate quantity

        Dim Count As Integer = 0
        Dim I_items As String() = Split(SelectedInsuranceItems, ",")
        For Each item As String In I_items
            If item = strItem Then Count = Count + 1
        Next

        Dim C_items As String() = Split(SelectedCashItems, ",")
        For Each item As String In C_items
            If item = strItem Then Count = Count + 1
        Next

        If func.checkStock(strItem, dateTransaction, byteWarehouse, dateExpiry) - (curQty + Count) < 0 Then
            Return "Err:No balance of this item."
        End If
        'If func.checkStock(strItem, curQty, dateTransaction, byteWarehouse, SelectedInsuranceItems, SelectedCashItems) = False Then
        '    Return "Err:No balance of this item."
        'End If

        '3====> Custom invoice (Moving Item)
        AutoPrint = True
        If ItemType <> 0 Then
            If ItemType = 1 Then bCash = True Else bCash = False
            itemFlag = "<i class=""icon-loop2 gray"" data-toggle=""tooltip"" data-delay=""500"" data-original-title=""Moved"" title=""Moved""></i>"
            AutoPrint = False
        End If

        '3====> Check Approvals
        If bCash = True Then
            'Cash
            bApproval = True
            bCash = True ' convert to Cash invoice
        Else
            'Insurance
            Dim result As Integer = func.getItemApprovalStatus(strItem, lngPatient, dateTransaction, strReference)
            If result < 0 Then
                'send errors and exit
                Select Case result
                    Case -1
                        Return "Err:intVisit not found"
                    Case -2
                        Return "Err:no record"
                    Case -3
                        'Return "Err:not approved or rejected yet"
                        Select Case ItemType
                            Case 1
                                bCash = True
                                bApproval = True
                            Case 2
                                bCash = False
                                bApproval = True
                            Case Else
                                bCash = True
                                bApproval = True
                                confirm_text = "Order is not approved or rejected yet, do you want to move it to cash invoice?"
                                confirm = True
                        End Select
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
                        'itemFlag = ""
                        bApproval = True
                        If ItemType <> 2 Then
                            If Array.IndexOf(QContacts, lngContact.ToString) > -1 Then bCash = False Else bCash = True ' convert to Cash invoice
                        Else
                            bCash = False
                        End If
                End Select
            End If
        End If

        '4====> Get Item Medical Information
        If bInvoiceType = False And ItemType <> 1 And bCash = False And Array.IndexOf(QContacts, lngContact.ToString) = -1 Then
            Dim dsMInfoCount As DataSet
            dsMInfoCount = dcl.GetDS("SELECT SUM(curQuantity) AS curQuantity FROM Hw_Treatments_Pharmacy WHERE lngPatient=" & lngPatient & " AND strReference='" & strReference & "'")
            Dim OrderItemsCount As Decimal = dsMInfoCount.Tables(0).Rows(0).Item("curQuantity")
            Dim CurrentItemCount As String() = Split(SelectedInsuranceItems, ",")
            'If CurrentItemCount.Length > OrderItemsCount Then Return "Err:Only " & OrderItemsCount & " Items are allowed in this credit invoice"
        End If

        If bCash = False Then
            bMove = True
        Else
            'If (intService = 1) And (lngTransaction > -1) And (bInvoiceType = False) Then bMove = True
            If (intGroup = 21) And (lngTransaction > -1) And (bInvoiceType = False) Then bMove = True
        End If

        '2.1===> Stop Duplicate if required
        If AllowExtraItem_Insurance = False And bCash = False And ItemType <> 2 Then
            If func.checkDuplicateItem(curOrderQuantity, strItem, SelectedInsuranceItems) = False Then
                bApproval = False
                bCash = True
                If AutoMoveExtraToCash_Insurance = False Then
                    confirm_text = "Quantity for this item is " & Math.Round(curOrderQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<br>Move it to cash?"
                    confirm = True
                End If
            End If
        End If

        '5====> Get Price
        curBasePrice = func.getPrice(curBasePrice, strItem)
        'curUnitPrice = curBaseDiscount

        '6====> Get Discount (Contact Discount=> Insurance) Or (Promotion Discount=> Cash)
        curDiscount = func.getDiscount(bCash, strItem, bytePriceType, intGroup, lngPatient, dateTransaction)

        '7====> Get Insurance Coverage
        'Dim thisPrice, thisDiscount, PatientCash, InsuranceCoverage As Decimal
        If bCash = True Then
            'Cash
            thisCoverage = 0
            curUnitPrice = curBasePrice
            'thisPrice = curBasePrice
            'PatientCash = curBasePrice
            'InsuranceCoverage = 0
            'thisDiscount = curBaseDiscount
        Else
            'Insurance
            If bApproval = True Then
                'curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
                curUnitPrice = Math.Round(curBasePrice - (curBasePrice * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
                Dim re As String = func.CheckItemCoverage(lngTransaction, lngContract, byteScheme, strItem, curBasePrice, curQuantity, dateTransaction, lngPatient, curBasePriceTotal)
                If Left(re, 4) = "Err:" Then
                    'Return re
                    thisCoverage = 0 'return the price to cash without discount
                    curUnitPrice = curBasePrice 'return the price to cash without discount
                    curDiscount = 0 'return the price to cash without discount
                    If lngContact <> 380 Then
                        bCash = True
                        confirm_text = Mid(re, 5, Len(re)) & "<br>Move it to cash?"
                        confirm = True
                    End If
                Else
                    thisCoverage = Math.Round(func.CalculateCoverage(lngTransaction, strItem, curUnitPrice, curQuantity, curCoverage, lngContract, byteScheme, bytePriceType, bytePrimaryDep, lngPatient, lngSalesman, dateTransaction, intGroup), byteCurrencyRound, MidpointRounding.AwayFromZero)
                    'thisPrice = curBasePrice
                    'PatientCash = thisCoverage
                    'InsuranceCoverage = curBasePrice - thisCoverage
                    'thisDiscount = InsuranceCoverage
                End If
            End If
        End If

        '7====> Get Tax (VAT)
        'If bTax = False Then curTax = 0
        If TaxEnabled = True Then
            If bTax = True And curTax <> 0 Then
                curVAT = curUnitPrice * (curTax / 100)
            Else
                curVAT = 0
            End If
        Else
            curVAT = 0
        End If

        '7====> Build record
        Dim Quantity, Discount, ExpireDate, Typ, Proc As String
        If bCash = True Then
            Typ = "C"
            Proc = "calculateCash"
            If ChangeQuantity_Cash = True Then Quantity = "<input type=""text"" class=""form-control input-xs width-10 text-md-center"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """ /><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>" Else Quantity = curQuantity & "<input type=""hidden"" id=""quantity"" name=""quantity_" & Typ & """ value=""" & curQuantity & """/><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/>"
            'If AddDiscount_Cash = True Then Discount = "<input type=""text"" class=""form-control form-filter input-xs width-10 text-md-center"" name=""discount"" value=""" & thisDiscount & """ />" Else Discount = thisDiscount
        Else
            Typ = "I"
            Proc = "calculateInsurance"
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
            Ret = Ret & "var item = '" & func.createItemRow(lngTransaction, RowCounter, False, itemFlag, strBarcode, strItem, strItemName, byteUnit, dateExpiry, curBasePrice, curDiscount, curQuantity, curBaseDiscount, thisCoverage, curVAT, intService, byteWarehouse, strDose, True, False, AutoPrint, bMove) & "';"
            Ret = Ret & "$('#tblInsurance' + curTab + ' > tbody').prepend(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);InsuranceOn[curTab]=!(InsuranceOn[curTab]);changeToInsurance(curTab);calculateInsurance(curTab);$('#items_I_' + curTab).val($('#items_I_' + curTab).val()+'" & strItem & ",');"
        Else
            Ret = Ret & "var item = '" & func.createItemRow(lngTransaction, RowCounter, True, itemFlag, strBarcode, strItem, strItemName, byteUnit, dateExpiry, curBasePrice, curDiscount, curQuantity, curBaseDiscount, thisCoverage, curVAT, intService, byteWarehouse, strDose, True, False, AutoPrint, bMove) & "';"
            Ret = Ret & "$('#tblCash' + curTab + ' > tbody').prepend(item);$('#counter' + curTab).val(parseInt($('#counter' + curTab).val())+1);cashOn[curTab]=!(cashOn[curTab]);changeToCash(curTab);calculateCash(curTab);$('#items_C_' + curTab).val($('#items_C_' + curTab).val()+'" & strItem & ",');"
        End If
        Ret = Ret & "refreshListeners();createPrintDoseLink(curTab);$('#txtBarcode').focus();" & Script
        Ret = Ret & "}"

        If confirm = True Then Ret = Ret & "confirm('','" & confirm_text & "', addThis);" Else Ret = Ret & "addThis();"

        Ret = Ret & "</script>"
        Return Ret
    End Function

    Public Function completeBarcode_New(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal byteBase As Byte, ByVal strFunction As String) As String
        Dim str As String = ""
        Dim strItem As String
        'Dim ExpireDate As String
        Dim Price As Decimal
        Dim ds As DataSet
        Dim lstExpire As String = ""
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
            '1=> get list of expire date
            Dim dsExpire As DataSet
            If byteBase > 0 Then
                dsExpire = dcl.GetDS("")
            Else
                'default balance
                dsExpire = dcl.GetDS("SELECT XI.byteWarehouse, W.strWarehouseEn, CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = " & intYear & " AND XI.byteWarehouse = " & byteWarehouse & " AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' GROUP BY XI.byteWarehouse, W.strWarehouseEn, XI.dateExpiry HAVING SUM(B.intSign * XI.curQuantity * U.curFactor) > 0 ORDER BY XI.dateExpiry")
            End If

            Dim firstChecked As Boolean = False
            Dim active, checked As String
            If dsExpire.Tables(0).Rows.Count > 0 Then
                For I = 0 To dsExpire.Tables(0).Rows.Count - 1
                    If firstChecked = False Then
                        firstChecked = True
                        active = "active"
                        checked = "checked=""checked"""
                    Else
                        active = ""
                        checked = ""
                    End If
                    lstExpire = lstExpire & "<label class=""btn btn-secondary " & active & """><input type=""radio"" name=""dateExpiry"" value=""" & CDate(dsExpire.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ " & checked & ">" & CDate(dsExpire.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & " ==> (<b>" & Math.Round(dsExpire.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</b>)" & "</label>"
                Next
            Else
                Return "Err:No balance for this item.."
            End If
            '2=> get price from purchases if any
            Dim dsPrice As DataSet
            dsPrice = dcl.GetDS("SELECT XI.curUnitPrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curUnitPrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10, 40) And XI.strItem='" & strItem & "' ORDER BY T.dateTransaction DESC")
            If dsPrice.Tables(0).Rows.Count > 0 Then
                Price = dsPrice.Tables(0).Rows(0).Item("Price")
            Else
                Price = 0
            End If

            '3=> buile UI
            lstExpire = "<div class=""btn-group btn-group-toggle btn-group-vertical full-width"" data-toggle=""buttons"">" & lstExpire & "</div>"
            str = str & "<div class=""row pl-1 pr-1"">"
            str = str & "<div class=""form-group""><label>" & lblExpiryDate & ":</label>" & lstExpire & "</div>"
            str = str & "<div class=""form-group""><label>" & lblBasePrice & ":</label><input class=""form-control text-md-center"" id=""curBasePrice"" type=""number"" value=""" & Math.Round(Price, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></div>"
            str = str & "</div>"
            str = str & "<script type=""text/javascript"">"
            str = str & "function completeIt(){"
            str = str & "$('input[name=""dateExpiry""]:checked').focus();"
            str = str & "var dateExpiry = $('input[name=""dateExpiry""]:checked').val();"
            str = str & "var eYear=dateExpiry.substr(2,2); var eMonth=dateExpiry.substr(5,2);"
            str = str & "var iPrice=pad(parseFloat($('#curBasePrice').val()).toFixed(2).replace('.',''),6);"
            'str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; alert(newbarcode);"
            ''str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; getItemInfo(newbarcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val(),0); $('#mdlMessage').modal('hide');"
            str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice;" & strFunction.Replace("[BARCODE]", "newbarcode") & ";$('#mdlMessage').modal('hide');"
            str = str & "}"
            str = str & "</script>"

            Dim sh As New Share.UI
            Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" id=""btnSaveBarcode"" class=""btn btn-success ml-1"" onclick=""javascript:completeIt();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.XSmall, "bg-grey bg-lighten-2")
        Else
            Return "Err:item not defined"
        End If
    End Function

    Public Function completeBarcode_old(ByVal strBarcode As String) As String
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
        str = str & "function completeIt(){"
        str = str & "var eYear=$('#dateExpiry').val().substr(2,2); var eMonth=$('#dateExpiry').val().substr(5,2);"
        str = str & "var iPrice=pad(parseFloat($('#curBasePrice').val()).toFixed(2).replace('.',''),6);"
        str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; getItemInfo(newbarcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val(),0); $('#mdlMessage').modal('hide');"
        str = str & "}"
        str = str & "</script>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" id=""btnSaveBarcode"" class=""btn btn-success ml-1"" onclick=""javascript:completeIt();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.Small, "bg-grey bg-lighten-2")
    End Function

    Public Function getQuantity(ByVal strBarcode As String) As String
        Dim str As String = ""
        Dim strItem As String
        Dim Quantity As String = "1"

        Dim Header As String
        Dim btnSave, btnClose As String
        Dim lblQuantity As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "كمية الصنف"
                lblQuantity = "الكمية"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Item Quantity"
                lblQuantity = "Quantity"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        str = str & "<div class=""row pl-1 pr-1"">"
        str = str & "<div class=""form-group""><label>" & lblQuantity & ":</label><input class=""form-control text-md-center"" id=""curQuantity"" type=""number"" value=""" & Quantity & """ /></div>"
        str = str & "</div>"
        str = str & "<script type=""text/javascript"">"
        str = str & "function assignQuantity(){"
        str = str & "if (parseInt($('#curQuantity').val()) > 0) {getItemInfo('" & strBarcode & "',$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val(),0,parseInt($('#curQuantity').val())); $('#mdlConfirm').modal('hide');}"
        str = str & "}"
        str = str & "</script>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" id=""btnSaveBarcode"" class=""btn btn-success ml-1"" onclick=""javascript:assignQuantity();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.XSmall, "bg-grey bg-lighten-2")
    End Function

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

        Return "<script type=""text/javascript"">$('#mdlAlpha').modal('hide');</script>"
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

    Public Function fillPrepare() As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance As String
        Dim btnView, btnReturn As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String
        Dim PrepareUsers As String = "anwar,على مكى,eissa,jameel,السلطان,jamal"
        Dim ButtonsPerRow As Byte = 3

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "آجل"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = ""
                'Buttons
                btnView = "عرض"
                btnReturn = "ارجاع"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Credit"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = ""
                'Buttons
                btnView = "View"
                btnReturn = "Return"
        End Select

        Dim PUsers As String() = PrepareUsers.Split(",")

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th style=""width:70px"">" & colInvoice & "</th><th class=""width-300"">" & colPatient & "</th><th class=""width-200"">" & colDoctor & "</th><th style=""width:100px"">" & colStatus & "</th><th>" & colDate & "</th></tr></thead><tbody>")

        Dim Where As String = " AND ST.bCollected = 1 AND (ST.strUserPrint IS NULL OR ST.datePrepeare IS NULL)"
        Dim disabled, RowClick As String
        Try
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = " & intYear & " AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) " & Where & " ORDER BY ST.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If p_Prepare = False Then
                    RowClick = ""
                    disabled = "disabled=""disabled"""
                Else
                    RowClick = " onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: true, ToPrepare: false}', '#mdlAlpha');"""
                    disabled = ""
                End If
                Dim Counter As Integer = 1
                Dim PButtons As String = ""
                For Each user In PUsers
                    PButtons = PButtons & "<td style=""padding:2px""><button type=""button"" style=""min-width:70px"" class=""btn btn-sm btn-secondary"" onclick=""javascript:takeOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ",'" & user & "')"">" & user & "</button></td>"
                    Counter = Counter + 1
                    If Counter > ButtonsPerRow Then
                        PButtons = PButtons & "</tr><tr>"
                        Counter = 1
                    End If
                Next

                'table.Append("<tr class=""cursor-pointer"" onclick=""javascript:showOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ", false);"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("InvoiceNo") & "</td>")
                table.Append("<td class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("PatientName") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("DoctorName") & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: true, ToPrepare: false}', '#mdlAlpha');"" class=""btn btn-sm btn-primary"" " & disabled & "> " & btnView & " </button> <button type=""button"" onclick=""javascript:returnToOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ");"" class=""btn btn-sm btn-info"" " & disabled & "> " & btnReturn & " </button></td>")
                table.Append("<td class=""p-0 text-md-center""><table style=""width: 100%"" border=""0""><tr>" & PButtons & "</td></tr></table></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try

        table.Append("</tbody></table>")

        table.Append("<script>")
        table.Append("$('table.tableAjax').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});")
        table.Append("function returnToOrder(transNo) {var dataJsonString = JSON.stringify({ lngTransaction: transNo });$.ajax({type: 'POST',url: 'ajax.aspx/returnToOrder',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        table.Append("function takeOrder(transNo, user) {var dataJsonString = JSON.stringify({ lngTransaction: transNo, strUserName: user });$.ajax({type: 'POST',url: 'ajax.aspx/takeOrder',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        table.Append("</script>")
        Return table.ToString
    End Function

    Public Function fillCheckout(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String) As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance As String
        Dim btnCheckout As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colPrepare, colStatus As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "آجل"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colPrepare = "التحضير"
                colStatus = ""
                'Buttons
                btnCheckout = "الدفع"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Credit"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colPrepare = "Prepare"
                colStatus = ""
                'Buttons
                btnCheckout = "Checkout"
        End Select

        Dim SendColumn As String = ""
        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th><th>" & colPrepare & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        'table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th></tr></thead><tbody>")

        Dim Where As String = " AND ST.bCollected = 1 AND (ST.strUserPrint IS NOT NULL OR ST.datePrepeare IS NOT NULL)"
        If byteDepartment <> 0 Then Where = Where & " AND D.byteDepartment=" & byteDepartment
        If lngSalesman <> 0 Then Where = Where & " AND ST.lngSalesman=" & lngSalesman
        If strSearch <> "" Then
            If IsNumeric(strSearch) = True Then
                Where = Where & " AND (P.strID='" & strSearch & "' OR ST.strTransaction='" & strSearch & "' OR ST.strReference='" & strSearch & "' OR ST.lngTransaction='" & strSearch & "' OR P.strPhone1='" & strSearch & "')"
            Else
                Where = Where & " AND (P.strFirst" & DataLang & " LIKE '%" & strSearch & "%' OR P.strSecond" & DataLang & " LIKE '%" & strSearch & "%' OR P.strThird" & DataLang & " LIKE '%" & strSearch & "%' OR P.strLast" & DataLang & " LIKE '%" & strSearch & "%')"
            End If
        End If
        Dim disabled, RowClick As String
        Try
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.strUserPrint FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = " & intYear & " AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) " & Where & " ORDER BY ST.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If p_Prepare = False Then
                    RowClick = ""
                    disabled = "disabled=""disabled"""
                Else
                    RowClick = " onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: false}', '#mdlAlpha');"""
                    disabled = ""
                End If
                'table.Append("<tr class=""cursor-pointer"" onclick=""javascript:showOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ", false);"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("InvoiceNo") & "</td>")
                table.Append("<td " & RowClick & " class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("PatientName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DoctorName") & "</td>")
                table.Append("<td " & RowClick & " class=""center"">" & CDate(ds.Tables(0).Rows(I).Item("InvoiceDate")).ToString(strDateFormat) & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DepartmentName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("CompanyName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("PaymentType") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("strUserPrint") & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: false}', '#mdlAlpha');"" class=""btn btn-sm btn-primary"" " & disabled & "> " & btnCheckout & " </button></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try

        table.Append("</tbody></table>")

        table.Append("<script>")
        table.Append("$('table.tableAjax').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});")
        table.Append("function sendToPrapare(transNo) {var dataJsonString = JSON.stringify({ lngTransaction: transNo });$.ajax({type: 'POST',url: 'ajax.aspx/sendToPrapare',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        table.Append("</script>")
        Return table.ToString
    End Function

    Public Function fillOrders(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String, ByVal ToPrepare As Boolean) As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance As String
        Dim btnView, btnSend As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "آجل"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = ""
                'Buttons
                btnView = "عرض"
                btnSend = "ارسال"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Credit"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = ""
                'Buttons
                btnView = "View"
                btnSend = "Send"
        End Select

        Dim SendColumn As String = ""
        If ToPrepare = True Then SendColumn = "<th>" & colStatus & "</th>"
        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th><th>" & colStatus & "</th>" & SendColumn & "</tr></thead><tbody>")
        'table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th></tr></thead><tbody>")

        Dim Where As String = ""
        If ToPrepare = True Then Where = Where & " AND (ST.bCollected = 0 OR ST.bCollected IS NULL)"
        If byteDepartment <> 0 Then Where = Where & " AND D.byteDepartment=" & byteDepartment
        If lngSalesman <> 0 Then Where = Where & " AND ST.lngSalesman=" & lngSalesman
        If strSearch <> "" Then
            If IsNumeric(strSearch) = True Then
                Where = Where & " AND (P.strID='" & strSearch & "' OR ST.strTransaction='" & strSearch & "' OR ST.strReference='" & strSearch & "' OR ST.lngTransaction='" & strSearch & "' OR P.strPhone1='" & strSearch & "')"
            Else
                Where = Where & " AND (P.strFirst" & DataLang & " LIKE '%" & strSearch & "%' OR P.strSecond" & DataLang & " LIKE '%" & strSearch & "%' OR P.strThird" & DataLang & " LIKE '%" & strSearch & "%' OR P.strLast" & DataLang & " LIKE '%" & strSearch & "%')"
            End If
        End If
        Dim disabled, RowClick As String
        Try
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM " & PharmacyDB & "Stock_Trans AS ST LEFT JOIN " & PharmacyDB & "Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN " & PharmacyDB & "Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN " & PharmacyDB & "Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN " & PharmacyDB & "Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN " & PharmacyDB & "Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = " & intYear & " AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) = '" & Today.ToString("yyyy-MM-dd") & "' " & Where & " ORDER BY ST.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If p_Prepare = False Then
                    RowClick = ""
                    disabled = "disabled=""disabled"""
                Else
                    RowClick = " onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: " & ToPrepare.ToString.ToLower & "}', '#mdlAlpha');"""
                    disabled = ""
                End If
                'table.Append("<tr class=""cursor-pointer"" onclick=""javascript:showOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ", false);"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("InvoiceNo") & "</td>")
                table.Append("<td " & RowClick & " class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("PatientName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DoctorName") & "</td>")
                table.Append("<td " & RowClick & " class=""center"">" & CDate(ds.Tables(0).Rows(I).Item("InvoiceDate")).ToString(strDateFormat) & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DepartmentName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("CompanyName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("PaymentType") & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: " & ToPrepare.ToString.ToLower & "}', '#mdlAlpha');"" class=""btn btn-sm btn-primary"" " & disabled & "> " & btnView & " </button></td>")
                If ToPrepare = True Then table.Append("<td><button type=""button"" onclick=""javascript:sendToPrapare(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ");"" class=""btn btn-sm btn-info"" " & disabled & "> " & btnSend & " </button></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try

        table.Append("</tbody></table>")

        table.Append("<script>")
        table.Append("$('table.tableAjax').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});")
        table.Append("function sendToPrapare(transNo) {var dataJsonString = JSON.stringify({ lngTransaction: transNo });$.ajax({type: 'POST',url: 'ajax.aspx/sendToPrapare',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        table.Append("</script>")
        Return table.ToString
    End Function

    Public Function fillOldOrders(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String, ByVal ToPrepare As Boolean) As String
        Dim ds As DataSet
        Dim table As New StringBuilder("")
        Dim Cash, Insurance As String
        Dim btnView, btnSend As String
        Dim colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Cash = "نقدي"
                Insurance = "آجل"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = "الحالة"
                'Buttons
                btnView = "عرض"
                btnSend = "ارسال"
            Case Else
                DataLang = "En"
                'Variables
                Cash = "Cash"
                Insurance = "Credit"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = "Status"
                'Buttons
                btnView = "View"
                btnSend = "Send"
        End Select

        Dim SendColumn As String = ""
        If ToPrepare = True Then SendColumn = "<th>" & colStatus & "</th>"
        table.Append("<table class=""table tableAjax2 table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th><th>" & colStatus & "</th>" & SendColumn & "</tr></thead><tbody>")
        'table.Append("<thead><tr><th>" & colInvoice & "</th><th>" & colPatient & "</th><th>" & colDoctor & "</th><th class=""width-100"">" & colDate & "</th><th>" & colDepartment & "</th><th>" & colCompany & "</th><th>" & colType & "</th></tr></thead><tbody>")

        Dim Where As String = ""
        If ToPrepare = True Then Where = Where & " AND (ST.bCollected = 0 OR ST.bCollected IS NULL)"
        If byteDepartment <> 0 Then Where = Where & " AND D.byteDepartment=" & byteDepartment
        If lngSalesman <> 0 Then Where = Where & " AND ST.lngSalesman=" & lngSalesman
        If strSearch <> "" Then
            If IsNumeric(strSearch) = True Then
                Where = Where & " AND (P.strID='" & strSearch & "' OR ST.strTransaction='" & strSearch & "' OR ST.strReference='" & strSearch & "' OR ST.lngTransaction='" & strSearch & "' OR P.strPhone1='" & strSearch & "')"
            Else
                Where = Where & " AND (P.strFirst" & DataLang & " LIKE '%" & strSearch & "%' OR P.strSecond" & DataLang & " LIKE '%" & strSearch & "%' OR P.strThird" & DataLang & " LIKE '%" & strSearch & "%' OR P.strLast" & DataLang & " LIKE '%" & strSearch & "%')"
            End If
        End If
        Dim disabled, RowClick As String
        Try
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & DateAdd(DateInterval.Day, -1, Today).ToString("yyyy-MM-dd") & "' " & Where & " ORDER BY ST.lngTransaction DESC")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If p_Prepare = False Then
                    RowClick = ""
                    disabled = "disabled=""disabled"""
                Else
                    RowClick = " onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: " & ToPrepare.ToString.ToLower & "}', '#mdlAlpha');"""
                    disabled = ""
                End If
                'table.Append("<tr class=""cursor-pointer"" onclick=""javascript:showOrder(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ", false);"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("TransactionNo") & """>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("InvoiceNo") & "</td>")
                table.Append("<td " & RowClick & " class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("PatientName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DoctorName") & "</td>")
                table.Append("<td " & RowClick & " class=""center"">" & CDate(ds.Tables(0).Rows(I).Item("InvoiceDate")).ToString(strDateFormat) & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("DepartmentName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("CompanyName") & "</td>")
                table.Append("<td " & RowClick & ">" & ds.Tables(0).Rows(I).Item("PaymentType") & "</td>")
                table.Append("<td><button type=""button"" onclick=""javascript:showModal('viewOrder', '{TransNo: " & ds.Tables(0).Rows(I).Item("TransactionNo") & ", ShowOnly: false, ToPrepare: " & ToPrepare.ToString.ToLower & "}', '#mdlAlpha');"" class=""btn btn-sm btn-primary"" " & disabled & "> " & btnView & " </button></td>")
                If ToPrepare = True Then table.Append("<td><button type=""button"" onclick=""javascript:sendToPrapare(" & ds.Tables(0).Rows(I).Item("TransactionNo") & ");"" class=""btn btn-sm btn-info"" " & disabled & "> " & btnSend & " </button></td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try


        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax2').dataTable({language: tableLanguage,searching: searching,ordering: ordering,paging: paging,'info': info,'order': order,'columnDefs': [{ orderable: false, targets: noorder }]});</script>")
        Return table.ToString
    End Function

    Public Function sendToPrapare(ByVal lngTransaction As Long) As String
        Const ErrorType As Integer = 5

        Dim ds As DataSet
        Dim DataLang As String
        Dim usr As New Share.User

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.byteStatus = 1 AND ST.bCollected1 = 1 AND (ST.bCollected = 0 OR ST.bCollected IS NULL) AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                dcl.ExecSQuery("UPDATE Stock_Trans SET bCollected=1 WHERE lngTransaction=" & lngTransaction)
                usr.AddLog(strUserName, Now, 1, "Order", lngTransaction, 2, "Send order to prepare")
                Return "<script type=""text/javascript"">msg('','Order has sent successfully!','info');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
            Catch ex As Exception
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "Order", lngTransaction, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function

    Public Function returnToOrder(ByVal lngTransaction As Long) As String
        Const ErrorType As Integer = 6

        Dim ds As DataSet
        Dim DataLang As String
        Dim usr As New Share.User

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.byteStatus = 1 AND ST.bCollected1 = 1 AND ST.bCollected = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                dcl.ExecSQuery("UPDATE Stock_Trans SET bCollected=NULL WHERE lngTransaction=" & lngTransaction)
                usr.AddLog(strUserName, Now, 1, "Order", lngTransaction, 2, "Return order from prepare")
                Return "<script type=""text/javascript"">msg('','Order has returned successfully!','info');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
            Catch ex As Exception
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "Order", lngTransaction, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function

    Public Function takeOrder(ByVal lngTransaction As Long, ByVal strUserName As String) As String
        Const ErrorType As Integer = 7

        Dim ds As DataSet
        Dim DataLang As String
        Dim usr As New Share.User

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.byteStatus = 1 AND ST.bCollected1 = 1 AND ST.bCollected = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
        If ds.Tables(0).Rows.Count > 0 Then
            Try
                dcl.ExecSQuery("UPDATE Stock_Trans SET strUserPrint='" & strUserName & "', datePrepeare='" & Now.ToString("yyyy-MM-dd HH:mm") & "' WHERE lngTransaction=" & lngTransaction)
                usr.AddLog(strUserName, Now, 1, "Order", lngTransaction, 2, "Take order to prepare")
                Return "<script type=""text/javascript"">msg('','Order has taken successfully!','info');$('#mdlAlpha').modal('hide');$('#row" & lngTransaction & "').remove();updateUI();</script>"
            Catch ex As Exception
                If ErrorLogsEnabled Then usr.AddError(strUserName, Now, 1, "Order", lngTransaction, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:This record is unavailable, please refresh the orders again.."
        End If
    End Function
End Class

