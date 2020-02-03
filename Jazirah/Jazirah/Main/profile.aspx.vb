Public Class profile
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public lblUserName, lblFullName, lblPosition, lblMobile, lblEmail, lblExtension As String
    Public UserName, FullName, Position, Mobile, Email, Extension As String
    Public btnUpdateProfile, btnChangePassword, btnUploadPicture As String


    Private Sub profile_Init(sender As Object, e As EventArgs) Handles Me.Init
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

        Dim ds As DataSet = dcl.GetDS("SELECT * FROM Cmn_Users_Details WHERE strUserName='" & strUserName & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            UserName = ds.Tables(0).Rows(0).Item("strUserName").ToString
            FullName = ds.Tables(0).Rows(0).Item("strFullName").ToString
            Position = ds.Tables(0).Rows(0).Item("strPosition").ToString
            Mobile = ds.Tables(0).Rows(0).Item("strMobile").ToString
            Email = ds.Tables(0).Rows(0).Item("strEmail").ToString
            Extension = ds.Tables(0).Rows(0).Item("strExtension").ToString
        Else
            UserName = ""
            FullName = ""
            Position = ""
            Mobile = ""
            Email = ""
            Extension = ""
        End If
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "الرئيسية | ملفك الشخصي"

                lblUserName = "اسم الدخول"
                lblFullName = "الاسم الكامل"
                lblPosition = "المنصب"
                lblMobile = "الجوال"
                lblEmail = "البريد"
                lblExtension = "التحويلة"

                btnUpdateProfile = "تحديث البيانات"
                btnChangePassword = "تغيير كلمة المرور"
                btnUploadPicture = "رفع صورة"
            Case Else
                DataLang = "En"
                Title = "Main | Your Profile"

                lblUserName = "Login Name"
                lblFullName = "Full Name"
                lblPosition = "Position"
                lblMobile = "Mobile"
                lblEmail = "Email"
                lblExtension = "Extension"

                btnUpdateProfile = "Update Profile"
                btnChangePassword = "Change Password"
                btnUploadPicture = "Upload Picture"
        End Select
    End Sub

End Class