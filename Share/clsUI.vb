Imports System.Text

Public Class UI
    Public Enum ModalSize
        Large
        Medium
        Small
    End Enum

    Public Function drawModal(ByVal Header As String, ByVal Body As String, ByVal Footer As String, Optional ByVal Size As ModalSize = ModalSize.Medium, Optional ContentClass As String = "", Optional BodyClass As String = "", Optional FooterClass As String = "") As String
        Dim str As New StringBuilder("")
        Dim mSize As String
        Select Case Size
            Case ModalSize.Large
                mSize = "modal-lg"
            Case ModalSize.Small
                mSize = "modal-sm"
            Case Else
                mSize = ""
        End Select

        str.Append("<div class=""modal-dialog " & mSize & """ role=""document""><div class=""modal-content " & ContentClass & """>")
        str.Append("<div class=""modal-header""><button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button><h4 class=""modal-title"" id=""modalTitle"">" & Header & "</h4></div>")
        str.Append("<div class=""modal-body " & BodyClass & """>" & Body & "</div>")
        If Footer <> "" Then str.Append("<div class=""modal-footer " & FooterClass & """>" & Footer & "</div>")
        str.Append("</div></div>")
        Return str.ToString
    End Function


End Class
