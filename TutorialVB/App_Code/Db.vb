Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Web

''' <summary>
''' Summary description for Db
''' </summary>
Public NotInheritable Class Db

	Private Sub New()
	End Sub


	Public Shared Function LoadDoctors() As DataTable
		Dim da As New SqlDataAdapter("SELECT * FROM [Doctor] ORDER BY [DoctorName]", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		Dim dt As New DataTable()
		da.Fill(dt)

		Return dt
	End Function

	Public Shared Function LoadDoctor(ByVal id As Integer) As DataRow
		Dim da As New SqlDataAdapter("SELECT * FROM [Doctor] WHERE [DoctorId] = @id", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		da.SelectCommand.Parameters.AddWithValue("id", id)

		Dim dt As New DataTable()
		da.Fill(dt)

		If dt.Rows.Count > 0 Then
			Return dt.Rows(0)
		End If
		Return Nothing

	End Function

	Public Shared Function LoadAppointment(ByVal id As Integer) As DataRow
		Dim da As New SqlDataAdapter("SELECT * FROM [Appointment] WHERE [AppointmentId] = @id", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		da.SelectCommand.Parameters.AddWithValue("id", id)

		Dim dt As New DataTable()
		da.Fill(dt)

		If dt.Rows.Count > 0 Then
			Return dt.Rows(0)
		End If
		Return Nothing

	End Function


	Public Shared Function LoadAppointmentsForDoctor(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date) As DataTable
		Dim da As New SqlDataAdapter("SELECT * FROM [Appointment] WHERE [DoctorId] = @doctor AND NOT (([AppointmentEnd] <= @start) OR ([AppointmentStart] >= @end))", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		da.SelectCommand.Parameters.AddWithValue("doctor", id)
		da.SelectCommand.Parameters.AddWithValue("start", start)
		da.SelectCommand.Parameters.AddWithValue("end", [end])
		Dim dt As New DataTable()
		da.Fill(dt)

		Return dt
	End Function

	Public Shared Function LoadAppointments(ByVal start As Date, ByVal [end] As Date) As DataTable
		Dim da As New SqlDataAdapter("SELECT * FROM [Appointment] WHERE NOT (([AppointmentEnd] <= @start) OR ([AppointmentStart] >= @end))", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		da.SelectCommand.Parameters.AddWithValue("start", start)
		da.SelectCommand.Parameters.AddWithValue("end", [end])
		Dim dt As New DataTable()
		da.Fill(dt)

		Return dt

	End Function

	Public Shared Function LoadFreeAndMyAppointments(ByVal start As Date, ByVal [end] As Date, ByVal sessionId As String) As DataTable
		Dim da As New SqlDataAdapter("SELECT * FROM [Appointment] WHERE ([AppointmentStatus] = 'free' OR ([AppointmentStatus] <> 'free' AND [AppointmentPatientSession] = @session)) AND NOT (([AppointmentEnd] <= @start) OR ([AppointmentStart] >= @end))", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
		da.SelectCommand.Parameters.AddWithValue("session", sessionId)
		da.SelectCommand.Parameters.AddWithValue("start", start)
		da.SelectCommand.Parameters.AddWithValue("end", [end])
		Dim dt As New DataTable()
		da.Fill(dt)

		Return dt

	End Function


	Public Shared Sub CreateAppointment(ByVal doctor As Integer, ByVal start As Date, ByVal [end] As Date)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("INSERT INTO [Appointment] ([AppointmentStart], [AppointmentEnd], [DoctorId]) VALUES(@start, @end, @doctor)", con)
			cmd.Parameters.AddWithValue("start", start)
			cmd.Parameters.AddWithValue("end", [end])
			cmd.Parameters.AddWithValue("doctor", doctor)
			cmd.ExecuteNonQuery()

'            
'            cmd = new SqlCommand("select @@identity;", con);
'            int id = Convert.ToInt32(cmd.ExecuteScalar());
'            return id;
'             
		End Using
	End Sub

	Public Shared Sub DeleteAppointmentsFree(ByVal start As Date, ByVal [end] As Date)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("DELETE FROM [Appointment] WHERE [AppointmentStatus] = 'free' AND NOT (([AppointmentEnd] <= @start) OR ([AppointmentStart] >= @end))", con)
			cmd.Parameters.AddWithValue("start", start)
			cmd.Parameters.AddWithValue("end", [end])
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Shared Sub DeleteAppointmentIfFree(ByVal id As Integer)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("DELETE FROM [Appointment] WHERE [AppointmentId] = @id AND [AppointmentStatus] = 'free'", con)
			cmd.Parameters.AddWithValue("id", id)
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Shared Function LoadFirstDoctor() As DataRow
		Dim da As New SqlDataAdapter("SELECT top 1 * FROM [Doctor] ORDER BY [DoctorName]", ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)

		Dim dt As New DataTable()
		da.Fill(dt)

		If dt.Rows.Count > 0 Then
			Return dt.Rows(0)
		End If
		Return Nothing
	End Function

	Public Shared Sub UpdateAppointment(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date, ByVal name As String, ByVal doctor As Integer, ByVal status As String)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("UPDATE [Appointment] SET [AppointmentStart] = @start, [AppointmentEnd] = @end, [AppointmentPatientName] = @name, [AppointmentStatus] = @status WHERE [AppointmentId] = @id", con)
			cmd.Parameters.AddWithValue("start", start)
			cmd.Parameters.AddWithValue("end", [end])
			cmd.Parameters.AddWithValue("name", name)
			cmd.Parameters.AddWithValue("status", status)
			cmd.Parameters.AddWithValue("id", id)
			cmd.ExecuteNonQuery()
		End Using

	End Sub

	Public Shared Sub DeleteAppointment(ByVal id As Integer)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("DELETE FROM [Appointment] WHERE [AppointmentId] = @id", con)
			cmd.Parameters.AddWithValue("id", id)
			cmd.ExecuteNonQuery()
		End Using

	End Sub

	Public Shared Sub RequestAppointment(ByVal id As Integer, ByVal name As String, ByVal sessionId As String)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("UPDATE [Appointment] SET [AppointmentPatientName] = @name, [AppointmentStatus] = @status, [AppointmentPatientSession] = @session WHERE [AppointmentId] = @id", con)
			cmd.Parameters.AddWithValue("name", name)
			cmd.Parameters.AddWithValue("status", "waiting")
			cmd.Parameters.AddWithValue("session", sessionId)
			cmd.Parameters.AddWithValue("id", id)
			cmd.ExecuteNonQuery()
		End Using

	End Sub

	Public Shared Sub MoveAppointment(ByVal id As String, ByVal start As Date, ByVal [end] As Date)
		Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("daypilot").ConnectionString)
			con.Open()
			Dim cmd As New SqlCommand("UPDATE [Appointment] SET [AppointmentStart] = @start, [AppointmentEnd] = @end WHERE [AppointmentId] = @id", con)
			cmd.Parameters.AddWithValue("start", start)
			cmd.Parameters.AddWithValue("end", [end])
			cmd.Parameters.AddWithValue("id", id)
			cmd.ExecuteNonQuery()
		End Using

	End Sub
End Class