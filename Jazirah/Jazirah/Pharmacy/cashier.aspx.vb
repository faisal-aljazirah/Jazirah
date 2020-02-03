Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class cashier
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, All As String
    Public btnView, btnPay, btnReturn As String

    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colUser, colPrice, colStatus As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Dim AskBeforeReturn As Boolean
    Dim byteOrdersLimitDays As Byte

    Dim p_Prepare, p_Sales, p_Cashier As Boolean
    Public C_disabled, S_disabled As String

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
        ByteLanguage = HttpContext.Current.Session("UserLanguage")
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
        If application.SelectSingleNode("AskBeforeReturn") Is Nothing Then AskBeforeReturn = True Else AskBeforeReturn = application.SelectSingleNode("AskBeforeReturn").InnerText
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
        If p_Cashier = False And p_Sales = False Then S_disabled = "disabled=""disabled"""

        Dim ds As DataSet
        ds = dcl.GetDS("SELECT ST.lngTransaction AS TransactionNo, ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(P.strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(P.strLast" & DataLang & ",''))) AS PatientName, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, D.byteDepartment AS DepartmentNo, D.strDepartment" & DataLang & " AS DepartmentName, C1.lngContact AS DoctorNo, C1.strContact" & DataLang & " AS DoctorName, ST.strReference AS ClinicInvoiceNo, CASE WHEN ST.bCreatCash=1 THEN '" & All & "' ELSE (CASE WHEN ST.bCash = 1 THEN '" & Cash & "' ELSE '" & Insurance & "' END) END AS PaymentType, STI.curCash + STI.curCashVAT AS Price , C2.lngContact AS CompanyNo, C2.strContact" & DataLang & " AS CompanyName, STA.strCreatedBy AS UserName, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Stock_Trans_Invoices AS STI ON STI.lngTransaction  = ST.lngTransaction  INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 50 AND ST.bCollected1 = 1 AND ST.byteStatus = 2 AND ST.bApproved1 = 1 AND (ST.bSubCash = 0 OR ST.bSubCash IS NULL) AND CONVERT(varchar(10), ST.dateTransaction, 120) BETWEEN '" & DateAdd(DateInterval.Day, -1 * byteOrdersLimitDays, Today).ToString("yyyy-MM-dd") & "' AND '" & Today.ToString("yyyy-MM-dd") & "' ORDER BY ST.lngTransaction DESC")
        repInvoices.DataSource = ds
        repInvoices.DataBind()
    End Sub

    Function AllowReturn(ByVal TransNo As Long) As String
        If AskBeforeReturn = True Then
            Return "confirm('', 'Return this invoice to sales?', function(){returnToSales(" & TransNo & ");});"
        Else
            Return "returnToSales(" & TransNo & ");"
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
                All = "الكل"
                'Buttons
                btnView = "عرض"
                btnPay = "دفع"
                btnReturn = "إرجاع"
                'Columns
                colInvoice = "رقم الفاتورة"
                colPatient = "اسم المريض"
                colDoctor = "الدكتور المعالج"
                colDate = "تاريخ الفاتورة"
                colDepartment = "العيادة"
                colCompany = "الشركة"
                colType = "النوع"
                colUser = "المستخدم"
                colPrice = "السعر"
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
                btnPay = "Pay"
                btnReturn = "Return"
                'Columns
                colInvoice = "Invoice No"
                colPatient = "Patient Name"
                colDoctor = "Doctor Name"
                colDate = "Invoice Date"
                colDepartment = "Clenic"
                colCompany = "Company"
                colType = "Type"
                colUser = "User"
                colPrice = "Price"
                colStatus = "Status"
        End Select
    End Sub
End Class