$(document).ready(function () {
    var inputTexts = document.getElementsByClassName("messageContent");

    //iterate through all found post-contents
    for (var i = 0; i < inputTexts.length; i++) {
        var innerHTML = inputTexts[i].innerHTML;
        innerHTML = innerHTML.replace(/(^|\W)#([a-zA-z0-9]{3,16})/g, "$1<span style='color: orange;'>#$2</span>");
        inputTexts[i].innerHTML = innerHTML;
    }
});
