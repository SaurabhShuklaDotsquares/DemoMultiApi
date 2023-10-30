/*global window, $*/
var Global = {
    SelectedFiles: [],
    MessageType: {
        Success: 0,
        Error: 1,
        Warning: 2,
        Info: 3
    }
};

Global.OldFormHelper = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var methodPost = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
        formElement = $(formElement);
        var settings = {};
        settings = $.extend({}, settings, options);
        $.validator.unobtrusive.parse(formElement);
        if (settings.validateSettings !== null && settings.validateSettings !== undefined) {
            formElement.validate(settings.validateSettings);
        }
        formElement.off("submit").submit(function (e) {

            e.preventDefault();
            e.stopImmediatePropagation();

            var formdata = new FormData();
            formElement.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
                if (elem.files && elem.files.length) {
                    for (var i = 0; i < elem.files.length; i++) {
                        var file = elem.files[i];
                        formdata.append(elem.getAttribute('name'), file);
                    }
                }
            });

            $.each(formElement.serializeArray(), function (i, item) {
                formdata.append(item.name, item.value);
            });

            if (options && options.updateFormData) {
                var updateformdata = options.updateFormData(formdata);
                if (updateformdata !== null && updateformdata !== undefined) {
                    formdata = updateformdata;
                }
            }


            var submitBtn = formElement.find('.btn-primary');
            if (formElement.validate().valid() && formElement.valid()) {

                if (options && options.beforeSubmit) {
                    if (!options.beforeSubmit()) {
                        return false;
                    }
                }

                var $buttonI = submitBtn.find('i');
                submitBtn.attr({ "data-visible-class": submitBtn.attr("class") }, { "data-text": submitBtn.find("span").text() });
                //submitBtn.removeClass(submitBtn.attr("class"));
                submitBtn.addClass("spinning");
                //submitBtn.prop('disabled', true);
                submitBtn.attr('disabled', 'disabled');
                submitBtn.find('span').html('Submiting..');

                $.ajax(formElement.attr("action"), {
                    type: "POST",
                    data: formdata,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                            $("#" + settings.loadingElementId).show();
                            submitBtn.hide();
                        }
                    },
                    success: function (result) {
                        if (onSucccess === null || onSucccess === undefined) {
                            if (result.isSuccess) {
                                window.location.href = result.redirectUrl;
                            } else {
                                if (settings.updateTargetId) {
                                    var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                    $("#" + settings.updateTargetId).html(datatresult);
                                }
                            }
                        } else {
                            onSucccess(result);
                        }
                    },
                    error: function (jqXHR, status, error) {
                        if (onError !== null && onError !== undefined) {
                            onError(jqXHR, status, error);
                            $("#loadingElement").hide();
                        }
                    },
                    complete: function (result) {
                        if (onComplete === null || onComplete === undefined) {
                            if (settings.loadingElementId !== null || settings.loadingElementId !== undefined) {
                                $("#" + settings.loadingElementId).hide();
                            }
                            submitBtn.removeClass("spinning");
                            submitBtn.addClass(submitBtn.attr("data-visible-class"));
                            submitBtn.find('span').text(submitBtn.attr("data-text"));
                            //submitBtn.prop('disabled', false);
                            submitBtn.removeAttr('disabled');
                        } else {
                            onComplete(result);
                        }

                    }
                });
            }

            e.preventDefault();
        });
    }
    //if ($(formElement).length > 0) {
    //    methodPost($(formElement), options, onSucccess, onError, loadingElementId, onComplete);
    //}
    //else {
    //    setTimeout(function () {
    //        methodPost($(formElement), options, onSucccess, onError, loadingElementId, onComplete);
    //    }, 500);
    //}

    return formElement;
};
Global.ModalClear = function (modalElement) {
    if ($(modalElement).length > 0) {
        $(modalElement).removeData('bs.modal');
        $(modalElement).find(".modal-content").html("");
    }
};
Global.FormHelper = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    if (settings.validateSettings !== null && settings.validateSettings !== undefined) {
        formElement.validate(settings.validateSettings);
    }
    formElement.off("submit").submit(function (e) {

        e.preventDefault();
        e.stopImmediatePropagation();

        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid() && formElement.valid()) {
            var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();

            if (options && options.beforeSubmit) {
                if (!options.beforeSubmit()) {
                    return false;
                }
            }


            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formElement.serializeArray(),
                beforeSend: function () {
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                    }
                    submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                    $(':input[type="submit"]').prop('disabled', true);
                },
                success: function (result) {
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        $("#loadingElement").hide();
                    }
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).hide(); submitBtn.show();
                    }
                    submitBtn.prop('disabled', false);
                },
                complete: function () {
                    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    $(':input[type="submit"]').prop('disabled', false);
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).hide(); submitBtn.show();
                    }
                    submitBtn.prop('disabled', false);
                }
            });
        }

        e.preventDefault();
    });
    return formElement;
};
Global.FormHelperWithFiles = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    formElement.validate(settings.validateSettings);
    formElement.submit(function (e) {
        
        if (options && options.beforeSubmit) {
            if (!options.beforeSubmit()) {
                return false;
            }
        }

        var formdata = new FormData();

        formElement.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
            if (elem.files && elem.files.length) {
                for (var i = 0; i < elem.files.length; i++) {
                    var file = elem.files[i];
                    formdata.append(elem.getAttribute('name'), file);
                }
            }
        });

        $.each(formElement.serializeArray(), function (i, item) {
            formdata.append(item.name, item.value);
        });

        var submitBtn = formElement.find(':submit');
        //For show progress bar Drj-11092020
        //var bar = formElement.find('.bar');
        //var percent = formElement.find('.percent');
        //var status = formElement.find('#status');

        if (formElement.validate().valid()) {
            submitBtn.find('i').removeClass("fa fa-arrow-circle-right");
            submitBtn.find('i').addClass("fa fa-refresh");
            submitBtn.prop('disabled', true);
            submitBtn.find('span').html('Submiting..');
            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                        submitBtn.hide();
                    }
                   
                    if (settings.isProgress != null && settings.isProgress != undefined) {
                       //settings.status.empty();
                        var percentVal = '0%';
                        settings.bar.width(percentVal);
                        settings.percent.html(percentVal);
                    }
                },
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    //if require to show progress bar 
                    if (settings.isProgress != null && settings.isProgress != undefined) {
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = parseInt((evt.loaded / evt.total) * 100);
                                var percentVal = parseInt(percentComplete) + '%';
                                settings.bar.width(percentVal);
                                settings.percent.html(percentVal);
                            }
                        }, false);
                    }
                    return xhr;
                },
                success: function (result) {
                   
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        $("#loadingElement").hide();
                    }
                },
                complete: function (result) {
                    if (onComplete === null || onComplete === undefined) {
                        if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                            $("#" + settings.loadingElementId).hide();
                        }
                        submitBtn.find('i').removeClass("fa fa-refresh");
                        submitBtn.find('i').addClass("fa fa-arrow-circle-right");
                        submitBtn.find('span').html('Submit');
                        submitBtn.prop('disabled', false);
                    } else {
                        onComplete(result);
                    }
                }
            });
        }

        e.preventDefault();
    });

    return formElement;
};

