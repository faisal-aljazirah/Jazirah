Imports System.IO
Imports System.Security.Cryptography

Public Class Security
    Public Declare Function desinit Lib "des3w32.dll" (ByVal strKey As String) As Integer
    Public Declare Function ecbencode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Public Declare Function ecbdecode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Const DesKey As String = "$SoftNet"
    Dim dcl As New DCL.Conn.DataClassLayer

    Function AuthenticateUser(ByVal strUserName As String, ByVal strPassword As String) As String
        Dim ds As DataSet
        Dim enc As New Encryption
        Dim strEncodePW As String = ""
        If Trim(strUserName) <> "" And Trim(strPassword) <> "" Then
            Try
                ds = dcl.GetDS("SELECT U.strUserName, U.strPassword, UD.strEPassword FROM Cmn_Users AS U INNER JOIN Hw_Departments AS D ON U.byteDepartment = D.byteDepartment LEFT JOIN Cmn_Users_Details AS UD ON U.strUserName=UD.strUserName WHERE U.strUserName='" & strUserName & "' AND bAccountStatus=1 AND (datePasswordExpiry>='" & Today.ToString("yyyy-MM-dd") & "' OR datePasswordExpiry Is Null)")
                If ds.Tables(0).Rows.Count > 0 Then
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strEPassword")) Then
                        desinit(DesKey)
                        Dim strPlainPW As String = strPassword.PadRight(8, " ")
                        'strPassword = strPassword.PadRight(8, " ")
                        strEncodePW = strEncodePW.PadRight(8, " ")
                        ecbencode(strEncodePW, strPlainPW)
                        If strEncodePW = ds.Tables(0).Rows(0).Item("strPassword") Then
                            dcl.ExecSQuery("INSERT INTO Cmn_Users_Details (strUserName, strEPassword) VALUES ('" & strUserName & "', '" & enc.Encrypt(strPassword) & "')")
                            Return ds.Tables(0).Rows(0).Item("strUserName").ToString
                        Else
                            Return "4"
                        End If
                    Else
                        Dim str1 As String = enc.Encrypt(strPassword)
                        Dim str2 As String = ds.Tables(0).Rows(0).Item("strEPassword")
                        If enc.Encrypt(strPassword) = ds.Tables(0).Rows(0).Item("strEPassword") Then
                            Return ds.Tables(0).Rows(0).Item("strUserName").ToString
                        Else
                            Return "4"
                        End If
                    End If
                Else
                    Return "3"
                End If
            Catch ex As Exception
                Return "2"
            End Try
        Else
            Return "1"
        End If
    End Function

    Public Function ChangePassword(ByVal UserName As String, ByVal OldPassword As String, ByVal NewPassword As String) As String
        Dim ds As DataSet
        Dim enc As New Encryption
        Dim strEncodePW As String = ""

        If Trim(UserName) <> "" And Trim(OldPassword) <> "" And Trim(NewPassword) <> "" Then
            Try
                ds = dcl.GetDS("SELECT U.strUserName, U.strPassword, UD.strEPassword FROM Cmn_Users AS U INNER JOIN Hw_Departments AS D ON U.byteDepartment = D.byteDepartment LEFT JOIN Cmn_Users_Details AS UD ON U.strUserName=UD.strUserName WHERE U.strUserName='" & UserName & "' AND bAccountStatus=1 AND (datePasswordExpiry>='" & Today.ToString("yyyy-MM-dd") & "' OR datePasswordExpiry Is Null)")
                If ds.Tables(0).Rows.Count > 0 Then
                    If Not (IsDBNull(ds.Tables(0).Rows(0).Item("strEPassword"))) Then
                        If enc.Encrypt(OldPassword) = ds.Tables(0).Rows(0).Item("strEPassword") Then
                            'validate password policy
                            If Len(NewPassword) >= 3 Then
                                Dim news As String = enc.Encrypt(NewPassword)
                                dcl.ExecScalar("UPDATE Cmn_Users_Details SET strEPassword='" & enc.Encrypt(NewPassword) & "' WHERE strUserName='" & UserName & "'")
                                Dim usr As New Share.User
                                usr.AddLog(UserName, Now, 0, "Profile", 0, 2, "Change Password")
                                Return "<script>msg('','Your password has changed successfully!','success');$('#mdlConfirm').modal('hide');</script>"
                            Else
                                Return "Err:Your new password must be greater than 3 digits"
                            End If
                        Else
                            Return "Err:Wrong password"
                        End If
                    Else
                        Return "Err:Your password not updated, please logout and login again"
                    End If
                Else
                    Return "Err:User cannot be found"
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Missing data"
        End If
    End Function

    Class Encryption
        Private enc As System.Text.UTF8Encoding
        Private encryptor As ICryptoTransform
        Private decryptor As ICryptoTransform

        Sub New()
            Dim KEY_128 As Byte() = {42, 1, 52, 67, 231, 13, 94, 101, 123, 6, 0, 12, 32, 91, 4, 111, 31, 70, 21, 141, 123, 142, 234, 82, 95, 129, 187, 162, 12, 55, 98, 23}
            Dim IV_128 As Byte() = {234, 12, 52, 44, 214, 222, 200, 109, 2, 98, 45, 76, 88, 53, 23, 78}
            Dim symmetricKey As RijndaelManaged = New RijndaelManaged()
            symmetricKey.Mode = CipherMode.CBC

            Me.enc = New System.Text.UTF8Encoding
            Me.encryptor = symmetricKey.CreateEncryptor(KEY_128, IV_128)
            Me.decryptor = symmetricKey.CreateDecryptor(KEY_128, IV_128)
        End Sub

        Public Function Encrypt(ByVal PlainText As String)
            Dim CypherText As String = ""
            If Not String.IsNullOrEmpty(PlainText) Then
                Dim memoryStream As MemoryStream = New MemoryStream()
                Dim cryptoStream As CryptoStream = New CryptoStream(memoryStream, Me.encryptor, CryptoStreamMode.Write)
                cryptoStream.Write(Me.enc.GetBytes(PlainText), 0, PlainText.Length)
                cryptoStream.FlushFinalBlock()
                CypherText = Convert.ToBase64String(memoryStream.ToArray())
                memoryStream.Close()
                cryptoStream.Close()
            End If
            Return CypherText
        End Function

        Public Function Decrypt(ByVal CypherText As String)
            Dim cypherTextBytes As Byte() = Convert.FromBase64String(CypherText)
            Dim memoryStream As MemoryStream = New MemoryStream(cypherTextBytes)
            Dim cryptoStream As CryptoStream = New CryptoStream(memoryStream, Me.decryptor, CryptoStreamMode.Read)
            Dim plainTextBytes(cypherTextBytes.Length) As Byte
            Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)
            memoryStream.Close()
            cryptoStream.Close()
            Return Me.enc.GetString(plainTextBytes, 0, decryptedByteCount)
        End Function
    End Class
End Class
