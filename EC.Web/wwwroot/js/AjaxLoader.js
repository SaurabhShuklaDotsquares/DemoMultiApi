$(document).ajaxStart(function () {
    //if(!DisableAjaxLoader)
   $('.loading-common,.loading-overlay').show()
});

$(document).ajaxStop(function () {
    //if (!DisableAjaxLoader)
   $('.loading-common,.loading-overlay').hide()
});

$(document).ajaxError(function () {
    $('.loading-common,.loading-overlay').hide()
});
$(document).ajaxComplete(function () {
    $('.loading-common,.loading-overlay').hide()
});