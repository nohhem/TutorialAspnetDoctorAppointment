Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DayPilot.Web.Ui.Events
Imports DayPilot.Web.Ui.Events.Calendar
Imports BeforeCellRenderEventArgs = DayPilot.Web.Ui.Events.Navigator.BeforeCellRenderEventArgs
Imports CommandEventArgs = DayPilot.Web.Ui.Events.CommandEventArgs

Partial Public Class Doctor
	Inherits System.Web.UI.Page

	Private _appointments As DataTable

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then

			If String.IsNullOrEmpty(Request.QueryString("id")) Then
				'Schedule.Visible = false;
				'return;
				Dim first As DataRow = Db.LoadFirstDoctor()
				If first IsNot Nothing Then
					Response.Redirect("Doctor.aspx?id=" & first("DoctorId"), True)
					Return
				End If
			End If

            Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id")) ' basic validation
			DropDownListDoctor.SelectedValue = id_Renamed.ToString()

			Dim doctor As DataRow = Db.LoadDoctor(id_Renamed)
			If doctor Is Nothing Then
				Schedule.Visible = False
				Return
			End If

			DropDownListDoctor.AppendDataBoundItems = False

			LoadDoctors()
			LoadNavigatorData()
			LoadCalendarData()
		End If
	End Sub

	Private Sub LoadDoctors()
		DropDownListDoctor.DataSource = Db.LoadDoctors()
		DropDownListDoctor.DataTextField = "DoctorName"
		DropDownListDoctor.DataValueField = "DoctorId"
		DropDownListDoctor.DataBind()
	End Sub

	Private Sub LoadNavigatorData()
		If _appointments Is Nothing Then
			LoadAppointments()
		End If

		DayPilotNavigator1.DataSource = _appointments
		DayPilotNavigator1.DataStartField = "AppointmentStart"
		DayPilotNavigator1.DataEndField = "AppointmentEnd"
		DayPilotNavigator1.DataIdField = "AppointmentId"
		DayPilotNavigator1.DataBind()
	End Sub

	Private Sub LoadCalendarData()
		If _appointments Is Nothing Then
			LoadAppointments()
		End If

		DayPilotCalendar1.DataSource = _appointments
		DayPilotCalendar1.DataStartField = "AppointmentStart"
		DayPilotCalendar1.DataEndField = "AppointmentEnd"
		DayPilotCalendar1.DataIdField = "AppointmentId"
		DayPilotCalendar1.DataTextField = "AppointmentPatientName"
		DayPilotCalendar1.DataTagFields = "AppointmentStatus"
		DayPilotCalendar1.DataBind()
		DayPilotCalendar1.Update()
	End Sub

	Private Sub LoadAppointments()
        Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id")) ' basic validation
		_appointments = Db.LoadAppointmentsForDoctor(id_Renamed, DayPilotNavigator1.VisibleStart, DayPilotNavigator1.VisibleEnd)
	End Sub

	Protected Sub DayPilotCalendar1_OnCommand(ByVal sender As Object, ByVal e As CommandEventArgs)
		Select Case e.Command
			Case "navigate"
				DayPilotCalendar1.StartDate = CDate(e.Data("day"))
				LoadCalendarData()
			Case "refresh"
				LoadCalendarData()
		End Select
	End Sub

	Protected Sub DayPilotNavigator1_OnBeforeCellRender(ByVal sender As Object, ByVal e As BeforeCellRenderEventArgs)
	End Sub

	Protected Sub DropDownListDoctor_OnSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		Dim selected As String = DropDownListDoctor.SelectedValue
		If selected = "NONE" Then
			Response.Redirect("Doctor.aspx", True)
			Return
		End If

		Response.Redirect("Doctor.aspx?id=" & selected, True)
	End Sub

	Protected Sub DayPilotCalendar1_OnTimeRangeSelected(ByVal sender As Object, ByVal e As TimeRangeSelectedEventArgs)
		Dim doctor As Integer = Convert.ToInt32(Request.QueryString("id"))

		Db.CreateAppointment(doctor, e.Start, e.End)

		LoadCalendarData()

	End Sub

	Protected Sub DayPilotCalendar1_OnBeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
		Dim status As String = e.Tag("AppointmentStatus")
		Select Case status
			Case "free"
				e.DurationBarColor = "green"
			Case "waiting"
				e.DurationBarColor = "orange"
			Case "confirmed"
				e.DurationBarColor = "#f41616" ' red
		End Select
	End Sub

	Protected Sub DayPilotCalendar1_OnEventMove(ByVal sender As Object, ByVal e As EventMoveEventArgs)
		Db.MoveAppointment(e.Id, e.NewStart, e.NewEnd)
		LoadCalendarData()
	End Sub

	Protected Sub DayPilotCalendar1_OnEventResize(ByVal sender As Object, ByVal e As EventResizeEventArgs)
		Db.MoveAppointment(e.Id, e.NewStart, e.NewEnd)
		LoadCalendarData()
	End Sub
End Class