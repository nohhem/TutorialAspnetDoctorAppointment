<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" MasterPageFile="~/Site.master"  %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<h1>Request an Appointment</h1>

<div style="float:left; width: 150px;">
	<DayPilot:DayPilotNavigator 
		runat="server" 
		ID="DayPilotNavigator1" 
		ClientIDMode="Static"
		BoundDayPilotID="DayPilotCalendar1"
		ShowMonths="3"    
		SelectMode="Week"

		OnBeforeCellRender="DayPilotNavigator1_OnBeforeCellRender"        
		/>  
</div>

<div style="margin-left: 150px;">
	<p>Available time slots:</p>

	<DayPilot:DayPilotCalendar 
		runat="server" 
		ID="DayPilotCalendar1"
		ClientIDMode="Static"
		ClientObjectName="dp"
		ViewType="Week"

		DurationBarWidth="10"

		OnCommand="DayPilotCalendar1_OnCommand"
		OnBeforeEventRender="DayPilotCalendar1_OnBeforeEventRender"

		EventClickHandling="JavaScript"
		EventClickJavaScript="edit(e)"
		/>
</div>    


<script>
	function edit(e) {
		new DayPilot.Modal({
			onClosed: function (args) {
				if (args.result == "OK") {
					dp.commandCallBack('refresh');
				}
			}
		}).showUrl("Request.aspx?id=" + e.id());
	}
</script>

</asp:Content>