Global.FormHelperWithFiles1 = function (formElement, options, onSucccess, onError) {
    //"use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement)
    formElement.validate(settings.validateSettings);

    formElement.off("submit").submit(function (e) {

        var submitBtn = formElement.find(':submit');

        if (formElement.validate().valid() && formElement.valid()) {


            var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();

            var formdata = new FormData();
            formElement.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
                if (elem.files && elem.files.length) {
                    for (var i = 0; i < elem.files.length; i++) {
                        var file = elem.files[i];
                        formdata.append(elem.getAttribute('name'), file);

                    }
                }
            });

            $.each(formElement.serializeArray(), function (i, item) {
                formdata.append(item.name, item.value);
            });

            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {

                    submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                    $(':input[type="submit"]').prop('disabled', true);

                },
                success: function (result) {

                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                if (result.data == undefined) {
                                    $("#" + settings.updateTargetId).html("<span>" + result + "</span>");
                                }
                                else {
                                    $("#" + settings.updateTargetId).html("<span>" + result.data + "</span>");
                                }
                            }
                        }
                    } else {

                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {

                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                    }
                    //submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    //$(':input[type="submit"]').prop('disabled', false);
                }, complete: function () {

                    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    $(':input[type="submit"]').prop('disabled', false);
                }
            });
        }
        e.preventDefault();
    });

    return formElement;
};
Global.AjaxPost = function (options, onSucccess, onError, onComplete, asyncRequest) {
    "use strict";
    asyncRequest = asyncRequest === null || asyncRequest === undefined || asyncRequest === true;
    if (options && options.beforeSubmit) {
        if (!options.beforeSubmit()) {
            return false;
        }
    }
    var formdata = new FormData();
    if (options && options.updateFormData) {
        var updateformdata = options.updateFormData(formdata);
        if (updateformdata !== null && updateformdata !== undefined) {
            formdata = updateformdata;
        }
    }
    $.ajax(options.url, {
        type: "POST",
        data: formdata,
        contentType: false,
        processData: false,
        async: asyncRequest,
        success: function (result) {
            if (onSucccess === null || onSucccess === undefined) {
                if (result.isSuccess) {
                    window.location.href = result.redirectUrl;
                } else {
                    if (settings.updateTargetId) {
                        var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                        $("#" + settings.updateTargetId).html(datatresult);
                    }
                }
            } else {
                onSucccess(result);
            }
        },
        error: function (jqXHR, status, error) {
            if (onError !== null && onError !== undefined) {
                onError(jqXHR, status, error);
                $("#loadingElement").hide();
            }
        }
    });
};



