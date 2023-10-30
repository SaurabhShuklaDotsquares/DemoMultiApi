(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            /*ckEditor();*/
            var gridCms = new Global.GridHelper('#grid-Tax', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": false },
                    { "targets": [4], "sortable": true, "searchable": false },
                   
                    {
                        "targets": 5,
                        "sortable": false,
                        "searchable": false,
                        "data": "5",
                        "render": function (data, type, row, meta) {
                            var json = {
                                type: "checkbox",
                                class: "switchBox switch-small",
                                value: row[0],
                                'data-on': "success",
                                'data-off': "danger"
                            };

                            if (data === "True") {
                                json.checked = true;
                            }
                            return $('<input/>', json).get(0).outerHTML;
                        }
                    },
                    { "targets": [6], "sortable": true, "searchable": false },
                    {
                        "targets": [7], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Taxs/Create/" + row[0],
                                id: "addeditModel",
                                title: "Edit",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-addedit-detail",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[0] != 8) {
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Taxs/delete/" + row[0],
                                    id: "deleteType",
                                    title: "Delete",
                                    class: "btn btn-danger btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete",
                                    html: $("<i/>", {
                                        class: "fa fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;"
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Taxs/View/" + row[0],
                                    id: "deleteType",
                                    title: "View",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "form",
                                    'data-target': "#viewModel",
                                    html: $("<i/>", {
                                        class: "fa fa-eye"
                                    }),
                                }).append("").get(0).outerHTML;
                            }
                            return actionLink;
                        }
                    }
                ],
                "direction": "rtl",
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/admin/Taxs/Index",
                "fnServerData": function (url, data, callback) {
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "fnDrawCallback": function (oSettings) {
                    /*ckEditor();*/
                    initGridControlsWithEvents();
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                    }
                    else {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                    }
                },

                "stateSaveCallback": function (settings, data) {
                    localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
                },
                "stateLoadCallback": function (settings) {
                    return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
                },
                "language": {
                    "searchPlaceholder": "Search by title"
                }
            });
            table = gridCms.DataTable();
            table.search("").draw();
        }

        function initGridControlsWithEvents() {
            if ($('.switchBox').data('bootstrapSwitch')) {
                $('.switchBox').off('switchChange.bootstrapSwitch');
                $('.switchBox').bootstrapSwitch('destroy');
            }

            $('.switchBox').bootstrapSwitch()
                .on('switchChange.bootstrapSwitch', function () {
                    var switchElement = this;
                    $.get(domain + '/admin/Taxs/ActiveTaxStatus', { id: this.value }, function (result) {
                        if (!result.isSuccess) {
                            $(switchElement).bootstrapSwitch('toggleState', true);

                        }
                        else {
                            alertify.success(result.data);
                            // $("#validation-summary").html(result.data);
                        }
                    });
                });
        }

        function initilizeModel() {
            $("#modal-addedit-detail").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-addedit-detail .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelper($("#modal-addedit-detail").find("#frm-add-edit-Tax form"), { updateTargetId: "validation-summary" }, function (data) {
                        //console.log(data.isSuccess);
                        if (data.isSuccess == true) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            $('.errorCategory').text();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            // $("#validation-summary").show();
                            $('.errorCategory').text(data.message);
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    $('.form-checkbox').bootstrapSwitch();
                    $("#SubCategoryId").select2({ theme: "classic" });
                    $(".custom-file-input").on("change", function () {
                        var fileName = $(this).val().split("\\").pop();
                        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
                    });

                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            $("#modal-delete").on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        function initlizeForm() {
            formUserRegister = new Global.FormHelper($("#taxform"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: ''
                },
                //beforeSubmit: function () {
                //    debugger;
                //    alert('ca');
                //    var returnresult = true;
                //    //$('#formRegisterprovider #CountryCode').val($("#Mobile").intlTelInput("getSelectedCountryData").iso2);
                //    return returnresult;
                //}
            }, function onSuccess(result) {

                if (result) {
                    if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                        Global.Alert(result.errorMessage);
                    }
                    else {
                        window.location.href = result.redirectUrl;
                    }
                }
            });
        }

        $(document).off('change', '#CategoryId').on('change', '#CategoryId', function () {
            categoryId = $(this).val()
            $("#SubCategoryId").html('')
            if (categoryId != '' && categoryId != undefined) {
                $.ajax({
                    type: "Post",
                    url: "Taxs/GetSubcategory",
                    data: { CategoryId: categoryId },
                    success: function (response) {
                        if (response != '' && response != undefined) {
                            $.each(response, function (index, value) {
                                $("#SubCategoryId").append($("<option></option>").val(value.value).html(value.text));
                            })  
                        }
                    }
                });
            }
        });

        $this.init = function () {
            initializeGrid();
            initilizeModel();
            initlizeForm();
        }

    }
    $(function () {
        var self = new Cms();
        self.init();
    });
})(jQuery)