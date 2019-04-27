' Copyright � 2015 Annpoint, s.r.o.
'   Use of this software is subject to license terms. 
'   http://www.daypilot.org/
'
'   If you have purchased a DayPilot Pro license, you are allowed to use this 
'   code under the conditions of DayPilot Pro License Agreement:
'
'   http://www.daypilot.org/files/LicenseAgreement.pdf
'
'   Otherwise, you are allowed to use it for evaluation purposes only under 
'   the conditions of DayPilot Pro Trial License Agreement:
'   
'   http://www.daypilot.org/files/LicenseAgreementTrial.pdf
'   
'

Imports System
Imports System.Data
Imports DayPilot.Web.Ui.Events.Calendar
Imports BeforeCellRenderEventArgs = DayPilot.Web.Ui.Events.Navigator.BeforeCellRenderEventArgs
Imports CommandEventArgs = DayPilot.Web.Ui.Events.CommandEventArgs

Partial Public Class _Default
	Inherits System.Web.UI.Page

	Private _appointments As DataTable

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			LoadCalendarData()
			LoadNavigatorData()
		End If

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
		_appointments = Db.LoadFreeAndMyAppointments(DayPilotNavigator1.VisibleStart, DayPilotNavigator1.VisibleEnd, Session.SessionID)
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

	Protected Sub DayPilotCalendar1_OnBeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
		Dim status As String = e.Tag("AppointmentStatus")

		Select Case status
			Case "free"
				e.DurationBarColor = "green"
				e.Html = "Available"
				e.ToolTip = "Click to Request This Time Slot"
			Case "waiting"
				e.DurationBarColor = "orange"
				e.Html = "Your appointment, waiting for confirmation"
			Case "confirmed"
				e.DurationBarColor = "#f41616"
				e.Html = "Your appointment, confirmed"
		End Select
	End Sub

End Class