Global.IsNull = function (o) { return typeof o === "undefined" || typeof o === "unknown" || o == null };
Global.IsNotNull = function (o) { return !Global.IsNull(o); };
Global.IsNullOrEmptyString = function (str) {
    return Global.IsNull(str) || typeof str === "string" && $.trim(str).length == 0
};
Global.IsNotNullOrEmptyString = function (str) { return !Global.IsNullOrEmptyString(str); };

//Global.GridHelper = function (gridElement, options) {
//    if ($(gridElement).length > 0) {
//        var settings = {};
//        settings = $.extend({}, settings, options);
//        $.fn.dataTable.ext.errMode = 'throw';
//        return $(gridElement).DataTable(settings);
//    }
//};

Global.GridHelper = function (gridElement, options) {
    if ($(gridElement).length > 0) {
        var settings = {};
        settings = $.extend({}, settings, options);
        return $(gridElement).dataTable(settings);
        //return $(gridElement);
    }
};

Global.FormValidationReset = function (formElement, validateOption) {
    if ($(formElement).data('validator')) {
        $(formElement).data('validator', null);
    }

    $(formElement).validate(validateOption);
    return $(formElement);
};

Global.DateProcess = function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
Global.Confirm = function (title, message, okCallback, cancelCallback) {
    return alertify.confirm(title, message, function () {
        if (okCallback)
            okCallback();
    }, function () {
        if (cancelCallback)
            cancelCallback();
    }).set({ transition: 'fade', 'closable': false });
};
Date.prototype.isSameDateAs = function (pDate) {
    return (
        this.getFullYear() === pDate.getFullYear() &&
        this.getMonth() === pDate.getMonth() &&
        this.getDate() === pDate.getDate()
    );
}
Global.ShowMessage = function (message, type) {
    if (type === Global.MessageType.Success) {
        alertify.success(message);
    }
    else if (type === Global.MessageType.Error) {
        alertify.error(message);
    }
    else if (type === Global.MessageType.Warning) {
        alertify.warning(message);
    }
    else if (type === Global.MessageType.Info) {
        alertify.message(message);
    }
};
Global.Alert = function (title, message, callback) {
    alertify.alert(title, message, function () {
        if (callback)
            callback();
    }).set({ transition: 'fade' });
    if (title === "Alert!") {
        $('.ajs-content').addClass('error-text');
    }
    else {
        $('.ajs-content').removeClass('error-text');
    }
};

Global.ValidateImage = function (input, fileId, displayId, alertId) {
    
    var reader = new FileReader();
    if (input.files[0].size > 528385) {
        $("#" + fileId).val('');
        $("#" + displayId).removeAttr("src");
        $("#" + displayId).hide();
       
        $("#" + alertId).html("Image Size should not be greater than 500Kb");

        $("#" + fileId).val('');
        return false;
    }

    if (input.files[0].type.indexOf("image/jpeg") == -1 && input.files[0].type.indexOf("image/png") == -1) {
        $("#" + alertId).html("Invalid format(Only .png,.jpeg,.jpg Allowed)");
        $("#" + fileId).val('');
        $("#" + displayId).removeAttr("src");
        $("#" + displayId).hide();
        return false;
    }
    else
        $("#" + alertId).html("");


    reader.onload = function (e) {
        $("#" + displayId).show();
        $("#" + displayId).attr("src", e.target.result);
    };
    // read the image file as a data URL.
    reader.readAsDataURL(input.files[0]);
}

