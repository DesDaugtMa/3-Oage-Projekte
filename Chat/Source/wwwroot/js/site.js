$(document).ready(function () {
    var inputTexts = document.getElementsByClassName("messageContent");

    //iterate through all found post-contents
    for (var i = 0; i < inputTexts.length; i++) {
        var innerHTML = inputTexts[i].innerHTML;
        innerHTML = innerHTML.replace(/(^|\W)#([a-zA-z0-9]{3,16})/g, "$1<span style='color: orange;'>#$2</span>");
        inputTexts[i].innerHTML = innerHTML;
    }



    // Get newest Messages every second

    const interval = setInterval(function () {

        $.get("/home/getlastmessages", function (data, status) {

            var messagesHtmlString = "";

            $.each(JSON.parse(data), function (i, item) {

                var date = new Date(item.PostedAt);

                // Display Message
                messagesHtmlString += `
                    <div class="row ownContainer">
                        <div class="col-9">
                            <small>
                                <a href="/Account/Profile?username=` + item.User.Username + `" style="color: black;">` + item.User.Username + `</a>
                            </small>
                        </div>

                        <div class="col-3">
                            <small class="text-secondary">
                                ` + item.DateTimeFormated + `
                            </small>
                        </div>

                        <div class="col-12">
                            <p class="messageContent">
                                ` + item.Text + `
                            </p>
                        </div>
                `;

                // If User is logged in, show Like-Button
                if (item.IsUserLoggedIn) {
                    messagesHtmlString += `
                        <div class="col-11"></div>

                        <div class="col-1 text-center">
                            <form>`;
                }

                // If User liked, show full Like-Button, if not, show empty Like-Button
                if (item.DidCurrentUserLike)
                    messagesHtmlString += `<a type="submit" class="btn btn-danger" href="/Home/LikeOrDislike/` + item.Id + `"><i class="bi bi-suit-heart-fill"></i></a></form>`;
                else if (item.IsUserLoggedIn)
                    messagesHtmlString += `<a type="submit" class="btn btn-outline-danger" href="/Home/LikeOrDislike/` + item.Id + `"><i class="bi bi-suit-heart"></i></a></form>`;

                // Show Number of Likes, if User is logged in
                if (item.IsUserLoggedIn) {
                    messagesHtmlString += `
                        <small class="text-danger"><b>` + item.NumberOfLikes + `</b></small>
                    `;
                }

                // Close all HTML-Tags
                messagesHtmlString += `</div ></div>`;

                $("#lastMessagesSentContainer").html(messagesHtmlString);

                var inputTexts = document.getElementsByClassName("messageContent");

                //iterate through all found post-contents
                for (var i = 0; i < inputTexts.length; i++) {
                    var innerHTML = inputTexts[i].innerHTML;
                    innerHTML = innerHTML.replace(/(^|\W)#([a-zA-z0-9]{3,16})/g, "$1<span style='color: orange;'>#$2</span>");
                    inputTexts[i].innerHTML = innerHTML;
                }
            });
        });

    }, 1000);
});
