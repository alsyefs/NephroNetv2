﻿<%@ Page Title="View Topic" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="ViewTopic.aspx.cs" Inherits="NephroNet.Accounts.Admin.ViewTopic" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="default" runat="server">
    </asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <%--<h2><%: Title %></h2>--%>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                <script>
                    function pleaseWait() {
                        $(".modal").show();
                        return true;
                    }
                </script>
                <div id="modal" class="modal" style="background-color:rgba(64,64,64,0.5);width:100%;height:100%;z-index:1000;display:none"></div>
                <div id="wait" class="modal" style="width:200px;height:20px;margin:100px auto 0 auto;display:none;background-color:#fff;z-index:1001;text-align:center;">PLEASE WAIT...</div>
                <div runat="server" id="ViewTopicDiv">
                    <asp:Label ID="lblHeader" runat="server" Text="Header" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:Label ID="lblContents" runat="server" Text="Contents"></asp:Label>
                    <br />
                    <asp:Label ID="lblEntry" runat="server" Text="Message"></asp:Label>
                    &nbsp;
                        <style>
                            .content {
                                min-width: 100%;
                            }
                        </style>
                    <asp:TextBox ID="txtEntry" runat="server" Height="130px" Width="100%" TextMode="MultiLine" CssClass="content"></asp:TextBox>
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lblEntryError" runat="server" Text="Invalid input: Please type a description." Visible="false" ForeColor="red"></asp:Label>
                    <br />
                    <br />
                    <asp:FileUpload ID="FileUpload1" runat="server" Width="100%" AllowMultiple="true" onchange="onInputChange(event)" class="btn btn-primary" />
                    <div id='fileNames'></div>
                    &nbsp;
                        <asp:Label ID="lblImageError" runat="server" Text="Image" Visible="false" ForeColor="red"></asp:Label>
                    <%--Submit--%><br />
                    <br />
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnSubmit_Click" OnClientClick="pleaseWait();"/>
                    &nbsp;
                        <%--Cancel button--%>    
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" OnClientClick="pleaseWait();"/>
                    <%--Error message--%>
                    <br />
                    <br />
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
                </div>
                <%--Content end--%>
                <script type="text/javascript">
                    function terminateTopic(topicId, creatorId) {
                        pleaseWait();
                        if (confirm('Are sure you want to terminate the selected topic?'))
                            terminateTopicConfirmed(topicId, creatorId);
                    }
                    function terminateTopicConfirmed(topicId, creatorId) {
                        console.log('You just confirmed!');
                        var topicID = parseInt(topicId);
                        var creatorID = parseInt(creatorId);
                        var obj = {
                            topicId: topicID,
                            entry_creatorId: creatorID
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("ViewTopic.aspx/terminateTopic_Click") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (msg) {
                                window.location.href = window.location.href;
                                //$('#ViewTopicDiv').load(document.href + '#ViewTopicDiv');
                                console.log('Successfully terminated the topic!');
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                    }

                </script>

                <script type="text/javascript">
                    function removeTopic(topicId, creatorId) {
                        pleaseWait();
                        if (confirm('Are sure you want to remove the selected topic?'))
                            removeTopicConfirmed(topicId, creatorId);
                    }
                    function removeTopicConfirmed(topicId, creatorId) {
                        console.log('You just confirmed!');
                        var topicID = parseInt(topicId);
                        var creatorID = parseInt(creatorId);
                        var obj = {
                            topicId: topicID,
                            entry_creatorId: creatorID
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("ViewTopic.aspx/removeTopic_Click") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (msg) {
                                window.location.href = window.location.href;
                                //$('#ViewTopicDiv').load(document.href + '#ViewTopicDiv');
                                //console.log('Successfully updated the page!');
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                    }

                </script>
                <script type="text/javascript">
                    function complain(messageId, messageNumberInPage, userId) {
                        pleaseWait();
                        var message_text = prompt('Please enter your reason for reporting the selected message entry# (' + messageNumberInPage + '):');
                        if (message_text == null || message_text == "") {
                            if (confirm("You have not typed a reason. Do you still wish to submit a report without a reason?"))
                                complainAboutMessage(messageId, userId, "There is no specific reason");
                        }
                        else {
                                complainAboutMessage(messageId, userId, message_text);
                        }
                    }
                    function complainAboutMessage(messageId, userId, message_text) {
                        console.log('You just confirmed!');
                        var messageID = parseInt(messageId);
                        var userID = parseInt(userId);
                        var obj = {
                            entryId: messageID,
                            current_userId: userID,
                            complain_text: message_text
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("ViewTopic.aspx/reportMessage_Click") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (msg) {
                                console.log('Successfully reported message ID: ' + messageId + '!');
                                window.location.href = window.location.href;
                            },
                            error: function (xhr, status, error) {
                                console.log('The call failed to report the message ID: ' + messageId);
                                console.log(xhr.responseText);
                            }
                        });
                    }
                    function removeMessage(messageId, messageNumberInPage, creatorId) {
                        pleaseWait();
                        if (confirm('Are sure you want to remove the selected message entry# (' + messageNumberInPage + ')?'))
                            removeMessageConfirmed(messageId, creatorId);
                    }
                    function removeMessageConfirmed(messageId, creatorId) {
                        console.log('You just confirmed!');
                        var messageID = parseInt(messageId);
                        var creatorID = parseInt(creatorId);
                        var obj = {
                            entryId: messageID,
                            entry_creatorId: creatorID
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("ViewTopic.aspx/removeMessage_Click") %>',
                                        data: param,
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        async: true,
                                        cache: false,
                                        success: function (msg) {
                                            console.log('Successfully deleted message ID: ' + messageId + '!');
                                            window.location.href = window.location.href;
                                            //$('#ViewTopicDiv').load(document.href + '#ViewTopicDiv');
                                            //console.log('Successfully updated the page!');
                                        },
                                        error: function (xhr, status, error) {
                                            console.log('The call failed! for message ID: ' + messageId);
                                            console.log(xhr.responseText);
                                        }
                                    });
                                }

                </script>
                <%--Popup message--%>
                <script type="text/javascript">
                                function OpenPopup(site) { popup(site); }
                                // copied from http://www.dotnetfunda.com/codes/code419-code-to-open-popup-window-in-center-position-.aspx
                                function popup(url) {
                                    var width = 500;
                                    var height = 300;
                                    var left = (screen.width - width) / 2;
                                    var top = (screen.height - height) / 2;
                                    var params = 'width=' + width + ', height=' + height;
                                    params += ', top=' + top + ', left=' + left;
                                    params += ', directories=no';
                                    params += ', location=no';
                                    params += ', menubar=no';
                                    params += ', resizable=no';
                                    params += ', scrollbars=no';
                                    params += ', status=no';
                                    params += ', toolbar=no';
                                    newwin = window.open(url, 'windowname5', params);
                                    if (window.focus) { newwin.focus() }
                                    return false;
                                }
                </script>
                <script type="text/javascript">
                                function onInputChange(e) {
                                    //alert('Just clicked upload!');
                                    var res = "";
                                    for (var i = 0; i < $('#<%= FileUpload1.ClientID %>').get(0).files.length; i++) {
                                        res += $('#<%= FileUpload1.ClientID %>').get(0).files[i].name + "<br />";
                                    }
                                    $('#fileNames').html(res);
                                }
                </script>

            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