Global.ValidateImageBySize = function (input, fileId, min, max, Id) {
    debugger;
    var minSizeKB = min; //Size in KB
    var maxSizeKB = max; //Size in KB
    var minSize = minSizeKB * 1024;
    var maxSize = maxSizeKB * 1024; //File size is returned in Bytes
    if (input.files[0].size <= minSize || input.files[0].size >= maxSize) {
        $(this).val("");
        $("#" + fileId).val('');
        $('#' + Id).text("Size should be equal " + min + " kb "+ " and less than " + max +" kb.");
        return false;
    }
    else {
        $('#' + Id).text('')
    }
}

$(document).on('keypress', '.number', function (event) {
    //Added by arnav
    if ((event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

/*Arnav*/
$(document).on('keypress', '.decimal', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('keypress', '.decimalNegative', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('change', '.number, .decimal, .decimalNegative', function () { if ($(this).val() != "0" && $(this).val() != "" && !$.isNumeric($(this).val())) { alert("Please enter a valid value"); return false; } });

$(document).on('focus', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val() == default_value) $(this).val("");
});

$(document).on('blur', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val().length == 0) $(this).val(default_value);
});

$(document).on('bind', '.disablecopy', function (e) {
    e.preventDefault();
});

$(document).on('cut copy paste', '.disablecopy', function (e) {
    e.preventDefault();
});

$(".disableRightClick").on("contextmenu", function (e) {
    return false;
});

Global.TotalCartItem = function getCartItemCount() {
    $.post(domain + "JsonData/GetCartItemCount", function (result) {
        var spnTotalCartItem = $('#spnTotalCartItem');
        spnTotalCartItem.html('');
        spnTotalCartItem.removeAttr("class");

        if (result != null) {
            if (result != 0) {
                spnTotalCartItem.attr('class', 'caret solid-blue');
                spnTotalCartItem.html(result);
            }
        }
    })
}


Global.CartMessage = function showCartMessage(message) {
    var dvNotifyBar = $('#dvNotifyBar');
    var spnNotifyBar = $('#spnNotifyBar');
    spnNotifyBar.html(message);

    dvNotifyBar.removeClass('hide');
    dvNotifyBar.addClass('show');

    dvNotifyBar.fadeTo(3000, 500).slideUp(500, function () {
        dvNotifyBar.slideUp(500);
        dvNotifyBar.removeClass('show');
        dvNotifyBar.addClass('hide');

    });
}

$("div[role=dialog].modal").on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var url = button.attr("href");
    if (url !== "") {
        var modal = $(this);
        // note that this will replace the content of modal-content everytime the modal is opened
        modal.find('.modal-content').load(url);
    }
}).on('hidden.bs.modal', function (e) {
    $(this).removeData('bs.modal');
    $(this).find(".modal-content").empty();
});

$(document).on('click', '.closenotification', function () {
    var dvNotifyBar = $('#dvNotifyBar');
    dvNotifyBar.removeClass('show');
    dvNotifyBar.addClass('hide');
});

Global.CartMessageForPayment = function showCartMessageForPayment(message, redirectUrl) {
    var dvNotifyBar = $('#dvNotifyBar');
    var spnNotifyBar = $('#spnNotifyBar');
    spnNotifyBar.html(message);

    dvNotifyBar.removeClass('hide');
    dvNotifyBar.addClass('show');

    dvNotifyBar.fadeTo(3000, 500).slideUp(500, function () {
        dvNotifyBar.slideUp(500);
        dvNotifyBar.removeClass('show');
        dvNotifyBar.addClass('hide');
        window.location = redirectUrl;
    });
}

Global.showsPartial = function ($url, $divId) {
    $.ajax({
        url: $url,
        type: 'GET',
        async: false,
        crossDomain: true,
        cache: false,
        success: function (htmlElement) {
            $('#' + $divId).empty().html(htmlElement);
        }
    });
};

Global.SingleDatePicker = function (parentElement, options) {
    var defaultOptions = {
        drops: 'down',
        autoUpdateInput: false,
        singleDatePicker: true,
        autoApply: false,
        locale: {
            format: 'DD/MM/YYYY',
        }
    };

    if (options) {
        defaultOptions = $.extend({}, defaultOptions, options);
    }

    parentElement.find('.datepicker').daterangepicker(defaultOptions).on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.endDate.format(defaultOptions.locale.format)).change();
    })
        .on('keypress paste', function (e) {
            e.preventDefault();
            return false;
        }).attr("autocomplete", "off");
};
$(document).on('keypress keyup', '.form-control', function (e) {
    if (e.which === 32 && !this.value.length)
        e.preventDefault();
});

