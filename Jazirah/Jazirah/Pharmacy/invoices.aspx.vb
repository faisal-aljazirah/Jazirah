Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class invoices
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, All As String
    Public btnView, btnSearch, btnAdvanced, plcInvoice As String
    Public InvoiceStatus(4) As String

    Public strWait As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

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
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loadLanguage()
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الصيدلية | فواتير المبيعات"
                'Variables
                Cash = "نقدي"
                Insurance = "تأمين"
                All = "الكل"
                strWait = "فضلا انتظر..."
                'Buttons
                btnView = "عرض"
                btnSearch = "بحث"
                btnAdvanced = "متقدم"
                'List
                InvoiceStatus(0) = "الكل"
                InvoiceStatus(1) = "مدفوعة"
                InvoiceStatus(2) = "ملغية"
                InvoiceStatus(3) = "مرتجعة"
                'Others
                plcInvoice = "اكتب رقم الفاتورة, اسم العميل, الجوال, الهوية, فاتورة العيادة, رقم العملية, رقم الصنف.."
            Case Else
                DataLang = "En"
                Title = "Pharmacy | Sales Invoices"
                'Variables
                Cash = "Cash"
                Insurance = "Insurance"
                All = "All"
                strWait = "Please wait..."
                'Buttons
                btnView = "View"
                btnSearch = "Search"
                btnAdvanced = "Advanced"
                'List
                InvoiceStatus(0) = "All"
                InvoiceStatus(1) = "Paid"
                InvoiceStatus(2) = "Cancelled"
                InvoiceStatus(3) = "Returned"
                'Others
                plcInvoice = "Type Invoice Number, Patient Name, Mobile, ID, Clinic Invoice, Transaction No, Item Number.."
        End Select
    End Sub
End Class