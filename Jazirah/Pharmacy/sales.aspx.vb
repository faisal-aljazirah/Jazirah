Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Public Class sales
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colStatus As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public btnCashInvoice, btnCashBox As String
    Dim plcSearch, All As String

    Dim Selected_Department As Byte
    Dim Selected_Doctor As Long
    Dim Selected_Text As String

    Dim byteOrdersLimitDays As Byte
    Public PopupToPrint As Boolean

    Private Sub medicinesorders_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
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
        ByteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")

        byteOrdersLimitDays = 7
        PopupToPrint = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Remove this line after complete user login and settings
        '=>
        'Session("UserID") = 1
        'Session("UserLanguage") = 2
        '<=

        'TODO: check authorization
        'TODO: load system settings
        'TODO: load user settings

        'TODO: get data
        loadLanguage()

        Dim Where_SQL As String = ""
        If Request.QueryString("d") Is Nothing Then Selected_Department = 0 Else Selected_Department = Request.QueryString("d")
        If Request.QueryString("c") Is Nothing Then Selected_Doctor = 0 Else Selected_Doctor = Request.QueryString("c")
        If Request.QueryString("t") Is Nothing Then Selected_Text = "" Else Selected_Text = Request.QueryString("t")

        If Selected_Department <> 0 Then
            Where_SQL = " AND ST.byteDepartment=" & Selected_Department
        End If
        If Selected_Doctor <> 0 Then
            Where_SQL = " AND ST.lngSalesman=" & Selected_Doctor
        End If
        If Selected_Text <> "" Then
            If IsNumeric(Selected_Text) = True Then
                Where_SQL = " AND (P.strID='" & Selected_Text & "' OR ST.strTransaction='" & Selected_Text & "' OR ST.strReference='" & Selected_Text & "' OR ST.lngTransaction='" & Selected_Text & "' OR P.strPhone1='" & Selected_Text & "')"
            Else
                Where_SQL = " AND (P.strFirst" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strSecond" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strThird" & DataLang & " LIKE '%" & Selected_Text & "%' OR P.strLast" & DataLang & " LIKE '%" & Selected_Text & "%')"
            End If
        End If

        Dim ds As DataSet
        'old
        'ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ST.strRemarks AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.dateTransaction >= '2019-01-01'")
        'for testing
        'SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ST.strRemarks AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContactEn AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) = CONVERT(varchar(10), GETDATE(), 120) ORDER BY ST.lngTransaction DESC
        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' " & Where_SQL & " ORDER BY ST.lngTransaction DESC")
        repInvoices.DataSource = ds
        repInvoices.DataBind()
        'Response.Write(Encrypt("123"))
    End Sub

    Function showStatus(ByVal TransNo As Long, ByVal Status As Integer) As String
        If Status = 1 Then
            Return ""
        Else
            Return "<button type=""button"" onclick=""javascript:showOrder(" & TransNo & ");"" class=""btn btn-sm btn-primary""> " & View & " </button>"
        End If
    End Function

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الصيدلية | طلبات الأدوية"
                'Variables
                Cash = "نقدي"
                Insurance = "تأمين"
                View = "عرض"
                All = "الكل"
                plcSearch = "ابحث بالاسم, الهوية, الفاتورة, الجوال"
                'Columns
                colInvoice = "فاتورة العيادة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colStatus = "الحالة"
                ' Buttons
                btnCashInvoice = "فاتورة نقدية"
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
                'Columns
                colInvoice = "Clinic Invoice"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colStatus = "Status"
                ' Buttons
                btnCashInvoice = "Cash Invoice"
                btnCashBox = "Cashbox"
        End Select
    End Sub

    'Function getDepartmentList2() As String
    '    Dim str As String = "" '"<div class=""btn-group"" role=""group"" style="" width:100%; overflow:auto; display:inline-block"">"
    '    str = str & "<button type=""button"" class=""btn btn-primary"">All Departments</button>"
    '    Dim ds As DataSet
    '    ds = dcl.GetDS("SELECT * FROM Hw_Departments WHERE bMedicalCenter=1")
    '    For I = 0 To ds.Tables(0).Rows.Count - 1
    '        str = str & "<button type=""button"" class=""btn btn-secondary"">" & ds.Tables(0).Rows(I).Item("strDepartment" & DataLang) & "</button>"
    '    Next
    '    'str = str & "</div>"
    '    Return str
    'End Function

    Function getSearchText() As String
        Dim str As String = ""
        str = str & "<input type=""text"" class=""form-control"" id=""txtOrdersSearch"" placeholder=""" & plcSearch & """ value=""" & Selected_Text & """ />"
        Return str
    End Function

    Function getDepartmentList() As String
        Dim str As String = ""
        str = str & "<select class=""form-control"" onchange=""javascript:window.location='?d=' + this.value;"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Departments WHERE bMedicalCenter=1")
        Dim selected As String
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("byteDepartment") = Selected_Department Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("byteDepartment") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strDepartment" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        Return str
    End Function

    Function getDoctorList() As String
        Dim str As String = ""
        str = str & "<select class=""form-control"" onchange=""javascript:window.location='?c=' + this.value;"">"
        str = str & "<option value=""0"">" & All & "</button>"
        Dim ds As DataSet
        Dim where As String
        If Selected_Department <> 0 Then where = " AND byteDepartment=" & Selected_Department Else where = ""
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE byteClass=3 " & where)
        Dim selected As String
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(I).Item("lngContact") = Selected_Doctor Then selected = " selected=""selected""" Else selected = ""
            str = str & "<option value=""" & ds.Tables(0).Rows(I).Item("lngContact") & """ " & selected & ">" & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</option>"
        Next
        str = str & "</select>"
        Return str
    End Function

    Function getActiveDoctorList() As String
        Dim str As String = ""
        str = str & "<div class=""btn-group btn-group-toggle w-100"" data-toggle=""buttons"">"
        Dim ds As DataSet
        Dim where As String
        If Selected_Department <> 0 Then where = " AND byteDepartment=" & Selected_Department Else where = ""
        ds = dcl.GetDS("SELECT TOP 7 HC.lngContact, HC.strContact" & DataLang & ", COUNT(ST.lngTransaction) AS Invoices FROM Hw_Contacts AS HC INNER JOIN Stock_Trans AS ST ON HC.lngContact=ST.lngSalesman WHERE byteBase = 50 AND Year(dateTransaction) = 2019 AND bCollected1 = 1 AND byteStatus = 1 AND bApproved1 = 0 AND (bSubCash = 0 OR bSubCash IS NULL) AND CONVERT(varchar(10), dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, (-1 * byteOrdersLimitDays), Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' GROUP BY HC.lngContact, HC.strContact" & DataLang & " ORDER BY Invoices DESC")
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
            str = str & "<label class=""btn btn-secondary " & active & """ onclick=""javascript:window.location='?c=" & ds.Tables(0).Rows(I).Item("lngContact") & "';""><input type=""radio"" value=""0"" name=""options"" " & checked & "> " & ds.Tables(0).Rows(I).Item("strContact" & DataLang) & "</label>"
        Next
        str = str & "</div>"
        Return str
    End Function
End Class