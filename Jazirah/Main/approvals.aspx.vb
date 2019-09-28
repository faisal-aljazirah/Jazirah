Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class approvals
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, All As String
    Public btnView, btnApprove, btnReject As String

    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colUser, colStatus As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AskBeforeSend, AskBeforeReturn As Boolean
    Dim SusbendMax, byteDepartment_Cash As Byte
    Dim byteInvoicesLimitDays As Byte

    Private Sub medicinesorders_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If HttpContext.Current.Session("UserName") Is Nothing Then
            'Remove this line after complete user login and settings
            HttpContext.Current.Session("UserName") = "SoftNet"
            'Response.Redirect("../login.aspx")
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

        byteInvoicesLimitDays = 3
        ' User (Application) Options and Permissions
        ChangeQuantity_Cash = True ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        AddDiscount_Cash = False
        ChangeQuantity_Insurance = False ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        AddDiscount_Insurance = False
        AllowExtraItem_Insurance = True ' True = Allow more amount of approved item, False = Allow Exact amount or less
        AutoMoveRejectedToCash_Insurance = False 'True = move automaticily, False = Popup a confirm message
        SusbendMax = 5 ' Set a maximum number of suspend invoices
        AskBeforeSend = True 'True = Ask before send invoice to cashier, False = Send Directly
        AskBeforeReturn = True 'True = Ask before return invoice to sales, False = Return Directly
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


        'old
        'ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ST.strRemarks AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST INNER JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND ST.dateTransaction >= '2019-01-01'")
        'for testing
        'SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, ST.strRemarks AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContactEn AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCash = 1 THEN 'Cash' ELSE 'Insurance' END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND Year(ST.dateTransaction) = 2019 AND ST.bCollected1 = 1 AND ST.byteStatus = 1 AND ST.bApproved1 = 0 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) = CONVERT(varchar(10), GETDATE(), 120) ORDER BY ST.lngTransaction DESC

        divCancel.Visible = False
        divReturn.Visible = False

        Dim dc As New DCL.Conn.XMLData
        Dim doc As New XmlDocument
        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName) = True Then
            doc.Load(Server.MapPath("../data/xml/requests.xml"))
            Dim str As String = ""
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
            If nodes.Count > 0 Then
                For Each node As XmlNode In nodes
                    str = str & node.SelectSingleNode("Transaction").InnerText & ","
                Next

                Dim ds As DataSet
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus=1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteInvoicesLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction IN (" & str & "0) ORDER BY ST.lngTransaction DESC")
                If ds.Tables(0).Rows.Count > 0 Then
                    divCancel.Visible = True
                    repCancel.DataSource = ds
                    repCancel.DataBind()
                End If
            End If
        End If

        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Return_Items", "User", strUserName) = True Then
            doc.Load(Server.MapPath("../data/xml/requests.xml"))
            Dim str As String = ""
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items[@status=0]")
            If nodes.Count > 0 Then
                For Each node As XmlNode In nodes
                    str = str & node.SelectSingleNode("Transaction").InnerText & ","
                Next

                Dim ds As DataSet
                ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND ST.byteStatus=1 AND Year(ST.dateTransaction) = 2019 AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteInvoicesLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' AND ST.lngTransaction IN (" & str & "0) ORDER BY ST.lngTransaction DESC")
                If ds.Tables(0).Rows.Count > 0 Then
                    divReturn.Visible = True
                    repReturn.DataSource = ds
                    repReturn.DataBind()
                End If
            End If
        End If
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الصيدلية | طلبات الأدوية"
                'Variables
                Cash = "نقدي"
                Insurance = "تأمين"
                All = "الكل"
                'Buttons
                btnView = "عرض"
                btnApprove = "تعميد"
                btnReject = "رفض"
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
                Title = "Pharmacy | Medicines Orders"
                'Variables
                Cash = "Cash"
                Insurance = "Insurance"
                All = "All"
                'Buttons
                btnView = "View"
                btnApprove = "Approve"
                btnReject = "Reject"
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
    End Sub
End Class