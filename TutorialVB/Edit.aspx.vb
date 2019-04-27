' Copyright � 2005 - 2013 Annpoint, s.r.o.
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
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.UI
Imports Util

Partial Public Class Edit
	Inherits Page

	Private dr As DataRow

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Response.Cache.SetCacheability(HttpCacheability.NoCache)

		dr = Db.LoadAppointment(Convert.ToInt32(Request.QueryString("id")))

		If dr Is Nothing Then
			Throw New Exception("The requested time slot was not found")
		End If

		If Not IsPostBack Then

			TextBoxStart.Text = Convert.ToDateTime(dr("AppointmentStart")).ToString()
			TextBoxEnd.Text = Convert.ToDateTime(dr("AppointmentEnd")).ToString()
			TextBoxName.Text = TryCast(dr("AppointmentPatientName"), String)
			DropDownListStatus.SelectedValue = DirectCast(dr("AppointmentStatus"), String)

			DropDownListRoom.DataSource = Db.LoadDoctors()
			DropDownListRoom.DataTextField = "DoctorName"
			DropDownListRoom.DataValueField = "DoctorId"
			DropDownListRoom.SelectedValue = Convert.ToString(dr("DoctorId"))
			DropDownListRoom.DataBind()

			TextBoxName.Focus()
		End If
	End Sub
	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		'DateTime start = Convert.ToDateTime(TextBoxStart.Text);
		'DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
		'int doctor = Convert.ToInt32(DropDownListRoom.SelectedValue);

		Dim start As Date = DirectCast(dr("AppointmentStart"), Date)
		Dim [end] As Date = DirectCast(dr("AppointmentEnd"), Date)
		Dim doctor As Integer = DirectCast(dr("DoctorId"), Integer)

		Dim name As String = TextBoxName.Text
		Dim status As String = DropDownListStatus.SelectedValue

        Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id"))

		Db.UpdateAppointment(id_Renamed, start, [end], name, doctor, status)
		Modal.Close(Me, "OK")
	End Sub


	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub
	Protected Sub LinkButtonDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id"))
		Db.DeleteAppointment(id_Renamed)
		Modal.Close(Me, "OK")
	End Sub
End Class
