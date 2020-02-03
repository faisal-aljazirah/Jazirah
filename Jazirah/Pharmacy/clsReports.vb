Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.Web.Script.Serialization

Public Class Reports
    Dim dcl As New DCL.Conn.DataClassLayer
    Public byteLocalCurrency As Byte
    Public intStartupFY As Integer
    Public intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Public byteCurrencyRound As Byte

    Dim strUserName As String
    Dim byteLanguage As Byte
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
        byteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")

        loadSettings()

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

        If byteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""
    End Sub

#Region "Sales Reports"
    Public Function filterReport(ByVal Source As String, ByVal Filter As String) As String
        Dim lblReportType, lblDate, lblInvoiceNo, lblInvoiceStatus, lblPaymentType, lblDoctor, lblUser As String
        Dim Cash, Credit, Both, Paid, Posted, Cancelled, Returned, All, Totals, Detailed As String
        Dim btnSave, btnClose, btnApply, btnCancel As String
        Dim ReportTypeItem(6), ReportTypeList As String

        Select Case byteLanguage
            Case 2
                'Lables
                lblReportType = "نوع التقرير"
                lblDate = "التاريخ"
                lblInvoiceNo = "رقم الفاتورة"
                lblInvoiceStatus = "حالة الفاتورة"
                lblPaymentType = "نوع الدفع"
                lblDoctor = "الطبيب"
                lblUser = "المستخدم"
                'Buttons
                btnSave = "بدء الفرز"
                btnClose = "إغلاق"
                btnApply = "تطبيق"
                btnCancel = "تراجع"
                'Variables
                Cash = "نقدي"
                Credit = "آجل"
                Both = "كلاهما"
                Paid = "مدفوعة"
                Posted = "مرحلة"
                Cancelled = "ملغية"
                Returned = "مرتجعة"
                All = "الكل"
                Totals = "إجمالي"
                Detailed = "تفصيلي"
                'List
                ReportTypeItem(0) = "افتراضي"
                ReportTypeItem(1) = "حسب الطبيب"
                ReportTypeItem(2) = "حسب المستخدم"
                ReportTypeItem(3) = "حسب المستخدم(ملخص)"
                ReportTypeItem(4) = "الفواتير غير المطبوعة"
                ReportTypeItem(5) = "حسب الطبيب(ملخص)"
            Case Else
                'Lables
                lblReportType = "Report Type"
                lblDate = "Date"
                lblInvoiceNo = "Invoice No"
                lblInvoiceStatus = "Invoice Status"
                lblPaymentType = "Payment Type"
                lblDoctor = "Doctor"
                lblUser = "User"
                'Buttons
                btnSave = "Start Filter"
                btnClose = "Close"
                btnApply = "Apply"
                btnCancel = "Cancel"
                'Variables
                Cash = "Cash"
                Credit = "Credit"
                Both = "Both"
                Paid = "Paid"
                Posted = "Posted"
                Cancelled = "Cancelled"
                Returned = "Returned"
                All = "All"
                Totals = "Total"
                Detailed = "Detailed"
                'List
                ReportTypeItem(0) = "Default"
                ReportTypeItem(1) = "By Doctor"
                ReportTypeItem(2) = "By User"
                ReportTypeItem(3) = "By User (Summary)"
                ReportTypeItem(4) = "By Unprinted Invoices"
                ReportTypeItem(5) = "By Doctor (Summary)"
        End Select

        Dim fil() As String
        If Filter <> "" Then
            fil = Split(Filter, ",")
        Else
            fil = {1, Today.ToString("yyyy-MM-dd"), Today.ToString("yyyy-MM-dd"), "", "0", "1", "0", "0", "0"}
        End If
        Dim dsUsers, dsDoctors As DataSet
        Dim UsersList As String = "<option value="""">" & All & "</option>"
        Dim DoctorsList As String = "<option value="""">" & All & "</option>"
        Dim dcl As New DCL.Conn.DataClassLayer
        Dim selected As String = ""
        Dim checked As String = ""
        Dim cashChecked, creditChecked, bothChecked As String
        Dim cashActive, creditActive, bothActive As String
        Dim paidChecked, postedChecked, cancelledChecked, returnedChecked As String
        Dim paidActive, postedActive, cancelledActive, returnedActive As String
        Dim totalsChecked, detailedChecked As String
        Dim totalsActive, detailedActive As String

        If fil(5) = "1" Then
            cashChecked = "checked"
            cashActive = "active"
        Else
            cashChecked = ""
            cashActive = ""
        End If

        If fil(5) = "0" Then
            creditChecked = "checked"
            creditActive = "active"
        Else
            creditChecked = ""
            creditActive = ""
        End If

        If fil(5) = "2" Then
            bothChecked = "checked"
            bothActive = "active"
        Else
            bothChecked = ""
            bothActive = ""
        End If


        Dim st() As Char = fil(4).ToCharArray()
        If st(0) = "1" Then
            paidChecked = "checked"
            paidActive = "active"
        Else
            paidChecked = ""
            paidActive = ""
        End If

        If st(1) = "1" Then
            postedChecked = "checked"
            postedActive = "active"
        Else
            postedChecked = ""
            postedActive = ""
        End If

        If st(2) = "1" Then
            cancelledChecked = "checked"
            cancelledActive = "active"
        Else
            cancelledChecked = ""
            cancelledActive = ""
        End If

        If st(3) = "1" Then
            returnedChecked = "checked"
            returnedActive = "active"
        Else
            returnedChecked = ""
            returnedActive = ""
        End If

        dsUsers = dcl.GetDS("SELECT * FROM Cmn_Users")
        For I = 0 To dsUsers.Tables(0).Rows.Count - 1
            If dsUsers.Tables(0).Rows(I).Item("strUserName").ToString = fil(7).ToString Then selected = "selected=""selected""" Else selected = ""
            UsersList = UsersList & "<option value=""" & dsUsers.Tables(0).Rows(I).Item("strUserName") & """ " & selected & ">" & dsUsers.Tables(0).Rows(I).Item("strUserName") & "</option>"
        Next

        dsDoctors = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE byteClass=3")
        For I = 0 To dsDoctors.Tables(0).Rows.Count - 1
            If dsDoctors.Tables(0).Rows(I).Item("lngContact").ToString = fil(6).ToString Then selected = "selected=""selected""" Else selected = ""
            DoctorsList = DoctorsList & "<option value=""" & dsDoctors.Tables(0).Rows(I).Item("lngContact") & """ " & selected & ">" & dsDoctors.Tables(0).Rows(I).Item("strContact" & DataLang) & "</option>"
        Next

        ReportTypeList = ""
        For I = 0 To 5
            If fil(0) = I + 1 Then selected = "selected" Else selected = ""
            ReportTypeList = ReportTypeList & "<option value=""" & I + 1 & """ " & selected & ">" & ReportTypeItem(I) & "</option>"
        Next

        If fil(8) = "0" Then
            detailedChecked = "checked"
            detailedActive = "active"
        Else
            detailedChecked = ""
            detailedActive = ""
        End If

        If fil(8) = "1" Then
            totalsChecked = "checked"
            totalsActive = "active"
        Else
            totalsChecked = ""
            totalsActive = ""
        End If

        Dim JSFunc As String = ""
        Select Case Source
            Case "Sales"
                JSFunc = "fillSalesReport"
            Case "Cash"
                JSFunc = "fillCashReport"
        End Select

        Dim str As String = ""
        str = str & "<div class=""row"">"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblDate & ":</label></div><div class=""col-xs-9""><input id=""dtpPeriod"" value="""" type=""text"" class=""form-control form-control-sm input-sm date-formatter dir-ltr""><input type=""hidden"" id=""txtDateFrom"" value=""" & fil(1) & """ /><input type=""hidden"" id=""txtDateTo"" value=""" & fil(2) & """ /></div></div>"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblInvoiceNo & ":</label></div><div class=""col-xs-9""><input type=""text"" id=""txtInvoiceNo"" class=""form-control input-sm"" placeholder=""Search by Invoice Number  .."" value=""" & Replace(fil(3), "|", ",") & """ /></div></div>"
        If Source = "Sales" Then
            str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblInvoiceStatus & ":</label></div><div class=""col-xs-9""><div class=""btn-group btn-group-sm btn-group-toggle"" data-toggle=""buttons""><label class=""btn btn-sm btn-secondary " & paidActive & """><input type=""checkbox"" id=""chkPaid"" " & paidChecked & " autocomplete=""off"">" & Paid & "</label><label class=""btn btn-sm btn-secondary " & postedActive & """><input type=""checkbox"" id=""chkPosted"" " & postedChecked & " autocomplete=""off"">" & Posted & "</label><label class=""btn btn-sm btn-secondary " & cancelledActive & """><input type=""checkbox"" id=""chkCancelled"" " & cancelledChecked & " autocomplete=""off"">" & Cancelled & "</label><label class=""btn btn-sm btn-secondary " & returnedActive & """><input type=""checkbox"" id=""chkReturned"" " & returnedChecked & " autocomplete=""off"">" & Returned & "</label></div></div></div>"
        End If
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblPaymentType & ":</label></div><div class=""col-xs-9""><div class=""btn-group btn-group-sm btn-group-toggle"" data-toggle=""buttons""><label class=""btn btn-sm btn-secondary " & cashActive & """><input type=""radio"" name=""PaymentType"" value=""1"" " & cashChecked & ">" & Cash & "</label><label class=""btn btn-sm btn-secondary " & creditActive & """><input type=""radio"" name=""PaymentType"" value=""0"" " & creditChecked & ">" & Credit & "</label><label class=""btn btn-sm btn-secondary " & bothActive & """><input type=""radio"" name=""PaymentType"" value=""2"" " & bothChecked & ">" & Both & "</label></div></div></div>"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblDoctor & ":</label></div><div class=""col-xs-9""><select class=""form-control input-sm"" id=""drpDoctor"">" & DoctorsList & "</select></div></div>"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblUser & ":</label></div><div class=""col-xs-9""><select class=""form-control  input-sm"" id=""drpUser"">" & UsersList & "</select></div></div>"
        str = str & "<div class=""col-md-12 mb-1""><hr /></div>"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblReportType & ":</label></div><div class=""col-xs-9""><select id=""drpReportType"" class=""form-control input-sm"" disabled=""disabled"">" & ReportTypeList & "</select></div></div>"
        str = str & "<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label></label></div><div class=""col-xs-9""><div class=""btn-group btn-group-sm btn-group-toggle"" data-toggle=""buttons""><label class=""btn btn-sm btn-secondary " & detailedActive & """><input type=""radio"" name=""ReportView"" value=""0"" " & detailedChecked & ">" & Detailed & "</label><label class=""btn btn-sm btn-secondary " & totalsActive & """><input type=""radio"" name=""ReportView"" value=""1"" " & totalsChecked & ">" & Totals & "</label></div></div></div>"
        str = str & "</div>"

        str = str & "<script type=""text/javascript"">"
        'str = str & "var start = moment().subtract(29, 'days');var end = moment();function cb(start, end) {$('#daterange span').html(start.format('YYYY-MM-DD') + ' TO ' + end.format('YYYY-MM-DD'));}$('#daterange').daterangepicker({startDate: start,endDate: end}}, cb);cb(start, end);"
        str = str & "$('#dtpPeriod').daterangepicker({ startDate: '" & fil(1) & "', endDate: '" & fil(2) & "',locale: {format: 'YYYY-MM-DD', separator: separator, daysOfWeek: daysOfWeek, monthNames: monthNames, applyLabel: applyLabel, cancelLabel: cancelLabel, fromLabel: fromLabel, toLabel: toLabel} }, function(start, end, label) {$('#txtDateFrom').val(start.format('YYYY-MM-DD'));$('#txtDateTo').val(end.format('YYYY-MM-DD'));});"
        str = str & "datePatternNew = datePattern + separator + datePattern;$('.date-formatter').formatter({ pattern: datePatternNew });"
        'str = str & "$('#txtDoctorName').autocomplete({triggerSelectOnValidInput: true, onInvalidateSelection: function () {$('#txtDoctorName').val('');}, lookup: function (query, done) {if ($('#txtDoctorName').val().length > 0) {$.ajax({type: 'POST',url: '../CP/s_pharmacy.aspx/findContactDoct',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {$('#txtDoctorName').val(suggestion.value);$('#txtDoctorNo').val(suggestion.id);}});"
        'str = str & "function filterSales(){ var url='?f=1,' + $('#txtDateFrom').val() + ',' + $('#txtDateTo').val() + ',' + $('#txtInvoiceNo').val() + ',' + $('input[name=""InvoiceStatus""]:checked').val() + ',' + $('input[name=""PaymentType""]:checked').val() + ',' + $('#drpDoctor').val() + ',' + $('#drpUser').val(); window.location=url; }"
        str = str & "function filterSales(){ var status=''; if($('#chkPaid').prop('checked')) status+='1'; else status+='0'; if($('#chkPosted').prop('checked')) status+='1'; else status+='0'; if($('#chkCancelled').prop('checked')) status+='1'; else status+='0'; if($('#chkReturned').prop('checked')) status+='1'; else status+='0'; filter='1,' + $('#txtDateFrom').val() + ',' + $('#txtDateTo').val() + ',' + $('#txtInvoiceNo').val().replace(/,/g,'|') + ',' + status + ',' + $('input[name=""PaymentType""]:checked').val() + ',' + $('#drpDoctor').val() + ',' + $('#drpUser').val() + ',' + $('input[name=""ReportView""]:checked').val(); $('#mdlFilter').modal('hide');" & JSFunc & "(filter); }"
        'str = str & "function filterSales(){alert($('#chkPaid').prop('checked')+' '+$('#chkPosted').prop('checked')+' '+$('#chkCancelled').prop('checked')+' '+$('#chkReturned').prop('checked'))}"
        str = str & "</script>"

        Dim btns As String = "<button type=""button"" class=""btn btn-outline-primary"" onclick=""javascript:filterSales()"">" & btnSave & "</button> <button type=""button"" class=""btn grey btn-outline-secondary"" data-dismiss=""modal"">" & btnClose & "</button>"

        Dim sh As New Share.UI
        Return sh.drawModal("Filter", str, btns, Share.UI.ModalSize.Medium)
    End Function

    <Serializable> _
    Public Class ReportOption
        Public Property option_name() As String
            Get
                Return m_option_name
            End Get
            Set(value As String)
                m_option_name = value
            End Set
        End Property
        Private m_option_name As String
        Public Property option_value() As String
            Get
                Return m_option_value
            End Get
            Set(value As String)
                m_option_value = value
            End Set
        End Property
        Private m_option_value As String
    End Class

    Public Function getSalesReport(ByVal Report As String) As String
        Dim ReportName, CreateUser, CreateDate As String
        Dim Filter, Result As Dictionary(Of String, Object)
        Dim Columns() As Object
        'Filter = ""
        'Columns = ""
        'Result = ""
        Dim R_Select, R_From, R_Where, R_GroupBy, R_OrderBy As String

        Try
            Dim opt As Object = New JavaScriptSerializer().Deserialize(Of Object)(Report)
            ReportName = opt("reportname")
            CreateUser = opt("createuser")
            CreateDate = opt("createdate")
            Filter = opt("filter")
            Columns = opt("columns")
            Result = opt("result")
            'Dim x As String
            'If IsNothing(opt("x")) Then x = "" Else x = opt("x")
            'If opt("x") Is Nothing Then x = "" Else x = opt("x")
        Catch ex As Exception
            ' in case the structure of the object is not what we expected.
            Return "Err: Options is missing or currapted"
        End Try
        '' get form values
        'Dim jss As New JavaScriptSerializer()
        'Dim general() As ReportOption = jss.Deserialize(Of ReportOption())(Report)
        'For I = 0 To general.Length - 1
        '    Select Case general(I).option_name
        '        Case "reportname"
        '            ReportName = general(I).option_value
        '        Case "createuser"
        '            CreateUser = general(I).option_value
        '        Case "createdate"
        '            CreateDate = general(I).option_value
        '        Case "filter"
        '            Filter = general(I).option_value
        '        Case "columns"
        '            Columns = general(I).option_value
        '        Case "result"
        '            Result = general(I).option_value
        '    End Select
        'Next

        'Filtering
        Dim View As Integer
        If Result("viewing") Is Nothing Then View = 1 Else View = Result("viewing")
        R_Where = getWhere(Filter)
        R_Select = getSelect(Columns, Result("viewing"))
        R_From = getFrom(Columns)
        R_GroupBy = getGroupBy(Columns)
        R_OrderBy = getOrderBy(Result)
        Dim Query As String = ""
        'Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        'Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, XV.curValue FROM Stock_Xlink_Values AS XV INNER JOIN Stock_Xlink AS X ON XV.lngXlink = X.lngXlink;"
        'Query = Query & "INSERT INTO @Stock_Total SELECT T.lngTransaction, SUM(XI.curBasePrice), SUM((XI.curBasePrice * XI.curBaseDiscount) / 100), SUM(XI.curBasePrice - ((XI.curBasePrice * XI.curBaseDiscount) / 100)) FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON XI.lngXlink=X.lngXlink GROUP BY T.lngTransaction;"
        ''''''Query = Query & "INSERT INTO @Stock_Total SELECT T.lngTransaction, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))), SUM(ISNULL(((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) * XI.curBaseDiscount) / 100, 0)), SUM(ISNULL((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) - (((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) * XI.curBaseDiscount) / 100), -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0))))) FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit GROUP BY T.lngTransaction;"

        
        If View = 2 Then
            Query = Query & "DECLARE @Stock_Total as table(lngTransaction int, curGross money, curDiscount money, curNet money);"
            Query = Query & "INSERT INTO @Stock_Total SELECT T.lngTransaction, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))), SUM(ISNULL(((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) * XI.curDiscount) / 100, 0)), SUM(ISNULL((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) - (((-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) * XI.curDiscount) / 100), -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0))))) FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit GROUP BY T.lngTransaction;"

            Query = Query & "SELECT " & R_Select & " FROM " & R_From & " WHERE " & R_Where & " GROUP BY " & Mid(R_GroupBy, 2, Len(R_GroupBy)) & " ORDER BY " & R_OrderBy
        Else
            Query = Query & "SELECT " & R_Select & " FROM " & R_From & " WHERE " & R_Where & " ORDER BY " & R_OrderBy
        End If

        Return buildCurrentTable(Query, Result("grouping"))
    End Function

    Function getGroupBy(ByVal Columns As Object()) As String
        'For I = 0 To Result("grouping").Length - 1
        '    Select Case Result("grouping")(I)
        '        Case 12
        '            Dim str As String = "SELECT * FROM hw_Contact WHERE"
        '    End Select
        'Next
        Dim Col As String = ""
        If Columns.Length > 0 Then
            For Each s As Integer In Columns
                Select Case s
                    Case 1 'date
                        Col = Col & ",CONVERT(varchar(10), TI.dateTransaction, 120)"
                    Case 2 'time
                        Col = Col & ",CONVERT(varchar(10), TI.dateTransaction, 108)"
                    Case 3 'trans type id
                        Col = Col & ",T.byteTransType"
                    Case 4 'trans type name
                        Col = Col & ",TT.strType" & DataLang
                    Case 5 'status
                        Col = Col & ",CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END"
                    Case 6 'user
                        Col = Col & ",TI.strUserName"
                    Case 7 'patient id
                        Col = Col & ",T.lngPatient"
                    Case 8 'patient name
                        Col = Col & ",RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",'')))"
                    Case 9 'company id
                        Col = Col & ",T.lngContact"
                    Case 10 'company name
                        Col = Col & ",C.strContact" & DataLang
                    Case 11 'doctor id
                        Col = Col & ",T.lngSalesman"
                    Case 12 'doctor name
                        Col = Col & ",D.strContact" & DataLang
                    Case 13 'amount

                    Case 14 'vat

                    Case 15 'cash

                    Case 16 'cash vat

                    Case 17 'credit

                    Case 18 'credit vat

                    Case 19 'discount

                    Case 20 'type
                        Col = Col & ",T.bCash"
                    Case 21 'gross amount

                    Case 22 'discount amount

                    Case 23 'net amount

                End Select
            Next
        End If
        'T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr,
        'CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid
        'Query = Query & "SELECT T.lngTransaction, T.byteBase, T.byteStatus, T.strTransaction, T.dateTransaction, T.bCash, T.byteTransType, TT.strTypeAr, TT.strTypeEn, CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END AS [strStatus], T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curAmount, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curAmountVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END AS curCash, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END AS curCashVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curCredit, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, ISNULL(curValue,0) AS curDiscount, TI.strUserName AS strUserName FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN @Stock_Invoice_Discount AS ID ON ID.lngTransaction=T.lngTransaction WHERE " & Where
        Return Col
    End Function

    Function getOrderBy(ByVal Result As Dictionary(Of String, Object)) As String
        'If Filter("invoice")("datetype").ToString = "" Then DateType = 1 Else DateType = Filter("invoice")("datetype")
        Dim OrderBy As String = ""
        Dim sorting As Object()
        sorting = Result("sorting")
        For I = 0 To sorting.Length - 1
            Select Case Result("sorting")(I)("col")
                Case 0
                    OrderBy = ",strTransaction " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 1
                    OrderBy = ",CONVERT(varchar(10), TI.dateTransaction, 120) " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 2
                    OrderBy = ",CONVERT(varchar(10), TI.dateTransaction, 108) " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 3
                    OrderBy = ",T.byteTransType " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 4
                    OrderBy = ",TT.strType" & DataLang & " " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 5
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 6 'user
                    OrderBy = ",TI.strUserName " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 7 'patient id
                    OrderBy = ",T.lngPatient " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 8 'patient name
                    OrderBy = ",RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 9 'company id
                    OrderBy = ",T.lngContact " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 10 'company name
                    OrderBy = ",C.strContact" & DataLang & " " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 11 'doctor id
                    OrderBy = ",T.lngSalesman"
                Case 12 'doctor name
                    OrderBy = ",D.strContact" & DataLang & " " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 13 'amount
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 14 'vat
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 15 'cash
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 16 'cash vat
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 17 'credit
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 18 'credit vat
                    OrderBy = ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 19 'discount
                    OrderBy = ",ISNULL(XV.curValue,0) " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 20 'type
                    OrderBy = ",T.bCash " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 21 'gross amount
                    OrderBy = ",G.curGross " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 22 'discount amount
                    OrderBy = ",G.curDiscount " & Result("sorting")(I)("sort").ToString.ToUpper
                Case 23 'net amount
                    OrderBy = ",G.curNet " & Result("sorting")(I)("sort").ToString.ToUpper
            End Select
        Next
        Return Mid(OrderBy, 2, Len(OrderBy))
    End Function

    Function getFrom(ByVal Columns As Object()) As String
        Dim TransType As Boolean = False
        Dim Patient As Boolean = False
        Dim Company As Boolean = False
        Dim Doctor As Boolean = False
        Dim Discount As Boolean = False
        Dim Amount As Boolean = False
        Dim Db As String = " Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON TI.lngTransaction=T.lngTransaction "
        If Columns.Length > 0 Then
            For Each s As Integer In Columns
                If s = 4 Then TransType = True
                If s = 8 Then Patient = True
                If s = 10 Then Company = True
                If s = 12 Then Doctor = True
                If s = 19 Then Discount = True
                If (s = 21) Or (s = 22) Or (s = 23) Then Amount = True
            Next
            If TransType = True Then Db = Db & " INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType "
            If Patient = True Then Db = Db & " INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient "
            If Company = True Then Db = Db & " LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact "
            If Doctor = True Then Db = Db & " LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact "
            If Discount = True Then Db = Db & " INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction LEFT JOIN Stock_Xlink_Values AS XV ON XV.lngXlink=X.lngXlink "
            If Amount = True Then Db = Db & " INNER JOIN @Stock_Total AS G ON G.lngTransaction=T.lngTransaction "
        End If
        'Query = Query & "SELECT T.lngTransaction, T.byteBase, T.byteStatus, T.strTransaction, T.dateTransaction, T.bCash, T.byteTransType, TT.strTypeAr, TT.strTypeEn, CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END AS [strStatus], T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curAmount, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curAmountVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END AS curCash, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END AS curCashVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curCredit, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, ISNULL(curValue,0) AS curDiscount, TI.strUserName AS strUserName FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN @Stock_Invoice_Discount AS ID ON ID.lngTransaction=T.lngTransaction WHERE " & Where        Return Db
        Return Db
    End Function

    Function getSelect(ByVal Columns As Object(), ByVal View As Object) As String
        If View Is Nothing Then View = 1

        Dim Col As String
        If View = 2 Then
            Col = "COUNT(T.strTransaction) AS strTransaction"
        Else
            Col = "T.strTransaction"
        End If
        If Columns.Length > 0 Then
            For Each s As Integer In Columns
                Select Case s
                    Case 1 'date
                        Col = Col & ",CONVERT(varchar(10), TI.dateTransaction, 120) AS dateInvoice"
                    Case 2 'time
                        Col = Col & ",CONVERT(varchar(10), TI.dateTransaction, 108) AS timeInvoice"
                    Case 3 'trans type id
                        Col = Col & ",T.byteTransType"
                    Case 4 'trans type name
                        Col = Col & ",TT.strType" & DataLang
                    Case 5 'status
                        Col = Col & ",CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END AS [strStatus]"
                    Case 6 'user
                        Col = Col & ",TI.strUserName"
                    Case 7 'patient id
                        Col = Col & ",T.lngPatient"
                    Case 8 'patient name
                        Col = Col & ",RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS strPatientName"
                    Case 9 'company id
                        Col = Col & ",T.lngContact"
                    Case 10 'company name
                        Col = Col & ",C.strContact" & DataLang & " AS strCompanyName"
                    Case 11 'doctor id
                        Col = Col & ",T.lngSalesman"
                    Case 12 'doctor name
                        Col = Col & ",D.strContact" & DataLang & " AS strDoctorName"
                    Case 13 'amount
                        If View = 1 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END) AS curAmount"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curAmount"
                        End If
                    Case 14 'vat
                        If View = 2 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END) AS curAmountVAT"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curAmountVAT"
                        End If
                    Case 15 'cash
                        If View = 2 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END) AS curCash"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END AS curCash"
                        End If
                    Case 16 'cash vat
                        If View = 2 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END) AS curCashVAT"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END AS curCashVAT"
                        End If
                    Case 17 'credit
                        If View = 2 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END) AS curCredit"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curCredit"
                        End If
                    Case 18 'credit vat
                        If View = 2 Then
                            Col = Col & ",SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END) AS curCreditVAT"
                        Else
                            Col = Col & ",CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT"
                        End If
                    Case 19 'discount
                        If View = 2 Then
                            Col = Col & ",SUM(ISNULL(XV.curValue,0)) AS curFullDiscount"
                        Else
                            Col = Col & ",ISNULL(XV.curValue,0) AS curFullDiscount"
                        End If
                    Case 20 'type
                        Col = Col & ",T.bCash"
                    Case 21 'gross amount
                        If View = 2 Then
                            Col = Col & ",SUM(G.curGross) AS curGross"
                        Else
                            Col = Col & ",G.curGross"
                        End If
                    Case 22 'discount
                        If View = 2 Then
                            Col = Col & ",SUM(G.curDiscount) AS curDiscount"
                        Else
                            Col = Col & ",G.curDiscount"
                        End If
                    Case 23 'net amount
                        If View = 2 Then
                            Col = Col & ",SUM(G.curNet) AS curNet"
                        Else
                            Col = Col & ",G.curNet"
                        End If
                End Select
            Next
        End If
        'T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr,
        'CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid
        'Query = Query & "SELECT T.lngTransaction, T.byteBase, T.byteStatus, T.strTransaction, T.dateTransaction, T.bCash, T.byteTransType, TT.strTypeAr, TT.strTypeEn, CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END AS [strStatus], T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curAmount, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curAmountVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END AS curCash, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END AS curCashVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curCredit, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, ISNULL(curValue,0) AS curDiscount, TI.strUserName AS strUserName FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN @Stock_Invoice_Discount AS ID ON ID.lngTransaction=T.lngTransaction WHERE " & Where
        Return Col
    End Function

    Function getWhere(ByVal Filter As Dictionary(Of String, Object)) As String
        Dim DateType, NumberType, InvoiceType As Integer
        Dim Where, DateFrom, DateTo, NumberFactor, NumberValue As String
        Dim InvoiceStatus() As Object
        Dim CompanyId() As Object

        'invoice
        If Filter("invoice")("datetype").ToString = "" Then DateType = 1 Else DateType = Filter("invoice")("datetype")
        DateFrom = Filter("invoice")("datefrom")
        DateTo = Filter("invoice")("dateto")
        If Filter("invoice")("numbertype").ToString = "" Then NumberType = 0 Else NumberType = Filter("invoice")("numbertype")
        NumberFactor = Filter("invoice")("numberfactor")
        If Filter("invoice")("numbervalue").ToString = "" Then NumberValue = 0 Else NumberValue = Filter("invoice")("numbervalue")
        InvoiceType = Filter("invoice")("invoicetype")
        InvoiceStatus = Filter("invoice")("invoicestatus")
        CompanyId = Filter("company")("compnayid")

        Where = " T.lngTransaction <> 0"
        If DateType = 0 Then DateType = 1
        If DateFrom = "" Or Not (IsDate(DateFrom)) Then DateFrom = Today.ToString("yyyy-MM-dd")
        If DateTo = "" Or Not (IsDate(DateTo)) Then DateTo = Today.ToString("yyyy-MM-dd")
        Select Case DateType
            Case 1
                Where = Where & " AND CONVERT(varchar(10), TI.dateTransaction, 120) BETWEEN '" & DateFrom & "' AND '" & DateTo & "'"
            Case 2
                Where = Where & " AND CONVERT(varchar(10), TI.dateTransaction, 120) BETWEEN '" & Today.ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "'"
            Case 3
                'ask for it
        End Select

        If NumberType <> 0 Then
            Select Case NumberType
                Case 1
                    If NumberValue <> "" And NumberFactor <> "" Then Where = Where & " AND T.strTransaction " & NumberFactor & " " & NumberValue
                Case 2
                    If NumberValue <> "" Then Where = Where & " AND T.strTransaction IN (" & NumberValue & ")"
                Case 3
                    'ask for it
            End Select
        End If

        If InvoiceType <> 0 Then
            Select Case InvoiceType
                Case 1
                    ' both
                Case 2
                    Where = Where & " AND T.bCash = 0"
                Case 3
                    Where = Where & " AND T.bCash = 1"
                Case 4
                    'ask for it
            End Select
        End If

        If InvoiceStatus.Length > 0 Then
            Dim byteStatus As String = ""
            Dim byteBase1 As String = ""
            Dim byteBase2 As String = ""
            For Each s As Integer In InvoiceStatus
                Select Case s
                    Case 1
                        byteStatus = byteStatus & ",1"
                    Case 2
                        byteStatus = byteStatus & ",2"
                    Case 3
                        byteStatus = byteStatus & ",0"
                    Case 4
                        byteBase1 = "(T.byteBase = 18 AND T.byteStatus = 1)"
                End Select
            Next
            If byteStatus <> "" Then byteBase2 = "(T.byteBase = 40 AND T.byteStatus IN (" & Mid(byteStatus, 2, Len(byteStatus)) & "))"
            If byteBase1 <> "" And byteBase2 <> "" Then Where = Where & " AND (" & byteBase1 & " OR " & byteBase2 & ")" Else Where = Where & " AND (" & byteBase1 & byteBase2 & ")"
        End If

        If CompanyId.Length > 0 Then
            Dim companies As String = ""
            For Each id As Integer In CompanyId
                companies = companies & id & ","
            Next
            Where = Where & " AND T.lngContact IN (" & Mid(companies, 1, Len(companies) - 1) & ")"
            'If byteStatus <> "" Then byteBase2 = "(T.byteBase = 40 AND T.byteStatus IN (" & Mid(byteStatus, 2, Len(byteStatus)) & "))"
            'If byteBase1 <> "" And byteBase2 <> "" Then Where = Where & " AND (" & byteBase1 & " OR " & byteBase2 & ")" Else Where = Where & " AND (" & byteBase1 & byteBase2 & ")"
        End If

        Return Where
    End Function

    Public Function fillReportButtons() As String
        Dim Reportbuttons As String = "&nbsp;"
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/options.xml"))
        Dim nodes As XmlNodeList = doc.SelectNodes("Options/Reports[@user='" & strUserName & "' and @app=1]/Report")
        Dim counter As Integer = 1
        For Each node As XmlNode In nodes
            Dim name As String = node.Attributes("name").Value
            Dim script As String = node.InnerText
            Reportbuttons = Reportbuttons & "<script>var rep" & counter & "=" & script & ";</script>"
            Reportbuttons = Reportbuttons & "<button type=""button"" class=""btn btn-secondary btn-sm"" onclick=""javascript:generateReport('getSalesReport', " & script & ");"">" & name & "</button> "
            counter = counter + 1
        Next

        Return Reportbuttons
    End Function

    Public Function generateReport(ByVal Func As String, ByVal Options As String) As String
        Dim Header As String
        Dim btnSave, btnClose As String
        Dim lblExpiryDate, lblBasePrice, lblDate As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Header = "ادخل البيانات المطلوبة.."
                lblDate = "تاريخ الفواتير"
                lblExpiryDate = "تاريخ الإنتهاء"
                lblBasePrice = "سعر الصنف"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Enter required data.."
                lblDate = "Invoices Date"
                lblExpiryDate = "Expire Date"
                lblBasePrice = "Item Price"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        Dim Filter As Dictionary(Of String, Object)
        Dim opt As Object
        Try
            opt = New JavaScriptSerializer().Deserialize(Of Object)(Options)
            Filter = opt("filter")
        Catch ex As Exception
            Return "Err: Options is missing or currapted"
        End Try

        Dim content As New StringBuilder("")
        Dim script As String = ""
        Dim change As String = ""
        If Filter("invoice")("datetype").ToString = "" Or Filter("invoice")("datetype") = 3 Then
            content.Append("<div class=""col-md-12 mb-1""><div class=""col-xs-3 text-xs-right""><label>" & lblDate & ":</label></div><div class=""col-xs-9""><input id=""dtpInvoicesPeriod"" value="""" type=""text"" class=""form-control form-control-sm input-sm date-formatter dir-ltr""><input type=""hidden"" id=""txtInvoiceDateFrom"" value=""" & Today.ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""txtInvoiceDateTo"" value=""" & Today.ToString("yyyy-MM-dd") & """ /></div></div>")
            change = change & "report.filter.invoice.datetype=1;report.filter.invoice.datefrom=$('#txtInvoiceDateFrom').val();report.filter.invoice.dateto=$('#txtInvoiceDateTo').val();"
        End If

        script = script & "<script type=""text/javascript"">"
        script = script & "var report=" & Options & ";"
        script = script & "$('#dtpInvoicesPeriod').daterangepicker({ startDate: '" & Today.ToString("yyyy-MM-dd") & "', endDate: '" & Today.ToString("yyyy-MM-dd") & "',locale: {format: 'YYYY-MM-DD', separator: separator, daysOfWeek: daysOfWeek, monthNames: monthNames, applyLabel: applyLabel, cancelLabel: cancelLabel, fromLabel: fromLabel, toLabel: toLabel} }, function(start, end, label) {$('#txtInvoiceDateFrom').val(start.format('YYYY-MM-DD'));$('#txtInvoiceDateTo').val(end.format('YYYY-MM-DD'));});"
        script = script & "datePatternCurrent = datePattern + separator + datePattern;$('.date-formatter').formatter({ pattern: datePatternCurrent });"
        script = script & "function makeReport() {"
        script = script & change
        script = script & Func & "(report); $('#mdlMessage').modal('hide');"
        script = script & "}"
        script = script & "</script>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, "<form id=""frmOptions""><div class=""row"">" & content.ToString & "</div></form>" & script, "<div><button type=""button"" id=""btnSaveOptions"" class=""btn btn-success ml-1"" onclick=""javascript:makeReport();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.Medium)
    End Function

    Private Function buildCurrentTable(ByVal Query As String, ByVal GroupBy As Object()) As String
        Dim colInvoiceNo, colDate, colTime, colUser, colPatientNo, colPatientName, colCompanyNo, colCompany, colDoctorNo, colDoctor, colAmount, colAmountVAT, colFullDiscount, colCash, colCredit, colPayment, colVAT, colCashPayment, colCreditPayment, colStatus, colCashVAT, colCreditVAT, colType, colGross, colDiscount, colNet As String
        Dim divStatus As String
        Dim Paid, Posted, Cancelled, Returned As String
        Dim Cash, Credit As String
        Dim Total As String
        Dim totalInvoice, totalAmount, totalFullDiscount, totalVAT, totalCash, totalCredit, totalCashVAT, totalCreditVAT, totalGross, totalDiscount, totalNet As Decimal
        Dim totalSubInvoice, totalSubAmount, totalSubFullDiscount, totalSubVAT, totalSubCash, totalSubCredit, totalSubCashVAT, totalSubCreditVAT, totalSubGross, totalSubDiscount, totalSubNet As Decimal
        Dim btnPrint, btnExcel, btnSend As String

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Paid = "مدفوعة"
                Posted = "مرحلة"
                Cancelled = "ملغية"
                Returned = "مرتجعة"
                Cash = "نقدي"
                Credit = "آجل"
                Total = "المجموع"
                'Columns
                colInvoiceNo = "الرقم"
                colDate = "التاريخ"
                colTime = "الوقت"
                colUser = "المستخدم"
                colPatientNo = "الملف"
                colPatientName = "المريض"
                colCompanyNo = "رقم الشركة"
                colCompany = "الشركة"
                colDoctorNo = "رقم الطبيب"
                colDoctor = "الطبيب"
                colAmount = "المبلغ"
                colAmountVAT = "الضريبة"
                colFullDiscount = "الخصم"
                colVAT = "الضريبة"
                colCash = "النقد"
                colCredit = "الآجل"
                colPayment = "المدفوع"
                colCashPayment = "مدفوع النقدي"
                colCreditPayment = "مدفوع الآجل"
                colCashVAT = "ضريبة النقدي"
                colCreditVAT = "ضريبة الآجل"
                colStatus = "الحالة"
                colType = "النوع"
                colGross = "المبلغ"
                colDiscount = "الخصم"
                colNet = "الصافي"
                'Buttons
                btnPrint = "طباعة"
                btnExcel = "إكسل"
                btnSend = "ارسال"
            Case Else
                DataLang = "En"
                'Variables
                Paid = "Paid"
                Posted = "Posted"
                Cancelled = "Cancelled"
                Returned = "Returned"
                Cash = "Cash"
                Credit = "Credit"
                Total = "Total"
                'Columns
                colInvoiceNo = "No"
                colDate = "Date"
                colTime = "Time"
                colUser = "User"
                colPatientNo = "File"
                colPatientName = "Patient Name"
                colCompanyNo = "Company No"
                colCompany = "Company"
                colDoctorNo = "Doctor No"
                colDoctor = "Doctor"
                colAmount = "Amount"
                colAmountVAT = "VAT"
                colFullDiscount = "Discount"
                colVAT = "VAT"
                colCash = "Cash"
                colCredit = "Credit"
                colPayment = "Payment"
                colCashPayment = "Cash Payment"
                colCreditPayment = "Credit Payment"
                colCashVAT = "Cash VAT"
                colCreditVAT = "Credit VAT"
                colStatus = "Status"
                colType = "Type"
                colGross = "Gross"
                colDiscount = "Discount"
                colNet = "Net"
                'Buttons
                btnPrint = "Print"
                btnExcel = "Excel"
                btnSend = "Send"
        End Select

        Dim StartIndex As Integer = 0
        Dim GroupByField As String = ""
        Dim CurrentRecord As String = ""
        Dim NextGroup As Boolean = False
        ''
        If GroupBy.Length > 0 Then
            Select Case GroupBy(0)
                Case 1 'date
                    GroupByField = "CONVERT(varchar(10), TI.dateTransaction, 120)"
                    StartIndex = 1
                Case 2 'time
                    GroupByField = "CONVERT(varchar(10), TI.dateTransaction, 108)"
                    StartIndex = 1
                Case 3 'trans type id
                    GroupByField = "T.byteTransType,TT.strType" & DataLang
                    StartIndex = 2
                Case 4 'trans type name
                    GroupByField = "TT.strType" & DataLang
                    StartIndex = 1
                Case 5 'status
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END"
                    StartIndex = 1
                Case 6 'user
                    GroupByField = "TI.strUserName"
                    StartIndex = 1
                Case 7 'patient id
                    GroupByField = "T.lngPatient,RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",'')))"
                    StartIndex = 2
                Case 8 'patient name
                    GroupByField = "RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",'')))"
                    StartIndex = 1
                Case 9 'company id
                    GroupByField = "T.lngContact,C.strContact" & DataLang
                    StartIndex = 2
                Case 10 'company name
                    GroupByField = "C.strContact" & DataLang
                    StartIndex = 1
                Case 11 'doctor id
                    GroupByField = "T.lngSalesman,D.strContact" & DataLang
                    StartIndex = 2
                Case 12 'doctor name
                    GroupByField = "D.strContact" & DataLang
                    StartIndex = 1
                Case 13 'amount
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END"
                    StartIndex = 1
                Case 14 'vat
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END"
                    StartIndex = 1
                Case 15 'cash
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END"
                    StartIndex = 1
                Case 16 'cash vat
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END"
                    StartIndex = 1
                Case 17 'credit
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END"
                    StartIndex = 1
                Case 18 'credit vat
                    GroupByField = "CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END"
                    StartIndex = 1
                Case 19 'discount
                    GroupByField = "ISNULL(XV.curValue,0)"
                    StartIndex = 1
                Case 20 'type
                    GroupByField = "T.bCash"
                    StartIndex = 1
                Case 21 'gross amount
                    GroupByField = "G.curGross"
                    StartIndex = 1
                Case 22 'discount amount
                    GroupByField = "G.curDiscount"
                    StartIndex = 1
                Case 23 'net amount
                    GroupByField = "G.curNet"
                    StartIndex = 1
            End Select
            Query = Replace(Query, "SELECT ", "SELECT " & GroupByField & ",")
            Query = Replace(Query, "ORDER BY ", "ORDER BY " & GroupByField & ",")
        End If
        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        Dim GroupName As String
        ds = dcl.GetDS(Query)
        tbl.Append("<div class=""col-xs-12""><div class=""card p-1""><div class=""card-body""><div class=""row"">")
        tbl.Append("<div class=""col-xs-10"">...</div>")
        tbl.Append("<div class=""col-xs-2""><button type=""button"" class=""btn btn-sm btn-secondary"" onclick=""javascript:printReport();"">" & btnPrint & "</button> <button type=""button"" class=""btn btn-sm btn-secondary"" onclick=""javascript:exportReport();"">" & btnExcel & "</button> <button type=""button"" class=""btn btn-sm btn-secondary"" onclick=""javascript:testPrint();"" disabled=""disabled"">" & btnSend & "</button></div>")
        tbl.Append("</div></div></div></div>")
        tbl.Append("<div class=""col-xs-12""><div class=""card p-1""><div class=""card-body""><div class=""table-responsive""><table class=""table tableAjax table-bordered table-hover table-striped mb-0"" id=""tblReport"">")
        ''''''''''tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colAmount & "</th><th>" & colDiscount & "</th><th>" & colVAT & "</th><th>" & colCash & "</th><th>" & colCredit & "</th><th>" & colPayment & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        'tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colDoctorNo & "</th><th>" & colDoctor & "</th><th>" & colAmount & "</th><th>" & colAmountVAT & "</th><th>" & colDiscount & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        tbl.Append("<thead><tr class=""head"">")
        For C = StartIndex To ds.Tables(0).Columns.Count - 1
            Select Case ds.Tables(0).Columns(C).ColumnName
                Case "strTransaction"
                    tbl.Append("<th>" & colInvoiceNo & "</th>")
                Case "byteTransType"
                    tbl.Append("<th>byteTransType</th>")
                Case "strType" & DataLang
                    tbl.Append("<th>strType" & DataLang & "</th>")
                Case "dateInvoice"
                    tbl.Append("<th>" & colDate & "</th>")
                Case "timeInvoice"
                    tbl.Append("<th>" & colTime & "</th>")
                Case "strStatus"
                    tbl.Append("<th>" & colStatus & "</th>")
                Case "strUserName"
                    tbl.Append("<th>" & colUser & "</th>")
                Case "lngPatient"
                    tbl.Append("<th>" & colPatientNo & "</th>")
                Case "strPatientName"
                    tbl.Append("<th>" & colPatientName & "</th>")
                Case "lngContact"
                    tbl.Append("<th>" & colCompanyNo & "</th>")
                Case "strCompanyName"
                    tbl.Append("<th>" & colCompany & "</th>")
                Case "lngSalesman"
                    tbl.Append("<th>" & colDoctorNo & "</th>")
                Case "strDoctorName"
                    tbl.Append("<th>" & colDoctor & "</th>")
                Case "curAmount"
                    tbl.Append("<th>" & colAmount & "</th>")
                Case "curAmountVAT"
                    tbl.Append("<th>" & colAmountVAT & "</th>")
                Case "curCash"
                    tbl.Append("<th>" & colCash & "</th>")
                Case "curCashVAT"
                    tbl.Append("<th>" & colCashVAT & "</th>")
                Case "curCredit"
                    tbl.Append("<th>" & colCredit & "</th>")
                Case "curCreditVAT"
                    tbl.Append("<th>" & colCreditVAT & "</th>")
                Case "curFullDiscount"
                    tbl.Append("<th>" & colFullDiscount & "</th>")
                Case "bCash"
                    tbl.Append("<th>" & colType & "</th>")
                Case "curGross"
                    tbl.Append("<th>" & colGross & "</th>")
                Case "curDiscount"
                    tbl.Append("<th>" & colDiscount & "</th>")
                Case "curNet"
                    tbl.Append("<th>" & colNet & "</th>")
            End Select
        Next
        tbl.Append("</tr></thead>")
        ''

        tbl.Append("<tbody>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If GroupBy.Length > 0 Then
                If ds.Tables(0).Rows(I).Item(0).ToString <> CurrentRecord Then
                    If NextGroup = True Then
                        tbl.Append("<tr>")
                        For C = StartIndex To ds.Tables(0).Columns.Count - 1
                            Select Case ds.Tables(0).Columns(C).ColumnName
                                Case "strTransaction"
                                    tbl.Append("<th class=""subtotal"">" & totalSubInvoice & "</th>")
                                Case "curAmount"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curAmountVAT"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curCash"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curCashVAT"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curCredit"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curCreditVAT"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curFullDiscount"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubFullDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curGross"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubGross, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curDiscount"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case "curNet"
                                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubNet, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                                Case Else
                                    tbl.Append("<th></th>")
                            End Select
                        Next
                        totalSubInvoice = 0
                        totalSubAmount = 0
                        totalSubVAT = 0
                        totalSubCash = 0
                        totalSubCashVAT = 0
                        totalSubCredit = 0
                        totalSubCreditVAT = 0
                        totalSubFullDiscount = 0
                        totalSubGross = 0
                        totalSubDiscount = 0
                        totalSubNet = 0
                        tbl.Append("</tr>")
                    End If
                    CurrentRecord = ds.Tables(0).Rows(I).Item(0).ToString
                    If StartIndex = 2 Then GroupName = ds.Tables(0).Rows(I).Item(0).ToString & " - " & ds.Tables(0).Rows(I).Item(1).ToString Else GroupName = ds.Tables(0).Rows(I).Item(0).ToString
                    If NextGroup = False Then NextGroup = True
                    tbl.Append("<tr class=""group""><td colspan=""" & ds.Tables(0).Columns.Count - 1 & """><h3>" & GroupName & "</h3></td></tr>")
                End If
            End If
            tbl.Append("<tr>")
            totalSubInvoice = totalSubInvoice + 1
            totalInvoice = totalInvoice + 1
            For C = StartIndex To ds.Tables(0).Columns.Count - 1
                Select Case ds.Tables(0).Columns(C).ColumnName
                    Case "dateInvoice"
                        If IsDBNull(ds.Tables(0).Rows(I).Item(C)) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item(C)).ToString(strDateFormat) & "</td>")
                    Case "timeInvoice"
                        If IsDBNull(ds.Tables(0).Rows(I).Item(C)) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item(C)).ToString(strTimeFormat) & "</td>")
                    Case "curAmount"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubAmount = totalSubAmount + ds.Tables(0).Rows(I).Item(C)
                        totalAmount = totalAmount + ds.Tables(0).Rows(I).Item(C)
                    Case "curAmountVAT"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubVAT = totalSubVAT + ds.Tables(0).Rows(I).Item(C)
                        totalVAT = totalVAT + ds.Tables(0).Rows(I).Item(C)
                    Case "curCash"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubCash = totalSubCash + ds.Tables(0).Rows(I).Item(C)
                        totalCash = totalCash + ds.Tables(0).Rows(I).Item(C)
                    Case "curCashVAT"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubCashVAT = totalSubCashVAT + ds.Tables(0).Rows(I).Item(C)
                        totalCashVAT = totalCashVAT + ds.Tables(0).Rows(I).Item(C)
                    Case "curCredit"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubCredit = totalSubCredit + ds.Tables(0).Rows(I).Item(C)
                        totalCredit = totalCredit + ds.Tables(0).Rows(I).Item(C)
                    Case "curCreditVAT"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubCreditVAT = totalSubCreditVAT + ds.Tables(0).Rows(I).Item(C)
                        totalCreditVAT = totalCreditVAT + ds.Tables(0).Rows(I).Item(C)
                    Case "curFullDiscount"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubFullDiscount = totalSubFullDiscount + ds.Tables(0).Rows(I).Item(C)
                        totalFullDiscount = totalFullDiscount + ds.Tables(0).Rows(I).Item(C)
                    Case "bCash"
                        If ds.Tables(0).Rows(I).Item(C) = True Then tbl.Append("<td>" & Cash & "</td>") Else tbl.Append("<td>" & Credit & "</td>")
                    Case "curGross"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubGross = totalSubGross + ds.Tables(0).Rows(I).Item(C)
                        totalGross = totalGross + ds.Tables(0).Rows(I).Item(C)
                    Case "curDiscount"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubDiscount = totalSubDiscount + ds.Tables(0).Rows(I).Item(C)
                        totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item(C)
                    Case "curNet"
                        tbl.Append("<td><center>" & formatNumber(ds.Tables(0).Rows(I).Item(C)) & "</center></td>")
                        totalSubNet = totalSubNet + ds.Tables(0).Rows(I).Item(C)
                        totalNet = totalNet + ds.Tables(0).Rows(I).Item(C)
                    Case Else
                        tbl.Append("<td>" & ds.Tables(0).Rows(I).Item(C) & "</td>")
                End Select

            Next
            tbl.Append("</tr>")
        Next
        If GroupBy.Length > 0 Then
            tbl.Append("<tr>")
            For C = StartIndex To ds.Tables(0).Columns.Count - 1
                Select Case ds.Tables(0).Columns(C).ColumnName
                    Case "strTransaction"
                        tbl.Append("<th class=""subtotal"">" & totalSubInvoice & "</th>")
                    Case "curAmount"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curAmountVAT"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curCash"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curCashVAT"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curCredit"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curCreditVAT"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curFullDiscount"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubFullDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curGross"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubGross, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curDiscount"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case "curNet"
                        tbl.Append("<th class=""subtotal"">" & Math.Round(totalSubNet, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                    Case Else
                        tbl.Append("<th></th>")
                End Select
            Next
            tbl.Append("</tr>")
        End If

        tbl.Append("</tbody>")
        tbl.Append("<tr class=""head""><td colspan=""" & ds.Tables(0).Columns.Count - 1 & """><h2>" & Total & "</h2></td></tr>")
        tbl.Append("<tr>")
        For C = StartIndex To ds.Tables(0).Columns.Count - 1
            Select Case ds.Tables(0).Columns(C).ColumnName
                Case "strTransaction"
                    tbl.Append("<th class=""subtotal"">" & totalInvoice & "</th>")
                Case "curAmount"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curAmountVAT"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curCash"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curCashVAT"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curCredit"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curCreditVAT"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curFullDiscount"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalFullDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curGross"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalGross, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curDiscount"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case "curNet"
                    tbl.Append("<th class=""subtotal"">" & Math.Round(totalNet, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th>")
                Case Else
                    tbl.Append("<th></th>")
            End Select
        Next
        tbl.Append("</tr>")
        tbl.Append("</table></div></div></div></div>")

        Return tbl.ToString
    End Function

    Public Function fillSalesReport(ByVal Filter As String) As String
        Dim ReportType, ReportView As Byte
        Dim dateFrom, dateTo, invoiceNo, invoiceStatus, paymentType, doctor, user As String

        If Filter = "" Then
            ReportType = 1
            dateFrom = Today.ToString("yyyy-MM-dd")
            dateTo = Today.ToString("yyyy-MM-dd")
            invoiceNo = ""
            invoiceStatus = "1"
            paymentType = "2"
            doctor = ""
            user = ""
            ReportView = ""
        Else
            Dim param() As String = Split(Filter, ",")
            ReportType = param(0)
            dateFrom = param(1)
            dateTo = param(2)
            invoiceNo = param(3)
            invoiceStatus = param(4)
            paymentType = param(5)
            doctor = param(6)
            user = param(7)
            ReportView = param(8)
        End If

        Dim strFilter, divFilter As String
        strFilter = ReportType & "," & dateFrom & "," & dateTo & "," & invoiceNo & "," & invoiceStatus & "," & paymentType & "," & doctor & "," & user
        divFilter = getDescription(strFilter)

        Dim ds As DataSet
        Dim Query As String = ""
        'Dim Where As String = "(T.byteBase BETWEEN 40 AND 49) AND TT.lngContact=27 AND T.byteCurrency=3 AND T.dateTransaction BETWEEN '" & dateFrom & "' AND '" & dateTo & "'"
        Dim Where As String = "CONVERT(varchar(10), TI.dateTransaction, 120) BETWEEN '" & dateFrom & "' AND '" & dateTo & "'"
        'If invoiceNo <> "" Then Where = Where & " AND T.strTransaction = '" & invoiceNo & "'"
        If invoiceNo <> "" Then
            If invoiceNo.Contains("-") Then
                Dim temp() As String = Split(invoiceNo, "-")
                If IsNumeric(temp(0).Trim) And IsNumeric(temp(1).Trim) Then Where = Where & " AND T.strTransaction BETWEEN " & temp(0).Trim & " AND " & temp(1).Trim
            ElseIf invoiceNo.Contains("|") Then
                Dim temp() As String = Split(invoiceNo, "|")
                Dim list As String = ""
                For T = 0 To temp.Length - 1
                    If IsNumeric(temp(T).Trim) Then list = list & temp(T).Trim & ","
                Next
                If list <> "" Then
                    Where = Where & " AND T.strTransaction IN (" & Left(list, list.Length - 1) & ")"
                End If
            Else
                If IsNumeric(invoiceNo.Trim) Then Where = Where & " AND T.strTransaction = " & invoiceNo.Trim
            End If
        End If
        If (doctor <> "" And doctor <> "0") Then Where = Where & " AND T.lngSalesman=" & doctor
        If user <> "" Then Where = Where & " AND TI.strUserName='" & user & "'"
        If paymentType <> "2" Then Where = Where & " AND T.bCash = " & paymentType

        Dim st() As Char = invoiceStatus.ToCharArray
        Dim byteStatus As String = ""
        Dim byteBase As String = ""
        If st(3) = "1" Then byteBase = " OR (T.byteBase = 18 AND T.byteStatus = 1)"
        If st(0) = "1" Then byteStatus = byteStatus & ",1"
        If st(1) = "1" Then byteStatus = byteStatus & ",2"
        If st(2) = "1" Then byteStatus = byteStatus & ",0"
        If byteStatus <> "" Then byteBase = byteBase & " OR (T.byteBase = 40 AND T.byteStatus IN (" & Mid(byteStatus, 2, Len(byteStatus)) & "))"
        Where = Where & " AND (T.byteBase = 0" & byteBase & ")"

        Dim tbl As String = ""
        'ReportType = 2
        Select Case ReportType
            Case 1
                Query = buildQuery2(Where, ReportView)
                If ReportView = 1 Then tbl = buildTotalTable(Query, divFilter, ReportView) Else tbl = buildTable(Query, divFilter, ReportView)
            Case 2
                Query = buildQuery2(Where, ReportView)
                tbl = buildDoctorTable(Query, divFilter)
        End Select

        Return tbl
    End Function

    Public Function fillCashReport(ByVal Filter As String) As String
        Dim ReportType As Byte
        Dim dateFrom, dateTo, invoiceNo, invoiceStatus, paymentType, doctor, user As String

        If Filter = "" Then
            ReportType = 1
            dateFrom = Today.ToString("yyyy-MM-dd")
            dateTo = Today.ToString("yyyy-MM-dd")
            invoiceNo = ""
            invoiceStatus = "1"
            paymentType = "2"
            doctor = ""
            user = ""
        Else
            Dim param() As String = Split(Filter, ",")
            ReportType = param(0)
            dateFrom = param(1)
            dateTo = param(2)
            invoiceNo = param(3)
            invoiceStatus = param(4)
            paymentType = param(5)
            doctor = param(6)
            user = param(7)
        End If

        Dim strFilter, divFilter As String
        strFilter = ReportType & "," & dateFrom & "," & dateTo & "," & invoiceNo & "," & invoiceStatus & "," & paymentType & "," & doctor & "," & user
        divFilter = getDescription(strFilter)

        Dim ds As DataSet
        Dim Query As String = ""
        'Dim Where As String = "(T.byteBase BETWEEN 40 AND 49) AND TT.lngContact=27 AND T.byteCurrency=3 AND T.dateTransaction BETWEEN '" & dateFrom & "' AND '" & dateTo & "'"
        Dim Where As String = "CONVERT(varchar(10), TI.dateTransaction, 120) BETWEEN '" & dateFrom & "' AND '" & dateTo & "'"
        Dim Where1 As String = Where
        'If invoiceNo <> "" Then Where = Where & " AND T.strTransaction = '" & invoiceNo & "'"
        If invoiceNo <> "" Then
            If invoiceNo.Contains("-") Then
                Dim temp() As String = Split(invoiceNo, "-")
                If IsNumeric(temp(0).Trim) And IsNumeric(temp(1).Trim) Then Where = Where & " AND T.strTransaction BETWEEN " & temp(0).Trim & " AND " & temp(1).Trim
            ElseIf invoiceNo.Contains("|") Then
                Dim temp() As String = Split(invoiceNo, "|")
                Dim list As String = ""
                For T = 0 To temp.Length - 1
                    If IsNumeric(temp(T).Trim) Then list = list & temp(T).Trim & ","
                Next
                If list <> "" Then
                    Where = Where & " AND T.strTransaction IN (" & Left(list, list.Length - 1) & ")"
                End If
            Else
                If IsNumeric(invoiceNo.Trim) Then Where = Where & " AND T.strTransaction = " & invoiceNo.Trim
            End If
        End If
        If (doctor <> "" And doctor <> "0") Then Where = Where & " AND T.lngSalesman=" & doctor
        If user <> "" Then Where = Where & " AND TI.strUserName='" & user & "'"
        If paymentType <> "2" Then Where = Where & " AND T.bCash = " & paymentType

        'Dim st() As Char = invoiceStatus.ToCharArray
        'Dim byteStatus As String = ""
        'Dim byteBase As String = ""
        'If st(3) = "1" Then byteBase = " OR (T.byteBase = 18 AND T.byteStatus = 1)"
        'If st(0) = "1" Then byteStatus = byteStatus & ",1"
        'If st(1) = "1" Then byteStatus = byteStatus & ",2"
        'If st(2) = "1" Then byteStatus = byteStatus & ",0"
        'If byteStatus <> "" Then byteBase = byteBase & " OR (T.byteBase = 40 AND T.byteStatus IN (" & Mid(byteStatus, 2, Len(byteStatus)) & "))"
        'Where = Where & " AND (T.byteBase = 0" & byteBase & ")"

        Dim tbl As String = ""
        Select Case ReportType
            Case 1
                Query = buildQuery3(Where1, Where)
                tbl = buildCashTable(Query, divFilter)
        End Select

        Return tbl
    End Function

    Private Function buildTotalTable(ByVal Query As String, ByVal divFilter As String, ByVal ReportView As Byte) As String
        Dim colInvoiceNo, colDate, colTime, colUser, colPatientNo, colPatientName, colCompany, colDoctorNo, colDoctor, colAmount, colAmountVAT, colDiscount, colCash, colCredit, colPayment, colVAT, colCashPayment, colCreditPayment, colStatus, colCashVAT, colCreditVAT As String
        Dim divStatus As String
        Dim Paid, Posted, Cancelled, Returned As String
        Dim totalInvoice, totalAmount, totalDiscount, totalVAT, totalCash, totalCredit, totalPayment, totalCashPayment, totalCreditPayment, totalCashVAT, totalCreditVAT As Decimal

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Paid = "مدفوعة"
                Posted = "مرحلة"
                Cancelled = "ملغية"
                Returned = "مرتجعة"
                'Columns
                colInvoiceNo = "الرقم"
                colDate = "التاريخ"
                colTime = "الوقت"
                colUser = "المستخدم"
                colPatientNo = "الملف"
                colPatientName = "المريض"
                colCompany = "الشركة"
                colDoctorNo = "رقم الطبيب"
                colDoctor = "الطبيب"
                colAmount = "المبلغ"
                colAmountVAT = "الضريبة"
                colDiscount = "الخصم"
                colVAT = "الضريبة"
                colCash = "النقد"
                colCredit = "الآجل"
                colPayment = "المدفوع"
                colCashPayment = "مدفوع النقدي"
                colCreditPayment = "مدفوع الآجل"
                colCashVAT = "ضريبة النقدي"
                colCreditVAT = "ضريبة الآجل"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                'Variables
                Paid = "Paid"
                Posted = "Posted"
                Cancelled = "Cancelled"
                Returned = "Returned"
                'Columns
                colInvoiceNo = "No"
                colDate = "Date"
                colTime = "Time"
                colUser = "User"
                colPatientNo = "File"
                colPatientName = "Patient Name"
                colCompany = "Company"
                colDoctorNo = "Doctor No"
                colDoctor = "Doctor"
                colAmount = "Amount"
                colAmountVAT = "VAT"
                colDiscount = "Discount"
                colVAT = "VAT"
                colCash = "Cash"
                colCredit = "Credit"
                colPayment = "Payment"
                colCashPayment = "Cash Payment"
                colCreditPayment = "Credit Payment"
                colCashVAT = "Cash VAT"
                colCreditVAT = "Credit VAT"
        End Select

        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        ds = dcl.GetDS(Query)
        tbl.Append("<table class=""table tableAjax table-bordered table-hover table-striped mb-0"">")
        'tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colAmount & "</th><th>" & colDiscount & "</th><th>" & colVAT & "</th><th>" & colCash & "</th><th>" & colCredit & "</th><th>" & colPayment & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colAmount & "</th><th>" & colAmountVAT & "</th><th>" & colDiscount & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th></tr></thead><tbody>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            'If ds.Tables(0).Rows(I).Item("byteBase") = 18 Then
            '    divStatus = "<span class=""tag tag-sm tag-warning"">" & Returned & "</span>"
            'Else
            '    Select Case ds.Tables(0).Rows(I).Item("byteStatus")
            '        Case 0
            '            divStatus = "<span class=""tag tag-sm tag-danger"">" & Cancelled & "</span>"
            '        Case 1
            '            divStatus = "<span class=""tag tag-sm tag-success"">" & Paid & "</span>"
            '        Case 2
            '            divStatus = "<span class=""tag tag-sm tag-info"">" & Posted & "</span>"
            '        Case Else
            '            divStatus = ds.Tables(0).Rows(I).Item("byteStatus")
            '    End Select
            'End If

            tbl.Append("<tr>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("Count") & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmountVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curDiscount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCashVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCreditVAT")) & "</td>")
            tbl.Append("</tr>")

            'totalInvoice = totalInvoice + 1
            'totalAmount = totalAmount + ds.Tables(0).Rows(I).Item("curAmount")
            'totalVAT = totalVAT + ds.Tables(0).Rows(I).Item("curAmountVAT")
            'totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item("curDiscount")

            ''totalCash = totalCash + ds.Tables(0).Rows(I).Item("curCash")
            ''totalCredit = totalCredit + ds.Tables(0).Rows(I).Item("curCredit")
            ''totalPayment = totalPayment + ds.Tables(0).Rows(I).Item("curPayment")

            'totalCashPayment = totalCashPayment + ds.Tables(0).Rows(I).Item("curCash")
            'totalCreditPayment = totalCreditPayment + ds.Tables(0).Rows(I).Item("curCredit")
            'totalCashVAT = totalCashVAT + ds.Tables(0).Rows(I).Item("curCashVAT")
            'totalCreditVAT = totalCreditVAT + ds.Tables(0).Rows(I).Item("curCreditVAT")
        Next
        tbl.Append("</tbody>")
        'tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th></tr></tfoot>")
        tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th></tr></tfoot>")
        tbl.Append("</table>")

        tbl.Append("<script>")
        tbl.Append("def = [{ 'visible': false, 'targets': [" & HttpContext.Current.Session("r_sales_col") & "] }];")
        tbl.Append("var table = $('table.tableAjax').dataTable({language: tableLanguage,dom: dom,buttons: buttons,'columnDefs': def});")
        tbl.Append("$('#divFilter').html('" & divFilter & "');")
        tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {selectColumns('r_sales_col',column,state);});")
        'tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {alert(column);});")
        'tbl.Append("table.columns( [ 0, 1, 2, 3 ] ).visible( false );")
        'tbl.Append("table.columns.adjust().draw( false );")
        tbl.Append("</script>")

        Return tbl.ToString
    End Function

    Private Function buildTable(ByVal Query As String, ByVal divFilter As String, ByVal ReportView As Byte) As String
        Dim colInvoiceNo, colDate, colTime, colUser, colPatientNo, colPatientName, colCompany, colDoctorNo, colDoctor, colAmount, colAmountVAT, colDiscount, colCash, colCredit, colPayment, colVAT, colCashPayment, colCreditPayment, colStatus, colCashVAT, colCreditVAT As String
        Dim divStatus As String
        Dim Paid, Posted, Cancelled, Returned As String
        Dim totalInvoice, totalAmount, totalDiscount, totalVAT, totalCash, totalCredit, totalPayment, totalCashPayment, totalCreditPayment, totalCashVAT, totalCreditVAT As Decimal

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Paid = "مدفوعة"
                Posted = "مرحلة"
                Cancelled = "ملغية"
                Returned = "مرتجعة"
                'Columns
                colInvoiceNo = "الرقم"
                colDate = "التاريخ"
                colTime = "الوقت"
                colUser = "المستخدم"
                colPatientNo = "الملف"
                colPatientName = "المريض"
                colCompany = "الشركة"
                colDoctorNo = "رقم الطبيب"
                colDoctor = "الطبيب"
                colAmount = "المبلغ"
                colAmountVAT = "الضريبة"
                colDiscount = "الخصم"
                colVAT = "الضريبة"
                colCash = "النقد"
                colCredit = "الآجل"
                colPayment = "المدفوع"
                colCashPayment = "مدفوع النقدي"
                colCreditPayment = "مدفوع الآجل"
                colCashVAT = "ضريبة النقدي"
                colCreditVAT = "ضريبة الآجل"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                'Variables
                Paid = "Paid"
                Posted = "Posted"
                Cancelled = "Cancelled"
                Returned = "Returned"
                'Columns
                colInvoiceNo = "No"
                colDate = "Date"
                colTime = "Time"
                colUser = "User"
                colPatientNo = "File"
                colPatientName = "Patient Name"
                colCompany = "Company"
                colDoctorNo = "Doctor No"
                colDoctor = "Doctor"
                colAmount = "Amount"
                colAmountVAT = "VAT"
                colDiscount = "Discount"
                colVAT = "VAT"
                colCash = "Cash"
                colCredit = "Credit"
                colPayment = "Payment"
                colCashPayment = "Cash Payment"
                colCreditPayment = "Credit Payment"
                colCashVAT = "Cash VAT"
                colCreditVAT = "Credit VAT"
                colStatus = "Status"
        End Select

        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        ds = dcl.GetDS(Query)
        tbl.Append("<table class=""table tableAjax table-bordered table-hover table-striped mb-0"">")
        'tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colAmount & "</th><th>" & colDiscount & "</th><th>" & colVAT & "</th><th>" & colCash & "</th><th>" & colCredit & "</th><th>" & colPayment & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colDoctorNo & "</th><th>" & colDoctor & "</th><th>" & colAmount & "</th><th>" & colAmountVAT & "</th><th>" & colDiscount & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("byteBase") = 18 Then
                divStatus = "<span class=""tag tag-sm tag-warning"">" & Returned & "</span>"
            Else
                Select Case ds.Tables(0).Rows(I).Item("byteStatus")
                    Case 0
                        divStatus = "<span class=""tag tag-sm tag-danger"">" & Cancelled & "</span>"
                    Case 1
                        divStatus = "<span class=""tag tag-sm tag-success"">" & Paid & "</span>"
                    Case 2
                        divStatus = "<span class=""tag tag-sm tag-info"">" & Posted & "</span>"
                    Case Else
                        divStatus = ds.Tables(0).Rows(I).Item("byteStatus")
                End Select
            End If

            'If ds.Tables(0).Rows(I).Item("strTransaction").ToString = "171861" Then
            '    tbl.Append("<tr id=""grouped"">")
            '    tbl.Append("<td colspan=""100"">Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping Grouping </td><td class=""hidden"" id=""grouped""></td><td class=""hidden"" id=""grouped""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td><td class=""hidden""></td>")
            '    tbl.Append("</tr>")
            'Else
            tbl.Append("<tr>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
            If IsDBNull(ds.Tables(0).Rows(I).Item("dateClosedValid")) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateClosedValid")).ToString(strDateFormat) & "</td>")
            If IsDBNull(ds.Tables(0).Rows(I).Item("timeClosedValid")) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("timeClosedValid")).ToString(strTimeFormat) & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strUserName").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("lngPatient").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strPatientEn").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strContactEn").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("lngSalesman").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strDoctorEn").ToString & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmountVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curDiscount")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curPayment")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCashVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCreditVAT")) & "</td>")
            tbl.Append("<td>" & divStatus & "</td>")
            tbl.Append("</tr>")
            'End If

            totalInvoice = totalInvoice + 1
            totalAmount = totalAmount + ds.Tables(0).Rows(I).Item("curAmount")
            totalVAT = totalVAT + ds.Tables(0).Rows(I).Item("curAmountVAT")
            totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item("curDiscount")

            'totalCash = totalCash + ds.Tables(0).Rows(I).Item("curCash")
            'totalCredit = totalCredit + ds.Tables(0).Rows(I).Item("curCredit")
            'totalPayment = totalPayment + ds.Tables(0).Rows(I).Item("curPayment")

            totalCashPayment = totalCashPayment + ds.Tables(0).Rows(I).Item("curCash")
            totalCreditPayment = totalCreditPayment + ds.Tables(0).Rows(I).Item("curCredit")
            totalCashVAT = totalCashVAT + ds.Tables(0).Rows(I).Item("curCashVAT")
            totalCreditVAT = totalCreditVAT + ds.Tables(0).Rows(I).Item("curCreditVAT")
        Next
        tbl.Append("</tbody>")
        'tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th></tr></tfoot>")
        tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th></tr></tfoot>")
        tbl.Append("</table>")

        tbl.Append("<script>")
        tbl.Append("def = [{ 'visible': false, 'targets': [" & HttpContext.Current.Session("r_sales_col") & "] }];")
        tbl.Append("var table = $('table.tableAjax').dataTable({language: tableLanguage,dom: dom,buttons: buttons,'columnDefs': def});")
        tbl.Append("$('#divFilter').html('" & divFilter & "');")
        tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {selectColumns('r_sales_col',column,state);});")
        'tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {alert(column);});")
        'tbl.Append("table.columns( [ 0, 1, 2, 3 ] ).visible( false );")
        'tbl.Append("table.columns.adjust().draw( false );")
        tbl.Append("</script>")

        Return tbl.ToString
    End Function

    Private Function buildDoctorTable(ByVal Query As String, ByVal divFilter As String) As String
        Dim colInvoiceNo, colDate, colTime, colUser, colPatientNo, colPatientName, colCompany, colAmount, colDiscount, colCash, colCredit, colPayment, colVAT, colCashPayment, colCreditPayment, colStatus, colCashVAT, colCreditVAT As String
        Dim divStatus As String
        Dim Paid, Posted, Cancelled, Returned As String
        Dim totalInvoice, totalAmount, totalDiscount, totalVAT, totalCash, totalCredit, totalPayment, totalCashPayment, totalCreditPayment, totalCashVAT, totalCreditVAT As Decimal

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Paid = "مدفوعة"
                Posted = "مرحلة"
                Cancelled = "ملغية"
                Returned = "مرتجعة"
                'Columns
                colInvoiceNo = "الرقم"
                colDate = "التاريخ"
                colTime = "الوقت"
                colUser = "المستخدم"
                colPatientNo = "الملف"
                colPatientName = "المريض"
                colCompany = "الشركة"
                colAmount = "المبلغ"
                colDiscount = "الخصم"
                colVAT = "الضريبة"
                colCash = "النقد"
                colCredit = "الآجل"
                colPayment = "المدفوع"
                colCashPayment = "مدفوع النقدي"
                colCreditPayment = "مدفوع الآجل"
                colCashVAT = "ضريبة النقدي"
                colCreditVAT = "ضريبة الآجل"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                'Variables
                Paid = "Paid"
                Posted = "Posted"
                Cancelled = "Cancelled"
                Returned = "Returned"
                'Columns
                colInvoiceNo = "No"
                colDate = "Date"
                colTime = "Time"
                colUser = "User"
                colPatientNo = "File"
                colPatientName = "Patient Name"
                colCompany = "Company"
                colAmount = "Amount"
                colDiscount = "Discount"
                colVAT = "VAT"
                colCash = "Cash"
                colCredit = "Credit"
                colPayment = "Payment"
                colCashPayment = "Cash Payment"
                colCreditPayment = "Credit Payment"
                colCashVAT = "Cash VAT"
                colCreditVAT = "Credit VAT"
                colStatus = "Status"
        End Select

        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        ds = dcl.GetDS(Query & " ORDER BY T.lngSalesman")
        tbl.Append("<table class=""table tableAjax table-bordered table-hover table-striped mb-0"">")
        'tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colAmount & "</th><th>" & colDiscount & "</th><th>" & colVAT & "</th><th>" & colCash & "</th><th>" & colCredit & "</th><th>" & colPayment & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        tbl.Append("<thead><tr><th>" & colInvoiceNo & "</th><th>" & colDate & "</th><th>" & colTime & "</th><th>" & colUser & "</th><th>" & colPatientNo & "</th><th>" & colPatientName & "</th><th>" & colCompany & "</th><th>" & colDiscount & "</th><th>" & colCashPayment & "</th><th>" & colCreditPayment & "</th><th>" & colCashVAT & "</th><th>" & colCreditVAT & "</th><th>" & colStatus & "</th></tr></thead><tbody>")
        Dim currentDoctor As Long = 0
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("byteBase") = 18 Then
                divStatus = "<span class=""tag tag-sm tag-warning"">" & Returned & "</span>"
            Else
                Select Case ds.Tables(0).Rows(I).Item("byteStatus")
                    Case 0
                        divStatus = "<span class=""tag tag-sm tag-danger"">" & Cancelled & "</span>"
                    Case 1
                        divStatus = "<span class=""tag tag-sm tag-success"">" & Paid & "</span>"
                    Case 2
                        divStatus = "<span class=""tag tag-sm tag-info"">" & Posted & "</span>"
                    Case Else
                        divStatus = ds.Tables(0).Rows(I).Item("byteStatus")
                End Select
            End If

            If ds.Tables(0).Rows(I).Item("lngSalesman") <> currentDoctor Then
                tbl.Append("<tr>")
                tbl.Append("<th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th>")
                tbl.Append("</tr>")
                tbl.Append("<tr>")
                tbl.Append("<td colspan=""13""><b>" & ds.Tables(0).Rows(I).Item("strDoctorEn") & "</b></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>")
                tbl.Append("</tr>")
                currentDoctor = ds.Tables(0).Rows(I).Item("lngSalesman")
                totalInvoice = 0
                totalDiscount = 0
                totalCashPayment = 0
                totalCreditPayment = 0
                totalCashVAT = 0
                totalCreditVAT = 0
            End If

            tbl.Append("<tr>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
            If IsDBNull(ds.Tables(0).Rows(I).Item("dateClosedValid")) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateClosedValid")).ToString(strDateFormat) & "</td>")
            If IsDBNull(ds.Tables(0).Rows(I).Item("timeClosedValid")) Then tbl.Append("<td></td>") Else tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("timeClosedValid")).ToString(strTimeFormat) & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strUserName").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("lngPatient").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strPatientEn").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strContactEn").ToString & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curDiscount")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curVAT")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            'tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curPayment")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCashVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCreditVAT")) & "</td>")
            tbl.Append("<td>" & divStatus & "</td>")
            tbl.Append("</tr>")

            totalInvoice = totalInvoice + 1
            'totalAmount = totalAmount + ds.Tables(0).Rows(I).Item("curAmount")
            totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item("curDiscount")
            'totalVAT = totalVAT + ds.Tables(0).Rows(I).Item("curVAT")
            'totalCash = totalCash + ds.Tables(0).Rows(I).Item("curCash")
            'totalCredit = totalCredit + ds.Tables(0).Rows(I).Item("curCredit")
            'totalPayment = totalPayment + ds.Tables(0).Rows(I).Item("curPayment")

            totalCashPayment = totalCashPayment + ds.Tables(0).Rows(I).Item("curCash")
            totalCreditPayment = totalCreditPayment + ds.Tables(0).Rows(I).Item("curCredit")
            totalCashVAT = totalCashVAT + ds.Tables(0).Rows(I).Item("curCashVAT")
            totalCreditVAT = totalCreditVAT + ds.Tables(0).Rows(I).Item("curCreditVAT")
        Next
        tbl.Append("</tbody>")
        'tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCredit, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th></tr></tfoot>")
        tbl.Append("<tfoot><tr><th>" & totalInvoice & "</th><th></th><th></th><th></th><th></th><th></th><th></th><th>" & Math.Round(totalDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCashVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th></th></tr></tfoot>")
        tbl.Append("</table>")

        tbl.Append("<script>")
        tbl.Append("def = [{ 'visible': false, 'targets': [] }];")
        tbl.Append("var table = $('table.tableAjax').dataTable({language: tableLanguage,dom: dom,buttons: buttons,'columnDefs': def, searching: false, ordering: false, paging: false});")
        tbl.Append("$('#divFilter').html('" & divFilter & "');")
        tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {selectColumns('r_sales_col',column,state);});")
        'tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {alert(column);});")
        'tbl.Append("table.columns( [ 0, 1, 2, 3 ] ).visible( false );")
        'tbl.Append("table.columns.adjust().draw( false );")
        tbl.Append("</script>")

        Return tbl.ToString
    End Function

    Private Function buildCashTable(ByVal Query As String, ByVal divFilter As String) As String
        Dim colUser, colDate, colPaidCount, colCancelledCount, colReturnedCount, colAmount, colDiscount, colCash, colCreditCard, colPayment As String
        Dim totalRecord, totalUsers, totalPaid, totalCancelled, Totalreturned, totalAmount, totalCash, totalCreditCard, totalPayment As Decimal

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Columns
                colUser = "المستخدم"
                colDate = "التاريخ"
                colPaidCount = "المدفوعة"
                colCancelledCount = "الملغية"
                colReturnedCount = "المرتجعة"
                colAmount = "المبلغ"
                colDiscount = "الخصم"
                colCash = "النقدي"
                colCreditCard = "الإئتمان"
                colPayment = "المدفوع"
            Case Else
                DataLang = "En"
                'Columns
                colUser = "User"
                colDate = "Date"
                colPaidCount = "Paid"
                colCancelledCount = "Cancelled"
                colReturnedCount = "Returned"
                colAmount = "Amount"
                colDiscount = "Discount"
                colCash = "Cash"
                colCreditCard = "Credit Card"
                colPayment = "Payment"
        End Select

        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        ds = dcl.GetDS(Query)
        tbl.Append("<table class=""table tableAjax table-bordered table-hover table-striped mb-0"">")
        tbl.Append("<thead><tr><th>" & colDate & "</th><th>" & colUser & "</th><th>" & colPaidCount & "</th><th>" & colCancelledCount & "</th><th>" & colReturnedCount & "</th><th>" & colAmount & "</th><th>" & colCash & "</th><th>" & colCreditCard & "</th><th>" & colPayment & "</th></tr></thead><tbody>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            tbl.Append("<tr>")
            tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateCashbox")).ToString(strDateFormat) & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strUserName") & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("intPaid").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("intCancelled").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("intReturned").ToString & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCreditCard")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curPayment")) & "</td>")
            tbl.Append("</tr>")

            totalRecord = totalRecord + 1
            totalPaid = totalPaid + ds.Tables(0).Rows(I).Item("intPaid")
            totalCancelled = totalCancelled + ds.Tables(0).Rows(I).Item("intCancelled")
            Totalreturned = Totalreturned + ds.Tables(0).Rows(I).Item("intReturned")
            totalAmount = totalAmount + ds.Tables(0).Rows(I).Item("curAmount")
            'totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item("curDiscount")
            totalCash = totalCash + ds.Tables(0).Rows(I).Item("curCash")
            totalCreditCard = totalCreditCard + ds.Tables(0).Rows(I).Item("curCreditCard")
            totalPayment = totalPayment + ds.Tables(0).Rows(I).Item("curPayment")
        Next
        tbl.Append("</tbody>")
        tbl.Append("<tfoot><tr><th></th><th>" & totalRecord & "</th><th>" & totalPaid & "</th><th>" & totalCancelled & "</th><th>" & Totalreturned & "</th><th>" & Math.Round(totalAmount, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCash, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalCreditCard, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th><th>" & Math.Round(totalPayment, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</th></tr></tfoot>")
        tbl.Append("</table>")

        tbl.Append("<script>")
        tbl.Append("def = [{ 'visible': false, 'targets': [" & HttpContext.Current.Session("r_cash_col") & "] }];")
        tbl.Append("var table = $('table.tableAjax').dataTable({language: tableLanguage,dom: dom,buttons: buttons,'columnDefs': def});")
        tbl.Append("$('#divFilter').html('" & divFilter & "');")
        tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {selectColumns('r_cash_col',column,state);});")
        'tbl.Append("table.on('column-visibility.dt', function (e, settings, column, state) {alert(column);});")
        'tbl.Append("table.columns( [ 0, 1, 2, 3 ] ).visible( false );")
        'tbl.Append("table.columns.adjust().draw( false );")
        tbl.Append("</script>")

        Return tbl.ToString
    End Function

    Private Function getDescription(ByVal strFilter As String) As String
        Dim lblReportType, lblDateFrom, lblDateTo, lblInvoiceNo, lblInvoiceStatus, lblPaymentType, lblDoctor, lblUser As String
        Dim ReportTypeItem(6), InvoiceStatus(5), PaymentType(3) As String

        Select Case byteLanguage
            Case 2
                'Lables
                lblReportType = "نوع التقرير"
                lblDateFrom = "من تاريخ"
                lblDateTo = "إلى تاريخ"
                lblInvoiceNo = "رقم الفاتورة"
                lblInvoiceStatus = "حالة الفاتورة"
                lblPaymentType = "نوع الدفع"
                lblDoctor = "الطبيب"
                lblUser = "المستخدم"
                'List
                PaymentType(1) = "نقدي"
                PaymentType(0) = "آجل"
                PaymentType(2) = "كلاهما"
                InvoiceStatus(1) = "مدفوعة"
                InvoiceStatus(2) = "غير مدفوعة"
                InvoiceStatus(0) = "ملغية"
                InvoiceStatus(3) = "غير ملغية"
                InvoiceStatus(4) = "الكل"
                ReportTypeItem(0) = "حسب نوع الفاتورة"
                ReportTypeItem(1) = "حسب الطبيب"
                ReportTypeItem(2) = "حسب المستخدم"
                ReportTypeItem(3) = "حسب المستخدم(ملخص)"
                ReportTypeItem(4) = "الفواتير غير المطبوعة"
                ReportTypeItem(5) = "حسب الطبيب(ملخص)"
            Case Else
                'Lables
                lblReportType = "Report Type"
                lblDateFrom = "From Date"
                lblDateTo = "To Date"
                lblInvoiceNo = "Invoice No"
                lblInvoiceStatus = "Invoice Status"
                lblPaymentType = "Payment Type"
                lblDoctor = "Doctor"
                lblUser = "User"
                'List
                PaymentType(1) = "Cash"
                PaymentType(0) = "Credit"
                PaymentType(2) = "Both"
                InvoiceStatus(1) = "Paid"
                InvoiceStatus(2) = "Unpaid"
                InvoiceStatus(0) = "Cancelled"
                InvoiceStatus(3) = "Not Cancelled"
                InvoiceStatus(4) = "All"
                ReportTypeItem(0) = "By Invoice Type"
                ReportTypeItem(1) = "By Doctor"
                ReportTypeItem(2) = "By User"
                ReportTypeItem(3) = "By User (Summary)"
                ReportTypeItem(4) = "By Unprinted Invoices"
                ReportTypeItem(5) = "By Doctor (Summary)"
        End Select

        Dim filter() As String = Split(strFilter, ",")
        If filter.Length > 0 Then
            Dim str As String = ""
            str = str & "<div class=""col-md-12""><div class=""col-md-6""><h3><b>" & lblReportType & ":</b> " & ReportTypeItem(filter(0) - 1) & "</h3></div>"
            str = str & "<div class=""col-md-6""><b>" & lblDateFrom & ":</b><span class=""red""> " & CDate(filter(1)).ToString(strDateFormat) & " </span><b>" & lblDateTo & ":</b><span class=""red""> " & CDate(filter(2)).ToString(strDateFormat) & "</span></div></div>"
            str = str & "<div class=""col-md-12""><div class=""col-md-2"">"
            str = str & "<b>" & lblPaymentType & ":</b> " & PaymentType(filter(5))
            str = str & "</div>"
            str = str & "<div class=""col-md-2"">"
            'str = str & "<b>" & lblInvoiceStatus & ":</b> " & InvoiceStatus(filter(4))
            str = str & "</div>"
            If filter(3) <> "" Then str = str & "<div class=""col-md-2""><b> " & lblInvoiceNo & ":</b> " & Replace(filter(3), "|", ",") & "</div>"
            If filter(6) <> "" Then str = str & "<div class=""col-md-2""><b>" & lblDoctor & ":</b> " & getDoctorName(filter(6)) & "</div>"
            If filter(7) <> "" Then str = str & "<div class=""col-md-2""><b>" & lblUser & ":</b> " & filter(7) & "</div>"
            str = str & "</div>"
            Return str
        Else
            Return ""
        End If
    End Function

    Private Function getDoctorName(ByVal lngContact As Long) As String
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact=" & lngContact)
        If ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString
        Else
            Return ""
        End If
    End Function

    Private Function formatNumber(ByVal Number As Decimal) As String
        Number = Math.Round(Number, byteCurrencyRound, MidpointRounding.AwayFromZero)
        If Number > 0 Then Return "<b>" & Number & "</b>" Else If Number < 0 Then Return "<b class=""red"">" & Number & "</b>" Else Return Number
    End Function

    Private Function buildQuery1(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Total AS TABLE (lngTransaction int, lngXlink int, TotalCost money, TotalNet money, TotalQuantity money, CashVAT money, CreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Total SELECT T.lngTransaction, XI.lngXlink, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS TotalCost, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS TotalNet, SUM(B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS TotalQuantity, SUM(ISNULL(XI.curVAT, 0)) AS CashVAT, SUM(ISNULL(XI.curVATI, 0)) AS CreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_OutPatPayments_Query AS TABLE (lngTransaction int, Payment money);"
        Query = Query & "INSERT INTO @Stock_OutPatPayments_Query SELECT TA.lngTransaction, SUM(ISNULL(curValue, 0)) AS Payment FROM Stock_Trans_Amounts AS TA INNER JOIN Stock_Trans AS T ON TA.lngTransaction = T.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType WHERE TA.byteAmountType=4 AND " & Where & " GROUP BY TA.lngTransaction;"
        Query = Query & "DECLARE @Stock_Values AS TABLE (lngXlink int, byteValueType int, strValueEn varchar(max), strValueAr varchar(max), curValue money, bPercentValue bit, intValueSign int, bytePercentCalculation int, byteAllocationBase int);"
        Query = Query & "INSERT INTO @Stock_Values SELECT Stock_Xlink_Values.lngXlink, Stock_Xlink_Values.byteValueType, Stock_Value_Types.strValueEn, Stock_Value_Types.strValueAr, Stock_Xlink_Values.curValue, Stock_Xlink_Values.bPercentValue, Stock_Value_Types.intValueSign, Stock_Value_Types.bytePercentCalculation, Stock_Value_Types.byteAllocationBase FROM Stock_Xlink_Values INNER JOIN Stock_Value_Types ON Stock_Xlink_Values.byteValueType = Stock_Value_Types.byteValueType;"
        Query = Query & "DECLARE @Stock_Discount AS TABLE (lngTransaction int, lngXlink int, TotalCost money, TotalQuantity money, Discount money, TotalNet money);"
        Query = Query & "INSERT INTO @Stock_Discount SELECT T.lngTransaction, T.lngXlink, T.TotalCost, T.TotalQuantity, SUM(T.TotalCost - CASE WHEN V.byteValueType=1 Then CASE WHEN V.bPercentValue = 1 THEN ISNULL(V.curValue,0) * (T.TotalCost/100) ELSE ISNULL(V.curValue,0) END ELSE 0 END) AS Discount, T.TotalNet FROM @Stock_Total AS T LEFT JOIN @Stock_Values AS V ON T.lngXlink = V.lngXlink GROUP BY T.lngTransaction, T.lngXlink, T.TotalCost, T.TotalQuantity, T.TotalNet;"
        Query = Query & "SELECT T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, ABS(Q.TotalCost) AS Total, ABS((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet)) AS curDisc, CASE WHEN T.bCash=1 THEN ABS(Q.TotalCost + ((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet))) ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN ABS(Q.TotalCost - ((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet))) ELSE 0 END AS curOutStanding, IsNull(Q1.Payment, 0) AS Payment, T.dateEntry, TA.strCreatedBy FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN @Stock_Discount AS Q ON T.lngTransaction = Q.lngTransaction INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN @Stock_OutPatPayments_Query AS Q1 ON T.lngTransaction = Q1.lngTransaction LEFT JOIN Lab_Order_Header AS O ON T.lngTransaction = O.lngTransaction LEFT JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " ORDER BY T.strTransaction;"
        Return Query
    End Function

    Private Function buildQuery2_old(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS curAmount, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS curNet, SUM(B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS curQuantity, SUM(B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)) AS curPayment, SUM(ISNULL(XI.curVAT, 0)) AS curCashVAT, SUM(ISNULL(XI.curVATI, 0)) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, hhh.curValue FROM Stock_Xlink_Values AS hhh INNER JOIN Stock_Xlink AS X ON hhh.lngXlink = X.lngXlink;"
        Query = Query & "SELECT T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, ABS(IT.curAmount) AS curAmount, ABS(IT.curAmount-(IT.curNet + ISNULL(curValue,0))) AS curDiscount, IT.curCashVAT + IT.curCreditVAT AS curVAT, CASE WHEN T.bCash=1 THEN ABS(IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0)))) ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN  ABS(IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0)))) ELSE 0 END AS curCredit, ABS(ISNULL(IT.curPayment, 0)) AS curPayment, TA.strCreatedBy, T.dateEntry, T.byteStatus FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
        Return Query
    End Function

    Private Function buildQuery2_old2(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) AS curAmount, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)))) AS curNet, SUM(-1 * B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS curQuantity, SUM(-1 * B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)) AS curPayment, SUM(ISNULL(XI.curVAT, 0)) AS curCashVAT, SUM(ISNULL(XI.curVATI, 0)) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, hhh.curValue FROM Stock_Xlink_Values AS hhh INNER JOIN Stock_Xlink AS X ON hhh.lngXlink = X.lngXlink;"
        Query = Query & "SELECT T.byteBase, T.byteStatus, T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, IT.curAmount AS curAmount, IT.curAmount-(IT.curNet + ISNULL(curValue,0)) AS curDiscount, IT.curCashVAT + IT.curCreditVAT AS curVAT, CASE WHEN T.bCash=1 THEN IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCashVAT ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN  IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCreditVAT ELSE 0 END AS curCredit, ISNULL(IT.curPayment, 0) AS curPayment, TA.strCreatedBy, T.dateEntry, T.byteStatus FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
        Return Query
    End Function

    Private Function buildQuery2_old3(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0))) ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0))) ELSE 0 END) AS curNet, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor ELSE 0 END) AS curQuantity, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)ELSE 0 END) AS curPayment, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END) AS curCashVAT, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVATI, 0) ELSE 0 END) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, XV.curValue FROM Stock_Xlink_Values AS XV INNER JOIN Stock_Xlink AS X ON XV.lngXlink = X.lngXlink;"
        Query = Query & "SELECT T.byteBase, T.byteStatus, T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, IT.curAmount AS curAmount, IT.curAmount-(IT.curNet + ISNULL(curValue,0)) AS curDiscount, IT.curCashVAT + IT.curCreditVAT AS curVAT, CASE WHEN T.bCash=1 THEN IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCashVAT ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN  IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCreditVAT ELSE 0 END AS curCredit, ISNULL(IT.curPayment, 0) AS curPayment, TA.strCreatedBy, T.dateEntry, CONVERT(varchar(10), T.dateClosedValid, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, T.byteStatus, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCredit, 0) ELSE 0 END AS curCreditPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCash, 0) ELSE 0 END AS curCashPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCashVAT, 0) ELSE 0 END AS curCashVAT FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS STI ON T.lngTransaction = STI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
        Return Query
    End Function

    Private Function buildQuery2(ByVal Where As String, ByVal ReportView As Byte) As String
        Dim Query As String = ""
        If ReportView = 1 Then
            'Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
            'Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0))) ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0))) ELSE 0 END) AS curNet, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor ELSE 0 END) AS curQuantity, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)ELSE 0 END) AS curPayment, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END) AS curCashVAT, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVATI, 0) ELSE 0 END) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
            Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
            Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, XV.curValue FROM Stock_Xlink_Values AS XV INNER JOIN Stock_Xlink AS X ON XV.lngXlink = X.lngXlink;"
            'Query = Query & "SELECT T.byteBase, T.byteStatus, T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType,  ISNULL(curValue,0) AS curDiscount, TA.strCreatedBy, T.dateEntry, CONVERT(varchar(10), T.dateClosedValid, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, T.byteStatus, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCredit, 0) ELSE 0 END AS curCreditPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCash, 0) ELSE 0 END AS curCashPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCashVAT, 0) ELSE 0 END AS curCashVAT FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS STI ON T.lngTransaction = STI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON T.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
            Query = Query & "SELECT COUNT(T.lngTransaction) AS Count, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END) AS curAmountVAT, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END) AS curCash, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END) AS curCashVAT, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END) AS curCredit, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END) AS curCreditVAT, SUM(ISNULL(curValue,0)) AS curDiscount FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN @Stock_Invoice_Discount AS ID ON ID.lngTransaction=T.lngTransaction WHERE " & Where
        Else
            'Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
            'Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0))) ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0))) ELSE 0 END) AS curNet, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor ELSE 0 END) AS curQuantity, SUM(CASE WHEN T.byteStatus > 0 THEN -1 * B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)ELSE 0 END) AS curPayment, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END) AS curCashVAT, SUM(CASE WHEN T.byteStatus > 0 THEN ISNULL(XI.curVATI, 0) ELSE 0 END) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
            Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
            Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, XV.curValue FROM Stock_Xlink_Values AS XV INNER JOIN Stock_Xlink AS X ON XV.lngXlink = X.lngXlink;"
            'Query = Query & "SELECT T.byteBase, T.byteStatus, T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType,  ISNULL(curValue,0) AS curDiscount, TA.strCreatedBy, T.dateEntry, CONVERT(varchar(10), T.dateClosedValid, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, T.byteStatus, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCredit, 0) ELSE 0 END AS curCreditPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCash, 0) ELSE 0 END AS curCashPayment, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, CASE WHEN T.byteStatus >  0 THEN ISNULL(STI.curCashVAT, 0) ELSE 0 END AS curCashVAT FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS STI ON T.lngTransaction = STI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON T.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
            Query = Query & "SELECT T.lngTransaction, T.byteBase, T.byteStatus, T.strTransaction, T.dateTransaction, T.bCash, T.byteTransType, TT.strTypeAr, TT.strTypeEn, CASE WHEN T.byteStatus > 0 THEN (CASE WHEN T.byteBase = 18 THEN 'Returned' ELSE 'Paid' END) ELSE 'Cancelled' END AS [strStatus], T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, T.lngContact, C.strContactEn, C.strContactAr, T.lngSalesman, D.strContactEn AS strDoctorEn, D.strContactAr AS strDoctorAr, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateClosedValid, CONVERT(varchar(10), T.dateClosedValid, 108) AS timeClosedValid, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curAmount, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END + CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curAmountVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCash, 0) ELSE 0 END AS curCash, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCashVAT, 0) ELSE 0 END AS curCashVAT, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCredit, 0) ELSE 0 END AS curCredit, CASE WHEN T.byteStatus > 0 THEN ISNULL(TI.curCreditVAT, 0) ELSE 0 END AS curCreditVAT, ISNULL(curValue,0) AS curDiscount, TI.strUserName AS strUserName FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN Hw_Contacts AS D ON T.lngSalesman = D.lngContact INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient LEFT JOIN @Stock_Invoice_Discount AS ID ON ID.lngTransaction=T.lngTransaction WHERE " & Where
        End If

        Return Query
    End Function

    Private Function buildQuery3_old(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "SELECT TA.strCashBy AS strUserName, CONVERT(varchar(10), T.dateClosedValid, 120) AS dateCashbox, SUM(CASE WHEN T.byteStatus>0 AND T.byteBase=40 THEN 1 ELSE 0 END) intInvoicesCount, SUM(CASE WHEN T.byteStatus=0 THEN 1 ELSE 0 END) AS intCancelledCount, SUM(CASE WHEN T.byteBase=18 THEN 1 ELSE 0 END) AS intReturnedCount, SUM(CASE WHEN T.byteStatus>0 THEN STI.curCash + STI.curCashVAT ELSE 0 END) AS curAmount, SUM(CASE WHEN T.byteStatus>0 AND STP.byteType=1 THEN STP.curAmount ELSE 0 END) AS curCash, SUM(CASE WHEN T.byteStatus>0 AND STP.byteType=2 THEN STP.curAmount ELSE 0 END) AS curCreditCard, SUM(CASE WHEN T.byteStatus>0 THEN ISNULL(STP.curAmount, 0) ELSE 0 END) AS curTotalPaid "
        Query = Query & "FROM Stock_Trans AS T INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON T.lngTransaction=STI.lngTransaction LEFT JOIN Stock_Trans_Payments AS STP ON T.lngTransaction=STP.lngTransaction "
        Query = Query & "WHERE " & Where
        Query = Query & "GROUP BY TA.strCashBy, CONVERT(varchar(10), T.dateClosedValid, 120)"
        Return Query
    End Function

    Private Function buildQuery3(ByVal Where1 As String, ByVal Where2 As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Paid AS Table (strUserName varchar(max), dateCashbox date, intPaid int);"
        Query = Query & "INSERT INTO @Stock_Paid SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intPaid FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=40 AND T.byteStatus>0 AND " & Where1 & " GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Cancelled AS Table (strUserName varchar(max), dateCashbox date, intCancelled int);"
        Query = Query & "INSERT INTO @Stock_Cancelled SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intCancelled FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=40 AND T.byteStatus=0 AND " & Where1 & " GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Returned AS Table (strUserName varchar(max), dateCashbox date, intReturned int);"
        Query = Query & "INSERT INTO @Stock_Returned SELECT TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120), COUNT(T.lngTransaction) AS intCancelled FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.byteBase=18 AND T.byteStatus>0 AND " & Where1 & " GROUP BY TI.strUserName, CONVERT(varchar(10), TI.dateTransaction, 120);"
        Query = Query & "DECLARE @Stock_Payment AS Table (lngTransaction int, curCash money, curCreditCard money);"
        Query = Query & "INSERT INTO @Stock_Payment SELECT lngTransaction, SUM(CASE WHEN byteType=1 THEN curAmount ELSE 0 END) AS curCash,SUM(CASE WHEN byteType=2 THEN curAmount ELSE 0 END) AS curCreditCard FROM Stock_Trans_Payments GROUP BY lngTransaction;"
        Query = Query & "SELECT ISNULL(P.intPaid, 0) AS intPaid, ISNULL(C.intCancelled, 0) AS intCancelled, ISNULL(R.intReturned, 0) AS intReturned, CONVERT(varchar(10), TI.dateTransaction, 120) AS dateCashbox, TI.strUserName AS strUserName, SUM(TI.curCash + TI.curCashVAT) AS curAmount, SUM(ISNULL(TP.curCash, 0)) AS curCash, SUM(ISNULL(TP.curCreditCard, 0)) AS curCreditCard, SUM(ISNULL(TP.curCash, 0) + ISNULL(TP.curCreditCard, 0)) AS curPayment FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON TI.lngTransaction=T.lngTransaction LEFT JOIN @Stock_Payment AS TP ON TP.lngTransaction=T.lngTransaction LEFT JOIN @Stock_Paid AS P ON CONVERT(varchar(10), P.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND P.strUserName=TI.strUserName LEFT JOIN @Stock_Returned AS R ON CONVERT(varchar(10), R.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND R.strUserName=TI.strUserName LEFT JOIN @Stock_Cancelled AS C ON CONVERT(varchar(10), C.dateCashbox, 120) = CONVERT(varchar(10), TI.dateTransaction, 120) AND C.strUserName=TI.strUserName WHERE T.byteBase IN (18,40) AND T.byteStatus>0 AND " & Where2 & " GROUP BY ISNULL(P.intPaid, 0), ISNULL(C.intCancelled, 0), ISNULL(R.intReturned, 0), CONVERT(varchar(10), TI.dateTransaction, 120), TI.strUserName order by TI.strUserName;"
        Return Query
    End Function
#End Region

    Public Function viewFilterReport(ByVal strFilter As String) As String
        Dim lblReportType, lblDateFrom, lblDateTo, lblInvoiceNo, lblInvoiceStatus, lblPaymentType, lblDoctor, lblUser As String
        Dim ReportTypeItem(6), InvoiceStatus(5), PaymentType(3) As String

        Select Case byteLanguage
            Case 2
                'Lables
                lblReportType = "نوع التقرير"
                lblDateFrom = "من تاريخ"
                lblDateTo = "إلى تاريخ"
                lblInvoiceNo = "رقم الفاتورة"
                lblInvoiceStatus = "حالة الفاتورة"
                lblPaymentType = "نوع الدفع"
                lblDoctor = "الطبيب"
                lblUser = "المستخدم"
                'List
                PaymentType(1) = "نقدي"
                PaymentType(0) = "آجل"
                PaymentType(2) = "كلاهما"
                InvoiceStatus(1) = "مدفوعة"
                InvoiceStatus(2) = "غير مدفوعة"
                InvoiceStatus(0) = "ملغية"
                InvoiceStatus(3) = "غير ملغية"
                InvoiceStatus(4) = "الكل"
                ReportTypeItem(0) = "حسب نوع الفاتورة"
                ReportTypeItem(1) = "حسب الطبيب"
                ReportTypeItem(2) = "حسب المستخدم"
                ReportTypeItem(3) = "حسب المستخدم(ملخص)"
                ReportTypeItem(4) = "الفواتير غير المطبوعة"
                ReportTypeItem(5) = "حسب الطبيب(ملخص)"
            Case Else
                'Lables
                lblReportType = "Report Type"
                lblDateFrom = "From Date"
                lblDateTo = "To Date"
                lblInvoiceNo = "Invoice No"
                lblInvoiceStatus = "Invoice Status"
                lblPaymentType = "Payment Type"
                lblDoctor = "Doctor"
                lblUser = "User"
                'List
                PaymentType(1) = "Cash"
                PaymentType(0) = "Credit"
                PaymentType(2) = "Both"
                InvoiceStatus(1) = "Paid"
                InvoiceStatus(2) = "Unpaid"
                InvoiceStatus(0) = "Cancelled"
                InvoiceStatus(3) = "Not Cancelled"
                InvoiceStatus(4) = "All"
                ReportTypeItem(0) = "By Invoice Type"
                ReportTypeItem(1) = "By Doctor"
                ReportTypeItem(2) = "By User"
                ReportTypeItem(3) = "By User (Summary)"
                ReportTypeItem(4) = "By Unprinted Invoices"
                ReportTypeItem(5) = "By Doctor (Summary)"
        End Select

        Dim filter() As String = Split(strFilter, ",")
        If filter.Length > 0 Then
            Dim str As String = ""
            str = str & "<div class=""col-md-12""><div class=""col-md-6""><h3><b>" & lblReportType & ":</b> " & ReportTypeItem(filter(0) - 1) & "</h3></div>"
            str = str & "<div class=""col-md-6""><b>" & lblDateFrom & ":</b><span class=""red""> " & CDate(filter(1)).ToString(strDateFormat) & " </span><b>" & lblDateTo & ":</b><span class=""red""> " & CDate(filter(2)).ToString(strDateFormat) & "</span></div></div>"
            str = str & "<div class=""col-md-12""><div class=""col-md-2"">"
            str = str & "<b>" & lblPaymentType & ":</b> " & PaymentType(filter(5))
            str = str & "</div>"
            str = str & "<div class=""col-md-2"">"
            'str = str & "<b>" & lblInvoiceStatus & ":</b> " & InvoiceStatus(filter(4))
            str = str & "</div>"
            If filter(3) <> "" Then str = str & "<div class=""col-md-2""><b> " & lblInvoiceNo & ":</b> " & Replace(filter(3), "|", ",") & "</div>"
            If filter(6) <> "" Then str = str & "<div class=""col-md-2""><b>" & lblDoctor & ":</b> " & getDoctorName(filter(6)) & "</div>"
            If filter(7) <> "" Then str = str & "<div class=""col-md-2""><b>" & lblUser & ":</b> " & filter(7) & "</div>"
            str = str & "</div>"
            Return str
        Else
            Return ""
        End If
    End Function
End Class
