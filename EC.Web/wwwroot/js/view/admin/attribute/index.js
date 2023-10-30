(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-attribute', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": true },
                    { "targets": [4], "sortable": true, "searchable": true },
                    {
                        "targets": [5], "sortable": true, "searchable": false, "data": "5",
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
                    {
                        "targets": [6], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/attribute/createedit/" + row[0],
                                id: "editTypeModal",
                                class: "btn btn-primary btn-sm",
                                title: "Edit",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-attribute",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[7] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/attribute/delete/" + row[0],
                                    id: "deleteType",
                                    class: "btn btn-danger btn-sm",
                                    title: "Delete",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-attribute",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/attribute/view/" + row[0],
                                    id: "viewTypeModal",
                                    class: "btn btn-primary btn-sm",
                                    title: "View",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-view-attribute",
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
                "sAjaxSource": "/admin/attribute/index",
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
                    "searchPlaceholder": "Search by attribute title"
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

                    $.get('/admin/attribute/activeattributestatus', { id: this.value }, function (result) {
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

        function addMultipleValueRow() {
            var value_row = 0;
            $('#addRow').click(function () {
                html = '<tr id="valueBox-' + value_row + '" class="valueBox">';
                html += ' <td class="text-left"><input  type="text" onkeypress="clearerrormessage(this)"  id="titlevalueBox-' + value_row + '" name="TitleOrderValue"  maxlength="15" placeholder="Maximum 15 Number Value Title" class="form-control"   data-val="true" data-val-required="Please Fill Value Title" aria-describedby="Title-error"/><span  data-valmsg-for="titlevalueBox-' + value_row + '"  data-valmsg-replace="true" class="field-validation-error"> </span></td>';
                html += '  <td class="text-right"><input  type="number" onkeyup="clearerrormessage(this)"  id="titlesortvalueBox-' + value_row + '" maxlength="4"  min="1" max="9999"  name="TitleOrderValue"  value="1" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');"   data-val="true" data-val-required="Please Fill Sort" aria-describedby="Title-error" placeholder="Maximum 4 number Sort Order" class="form-control sort_order_number" /><span  data-valmsg-for="titlesortvalueBox-' + value_row + '" data-valmsg-replace="true" class="field-validation-error"> </span></td>';
                html += '  <td class="text-left"><button type="button" onclick="$(\'#valueBox-' + value_row + ', .tooltip\').remove();" data-toggle="tooltip" title="Remove" class="btn btn-danger"><i class="fa fa-minus-circle"></i></button></td>';
                html += '</tr>';
                $('table#optionValuesList tbody').append(html);
                value_row++;
            });

            $('#btn-submit').off().click(function () {
                if ($("#frm-add-edit-attribute form").valid()) {
                    var TitleOrderValue = $('input[name="TitleOrderValue"]').map(function () {
                        return this;
                    }).get();
                    for (var i = 0; i < TitleOrderValue.length; i++) {
                        if (TitleOrderValue[i].value == "") {
                            TitleOrderValue[i].class = "form-control input-validation-error";
                            $(`#` + TitleOrderValue[i].id).addClass("form-control input-validation-error");
                            $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-hidden", false);
                            $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-invalid", true);
                            $(`#` + TitleOrderValue[i].id).siblings("span").html(TitleOrderValue[i].getAttribute("data-val-required"));
                            return false;
                        }
                        else {
                            if (TitleOrderValue[i].type == "number") {
                                if (parseFloat(TitleOrderValue[i].value) <= 0 || parseFloat(TitleOrderValue[i].value) >= 9999) {
                                    TitleOrderValue[i].class = "form-control input-validation-error";
                                    $(`#` + TitleOrderValue[i].id).addClass("form-control input-validation-error");
                                    $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-hidden", false);
                                    $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-invalid", true);
                                    $(`#` + TitleOrderValue[i].id).siblings("span").html('Enter the valid sort order value (less than 9999.)');
                                    return false;
                                }
                            }

                            TitleOrderValue[i].class = "form-control ";
                            $(`#` + TitleOrderValue[i].id).addClass("form-control ");
                            $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-hidden", true);
                            $(`#` + TitleOrderValue[i].id).siblings("span").attr("aria-invalid", false);
                            $(`#` + TitleOrderValue[i].id).siblings("span").html('');
                        }
                    }
                   
                    $.ajax({
                        url: '/admin/attribute/CreateEdit',
                        method: "POST",
                        data: $('form').serialize(),
                        type: 'json',
                        success: function (data) {
                            if (data.isSuccess == true) {
                                $("#validation-summary").html("");
                                $("#validation-summary").hide();
                                window.location.href = data.redirectUrl;
                            }
                            else {
                                if ($("#Id").val() != null && $("#Id").val() != undefined && $("#Id").val() != "" && $("#Id").val() != "0") {
                                    Global.ShowMessage(data.message, Global.MessageType.Error);
                                }
                                else {
                                    $('.titleExist').text(data.message);
                                    $('.titleExist').addClass('text-danger')
                                }
                                // $("#validation-summary").show();
                                $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                            }
                        }
                    });
                }

            });
        }

        function initilizeModel() {
            $("#modal-add-edit-attribute").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-attribute .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelper($("#modal-add-edit-attribute").find("#frm-add-edit-attribute form"), { updateTargetId: "validation-summary" }, function (data) {

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
                    addMultipleValueRow();

                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            $("#modal-delete-attribute").on('hidden.bs.modal', function (e) {
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