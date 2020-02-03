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
    Public lblCancelRequests, lblReturnRequests, lblReopenRequests As String
    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colUser, colStatus, colRequestUser, colRequestDate As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Private Sub medicinesorders_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If HttpContext.Current.Session("UserName") Is Nothing Then
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
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loadLanguage()

        divCancel.Visible = False
        divReturn.Visible = False
        divReopen.Visible = False

        Dim dc As New DCL.Conn.XMLData

        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"))
        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName) = True Then
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Cancel_Invoice[@status=0]")
            If nodes.Count > 0 Then
                Dim SQL As String = "DECLARE @Requests AS Table (lngTransaction money, RequestUser varchar(16), RequestDate datetime);"
                For Each node As XmlNode In nodes
                    SQL = SQL & "INSERT INTO @Requests VALUES (" & node.SelectSingleNode("Transaction").InnerText & ",'" & node.Attributes("user").Value & "','" & CDate(node.Attributes("date").Value).ToString("yyyy-MM-dd") & "');"
                Next

                Dim ds As DataSet
                ds = dcl.GetDS(SQL & "SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, R.RequestUser, R.RequestDate FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact INNER JOIN @Requests AS R ON R.lngTransaction=ST.lngTransaction WHERE ST.byteBase = 40 AND ST.byteStatus=1 ORDER BY ST.lngTransaction DESC")
                If ds.Tables(0).Rows.Count > 0 Then
                    divCancel.Visible = True
                    repCancel.DataSource = ds
                    repCancel.DataBind()
                End If
            End If
        End If

        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Return_Items", "User", strUserName) = True Then
            doc.Load(Server.MapPath("../data/xml/requests.xml"))
            Dim SQL As String = "DECLARE @Requests AS Table (lngTransaction money, RequestUser varchar(16), RequestDate datetime);"
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Return_Items[@status=0]")
            If nodes.Count > 0 Then
                For Each node As XmlNode In nodes
                    SQL = SQL & "INSERT INTO @Requests VALUES (" & node.SelectSingleNode("Transaction").InnerText & ",'" & node.Attributes("user").Value & "','" & CDate(node.Attributes("date").Value).ToString("yyyy-MM-dd") & "');"
                Next

                Dim ds As DataSet
                ds = dcl.GetDS(SQL & "SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, R.RequestUser, R.RequestDate FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact INNER JOIN @Requests AS R ON R.lngTransaction=ST.lngTransaction WHERE ST.byteBase = 40 AND ST.byteStatus=1 ORDER BY ST.lngTransaction DESC")
                If ds.Tables(0).Rows.Count > 0 Then
                    divReturn.Visible = True
                    repReturn.DataSource = ds
                    repReturn.DataBind()
                End If
            End If
        End If

        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Reopen_Invoice", "User", strUserName) = True Then
            doc.Load(Server.MapPath("../data/xml/requests.xml"))
            Dim SQL As String = "DECLARE @Requests AS Table (lngTransaction money, RequestUser varchar(16), RequestDate datetime);"
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Reopen_Invoice[@status=0]")
            If nodes.Count > 0 Then
                For Each node As XmlNode In nodes
                    SQL = SQL & "INSERT INTO @Requests VALUES (" & node.SelectSingleNode("Transaction").InnerText & ",'" & node.Attributes("user").Value & "','" & CDate(node.Attributes("date").Value).ToString("yyyy-MM-dd") & "');"
                Next

                Dim ds As DataSet
                ds = dcl.GetDS(SQL & "SELECT ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, R.RequestUser, R.RequestDate FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact INNER JOIN @Requests AS R ON R.lngTransaction=ST.lngTransaction WHERE ST.byteBase = 40 AND ST.byteStatus=1 ORDER BY ST.lngTransaction DESC")
                If ds.Tables(0).Rows.Count > 0 Then
                    divReopen.Visible = True
                    repReopen.DataSource = ds
                    repReopen.DataBind()
                End If
            End If
        End If
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الرئيسية | التعميدات"
                'Labels
                lblCancelRequests = "طلبات الإلغاء"
                lblReturnRequests = "طلبات استرجاع الأصناف"
                lblReopenRequests = "طلبات إعادة الفتح"
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
                colRequestUser = "صاحب الطلب"
                colRequestDate = "تاريخ الطلب"
            Case Else
                DataLang = "En"
                Title = "Home | Approvals"
                'Labels
                lblCancelRequests = "Cancellation Requests"
                lblReturnRequests = "Items Returning Requests"
                lblReopenRequests = "Re-Opening Requests"
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
                colRequestUser = "Request User"
                colRequestDate = "Request Date"
        End Select
    End Sub
End Class