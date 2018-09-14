<%@ Page Title="View Topic" Language="C#" MasterPageFile="~/Physician.Master" AutoEventWireup="true" CodeBehind="ViewTopic.aspx.cs" Inherits="NephroNet.Accounts.Physician.ViewTopic" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <%--<h2><%: Title %></h2>--%>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                <asp:Label ID="lblHeader" runat="server" Text="Header" Font-Bold="True"></asp:Label>
                <br />
               
                <%--<asp:Label ID="lblContents" runat="server" Text="Contents"></asp:Label>--%>

                <div runat="server" id="divMessages"></div>


                <br />
                <asp:Label ID="lblEntry" runat="server" Text="Message"></asp:Label>
                &nbsp;
                <style>
                    .content {
                        min-width: 100%;
                    }
                </style>
                <asp:TextBox ID="txtEntry" runat="server" Height="130px" Width="959px" TextMode="MultiLine" CssClass="content"></asp:TextBox>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblEntryError" runat="server" Text="Invalid input: Please type a description." Visible="false" ForeColor="red"></asp:Label>
                <br />
                <br />


                <asp:FileUpload ID="FileUpload1" runat="server" Width="385px" AllowMultiple="true" onchange="onInputChange(event)" class="btn btn-primary" />
                <div id='fileNames'></div>
                <script type="text/javascript">
                    function onInputChange(e) {
                        var res = "";
                        for (var i = 0; i < $('#<%= FileUpload1.ClientID %>').get(0).files.length; i++) {
                            res += $('#<%= FileUpload1.ClientID %>').get(0).files[i].name + "<br />";
                        }
                        $('#fileNames').html(res);
                    }
                </script>
                &nbsp;
                <asp:Label ID="lblImageError" runat="server" Text="Image" Visible="false" ForeColor="red"></asp:Label>
                <%--Submit--%><br />
                <br />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnSubmit_Click" />
                &nbsp;
                
                <%--Cancel button--%>    
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" />
                <%--Error message--%>
                <br />
                <br />
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
                <%--Content end--%>
                <script type="text/javascript">
                    function terminateTopic(topicId, creatorId) {

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
                                location.reload(true);
                                //$('#ViewTopicDiv').load(document.href + '#ViewTopicDiv');
                                console.log('Successfully terminated the topic!');
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                    }
                    function refreshMessages(topicId, userId) {
                        console.log('Started refreshMessages. topicId= ' + topicId + ' userId=' + userId);
                        var topicID = parseInt(topicId);
                        var userID = parseInt(userId);
                        var obj = {
                            topicId: topicID,
                            userId: userID
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        console.log('topicID= ' + topicID + ' userID=' + userID + ' param:\n'+param);
                        var marker;
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("ViewTopic.aspx/getMessages") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (data) {
                                console.log('refresh success:\n' + data.responseText);
                                //location.reload(true);
                                //$('#ViewTopicDiv').load(document.href + '#ViewTopicDiv');
                                
                                //console.log(data.responseText);
                                //marker = JSON.stringify(data);
                                //console.log(marker);

                                //var result = $('<div />').append(data).find('#divMessages').html();
                                //$('#divMessages').html(data);
                                $('#divMessages').html(data);
                                $("#divMessages").html(data.responseText);
                                //$('#divMessages').replaceWith($('#divMessages', data.responseText));
                                //$('body').html(data);
                                //$('body').html($('body', data.responseText).html());
                                //$('#divMessages').html(marker);
                                //$('#divMessages').load(document.href + '#divMessages');
                                //response = $(XmlHttpRequest.responseText);//.find('b\\:value').text();
                                //console.log(data + ' ' + textStatus + ' ' + response);
                                console.log('Successfully refreshed the div data.responseText:\n' + data.responseText);
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                    }
                </script>

                <script type="text/javascript">
                    function removeTopic(topicId, creatorId) {

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
                                location.reload(true);
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
                    function removeMessage(messageId, messageNumberInPage, creatorId, topicId) {

                        if (confirm('Are sure you want to remove the selected message entry# (' + messageNumberInPage + ')?'))
                            removeMessageConfirmed(messageId, creatorId, topicId);
                    }
                    function removeMessageConfirmed(messageId, creatorId, topicId) {
                        console.log('started removeMessageConfirmed. messageId=' + messageId + ' creatorId=' + creatorId + ' topicId=' + topicId);
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
                                //refreshMessages(topicId, creatorId);
                                //console.log('Successfully deleted message ID: ' + messageId + '!');
                                location.reload(true);
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
                    function OpenPopup(site) {popup(site);}
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
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
