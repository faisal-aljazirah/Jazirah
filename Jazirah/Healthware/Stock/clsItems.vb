Namespace Stock
    Public Class Items
        Dim dcl As New DCL.Conn.DataClassLayer

        Public Function getItems(Optional strItem As String = "", Optional ByVal strItemName As String = "", Optional ByVal intGroup As Integer = 0, Optional ByVal InUse As Boolean = True) As DataSet
            Try
                Dim Where As String = ""
                If strItem <> "" Then Where = Where & " AND I.strItem='" & strItem & "'"
                If strItemName <> "" Then Where = Where & " AND (I.strItemEn LIKE '%" & strItemName & "%' OR  I.strItemAr LIKE '%" & strItemName & "%')"
                If intGroup <> 0 Then Where = Where & " AND I.intGroup=" & intGroup
                Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Items AS I INNER JOIN Stock_Items_Details AS ID ON I.strItem=ID.strItem INNER JOIN Stock_Groups AS G ON I.intGroup=G.intGroup WHERE I.strItem<>'' " & Where)
                Return ds
            Catch ex As Exception
                Throw ex
            End Try
        End Function
    End Class
End Namespace