Global.DatePicker = function (dateElement, options) {
    if (dateElement.length > 0 && dateElement!= undefined) {
        if (options == undefined) {
            options = { format: "dd/mm/yyyy", autoclose: true };
        }
        $(dateElement).datepicker(options);
    }
}

//$("div[role=dialog].modal").on('show.bs.modal', function (event) {

//    var button = $(event.relatedTarget); // Button that triggered the modal
//    var url = button.attr("href");
//    if (url !== "") {
//        var modal = $(this);
//        // note that this will replace the content of modal-content everytime the modal is opened
//        modal.find('.modal-content').load(url);
//    }
//}).on('hidden.bs.modal', function (e) {
//    $(this).removeData('bs.modal');
//    $(this).find(".modal-content").empty();
//});

//$("div[role=dialog].modal").on('show.bs.modal', function (event) {
//    $(this).find('.modal-content').load(event.relatedTarget.href);
//}).on('hidden.bs.modal', function (e) {
//    $(this).removeData('bs.modal');
//    $(this).find(".modal-content").empty();
//});
Global.bindMaxLength = function () {
    //$('input[maxlength], textarea[maxlength]').off("maxlength").maxlength({
    //    alwaysShow: true, //if true the threshold will be ignored and the remaining length indication will be always showing up while typing or on focus on the input. Default: false.
    //    // threshold: 10, //Ignored if alwaysShow is true. This is a number indicating how many chars are left to start displaying the indications. Default: 10
    //    warningClass: "form-text text-muted mt-1", //it's the class of the element with the indicator. By default is the bootstrap "badge badge-success" but can be changed to anything you'd like.
    //    limitReachedClass: "form-text text-muted mt-1", //it's the class the element gets when the limit is reached. Default is "badge badge-danger". Replace with text-danger if you want it to be red.
    //    //separator: ' of ', //represents the separator between the number of typed chars and total number of available chars. Default is "/".
    //    //preText: 'You have ', //is a string of text that can be outputted in front of the indicator. preText is empty by default.
    //    //postText: ' chars remaining.', //is a string outputted after the indicator. postText is empty by default.
    //    showMaxLength: true, //showMaxLength: if false, will display just the number of typed characters, e.g. will not display the max length. Default: true.
    //    showCharsTyped: true, //if false, will display just the remaining length, e.g. will display remaining lenght instead of number of typed characters. Default: true.
    //    placement: 'centered-right', //is a string, object, or function, to define where to output the counter. 
    //    //Possible string values are: bottom( default option ), left, top, right, bottom - right, top - right, top - left, bottom - left and centered - right.
    //    //Are also available: ** bottom - right - inside ** (like in Google's material design, **top-right-inside**, **top-left-inside** and **bottom-left-inside**. 
    //    //stom placements can be passed as an object, with keys top, right, bottom, left, and position.These are passed to $.fn.css.
    //    //A custom function may also be passed.This method is invoked with the { $element } Current Input, the { $element } MaxLength Indicator, and the Current Input's Position { bottom height left right top width }.

    //    appendToParent: true, // appends the maxlength indicator badge to the parent of the input rather than to the body.
    //    //message: an alternative way to provide the message text, i.e. 'You have typed %charsTyped% chars, %charsRemaining% of %charsTotal% remaining'. %charsTyped%, %charsRemaining% and %charsTotal% will be replaced by the actual values. This overrides the options separator, preText, postText and showMaxLength. Alternatively you may supply a function that the current text and max length and returns the string to be displayed. For example, function(currentText, maxLength) { return '' + Math.ceil(currentText.length / 160) + ' SMS Message(s)';}
    //    utf8: true, //if true the input will count using utf8 bytesize/encoding. For example: the '£' character is counted as two characters.
    //    showOnReady:true, // shows the badge as soon as it is added to the page, similar to alwaysShow
    //    twoCharLinebreak:true, //count linebreak as 2 characters to match IE/Chrome textarea validation
    //    //customMaxAttribute: String -- allows a custom attribute to display indicator without triggering native maxlength behaviour. Ignored if value greater than a native maxlength attribute. 'overmax' class gets added when exceeded to allow user to implement form validation.
    //    allowOverMax:false //Will allow the input to be over the customMaxLength. Useful in soft max situations.
    //});

    !function ($) {
        $.fn.maxlength = function () {
            $(this).each(function () {
                var max = $(this).attr('maxlength');

                if (max <= 0 || max === undefined) {
                    throw new Error('maxlength attribute must be defined and greater than 0');
                }

                if (!$(this).parent().hasClass('input-group')) {
                    $(this).wrap("<div class=\"input-group\"></div>");
                }
                if ($(this).parent().find("div.input-group-append").length == 0) {
                    $(this).after("<div class=\"input-group-append maxlength-container\"><span class=\"input-group-text maxlength1\"></span></div>");
                }

                $(this).bind('input', function (e) {
                    var max = $(this).attr('maxlength');
                    var val = $(this).val();
                    var cur = 0;

                    if (val) {
                        cur = val.length;
                    }

                    var left = max - cur;

                    $(this).next(".maxlength-container").find(".maxlength").text(left.toString());

                    return this;
                }).trigger('input');
            });
            return this;
        };
    }(window.jQuery);
    $('input[maxlength]:visible:not(.no-maxlength), textarea[maxlength]:visible:not(.ckeditor,.no-maxlength)').off("maxlength").maxlength();
};
Global.bindMaxLength();

