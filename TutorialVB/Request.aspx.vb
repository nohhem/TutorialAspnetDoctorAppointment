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

Partial Public Class Request
	Inherits Page

	Private dr As DataRow

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Response.Cache.SetCacheability(HttpCacheability.NoCache)

		dr = Db.LoadAppointment(Convert.ToInt32(Request.QueryString("id")))

		If dr Is Nothing Then
			Throw New Exception("The slot was not found")
		End If

		If Not IsPostBack Then

			TextBoxStart.Text = Convert.ToDateTime(dr("AppointmentStart")).ToString()
			TextBoxEnd.Text = Convert.ToDateTime(dr("AppointmentEnd")).ToString()
			TextBoxName.Text = TryCast(dr("AppointmentPatientName"), String)

			TextBoxName.Focus()
		End If
	End Sub
	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim name As String = TextBoxName.Text

        Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id"))

		Db.RequestAppointment(id_Renamed, name, Session.SessionID)
		Modal.Close(Me, "OK")
	End Sub


	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub
	Protected Sub LinkButtonDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id"))
		Db.DeleteAppointmentIfFree(id_Renamed)
		Modal.Close(Me, "OK")
	End Sub
End Class
