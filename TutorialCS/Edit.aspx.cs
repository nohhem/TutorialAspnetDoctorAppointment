/* Copyright © 2005 - 2013 Annpoint, s.r.o.
   Use of this software is subject to license terms. 
   http://www.daypilot.org/

   If you have purchased a DayPilot Pro license, you are allowed to use this 
   code under the conditions of DayPilot Pro License Agreement:

   http://www.daypilot.org/files/LicenseAgreement.pdf

   Otherwise, you are allowed to use it for evaluation purposes only under 
   the conditions of DayPilot Pro Trial License Agreement:
   
   http://www.daypilot.org/files/LicenseAgreementTrial.pdf
   
*/

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using Util;

public partial class Edit : Page
{
    private DataRow dr;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        dr = Db.LoadAppointment(Convert.ToInt32(Request.QueryString["id"]));

        if (dr == null)
        {
            throw new Exception("The requested time slot was not found");
        }

        if (!IsPostBack)
        {

            TextBoxStart.Text = Convert.ToDateTime(dr["AppointmentStart"]).ToString();
            TextBoxEnd.Text = Convert.ToDateTime(dr["AppointmentEnd"]).ToString();
            TextBoxName.Text = dr["AppointmentPatientName"] as string;
            DropDownListStatus.SelectedValue = (string)dr["AppointmentStatus"];
            
            DropDownListRoom.DataSource = Db.LoadDoctors();
            DropDownListRoom.DataTextField = "DoctorName";
            DropDownListRoom.DataValueField = "DoctorId";
            DropDownListRoom.SelectedValue = Convert.ToString(dr["DoctorId"]);
            DropDownListRoom.DataBind();

            TextBoxName.Focus();
        }
    }
    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        //DateTime start = Convert.ToDateTime(TextBoxStart.Text);
        //DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
        //int doctor = Convert.ToInt32(DropDownListRoom.SelectedValue);

        DateTime start = (DateTime)dr["AppointmentStart"];
        DateTime end = (DateTime)dr["AppointmentEnd"];
        int doctor = (int) dr["DoctorId"];

        string name = TextBoxName.Text;
        string status = DropDownListStatus.SelectedValue;

        int id = Convert.ToInt32(Request.QueryString["id"]);

        Db.UpdateAppointment(id, start, end, name, doctor, status);
        Modal.Close(this, "OK");
    }


    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }
    protected void LinkButtonDelete_Click(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(Request.QueryString["id"]);
        Db.DeleteAppointment(id);
        Modal.Close(this, "OK");
    }
}