Global.ResetCtrlIdIndexing = function (newPropertyName, ...args) {
    $(args).each(function () {
        var ctrlName = $(this).attr("name");
        if (ctrlName) {
            var propertyName = ctrlName.substring(0, ctrlName.lastIndexOf("["));
            ctrlName = ctrlName.substring(ctrlName.lastIndexOf(".") + 1, ctrlName.length);
            var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
            var newPropertyId = newPropertyName != null ? newPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') : propertyId;
            //if (!newPropertyName || Global.IsNullOrEmptyString(newPropertyName)) {
            //    newPropertyName = propertyName;
            //}
            //var childCtrls = $('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]').parents(".flexible-content-box").find('.flexible-content-box [id^="' + propertyId + '_"][id$="_' + ctrlName + '"]');
            //var childLabels = $('label[for^="' + propertyId + '_"][for$="_' + ctrlName + '"]').parents(".flexible-content-box").find('.flexible-content-box label[for^="' + propertyId + '_"][for$="_' + ctrlName + '"]');
            //var index = 0;
            //$('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]').not(childCtrls).each(function (i, element) {


            var index = 0;
            if (newPropertyName && Global.IsNotNullOrEmptyString(newPropertyName)) {
                $('[id^="' + newPropertyId + '_"][id$="_' + ctrlName + '"]').each(function (i, element) {
                    var curentCtrlName = $(this).attr("name");
                    var isChildCtrl = curentCtrlName.replace(newPropertyName, '').split(']').length > 2;
                    if (!isChildCtrl) {
                        $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                        index++;
                    }
                });
            }
            if (newPropertyName != propertyName) {
                $('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]').each(function (i, element) {
                    var curentCtrlName = $(this).attr("name");
                    var isChildCtrl = curentCtrlName.replace(propertyName, '').split(']').length > 2;
                    if (!isChildCtrl) {
                        $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                        index++;
                    }
                });
            }


            index = 0;
            if (newPropertyName && Global.IsNotNullOrEmptyString(newPropertyName)) {
                $('label[for^="' + newPropertyId + '_"][for$="_' + ctrlName + '"]').each(function (i, element) {
                    var isChildCtrl = $(this).attr("for").replace(newPropertyId, '').split('__').length > 2;
                    if (!isChildCtrl) {
                        $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                        index++;
                    }
                });
            }
            if (newPropertyName != propertyName) {
                $('label[for^="' + propertyId + '_"][for$="_' + ctrlName + '"]').each(function (i, element) {
                    var isChildCtrl = $(this).attr("for").replace(propertyId, '').split('__').length > 2;
                    if (!isChildCtrl) {
                        $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                        index++;
                    }
                });
            }
        }
    });
};

Global.AttachEventCKEditor = function (instance) {
    CKEDITOR.instances[instance].on("instanceReady", function (e) {
        this.on("change", function () {
            CKEDITOR.instances[instance].updateElement();
        });
    });
}

Global.resetFlexibleContentBoxCtrlIndexing = function ($addNewButton) {    
    var $appendToControl = $addNewButton.parents(".main-dynamically-added-container:first").find($addNewButton.data("append-to-control"));
    var propertyName = $addNewButton.data("property-name");
    var controlNames = $addNewButton.data("control-names");
    var parentControlName = $addNewButton.data("parent-control-name");
    controlNames = controlNames.split(',');
    var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
    var newPropertyName = propertyName;


    $(controlNames).each(function (index, ctrlName) {
        if (parentControlName && parentControlName.length > 0) {
            var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));

            var $parentCtrl = $appendToControl.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
            if ($parentCtrl && $parentCtrl.length > 0) {
                parentPropertyName = $parentCtrl.attr("name");
                parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
            }
        }

        if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
            newPropertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
            propertyId = newPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
        }

        var $ctrl = $appendToControl.find('[id$="_' + ctrlName + '"]:first')
        if ($ctrl) {
            Global.ResetCtrlIdIndexing(newPropertyName, $ctrl);
        }
    });
};

