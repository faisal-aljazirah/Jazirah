Imports System.Text

Public Class UI
    Public Enum ModalSize
        XLarge
        Large
        Medium
        Small
        XSmall
    End Enum

    Public Function drawModal(ByVal Header As String, ByVal Body As String, ByVal Footer As String, Optional ByVal Size As ModalSize = ModalSize.Medium, Optional ContentClass As String = "", Optional BodyClass As String = "", Optional FooterClass As String = "") As String
        Dim str As New StringBuilder("")
        Dim mSize As String
        Select Case Size
            Case ModalSize.XLarge
                mSize = "modal-xl"
            Case ModalSize.Large
                mSize = "modal-lg"
            Case ModalSize.Small
                mSize = "modal-sm"
            Case ModalSize.XSmall
                mSize = "modal-xs"
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

    Public Function drowChart(ByVal ElementID As String, ByVal Type As String) As String
        Dim str As String = ""
        str = str & "<script type=""text/javascript"">"
        Dim func As New Share.UI

        Dim opt As New ChartOptions()
        opt.TitleDisplay = True
        opt.TitleText = "82%"
        opt.TitleFullWidth = True

        Dim dt As New ChartData.DataSet()
        dt.Label = "My First dataset"
        dt.Data = "28, 35, 36, 48, 46, 42, 60"
        Dim dt2 As New ChartData.DataSet()
        dt2.Label = "My second dataset"
        dt2.Data = "48, 46, 42, 60, 28, 35, 36"
        dt2.BorderColor = "#ccc"
        dt2.BackgroundColor = "#fff3f2"
        Dim data As New ChartData()
        data.Labels = "'January', 'February', 'March', 'April', 'May', 'June', 'July'"
        data.Datasets = {dt, dt2}

        str = str & "var myOptions = " & opt.getOptionsCreationText()
        str = str & "var myData = " & data.getDataCreationText()
        str = str & "drawChart('" & ElementID & "', '" & Type & "', myOptions, myData);"
        str = str & "</script>"
        Return str
    End Function

    Public Class ChartOptions
        Private _Responsive As Boolean
        Public Property Responsive() As Boolean
            Get
                Return _Responsive
            End Get
            Set(ByVal value As Boolean)
                _Responsive = value
            End Set
        End Property

        Private _MaintainAspectRatio As Boolean
        Public Property MaintainAspectRatio() As Boolean
            Get
                Return _MaintainAspectRatio
            End Get
            Set(ByVal value As Boolean)
                _MaintainAspectRatio = value
            End Set
        End Property

        Private _DatasetStrokeWidth As Integer
        Public Property DatasetStrokeWidth() As Integer
            Get
                Return _DatasetStrokeWidth
            End Get
            Set(ByVal value As Integer)
                _DatasetStrokeWidth = value
            End Set
        End Property

        Private _PointDotStrokeWidth As Integer
        Public Property PointDotStrokeWidth() As Integer
            Get
                Return _PointDotStrokeWidth
            End Get
            Set(ByVal value As Integer)
                _PointDotStrokeWidth = value
            End Set
        End Property

        Private _TooltipFillColor As String
        Public Property TooltipFillColor() As String
            Get
                Return _TooltipFillColor
            End Get
            Set(ByVal value As String)
                _TooltipFillColor = value
            End Set
        End Property

        Private _LegendDisplay As Boolean
        Public Property LegendDisplay() As Boolean
            Get
                Return _LegendDisplay
            End Get
            Set(ByVal value As Boolean)
                _LegendDisplay = value
            End Set
        End Property

        Enum Positions
            Top
            Bottom
            Left
            Right
        End Enum

        Private _LegendPosition As Positions
        Public Property LegendPosition() As Positions
            Get
                Return _LegendPosition
            End Get
            Set(ByVal value As Positions)
                _LegendPosition = value
            End Set
        End Property

        Enum Modes
            Label
        End Enum

        Private _HoverMode As Modes
        Public Property HoverMode() As Modes
            Get
                Return _HoverMode
            End Get
            Set(ByVal value As Modes)
                _HoverMode = value
            End Set
        End Property

        Private _AxesXDisplay As Boolean
        Public Property AxesXDisplay() As Boolean
            Get
                Return _AxesXDisplay
            End Get
            Set(ByVal value As Boolean)
                _AxesXDisplay = value
            End Set
        End Property

        Private _AxesXTickMin As Integer
        Public Property AxesXTickMin() As Integer
            Get
                Return _AxesXTickMin
            End Get
            Set(ByVal value As Integer)
                _AxesXTickMin = value
            End Set
        End Property

        Private _AxesXTickMax As Integer
        Public Property AxesXTickMax() As Integer
            Get
                Return _AxesXTickMax
            End Get
            Set(ByVal value As Integer)
                _AxesXTickMax = value
            End Set
        End Property

        Private _AxesYDisplay As Boolean
        Public Property AxesYDisplay() As Boolean
            Get
                Return _AxesYDisplay
            End Get
            Set(ByVal value As Boolean)
                _AxesYDisplay = value
            End Set
        End Property

        Private _AxesYTickMin As Integer
        Public Property AxesYTickMin() As Integer
            Get
                Return _AxesYTickMin
            End Get
            Set(ByVal value As Integer)
                _AxesYTickMin = value
            End Set
        End Property

        Private _AxesYTickMax As Integer
        Public Property AxesYTickMax() As Integer
            Get
                Return _AxesYTickMax
            End Get
            Set(ByVal value As Integer)
                _AxesYTickMax = value
            End Set
        End Property

        Private _TitleDisplay As Boolean
        Public Property TitleDisplay() As Boolean
            Get
                Return _TitleDisplay
            End Get
            Set(ByVal value As Boolean)
                _TitleDisplay = value
            End Set
        End Property

        Private _TitleFontColor As String
        Public Property TitleFontColor() As String
            Get
                Return _TitleFontColor
            End Get
            Set(ByVal value As String)
                _TitleFontColor = value
            End Set
        End Property

        Private _TitleFontSize As Integer
        Public Property TitleFontSize() As Integer
            Get
                Return _TitleFontSize
            End Get
            Set(ByVal value As Integer)
                _TitleFontSize = value
            End Set
        End Property

        Private _TitleFullWidth As Boolean
        Public Property TitleFullWidth() As Boolean
            Get
                Return _TitleFullWidth
            End Get
            Set(ByVal value As Boolean)
                _TitleFullWidth = value
            End Set
        End Property

        Private _TitleText As String
        Public Property TitleText() As String
            Get
                Return _TitleText
            End Get
            Set(ByVal value As String)
                _TitleText = value
            End Set
        End Property

        Sub New()
            Responsive = True
            MaintainAspectRatio = False
            DatasetStrokeWidth = 3
            PointDotStrokeWidth = 4
            TooltipFillColor = "rgba(0,0,0,0.8)"
            LegendDisplay = False
            LegendPosition = Positions.Bottom
            HoverMode = Modes.Label
            AxesXDisplay = False
            AxesYDisplay = False
            AxesYTickMin = 0
            AxesYTickMax = 70
            TitleDisplay = False
            TitleFontColor = "#FFF"
            TitleFontSize = 40
            TitleFullWidth = True
            TitleText = "82%"
        End Sub

        Function getOptionsCreationText() As String
            Dim script As String = "{"
            Dim temp As String = ""

            script = script & "responsive: " & Responsive.ToString.ToLower & ","
            script = script & "maintainAspectRatio: " & MaintainAspectRatio.ToString.ToLower & ","
            script = script & "datasetStrokeWidth: " & DatasetStrokeWidth & ","
            script = script & "pointDotStrokeWidth: " & PointDotStrokeWidth & ","
            script = script & "tooltipFillColor: '" & TooltipFillColor & "',"
            'Legend
            Select Case LegendPosition
                Case Positions.Top
                    temp = "top"
                Case Positions.Bottom
                    temp = "bottom"
                Case Positions.Left
                    temp = "left"
                Case Positions.Right
                    temp = "right"
            End Select
            script = script & "legend: {display: " & LegendDisplay.ToString.ToLower & "," & "position: '" & temp & "'},"
            'hover
            If HoverMode = Modes.Label Then temp = "label" Else temp = ""
            script = script & "hover: {mode:   '" & temp & "'},"
            'Scales
            script = script & "scales: {xAxes: [{display: " & AxesXDisplay.ToString.ToLower & "}],yAxes: [{display: " & AxesYDisplay.ToString.ToLower & ",ticks: {min: " & AxesYTickMin & ",max: " & AxesYTickMax & "}}]},"
            'Title
            script = script & "title: {display: " & TitleDisplay.ToString.ToLower & ",fontColor: '" & TitleFontColor & "',fontSize: " & TitleFontSize & ",fullWidth: " & TitleFullWidth.ToString.ToLower & ",text: '" & TitleText & "'}"
            '
            script = script & "};"

            Return script
        End Function

    End Class

    Public Class ChartData
        Public Class DataSet
            Private _Label As String
            Public Property Label() As String
                Get
                    Return _Label
                End Get
                Set(ByVal value As String)
                    _Label = value
                End Set
            End Property

            Private _Data As String
            Public Property Data() As String
                Get
                    Return _Data
                End Get
                Set(ByVal value As String)
                    _Data = value
                End Set
            End Property

            Private _BackgroundColor As String
            Public Property BackgroundColor() As String
                Get
                    Return _BackgroundColor
                End Get
                Set(ByVal value As String)
                    _BackgroundColor = value
                End Set
            End Property

            Private _BorderColor As String
            Public Property BorderColor() As String
                Get
                    Return _BorderColor
                End Get
                Set(ByVal value As String)
                    _BorderColor = value
                End Set
            End Property

            Private _BorderWidth As Integer
            Public Property BorderWidth() As Integer
                Get
                    Return _BorderWidth
                End Get
                Set(ByVal value As Integer)
                    _BorderWidth = value
                End Set
            End Property

            Private _StrokeColor As String
            Public Property StrokeColor() As String
                Get
                    Return _StrokeColor
                End Get
                Set(ByVal value As String)
                    _StrokeColor = value
                End Set
            End Property

            Private _CapBezierPoints As Boolean
            Public Property CapBezierPoints() As Boolean
                Get
                    Return _CapBezierPoints
                End Get
                Set(ByVal value As Boolean)
                    _CapBezierPoints = value
                End Set
            End Property

            Private _PointColor As String
            Public Property PointColor() As String
                Get
                    Return _PointColor
                End Get
                Set(ByVal value As String)
                    _PointColor = value
                End Set
            End Property

            Private _PointBorderColor As String
            Public Property PointBorderColor() As String
                Get
                    Return _PointBorderColor
                End Get
                Set(ByVal value As String)
                    _PointBorderColor = value
                End Set
            End Property

            Private _PointBackgroundColor As String
            Public Property PointBackgroundColor() As String
                Get
                    Return _PointBackgroundColor
                End Get
                Set(ByVal value As String)
                    _PointBackgroundColor = value
                End Set
            End Property

            Private _PointBorderWidth As Integer
            Public Property PointBorderWidth() As Integer
                Get
                    Return _PointBorderWidth
                End Get
                Set(ByVal value As Integer)
                    _PointBorderWidth = value
                End Set
            End Property

            Private _PointRadius As Integer
            Public Property PointRadius() As Integer
                Get
                    Return _PointRadius
                End Get
                Set(ByVal value As Integer)
                    _PointRadius = value
                End Set
            End Property

            Sub New()
                Label = ""
                Data = ""
                BackgroundColor = "rgba(45,149,191,0.1)"
                BorderColor = "transparent"
                BorderWidth = 0
                StrokeColor = "#ff6c23"
                CapBezierPoints = True
                PointColor = "#fff"
                PointBorderColor = "rgba(45,149,191,1)"
                PointBackgroundColor = "#FFF"
                PointBorderWidth = 2
                PointRadius = 4
            End Sub

            Public Function getDataValues() As String
                Dim script As String = "{"
                script = script & "label: '" & Label & "',"
                script = script & "data: [" & Data & "],"
                script = script & "backgroundColor: '" & BackgroundColor & "',"
                script = script & "borderColor: '" & BorderColor & "',"
                script = script & "borderWidth: " & BorderWidth & ","
                script = script & "strokeColor: '" & StrokeColor & "',"
                script = script & "capBezierPoints: " & CapBezierPoints.ToString.ToLower & ","
                script = script & "pointColor: '" & PointColor & "',"
                script = script & "pointBorderColor: '" & PointBorderColor & "',"
                script = script & "pointBackgroundColor: '" & PointBackgroundColor & "',"
                script = script & "pointBorderWidth: " & PointBorderWidth & ","
                script = script & "pointRadius: " & PointRadius & ","
                script = script & "}"
                Return script
            End Function

            Public Function getDataValues(ByVal TheLabel As String, ByVal TheData As String) As String
                Label = TheLabel
                Data = TheData
                Dim script As String = "{"
                script = script & "label: '" & Label & "',"
                script = script & "data: [" & Data & "],"
                script = script & "backgroundColor: '" & BackgroundColor & "',"
                script = script & "borderColor: '" & BorderColor & "',"
                script = script & "borderWidth: " & BorderWidth & ","
                script = script & "strokeColor: '" & StrokeColor & "',"
                script = script & "capBezierPoints: " & CapBezierPoints.ToString.ToLower & ","
                script = script & "pointColor: '" & PointColor & "',"
                script = script & "pointBorderColor: '" & PointBorderColor & "',"
                script = script & "pointBackgroundColor: '" & PointBackgroundColor & "',"
                script = script & "pointBorderWidth: " & PointBorderWidth & ","
                script = script & "pointRadius: " & PointRadius & ","
                script = script & "}"
                Return script
            End Function
        End Class

        Private _Labels As String
        Public Property Labels() As String
            Get
                Return _Labels
            End Get
            Set(ByVal value As String)
                _Labels = value
            End Set
        End Property

        Private _Datasets As DataSet()
        Public Property Datasets() As DataSet()
            Get
                Return _Datasets
            End Get
            Set(ByVal value As DataSet())
                _Datasets = value
            End Set
        End Property

        Sub New()
            Labels = ""
            Datasets = {}
        End Sub

        Public Function getDataCreationText() As String
            Dim script As String = "{"
            script = script & "labels: [" & Labels & "],"

            script = script & "datasets: ["
            For Each ds As DataSet In DataSets
                script = script & ds.getDataValues & ","
            Next
            script = script & "]"
            script = script & "};"
            Return script
        End Function

    End Class
End Class
