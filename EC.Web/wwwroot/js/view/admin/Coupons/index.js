(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            /*ckEditor();*/
            var gridCms = new Global.GridHelper('#grid-Coupons', {
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
                    { "targets": [7], "sortable": true, "searchable": false },
                    {
                        "targets": [8], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Coupons/Create/" + row[0],
                                id: "addModel",
                                title: "Edit",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#modal-add-Coupons",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[0] != 0) {
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Coupons/delete/" + row[0],
                                    id: "deleteType",
                                    title: "Delete",
                                    class: "btn btn-danger btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-Coupons",
                                    html: $("<i/>", {
                                        class: "fa fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;"
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Coupons/View/" + row[0],
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
                "sAjaxSource": "/admin/Coupons/Index",
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
                    "searchPlaceholder": "Search by Code"
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
                    $.get(domain + '/admin/Coupons/ActiveCouponStatus', { id: this.value }, function (result) {
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
            $("#modal-add-Coupons").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-Coupons .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelperWithFiles($("#modal-add-Coupons").find("#frm-add-edit-Tax form"), { updateTargetId: "validation-summary" }, function (data) {

                        //console.log(data.isSuccess);
                        if (data.isSuccess == true) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            // $("#validation-summary").show();
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    $('.form-checkbox').bootstrapSwitch();
                    $(".custom-file-input").on("change", function () {
                        var fileName = $(this).val().split("\\").pop();
                        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
                    });

                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            $("#modal-delete-Coupons").on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initializeGrid();
            initilizeModel();
        }

    }
    $(function () {
        var self = new Cms();
        self.init();
    });
})(jQuery)