$(document).on("click", "button.remove-dynamically-added i.fa-remove", function () {
    var $container = $(this).parents(".flexible-content-box:first");
    var $addNewButton = $container.parents(".main-dynamically-added-container:first").find("button.add-multiple-section:last");
    //var $appendToControl = $addNewButton.parents(".main-dynamically-added-container:first").find($addNewButton.data("append-to-control"));
    //var propertyName = $addNewButton.data("property-name");
    var controlNames = $addNewButton.data("control-names");
    //var parentControlName = $addNewButton.data("parent-control-name");
    controlNames = controlNames.split(',');
    //var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
    //var newPropertyName = propertyName;

    $container.fadeOut("fast", function () {
        $container.remove();
        $(controlNames).each(function (index, ctrlName) {
            Global.resetFlexibleContentBoxCtrlIndexing($addNewButton);
            //if (parentControlName && parentControlName.length > 0) {
            //    var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));

            //    var $parentCtrl = $appendToControl.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
            //    if ($parentCtrl && $parentCtrl.length > 0) {
            //        parentPropertyName = $parentCtrl.attr("name");
            //        parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
            //    }
            //}

            //if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
            //    newPropertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
            //    propertyId = newPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
            //}

            //var $ctrl = $('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]:first')
            //if ($ctrl) {
            //    Global.ResetCtrlIdIndexing(newPropertyName, $ctrl);
            //}


        });
        $(".flexible-content-box input[type=file].app-img-uploader").each(function () {
            $.resetImageCtrlIndexing($(this));
        });
        $(".main-dynamically-added-container .main-dynamically-added-container button.add-multiple-section").each(function () {
            Global.resetFlexibleContentBoxCtrlIndexing($(this));
        });
    });
});
$(document).on("click", "button.add-multiple-section", function () {
    var $button = $(this);
    var url = $(this).data("action-url");
    var $appendToControl = $(this).parents(".main-dynamically-added-container:first").find($(this).data("append-to-control"));
    var propertyName = $(this).data("property-name");
    var controlNames = $(this).data("control-names");
    var parentControlName = $(this).data("parent-control-name");
    var callback = $(this).data("fncallback");
    controlNames = controlNames.split(',');
    var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
    var newPropertyName = propertyName;

    Global.AjaxPost({ url: url }, function (result) {
        $appendToControl.append(result).promise().done(function () {

            $(controlNames).each(function (index, ctrlName) {
                if (parentControlName && parentControlName.length > 0) {
                    var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));
                    //parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('_'));

                    var $parentCtrl = $appendToControl.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
                    if ($parentCtrl && $parentCtrl.length > 0) {
                        parentPropertyName = $parentCtrl.attr("name");
                        parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
                    }
                }

                if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
                    newPropertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
                }

                var $ctrl = $appendToControl.find('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]:first');
                if ($ctrl) {
                    Global.ResetCtrlIdIndexing(newPropertyName, $ctrl);
                }
            });

            Global.bindMaxLength();

            if (callback && Global.IsNotNullOrEmptyString(callback)) {
                eval(callback);
            }

            var $currentFlexibleContent = $appendToControl.find('.flexible-content-box:last');
            $currentFlexibleContent.find('textarea.ckeditor').each(function () {
                var ckeditorCtrlId = $(this).attr('id');
                if (ckeditorCtrlId != undefined && Global.IsNotNullOrEmptyString(ckeditorCtrlId) && ckeditorCtrlId.length > 0) {
                    CKEDITOR.replace(ckeditorCtrlId, {});
                    Global.AttachEventCKEditor(ckeditorCtrlId);
                }
            });

            $('html, body').animate({
                scrollTop: $button.offset().top - ($currentFlexibleContent.height() + 150)
            }, 1000);

            $button.focus();
        });
    });
});

String.prototype.replaceBetween = function (start, end, value) {
    return this.substring(0, start) + value + this.substring(end);
};
function reindexLast(ele, index) {

    var i = $(ele).attr("name");
    var start = i.lastIndexOf("[");
    var end = i.lastIndexOf("]") + 1;
    i = i.replaceBetween(start, end, "[" + index + "]");
    $(ele).attr("name", i);
}

