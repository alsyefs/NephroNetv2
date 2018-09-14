using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ApproveUsers : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            int countNewUsers = getTotalNewUsers();
            if(countNewUsers > 0)
            {
                lblMessage.Visible = false;
                createTable(countNewUsers);
            }
            else if (countNewUsers == 0)
            {
                lblMessage.Visible = true;
            }
        }
        protected int getTotalNewUsers()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count the not-approved users:
            cmd.CommandText = "select count(*) from [Registrations]";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            return count;
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Role", typeof(string));
            string id = "", name = "", email = "", role = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i <= count; i++)
            {
                //Get the register ID:
                cmd.CommandText = "select [registerId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY registerId ASC), *FROM [Registrations]) as t where rowNum = '" + i + "'";
                id = cmd.ExecuteScalar().ToString();
                //Get first name:
                cmd.CommandText = "select register_firstname from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY registerId ASC), *FROM [Registrations]) as t where rowNum = '" + i + "'";
                name = cmd.ExecuteScalar().ToString();
                //Get last name and add it to the end of the first name:
                cmd.CommandText = "select register_lastname from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY registerId ASC), *FROM [Registrations]) as t where rowNum = '" + i + "'";
                name = name + " " + cmd.ExecuteScalar().ToString();
                //Get email:
                cmd.CommandText = "select register_email from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY registerId ASC), *FROM [Registrations]) as t where rowNum = '" + i + "'";
                email = cmd.ExecuteScalar().ToString();
                //Get role (1 = Admin, 2 = Physician, 3 = Patient):
                cmd.CommandText = "select register_roleId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY registerId ASC), *FROM [Registrations]) as t where rowNum = '" + i + "'";
                int tempRole = Convert.ToInt32(cmd.ExecuteScalar());
                if (tempRole == 1)
                    role = "Admin";
                else if (tempRole == 2)
                    role = "Physician";
                else// if (tempRole == 3)
                    role = "Patient";
                dt.Rows.Add(id, name, email, role);
            }
            connect.Close();
            grdUsers.DataSource = dt;
            grdUsers.DataBind();
            if (grdUsers.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdUsers.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdUsers.Rows.Count; i++)
                {
                    grdUsers.Rows[i].Cells[1].Visible = false;
                }
            }
        }
        protected void initialPageAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            CheckAdminSession session = new CheckAdminSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            
        }
        protected void clearSession()
        {

            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/");
        }
        protected void addSession()
        {
            Session.Add("username", username);
            Session.Add("roleId", roleId);
            Session.Add("loginId", loginId);
            Session.Add("token", token);
        }
        protected void getSession()
        {
            username = (string)(Session["username"]);
            roleId = (string)(Session["roleId"]);
            loginId = (string)(Session["loginId"]);
            token = (string)(Session["token"]);
        }
        protected void grdUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdUsers.PageIndex = e.NewPageIndex;
            grdUsers.DataBind();
            if (grdUsers.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdUsers.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdUsers.Rows.Count; i++)
                {
                    grdUsers.Rows[i].Cells[1].Visible = false;
                }                
            }
        }
    }
}