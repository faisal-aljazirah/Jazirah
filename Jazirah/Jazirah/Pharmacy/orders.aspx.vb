Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class orders
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String

    Public byteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public btnCashInvoice, btnCreditInvoice, btnCashBox As String
    Dim plcSearch, All As String

    Dim Selected_Department As Byte
    Dim Selected_Doctor As Long
    Dim Selected_Text As String

    Dim byteOrdersLimitDays As Byte
    Public PopupToPrint, AllowCashbox As Boolean

    Dim p_Prepare, p_Sales, p_Cashier As Boolean
    Public C_disabled, S_disabled As String

    Public strWait As String

    Private Sub medicinesorders_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            Response.Redirect("../login.aspx")
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

        loadAppSettings()
        loadUserSettings()
    End Sub

    Private Sub loadAppSettings()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/settings.xml"))
        Dim application As XmlNode = doc.SelectSingleNode("Settings/Pharmacy")
        byteOrdersLimitDays = application.SelectSingleNode("OrdersLimitDays").InnerText
        If application.SelectSingleNode("PopupToPrint") Is Nothing Then PopupToPrint = True Else PopupToPrint = application.SelectSingleNode("PopupToPrint").InnerText
        If application.SelectSingleNode("AllowCashbox") Is Nothing Then AllowCashbox = True Else AllowCashbox = application.SelectSingleNode("AllowCashbox").InnerText
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loadLanguage()

        If p_Cashier = False Then C_disabled = "disabled=""disabled"""
        If p_Sales = False Then S_disabled = "disabled=""disabled"""

        Dim Where_SQL As String = ""
        If Request.QueryString("d") Is Nothing Then Selected_Department = 0 Else Selected_Department = Request.QueryString("d")
        If Request.QueryString("c") Is Nothing Then Selected_Doctor = 0 Else Selected_Doctor = Request.QueryString("c")
        If Request.QueryString("t") Is Nothing Then Selected_Text = "" Else Selected_Text = Request.QueryString("t")

        'If Selected_Department <> 0 Then
        '    Where_SQL = " AND ST.byteDepartment=" & Selected_Department
        'End If
        'If Selected_Doctor <> 0 Then
        '    Where_SQL = " AND ST.lngSalesman=" & Selected_Doctor
        'End If
        'If Selected_Text <> "" Then
        '    If IsNumeric(Selected_Text) = True Then
        '        Where_SQL = " AND (P.strID='" & Selected_Text & "' OR ST.strTransaction='" & Selected_Text & "' OR ST.strReference='" & Selected_Text & "' OR ST.lngTransaction='" & Selected_Text & "' OR P.strPhone1='" & Selected_Text & "')"
        '    Else
        '        Where_SQL = " AND (P.strFirst" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strSecond" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strThird" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strLast" & DataLang & " LIKE '%" & Selected_Text & "%')"
        '    End If
        'End If

        'Dim ds As DataSet
        'ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & DateAdd(DateInterval.Day, -1, Today).ToString("yyyy-MM-dd") & "' " & Where_SQL & " ORDER BY ST.lngTransaction DESC")
        'repInvoices.DataSource = ds
        'repInvoices.DataBind()
    End Sub

    Function showStatus(ByVal TransNo As Long, ByVal Status As Integer) As String
        Dim disabled As String = ""
        If p_Prepare = False Then disabled = "disabled=""disabled"""
        Return "<button type=""button"" onclick=""javascript:showOrder(" & TransNo & ", false);"" class=""btn btn-sm btn-primary"" " & disabled & "> " & View & " </button>"
    End Function

    Sub loadLanguage()
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الصيدلية | طلبات الأدوية"
                'Variables
                Cash = "نقدي"
                Insurance = "تأمين"
                View = "عرض"
                All = "الكل"
                plcSearch = "ابحث بالاسم, الهوية, الفاتورة, الجوال"
                strWait = "فضلا انتظر..."
                'Columns
                colInvoice = "الطلب"
                colPatient = "اسم المريض"
                colDoctor = "اسم الطبيب"
                colDate = "التاريخ"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = "الحالة"
                ' Buttons
                btnCashInvoice = "فاتورة نقدية"
                btnCreditInvoice = "فاتورة آجلة"
                btnCashBox = "الصندوق"
            Case Else
                DataLang = "En"
                Title = "Pharmacy | Medicines Orders"
                'Variables
                Cash = "Cash"
                Insurance = "Insurance"
                View = "View"
                All = "All"
                plcSearch = "Search by name, id, invoice, mobile"
                strWait = "Please wait..."
                'Columns
                colInvoice = "Order"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = "Status"
                ' Buttons
                btnCashInvoice = "Cash Invoice"
                btnCreditInvoice = "Credit Invoice"
                btnCashBox = "Cashbox"
        End Select
    End Sub

    Function getSearchText1() As String
        Dim str As String = ""
        str = str & "<input type=""text"" class=""form-control form-control-sm input-sm selAll"" id=""txtOrdersSearch1"" placeholder=""" & plcSearch & """ value=""" & Selected_Text & """ />"
        Return str
    End Function

    Function getSearchText2() As String
        Dim str As String = ""
        str = str & "<input type=""text"" class=""form-control form-control-sm input-sm selAll"" id=""txtOrdersSearch2"" placeholder=""" & plcSearch & """ value=""" & Selected_Text & """ />"
        Return str
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillDepartments1() As String
        Dim DataLang, All As String
        Select Case HttpContext.Current.Session("UserLanguage")
            Case 2
                DataLang = "Ar"
                All = "الكل"
            Case Else
                DataLang = "En"
                All = "All"
        End Select
        Dim str As String = ""
        'str = str & "<select class=""form-control"" onchange=""javascript:window.location='?d=' + this.value;"">"
        str = str & "<select class=""form-control select2"" id=""drpDepartments1"" onchange=""javascript:fillOrders(this.value,'','');fillDoctors1($('#drpDepartments1').val());"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        Dim dcl As New DCL.Conn.DataClassLayer
        ds = dcl.GetDS("SELECT * FROM Hw_Departments AS D INNER JOIN Hw_Departments_Details AS DD ON D.byteDepartment=DD.byteDepartment WHERE DD.bPharmacyOrder=1 AND D.bMedicalCenter=1")
        Dim selected As String = ""
        For I = 0 To ds.Tables(0).Rows.Count - 1
            'If ds.Tables(0).Rows(I).Item("byteDepartment") = Selected_Department Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("byteDepartment") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strDepartment" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        str = str & "<script type=""text/javascript"">$('.select2').select2();</script>"
        Return str
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillDepartments2() As String
        Dim DataLang, All As String
        Select Case HttpContext.Current.Session("UserLanguage")
            Case 2
                DataLang = "Ar"
                All = "الكل"
            Case Else
                DataLang = "En"
                All = "All"
        End Select
        Dim str As String = ""
        'str = str & "<select class=""form-control"" onchange=""javascript:window.location='?d=' + this.value;"">"
        str = str & "<select class=""form-control select2"" id=""drpDepartments2"" onchange=""javascript:fillOldOrders(this.value,'','');fillDoctors2($('#drpDepartments2').val());"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        Dim dcl As New DCL.Conn.DataClassLayer
        ds = dcl.GetDS("SELECT * FROM Hw_Departments AS D INNER JOIN Hw_Departments_Details AS DD ON D.byteDepartment=DD.byteDepartment WHERE DD.bPharmacyOrder=1 AND D.bMedicalCenter=1")
        Dim selected As String = ""
        For I = 0 To ds.Tables(0).Rows.Count - 1
            'If ds.Tables(0).Rows(I).Item("byteDepartment") = Selected_Department Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("byteDepartment") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strDepartment" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        str = str & "<script type=""text/javascript"">$('.select2').select2();</script>"
        Return str
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillDoctors1(ByVal Department As Byte) As String
        Dim DataLang, All As String
        Select Case HttpContext.Current.Session("UserLanguage")
            Case 2
                DataLang = "Ar"
                All = "الكل"
            Case Else
                DataLang = "En"
                All = "All"
        End Select
        Dim str As String = ""
        'str = str & "<select class=""form-control"" onchange=""javascript:window.location='?c=' + this.value;"">"
        str = str & "<select class=""form-control select2"" id=""drpDoctors1"" onchange=""javascript:fillOrders('',this.value,'');"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        Dim dcl As New DCL.Conn.DataClassLayer
        Dim where As String
        If Department <> 0 Then where = " AND C.byteDepartment=" & Department Else where = ""
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts AS C INNER JOIN Hw_Contacts_Details AS CD ON C.lngContact=CD.lngContact WHERE CD.bPharmacyOrder=1 AND C.byteClass=3 " & where)
        Dim selected As String = ""
        For I = 0 To ds.Tables(0).Rows.Count - 1
            'If ds.Tables(0).Rows(I).Item("lngContact") = Selected_Doctor Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("lngContact") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        str = str & "<script type=""text/javascript"">$('.select2').select2();</script>"
        Return str
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillDoctors2(ByVal Department As Byte) As String
        Dim DataLang, All As String
        Select Case HttpContext.Current.Session("UserLanguage")
            Case 2
                DataLang = "Ar"
                All = "الكل"
            Case Else
                DataLang = "En"
                All = "All"
        End Select
        Dim str As String = ""
        'str = str & "<select class=""form-control"" onchange=""javascript:window.location='?c=' + this.value;"">"
        str = str & "<select class=""form-control select2"" id=""drpDoctors2"" onchange=""javascript:fillOldOrders('',this.value,'');"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        Dim dcl As New DCL.Conn.DataClassLayer
        Dim where As String
        If Department <> 0 Then where = " AND C.byteDepartment=" & Department Else where = ""
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts AS C INNER JOIN Hw_Contacts_Details AS CD ON C.lngContact=CD.lngContact WHERE CD.bPharmacyOrder=1 AND C.byteClass=3 " & where)
        Dim selected As String = ""
        For I = 0 To ds.Tables(0).Rows.Count - 1
            'If ds.Tables(0).Rows(I).Item("lngContact") = Selected_Doctor Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("lngContact") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        str = str & "<script type=""text/javascript"">$('.select2').select2();</script>"
        Return str
    End Function

    Function getActiveDoctorList1() As String
        Dim str As String = ""
        str = str & "<div class=""btn-group btn-group-toggle"" data-toggle=""buttons"">"
        str = str & "<label class=""btn btn-primary btn-sm"" onclick=""javascript:fillOrders('','','');fillDepartments1();fillDoctors1(0);$('#txtOrdersSearch1').val('');""><input type=""radio"" value=""0"" name=""options""> " & All & "</label>"
        Dim ds As DataSet
        Dim where As String
        If Selected_Department <> 0 Then where = " AND byteDepartment=" & Selected_Department Else where = ""
        ds = dcl.GetDS("SELECT TOP 10 HC.lngContact, HC.strContact" & DataLang & ", COUNT(ST.lngTransaction) AS Invoices FROM Hw_Contacts AS HC INNER JOIN Stock_Trans AS ST ON HC.lngContact=ST.lngSalesman WHERE byteBase = 50 AND bCollected1 = 1 AND byteStatus = 1 AND bApproved1 = 0 AND (bSubCash = 0 OR bSubCash IS NULL) AND CONVERT(varchar(10), dateTransaction, 120) = '" & Today.ToString("yyyy-MM-dd") & "' GROUP BY HC.lngContact, HC.strContact" & DataLang & " ORDER BY Invoices DESC")
        'ds = dcl.GetDS("SELECT TOP 7 * FROM Hw_Contacts WHERE lngContact IN (SELECT lngSalesman FROM Stock_Trans WHERE byteBase = 50 AND Year(dateTransaction) = 2019 AND bCollected1 = 1 AND byteStatus = 1 AND bApproved1 = 0 AND (bSubCash = 0 OR bSubCash IS NULL) AND CONVERT(varchar(10), dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "')")
        Dim checked, active As String
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("lngContact") = Selected_Doctor Then
                checked = " checked=""checked"""
                active = "active"
            Else
                checked = ""
                active = ""
            End If
            'str = str & "<label class=""btn btn-secondary " & active & """ onclick=""javascript:window.location='?c=" & ds.Tables(0).Rows(I).Item("lngContact") & "';""><input type=""radio"" value=""0"" name=""options"" " & checked & "> " & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</label>"
            str = str & "<label class=""btn btn-secondary btn-sm " & active & """ onclick=""javascript:fillOrders(''," & ds.Tables(0).Rows(I).Item("lngContact") & ",'');""><input type=""radio"" value=""0"" name=""options"" " & checked & "> " & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</label>"
        Next
        str = str & "<label class=""btn btn-primary btn-sm"" onclick=""javascript:fillOrders($('#drpDepartments1').val(), $('#drpDoctors1').val(), $('#txtOrdersSearch1').val());""><input type=""radio"" value=""0"" name=""options""> Refresh </label>"
        str = str & "</div>"
        Return str
    End Function

    Function getActiveDoctorList2() As String
        Dim str As String = ""
        str = str & "<div class=""btn-group btn-group-toggle"" data-toggle=""buttons"">"
        str = str & "<label class=""btn btn-primary btn-sm"" onclick=""javascript:fillOldOrders('','','');fillDepartments2();fillDoctors2(0);$('#txtOrdersSearch2').val('');""><input type=""radio"" value=""0"" name=""options""> " & All & "</label>"
        Dim ds As DataSet
        Dim where As String
        If Selected_Department <> 0 Then where = " AND byteDepartment=" & Selected_Department Else where = ""
        ds = dcl.GetDS("SELECT TOP 10 HC.lngContact, HC.strContact" & DataLang & ", COUNT(ST.lngTransaction) AS Invoices FROM Hw_Contacts AS HC INNER JOIN Stock_Trans AS ST ON HC.lngContact=ST.lngSalesman WHERE byteBase = 50 AND bCollected1 = 1 AND byteStatus = 1 AND bApproved1 = 0 AND (bSubCash = 0 OR bSubCash IS NULL) AND CONVERT(varchar(10), dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' GROUP BY HC.lngContact, HC.strContact" & DataLang & " ORDER BY Invoices DESC")
        'ds = dcl.GetDS("SELECT TOP 7 * FROM Hw_Contacts WHERE lngContact IN (SELECT lngSalesman FROM Stock_Trans WHERE byteBase = 50 AND Year(dateTransaction) = 2019 AND bCollected1 = 1 AND byteStatus = 1 AND bApproved1 = 0 AND (bSubCash = 0 OR bSubCash IS NULL) AND CONVERT(varchar(10), dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "')")
        Dim checked, active As String
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("lngContact") = Selected_Doctor Then
                checked = " checked=""checked"""
                active = "active"
            Else
                checked = ""
                active = ""
            End If
            'str = str & "<label class=""btn btn-secondary " & active & """ onclick=""javascript:window.location='?c=" & ds.Tables(0).Rows(I).Item("lngContact") & "';""><input type=""radio"" value=""0"" name=""options"" " & checked & "> " & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</label>"
            str = str & "<label class=""btn btn-secondary btn-sm " & active & """ onclick=""javascript:fillOldOrders(''," & ds.Tables(0).Rows(I).Item("lngContact") & ",'');""><input type=""radio"" value=""0"" name=""options"" " & checked & "> " & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</label>"
        Next
        str = str & "<label class=""btn btn-primary btn-sm"" onclick=""javascript:fillOldOrders($('#drpDepartments2').val(), $('#drpDoctors2').val(), $('#txtOrdersSearch2').val());""><input type=""radio"" value=""0"" name=""options""> Refresh </label>"
        str = str & "</div>"
        Return str
    End Function

    Function fillDepartment() As String
        Dim str As String = ""

        str = str & "<div class=""bg-grey bg-lighten-2 dep_content""><ul class=""nav nav-tabs nav-fill"" id=""myTab"" role=""tablist"">"
        Dim ds_dep As DataSet
        ds_dep = dcl.GetDS("SELECT * FROM Hw_Departments WHERE byteDepartment IN (SELECT DISTINCT(byteDepartment) FROM Stock_Trans WHERE byteBase=40)")
        For I = 0 To ds_dep.Tables(0).Rows.Count - 1
            str = str & "<li class=""nav-item""><a class=""nav-link"" id=""" & ds_dep.Tables(0).Rows(I).Item("byteDepartment") & "-tab"" data-toggle=""tab"" href=""#tab" & ds_dep.Tables(0).Rows(I).Item("byteDepartment") & """ role=""tab"" aria-controls=""home"" aria-selected=""true""><div class=""avatar""><img src=""../app-assets/images/portrait/small/avatar-s-1.png"" alt=""avatar""></div><div>" & ds_dep.Tables(0).Rows(I).Item("strDepartment" & DataLang) & "</div></a></li>"
        Next
        str = str & "</ul>"
        str = str & "<div class=""tab-content"" id=""myTabContent"">"
        Dim ds_doc As DataSet
        ds_doc = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE byteClass=3 AND byteDepartment IN (SELECT byteDepartment FROM Hw_Departments WHERE bMedicalCenter=1) ORDER BY byteDepartment")
        Dim sel_dep As Byte = 0
        str = str & "<div class=""tab-pane fade"" id=""home"" role=""tabpanel"" aria-labelledby=""0-tab""><ul class=""doc_list""><ul class=""doc_list"">"
        For I = 0 To ds_doc.Tables(0).Rows.Count - 1
            If ds_doc.Tables(0).Rows(I).Item("byteDepartment") <> sel_dep Then
                sel_dep = ds_doc.Tables(0).Rows(I).Item("byteDepartment")
                str = str & "</ul></div>"
                str = str & "<div class=""tab-pane fade"" id=""tab" & ds_doc.Tables(0).Rows(I).Item("byteDepartment") & """ role=""tabpanel"" aria-labelledby=""" & ds_doc.Tables(0).Rows(I).Item("byteDepartment") & "-tab""><ul class=""doc_list""><ul class=""doc_list"">"
            End If
            str = str & "<li><a><div class=""avatar""><img src=""../app-assets/images/portrait/small/avatar-s-1.png"" alt=""avatar""></div><div>" & ds_doc.Tables(0).Rows(I).Item("strContact" & DataLang) & "</div></a></li>"
        Next
        str = str & "</ul></div>"
        str = str & "</div>"
        Return str
    End Function
End Class