Global.ReIndexList = function (list) {

    if (list.length) {

        var i = 0;
        list.each(function (f, g) {
            $(g).find(":input.reindex:not(:disabled)").each(function (h, j) {
                reindexLast(j, i);
            });
            i++
        });
    }
};
$(".alert-success,alert-danger").fadeTo(5000, 1000).slideUp(1000, function () {
    $(".alert-success,alert-danger").slideUp(1000);
});
//$(".alert-success,.alert-danger").slideUp(10000);

//Global.SelectAutoComplete = function (url, selectId, placeholder) {
//    $("." + selectId).select2({
//        placeholder: placeholder,
//        multiple: true,
//        allowClear: true,
//        tags: false,
//        async: true,
//        minimumInputLength: 0,
//        closeOnSelect: false,
//        width: 'resolve',
//        escapeMarkup: function (markup) { return markup; },
//        ajax: {
//            url: domain + url,
//            dataType: 'json',
//            data: function (params) {
//                return {
//                    search: params.term,
//                    page: params.page
//                };
//            },
//            processResults: function (response, params) {
//                params.page = params.page || 0;
//                var items = [];
//                for (var i = 0; i < response.data.length; i++) {
//                    items.push({
//                        id: response.data[i].id,
//                        text: response.data[i].name,
//                    });
//                }
//                return {
//                    results: items,
//                    pagination: {
//                        more: (params.page * 5) < response.totalItems
//                    }
//                };
//            },
//        },
//        cache: true,
//    });
//}

Global.ValidateTabCtrl = function ($form) {
    $('.tab-no-validation input, .tab-no-validation select, .tab-no-validation textarea').each(function () {
        $(this).rules('remove', 'required');
    });
    var _validator = $form.data('validator');
    _validator.settings.ignore = "";

    var isValid = $form.validate().valid();
    if (!isValid) {


        var $ctrlToFocus = null;
        var makeActiveTab = function (ctrlObject) {
            if (!_validator.element(ctrlObject)) {
                var $ctrl = $(ctrlObject);
                var items = $form.validate();
                var $tabls = $ctrl.parents('div.tab-pane');
                if ($tabls.length > 0) {
                    if ($ctrlToFocus == null) {
                        $tabls.each(function (index, item) {
                            var tabId = $(item).attr('id');
                            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
                            //var tabId = $ctrl.parents('div.tab-pane').find('div.tab-pane.fade').attr('id')
                            //var innerTabId = $(this).parents("div.tab-pane:first").attr('id');
                            //$('.nav-tabs a[href="#' + innerTabId + '"]').tab('show');
                            //$('.nav-tabs a[href="#' + tabId + '"]').tab('show');
                            $ctrlToFocus = $ctrl;
                            isValid = false;
                        });
                    }
                }
                else {
                    $ctrlToFocus = $ctrl;
                }
                return false;
            }
            return true;
        }

        $form.find('input[type=text], select, textarea').each(function () {
            if ($ctrlToFocus == null && !_validator.element(this)) {
                if (!makeActiveTab(this)) {
                    $ctrlToFocus.focus();
                }
            }
        });
    }
    return isValid;
}


Global.TabAutocollapse = function (menu, maxHeight) {
    var nav = $(menu)
    var navHeight = nav.innerHeight()
    if (navHeight >= maxHeight) {

        $(menu + ' .dropdown').removeClass('d-none');
        $(".navbar-nav").removeClass('w-auto').addClass("w-100");

        while (navHeight > maxHeight) {
            //  add child to dropdown
            var children = nav.children(menu + ' li:not(:last-child)');
            var count = children.length;
            $(children[count - 1]).prependTo(menu + ' .dropdown-menu');
            navHeight = nav.innerHeight();
        }
        $(".navbar-nav").addClass("w-auto").removeClass('w-100');

    }
    else {

        var collapsed = $(menu + ' .dropdown-menu').children(menu + ' li');

        if (collapsed.length === 0) {
            $(menu + ' .dropdown').addClass('d-none');
        }

        while (navHeight < maxHeight && (nav.children(menu + ' li').length > 0) && collapsed.length > 0) {
            //  remove child from dropdown
            collapsed = $(menu + ' .dropdown-menu').children('li');
            $(collapsed[0]).insertBefore(nav.children(menu + ' li:last-child'));
            navHeight = nav.innerHeight();
        }

        if (navHeight > maxHeight) {
            Global.TabAutocollapse(menu, maxHeight);
        }

    }
};
