(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-Banner', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    /*{ "targets": [3], "sortable": true, "searchable": true },*/
                    {
                        "targets": [3], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {

                            return '<img src="/Uploads/' + row[3] + '" width="50px" height="50px">';
                        }
                    },
                   
                    {
                        "targets": [4], "sortable": true, "searchable": false, "data": "4",
                        "render": function (data, type, row, meta) {
                            var json = {
                                type: "checkbox",
                                class: "switchBox switch-small",
                                value: row[0],
                                'data-Active': "success",
                                'data-InActive': "danger"
                            };

                            if (JSON.parse(data.toLowerCase())) {
                                json.checked = true;
                            }
                            return $('<input/>', json).get(0).outerHTML;
                        }
                    },
                    { "targets": [5], "sortable": true, "searchable": false },
                    {
                        "targets": [6], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Banner/CreateEdit/" + row[0],
                                id: "editTypeModal",
                                title:"Edit",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-banner",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[7] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/Banner/Delete/" + row[0],
                                    id: "deleteType",
                                    title: "Delete",
                                    class: "btn btn-danger btn-sm",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-banner",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/Banner/View/" + row[0],
                                    id: "viewTypeModal",
                                    title: "View",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-view-banner",
                                    html: $("<i/>", {
                                        class: "fa fa-eye"
                                    }),
                                }).append("").get(0).outerHTML;
                                return actionLink;
                            }
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
                "sAjaxSource": "/admin/Banner/index",
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

                    $.get('/admin/Banner/ActiveBannerStatus', { id: this.value }, function (result) {
                        if (!result.isSuccess) {
                            $(switchElement).bootstrapSwitch('toggleState', true);
                        }
                        else {
                            Global.ShowMessage("Status Updated Successfully.", Global.MessageType.Success);
                        }
                    });
                });
        }
        function initilizeModel() {
            $("#modal-add-edit-banner").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-banner .modal-content').load(recipient1, function () {
                    formAddEditBrands = new Global.FormHelperWithFiles($("#modal-add-edit-banner").find("#frm-add-edit-banner form"), {
                        updateTargetId: "validation-summary", beforeSubmit: function () {

                            if (($('#Id').val() == null || $('#Id').val() == '' || $('#Id').val() == undefined || $('#Id').val() == 0) && ($('#yourElem').val() == null || $('#yourElem').val() == '' || $('#yourElem').val() == undefined)) {
                                $('#lblError').text("Please select image")
                                return false;
                            }
                            else {
                                $('#lblError').text("")
                                return true;
                            }
                        }
                        }, function (data) {

                        //console.log(data.isSuccess);
                        if (data.isSuccess == true) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            $('.titleExist').text(data.message);
                            $('.titleExist').addClass('text-danger')
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
            $("#modal-delete-banner").on('hidden.bs.modal', function (e) {
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