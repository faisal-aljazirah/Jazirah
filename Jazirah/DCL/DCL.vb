Imports System.IO
Imports System.Reflection
Imports System.Configuration
Imports System.Data
Imports System.Xml

Namespace Conn

    '----------------------------------------------------------
    'Notes
    'Data Access Tier for SANG All programs
    '----------------------------------------------------------
    Public Class DataClassLayer
        Dim Con As New System.Data.SqlClient.SqlConnection
        Dim ConnectionString As String
        Dim appString As String = System.Configuration.ConfigurationManager.ConnectionStrings("ApplicationServices").ConnectionString

        'Sub New(ByVal key As String)
        '    ConnectionString = key
        'End Sub

        '=====================================================================
        ' purpose : Connect to database
        ' input : nothing
        ' output : true if connection ok else return false
        '------------------------------------
        Private Function Connect() As Boolean
            Dim Value As Boolean = True


            If Con.State = ConnectionState.Open Then
                Con.Close()
            End If
            Con.ConnectionString = appString
            Try
                Con.Open()
            Catch ex As Exception
                Value = False
                Throw ex
            End Try
            Connect = Value
        End Function
        '=====================================================================


        '=====================================================================
        ' purpose : This function receives as array of sql select statements and then builds a single
        '                dataset with these and then returns the dataset
        ' input : array of select statments
        ' output : dataset of execute all statment in an array or Exception
        '------------------------------------
        Public Function GetDS(ByVal QStr() As String) As System.Data.DataSet
            Dim NewDS As New System.Data.DataSet
            Dim MyaDapt As New System.Data.SqlClient.SqlDataAdapter
            Dim Mycmd As New System.Data.SqlClient.SqlCommand
            Dim Index As Int16
            If Not QStr Is Nothing Then
                Try
                    Connect()
                    Mycmd.Connection = Con
                    Mycmd.CommandType = CommandType.Text
                    For Index = 0 To QStr.GetUpperBound(0)
                        If QStr(Index).Length > 0 Then
                            Mycmd.CommandText = QStr(Index)
                            MyaDapt.SelectCommand = Mycmd
                            MyaDapt.Fill(NewDS, "Table" & CType(Index, String))
                        End If
                    Next
                    GetDS = NewDS
                Catch ex As Exception
                    Throw ex
                Finally
                    If Con.State = ConnectionState.Open Then
                        Con.Close()
                    End If
                End Try
            End If
        End Function
        '=====================================================================

        '=====================================================================
        ' purpose : This function receives as single sql select statement and then executes it 
        '                to return a dataset with a single table
        ' input : a select statment
        ' output : dataset of execute statment or Exception
        '------------------------------------
        Public Function GetDS(ByVal QStr As String) As System.Data.DataSet
            Dim NewDS As New System.Data.DataSet
            Dim MyaDapt As New System.Data.SqlClient.SqlDataAdapter
            Dim Mycmd As New System.Data.SqlClient.SqlCommand

            Try
                Connect()
                Mycmd.Connection = Con
                Mycmd.CommandType = CommandType.Text
                Mycmd.CommandText = QStr
                MyaDapt.SelectCommand = Mycmd
                MyaDapt.Fill(NewDS, "Table1")
                GetDS = NewDS
            Catch ex As Exception
                Throw ex
            Finally
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function

        '=> Created by: Faisal Al-Aseery
        Public Function GetDS(ByVal Table As String, Optional ByVal Where As String = "", Optional ByVal Order As String = "") As System.Data.DataSet
            Dim NewDS As New System.Data.DataSet
            Dim MyaDapt As New System.Data.SqlClient.SqlDataAdapter
            Dim Mycmd As New System.Data.SqlClient.SqlCommand
            Dim WhereStr As String = ""
            If Where <> "" Then WhereStr = " WHERE " & Where
            Dim OrderStr As String = ""
            If Order <> "" Then OrderStr = " ORDER BY " & Order
            Try
                Connect()
                Mycmd.Connection = Con
                Mycmd.CommandType = CommandType.Text
                Mycmd.CommandText = "SELECT * FROM table" & WhereStr & OrderStr
                MyaDapt.SelectCommand = Mycmd
                MyaDapt.Fill(NewDS, "Table1")
                GetDS = NewDS
            Catch ex As Exception
                Throw ex
            Finally
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function
        '=====================================================================

        '=====================================================================
        ' purpose : This function receives as single insert\update\delete statement to executes it 
        ' input : a select statment
        ' output : the number of rows affected
        '------------------------------------
        Public Function ExecSQuery(ByVal QueryString As String) As Int16
            Dim Cmd As New System.Data.SqlClient.SqlCommand
            Try
                Connect()
                Cmd.Connection = Con
                Cmd.CommandType = CommandType.Text
                Cmd.CommandText = QueryString
                ExecSQuery = Cmd.ExecuteNonQuery()
            Catch ex As Exception
                ExecSQuery = -2
                Throw ex
            Finally
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function
        '=====================================================================
        '=====================================================================
        ' Updated by me
        '------------------------------------
        Public Function ExecIQuery(ByVal QueryString As String) As Long
            Dim Cmd As New System.Data.SqlClient.SqlCommand
            Try
                Connect()
                Cmd.Connection = Con
                Cmd.CommandType = CommandType.Text
                Cmd.CommandText = QueryString
                Cmd.ExecuteNonQuery()
                Cmd.CommandText = "Select @@Identity"
                ExecIQuery = Cmd.ExecuteScalar()
            Catch ex As Exception
                ExecIQuery = -2
                Throw ex
            Finally
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function
        '=====================================================================

        '=====================================================================
        ' purpose : This function will take an array of insert\update\delete statements to
        '               execute them one at a time within a transaction. 
        '               If any one statement fails - the transaction is aborted.
        ' input : a array of select statments
        ' output : ??
        '------------------------------------
        Public Function ExecSQuery(ByVal QueryString() As String) As Int16
            Dim Index As Integer
            ' Dim Recs As Integer
            Dim Cmd As System.Data.SqlClient.SqlCommand

            If Not QueryString Is Nothing Then
                Try
                    Cmd = TransConnect()
                    For Index = 0 To QueryString.GetUpperBound(0)
                        If Not QueryString(Index) Is Nothing Then
                            If QueryString(Index).Length > 0 Then
                                Cmd.CommandText = QueryString(Index)
                                ExecSQuery = TransExecSQuery(Cmd)
                            End If
                        End If
                    Next
                    TransCommit(Cmd)
                Catch ex As Exception
                    ExecSQuery = -2
                    TransRollback(Cmd)
                    Throw ex
                Finally
                    TransEnd(Cmd)
                End Try
            End If
        End Function
        '=====================================================================

        '=====================================================================
        ' purpose : This function will take an array of insert\update\delete statements to
        '               execute them one at a time within a transaction. 
        '               If any one statement fails - the transaction is aborted.
        ' input : a array of select statments
        ' output : ??
        '------------------------------------
        Public Function ExecScalar(ByVal QueryString As String) As Object
            Dim Cmd As New System.Data.SqlClient.SqlCommand
            Try
                Connect()
                Cmd.Connection = Con
                Cmd.CommandText = QueryString
                ExecScalar = Cmd.ExecuteScalar
            Catch ex As Exception
                Throw ex
            Finally
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function

        Public Function TransConnect() As System.Data.SqlClient.SqlCommand
            Dim MyCommand As New System.Data.SqlClient.SqlCommand
            Dim MyTran As System.Data.SqlClient.SqlTransaction
            Try
                Connect()
                MyTran = Con.BeginTransaction
                MyCommand.Connection = Con
                MyCommand.Transaction = MyTran
                MyCommand.CommandType = CommandType.Text
                TransConnect = MyCommand
            Catch ex As Exception
                Throw ex
            End Try
        End Function
        Public Function TransGetDS(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal QStr As String) As System.Data.DataSet

            Dim NewDS As New System.Data.DataSet
            Dim MyaDapt As New System.Data.SqlClient.SqlDataAdapter
            Dim Mycmd As New System.Data.SqlClient.SqlCommand
            Try
                iMyCommand.CommandText = QStr
                MyaDapt.SelectCommand = iMyCommand
                MyaDapt.Fill(NewDS, "Table1")
                Return NewDS
            Catch ex As Exception
                Return NewDS
                Throw ex
            Finally
                If iMyCommand.Connection.State = ConnectionState.Open Then
                    ' iMyCommand.Connection.Close()
                End If
            End Try
        End Function
        Public Function TransExecScalar(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal QueryString As String) As Object
            Try
                iMyCommand.CommandText = QueryString
                TransExecScalar = iMyCommand.ExecuteScalar
            Catch ex As Exception
                Throw ex
            End Try
        End Function
        Public Function TransExecSQuery(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal QueryString As String) As Int16
            Dim Recs As Integer
            Try
                iMyCommand.CommandText = QueryString
                Recs = iMyCommand.ExecuteNonQuery()
                Return Recs
            Catch ex As Exception
                Throw ex
                Return -2
            End Try
        End Function
        Public Function TransExecSQuery(ByRef iMyCommand As System.Data.SqlClient.SqlCommand) As Int16
            Dim Recs As Integer
            Try
                Recs = iMyCommand.ExecuteNonQuery()
                Return Recs
            Catch ex As Exception
                Throw ex
                Return -2
            End Try
        End Function
        Public Function TransEnd(ByRef iMyCommand As System.Data.SqlClient.SqlCommand) As Boolean
            If iMyCommand.Connection Is Nothing Then Exit Function
            If iMyCommand.Connection.State = ConnectionState.Open Then
                iMyCommand.Connection.Close()
            End If
        End Function
        Public Function TransRollback(ByRef iMyCommand As System.Data.SqlClient.SqlCommand) As Boolean
            If iMyCommand.Transaction Is Nothing Then Exit Function
            iMyCommand.Transaction.Rollback()
        End Function
        Public Function TransCommit(ByRef iMyCommand As System.Data.SqlClient.SqlCommand) As Boolean
            iMyCommand.Transaction.Commit()
        End Function
        '--------------------------------------------------------
        Public Function ConnectForBatch() As System.Data.SqlClient.SqlCommand
            ' Con.ConnectionString = "Provider = SQLOLEDB;Data Source = paranor;Initial Catalog = Northwind;integrated Security = SSPI"
            Dim Value As Boolean = True
            ' Dim appString As String
            Dim MyCon As New System.Data.SqlClient.SqlConnection
            Dim MyCommand As New System.Data.SqlClient.SqlCommand
            Dim MyTran As System.Data.SqlClient.SqlTransaction
            'Read connection string from the Machine.Config file AppSettings Section

            '  appString = ConfigurationSettings.AppSettings(ConnectionString)

            If MyCon.State = ConnectionState.Open Then
                MyCon.Close()
            End If
            MyCon.ConnectionString = appString
            Try
                MyCon.Open()
                MyTran = MyCon.BeginTransaction
                MyCommand.Connection = MyCon
                MyCommand.Transaction = MyTran
                MyCommand.CommandType = CommandType.Text
            Catch ex As Exception
                Throw ex
                Value = False
            End Try
            If Value = True Then
                ConnectForBatch = MyCommand
            Else
                ConnectForBatch = Nothing
            End If
        End Function
        Public Function ExecSQueryForBatch(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal QueryString As String) As Int16
            'This function receives as single insert\update\delete statement,
            'executes it and then returns the number of rows affected
            Dim Recs As Integer

            Try
                iMyCommand.CommandText = QueryString
                Recs = iMyCommand.ExecuteNonQuery()
                Return Recs
            Catch ex As Exception
                Throw ex
                Return -2
            End Try

        End Function
        Public Function GetDSForBatch(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal QStr As String) As System.Data.DataSet
            'This function receives as single sql select statement and then executes it 
            'to return a dataset with a single table

            Dim NewDS As New System.Data.DataSet
            Dim MyaDapt As New System.Data.SqlClient.SqlDataAdapter

            Try
                iMyCommand.CommandText = QStr
                MyaDapt.SelectCommand = iMyCommand
                MyaDapt.Fill(NewDS, "Table1")
                Return NewDS
            Catch ex As Exception
                Throw ex
                Return NewDS
            End Try

        End Function
        Function EndBatchConnection(ByRef iMyCommand As System.Data.SqlClient.SqlCommand, ByVal TranStatus As Boolean) As Boolean
            EndBatchConnection = True
            Try
                If TranStatus = True Then
                    iMyCommand.Transaction.Commit()
                Else
                    iMyCommand.Transaction.Rollback()
                End If
            Catch
                EndBatchConnection = False
            End Try
        End Function

        Public Function GetSchema(ByVal QStr As String) As DataTable
            'This function receives as single sql select statement and then executes it 
            'to return a dataset with a single table
            Dim rd As SqlClient.SqlDataReader
            Dim cmd As New SqlClient.SqlCommand
            Connect()
            cmd.Connection = Con
            cmd.CommandText = "select * table"
            rd = cmd.ExecuteReader(CommandBehavior.SchemaOnly)
            GetSchema = rd.GetSchemaTable
            Con.Close()
        End Function


        '=====================================================================
        ' purpose : check number of rows in select statment
        ' input : select statment
        ' output : -2 if statment not correct no of rows as integer
        '------------------------------------
        Public Function CheckDSrows(ByVal Qstr As String) As Integer
            Dim cmd As New System.Data.SqlClient.SqlCommand
            Dim TStr As String = CountStr(Qstr)
            Dim Rows As Integer
            Try
                If UCase(TStr) <> "NOT SQL" Then
                    If Connect() = True Then
                        cmd.Connection = Con
                        cmd.CommandType = CommandType.Text
                        cmd.CommandText = TStr
                        Rows = cmd.ExecuteScalar()
                        Con.Close()
                    Else
                        Rows = -2                                              'Connection failed
                    End If
                Else
                    Rows = -1                                               'Invalid SQL statement
                End If

            Catch ex As Exception
                CheckDSrows = -1
            Finally
                CheckDSrows = Rows
                cmd = Nothing
                If Con.State = ConnectionState.Open Then
                    Con.Close()
                End If
            End Try
        End Function
        '=====================================================================

        '=====================================================================
        ' purpose : This function will take a sql select statement and remove everything before the "from"
        '                keyword and the prefix "select count(*) in order to return the number of rows this query 
        '                would return
        ' input : select statment
        ' output : "NOT SQL" if there is an error in sql statment or correct sql statment
        '------------------------------------
        Private Function CountStr(ByVal Qstr As String) As String
            Dim Pos As Integer = InStr(UCase(Qstr), "FROM")
            If Pos > 0 Then
                Return "Select count(*) " & Mid(Qstr, Pos)
            Else
                Return "NOT SQL"
            End If

        End Function

    End Class

    Public Class XMLData
        Dim doc As New XmlDocument

        Private Function Load(ByVal XMLFile As String) As Boolean
            Try
                doc.Load(XMLFile)
            Catch ex As Exception
                Throw ex
            End Try
        End Function


        Public Function GetDataCount(ByVal XMLFile As String, ByVal NodeName As String, Optional Attribute As String = "") As Integer
            Try
                Load(XMLFile)
                Dim Condition As String = ""
                If Attribute <> "" Then Condition = "[" & Attribute & "]"
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes(NodeName & Condition)
                Return nodes.Count
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function GetDataList(ByVal XMLFile As String, ByVal NodeName As String, Optional Attribute As String = "") As XmlNodeList
            Try
                Load(XMLFile)
                Dim Condition As String = ""
                If Attribute <> "" Then Condition = "[" & Attribute & "]"
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes(NodeName & Condition)
                Return nodes
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function CheckExistElement(ByVal XMLFile As String, ByVal NodeName As String, ByVal ElementName As String, ByVal ElementValue As String, Optional Attribute As String = "") As Boolean
            Try
                Dim Found As Boolean = False
                Load(XMLFile)
                Dim Condition As String = ""
                If Attribute <> "" Then Condition = "[" & Attribute & "]"
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes(NodeName & Condition)
                For Each node As XmlNode In nodes
                    If node.SelectSingleNode(ElementName).InnerText = ElementValue Then Found = True
                Next
                Return Found
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        Public Function CheckExistNode(ByVal XMLFile As String, ByVal NodeName As String, ByVal ElementName As String, ByVal ElementValue As String, Optional Attribute As String = "") As Boolean
            Try
                Dim Found As Boolean = False
                Load(XMLFile)
                Dim Condition As String = ""
                If Attribute <> "" Then Condition = "[" & Attribute & "]"
                Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes(NodeName & "/" & ElementName & Condition)
                For Each node As XmlNode In nodes
                    If node.InnerText = ElementValue Then Found = True
                Next
                Return Found
            Catch ex As Exception
                Throw ex
            End Try
        End Function
    End Class

End Namespace
