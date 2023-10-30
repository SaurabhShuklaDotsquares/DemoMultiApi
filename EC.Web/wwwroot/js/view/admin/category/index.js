(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-category', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": false, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": false },
                    { "targets": [4], "sortable": true, "searchable": false },
                    {
                        "targets": [5], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {
                            if (row[5] != null) {

                                return '<img src="/Uploads/' + row[5] + '" width="50px" height="50px">';
                            } else
                            {
                                return '<img src="/Uploads/image.jpg" width="50px" height="50px">';
                            }
                        }
                    },
                    { "targets": [6], "sortable": true, "searchable": false },
                    {
                        "targets": [7], "sortable": false, "searchable": false, "data": "7",
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
                    { "targets": [8], "sortable": false, "searchable": false },
                    {
                        "targets": [9], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/category/createedit/" + row[0],
                                id: "editTypeModal",
                                title:"Edit",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-category",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[9] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/category/delete/" + row[0],
                                    id: "deleteType",
                                    title: "Delete",
                                    class: "btn btn-danger btn-sm",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-category",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/category/view/" + row[0],
                                    id: "viewTypeModal",
                                    title: "View",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-view-category",
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
                "sAjaxSource": "/admin/category/index",
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
                    "searchPlaceholder": "Search by Title"
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

                    $.get('/admin/category/activecategorystatus', { id: this.value }, function (result) {
                        if (!result.isSuccess) {
                            $(switchElement).bootstrapSwitch('toggleState', true);
                            Global.ShowMessage(result.data, Global.MessageType.Error);
                        }
                        else {
                            Global.ShowMessage("Status Updated Successfully.", Global.MessageType.Success);
                        }
                    });
                });
        }
        function initilizeModel() {
            $("#modal-add-edit-category").on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-category .modal-content').load(recipient1, function () {
                   
                    $('.form-checkbox').bootstrapSwitch();  
                    formAddEditCategory = new Global.FormHelperWithFiles($("#modal-add-edit-category").find("#frm-add-edit-category form"), {
                        updateTargetId: "validation-summary", beforeSubmit: function () {
                            debugger;
                            if (($('#Id').val() == null || $('#Id').val() == '' || $('#Id').val() == undefined || $('#Id').val() == 0) && ($('#yourElem').val() == null || $('#yourElem').val() == '' || $('#yourElem').val() == undefined)) {
                                $('#lblError').text("Please select image")
                                return false;
                            }
                            else {
                                $('#lblError').text("")
                                return true;
                            }
                        }
                    }, function(data) {

                        //console.log(data.isSuccess);
                        if (data.isSuccess == true) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            // $("#validation-summary").show();
                            $('.titleExist').text(data.message);
                            $('.titleExist').addClass('text-danger')
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    //$('.form-checkbox').bootstrapSwitch();
                    $(".custom-file-input").on("change", function () {
                        var fileName = $(this).val().split("\\").pop();
                        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
                    });

                });
            }).on('hidden.bs.modal', function (e) {
                $("#modal-add-edit-category").find(".modal-content").html("");
                $(this).removeData('bs.modal');
            });

            $("#modal-delete-category").on('hidden.bs.modal', function (e) {
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