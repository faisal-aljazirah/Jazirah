Imports System.Xml
Imports System.Web
Imports System.Text
Imports System.Web.Script.Serialization

Public Class Cashier
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim func As New Functions
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
    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, AllowPrintEmptyDose, AllowCashbox As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim p_Prepare, p_Sales, p_Cashier As Boolean

    Dim AllowCancel As Boolean

    Private Class NameValue
        Public Property name As String
        Public Property value As String
    End Class

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
        loadUserSettings()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName)
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
        If application.SelectSingleNode("AllowCashbox") Is Nothing Then AllowCashbox = True Else AllowCashbox = application.SelectSingleNode("AllowCashbox").InnerText

        If ByteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""
    End Sub

    Private Function createCashBox(ByVal strPatientName As String, ByVal curAmount As Decimal, ByVal curVat As Decimal, ByVal IsCashier As Boolean) As String()
        Dim body As New StringBuilder("")
        Dim btnPayment, btnCash, btnCredit, btnSplit, btnJoin, btnCancel As String
        Dim lblPatient, lblCash, lblPaid, lblRemind, lblCredit, lblAmount, lblVat, lblTotal, btnCalculator As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'labels
                lblPatient = "المريض"
                lblAmount = "المبلغ"
                lblVat = "الضريبة"
                lblTotal = "المجموع"
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
                lblVat = "VAT"
                lblTotal = "Total"
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

        Dim curQuickPay As Decimal
        body.Append("<div class=""row""><div class=""col-md-12""><div class=""col-md-3 text-md-right text-bold-900"">" & lblPatient & ":</div><div class=""col-md-9 teal"">" & strPatientName & "</div></div><div class=""col-md-12""><hr /></div><div class=""col-md-6"">")
        'left part
        If TaxEnabled = True Then
            body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblAmount & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_amount"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""" & curAmount & """ /></div></div>")
            body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblVat & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_vat"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""" & curVat & """ /></div></div>")
            body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblTotal & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_total"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey border-red box-shadow-1"" value=""" & curAmount + curVat & """ /></div></div>")
            curQuickPay = curAmount + curVat
        Else
            body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblAmount & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_total"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey border-red box-shadow-1"" value=""" & curAmount & """ /></div></div>")
            curQuickPay = curAmount
        End If
        body.Append("<div class=""col-md-12"" id=""divTotalPaid""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblPaid & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_paid"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey border-blue box-shadow-1 selAll"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12"" id=""divTotalCash""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblCredit & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_credit"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey border-blue box-shadow-1 selAll"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12"" id=""divTotalCredit""><div class=""col-md-5 p-0""><label class=""col-form-label""><h5 class=""text-md-right"">" & lblCash & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_cash"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey border-blue box-shadow-1 selAll"" value=""0"" /></div></div>")
        body.Append("<div class=""col-md-12""><div class=""col-md-5 p-0""><label lass=""col-form-label""><h5 class=""text-md-right"">" & lblRemind & ":</h5></label></div><div class=""col-md-7 p-0""><input type=""number"" id=""net_remind"" readonly=""readonly"" class=""form-control text-md-center white text-bold-100 col-md-12 bg-grey"" value=""0"" /></div></div>")

        body.Append("</div><div class=""col-md-6"">")
        'right part
        body.Append("<table style=""width:100%""><tr><td colspan=""3""><button type=""button"" class=""btn btn-teal col-md-12"">" & btnCalculator & "</button></td></tr><tr><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">5</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">10</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">50</button></td></tr><tr><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">100</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">200</button></td><td><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">500</button></td></tr><tr><td colspan=""3""><button type=""button"" class=""btn btn-teal col-md-12 calcNum"">" & curQuickPay & "</button></td></tr></table>")
        body.Append("</div></div>")

        Dim btns As String
        If OnePaymentForCashier = True Then
            btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment"" onclick=""javascript:if(validatePayment()) {disableMe(this, '#btnCash, #btnCredit');updatePayment(3);}""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        Else
            btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment"" onclick=""javascript:if(validatePayment()) {disableMe(this, '#btnCash, #btnCredit');updatePayment(3);}""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCash"" onclick=""javascript:if(validatePayment()) {disableMe(this, '#btnPayment, #btnCredit');updatePayment(1);}""><i class=""icon-cash""></i> " & btnCash & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCredit"" onclick=""javascript:if(validatePayment()) {disableMe(this, '#btnCash, #btnPayment');updatePayment(2);}""><i class=""icon-credit-card""></i> " & btnCredit & "</button><button type=""button"" class=""btn btn-primary ml-1"" id=""btnSplit""><i class=""icon-calculator""></i> " & btnSplit & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        End If
        'btns = "<button type=""button"" class=""btn btn-success mr-2"" id=""btnPayment""><i class=""icon-cash""></i> " & btnPayment & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCash""><i class=""icon-cash""></i> " & btnCash & "</button><button type=""button"" class=""btn btn-success mr-1"" id=""btnCredit""><i class=""icon-credit-card""></i> " & btnCredit & "</button><button type=""button"" class=""btn btn-primary ml-1"" id=""btnSplit""><i class=""icon-calculator""></i> " & btnSplit & "</button><button type=""button"" class=""btn btn-warning ml-1"" id=""btnCancel"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnCancel & "</button>"
        body.Append("<script type=""text/javascript"">")
        body.Append("var currentFocus=$('#net_paid');$('#net_paid, #net_credit, #net_cash').focus(function() {currentFocus = $(this)});")
        If OnePaymentForCashier = True Then body.Append("var onePayment = true; var byTotal = false;") Else body.Append("var onePayment = false; var byTotal = false;")
        body.Append("btnSplit='<i class=""icon-calculator""></i> " & btnSplit & "';btnJoin='<i class=""icon-calculator""></i> " & btnJoin & "';")
        body.Append("$('#net_paid').on('change paste keyup', calcRemind);$('#net_cash').on('change paste keyup', calcRemind);$('#net_credit').on('change paste keyup', calcRemind);$('.calcNum').click(function () {if (typeof currentFocus !== 'undefined' && currentFocus != '') {$(currentFocus).val($(this).text()).focus().select();} else {if (onePayment == true) $('#net_paid').val($(this).text()).focus();  else { if (byTotal == true) $('#net_paid').val($(this).text()).focus(); else $('#net_cash').val($(this).text()).focus();}}calcRemind();});$('#btnSplit').click(changePayment);changePayment();")
        If ForcePaymentOnCloseInvoice = True Then body.Append("function validatePayment(){if($('#net_remind').val()<=0) return true; else return false;}") Else body.Append("function validatePayment(){return true}")
        body.Append("refreshListeners()")
        body.Append("</script>")
        Return {body.ToString, btns}
    End Function

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

    Public Function viewCashier(ByVal lngTransaction As Long) As String
        Dim ds, dsTemp, dsInvoice As DataSet
        Dim lngPatient, lngSalesman, lngContact As Long
        Dim coveredCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim coveredVat, nonCoveredVat, TotalVatAmount As Decimal
        Dim bCreatCash, IsCash As Boolean
        Dim strPatientName As String
        Dim dateTransaction As Date

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        'analize data and verify
        If lngTransaction > 0 Then
            dsInvoice = dcl.GetDS("SELECT ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, ST.strReference AS ClinicInvoiceNo, ST.bCreatCash, STI.curCash, STI.curCashVAT FROM Stock_Trans AS ST INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Invoices AS STI ON ST.lngTransaction=STI.lngTransaction WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND ST.lngTransaction = " & lngTransaction)
            If dsInvoice.Tables(0).Rows.Count > 0 Then
                lngPatient = dsInvoice.Tables(0).Rows(0).Item("PatientNo")
                strPatientName = dsInvoice.Tables(0).Rows(0).Item("PatientName")
                TotalCashAmount = dsInvoice.Tables(0).Rows(0).Item("curCash")
                TotalVatAmount = dsInvoice.Tables(0).Rows(0).Item("curCashVAT")
                If IsDBNull(dsInvoice.Tables(0).Rows(0).Item("bCreatCash")) = True Or dsInvoice.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
                If bCreatCash = True Then
                    dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Invoices AS STI ON ST.lngTransaction=STI.lngTransaction WHERE ST.strReference='" & dsInvoice.Tables(0).Rows(0).Item("ClinicInvoiceNo") & "' AND ST.lngPatient=" & lngPatient & " AND ST.dateTransaction='" & CDate(dsInvoice.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND ST.bSubCash=1")
                    If dsTemp.Tables(0).Rows.Count > 0 Then
                        TotalCashAmount = TotalCashAmount + dsTemp.Tables(0).Rows(0).Item("curCash")
                        TotalVatAmount = TotalVatAmount + dsTemp.Tables(0).Rows(0).Item("curCashVAT")
                    End If
                End If
                TotalCashAmount = Math.Round(TotalCashAmount, byteCurrencyRound, MidpointRounding.AwayFromZero)
                TotalVatAmount = Math.Round(TotalVatAmount, byteCurrencyRound, MidpointRounding.AwayFromZero)
            Else
                ' This part for old records that dosent have data in (Stock_Trans_Invoices) table
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus,ST.bCreatCash,ST.bCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND ST.lngTransaction = " & lngTransaction)
                lngPatient = ds.Tables(0).Rows(0).Item("PatientNo")
                strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
                IsCash = ds.Tables(0).Rows(0).Item("bCash")
                lngContact = ds.Tables(0).Rows(0).Item("CompanyNo")
                lngSalesman = ds.Tables(0).Rows(0).Item("DoctorNo")
                dateTransaction = ds.Tables(0).Rows(0).Item("TransactionDate")
                Dim dsCovered, dsNonCovered As DataSet
                dsCovered = dcl.GetDS("SELECT SUM(XI.curUnitPrice) AS Total, SUM(XI.curVAT) AS TotalVat FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction)
                coveredCash = CDec("0" & dsCovered.Tables(0).Rows(0).Item("Total").ToString)
                coveredVat = CDec("0" & dsCovered.Tables(0).Rows(0).Item("TotalVat").ToString)

                Dim MaxP, CICov, MICov As Decimal
                Dim bPercent As Boolean
                Dim curPercent As Decimal
                If IsCash = False Then
                    Dim result As String() = func.getCoverage(lngPatient, lngContact)
                    If Left(result(0), 4) <> "Err:" Then
                        MaxP = result(2)
                        curPercent = result(1)
                        bPercent = result(0)
                    Else
                        Return result(0)
                    End If
                    CICov = func.getTotalClinicInvoices(lngPatient, lngSalesman, dateTransaction)
                    MICov = func.getTotalPharmacyInvoices(lngPatient, lngSalesman, dateTransaction, lngTransaction, False)
                    coveredCash = func.calcCoveredCash(coveredCash, curPercent, bPercent, MaxP, CICov, MICov)
                Else
                    MaxP = 0
                    CICov = 0
                    MICov = 0
                End If
                If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
                If bCreatCash = True Then
                    Dim lngXlink As Long = 0
                    dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & ds.Tables(0).Rows(0).Item("ClinicInvoiceNo") & "' AND lngPatient=" & lngPatient & " AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND bSubCash=1")
                    If dsTemp.Tables(0).Rows.Count > 0 Then
                        dsNonCovered = dcl.GetDS("SELECT SUM(XI.curUnitPrice) AS Total, SUM(XI.curVAT) AS TotalVat FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & dsTemp.Tables(0).Rows(0).Item("lngTransaction"))
                        nonCoveredCash = CDec("0" & dsNonCovered.Tables(0).Rows(0).Item("Total").ToString)
                        nonCoveredVat = CDec("0" & dsNonCovered.Tables(0).Rows(0).Item("TotalVat").ToString)
                    Else
                        nonCoveredCash = 0
                        nonCoveredVat = 0
                    End If
                Else
                    nonCoveredCash = 0
                    nonCoveredVat = 0
                End If
                TotalCashAmount = Math.Round((coveredCash + nonCoveredCash), byteCurrencyRound, MidpointRounding.AwayFromZero)
                TotalVatAmount = Math.Round((coveredVat + nonCoveredVat), byteCurrencyRound, MidpointRounding.AwayFromZero)
            End If
        Else
            Return "Err:"
        End If

        'Dim source As String
        Dim res As String() = createCashBox(strPatientName, TotalCashAmount, TotalVatAmount, True)
        Dim body As String = res(0)
        Dim btns As String = res(1)
        'If OnePaymentForCashier Then source = "$('#net_total').val()" Else source = "" ' ====> for later work
        Dim script As String = "<script type=""text/javascript"">function updatePayment(type){getPaid1(" & lngTransaction & ",parseFloat($('#net_paid').val()) + parseFloat($('#net_cash').val()),$('#net_credit').val(),type)};</script>"

        Dim mdl As New Share.UI
        Return mdl.drawModal("Cashier", body & script, btns, Share.UI.ModalSize.Medium, "bg-grey bg-lighten-3", "", "text-md-center")
    End Function

    Public Function viewCashier(ByVal TabCounter As Integer, ByVal Fields As String) As String
        Dim ds As DataSet
        Dim lngTransaction, lngPatient As Long
        Dim coveredCash, deductionCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim coveredVat, deductionVat, nonCoveredVat, TotalVatAmount As Decimal
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
                Case "coveredVat"
                    coveredVat = field(I).value()
                Case "deductionVat"
                    deductionVat = field(I).value()
                Case "nonCoveredVat"
                    nonCoveredVat = field(I).value()
            End Select
        Next

        'analize data and verify
        TotalCashAmount = coveredCash + nonCoveredCash
        TotalVatAmount = coveredVat + nonCoveredVat
        If lngTransaction > 0 Then
            ' Insurance
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
            lngPatient = ds.Tables(0).Rows(0).Item("PatientNo")
            strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
        Else
            ' Cash
            lngPatient = 16
            ds = dcl.GetDS("SELECT RTRIM(LTRIM(ISNULL(strFirstEn,'') + ' ') + LTRIM(ISNULL(strSecondEn,'') + ' ') + LTRIM(ISNULL(strThirdEn,'') + ' ') + LTRIM(ISNULL(strLastEn,'') + ' ')) AS PatientName,* FROM Hw_Patients WHERE lngPatient=" & lngPatient)
            strPatientName = ds.Tables(0).Rows(0).Item("PatientName")
        End If

        Dim res As String() = createCashBox(strPatientName, TotalCashAmount, TotalVatAmount, False)
        Dim body As String = res(0)
        Dim btns As String = res(1)
        Dim script As String = "<script type=""text/javascript"">function updatePayment(type){getPaid2(" & TabCounter & ",'" & Replace(Fields, """", "|") & "',parseFloat($('#net_paid').val()) + parseFloat($('#net_cash').val()),$('#net_credit').val(),type)};</script>"

        Dim mdl As New Share.UI
        Return mdl.drawModal("Cashier", body & script, btns, Share.UI.ModalSize.Medium, "bg-grey bg-lighten-3", "", "text-md-center")
    End Function

    Public Function GetPaid(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim coveredCash, nonCoveredCash, TotalCashAmount As Decimal
        Dim coveredVat, nonCoveredVat, TotalVatAmount As Decimal
        Dim CashItem, InsuranceItem As Integer
        Dim bCreatCash As Boolean
        Dim lngTransaction_2 As Long = 0 'For the second invoice if any

        If lngTransaction > 0 Then
            ' Insurance
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus,bCreatCash FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.lngTransaction = " & lngTransaction)
            Dim dsTemp, dsCovered, dsNonCovered As DataSet
            dsCovered = dcl.GetDS("SELECT SUM(XI.curBasePrice) AS Total, SUM(XI.curVAT) AS TotalVat, COUNT(XI.lngXlink) AS ItemCount FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction)
            coveredCash = dsCovered.Tables(0).Rows(0).Item("Total")
            coveredVat = dsCovered.Tables(0).Rows(0).Item("TotalVat")
            InsuranceItem = dsCovered.Tables(0).Rows(0).Item("ItemCount")
            If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
            If bCreatCash = True Then
                Dim lngXlink As Long = 0
                dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans WHERE strReference='" & ds.Tables(0).Rows(0).Item("ClinicInvoiceNo") & "' AND lngPatient=" & ds.Tables(0).Rows(0).Item("PatientNo") & " AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("TransactionDate")).ToString("yyyy-MM-dd") & "' AND bSubCash=1 AND bytebase=50 AND byteStatus>0")
                lngTransaction_2 = dsTemp.Tables(0).Rows(0).Item("lngTransaction")
                dsNonCovered = dcl.GetDS("SELECT SUM(XI.curBasePrice) AS Total, SUM(XI.curVAT) AS TotalVat, COUNT(XI.lngXlink) AS ItemCount FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink WHERE X.lngTransaction=" & lngTransaction_2)
                nonCoveredCash = dsNonCovered.Tables(0).Rows(0).Item("Total")
                nonCoveredVat = dsNonCovered.Tables(0).Rows(0).Item("TotalVat")
                CashItem = dsNonCovered.Tables(0).Rows(0).Item("ItemCount")
            Else
                nonCoveredCash = 0
            End If
        Else
            Return "Err:"
        End If
        TotalCashAmount = Math.Round((coveredCash + nonCoveredCash), byteCurrencyRound, MidpointRounding.AwayFromZero)
        TotalVatAmount = Math.Round((coveredVat + nonCoveredVat), byteCurrencyRound, MidpointRounding.AwayFromZero)

        'validate payment with invoice
        If CashItem + InsuranceItem = 0 Then Return "Err:No items in this invoice!"
        'If TotalCashAmount <> (coveredCash + nonCoveredCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        'If TotalVatAmount <> (coveredVat + nonCoveredVat) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        'If (TotalCashAmount + TotalVatAmount) < (P_SPAN + P_Cash) Then Return "Err:Payment dose not match the invoice total!"
        ' Payment type
        '==> here must decide if the payment is CASH or SPAN or BOTH (do it later)
        'convert trans to invoice
        Dim res As String = closeInvoice(lngTransaction, P_Cash, P_SPAN, PaymentType)
        If Left(res, 4) = "Err:" Then Return res

        Return "<script type=""text/javascript"">$('#mdlMessage').modal('hide');$('#large').modal('hide');$('#row" & lngTransaction & "').remove();msg('','Invoice close successfully!','info');" & res & "</script>"
    End Function

    Public Function GetPaid(ByVal tabCounter As Integer, ByVal Fields As String, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim ds As DataSet
        Dim body As New StringBuilder("")
        Dim lngTransaction As Long
        Dim coveredCash, deductionCash, nonCoveredCash As Decimal
        Dim coveredVat, deductionVat, nonCoveredVat As Decimal
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
                Case "coveredVat"
                    coveredVat = field(I).value()
                Case "deductionVat"
                    deductionVat = field(I).value()
                Case "nonCoveredVat"
                    nonCoveredVat = field(I).value()
            End Select
        Next
        If lngTransaction = -1 Then lngTransaction = CLng("0" & result)

        ' Collect items
        Dim Items_Insurance, Items_Cash As New List(Of InvoiceItem)
        Dim Total_Insurance, Total_Cash, Total_All As Decimal
        Dim Total_V_Insurance, Total_V_Cash, Total_V_All As Decimal
        If cashOnly = False Then
            Items_Insurance = getInvoiceItems(Fields, False)
            For Each item As InvoiceItem In Items_Insurance
                Total_Insurance = Total_Insurance + item.Coverage
                Total_V_Insurance = Total_V_Insurance + item.VAT
            Next
        End If
        Items_Cash = getInvoiceItems(Fields, True)
        For Each item As InvoiceItem In Items_Cash
            Total_Cash = Total_Cash + item.UnitPrice
            Total_V_Cash = Total_V_Cash + item.VAT
        Next
        Total_All = Math.Round(Total_Insurance + Total_Cash, byteCurrencyRound, MidpointRounding.AwayFromZero)
        Total_V_All = Math.Round(Total_V_Insurance + Total_V_Cash, byteCurrencyRound, MidpointRounding.AwayFromZero)
        'validate payment with invoice
        If Items_Cash.Count + Items_Insurance.Count = 0 Then Return "Err:No items in this invoice!"
        If Items_Insurance.Count = 0 And Items_Cash.Count > 0 Then lngTransaction = CLng("0" & result) '<== Convert to cash only if no credit items
        'If Total_All <> (coveredCash + nonCoveredCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        'If Total_V_All <> (coveredVat + nonCoveredVat) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        'If (Total_All + Total_V_All) < (P_SPAN + P_Cash) Then Return "Err:Payment dose not match the invoice total!"
        ' Payment type
        '==> here must decide if the payment is CASH or SPAN or BOTH (do it later)
        'convert trans to invoice
        Dim res As String = closeInvoice(lngTransaction, P_Cash, P_SPAN, PaymentType)
        If Left(res, 4) = "Err:" Then Return res

        Return "<script type=""text/javascript"">$('#mdlMessage').modal('hide');$('#row" & lngTransaction & "').remove();msg('','Invoice close successfully!','info');" & res & "</script>"
    End Function

    Private Function createPrintBox(ByVal lngTransaction As Long, ByVal lngTransaction_2 As Long) As String
        Dim Header, btnPrintInvoice, btnPrintCash, btnPrintInsurance, btnClose As String
        Dim strBox As String = ""

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "طباعة الفاتورة.."
                btnPrintInvoice = "'طباعة"
                btnPrintCash = "فاتورة النقدي"
                btnPrintInsurance = "فاتورة الآجل"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Print Invoice.."
                btnPrintInvoice = "Print"
                btnPrintCash = "Cash Invoice"
                btnPrintInsurance = "Credit Invoice"
                btnClose = "Close"
        End Select

        Dim Extra As String = ""
        Try
            Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact WHERE T.lngTransaction = " & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("bCash") = 0 Then
                    Extra = "<h4 class=""purple darken-2"">" & ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString & "</h4>"
                End If
            End If
        Catch ex As Exception

        End Try

        strBox = strBox & "<div class=""row""><div class=""col-md-12 text-md-center"">"
        strBox = strBox & Extra
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

    Private Function closeInvoice2(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Const ErrorType As Integer = 1

        If lngTransaction > 0 Then
            Dim lngTransaction_2 As Long = 0
            Try
                ' Get Payment Type (For Table [Stock_Trans])
                Dim CC As String = ""
                If PaymentType <> 1 Then CC = ",lngCRCard=330"
                ' Get last invoice number
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE byteBase = 40")
                Dim strTransaction As String
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("LastNo")) Then
                    strTransaction = 1
                Else
                    strTransaction = dsLast.Tables(0).Rows(0).Item("LastNo") + 1
                End If
                ' Get byteTransType
                Dim dsTransType As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteBase = 40")
                Dim byteTransType As Byte = dsTransType.Tables(0).Rows(0).Item("byteTransType")
                ' Update transaction
                dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction & "', byteBase=40, dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteTransType=" & byteTransType & ", dateClosedValid='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1" & CC & " WHERE lngTransaction = " & lngTransaction)
                ' Update Audit
                Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction)
                If dsAudit.Tables(0).Rows.Count > 0 Then
                    Dim CreateSQL As String = ""
                    Dim LastSaveSQL As String = ""
                    Dim ApproveSQL As String = ""
                    Dim CashSQL As String = ""
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCreatedBy")) Then CreateSQL = ",strCreatedBy='" & strUserName & "', dateCreated='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strLastSavedBy")) Then LastSaveSQL = ",strLastSavedBy='" & strUserName & "', dateLastSaved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strApprovedBy")) Then ApproveSQL = ",strApprovedBy='" & strUserName & "', dateApproved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    'If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction)
                Else
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                End If
                ' update user in invoices
                dcl.ExecSQuery("UPDATE Stock_Trans_Invoices SET dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "', strUserName='" & strUserName & "' WHERE lngTransaction = " & lngTransaction)
                ' update user logs
                Dim usr As New Share.User
                usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction, 2, "Get payment")

                'Get Related Cash Invoice
                Dim ds, dsTemp As DataSet
                Dim bCreatCash, bCash As Boolean
                Dim Amount As Decimal
                ds = dcl.GetDS("SELECT T.bCreatCash,T.bCash,T.strReference,T.lngPatient,T.dateTransaction,TI.curCash,TI.curCashVAT FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
                bCash = ds.Tables(0).Rows(0).Item("bCash")
                If bCreatCash = True Then
                    dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.strReference='" & ds.Tables(0).Rows(0).Item("strReference") & "' AND T.lngPatient=" & ds.Tables(0).Rows(0).Item("lngPatient") & " AND T.dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "' AND T.bSubCash=1 AND T.byteBase=50 AND T.byteStatus>0")
                    If dsTemp.Tables(0).Rows.Count > 0 Then
                        lngTransaction_2 = dsTemp.Tables(0).Rows(0).Item("lngTransaction")
                        ' Get last invoice number
                        Dim dsLast2 As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = " & intYear & " AND byteBase = 40")
                        Dim strTransaction2 As String
                        If IsDBNull(dsLast2.Tables(0).Rows(0).Item("LastNo")) Then
                            strTransaction2 = 1
                        Else
                            strTransaction2 = dsLast2.Tables(0).Rows(0).Item("LastNo") + 1
                        End If
                        ' Update transaction
                        dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction2 & "', byteBase=40, dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteTransType=" & byteTransType & ", dateClosedValid='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1" & CC & " WHERE lngTransaction = " & lngTransaction_2)
                        ' Update Audit
                        Dim dsAudit2 As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction_2)
                        If dsAudit2.Tables(0).Rows.Count > 0 Then
                            Dim CreateSQL As String = ""
                            Dim LastSaveSQL As String = ""
                            Dim ApproveSQL As String = ""
                            Dim CashSQL As String = ""
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCreatedBy")) Then CreateSQL = ",strCreatedBy='" & strUserName & "', dateCreated='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strLastSavedBy")) Then LastSaveSQL = ",strLastSavedBy='" & strUserName & "', dateLastSaved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strApprovedBy")) Then ApproveSQL = ",strApprovedBy='" & strUserName & "', dateApproved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            'If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                            dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction_2 & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction_2)
                        Else
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction_2 & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                        End If
                        ' update user in invoices
                        dcl.ExecSQuery("UPDATE Stock_Trans_Invoices SET dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "', strUserName='" & strUserName & "' WHERE lngTransaction = " & lngTransaction_2)
                        ' update user logs
                        usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction_2, 2, "Get payment")
                    End If
                End If

                '--------------------New Table---------------------------------
                dcl.ExecSQuery("DELETE FROM Stock_Trans_Payments WHERE lngTransaction=" & lngTransaction)
                dcl.ExecSQuery("DELETE FROM Stock_Trans_Payments WHERE lngTransaction=" & lngTransaction_2)
                If P_Cash + P_SPAN <> 0 Then
                    Dim Cash_Percent As Decimal
                    Dim SPAN_Percent As Decimal
                    If IsDBNull(ds.Tables(0).Rows(0).Item("curCash")) Then Amount = 0 Else Amount = ds.Tables(0).Rows(0).Item("curCash") + ds.Tables(0).Rows(0).Item("curCashVAT")
                    Select Case PaymentType
                        Case 1
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & Amount & ", 0)")
                        Case 2
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & Amount & ", 0)")
                        Case 3
                            If lngTransaction_2 = 0 Then
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & P_Cash & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & P_SPAN & ", 0)")
                            Else
                                Cash_Percent = Math.Round((P_Cash / (P_Cash + P_SPAN)) * 100, 2, MidpointRounding.AwayFromZero)
                                SPAN_Percent = Math.Round(100 - Cash_Percent, 2, MidpointRounding.AwayFromZero)
                                Dim Cash_Credit As Decimal = Math.Round((Amount * Cash_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                Dim SPAN_Credit As Decimal = Math.Round((Amount * SPAN_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & Cash_Credit & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & SPAN_Credit & ", 0)")
                            End If
                    End Select
                    ' Second Invoice (Cash)
                    If lngTransaction_2 > 0 Then
                        Amount = dsTemp.Tables(0).Rows(0).Item("curCash") + dsTemp.Tables(0).Rows(0).Item("curCashVAT")
                        Select Case PaymentType
                            Case 1
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 1, " & Amount & ", 0)")
                            Case 2
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 2, " & Amount & ", 0)")
                            Case 3
                                Dim Cash_Cash As Decimal = Math.Round((Amount * Cash_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                Dim SPAN_Cash As Decimal = Math.Round((Amount * SPAN_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 1, " & Cash_Cash & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 2, " & SPAN_Cash & ", 0)")
                        End Select
                    End If
                End If
                '--------------------------------------------------------------

                Dim PrintScript As String = "$('#mdlAlpha').modal('hide');"
                Select Case PrintInvoice
                    Case 0 'No Print
                        ' Nothing
                    Case 1 'Auto Print
                        PrintScript = ""
                    Case 2 'Ask To Print
                        PrintScript = "$('#mdlAlpha').html('" & createPrintBox(lngTransaction, lngTransaction_2) & "');$('#mdlAlpha').modal('show');"
                    Case 3 'User Defined
                        '
                End Select
                Return PrintScript
            Catch ex As Exception
                Dim usr As New Share.User
                usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
                If lngTransaction_2 > 0 Then usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction_2, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err: Transaction Lost"
        End If
    End Function

    Private Function closeInvoice(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Const ErrorType As Integer = 2

        If lngTransaction > 0 Then
            Dim lngTransaction_2 As Long = 0
            Try
                Dim ds, dsTemp As DataSet
                Dim bCreatCash, bCash As Boolean
                Dim Amount As Decimal
                ds = dcl.GetDS("SELECT T.bCreatCash,T.bCash,T.strReference,T.lngPatient,T.dateTransaction,TI.curCash,TI.curCashVAT FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                If IsDBNull(ds.Tables(0).Rows(0).Item("bCreatCash")) = True Or ds.Tables(0).Rows(0).Item("bCreatCash").ToString = "0" Then bCreatCash = False Else bCreatCash = True
                bCash = ds.Tables(0).Rows(0).Item("bCash")
                If bCash = False Then
                    ' Get Payment Type (For Table [Stock_Trans])
                    Dim CC As String = ""
                    If PaymentType <> 1 Then CC = ",lngCRCard=330"
                    ' Get last invoice number
                    Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = " & intYear & " AND byteBase = 40")
                    Dim strTransaction As String
                    If IsDBNull(dsLast.Tables(0).Rows(0).Item("LastNo")) Then
                        strTransaction = 1
                    Else
                        strTransaction = dsLast.Tables(0).Rows(0).Item("LastNo") + 1
                    End If
                    ' Get byteTransType
                    Dim dsTransType As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteBase = 40")
                    Dim byteTransType As Byte = dsTransType.Tables(0).Rows(0).Item("byteTransType")
                    ' Update transaction
                    dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction & "', byteBase=40, dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteTransType=" & byteTransType & ", dateClosedValid='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1" & CC & " WHERE lngTransaction = " & lngTransaction)
                    ' Update Audit
                    Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngTransaction)
                    If dsAudit.Tables(0).Rows.Count > 0 Then
                        Dim CreateSQL As String = ""
                        Dim LastSaveSQL As String = ""
                        Dim ApproveSQL As String = ""
                        Dim CashSQL As String = ""
                        If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCreatedBy")) Then CreateSQL = ",strCreatedBy='" & strUserName & "', dateCreated='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strLastSavedBy")) Then LastSaveSQL = ",strLastSavedBy='" & strUserName & "', dateLastSaved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strApprovedBy")) Then ApproveSQL = ",strApprovedBy='" & strUserName & "', dateApproved='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        'If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction)
                    Else
                        dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                    End If
                    ' update user in invoices
                    dcl.ExecSQuery("UPDATE Stock_Trans_Invoices SET dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "', strUserName='" & strUserName & "' WHERE lngTransaction = " & lngTransaction)
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction, 2, "Get payment")

                    'Get Related Cash Invoice
                    If bCreatCash = True Then
                        dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.strReference='" & ds.Tables(0).Rows(0).Item("strReference") & "' AND T.lngPatient=" & ds.Tables(0).Rows(0).Item("lngPatient") & " AND T.dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "' AND T.bSubCash=1 AND T.byteBase=50 AND T.byteStatus>0")
                        If dsTemp.Tables(0).Rows.Count > 0 Then
                            lngTransaction_2 = dsTemp.Tables(0).Rows(0).Item("lngTransaction")
                            ' Get last invoice number
                            Dim dsLast2 As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = " & intYear & " AND byteBase = 40")
                            Dim strTransaction2 As String
                            If IsDBNull(dsLast2.Tables(0).Rows(0).Item("LastNo")) Then
                                strTransaction2 = 1
                            Else
                                strTransaction2 = dsLast2.Tables(0).Rows(0).Item("LastNo") + 1
                            End If
                            ' Update transaction
                            dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction2 & "', byteBase=40, dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteTransType=" & byteTransType & ", dateClosedValid='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1" & CC & " WHERE lngTransaction = " & lngTransaction_2)
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
                                'If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                                CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                                dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction_2 & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction_2)
                            Else
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction_2 & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                            End If
                            ' update user in invoices
                            dcl.ExecSQuery("UPDATE Stock_Trans_Invoices SET dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "', strUserName='" & strUserName & "' WHERE lngTransaction = " & lngTransaction_2)
                            ' update user logs
                            usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction_2, 2, "Get payment")
                        End If
                    End If
                Else
                    ' Get Payment Type (For Table [Stock_Trans])
                    Dim CC As String = ""
                    If PaymentType <> 1 Then CC = ",lngCRCard=330"
                    ' Get last invoice number
                    Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS LastNo FROM Stock_Trans WHERE Year(dateTransaction) = " & intYear & " AND byteBase = 40")
                    Dim strTransaction As String
                    If IsDBNull(dsLast.Tables(0).Rows(0).Item("LastNo")) Then
                        strTransaction = 1
                    Else
                        strTransaction = dsLast.Tables(0).Rows(0).Item("LastNo") + 1
                    End If
                    ' Get byteTransType
                    Dim dsTransType As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteBase = 40")
                    Dim byteTransType As Byte = dsTransType.Tables(0).Rows(0).Item("byteTransType")
                    ' Update transaction
                    dcl.ExecSQuery("UPDATE Stock_Trans SET strTransaction='" & strTransaction & "', byteBase=40, dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteTransType=" & byteTransType & ", dateClosedValid='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', byteStatus=1, bApproved1=1" & CC & " WHERE lngTransaction = " & lngTransaction)
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
                        'If IsDBNull(dsAudit.Tables(0).Rows(0).Item("strCashBy")) Then CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        CashSQL = ",strCashBy='" & strUserName & "', dateCash='" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "'"
                        dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET lngTransaction=" & lngTransaction & CreateSQL & LastSaveSQL & ApproveSQL & CashSQL & " WHERE lngTransaction = " & lngTransaction)
                    Else
                        dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved,strApprovedBy,dateApproved,strCashBy,dateCash) VALUES (" & lngTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd HH:mm:dd") & "')")
                    End If
                    ' update user in invoices
                    dcl.ExecSQuery("UPDATE Stock_Trans_Invoices SET dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:dd") & "', strUserName='" & strUserName & "' WHERE lngTransaction = " & lngTransaction)
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction, 2, "Get payment")
                End If

                '--------------------New Table---------------------------------
                dcl.ExecSQuery("DELETE FROM Stock_Trans_Payments WHERE lngTransaction=" & lngTransaction)
                dcl.ExecSQuery("DELETE FROM Stock_Trans_Payments WHERE lngTransaction=" & lngTransaction_2)
                If P_Cash + P_SPAN <> 0 Then
                    Dim Cash_Percent As Decimal
                    Dim SPAN_Percent As Decimal
                    If IsDBNull(ds.Tables(0).Rows(0).Item("curCash")) Then Amount = 0 Else Amount = ds.Tables(0).Rows(0).Item("curCash") + ds.Tables(0).Rows(0).Item("curCashVAT")
                    Select Case PaymentType
                        Case 1
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & Amount & ", 0)")
                        Case 2
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & Amount & ", 0)")
                        Case 3
                            If lngTransaction_2 = 0 Then
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & P_Cash & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & P_SPAN & ", 0)")
                            Else
                                Cash_Percent = Math.Round((P_Cash / (P_Cash + P_SPAN)) * 100, 2, MidpointRounding.AwayFromZero)
                                SPAN_Percent = Math.Round(100 - Cash_Percent, 2, MidpointRounding.AwayFromZero)
                                Dim Cash_Credit As Decimal = Math.Round((Amount * Cash_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                Dim SPAN_Credit As Decimal = Math.Round((Amount * SPAN_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 1, " & Cash_Credit & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction & ", 2, " & SPAN_Credit & ", 0)")
                            End If
                    End Select
                    ' Second Invoice (Cash)
                    If lngTransaction_2 > 0 Then
                        Amount = dsTemp.Tables(0).Rows(0).Item("curCash") + dsTemp.Tables(0).Rows(0).Item("curCashVAT")
                        Select Case PaymentType
                            Case 1
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 1, " & Amount & ", 0)")
                            Case 2
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 2, " & Amount & ", 0)")
                            Case 3
                                Dim Cash_Cash As Decimal = Math.Round((Amount * Cash_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                Dim SPAN_Cash As Decimal = Math.Round((Amount * SPAN_Percent) / 100, 2, MidpointRounding.AwayFromZero)
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 1, " & Cash_Cash & ", 0)")
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Payments VALUES (" & lngTransaction_2 & ", 2, " & SPAN_Cash & ", 0)")
                        End Select
                    End If
                End If
                '--------------------------------------------------------------

                Dim PrintScript As String = "$('#mdlAlpha').modal('hide');"
                Select Case PrintInvoice
                    Case 0 'No Print
                        ' Nothing
                    Case 1 'Auto Print
                        PrintScript = ""
                    Case 2 'Ask To Print
                        PrintScript = "$('#mdlAlpha').html('" & createPrintBox(lngTransaction, lngTransaction_2) & "');$('#mdlAlpha').modal('show');"
                    Case 3 'User Defined
                        '
                End Select
                Return PrintScript
            Catch ex As Exception
                Dim usr As New Share.User
                usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
                If lngTransaction_2 > 0 Then usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction_2, ErrorType, ex.Message)
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err: Transaction Lost"
        End If
    End Function

    Public Function ReturnToSales(ByVal lngTransaction As Long) As String
        Const ErrorType As Integer = 3

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        If lngTransaction > 0 Then
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = " & intYear & " AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction=" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                Try
                    Dim lngTranslink As Long
                    Dim dsTranslink As DataSet

                    dsTranslink = dcl.GetDS("SELECT * FROM Stock_Trans WHERE byteBase=50 AND bSubCash=1 AND dateTransaction='" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "' AND strReference='" & ds.Tables(0).Rows(0).Item("strReference").ToString & "'")
                    If dsTranslink.Tables(0).Rows.Count > 0 Then lngTranslink = dsTranslink.Tables(0).Rows(0).Item("lngTransaction") Else lngTranslink = 0
                    dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus = 1, bCollected1 = 1,bApproved1 = 0 WHERE lngTransaction=" & lngTransaction)
                    dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus = 1, bCollected1 = 1,bApproved1 = 0 WHERE lngTransaction=" & lngTranslink)
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction, 2, "Return to sales")
                Catch ex As Exception
                    Dim usr As New Share.User
                    usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
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

    Public Function SendToCashier(ByVal Fields As String, Optional ForPayment As Boolean = False) As String
        Const ErrorType As Integer = 4

        Dim ds As DataSet
        Dim body As New StringBuilder("")
        Dim lngTransaction, returnTransaction As Long
        Dim coveredCash, deductionCash, nonCoveredCash As Decimal
        Dim coveredVat, deductionVat, nonCoveredVat As Decimal
        Dim cashOnly As Boolean
        Dim script As String = ""

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
                Case "coveredVat"
                    coveredVat = field(I).value()
                Case "deductionVat"
                    deductionVat = field(I).value()
                Case "nonCoveredVat"
                    nonCoveredVat = field(I).value()
            End Select
        Next
        ' Collect items
        Dim Items_Insurance, Items_Cash As New List(Of InvoiceItem)
        Dim Total_Insurance, Total_Cash, Total_All As Decimal
        Dim Total_V_Insurance, Total_V_Cash, Total_V_All As Decimal
        If cashOnly = False Then
            Items_Insurance = getInvoiceItems(Fields, False)
            For Each item As InvoiceItem In Items_Insurance
                Total_Insurance = Total_Insurance + item.UnitPrice
                Total_V_Insurance = Total_V_Insurance + item.VAT
            Next
        End If
        Items_Cash = getInvoiceItems(Fields, True)
        For Each item As InvoiceItem In Items_Cash
            Total_Cash = Total_Cash + item.UnitPrice
            Total_V_Cash = Total_V_Cash + item.VAT
        Next
        Total_All = Math.Round(Total_Insurance + Total_Cash, byteCurrencyRound, MidpointRounding.AwayFromZero)
        Total_V_All = Math.Round(Total_V_Insurance + Total_V_Cash, byteCurrencyRound, MidpointRounding.AwayFromZero)

        'validateion
        If Items_Cash.Count + Items_Insurance.Count = 0 Then Return "Err:No items in this invoice!"
        'If Total_All <> (coveredCash + nonCoveredCash + deductionCash) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"
        'If Total_V_All <> (coveredVat + nonCoveredVat + deductionVat) Then Return "Err:Something happened when calculate the invoice, please remove items then add them again!"

        'send to cashier
        If lngTransaction > 0 Then
            ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, P.lngContract, P.byteScheme FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.lngTransaction = " & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                returnTransaction = lngTransaction
                Dim HasSubCash As Boolean = False
                'get Transaction info
                Dim PatientName As String = ds.Tables(0).Rows(0).Item("PatientName").ToString
                Dim InvoiceNo As String = ds.Tables(0).Rows(0).Item("ClinicInvoiceNo").ToString
                Dim DoctorNo As Long = ds.Tables(0).Rows(0).Item("DoctorNo").ToString
                Dim CompanyNo As Long = ds.Tables(0).Rows(0).Item("CompanyNo").ToString
                Dim DepartmentNo As Long = byteDepartment 'ds.Tables(0).Rows(0).Item("DepartmentNo").ToString '(should be 15 for pharmacy to correct posting)
                Dim PatientNo As Long = ds.Tables(0).Rows(0).Item("PatientNo").ToString
                Dim TransactionDate As Date = ds.Tables(0).Rows(0).Item("TransactionDate")
                Dim ContractNo As Long
                If IsDBNull(ds.Tables(0).Rows(0).Item("lngContract")) Then ContractNo = 0 Else ContractNo = ds.Tables(0).Rows(0).Item("lngContract")
                Dim SchemeNo As Byte
                If IsDBNull(ds.Tables(0).Rows(0).Item("byteScheme")) Then SchemeNo = 0 Else SchemeNo = ds.Tables(0).Rows(0).Item("byteScheme")

                'insurance
                If Items_Insurance.Count > 0 Then
                    Try
                        HasSubCash = True
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
                            'dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1," & item.VAT & ")")
                            '======> curDiscount = curBaseDiscount
                            dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.Discount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1," & item.VAT & ")")
                            intEntryNumber = intEntryNumber + 1
                        Next
                        '"INSERT INTO Stock_Trans_Insurance (lngTransaction, lngContact, lngContract, byteScheme, bytePrimaryDep, curCoverage, bPercentValue) VALUES (" & [lngTransaction] & ", " & rs!lngGuarantor & ", " & rs!lngContract & ", " & rs!byteScheme & ", " & Nz(rs!bytePrimaryDep, 1) & ", " & [sfrm1]![curCoverage] & "," & [sfrm1]![bPercentValue] & ")"
                        ' insert Insurance (Only for credit invoice)
                        Dim dsInsurance As DataSet
                        dsInsurance = dcl.GetDS("SELECT * FROM Stock_Trans_Insurance WHERE lngTransaction=" & lngTransaction)
                        If dsInsurance.Tables(0).Rows.Count > 0 Then
                            'no need to update
                        Else
                            Dim dsIns As DataSet
                            dsIns = dcl.GetDS("SELECT HP.bytePrimaryDep, HP.lngGuarantor, IC.lngContract, IC.byteScheme, IC.curDeductionValueP, IC.curDeductionPercentP, IC.curDeductionValueD, IC.curDeductionPercentD FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & PatientNo)
                            If dsIns.Tables(0).Rows.Count > 0 Then
                                Dim lngContract As Long = dsIns.Tables(0).Rows(0).Item("lngContract")
                                Dim byteScheme As Byte = dsIns.Tables(0).Rows(0).Item("byteScheme")
                                Dim bytePrimaryDep As Byte = dsIns.Tables(0).Rows(0).Item("bytePrimaryDep")
                                Dim curCoverage As Decimal
                                Dim bPercentValue As Integer
                                If bytePrimaryDep = 1 Then
                                    If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentP")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                                    bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                                Else
                                    If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentD")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                                    bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                                End If
                                dcl.ExecSQuery("INSERT INTO Stock_Trans_Insurance (lngTransaction, lngContact, lngContract, byteScheme, bytePrimaryDep, curCoverage, bPercentValue) VALUES (" & lngTransaction & "," & CompanyNo & "," & ContractNo & "," & SchemeNo & "," & bytePrimaryDep & "," & curCoverage & "," & bPercentValue & ")")
                            Else
                                Return "Err: Credit invoice,Complete insurance information"
                            End If
                        End If
                        '---------------New Table
                        ' insert Invoice
                        Dim dsInvoice As DataSet
                        dsInvoice = dcl.GetDS("SELECT * FROM Stock_Trans_Invoices WHERE lngTransaction=" & lngTransaction)
                        If dsInvoice.Tables(0).Rows.Count > 0 Then
                            dcl.ExecScalar("UPDATE Stock_Trans_Invoices SET curCredit=" & deductionCash & ", curCash=" & coveredCash & ",curCreditVAT=" & deductionVat & ", curCashVAT=" & coveredVat & ", dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', strUserName='" & strUserName & "' WHERE lngTransaction=" & lngTransaction)
                        Else
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Invoices VALUES (" & lngTransaction & "," & deductionCash & "," & coveredCash & "," & deductionVat & "," & coveredVat & ",1,'" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "','" & strUserName & "')")
                        End If
                        ' update user logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction, 2, "Close invoice")
                        '------------------------------------------------------------------------------
                        ' --- Important
                        ' --- if this [credit] invoice related to another cash invoice which becomes empty, then, must (clear items) and (clear amount) from that invoice because the next code [for cash invoice] will not execute
                        If cashOnly = False Then
                            Dim dsNextas As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans WHERE byteBase=50 AND byteStatus>0 AND bSubCash=1 AND lngPatient=" & PatientNo & " AND  dateTransaction='" & TransactionDate.ToString("yyyy-MM-dd") & "' AND strReference='" & InvoiceNo & "'")
                            If dsNextas.Tables(0).Rows.Count > 0 Then
                                Dim lngTransaction_Next As Long = dsNextas.Tables(0).Rows(0).Item("lngTransaction")
                                Dim dsXlinkNext = dcl.GetDS("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransaction_Next)
                                If dsXlinkNext.Tables(0).Rows.Count > 0 Then
                                    Dim lngXlinkNext As Long = dsXlinkNext.Tables(0).Rows(0).Item("lngXlink")
                                    Dim dsXlinkItemsNext = dcl.GetDS("SELECT * FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlinkNext)
                                    If dsXlinkItemsNext.Tables(0).Rows.Count > 0 And Items_Cash.Count = 0 Then
                                        dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlinkNext)
                                        dcl.ExecScalar("UPDATE Stock_Trans_Invoices SET curCredit=0, curCash=0,curCreditVAT=0, curCashVAT=0, dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', strUserName='" & strUserName & "' WHERE lngTransaction=" & lngTransaction_Next)
                                        script = "msg('','Cash invoice has been cleared!','info');"
                                    End If
                                End If
                            End If
                        End If
                        '------------------------------------------------------------------------------

                        returnTransaction = lngTransaction '<=====
                    Catch ex As Exception
                        Dim usr As New Share.User
                        usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
                        Return "Err:" & ex.Message
                    End Try
                End If
                'Cash
                If Items_Cash.Count > 0 Then
                    Try
                        Dim lngContact As Long = lngContact_Cash 'lngContact should be 27 and remove anything else
                        ' find any related transaction or insert a new transaction 
                        Dim dsTrans As DataSet
                        Dim lngTransaction_New As Long
                        If cashOnly = True Then
                            lngTransaction_New = lngTransaction
                            dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus=2, bCollected1=1, bApproved1=1, lngContact=" & lngContact & " WHERE lngTransaction=" & lngTransaction_New)
                        Else
                            dsTrans = dcl.GetDS("SELECT * FROM Stock_Trans WHERE byteBase=50 AND byteStatus>0 AND bSubCash=1 AND lngPatient=" & PatientNo & " AND  dateTransaction='" & TransactionDate.ToString("yyyy-MM-dd") & "' AND strReference='" & InvoiceNo & "'")
                            If dsTrans.Tables(0).Rows.Count > 0 Then
                                ' get transaction no
                                lngTransaction_New = dsTrans.Tables(0).Rows(0).Item("lngTransaction")
                                dcl.ExecScalar("UPDATE Stock_Trans SET byteStatus=2, bCollected1=1, bApproved1=1 WHERE lngTransaction=" & lngTransaction_New)
                            Else
                                ' get MAX strTransaction
                                Dim LastTrans As Long
                                ds = dcl.GetDS("SELECT Max(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteTransType=20 AND byteBase=40")
                                If IsDBNull(ds.Tables(0).Rows(0).Item("Last")) Then
                                    LastTrans = 1
                                Else
                                    LastTrans = ds.Tables(0).Rows(0).Item("Last") + 1
                                End If
                                ' insert a new transaction 
                                Dim bSubCash As String = "NULL"
                                If HasSubCash = True Then bSubCash = "1"
                                lngTransaction_New = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, bCash, bSubCash, dateEntry) VALUES (50, 20,'" & LastTrans & "', " & byteDepartment_Cash & " , " & lngContact_Cash & ", '" & TransactionDate.ToString("yyyy-MM-dd") & "', 2, " & byteLocalCurrency & ", " & lngSalesman_Cash & ", " & PatientNo & ", '" & Replace(PatientName, "'", "") & "','" & InvoiceNo & "', 1, 1, 1," & bSubCash & ",'" & Today.ToString("yyyy-MM-dd") & "')")
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
                            'dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1, " & item.VAT & ")")
                            '=== curDiscount = curBaseDiscount
                            dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1, " & item.VAT & ")")
                            intEntryNumber = intEntryNumber + 1
                        Next
                        '---------------New Table
                        ' insert Invoice
                        Dim dsInvoice As DataSet
                        dsInvoice = dcl.GetDS("SELECT * FROM Stock_Trans_Invoices WHERE lngTransaction=" & lngTransaction_New)
                        If dsInvoice.Tables(0).Rows.Count > 0 Then
                            dcl.ExecScalar("UPDATE Stock_Trans_Invoices SET curCredit=0, curCash=" & nonCoveredCash & ",curCreditVAT=0, curCashVAT=" & nonCoveredVat & ", dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', strUserName='" & strUserName & "' WHERE lngTransaction=" & lngTransaction_New)
                        Else
                            dcl.ExecSQuery("INSERT INTO Stock_Trans_Invoices VALUES (" & lngTransaction_New & ",0," & nonCoveredCash & ",0," & nonCoveredVat & ",1,'" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "','" & strUserName & "')")
                        End If
                        ' update user logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction_New, 2, "Close invoice")

                        If HasSubCash = False Then returnTransaction = lngTransaction_New '<=====
                    Catch ex As Exception
                        Dim usr As New Share.User
                        usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
                        Return "Err:" & ex.Message
                    End Try
                End If
            Else
                Return "Err:This record is unavailable, please refresh the orders again.."
            End If
        Else
            If Items_Cash.Count > 0 Then
                Try
                    Dim DepartmentNo As Byte = byteDepartment 'byteDepartment_Cash ====> (should be 15 for pharmacy to correct posting)
                    ' get MAX strTransaction
                    Dim LastTrans As Long
                    ds = dcl.GetDS("SELECT Max(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteTransType=20 AND byteBase=40")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("Last")) Then
                        LastTrans = 1
                    Else
                        LastTrans = ds.Tables(0).Rows(0).Item("Last") + 1
                    End If
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
                        'dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1, " & item.VAT & ")")
                        '=== curDiscount = curBaseDiscount
                        dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, curDiscount, curCoverage, curBaseDiscount, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1, bApproval, curVAT) VALUES(" & lngXlink & ", " & intEntryNumber & ", " & DepartmentNo & ", " & item.Service & ", '" & item.Item & "', " & item.Unit & ", 1, " & item.Quantity & ", '" & item.Expire.ToString("yyyy-MM-dd") & "', " & item.BasePrice & ", " & item.UnitPrice & ", NULL, " & item.Discount & ", " & item.Coverage & ", " & item.BaseDiscount & ", 0, " & item.Warehouse & ", '" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & item.Barcode & "','0000000000',1, " & item.VAT & ")")
                        intEntryNumber = intEntryNumber + 1
                    Next
                    '---------------New Table
                    ' insert Invoice
                    Dim dsInvoice As DataSet
                    dsInvoice = dcl.GetDS("SELECT * FROM Stock_Trans_Invoices WHERE lngTransaction=" & lngTransaction_New)
                    If dsInvoice.Tables(0).Rows.Count > 0 Then
                        dcl.ExecScalar("UPDATE Stock_Trans_Invoices SET curCredit=0, curCash=" & nonCoveredCash & ",curCreditVAT=0, curCashVAT=" & nonCoveredVat & ", dateTransaction='" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "', strUserName='" & strUserName & "' WHERE lngTransaction=" & lngTransaction_New)
                    Else
                        dcl.ExecSQuery("INSERT INTO Stock_Trans_Invoices VALUES (" & lngTransaction_New & ",0," & nonCoveredCash & ",0," & nonCoveredVat & ",1,'" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "','" & strUserName & "')")
                    End If
                    ' update user logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 1, "Cashier", lngTransaction_New, 2, "Close invoice")

                    returnTransaction = lngTransaction_New '<=====
                Catch ex As Exception
                    Dim usr As New Share.User
                    usr.AddError(strUserName, Now, 1, "Cashier", lngTransaction, ErrorType, ex.Message)
                    Return "Err:" & ex.Message
                End Try
            End If
        End If
        If ForPayment = True Then
            Return returnTransaction
        Else
            Return "<script type=""text/javascript"">msg('','Invoice has been sent to cashier!','notice');" & script & "$('#row" & lngTransaction & "').remove();</script>"
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

    Public Function viewCashbox() As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim btnClose As String
        Dim lblCash, lblCredit, lblTotalAmount, lblPaidCount, lblCancelledCount, lblReturnedCount As String
        Dim Header, cbStatus(2) As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "الصندوق"
                cbStatus(0) = "مفتوح"
                cbStatus(1) = "مغلق"
                lblCash = "نقدي"
                lblCredit = "بطاقة إئتمانية"
                lblTotalAmount = "الفواتير"
                lblPaidCount = "المدفوعة"
                lblCancelledCount = "الملغية"
                lblReturnedCount = "المرجتعة"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Cashbox"
                cbStatus(0) = "Opened"
                cbStatus(1) = "Closed"
                lblCash = "Cash"
                lblCredit = "Credit"
                lblTotalAmount = "Invoices"
                lblPaidCount = "Paid"
                lblCancelledCount = "Cancelled"
                lblReturnedCount = "Returned"
                btnClose = "Close"
        End Select

        Dim dateToday As Date = Today
        'Dim dateToday As Date = New Date(2019, 11, 1)
        'strUserName = "alih"
        Dim Where As String = "CONVERT(varchar(10), T.dateClosedValid, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' AND T.byteBase IN (40,18) AND TA.strCashBy='" & strUserName & "'"
        Dim Query As String = ""

        'Query = Query & "SELECT SUM(TI.curCash + TI.curCashVAT) AS curAmount, SUM(CASE WHEN TP.byteType = 1 THEN TP.curAmount ELSE 0 END) AS curCash, SUM(CASE WHEN TP.byteType = 2 THEN TP.curAmount ELSE 0 END) AS curCreditCard, SUM(TP.curAmount) AS curPayment FROM Stock_Trans AS T INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction=TA.lngTransaction INNER JOIN Stock_Trans_Invoices AS TI ON TI.lngTransaction=T.lngTransaction INNER JOIN Stock_Trans_Payments AS TP ON TP.lngTransaction=T.lngTransaction WHERE T.byteBase=40 AND T.byteStatus>0 AND CONVERT(varchar(10), T.dateClosedValid, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' AND TA.strCashBy='" & strUserName & "'"

        'Query = Query & "SELECT TA.strCashBy AS strUserName, CONVERT(varchar(10), T.dateClosedValid, 120) AS dateCashbox, SUM(CASE WHEN T.byteStatus>0 AND T.byteBase=40 THEN 1 ELSE 0 END) intInvoicesCount, SUM(CASE WHEN T.byteStatus=0 THEN 1 ELSE 0 END) AS intCancelledCount, SUM(CASE WHEN T.byteBase=18 THEN 1 ELSE 0 END) AS intReturnedCount, SUM(CASE WHEN T.byteStatus>0 THEN STI.curCash + STI.curCashVAT ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus>0 AND STP.byteType=1 THEN STP.curAmount ELSE 0 END) AS curCash, SUM(CASE WHEN T.byteStatus>0 AND STP.byteType=2 THEN STP.curAmount ELSE 0 END) AS curCreditCard, SUM(CASE WHEN T.byteStatus>0 THEN STP.curAmount ELSE 0 END) AS curTotalPaid"
        'Query = Query & " FROM Stock_Trans AS T INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON T.lngTransaction=STI.lngTransaction LEFT JOIN Stock_Trans_Payments AS STP ON T.lngTransaction=STP.lngTransaction"
        'Query = Query & " WHERE " & Where
        'Query = Query & " GROUP BY TA.strCashBy, CONVERT(varchar(10), T.dateClosedValid, 120)"

        'Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curNet money, curVAT money);"
        'Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)))) AS curNet, SUM(ISNULL(XI.curVAT, 0)) AS curVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        'Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        'Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT SX.lngTransaction, SX.lngXlink, SXV.curValue FROM Stock_Xlink_Values AS SXV INNER JOIN Stock_Xlink AS SX ON SXV.lngXlink = SX.lngXlink;"
        'Query = Query & "SELECT SUM(CASE WHEN STP.byteType=1 THEN STP.curAmount ELSE 0 END) AS curCash, SUM(CASE WHEN STP.byteType=2 THEN STP.curAmount ELSE 0 END) AS curCredit, SUM(STP.curAmount) AS curTotalMoney, COUNT(T.lngTransaction) AS curCountInvoices, SUM((IT.curNet + ISNULL(curValue,0)) + IT.curVAT) AS curTotalInvoices FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction LEFT JOIN Stock_Trans_Payments AS STP ON T.lngTransaction = STP.lngTransaction WHERE " & Where

        Query = Query & "DECLARE @Stock_Paid AS Table (strUserName varchar(max), dateCashbox date, intPaid int);"
        Query = Query & "INSERT INTO @Stock_Paid SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intPaid FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=40 AND T.byteStatus>0 AND CONVERT(varchar(10), TI.dateTransaction, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Cancelled AS Table (strUserName varchar(max), dateCashbox date, intCancelled int);"
        Query = Query & "INSERT INTO @Stock_Cancelled SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intCancelled FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=40 AND T.byteStatus=0 AND CONVERT(varchar(10), TI.dateTransaction, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Returned AS Table (strUserName varchar(max), dateCashbox date, intReturned int);"
        Query = Query & "INSERT INTO @Stock_Returned SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intCancelled FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=18 AND T.byteStatus>0 AND CONVERT(varchar(10), TI.dateTransaction, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Payment AS Table (lngTransaction int, curCash money, curCreditCard money);"
        Query = Query & "INSERT INTO @Stock_Payment SELECT lngTransaction, SUM(CASE WHEN byteType=1 THEN curAmount ELSE 0 END) AS curCash,SUM(CASE WHEN byteType=2 THEN curAmount ELSE 0 END) AS curCreditCard FROM Stock_Trans_Payments GROUP BY lngTransaction;"
        Query = Query & "SELECT ISNULL(P.intPaid, 0) AS intPaid, ISNULL(C.intCancelled, 0) AS intCancelled, ISNULL(R.intReturned, 0) AS intReturned, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateCashbox, TI.strUserName AS strUserName, SUM(TI.curCash + TI.curCashVAT) AS curAmount, SUM(TP.curCash) AS curCash, SUM(TP.curCreditCard) AS curCreditCard, SUM(TP.curCash + TP.curCreditCard) AS curPayment FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON TI.lngTransaction=T.lngTransaction INNER JOIN @Stock_Payment AS TP ON TP.lngTransaction=T.lngTransaction LEFT JOIN @Stock_Paid AS P ON CONVERT(varchar(10), P.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND P.strUserName=TI.strUserName LEFT JOIN @Stock_Returned AS R ON CONVERT(varchar(10), R.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND R.strUserName=TI.strUserName LEFT JOIN @Stock_Cancelled AS C ON CONVERT(varchar(10), C.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND C.strUserName=TI.strUserName WHERE T.byteBase IN (18,40) AND T.byteStatus>0 AND CONVERT(varchar(10), TI.dateTransaction, 120)='" & dateToday.ToString("yyyy-MM-dd") & "' AND TI.strUserName='" & strUserName & "' GROUP BY ISNULL(P.intPaid, 0), ISNULL(C.intCancelled, 0), ISNULL(R.intReturned, 0), CONVERT(varchar(10), TI.dateTransaction, 120), TI.strUserName order by TI.strUserName;"
        ds = dcl.GetDS(Query)

        Dim curCash, curCredit, curTotalAmount, intPaidCount, intCancelledCount, intReturnedCount As Decimal
        If ds.Tables(0).Rows.Count > 0 Then
            curCash = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("curCash").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curCredit = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("curCreditCard").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
            intPaidCount = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("intPaid").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
            intCancelledCount = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("intCancelled").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
            intReturnedCount = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("intReturned").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curTotalAmount = Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("curAmount").ToString), byteCurrencyRound, MidpointRounding.AwayFromZero)
        Else
            curCash = 0
            curCredit = 0
            intPaidCount = 0
            intCancelledCount = 0
            intReturnedCount = 0
            curTotalAmount = 0
        End If

        Dim Content As New StringBuilder("")
        Content.Append("<div class=""col-xl-12 col-lg-12 col-xs-12 mt-0 mb-0""><div class=""card""><div class=""card-body""><div class=""media""><div class=""p-2 text-xs-center bg-pink bg-darken-2 media-left media-middle""><i class=""icon-banknote font-large-2 white""></i></div><div class=""p-2 bg-pink white media-body""><h5 class=""col-md-6""><i class=""icon-bag2""></i> 0</h5><h5 class=""col-md-6""><i class=""icon-user1""></i> " & strUserName & "</h5><h5 class=""col-md-6""><i class=""icon-pencil""></i> " & cbStatus(0) & "</h5><h5 class=""col-md-6""><i class=""icon-calendar""></i> " & Now.ToString(strDateFormat & " " & strTimeFormat) & "</h5></div></div></div></div></div>")
        Content.Append("<div class=""col-xl-6 col-lg-6 col-xs-12 mt-0 mb-0""><div class=""card""><div class=""card-body""><div class=""card-block""><div class=""media""><div class=""media-left media-middle""><i class=""icon-money teal font-large-2 float-xs-left""></i></div><div class=""media-body text-xs-right""><h3>" & curCash & "</h3><span>" & lblCash & "</span></div></div></div></div></div></div>")
        Content.Append("<div class=""col-xl-6 col-lg-6 col-xs-12 mt-0 mb-0""><div class=""card""><div class=""card-body""><div class=""card-block""><div class=""media""><div class=""media-left media-middle""><i class=""icon-credit-card2 deep-orange font-large-2 float-xs-left""></i></div><div class=""media-body text-xs-right""><h3>" & curCredit & "</h3><span>" & lblCredit & "</span></div></div></div></div></div></div>")
        Content.Append("<div class=""col-xl-12 col-lg-6 col-xs-12 mt-0 mb-0""><div class=""card""><div class=""card-body""><div class=""card-block""><div class=""media""><div class=""media-left media-middle""><i class=""icon-diagram red accent-4 font-large-2 float-xs-left""></i></div><div class=""media-body text-xs-right""><div class=""col-md-3""><h3>" & curTotalAmount & "</h3><span>" & lblTotalAmount & "</span></div><div class=""col-md-3""><h3>" & intPaidCount & "</h3><span>" & lblPaidCount & "</span></div><div class=""col-md-3""><h3>" & intCancelledCount & "</h3><span>" & lblCancelledCount & "</span></div><div class=""col-md-3""><h3>" & intReturnedCount & "</h3><span>" & lblReturnedCount & "</span></div></div></div></div></div></div></div>")

        Dim mdl As New Share.UI
        Return mdl.drawModal(Header, Content.ToString, "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "bg-grey bg-lighten-2")
    End Function
End Class
