




async function GetToken() {
    var loginData = new Object();

    var token = "";
    loginData.email = "admin@birpors.az";
    loginData.password = "12345678";
   await $.ajax({
        url: "https://birpors.azurewebsites.net/api/account/get-token",
        type: "POST",
        data: loginData,
        success: function (res) {

            token = res.output;

        }
    });
    return token;
}

const connection = new signalR.HubConnectionBuilder().withUrl("https://birpors.azurewebsites.net/chatHub", {
    accessTokenFactory: async () => await GetToken(),
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
}).build();

connection.keepAliveIntervalInMilliseconds = 1000 * 60 * 60; // Three minutes
connection.serverTimeoutInMilliseconds = 1000 * 60 * 60; // Six minutes

$(document).ready(function () {
    var chat = new Chat();

    chat.startConnection(() => {

        chat.joinChat(() => {

            ////Sidebar-daki elementlere click olunduqda sohbet mesajlari gorsensin
            //$(".participants").on("click", "li.msg-item", function (e) {
            //    //her klikde input temizlensin
            //    if (!$(e.currentTarget).hasClass("active")) {
            //        $("#message").val("");
            //    }

            //    var securtyStamp = $(this).data("id");
            //    $(".chat").removeClass("active");
            //    $(`.chat[data-id="${securtyStamp}"`).empty();

            //    //mesajlarin gorsenmesi ucun div active olunsun
            //    $(`.chat[data-id="${securtyStamp}"`).addClass("active");

            //    chat.getConversationMessages(securtyStamp);
            //});


            //sehife acilan anda aktiv sohbetin mesajlarini getir
            //$(".msg-item.active").trigger("click");

            //Mesaj gonder button-a click olunduqda mesaji gonderilmesi

            $(".chat-form").submit(function (e) {
                e.preventDefault();
                let text = $("#message").val();
                SendMessage(text);

            });

            chat.receiveMessage();

            //chat.makeConversation();
        });

    });

    //$("#message").keyup((e) => {
    //    if ($(e.target).val()) {
    //        $("#btn-send-file").hide();
    //        $("#btn-send-message").show();
    //    }
    //    else {
    //        $("#btn-send-file").show();
    //        $("#btn-send-message").hide();
    //    }
    //});

    function SendMessage(text) {
        var securityStamp = $(".chat.active-chat").data('conversation');
        console.log(text)
        console.log(securityStamp)
        if (text && securityStamp) {
            chat.sendMessage(text, securityStamp);
            $("#message").val("");
        }
    }

});