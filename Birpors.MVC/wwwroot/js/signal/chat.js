﻿function Chat() {

    this.joinChat = (thenCallBack, catchCallBack) => {
        connection
            .invoke("Join")

            .then(() => {
                thenCallBack != null ? thenCallBack() : () => { };
            })

            .catch(() => {
                catchCallBack != null ? catchCallBack() : () => { };
            });
    };

    this.sendMessage = async function (text, securtyStamp) {
      
        await connection.invoke("SendMessage", text, securtyStamp.toString());
        let sound = document.getElementById("mynotification");
        sound.play();
    };
    const startbtn = document.getElementById("startbtn");

    startbtn.addEventListener("click", function () {
        let sound = document.getElementById("mynotification");
        sound.play();
    })
    this.receiveMessage = function () {
        connection.on("ReceiveMessage", (data, position) => {
            let element = "";
            if (position)
                element = _createSendMessageView(data.photo, data.text);
            else
                element = _createReplyMessageView(data.photo, data.text);


          

            $(`.chat[data-conversation="${data.conversationId}"`).append(element);



            startbtn.click();
           

            Snackbar.show({
                text: `${data.text}.`,
                width: 'auto'
            });



        });

    };

    this.startConnection = function (thenCallback, catchCallBack) {
        connection
            .start()

            .then(() => {

                thenCallback != null ? thenCallback() : () => { };
            })
            .catch(() => {

                catchCallBack != null ? catchCallBack() : () => { };
            });

    };
    

    this.makeConversation = function () {
        connection.on("MakeNewConversation", function (data) {
            let element = _createConversationListItem(data);
            $(".participants").prepend(element);

            let msgWrapper = _createConversationMessageWrapper(data);
            $("#chats").append(msgWrapper);




        });
    };


    this.receiveNotification = function () {

    };

    this.closeConnection = function () {

    };

    this.getConversationMessages = function (_securityStamp) {

        $.ajax({
            url: "/chat/getConversationMessages",
            data: {
                securityStamp: _securityStamp
            },
            dataType: "json",
            type: "get",
            success: function (res) {
                for (var message of res.data) {
                    let element = "";
                    if (message.appUserId == res.loggedUserId) {
                        element = _createSendMessageView(message.photo, message.text);
                    }
                    else {
                        element = _createReplyMessageView(message.photo, message.text);
                    }
                    $(`.chat[data-id="${_securityStamp}"`).append(element);
                }


            }
        });
    };

    function _createReplyMessageView(photo, text) {
        let element = ` <div class="bubble you">
                                                   ${text}
                                                </div>`;

        return element;
    }

    function _createSendMessageView(photo, text) {
        let element = ` <div class="bubble me">
                                                   ${text}
                                                </div>`;

     
        return element;
    }


    function _createConversationListItem(data) {

        let element = `<li class="msg-item" data-id="${data.securityStamp}">
                            <div class="d-flex">
                                <div class="d-flex align-items-center img-user-div">
                                    <div class="checkbox mr-2">
                                        <label class="select-all-txt medium">
                                            <input class="individual" type="checkbox"><span class="checkbox-material checkbox-msg"><span class="check"></span></span>
                                        </label>
                                    </div>
                                    <div class="circular-img mr-3">
                                        <img src="/${data.photo}" alt="user-img">
                                    </div>
                                </div>
                                <div class="img-text w-100">
                                    <div class="d-flex participant-info justify-content-between white">
                                        <p class="notification-text extra-bold mb-0 sender-name">${data.name} ${data.surname}</p>
                                        <p class="notification-date medium mb-0">${data.createdDate}</p>
                                    </div>
                                    <p class="mb-0 medium last-message">
                                       ${data.text}
                                    </p>
                                </div>
                            </div>
                        </li>`;


        return element;
    }


    function _createConversationMessageWrapper(data) {
        let element = ` <div style="overflow-y:auto;" data-id="${data.securityStamp}" class="chat mt-3">
                    </div>`;

        return element;
    }

}