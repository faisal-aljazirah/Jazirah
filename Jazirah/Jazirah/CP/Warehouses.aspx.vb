Public Class Warehouses
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public byteWarehouse, strWarehouseEn, strWarehouseAr, strKeeper, strTelephone, strAddress, intCenter, bExpiryControlled, upsize_ts, bActive As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Remove this line after complete user login and settings
        '=>
        'Session("UserID") = 1
        'Session("UserLanguage") = 2
        '<=

        'TODO: check authorization
        'TODO: load system settings
        'TODO: load user settings
        UserLanguage = Session("UserLanguage")
        'TODO: get data

        loadLanguage()

        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Stock_Warehouses")
        repDepartments.DataSource = ds
        repDepartments.DataBind()
    End Sub


    Function showStatus(ByVal TransNo As Long, ByVal Status As Integer) As String
        If Status = 1 Then
            Return ""
        Else
            Return "<button type=""button"" onclick=""javascript:showOrder(" & TransNo & ");"" class=""btn btn-sm btn-primary""> " & View & " </button>"
        End If
    End Function

    Sub loadLanguage()
        Select Case UserLanguage
            Case 2
                DataLang = "Ar"
                Title = "لوحة التحكم | المستودع"
                PageHeader.InnerText = "المستودع"
                'Variables

                'Columns
                byteWarehouse = "الرقم"
                strWarehouseEn = "المستودع عربي"
                strWarehouseAr = "المستودع إنجليزي"
                strKeeper = "المحافظ"
                strTelephone = "الهاتف"
                strAddress = "العنوان"
                intCenter = "رقم المركز"
                bExpiryControlled = "التاريخ"
                upsize_ts = "الحجم"
                bActive = "النشاط"


            Case Else
                DataLang = "En"
                Title = "Control Panel | Warehouses"
                PageHeader.InnerText = "Warehouses"
                'Variables

                'Columns
                byteWarehouse = "No"
                strWarehouseEn = "Warehouses Arabic"
                strWarehouseAr = "Warehouses English"
                strKeeper = "Keeper"
                strTelephone = "Telephone "
                strAddress = "Address"
                intCenter = "Center"
                bExpiryControlled = "Date"
                upsize_ts = "Size"
                bActive = "Active"
        End Select
    End Sub
End Class