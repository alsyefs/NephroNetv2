<%@ Page Title="About" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="NephroNet.Accounts.Patient.About" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
    <h2><%: Title %></h2>
        <div class="panel panel-default">
        <div class="panel-body">
        <script>
                    function pleaseWait() {
                        $(".modal").show();
                        return true;
                    }
                </script>
                <div id="modal" class="modal" style="background-color:rgba(64,64,64,0.5);width:100%;height:100%;z-index:1000;display:none"></div>
                <div id="wait" class="modal" style="width:200px;height:20px;margin:100px auto 0 auto;display:none;background-color:#fff;z-index:1001;text-align:center;">PLEASE WAIT...</div>
    <h3>Nephrology social network</h3>
    <p>This website serves as a social network for people who need to communicate with others having the same medical situations to find help.</p>
    </div>
            </div>
        </div>
</asp:Content>
