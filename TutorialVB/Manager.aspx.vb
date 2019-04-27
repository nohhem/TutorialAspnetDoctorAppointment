Imports System
Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.ServiceModel.Channels
Imports DayPilot.Web.Ui
Imports DayPilot.Web.Ui.Enums
Imports DayPilot.Web.Ui.Events
Imports DayPilot.Web.Ui.Events.Scheduler
Imports CommandEventArgs = DayPilot.Web.Ui.Events.CommandEventArgs

Partial Public Class Manager
	Inherits System.Web.UI.Page

	Private Const MorningShiftStarts As Integer = 9
	Private Const MorningShiftEnds As Integer = 13

	Private Const AfternoonShiftStarts As Integer = 14
	Private Const AfternoonShiftEnds As Integer = 18

	Private Const showSeparators As Boolean = False

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

		LoadDoctors()

		If Not IsPostBack Then
			DayPilotScheduler1.StartDate = New Date(Date.Today.Year, Date.Today.Month, 1)
			DayPilotScheduler1.Days = Date.DaysInMonth(Date.Today.Year, Date.Today.Month)
			DayPilotScheduler1.SetScrollX(Date.Today)
			LoadTimeline()
			LoadAppointments()
		End If


	End Sub

	Private Sub LoadDoctors()
		Dim doctors As DataTable = Db.LoadDoctors()

		DayPilotScheduler1.Resources.Clear()
		For Each row As DataRow In doctors.Rows
			DayPilotScheduler1.Resources.Add(DirectCast(row("DoctorName"), String), Convert.ToString(row("DoctorId")))
		Next row
	End Sub


	Private Sub LoadAppointments()
		Dim table As DataTable = Db.LoadAppointments(DayPilotScheduler1.VisibleStart, DayPilotScheduler1.VisibleEnd)
		DayPilotScheduler1.DataSource = table
		DayPilotScheduler1.DataIdField = "AppointmentId"
		DayPilotScheduler1.DataTextField = "AppointmentPatientName"
		DayPilotScheduler1.DataStartField = "AppointmentStart"
		DayPilotScheduler1.DataEndField = "AppointmentEnd"
		DayPilotScheduler1.DataTagFields = "AppointmentStatus"
		DayPilotScheduler1.DataResourceField = "DoctorId"
		DayPilotScheduler1.DataBind()
		DayPilotScheduler1.Update()
	End Sub

	Private ReadOnly Property ScaleResolved() As String
		Get
			Dim scaleSize As String = CStr(DayPilotScheduler1.ClientState("size"))

			If String.IsNullOrEmpty(scaleSize) Then
				scaleSize = "shifts"
			End If
			Return scaleSize
		End Get
	End Property

	Private Sub LoadTimeline()
		Select Case ScaleResolved
			Case "hours"
				LoadTimelineHours()
			Case "shifts"
				LoadTimelineShifts()
		End Select
	End Sub

	Private Sub LoadTimelineHours()

		DayPilotScheduler1.Scale = TimeScale.Manual
		DayPilotScheduler1.Timeline.Clear()

		DayPilotScheduler1.Separators.Clear()

		For i As Integer = 0 To DayPilotScheduler1.Days - 1
			Dim day As Date = DayPilotScheduler1.StartDate.AddDays(i)

			For x As Integer = MorningShiftStarts To MorningShiftEnds - 1
				DayPilotScheduler1.Timeline.Add(day.AddHours(x), day.AddHours(x + 1))
			Next x
			For x As Integer = AfternoonShiftStarts To AfternoonShiftEnds - 1
				DayPilotScheduler1.Timeline.Add(day.AddHours(x), day.AddHours(x + 1))
			Next x

			If showSeparators Then
				DayPilotScheduler1.Separators.Add(day.AddHours(MorningShiftStarts), Color.Black)
				DayPilotScheduler1.Separators.Add(day.AddHours(AfternoonShiftStarts), Color.DarkGray)
			End If

		Next i


		DayPilotScheduler1.TimeHeaders.Clear()
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Month))
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Day, "ddd d"))
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Hour, "ht"))

	End Sub

	Private Sub LoadTimelineShifts()
		DayPilotScheduler1.Scale = TimeScale.Manual
		DayPilotScheduler1.Timeline.Clear()

		DayPilotScheduler1.Separators.Clear()

		For i As Integer = 0 To DayPilotScheduler1.Days - 1
			Dim day As Date = DayPilotScheduler1.StartDate.AddDays(i)

			DayPilotScheduler1.Timeline.Add(day.AddHours(MorningShiftStarts), day.AddHours(MorningShiftEnds))
			DayPilotScheduler1.Timeline.Add(day.AddHours(AfternoonShiftStarts), day.AddHours(AfternoonShiftEnds))

			If showSeparators Then
				DayPilotScheduler1.Separators.Add(day.AddHours(MorningShiftStarts), Color.Black)
				DayPilotScheduler1.Separators.Add(day.AddHours(AfternoonShiftStarts), Color.DarkGray)
			End If

		Next i

		DayPilotScheduler1.TimeHeaders.Clear()
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Month))
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Day, "ddd d"))
		DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Cell, "tt"))

	End Sub

	Protected Sub DayPilotScheduler1_OnTimeRangeSelected(ByVal sender As Object, ByVal e As TimeRangeSelectedEventArgs)
		Dim slotDuration As Integer = 60

		Dim doctorId As Integer = Convert.ToInt32(e.Resource)

		For i As Integer = 0 To DayPilotScheduler1.Timeline.Count - 1
			Dim cell As TimeCell = DayPilotScheduler1.Timeline(i)

			If e.Start <= cell.Start AndAlso cell.End <= e.End Then
				Dim start As Date = cell.Start
				Do While start < cell.End
					CreateShift(start, start.AddMinutes(slotDuration), doctorId)
					start = start.AddMinutes(slotDuration)
				Loop
			End If
		Next i

		LoadTimeline()
		LoadAppointments()
	End Sub

	Private Sub CreateShift(ByVal start As Date, ByVal [end] As Date, ByVal doctorId As Integer)
		If start >= [end] Then
			Throw New Exception("Invalid shift, Start >= End")
		End If

		Db.CreateAppointment(doctorId, start, [end])
	End Sub


	Protected Sub ButtonClear_OnClick(ByVal sender As Object, ByVal e As EventArgs)

		Dim start As Date = DayPilotScheduler1.StartDate
		Dim [end] As Date = DayPilotScheduler1.StartDate.AddDays(DayPilotScheduler1.Days)

		Db.DeleteAppointmentsFree(start, [end])

		Response.Redirect("Manager.aspx", True)
	End Sub

	Protected Sub DayPilotScheduler1_OnEventDelete(ByVal sender As Object, ByVal e As EventDeleteEventArgs)
        Dim id_Renamed As Integer = Convert.ToInt32(e.Id)
		Db.DeleteAppointmentIfFree(id_Renamed)

		LoadAppointments()
	End Sub

	Protected Sub DayPilotScheduler1_OnBeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
		Dim status As String = e.Tag("AppointmentStatus")
		Select Case status
			Case "free"
				e.DurationBarColor = "green"
				e.EventDeleteEnabled = ScaleResolved = "hours" ' only allow deleting in the more detailed hour scale mode
			Case "waiting"
				e.DurationBarColor = "orange"
				e.EventDeleteEnabled = False
			Case "confirmed"
				e.DurationBarColor = "#f41616" ' red
				e.EventDeleteEnabled = False
		End Select

	End Sub

	Protected Sub DayPilotScheduler1_OnCommand(ByVal sender As Object, ByVal e As CommandEventArgs)
		Select Case e.Command
			Case "navigate"
				DayPilotScheduler1.StartDate = CDate(e.Data("start"))
                DayPilotScheduler1.Days = CInt(e.Data("days"))
				DayPilotScheduler1.ScrollX = 0
				LoadTimeline()
				LoadDoctors()
				LoadAppointments()
			Case "refresh"
				LoadTimeline()
				LoadDoctors()
				LoadAppointments()
		End Select
	End Sub
End Class