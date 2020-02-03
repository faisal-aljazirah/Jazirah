Module Core
    Dim dcl As New DCL.Conn.DataClassLayer

    Public Class Stock
        Public Class Items

            Public Function getItems() As DataSet
                Try
                    Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Items AS I INNER JOIN Stock_Items_Details AS ID ON I.strItem=ID.strItem")
                    Return ds
                Catch ex As Exception
                    Throw ex
                End Try
            End Function
        End Class

        Public Class Warehouses

        End Class

        Public Class Suppliers

        End Class
    End Class
End Module
