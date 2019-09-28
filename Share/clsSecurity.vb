Public Class Security
    Public Declare Function desinit Lib "des3w32.dll" (ByVal strKey As String) As Integer
    Public Declare Function ecbencode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Public Declare Function ecbdecode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Const DesKey As String = "$SoftNet"
    Dim dcl As New DCL.Conn.DataClassLayer

    Function AuthenticateUser(ByVal strUserName As String, ByVal strPassword As String) As Int32
        Dim ds As DataSet
        Dim strEncodePW As String = ""
        If Trim(strUserName) <> "" And Trim(strPassword) <> "" Then
            Try
                ds = dcl.GetDS("SELECT * FROM Cmn_Users AS U INNER JOIN Hw_Departments AS D ON U.byteDepartment = D.byteDepartment AND strUserName='" & strUserName & "' AND bAccountStatus=1 AND (datePasswordExpiry>='" & Today.ToString("yyyy-MM-dd") & "' OR datePasswordExpiry Is Null)")
                If ds.Tables(0).Rows.Count > 0 Then
                    desinit(DesKey)
                    strPassword = strPassword.PadRight(8, " ")
                    strEncodePW = strEncodePW.PadRight(8, " ")
                    ecbencode(strEncodePW, strPassword)
                    If strEncodePW = ds.Tables(0).Rows(0).Item("strPassword") Then
                        Return 0
                    Else
                        Return -4
                    End If
                Else
                    Return -3
                End If
            Catch ex As Exception
                Return -2
            End Try
        Else
            Return -1
        End If
    End Function
End Class
