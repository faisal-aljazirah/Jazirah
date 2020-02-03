Imports System.Xml
Imports System.Web

Public Class Transfer
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
    Dim byteWarehouse As Byte

    Dim TransferApprove, SuspendExpiredApprove, SuspendReturnedApprove As Boolean
    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, DirectChangeTransfer As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim TransferLimitDays As Byte

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
        LoadPermissions()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Transfer", "User", strUserName)
    End Sub

    Dim BalanceString As String = ""
    Dim BalanceList As String()
    Dim TransferString As String = ""
    Dim TransferList As String()
    Dim SExpiredString As String = ""
    Dim SExpiredList As String()
    Dim SReturnedString As String = ""
    Dim SReturnedList As String()

    Private Sub LoadPermissions()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim balance As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Balance")
        For Each w As XmlNode In balance
            BalanceString = BalanceString & w.InnerText & ","
        Next
        If BalanceString <> "" Then
            BalanceString = Left(BalanceString, BalanceString.Length - 1)
            BalanceList = Split(BalanceString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim transfer As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Transfer")
        For Each w As XmlNode In transfer
            TransferString = TransferString & w.InnerText & ","
        Next
        If TransferString <> "" Then
            TransferString = Left(TransferString, TransferString.Length - 1)
            TransferList = Split(TransferString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim suspend_expire As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Suspend_Expired")
        For Each w As XmlNode In suspend_expire
            SExpiredString = SExpiredString & w.InnerText & ","
        Next
        If SExpiredString <> "" Then
            SExpiredString = Left(SExpiredString, SExpiredString.Length - 1)
            SExpiredList = Split(SExpiredString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim suspend_return As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Suspend_Returned")
        For Each w As XmlNode In suspend_return
            SReturnedString = SReturnedString & w.InnerText & ","
        Next
        If SReturnedString <> "" Then
            SReturnedString = Left(SReturnedString, SReturnedString.Length - 1)
            SReturnedList = Split(SReturnedString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
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

        If application.SelectSingleNode("DirectChangeTransfer") Is Nothing Then DirectChangeTransfer = False Else DirectChangeTransfer = application.SelectSingleNode("DirectChangeTransfer").InnerText

        If application.SelectSingleNode("PopupToPrint") Is Nothing Then PopupToPrint = True Else PopupToPrint = application.SelectSingleNode("PopupToPrint").InnerText
        If application.SelectSingleNode("PrintDose") Is Nothing Then PrintDose = 3 Else PrintDose = application.SelectSingleNode("PrintDose").InnerText
        If application.SelectSingleNode("PrintInvoice") Is Nothing Then PrintInvoice = 2 Else PrintInvoice = application.SelectSingleNode("PrintInvoice").InnerText
        If application.SelectSingleNode("DosePrinter") Is Nothing Then DosePrinter = "ZDesigner GK420t" Else DosePrinter = application.SelectSingleNode("DosePrinter").InnerText
        If application.SelectSingleNode("InvoicePrinter") Is Nothing Then InvoicePrinter = "HP LaserJet Professional P1566" Else InvoicePrinter = application.SelectSingleNode("InvoicePrinter").InnerText
        If application.SelectSingleNode("TaxEnabled") Is Nothing Then TaxEnabled = True Else TaxEnabled = application.SelectSingleNode("TaxEnabled").InnerText

        If application.SelectSingleNode("TransferLimitDays") Is Nothing Then TransferLimitDays = 4 Else TransferLimitDays = application.SelectSingleNode("TransferLimitDays").InnerText
        If application.SelectSingleNode("TransferApprove") Is Nothing Then TransferApprove = True Else TransferApprove = application.SelectSingleNode("TransferApprove").InnerText
        If application.SelectSingleNode("SuspendExpiredApprove") Is Nothing Then SuspendExpiredApprove = True Else SuspendExpiredApprove = application.SelectSingleNode("SuspendExpiredApprove").InnerText
        If application.SelectSingleNode("SuspendReturnedApprove") Is Nothing Then SuspendReturnedApprove = True Else SuspendReturnedApprove = application.SelectSingleNode("SuspendReturnedApprove").InnerText

        If byteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""

        'byteWarehouse = 3
        Dim dsWarehouse As DataSet
        dsWarehouse = dcl.GetDS("SELECT * FROM Cmn_Users WHERE strUserName='" & strUserName & "'")
        If dsWarehouse.Tables(0).Rows.Count > 0 Then
            If IsDBNull(dsWarehouse.Tables(0).Rows(0).Item("byteWarehouse")) Then
                Throw New Exception("Wharehouse not assigned")
            Else
                byteWarehouse = dsWarehouse.Tables(0).Rows(0).Item("byteWarehouse")
            End If
        Else
            Throw New Exception("Wharehouse not assigned")
        End If
        'byteWarehouse = 3
    End Sub
End Class
